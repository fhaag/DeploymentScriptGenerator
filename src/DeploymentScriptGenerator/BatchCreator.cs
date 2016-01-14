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

namespace Deployment.ScriptGenerator
{
	internal static class BatchCreator
	{
		public static void WriteBatchFiles(GeneralSettings settings)
		{
			if (settings == null) {
				throw new ArgumentNullException("settings");
			}
			
			File.WriteAllText(Path.Combine(settings.BasePath, "buildrelease.bat"), "call nant -D:release.version=%1 release");
			File.WriteAllText(Path.Combine(settings.BasePath, "publishrelease.bat"), "call nant -D:release.version=%1 publish");
			
			if (settings.Options.MinimalMode && settings.Options.CreateScriptCreationBatchFile) {
				var creationBatchCommand = new System.Text.StringBuilder("call DeploymentScriptCreator");
				
				var flags = RetrieveActiveFlags(settings);
				if (flags.Count > 0) {
					creationBatchCommand.Append(" -");
					foreach (char flag in flags.OrderBy(ch => ch)) {
						creationBatchCommand.Append(flag);
					}
				}
				
				if (settings.DownloadFormat != ArchiveType.Zip) {
					creationBatchCommand.Append(" -f " + settings.DownloadFormat.ArchiveTypeToString());
				}
				if (settings.CodePlexDownloadFormat != settings.DownloadFormat) {
					creationBatchCommand.Append(" --codeplexformat " + settings.CodePlexDownloadFormat.ArchiveTypeToString());
				}
				if (settings.GithubDownloadFormat != settings.DownloadFormat) {
					creationBatchCommand.Append(" --githubformat " + settings.GithubDownloadFormat.ArchiveTypeToString());
				}
				if (settings.SourceForgeDownloadFormat != settings.DownloadFormat) {
					creationBatchCommand.Append(" --sourceforgeformat " + settings.SourceForgeDownloadFormat.ArchiveTypeToString());
				}
				
				if (settings.Options.BuildFile != GeneratorOptions.DEFAULT_BUILD_FILE) {
					creationBatchCommand.Append(" --buildfile \"" + settings.Options.BuildFile + "\"");
				}
				if (settings.Options.PublicDirectory != GeneratorOptions.DEFAULT_PUBLIC_DIR) {
					creationBatchCommand.Append(" --publicdir \"" + settings.Options.PublicDirectory + "\"");
				}
				if (settings.Options.ToolDirectory != GeneratorOptions.DEFAULT_TOOLS_DIR) {
					creationBatchCommand.Append(" --toolsdir \"" + settings.Options.ToolDirectory + "\"");
				}
				if (settings.Options.TemporaryDirectory != GeneratorOptions.DEFAULT_TEMPORARY_DIR) {
					creationBatchCommand.Append(" --tempdir \"" + settings.Options.TemporaryDirectory + "\"");
				}
				if (settings.Options.ReleaseDirectory != GeneratorOptions.DEFAULT_RELEASE_DIR) {
					creationBatchCommand.Append(" --releasedir \"" + settings.Options.ReleaseDirectory + "\"");
				}
				if (settings.Options.KeyDirectory != GeneratorOptions.DEFAULT_KEY_DIR) {
					creationBatchCommand.Append(" --keydir \"" + settings.Options.KeyDirectory + "\"");
				}
				if (settings.Options.ExampleDirectory != GeneratorOptions.DEFAULT_EXAMPLE_DIR) {
					creationBatchCommand.Append(" --exampledir \"" + settings.Options.ExampleDirectory + "\"");
				}
				if (settings.Options.ProjectInfoFile != GeneratorOptions.DEFAULT_PROJECT_INFO_FILE) {
					creationBatchCommand.Append(" --projectinfofile \"" + settings.Options.ProjectInfoFile + "\"");
				}
				
				if (settings.Options.PrimaryColor != GeneratorOptions.DEFAULT_PRIMARY_COLOR) {
					creationBatchCommand.Append(" --primarycolor " + settings.Options.PrimaryColor);
				}
				
				File.WriteAllText(Path.Combine(settings.BasePath, "createdeploymentscript.bat"), creationBatchCommand.ToString());
			}
		}
		
		private static ISet<char> RetrieveActiveFlags(GeneralSettings settings)
		{
			var result = new HashSet<char>();
			
			if (settings.Options.MinimalMode) {
				result.Add(GeneratorOptions.MINIMAL_MODE_FLAG);
			}
			
			if (settings.Options.PublishGithubPages) {
				result.Add(GeneratorOptions.PUBLISH_GITHUB_PAGES_FLAG);
			}
			if (settings.Options.PublishSourceForgeProjectWeb) {
				result.Add(GeneratorOptions.PUBLISH_SOURCE_FORGE_PROJECT_WEB_FLAG);
			}
			
			if (settings.Options.ReleaseApiDoc) {
				result.Add(GeneratorOptions.RELEASE_API_DOC_FLAG);
			}
			if (settings.Options.WebApiDoc) {
				result.Add(GeneratorOptions.WEB_API_DOC_FLAG);
			}
			
			if (settings.Options.ReleaseOnCodePlex) {
				result.Add(GeneratorOptions.RELEASE_ON_CODE_PLEX_FLAG);
			}
			if (settings.Options.ReleaseOnGithub) {
				result.Add(GeneratorOptions.RELEASE_ON_GITHUB_FLAG);
			}
			if (settings.Options.ReleaseOnSourceForge) {
				result.Add(GeneratorOptions.RELEASE_ON_SOURCE_FORGE_FLAG);
			}
			if (settings.Options.ReleaseOnNuGet) {
				result.Add(GeneratorOptions.RELEASE_ON_NU_GET_FLAG);
			}
			
			if (settings.Options.TextChangeLog) {
				result.Add(GeneratorOptions.TEXT_CHANGE_LOG_FLAG);
			}
			if (settings.Options.WebChangeLog) {
				result.Add(GeneratorOptions.WEB_CHANGE_LOG_FLAG);
			}
			
			if (settings.Options.ProcessExampleProjects) {
				result.Add(GeneratorOptions.PROCESS_EXAMPLE_PROJECTS_FLAG);
			}
			
			if (settings.Options.CreateStyleSheets) {
				result.Add(GeneratorOptions.CREATE_STYLE_SHEETS_FLAG);
			}
			
			if (settings.Options.Simulate) {
				result.Add(GeneratorOptions.SIMULATE_FLAG);
			}
			
			return result;
		}
	}
}
