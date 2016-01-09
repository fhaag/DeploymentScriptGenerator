/*
This source file is a part of DeploymentScriptGenerator.

Copyright (c) 2015 - 2016 Florian Haag

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

using CommandLine;
using CommandLine.Text;

namespace Deployment.ScriptGenerator
{
	internal class GeneratorOptions
	{
		[Option('P', "githubpages", HelpText = "Indicates whether to publish website content via Github Pages.")]
		public bool PublishGithubPages { get; set; }
		
		[Option('W', "sfprojectweb", HelpText = "Indicates whether to publish website content via SourceForge's Project Web service.")]
		public bool PublishSourceForgeProjectWeb { get; set; }
		
		[Option('a', "apidoc", HelpText = "Indicates whether to release a downloadable API documentation.")]
		public bool ReleaseApiDoc { get; set; }
		
		[Option('c', "codeplex", HelpText = "Indicates whether to release downloads as CodePlex releases.")]
		public bool ReleaseOnCodePlex { get; set; }
		
		[Option('f', "format", DefaultValue = "zip", HelpText = "The default format for downloadable files (can be zip, tar, tgz, or tbz).")]
		public string DownloadFormat { get; set; }
		
		[Option('g', "github", HelpText = "Indicates whether to release downloads via the Github release feature.")]
		public bool ReleaseOnGithub { get; set; }
		
		[Option('i', "projectinfo", HelpText = "If set, a blank project info file will be created.")]
		public bool CreateProjectInfoFile { get; set; }
		
		[Option('l', "webchangelog", HelpText = "Indicates whether an HTML changelog is generated and uploaded to the website.")]
		public bool WebChangeLog { get; set; }
		
		[Option('n', "nuget", HelpText = "Indicates whether to release any NuGet packages for the project.")]
		public bool ReleaseOnNuGet { get; set; }
		
		[Option('s', "sourceforge", HelpText = "Indicates whether to release downloads via the SourceForge File Release System.")]
		public bool ReleaseOnSourceForge { get; set; }
		
		[Option('t', "textchangelog", HelpText = "Indicates whether a plain text changelog is generated and included in releases.")]
		public bool TextChangeLog { get; set; }
		
		[Option('w', "webapidoc", HelpText = "Indicates whether to publish a web-based API documentation on the website (stable releases only).")]
		public bool WebApiDoc { get; set; }
		
		[Option('x', "exampleprojects", HelpText = "Indicates whether example projects are considered.")]
		public bool ProcessExampleProjects { get; set; }
		
		[Option("gitignore", HelpText = "If set, .gitignore files will be created.")]
		public bool CreateGitIgnoreFile { get; set; }
		
		[Option("buildfile", DefaultValue = "release.build", HelpText = "Name of the main build file.")]
		public string BuildFile { get; set; }
		
		[Option("publicdir", DefaultValue = "pubinfo", HelpText = "Name of the folder for public project information.")]
		public string PublicDirectory { get; set; }
		
		[Option("projectinfofile", DefaultValue = "projectinfo.xml", HelpText = "Name of the project information file.")]
		public string ProjectInfoFile { get; set; }
		
		[Option("toolsdir", DefaultValue = "tools", HelpText = "Name of the folder for build support files.")]
		public string ToolDirectory { get; set; }
		
		[Option("tempdir", DefaultValue = "buildtmp", HelpText = "Name of the folder for temporary files created during the build process.")]
		public string TemporaryDirectory { get; set; }
		
		[Option("releasedir", DefaultValue = "release", HelpText = "Name of the folder that receives the final release files.")]
		public string ReleaseDirectory { get; set; }
		
		[Option("keydir", DefaultValue = "keys", HelpText = "Name of the folder that stores keys and access credentials.")]
		public string KeyDirectory { get; set; }
		
		[Option("exampledir", DefaultValue = "Samples", HelpText = "Name of the folder that stores example projects. This must be a subdirectory of the sources directory.")]
		public string ExampleDirectory { get; set; }
		
		[Option("sourceforgeformat", DefaultValue = "", HelpText = "The format for downloadable files on SourceForge. An empty string uses the default from 'format', otherwise overrides 'format'.")]
		public string SourceForgeFormat { get; set; }
		
		[Option("codeplexformat", DefaultValue = "", HelpText = "The format for downloadable files on CodePlex. An empty string uses the default from 'format', otherwise overrides 'format'.")]
		public string CodePlexFormat { get; set; }
		
		[Option("githubformat", DefaultValue = "", HelpText = "The format for downloadable files on Github. An empty string uses the default from 'format', otherwise overrides 'format'.")]
		public string GithubFormat { get; set; }
		
		[HelpOption]
		public string GetUsage()
		{
			var text = HelpText.AutoBuild(this);
			return text.ToString();
		}
	}
}
