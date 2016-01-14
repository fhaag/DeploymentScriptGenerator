/*
This source file is a part of DeploymentScriptGenerator.

Copyright (c) 2016 Florian Haag

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

using Deployment.CodePlex.Web;

namespace Deployment.CodePlex
{
	public static class Worker
	{
		public sealed class ReleaseFileInfo
		{
			public ReleaseFileInfo(ReleaseFile rf, bool stableDefault, bool prereleaseDefault)
			{
				if (rf == null) {
					throw new ArgumentNullException("rf");
				}
				
				this.rf = rf;
				this.stableDefault = stableDefault;
				this.prereleaseDefault = prereleaseDefault;
			}
			
			private readonly ReleaseFile rf;
			
			public ReleaseFile ReleaseFile {
				get {
					return rf;
				}
			}
			
			private readonly bool stableDefault;
			
			public bool StableDefault {
				get {
					return stableDefault;
				}
			}
			
			private readonly bool prereleaseDefault;
			
			public bool PrereleaseDefault {
				get {
					return prereleaseDefault;
				}
			}
		}
		
		private static Regex versionStructure = new Regex(@"^([0-9]+(?:\.[0-9]+)*)(?:-([^\-\.]+))?$");
		
		public static void Publish(string projectInfoFilename, string releaseBasePath, string releaseVersion, string userFriendlyDate, string isoDate, string user, string pw)
		{
			if (projectInfoFilename == null) {
				throw new ArgumentNullException("projectInfoFilename");
			}
			if (releaseVersion == null) {
				throw new ArgumentNullException("releaseVersion");
			}
			if (userFriendlyDate == null) {
				throw new ArgumentNullException("userFriendlyDate");
			}
			if (isoDate == null) {
				throw new ArgumentNullException("isoDate");
			}
			if (user == null) {
				throw new ArgumentNullException("user");
			}
			if (pw == null) {
				throw new ArgumentNullException("pw");
			}
			
			var nsMap = new XmlNamespaceManager(new NameTable());
			nsMap.AddNamespace("local", "http://local/");
			
			var doc = new XmlDocument();
			doc.Load(projectInfoFilename);
			string projectName;
			var node = doc.DocumentElement.SelectSingleNode("/local:project/local:id[@service = 'CodePlex']/text()", nsMap);
			if (node != null) {
				projectName = node.Value;
			} else {
				projectName = doc.DocumentElement.SelectSingleNode("/local:project/local:id/text()", nsMap).Value;
			}
			
			var version = versionStructure.Match(releaseVersion);
			if (!version.Success) {
				throw new ArgumentException(string.Format(System.Globalization.CultureInfo.InvariantCulture,
				                                          "The release version \"{0}\" does not match the format requirements.",
				                                          releaseVersion));
			}
			
			bool isPrerelease = (version.Groups.Count > 2) && !string.IsNullOrWhiteSpace(version.Groups[2].Value);
			string releaseName = CreateReleaseName(version, isPrerelease);
			string status = DetermineStatus(version, isPrerelease);
			string releaseDesc = CompileReleaseNotes(doc, nsMap);
			
			var files = ExtractReleaseFiles(doc, nsMap, releaseBasePath, releaseVersion, userFriendlyDate).ToArray();
			var defaultFile = files.FirstOrDefault(rfi => isPrerelease ? rfi.PrereleaseDefault : rfi.StableDefault);
			
			using (var svc = new ReleaseService()) {
				int releaseId = svc.CreateARelease(projectName,
				                                   releaseName,
				                                   releaseDesc,
				                                   isoDate,
				                                   status,
				                                   true,
				                                   !isPrerelease,
				                                   user,
				                                   pw);
				
				svc.UploadTheReleaseFiles(projectName,
				                          releaseName,
				                          files.Select(rfi => rfi.ReleaseFile).ToArray(),
				                          defaultFile == null ? null : defaultFile.ReleaseFile.FileName,
				                          user,
				                          pw);
			}
		}
		
		private static string CreateReleaseName(Match analyzedVersion, bool isPrerelease)
		{
			var result = new StringBuilder(analyzedVersion.Groups[1].Value);
			if (isPrerelease) {
				result.Append(" ");
				string prereleaseTag = analyzedVersion.Groups[2].Value;
				result.Append(char.ToUpperInvariant(prereleaseTag[0]));
				result.Append(prereleaseTag.Substring(1));
			}
			return result.ToString();
		}
		
		private static string DetermineStatus(Match analyzedVersion, bool isPrerelease)
		{
			if (isPrerelease) {
				string prereleaseTag = analyzedVersion.Groups[2].Value.ToLowerInvariant();
				if (prereleaseTag.StartsWith("alpha")) {
					return "Alpha";
				} else if (prereleaseTag.StartsWith("beta") || prereleaseTag.StartsWith("rc")) {
					return "Beta";
				} else {
					return "Planning";
				}
			} else {
				return "Stable";
			}
		}
		
		private static IEnumerable<ReleaseFileInfo> ExtractReleaseFiles(XmlDocument configFile, XmlNamespaceManager nsMap, string releaseBasePath, string releaseVersion, string userFriendlyDate)
		{
			foreach (var fileNode in configFile.SelectNodes("/local:project/local:downloads/local:file", nsMap).Cast<XmlElement>()) {
				var rf = new ReleaseFile();
				rf.Name = ReplaceSymbols(fileNode.GetAttribute("title"), releaseVersion, userFriendlyDate);
				
				rf.FileName = ReplaceSymbols(fileNode.InnerText, releaseVersion, userFriendlyDate);
				
				using (var f = File.OpenRead(Path.Combine(releaseBasePath, rf.FileName))) {
					using (var ms = new MemoryStream()) {
						f.CopyTo(ms);
						rf.FileData = ms.ToArray();
					}
				}
				
				rf.FileType = GetFileType(fileNode.GetAttribute("type"));
				yield return new ReleaseFileInfo(rf,
				                                 fileNode.GetAttribute("stableDefault") == "true",
				                                 fileNode.GetAttribute("prereleaseDefault") == "true");
			}
		}
		
		private static string ReplaceSymbols(string pattern, string version, string date)
		{
			return pattern.Replace("%VERSION%", version).Replace("%DATE%", date);
		}
		
		private static string GetFileType(string configuredType)
		{
			if (configuredType != null) {
				switch (configuredType) {
					case "bin":
						return "RuntimeBinary";
					case "chm":
						return "Documentation";
				}
			}
			return "SourceCode";
		}
		
		private static string CompileReleaseNotes(XmlDocument configFile, XmlNamespaceManager nsMap)
		{
			var result = new StringBuilder();
			
			foreach (var changeNode in configFile.SelectNodes("/local:project/local:history/local:current/local:change", nsMap).Cast<XmlElement>()) {
				if (result.Length > 0) {
					result.Append("; ");
				}
				result.Append(changeNode.InnerText);
			}
			
			return result.ToString();
		}
	}
}
