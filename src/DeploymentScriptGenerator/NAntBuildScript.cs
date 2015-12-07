﻿/*
This source file is a part of DeploymentScriptGenerator.

Copyright (c) 2015 Florian Haag

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
using System.ComponentModel;
using System.IO;
using System.Xml;

using Deployment.ScriptGenerator.Resources;

namespace Deployment.ScriptGenerator
{
	internal static class NAntBuildScript
	{
		public static void Write(GeneralSettings settings)
		{
			if (settings == null) {
				throw new ArgumentNullException("settings");
			}
			
			XmlWriterSettings writerSettings = new XmlWriterSettings();
			writerSettings.Indent = true;
			using (var w = XmlWriter.Create(Path.Combine(settings.BasePath, settings.Options.BuildFile), writerSettings)) {
				Write(settings, w);
			}
		}
		
		private static void Write(GeneralSettings settings, XmlWriter w)
		{
			w.WriteStartDocument();
			w.WriteComment("\n" + ResourceLoader.LineWrappedLicenseText + "\n");
			
			w.WriteStartElement("project");
			w.WriteAttributeString("name", "Release Script");
			w.WriteAttributeString("default", "help");
			w.WriteAttributeString("basedir", ".");
			
			{
				w.WriteStartElement("property");
				w.WriteAttributeString("name", "release.version");
				w.WriteAttributeString("value", "0.0.0");
				w.WriteAttributeString("overwrite", "false");
				w.WriteEndElement();
			}
			
			// project ID
			
			{
				w.WriteStartElement("xmlpeek");
				w.WriteAttributeString("file", settings.Options.PublicDirectory + "/" + settings.Options.ProjectInfoFile);
				w.WriteAttributeString("xpath", "/local:project/local:id");
				w.WriteAttributeString("property", "project.id");
				
				w.WriteStartElement("namespaces");
				WriteXmlNamespace(w, "local", "http://local/");
				w.WriteEndElement();
				
				w.WriteEndElement();
			}
			
			if (settings.SupportCodePlex) {
				DeclareSiteSpecificId(settings, w, "CodePlex");
			}
			if (settings.SupportGithub) {
				DeclareSiteSpecificId(settings, w, "Github");
			}
			if (settings.SupportSourceForge) {
				DeclareSiteSpecificId(settings, w, "SourceForge");
			}
			
			// date and time
			
			{
				w.WriteStartElement("tstamp");
				w.WriteAttributeString("property", "internal.userFriendlyDate");
				w.WriteAttributeString("pattern", "yyyy-MM-dd");
				w.WriteEndElement();
				
				w.WriteStartElement("tstamp");
				w.WriteAttributeString("property", "internal.isoDate");
				w.WriteAttributeString("pattern", "O");
				w.WriteEndElement();
			}
			
			// version type
			
			{
				w.WriteStartElement("property");
				w.WriteAttributeString("name", "internal.prerelease");
				w.WriteAttributeString("value", "no");
				w.WriteEndElement();
				
				w.WriteStartElement("regex");
				w.WriteAttributeString("input", "${release.version}");
				w.WriteAttributeString("pattern", @"^[0-9]+(\.[0-9]+)*(?'releasetag'(-[^\-\.]+)?)$");
				w.WriteEndElement();
				
				w.WriteStartElement("property");
				w.WriteAttributeString("name", "internal.prerelease");
				w.WriteAttributeString("value", "yes");
				w.WriteAttributeString("if", "${string::get-length(releasetag)>0}");
				w.WriteEndElement();
				
				w.WriteStartElement("property");
				w.WriteAttributeString("name", "internal.ReleaseState");
				w.WriteAttributeString("value", "prerelease");
				w.WriteAttributeString("if", "${internal.prerelease=='yes'}");
				w.WriteEndElement();
				
				w.WriteStartElement("property");
				w.WriteAttributeString("name", "internal.ReleaseState");
				w.WriteAttributeString("value", "stable");
				w.WriteAttributeString("if", "${internal.prerelease=='no'}");
				w.WriteEndElement();
			}
			
			// script help output
			
			WriteHeadlineComment(w, "Documentation");
			
			{
				StartTarget(w, "help");
				
				w.WriteElementString("echo", "Deployment Script");
				w.WriteElementString("echo", "=================");
				w.WriteElementString("echo", "This NAnt build file offers the following targets:");
				w.WriteElementString("echo", "== help ==");
				w.WriteElementString("echo", "Outputs this description.");
				w.WriteElementString("echo", "== release ==");
				w.WriteElementString("echo", "Assembles all release files. The following preconditions must be met:");
				// TODO: list release preconditions
				w.WriteElementString("echo", "== publish ==");
				w.WriteElementString("echo", "Publishes the files. The release target must have been run before.");
				w.WriteElementString("echo", "The following preconditions must be met:");
				// TODO: list publish preconditions
				
				w.WriteEndElement();
			}
			
			WriteHeadlineComment(w, "Prepare Release Files");
			
			WritePatternSet(w, "srcfiles",
			                new[] {
			                	"**/*.sln",
			                	"**/*.csproj",
			                	"**/*.cs",
			                	"**/*.xaml",
			                	"**/*.ico",
			                	"**/*.png",
			                	"**/readme.txt",
			                	"**/*.LICENSE",
			                	"**/*.snk",
			                	"**/*.resx",
			                	"**/*.config"
			                },
			                new[] {
			                	"**/bin/**",
			                	"**/obj/**"
			                });
			WritePatternSet(w, "binfiles",
			                new[] {
			                	"*.dll",
			                	"*.xml"
			                },
			                new[] {
			                	"*.CodeAnalysisLog.xml"
			                });
			
			if (settings.RequireXsltExtensions) {
				StartTarget(w, "build-xslt-extensions");
				
				w.WriteStartElement("mkdir");
				w.WriteAttributeString("dir", "bin/build/xslt");
				w.WriteEndElement();
				
				w.WriteStartElement("csc");
				w.WriteAttributeString("output", "bin/build/xslt/XsltExtensions.dll");
				w.WriteAttributeString("target", "library");
				w.WriteStartElement("sources");
				w.WriteStartElement("include");
				w.WriteAttributeString("name", settings.Options.ToolDirectory + "/XsltStringExtensions.cs"); // TODO: include this (and Json file) only when required
				w.WriteEndElement();
				w.WriteEndElement();
				w.WriteEndElement();
				
				w.WriteEndElement();
			}
			
			// readme
			
			{
				StartTarget(w, "prepare-readme", "build-xslt-extensions");
				
				w.WriteStartElement("mkdir");
				w.WriteAttributeString("dir", settings.Options.TemporaryDirectory + "/readme");
				w.WriteEndElement();
				
				CopyFile(w, "COPYING", settings.Options.TemporaryDirectory + "/readme/COPYING");
				
				{
					w.WriteStartElement("copy");
					w.WriteAttributeString("file", "readme.txt");
					w.WriteAttributeString("tofile", settings.Options.TemporaryDirectory + "/readme/readme.txt");
					w.WriteStartElement("filterchain");
					w.WriteStartElement("replacetokens");
					w.WriteAttributeString("begintoken", "%");
					w.WriteAttributeString("endtoken", "%");
					
					w.WriteStartElement("token");
					w.WriteAttributeString("key", "VERSION");
					w.WriteAttributeString("value", "${release.version}");
					w.WriteEndElement();
					
					w.WriteStartElement("token");
					w.WriteAttributeString("key", "DATE");
					w.WriteAttributeString("value", "${internal.userFriendlyDate}");
					w.WriteEndElement();
					
					w.WriteEndElement();
					w.WriteEndElement();
					w.WriteEndElement();
				}
				
				{
					w.WriteStartElement("style");
					w.WriteAttributeString("style", settings.Options.ToolDirectory + "/CreateTextChangeLog.xsl");
					w.WriteAttributeString("in", settings.Options.PublicDirectory + "/" + settings.Options.ProjectInfoFile);
					w.WriteAttributeString("out", settings.Options.TemporaryDirectory + "/readme/changelog.txt");
					
					w.WriteStartElement("parameters");
					WriteXsltParameter(w, "ReleaseVersion", "${release.version}");
					WriteXsltParameter(w, "ReleaseDate", "${internal.userFriendlyDate}");
					w.WriteEndElement();
					
					w.WriteStartElement("extensionobjects");
					WriteXsltExtensionObject(w, "urn:str", "XsltExtensions.XsltStringExtensions", "bin/build/xslt/XsltExtensions.dll");
					w.WriteEndElement();
					
					w.WriteEndElement();
				}
				
				if (settings.Options.WebChangeLog) {
					w.WriteStartElement("style");
					w.WriteAttributeString("style", settings.Options.ToolDirectory + "/CreateHtmlChangeLog.xsl");
					w.WriteAttributeString("in", settings.Options.PublicDirectory + "/" + settings.Options.ProjectInfoFile);
					w.WriteAttributeString("out", settings.Options.ReleaseDirectory + "/changelog.html");
					
					w.WriteStartElement("parameters");
					WriteXsltParameter(w, "ReleaseVersion", "${release.version}");
					WriteXsltParameter(w, "ReleaseDate", "${internal.userFriendlyDate}");
					w.WriteEndElement();
					
					w.WriteEndElement();
				}
				
				w.WriteEndElement();
			}
			
			// clean-up
			
			{
				StartTarget(w, "clean");
				
				w.WriteStartElement("delete");
				w.WriteAttributeString("dir", settings.Options.ReleaseDirectory);
				w.WriteAttributeString("if", "${directory::exists('" + settings.Options.ReleaseDirectory + "')}");
				w.WriteEndElement();
				
				w.WriteStartElement("delete");
				w.WriteAttributeString("dir", settings.Options.TemporaryDirectory);
				w.WriteAttributeString("if", "${directory::exists('" + settings.Options.TemporaryDirectory + "')}");
				w.WriteEndElement();
				
				w.WriteEndElement();
			}
			
			{
				StartTarget(w, "prepare-release-state");
				
				w.WriteStartElement("xmlpeek");
				w.WriteAttributeString("file", settings.Options.PublicDirectory + "/" + settings.Options.ProjectInfoFile);
				w.WriteAttributeString("xpath", "/local:project/local:history/local:current/local:change");
				w.WriteAttributeString("property", "tmp.changelogReady");
				w.WriteAttributeString("failonerror", "false");
				w.WriteStartElement("namespaces");
				WriteXmlNamespace(w, "local", "http://local/");
				w.WriteEndElement();
				w.WriteEndElement();
				
				w.WriteStartElement("fail");
				w.WriteAttributeString("message", "No changes indicated in project info file.");
				w.WriteAttributeString("if", "${not(property::exists('tmp.changelogReady')) or (tmp.changelogReady == '')}");
				w.WriteEndElement();
				
				w.WriteEndElement();
			}
			
			// pack release files
			
			{
				StartTarget(w, "release",
				            "prepare-release-state", "clean", "prepare-readme");
				
				w.WriteStartElement("mkdir");
				w.WriteAttributeString("dir", settings.Options.ReleaseDirectory);
				w.WriteEndElement();
				
				if (settings.ProcessExampleProjects) {
					string examplePjPath = settings.Options.TemporaryDirectory + "/exampleprojects";
					
					w.WriteStartElement("mkdir");
					w.WriteAttributeString("dir", examplePjPath);
					w.WriteEndElement();
					
					w.WriteStartElement("copy");
					w.WriteAttributeString("todir", examplePjPath);
					w.WriteStartElement("fileset");
					w.WriteAttributeString("basedir", "src/Samples"); // TODO: make flexible?
					w.WriteStartElement("include");
					w.WriteAttributeString("name", "*/*.csproj");
					w.WriteEndElement();
					w.WriteEndElement();
					w.WriteEndElement();
					
					w.WriteStartElement("foreach");
					w.WriteAttributeString("item", "Folder");
					w.WriteAttributeString("in", examplePjPath);
					w.WriteAttributeString("property", "foldername");
					
					CopyFile(w, "src/Version.cs", "${foldername}/Properties/Version.cs");
					
					w.WriteStartElement("foreach");
					w.WriteAttributeString("item", "File");
					w.WriteAttributeString("in", "${foldername}");
					w.WriteAttributeString("property", "filename");
					
					w.WriteStartElement("in");
					w.WriteStartElement("items");
					w.WriteStartElement("include");
					w.WriteAttributeString("name", "*.csproj");
					w.WriteEndElement();
					w.WriteEndElement();
					w.WriteEndElement();
					
					w.WriteStartElement("do");
					w.WriteStartElement("style");
					w.WriteAttributeString("style", settings.Options.ToolDirectory + "/ExampleProjects.xsl");
					w.WriteAttributeString("in", "${filename}");
					w.WriteAttributeString("out", "${filename}.tmp");
					w.WriteEndElement();
					w.WriteStartElement("delete");
					w.WriteAttributeString("file", "${filename}");
					w.WriteEndElement();
					w.WriteStartElement("move");
					w.WriteAttributeString("file", "${filename}.tmp");
					w.WriteAttributeString("tofile", "${filename}");
					w.WriteEndElement();
					w.WriteEndElement();
					
					w.WriteEndElement();
					
					w.WriteEndElement();
				}
				
				PackDownloadFile(settings, w, DownloadPackage.Binaries, settings.DownloadFormat);
				PackDownloadFile(settings, w, DownloadPackage.Sources, settings.DownloadFormat);
				if (settings.Options.ReleaseApiDoc) {
					PackDownloadFile(settings, w, DownloadPackage.ApiDocumentation, settings.DownloadFormat);
				}
				
				CopyFile(w, settings.Options.TemporaryDirectory + "/readme/readme.txt", settings.Options.ReleaseDirectory + "/readme.txt");
				CopyFile(w, settings.Options.TemporaryDirectory + "/readme/changelog.txt", settings.Options.ReleaseDirectory + "/changelog.txt");
				
				if (settings.Options.ReleaseOnNuGet) {
					w.WriteStartElement("copy");
					w.WriteAttributeString("todir", settings.Options.ReleaseDirectory);
					w.WriteAttributeString("flatten", "true");
					
					w.WriteStartElement("fileset");
					w.WriteStartElement("include");
					w.WriteAttributeString("name", settings.Options.PublicDirectory + "/*.nuspec");
					w.WriteEndElement();
					w.WriteEndElement();
					
					w.WriteStartElement("filterchain");
					w.WriteStartElement("replacetokens");
					w.WriteAttributeString("begintoken", "%");
					w.WriteAttributeString("endtoken", "%");
					w.WriteStartElement("token");
					w.WriteAttributeString("key", "VERSION");
					w.WriteAttributeString("value", "${release.version}");
					w.WriteEndElement();
					w.WriteEndElement();
					w.WriteEndElement();
					
					w.WriteEndElement();
					
					w.WriteStartElement("foreach");
					w.WriteAttributeString("item", "File");
					w.WriteAttributeString("property", "filename");
					
					w.WriteStartElement("in");
					w.WriteStartElement("items");
					w.WriteStartElement("include");
					w.WriteAttributeString("name", "release/*.nuspec");
					w.WriteEndElement();
					w.WriteEndElement();
					w.WriteEndElement();
					
					w.WriteStartElement("do");
					ExecuteProgram(w, "cmd", null,
					               "/c", "NuGet", "pack", "${filename}", "-OutputDirectory", settings.Options.ReleaseDirectory);
					w.WriteEndElement();
					
					w.WriteEndElement();
				}
				
				w.WriteStartElement("call");
				w.WriteAttributeString("target", "cleanup-after-release");
				w.WriteEndElement();
				
				w.WriteEndElement();
			}
			
			WriteHeadlineComment(w, "Publication");
			
			// TODO: load credentials
			
			{
				StartTarget(w, "publish");
				
				if (settings.Options.ReleaseOnGithub) {
					w.WriteStartElement("call");
					w.WriteAttributeString("target", "publish-Github-release");
					w.WriteEndElement();
				}
				
				if (settings.Options.ReleaseOnSourceForge) {
					w.WriteStartElement("call");
					w.WriteAttributeString("target", "publish-sourceforge-frs");
					w.WriteEndElement();
				}
				
				if (settings.Options.ReleaseOnCodePlex) {
					w.WriteStartElement("call");
					w.WriteAttributeString("target", "publish-codeplex-release");
					w.WriteEndElement();
				}
				
				if (settings.Options.ReleaseOnNuGet) {
					w.WriteStartElement("call");
					w.WriteAttributeString("target", "publish-NuGet");
					w.WriteEndElement();
				}
				
				if (settings.Options.WebChangeLog) {
					w.WriteStartElement("call");
					w.WriteAttributeString("target", "publish-web"); // TODO: exclude API doc for prereleases
					w.WriteEndElement();
				}
				
				w.WriteStartElement("call");
				w.WriteAttributeString("target", "advance-changelog");
				w.WriteEndElement();
				
				w.WriteEndElement();
			}
			
			if (settings.Options.ReleaseOnGithub || settings.Options.ReleaseOnCodePlex) {
				StartTarget(w, "build-deployment-libs");
				
				w.WriteStartElement("solution");
				w.WriteAttributeString("configuration", "Release");
				w.WriteAttributeString("solutionfile", settings.Options.ToolDirectory + "/DeploymentLibs/DeploymentLibs.sln");
				w.WriteAttributeString("outputdir", "bin/build/deployment");
				w.WriteEndElement();
				
				w.WriteEndElement();
			}
			
			if (settings.Options.ReleaseOnGithub) {
				StartTarget(w, "publish-Github-release",
				            "build-deployment-libs");
				
				w.WriteStartElement("xmlpeek");
				w.WriteAttributeString("file", settings.Options.KeyDirectory + "/apikeys.xml");
				w.WriteAttributeString("xpath", "/keys/key[@id = 'GithubPW']/@value");
				w.WriteAttributeString("property", "internal.GithubPW");
				w.WriteEndElement();
				
				w.WriteStartElement("script");
				w.WriteAttributeString("language", "c#");
				w.WriteStartElement("references");
				w.WriteStartElement("include");
				w.WriteAttributeString("name", "bin/build/deployment/GithubDeployment.dll");
				w.WriteEndElement();
				w.WriteEndElement();
				w.WriteStartElement("imports");
				w.WriteStartElement("import");
				w.WriteAttributeString("namespace", "System.IO");
				w.WriteEndElement();
				w.WriteEndElement();
				w.WriteStartElement("code");
				
				{
					var githubDeploymentCode = new System.Text.StringBuilder();
					githubDeploymentCode.AppendLine();
					githubDeploymentCode.AppendLine("public static void ScriptMain(Project project)");
					githubDeploymentCode.AppendLine("{");
					githubDeploymentCode.AppendLine("	Deployment.Github.Worker.Publish(Path.Combine(project.BaseDirectory, \"" + settings.Options.PublicDirectory + "\", \"" + settings.Options.ProjectInfoFile + "\"),");
					githubDeploymentCode.AppendLine("	                                 Path.Combine(project.BaseDirectory, \"" + settings.Options.ReleaseDirectory + "\"),");
					githubDeploymentCode.AppendLine("	                                 project.Properties[\"release.version\"],");
					githubDeploymentCode.AppendLine("	                                 project.Properties[\"internal.userFriendlyDate\"],");
					githubDeploymentCode.AppendLine("	                                 project.Properties[\"internal.isoDate\"],");
					githubDeploymentCode.AppendLine("	                                 project.Properties[\"internal.GithubUser\"],");
					githubDeploymentCode.AppendLine("	                                 project.Properties[\"internal.GithubPW\"]);");
					githubDeploymentCode.AppendLine("}");
					w.WriteCData(githubDeploymentCode.ToString());
				}
				
				w.WriteEndElement();
				w.WriteEndElement();
				
				w.WriteEndElement();
			}
			
			if (settings.Options.ReleaseOnSourceForge) {
				
			}
			
			if (settings.Options.ReleaseOnCodePlex) {
				
			}
			
			if (settings.Options.ReleaseOnNuGet) {
				StartTarget(w, "publish-NuGet");
				ExecuteProgram(w, "cmd", settings.Options.ReleaseDirectory,
				               "/c", "NuGet", "push", "*.nupkg");
				w.WriteEndElement();
			}
			
			if (settings.Options.WebChangeLog) {
				StartTarget(w, "publish-web");
				
				if (settings.Options.WebChangeLog) {
					// TODO: distinguish Github/SF website
					CopyFile(w, settings.Options.ReleaseDirectory + "/changelog.html", "${internal.WebPath}/");
				}
				
				if (settings.Options.WebApiDoc) {
					// TODO: web API doc
				}
				
				w.WriteEndElement();
			}
			
			// advance changelog
			
			{
				StartTarget(w, "advance-changelog", "build-xslt-extensions");
				
				string tempInfoFile = settings.Options.ReleaseDirectory + "/new_" + settings.Options.ProjectInfoFile;
				
				w.WriteStartElement("style");
				w.WriteAttributeString("style", settings.Options.ToolDirectory + "/AdvanceChangelog.xsl");
				w.WriteAttributeString("in", settings.Options.PublicDirectory + "/" + settings.Options.ProjectInfoFile);
				w.WriteAttributeString("out", tempInfoFile);
				w.WriteStartElement("parameters");
				WriteXsltParameter(w, "ReleaseDateTime", "${internal.isoDate}");
				WriteXsltParameter(w, "ReleaseVersion", "${release.version}");
				WriteXsltParameter(w, "ReleaseState", "${internal.ReleaseState}");
				WriteXsltParameter(w, "ReleaseDate", "${internal.userFriendlyDate}");
				w.WriteEndElement();
				w.WriteStartElement("extensionobjects");
				WriteXsltExtensionObject(w, "urn:str", "XsltExtensions.XsltStringExtensions", "bin/build/xslt/XsltExtensions.dll");
				w.WriteEndElement();
				w.WriteEndElement();
				
				w.WriteStartElement("copy");
				w.WriteAttributeString("file", tempInfoFile);
				w.WriteAttributeString("tofile", settings.Options.PublicDirectory + "/" + settings.Options.ProjectInfoFile);
				w.WriteAttributeString("overwrite", "true");
				w.WriteEndElement();
				
				w.WriteEndElement();
			}
			
			w.WriteEndElement(); // <project>
			
			w.WriteEndDocument();
		}
		
		private static void WriteHeadlineComment(XmlWriter w, string title)
		{
			w.WriteComment(" ================ " + title + " ================ ");
		}
		
		private static void DeclareSiteSpecificId(GeneralSettings settings, XmlWriter w, string site)
		{
			w.WriteStartElement("xmlpeek");
			w.WriteAttributeString("file", settings.Options.PublicDirectory + "/" + settings.Options.ProjectInfoFile);
			w.WriteAttributeString("xpath", "/local:project/local:id[@service = '" + site + "']");
			w.WriteAttributeString("property", "project." + site + "Id");
			w.WriteAttributeString("failonerror", "false");
			
			w.WriteStartElement("namespaces");
			WriteXmlNamespace(w, "local", "http://local/");
			w.WriteEndElement();
			
			w.WriteEndElement();
			
			// fallback to default ID
			
			w.WriteStartElement("if");
			w.WriteAttributeString("test", "${not(property::exists('project." + site + "Id'))}");
			
			w.WriteStartElement("property");
			w.WriteAttributeString("name", "project." + site + "Id");
			w.WriteAttributeString("value", "${project.id}");
			w.WriteEndElement();
			
			w.WriteEndElement();
		}
		
		private static void StartTarget(XmlWriter w, string name, params string[] dependsOn)
		{
			w.WriteStartElement("target");
			w.WriteAttributeString("name", name);
			
			if (dependsOn.Length > 0) {
				var dependsString = new System.Text.StringBuilder();
				foreach (string d in dependsOn) {
					if (dependsString.Length > 0) {
						dependsString.Append(",");
					}
					dependsString.Append(d);
				}
				w.WriteAttributeString("depends", dependsString.ToString());
			}
		}
		
		private static void WritePatternSet(XmlWriter w, string name, IEnumerable<string> include, IEnumerable<string> exclude)
		{
			w.WriteStartElement("patternset");
			w.WriteAttributeString("id", name);
			
			foreach (string p in include) {
				w.WriteStartElement("include");
				w.WriteAttributeString("name", p);
				w.WriteEndElement();
			}
			
			foreach (string p in exclude) {
				w.WriteStartElement("exclude");
				w.WriteAttributeString("name", p);
				w.WriteEndElement();
			}
			
			w.WriteEndElement();
		}
		
		private static void WriteXsltParameter(XmlWriter w, string name, string value)
		{
			w.WriteStartElement("parameter");
			w.WriteAttributeString("name", name);
			w.WriteAttributeString("value", value);
			w.WriteEndElement();
		}
		
		private static void WriteXsltExtensionObject(XmlWriter w, string namespaceUri, string type, string assembly)
		{
			w.WriteStartElement("extensionobject");
			w.WriteAttributeString("namespaceuri", namespaceUri);
			w.WriteAttributeString("typename", type);
			w.WriteAttributeString("assembly", assembly);
			w.WriteEndElement();
		}
		
		private static void WriteXmlNamespace(XmlWriter w, string prefix, string uri)
		{
			w.WriteStartElement("namespace");
			w.WriteAttributeString("prefix", prefix);
			w.WriteAttributeString("uri", uri);
			w.WriteEndElement();
		}
		
		private static void PackDownloadFile(GeneralSettings settings, XmlWriter w, DownloadPackage package, ArchiveType format)
		{
			switch (format) {
				case ArchiveType.Zip:
					w.WriteStartElement("zip");
					break;
				case ArchiveType.Tar:
					w.WriteStartElement("tar");
					w.WriteAttributeString("compression", "None");
					break;
				case ArchiveType.TarBz2:
					w.WriteStartElement("tar");
					w.WriteAttributeString("compression", "BZip2");
					break;
				case ArchiveType.TarGz:
					w.WriteStartElement("tar");
					w.WriteAttributeString("compression", "GZip");
					break;
				default:
					throw new InvalidEnumArgumentException("format", (int)format, typeof(ArchiveType));
			}
			// TODO: destination file name
			
			switch (package) {
				case DownloadPackage.Binaries:
					w.WriteStartElement("fileset");
					w.WriteAttributeString("basedir", "bin/Release");
					w.WriteAttributeString("prefix", "bin");
					w.WriteStartElement("patternset");
					w.WriteAttributeString("refid", "binfiles");
					w.WriteEndElement();
					w.WriteEndElement();
					
					if (settings.ProcessExampleProjects) {
						w.WriteStartElement("fileset");
						w.WriteAttributeString("basedir", "src/Samples");
						w.WriteStartElement("patternset");
						w.WriteAttributeString("refid", "srcfiles");
						w.WriteEndElement();
						w.WriteStartElement("exclude");
						w.WriteAttributeString("name", "**/*.csproj");
						w.WriteEndElement();
						w.WriteEndElement();
						
						w.WriteStartElement("fileset");
						w.WriteAttributeString("basedir", "src/Samples");
						w.WriteStartElement("include");
						w.WriteAttributeString("name", "*/bin/Release/*.exe");
						w.WriteEndElement();
						w.WriteStartElement("include");
						w.WriteAttributeString("name", "*/bin/Release/*.dll");
						w.WriteEndElement();
						w.WriteEndElement();
						
						w.WriteStartElement("fileset");
						w.WriteAttributeString("basedir", settings.Options.TemporaryDirectory + "/exampleprojects");
						w.WriteStartElement("include");
						w.WriteAttributeString("name", "*/*.csproj");
						w.WriteEndElement();
						w.WriteStartElement("include");
						w.WriteAttributeString("name", "**/*.cs");
						w.WriteEndElement();
						w.WriteEndElement();
					}
					
					w.WriteStartElement("fileset");
					w.WriteAttributeString("basedir", settings.Options.TemporaryDirectory + "/readme");
					w.WriteStartElement("include");
					w.WriteAttributeString("name", "*");
					w.WriteEndElement();
					w.WriteEndElement();
					break;
				case DownloadPackage.Sources:
					w.WriteStartElement("fileset");
					w.WriteAttributeString("basedir", "src");
					w.WriteAttributeString("prefix", "src");
					w.WriteStartElement("patternset");
					w.WriteAttributeString("refid", "srcfiles");
					w.WriteEndElement();
					w.WriteEndElement();
					
					w.WriteStartElement("fileset");
					w.WriteAttributeString("basedir", settings.Options.KeyDirectory);
					w.WriteAttributeString("prefix", settings.Options.KeyDirectory);
					w.WriteStartElement("patternset");
					w.WriteAttributeString("refid", "srcfiles");
					w.WriteEndElement();
					w.WriteStartElement("exclude");
					w.WriteAttributeString("name", "*.xml");
					w.WriteEndElement();
					w.WriteEndElement();
					
					w.WriteStartElement("fileset");
					w.WriteStartElement("include");
					w.WriteAttributeString("name", "doc/*.shfbproj");
					w.WriteEndElement();
					w.WriteStartElement("include");
					w.WriteAttributeString("name", "doc/additional/*.xml");
					w.WriteEndElement();
					w.WriteEndElement();
					
					w.WriteStartElement("fileset");
					w.WriteAttributeString("basedir", settings.Options.TemporaryDirectory + "/readme");
					w.WriteStartElement("include");
					w.WriteAttributeString("name", "*");
					w.WriteEndElement();
					w.WriteEndElement();
					break;
				case DownloadPackage.ApiDocumentation:
					w.WriteStartElement("fileset");
					w.WriteAttributeString("basedir", "doc/Help");
					w.WriteStartElement("include");
					w.WriteAttributeString("name", "*.chm");
					w.WriteEndElement();
					w.WriteEndElement();
					
					w.WriteStartElement("fileset");
					w.WriteAttributeString("basedir", settings.Options.TemporaryDirectory + "/readme");
					w.WriteStartElement("include");
					w.WriteAttributeString("name", "*");
					w.WriteEndElement();
					w.WriteEndElement();
					break;
				default:
					throw new InvalidEnumArgumentException("package", (int)package, typeof(DownloadPackage));
			}
			
			w.WriteEndElement();
		}
		
		private static void CopyFile(XmlWriter w, string src, string dest)
		{
			w.WriteStartElement("copy");
			w.WriteAttributeString("file", src);
			w.WriteAttributeString("tofile", dest);
			w.WriteEndElement();
		}
		
		private static void ExecuteProgram(XmlWriter w, string program, string workingDir, params string[] args)
		{
			w.WriteStartElement("exec");
			w.WriteAttributeString("program", program);
			if (workingDir != null) {
				w.WriteAttributeString("workingdir", workingDir);
			}
			foreach (var arg in args) {
				w.WriteStartElement("arg");
				w.WriteAttributeString("value", arg);
				w.WriteEndElement();
			}
			w.WriteEndElement();
		}
	}
}
