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
using System.IO;
using System.Reflection;

namespace Deployment.ScriptGenerator.Resources
{
	internal static class ResourceLoader
	{
		private static string LoadTextFromResource(string resourceName)
		{
			using (var r = new StreamReader(typeof(ResourceLoader).Assembly.GetManifestResourceStream("Deployment.ScriptGenerator.Resources." + resourceName), System.Text.Encoding.UTF8)) {
				return r.ReadToEnd();
			}
		}
		
		public static string LineWrappedLicenseText {
			get {
				return LoadTextFromResource("License");
			}
		}
		
		private static void SaveResourceToFile(string resourceName, string destPath)
		{
			using (var f = File.OpenWrite(destPath)) {
				using (var s = typeof(ResourceLoader).Assembly.GetManifestResourceStream("Deployment.ScriptGenerator.Resources." + resourceName)) {
					s.CopyTo(f);
				}
			}
		}
		
		public static void SaveXsltStringExtensionsFile(GeneralSettings settings)
		{
			if (settings == null) {
				throw new ArgumentNullException("settings");
			}
			
			SaveResourceToFile("Code.XsltStringExtensions", Path.Combine(settings.PrepareToolDirectory(), "XsltStringExtensions.cs"));
		}
		
		public static void SaveXsltJsonExtensionsFile(GeneralSettings settings)
		{
			if (settings == null) {
				throw new ArgumentNullException("settings");
			}
			
			SaveResourceToFile("Code.XsltJsonExtensions", Path.Combine(settings.PrepareToolDirectory(), "XsltJsonExtensions.cs"));
		}
		
		public static void SaveAdvanceChangeLogXslt(GeneralSettings settings)
		{
			if (settings == null) {
				throw new ArgumentNullException("settings");
			}
			
			SaveResourceToFile("Code.AdvanceChangeLogXslt", Path.Combine(settings.PrepareToolDirectory(), "AdvanceChangeLog.xsl"));
		}
		
		public static void SaveCreateHtmlChangeLogXslt(GeneralSettings settings)
		{
			if (settings == null) {
				throw new ArgumentNullException("settings");
			}
			
			SaveResourceToFile("Code.CreateHtmlChangeLogXslt", Path.Combine(settings.PrepareToolDirectory(), "CreateHtmlChangeLog.xsl"));
		}
		
		public static void SaveCreateTextChangeLogXslt(GeneralSettings settings)
		{
			if (settings == null) {
				throw new ArgumentNullException("settings");
			}
			
			SaveResourceToFile("Code.CreateTextChangeLogXslt", Path.Combine(settings.PrepareToolDirectory(), "CreateTextChangeLog.xsl"));
		}
		
		public static void SaveExampleProjectsXslt(GeneralSettings settings)
		{
			if (settings == null) {
				throw new ArgumentNullException("settings");
			}
			
			SaveResourceToFile("Code.ExampleProjectsXslt", Path.Combine(settings.PrepareToolDirectory(), "ExampleProjects.xsl"));
		}
	}
}
