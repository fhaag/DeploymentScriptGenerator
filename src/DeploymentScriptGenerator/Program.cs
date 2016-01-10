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

using Deployment.ScriptGenerator.Resources;

namespace Deployment.ScriptGenerator
{
	static class Program
	{
		public static void Main(string[] args)
		{
			try {
				var options = new GeneratorOptions();
				using (var parser = new CommandLine.Parser()) {
					if (!parser.ParseArguments(args, options)) {
						Console.WriteLine(options.GetUsage());
						return;
					}
				}
				
				GeneralSettings settings;
				try {
					settings = new GeneralSettings(options);
				}
				catch (ArgumentException ex) {
					Console.WriteLine("Invalid command line arguments: " + ex.Message);
					return;
				}
				
				NAntBuildScript.Write(settings);
				ApiKeysTemplateFile.Write(settings);
				
				if (settings.Options.CreateProjectInfoFile) {
					ProjectInfoFile.Write(settings);
				}
				
				ResourceLoader.SaveAdvanceChangeLogXslt(settings);
				if (settings.Options.WebChangeLog) {
					ResourceLoader.SaveCreateHtmlChangeLogXslt(settings);
				}
				if (settings.Options.TextChangeLog) {
					ResourceLoader.SaveCreateTextChangeLogXslt(settings);
				}
				
				if (settings.RequireXsltStringExtensions) {
					ResourceLoader.SaveXsltStringExtensionsFile(settings);
				}
				if (settings.RequireXsltJsonExtensions) {
					ResourceLoader.SaveXsltJsonExtensionsFile(settings);
				}
				
				if (settings.Options.CreateGitIgnoreFile) {
					GitIgnoreWriter.WriteIgnoreFile(settings);
				}
				
				if (settings.Options.CreateBatchFiles) {
					BatchCreator.WriteBatchFiles(settings);
				}
			}
			catch (Exception ex) {
				Console.WriteLine(ex.ToString());
			}
		}
	}
}