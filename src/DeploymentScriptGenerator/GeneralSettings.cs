/*
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

namespace Deployment.ScriptGenerator
{
	internal sealed class GeneralSettings
	{
		public GeneralSettings(GeneratorOptions options)
		{
			if (options == null) {
				throw new ArgumentNullException("options");
			}
			
			switch (options.DownloadFormat.ToLowerInvariant()) {
				case "zip":
					downloadFormat = ArchiveType.Zip;
					break;
				case "tar":
					downloadFormat = ArchiveType.Tar;
					break;
				case "tgz":
					downloadFormat = ArchiveType.TarGz;
					break;
				case "tbz":
					downloadFormat = ArchiveType.TarBz2;
					break;
				default:
					throw new ArgumentException("Invalid download format: " + options.DownloadFormat);
			}
			
			this.options = options;
			this.basePath = Environment.CurrentDirectory;
		}
		
		private readonly string basePath;
		
		public string BasePath {
			get {
				return basePath;
			}
		}
		
		private readonly GeneratorOptions options;
		
		public GeneratorOptions Options {
			get {
				return options;
			}
		}
		
		private readonly ArchiveType downloadFormat;
		
		public ArchiveType DownloadFormat {
			get {
				return downloadFormat;
			}
		}
		
		public bool SupportGithub {
			get {
				return options.ReleaseOnGithub;
			}
		}
		
		public bool SupportSourceForge {
			get {
				return options.ReleaseOnSourceForge;
			}
		}
		
		public bool SupportCodePlex {
			get {
				return options.ReleaseOnCodePlex;
			}
		}
		
		public bool RequireXsltExtensions {
			get {
				return false; // TODO
			}
		}
		
		public bool ProcessExampleProjects {
			get {
				return false; // TODO
			}
		}
	}
}
