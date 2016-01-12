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
	/// <summary>
	/// Writes Git ignore files.
	/// </summary>
	internal static class GitIgnoreWriter
	{
		private sealed class GitIgnoreFile
		{
			public GitIgnoreFile(string path)
			{
				if (path == null) {
					throw new ArgumentNullException("path");
				}
				
				this.path = path;
				
				try {
					foreach (string existing in File.ReadAllLines(path).Where(l => !string.IsNullOrWhiteSpace(l))) {
						this.entries.Add(existing);
					}
				}
				catch (IOException) {
				}
			}
			
			private readonly string path;
			
			private readonly ISet<string> entries = new HashSet<string>();
			
			public void Add(params string[] newEntries)
			{
				if (newEntries == null) {
					throw new ArgumentNullException("newEntries");
				}
				
				foreach (string ne in newEntries) {
					if (!string.IsNullOrWhiteSpace(ne)) {
						this.entries.Add(ne);
					}
				}
			}
			
			public void Save()
			{
				Directory.CreateDirectory(Path.GetDirectoryName(path));
				File.WriteAllLines(path, entries);
			}
		}
		
		public static void WriteIgnoreFile(GeneralSettings settings)
		{
			if (settings == null) {
				throw new ArgumentNullException("settings");
			}
			
			var mainFile = new GitIgnoreFile(Path.Combine(settings.BasePath, ".gitignore"));
			var sourcesFile = new GitIgnoreFile(Path.Combine(settings.BasePath, "src", ".gitignore"));
			
			mainFile.Add("/bin",
			             "/release");
			if (settings.Options.MinimalMode) {
				mainFile.Add("/release.build");
				mainFile.Add("/" + settings.Options.ToolDirectory);
				if (settings.Options.CreateBatchFiles) {
					mainFile.Add("/buildrelease.bat");
					mainFile.Add("/publishrelease.bat");
				}
			}
			
			sourcesFile.Add("bin/",
			                "obj/",
			                "packages/",
			                "*.PartCover.Settings");
			
			mainFile.Save();
			sourcesFile.Save();
		}
	}
}
