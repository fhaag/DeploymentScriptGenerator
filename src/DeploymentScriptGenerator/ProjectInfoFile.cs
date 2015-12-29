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
	internal static class ProjectInfoFile
	{
		public static void Write(GeneralSettings settings)
		{
			if (settings == null) {
				throw new ArgumentNullException("settings");
			}
			
			XmlWriterSettings writerSettings = new XmlWriterSettings();
			writerSettings.Indent = true;
			using (var w = XmlWriter.Create(Path.Combine(settings.PreparePublicDirectory(), settings.Options.ProjectInfoFile), writerSettings)) {
				Write(settings, w);
			}
		}
		
		private static void Write(GeneralSettings settings, XmlWriter w)
		{
			w.WriteStartDocument();
			w.WriteComment("\n" + ResourceLoader.LineWrappedLicenseText + "\n");
			
			w.WriteStartElement("", "project", settings.DefaultNamespaceUri);
			
			WriteToDoElement(w, "id", "insert project ID");
			WriteToDoElement(w, "prettyId", "insert nicely formatted project ID");
			
			WriteProjectId(w, "CodePlex", settings.SupportCodePlex);
			WriteProjectId(w, "Github", settings.SupportGithub);
			WriteProjectId(w, "SourceForge", settings.SupportSourceForge);
			
			WriteToDoElement(w, "title", "insert human-readable project title");
			WriteToDoElement(w, "shortDescription", "insert short project description");
			WriteToDoElement(w, "description", "insert long project description");
			WriteToDoElement(w, "logo", "insert public URL of logo graphic (preferrably 128 * 128px)");
			WriteToDoElement(w, "screenshot", "insert public URL of a representative screenshot or remove");
			WriteToDoElement(w, "licenseName", "insert human-readable name of the license to apply to the major part of the project");
			
			w.WriteStartElement("links");
			w.WriteStartElement("link");
			w.WriteAttributeString("type", "");
			w.WriteAttributeString("title", "");
			w.WriteAttributeString("url", "");
			w.WriteEndElement();
			w.WriteEndElement();
			
			w.WriteStartElement("downloads");
			
			//WriteReleaseBaseUrl(w, "CodePlex", settings.Options.ReleaseOnCodePlex, ""); // apparently no regular download ID available
			WriteReleaseBaseUrl(w, "Github", settings.Options.ReleaseOnGithub, "https://github.com/TODO: user-or-org/TODO: repository/releases/download/v%VERSION%/%FILE%");
			WriteReleaseBaseUrl(w, "SourceForge", settings.Options.ReleaseOnSourceForge, "http://sourceforge.net/projects/TODO: project/files/TODO: pretty project ID/%VERSION%/%FILE%/download");
			
			WriteReleaseFile(w, "src", "Project Sources + Sample Sources", false, true);
			WriteReleaseFile(w, "bin", "Compiled Project + Sample Sources", true, false);
			if (settings.Options.ReleaseApiDoc) {
				WriteReleaseFile(w, "chm", "CHM Help File", false, false);
			}
			
			w.WriteEndElement();
			
			w.WriteEndDocument();
		}
		
		private static void WriteToDoPlaceholder(XmlWriter w, string text)
		{
			w.WriteComment("TODO: " + text);
		}
		
		private static void WriteToDoElement(XmlWriter w, string elementName, string toDoText)
		{
			w.WriteStartElement(elementName);
			WriteToDoPlaceholder(w, toDoText);
			w.WriteEndElement();
		}
		
		private static void WriteProjectId(XmlWriter w, string serviceName, bool supportService)
		{
			if (supportService) {
				w.WriteStartElement("id");
				w.WriteAttributeString("service", serviceName);
				WriteToDoPlaceholder(w, "insert " + serviceName + " project ID, or remove if equal to ID from above");
				w.WriteEndElement();
			}
		}
		
		private static void WriteReleaseBaseUrl(XmlWriter w, string serviceName, bool supportReleaseOnService, string baseUrl)
		{
			if (supportReleaseOnService) {
				w.WriteStartElement("baseUrl");
				w.WriteAttributeString("service", serviceName);
				w.WriteString(baseUrl);
				w.WriteEndElement();
			}
		}
		
		private static void WriteReleaseFile(XmlWriter w, string type, string title, bool isStableDefault, bool isPrereleaseDefault)
		{
			w.WriteStartElement("file");
			w.WriteAttributeString("type", type);
			w.WriteAttributeString("title", title);
			if (isStableDefault) {
				w.WriteAttributeString("stableDefault", "true");
			}
			if (isPrereleaseDefault) {
				w.WriteAttributeString("prereleaseDefault", "true");
			}
			WriteToDoPlaceholder(w, "file name with placeholders %VERSION%, %DATE%, without file extension");
			w.WriteEndElement();
		}
	}
}
