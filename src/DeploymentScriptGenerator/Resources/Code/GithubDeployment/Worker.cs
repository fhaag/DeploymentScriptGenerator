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

using Octokit;

namespace Deployment.Github
{
	public static class Worker
	{
		public sealed class ReleaseFileInfo
		{
			public ReleaseFileInfo(string destName, string srcPath, string mimeType)
			{
				if (destName == null) {
					throw new ArgumentNullException("destName");
				}
				if (srcPath == null) {
					throw new ArgumentNullException("srcPath");
				}
				if (mimeType == null) {
					throw new ArgumentNullException("mimeType");
				}
				
				this.destName = destName;
				this.mimeType = mimeType;
				
				using (var f = File.OpenRead(srcPath)) {
					f.CopyTo(this.data);
				}
			}
			
			private readonly string destName;
			
			public string DestName {
				get {
					return destName;
				}
			}
			
			private readonly MemoryStream data = new MemoryStream();
			
			public MemoryStream Data {
				get {
					return data;
				}
			}
			
			private readonly string mimeType;
			
			public string MimeType {
				get {
					return mimeType;
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
			var node = doc.DocumentElement.SelectSingleNode("/local:project/local:id[@service = 'Github']/text()", nsMap);
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
			//string status = DetermineStatus(version, isPrerelease);
			string releaseDesc = CompileReleaseNotes(doc, nsMap);
			
			var github = new GitHubClient(new ProductHeaderValue(typeof(Worker).Assembly.FullName));
			github.Credentials = new Credentials(user, pw);
			var releaseClient = github.Release;
			
			var releaseData = new NewRelease(releaseName);
			releaseData.Body = releaseDesc;
			releaseData.Prerelease = isPrerelease;
			releaseData.Name = releaseName;
			
			var releaseTask = github.Release.Create(user, projectName, releaseData);
			releaseTask.RunSynchronously();
			var release = releaseTask.Result;
			
			var files = ExtractReleaseFiles(doc, nsMap, releaseBasePath, releaseVersion, userFriendlyDate).ToArray();
			foreach (ReleaseFileInfo f in files) {
				var asset = new ReleaseAssetUpload(f.DestName, f.MimeType, f.Data, null);
				var uploadTask = github.Release.UploadAsset(release, asset);
				uploadTask.RunSynchronously();
			}
		}
		
		private static string CreateReleaseName(Match analyzedVersion, bool isPrerelease)
		{
			var result = new StringBuilder("v" + analyzedVersion.Groups[1].Value);
			if (isPrerelease) {
				result.Append("-" + analyzedVersion.Groups[2].Value);
			}
			return result.ToString();
		}
		
		private static IEnumerable<ReleaseFileInfo> ExtractReleaseFiles(XmlDocument configFile, XmlNamespaceManager nsMap, string releaseBasePath, string releaseVersion, string userFriendlyDate)
		{
			foreach (var fileNode in configFile.SelectNodes("/local:project/local:downloads/local:file", nsMap).Cast<XmlElement>()) {
				string fn = ReplaceSymbols(fileNode.InnerText, releaseVersion, userFriendlyDate);
				yield return new ReleaseFileInfo(fn,
				                                 Path.Combine(releaseBasePath, fn),
				                                 GetMimeType(fn));
			}
		}
		
		private static string ReplaceSymbols(string pattern, string version, string date)
		{
			return pattern.Replace("%VERSION%", version).Replace("%DATE%", date);
		}
		
		private static string GetMimeType(string fn)
		{
			if (fn.EndsWith(".zip")) {
				return "application/zip";
			} else if (fn.EndsWith(".tar.gz")) {
				return "application/gzip";
			} else {
				throw new InvalidOperationException("Unsupported file type: " + fn);
			}
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
