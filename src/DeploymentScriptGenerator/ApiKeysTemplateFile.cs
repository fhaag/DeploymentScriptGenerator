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
using System.Xml;

using Deployment.ScriptGenerator.Resources;

namespace Deployment.ScriptGenerator
{
	/// <summary>
	/// Writes blank project info files.
	/// </summary>
	internal static class ApiKeysTemplateFile
	{
		public static void Write(GeneralSettings settings)
		{
			if (settings == null) {
				throw new ArgumentNullException("settings");
			}
			
			XmlWriterSettings writerSettings = new XmlWriterSettings();
			writerSettings.Indent = true;
			using (var w = XmlWriter.Create(Path.Combine(settings.PrepareKeyDirectory(), "apikeys.template.xml"), writerSettings)) {
				Write(settings, w);
			}
		}
		
		private static void Write(GeneralSettings settings, XmlWriter w)
		{
			w.WriteStartDocument();
			w.WriteComment("\n" + ResourceLoader.LineWrappedLicenseText + "\n");
			
			w.WriteStartElement("keys");
			
			if (settings.Options.ReleaseOnCodePlex) {
				WriteEntry(w, "CodePlexUser", "CodePlex user name");
				WriteEntry(w, "CodePlexPW", "CodePlex password");
			}
			if (settings.Options.ReleaseOnGithub) {
				WriteEntry(w, "GithubUser", "Github user name");
				WriteEntry(w, "GithubPW", "Github password");
			}
			if (settings.RequireLocalWebPath) {
				WriteEntry(w, "localWebPath", "relative path from project to working copy of website repository");
			}
			if (settings.Options.ReleaseOnSourceForge || settings.Options.PublishSourceForgeProjectWeb) { // TODO: alternatively, if SF website is required
				WriteEntry(w, "SourceForgeAPI", "SourceForge API key");
				WriteEntry(w, "SourceForgeUser", "SourceForge user name");
				WriteEntry(w, "SourceForgeSSH", "path to private SourceForge SSH key file");
			}
			
			w.WriteEndElement();
		}
		
		private static void WriteEntry(XmlWriter w, string keyName, string description)
		{
			w.WriteStartElement("key");
			w.WriteAttributeString("id", keyName);
			w.WriteAttributeString("value", "");
			w.WriteEndElement();
			w.WriteComment(description);
		}
	}
}
