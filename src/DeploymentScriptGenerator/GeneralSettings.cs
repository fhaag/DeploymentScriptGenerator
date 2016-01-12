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
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Deployment.ScriptGenerator
{
	internal sealed class GeneralSettings
	{
		public GeneralSettings(GeneratorOptions options)
		{
			if (options == null) {
				throw new ArgumentNullException("options");
			}
			
			var fmt = ArchiveTypeUtilities.StringToArchiveType(options.DownloadFormat);
			this.downloadFormat = fmt.HasValue ? fmt.Value : ArchiveType.Zip;
			
			fmt = ArchiveTypeUtilities.StringToArchiveType(options.SourceForgeFormat);
			this.sourceForgeDownloadFormat = fmt.HasValue ? fmt.Value : this.downloadFormat;
			
			fmt = ArchiveTypeUtilities.StringToArchiveType(options.CodePlexFormat);
			this.codePlexDownloadFormat = fmt.HasValue ? fmt.Value : this.downloadFormat;
			
			fmt = ArchiveTypeUtilities.StringToArchiveType(options.GithubFormat);
			this.githubDownloadFormat = fmt.HasValue ? fmt.Value : this.downloadFormat;
			
			this.options = options;
			this.basePath = Environment.CurrentDirectory;
		}
		
		private readonly string basePath;
		
		public string BasePath {
			get {
				return basePath;
			}
		}
		
		public string PrepareToolDirectory()
		{
			string result = Path.Combine(basePath, options.ToolDirectory);
			Directory.CreateDirectory(result);
			return result;
		}
		
		public string PrepareKeyDirectory()
		{
			string result = Path.Combine(basePath, options.KeyDirectory);
			Directory.CreateDirectory(result);
			return result;
		}
		
		public string PreparePublicDirectory()
		{
			string result = Path.Combine(basePath, options.PublicDirectory);
			Directory.CreateDirectory(result);
			return result;
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
		
		private readonly ArchiveType sourceForgeDownloadFormat;
		
		public ArchiveType SourceForgeDownloadFormat {
			get {
				return sourceForgeDownloadFormat;
			}
		}
		
		private readonly ArchiveType codePlexDownloadFormat;
		
		public ArchiveType CodePlexDownloadFormat {
			get {
				return codePlexDownloadFormat;
			}
		}
		
		private readonly ArchiveType githubDownloadFormat;
		
		public ArchiveType GithubDownloadFormat {
			get {
				return githubDownloadFormat;
			}
		}
		
		public IEnumerable<ArchiveType> AllDownloadFormats {
			get {
				var allFormats = new HashSet<ArchiveType>();
				allFormats.Add(this.downloadFormat);
				allFormats.Add(this.sourceForgeDownloadFormat);
				allFormats.Add(this.codePlexDownloadFormat);
				allFormats.Add(this.githubDownloadFormat);
				return allFormats.ToArray();
			}
		}
		
		public bool SupportGithub {
			get {
				return options.ReleaseOnGithub || options.PublishGithubPages;
			}
		}
		
		public bool SupportSourceForge {
			get {
				return options.ReleaseOnSourceForge || options.PublishSourceForgeProjectWeb;
			}
		}
		
		public bool SupportCodePlex {
			get {
				return options.ReleaseOnCodePlex;
			}
		}
		
		public bool RequireLocalWebPath {
			get {
				return options.PublishGithubPages;
			}
		}
		
		public bool RequireXsltExtensions {
			get {
				return RequireXsltStringExtensions || RequireXsltJsonExtensions;
			}
		}
		
		public bool RequireXsltStringExtensions {
			get {
				return true; // always required for advancing the changelog
			}
		}
		
		public bool RequireXsltJsonExtensions {
			get {
				return false; // TODO
			}
		}
		
		public bool ProcessExampleProjects {
			get {
				return options.ProcessExampleProjects;
			}
		}
		
		public string DefaultNamespaceUri {
			get {
				return "http://local/";
			}
		}
		
		public string WebChangeLogFileName(ExternalSite site)
		{
			return "changelog_" + site.ExternalSiteToString() + ".html";
		}
	}
}
