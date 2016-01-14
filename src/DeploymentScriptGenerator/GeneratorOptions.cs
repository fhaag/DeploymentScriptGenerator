﻿/*
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
using System.Text.RegularExpressions;

using CommandLine;
using CommandLine.Text;

namespace Deployment.ScriptGenerator
{
	internal class GeneratorOptions
	{
		public const char CREATE_BATCH_FILES_FLAG = 'B';
		
		[Option(CREATE_BATCH_FILES_FLAG, "batchfiles", HelpText = "If set, batch files for easier invocation of the deployment script will be created.")]
		public bool CreateBatchFiles { get; set; }
		
		public const char CREATE_STYLE_SHEETS_FLAG = 'C';
		
		[Option(CREATE_STYLE_SHEETS_FLAG, "createcss", HelpText = "If set, CSS files for any generated HTML files will be generated (and possibly overwritten). Otherwise, it is assumed the CSS files are already existing.")]
		public bool CreateStyleSheets { get; set; }
		
		public const char CREATE_GIT_IGNORE_FILE_FLAG = 'I';
		
		[Option(CREATE_GIT_IGNORE_FILE_FLAG, "gitignore", HelpText = "If set, .gitignore files will be created.")]
		public bool CreateGitIgnoreFile { get; set; }
		
		public const char MINIMAL_MODE_FLAG = 'M';
		
		[Option(MINIMAL_MODE_FLAG, "minimal", HelpText = "Ensures that a minimal amount of files will be left in the repository. Instead of keeping build script files, the Deployment Script Generator tool is expected to be available on any machine used to prepare and deploy releases. It is recommended to use this flag in combination with the 'scriptcreationbatch' option.")]
		public bool MinimalMode { get; set; }
		
		public const char PUBLISH_GITHUB_PAGES_FLAG = 'P';
		
		[Option(PUBLISH_GITHUB_PAGES_FLAG, "githubpages", HelpText = "Indicates whether to publish website content via Github Pages.")]
		public bool PublishGithubPages { get; set; }
		
		public const char SIMULATE_FLAG = 'S';
		
		[Option(SIMULATE_FLAG, "simulate", HelpText = "If set, files are generated, but no data is transferred or overwritten. Also, temporary directories will not be removed.")]
		public bool Simulate { get; set; }
		
		public const char PUBLISH_SOURCE_FORGE_PROJECT_WEB_FLAG = 'W';
		
		[Option(PUBLISH_SOURCE_FORGE_PROJECT_WEB_FLAG, "sfprojectweb", HelpText = "Indicates whether to publish website content via SourceForge's Project Web service.")]
		public bool PublishSourceForgeProjectWeb { get; set; }
		
		public const char RELEASE_API_DOC_FLAG = 'a';
		
		[Option(RELEASE_API_DOC_FLAG, "apidoc", HelpText = "Indicates whether to release a downloadable API documentation.")]
		public bool ReleaseApiDoc { get; set; }
		
		public const char RELEASE_ON_CODE_PLEX_FLAG = 'c';
		
		[Option(RELEASE_ON_CODE_PLEX_FLAG, "codeplex", HelpText = "Indicates whether to release downloads as CodePlex releases.")]
		public bool ReleaseOnCodePlex { get; set; }
		
		[Option('f', "format", DefaultValue = "zip", HelpText = "The default format for downloadable files (can be zip, tar, tgz, or tbz).")]
		public string DownloadFormat { get; set; }
		
		public const char RELEASE_ON_GITHUB_FLAG = 'g';
		
		[Option(RELEASE_ON_GITHUB_FLAG, "github", HelpText = "Indicates whether to release downloads via the Github release feature.")]
		public bool ReleaseOnGithub { get; set; }
		
		public const char CREATE_PROJECT_INFO_FILE = 'i';
		
		[Option(CREATE_PROJECT_INFO_FILE, "projectinfo", HelpText = "If set, a blank project info file will be created.")]
		public bool CreateProjectInfoFile { get; set; }
		
		public const char WEB_CHANGE_LOG_FLAG = 'l';
		
		[Option(WEB_CHANGE_LOG_FLAG, "webchangelog", HelpText = "Indicates whether an HTML changelog is generated and uploaded to the website.")]
		public bool WebChangeLog { get; set; }
		
		public const char RELEASE_ON_NU_GET_FLAG = 'n';
		
		[Option(RELEASE_ON_NU_GET_FLAG, "nuget", HelpText = "Indicates whether to release any NuGet packages for the project.")]
		public bool ReleaseOnNuGet { get; set; }
		
		public const char RELEASE_ON_SOURCE_FORGE_FLAG = 's';
		
		[Option(RELEASE_ON_SOURCE_FORGE_FLAG, "sourceforge", HelpText = "Indicates whether to release downloads via the SourceForge File Release System.")]
		public bool ReleaseOnSourceForge { get; set; }
		
		public const char TEXT_CHANGE_LOG_FLAG = 't';
		
		[Option(TEXT_CHANGE_LOG_FLAG, "textchangelog", HelpText = "Indicates whether a plain text changelog is generated and included in releases.")]
		public bool TextChangeLog { get; set; }
		
		public const char WEB_API_DOC_FLAG = 'w';
		
		[Option(WEB_API_DOC_FLAG, "webapidoc", HelpText = "Indicates whether to publish a web-based API documentation on the website (stable releases only).")]
		public bool WebApiDoc { get; set; }
		
		public const char PROCESS_EXAMPLE_PROJECTS_FLAG = 'x';
		
		[Option(PROCESS_EXAMPLE_PROJECTS_FLAG, "exampleprojects", HelpText = "Indicates whether example projects are considered.")]
		public bool ProcessExampleProjects { get; set; }
		
		public const string DEFAULT_BUILD_FILE = "release.build";
		
		[Option("buildfile", DefaultValue = DEFAULT_BUILD_FILE, HelpText = "Name of the main build file.")]
		public string BuildFile { get; set; }
		
		public const string DEFAULT_PUBLIC_DIR = "pubinfo";
		
		[Option("publicdir", DefaultValue = DEFAULT_PUBLIC_DIR, HelpText = "Name of the folder for public project information.")]
		public string PublicDirectory { get; set; }
		
		public const string DEFAULT_PROJECT_INFO_FILE = "projectinfo.xml";
		
		[Option("projectinfofile", DefaultValue = DEFAULT_PROJECT_INFO_FILE, HelpText = "Name of the project information file.")]
		public string ProjectInfoFile { get; set; }
		
		public const string DEFAULT_TOOLS_DIR = "tools";
		
		[Option("toolsdir", DefaultValue = DEFAULT_TOOLS_DIR, HelpText = "Name of the folder for build support files.")]
		public string ToolDirectory { get; set; }
		
		public const string DEFAULT_TEMPORARY_DIR = "buildtmp";
		
		[Option("tempdir", DefaultValue = DEFAULT_TEMPORARY_DIR, HelpText = "Name of the folder for temporary files created during the build process.")]
		public string TemporaryDirectory { get; set; }
		
		public const string DEFAULT_RELEASE_DIR = "release";
		
		[Option("releasedir", DefaultValue = DEFAULT_RELEASE_DIR, HelpText = "Name of the folder that receives the final release files.")]
		public string ReleaseDirectory { get; set; }
		
		public const string DEFAULT_KEY_DIR = "keys";
		
		[Option("keydir", DefaultValue = DEFAULT_KEY_DIR, HelpText = "Name of the folder that stores keys and access credentials.")]
		public string KeyDirectory { get; set; }
		
		public const string DEFAULT_EXAMPLE_DIR = "Samples";
		
		[Option("exampledir", DefaultValue = DEFAULT_EXAMPLE_DIR, HelpText = "Name of the folder that stores example projects. This must be a subdirectory of the sources directory.")]
		public string ExampleDirectory { get; set; }
		
		[Option("sourceforgeformat", DefaultValue = "", HelpText = "The format for downloadable files on SourceForge. An empty string uses the default from 'format', otherwise overrides 'format'.")]
		public string SourceForgeFormat { get; set; }
		
		[Option("codeplexformat", DefaultValue = "", HelpText = "The format for downloadable files on CodePlex. An empty string uses the default from 'format', otherwise overrides 'format'.")]
		public string CodePlexFormat { get; set; }
		
		[Option("githubformat", DefaultValue = "", HelpText = "The format for downloadable files on Github. An empty string uses the default from 'format', otherwise overrides 'format'.")]
		public string GithubFormat { get; set; }
		
		[Option("scriptcreationbatch", HelpText = "Indicates whether a batch file for creating the deployment scripts should be created. Only effective in minimal mode.")]
		public bool CreateScriptCreationBatchFile { get; set; }
		
		public const string DEFAULT_PRIMARY_COLOR = "002564";
		
		[Option("primarycolor", DefaultValue = DEFAULT_PRIMARY_COLOR, HelpText = "The primary color used in the web-based changelog. This must be a six digit hexadecimal RGB color with a good contrast to white.")]
		public string PrimaryColor { get; set; }
		
		[HelpOption]
		public string GetUsage()
		{
			var text = HelpText.AutoBuild(this);
			return text.ToString();
		}
		
		/// <summary>
		/// Replaces any invalid option values with default values and outputs appropriate messages.
		/// </summary>
		public void Sanitize()
		{
			if (!Regex.IsMatch(this.PrimaryColor, "^[0-9a-fA-F]{6}$")) {
				Console.WriteLine("Using default primary color.");
				this.PrimaryColor = DEFAULT_PRIMARY_COLOR;
			}
		}
	}
}
