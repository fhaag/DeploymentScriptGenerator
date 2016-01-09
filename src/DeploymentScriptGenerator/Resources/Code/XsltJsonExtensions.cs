/*
------------------------------------------------------------------------------
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
------------------------------------------------------------------------------
 */
using System;
using System.Linq;
using System.Text;
using System.Xml.XPath;

namespace XsltExtensions
{
	public class XsltJsonExtensions
	{
		public string Escape(string raw)
		{
			if (raw == null) {
				throw new ArgumentNullException("raw");
			}
			
			var result = new StringBuilder(raw.Length);
			foreach (char ch in raw) {
				switch (ch) {
					case '"':
					case '\\':
						result.Append('\\');
						result.Append(ch);
						break;
					default:
						if (char.IsControl(ch)) {
							result.Append("\\u" + ((int)ch).ToString("X4"));
						} else {
							result.Append(ch);
						}
						break;
				}
			}
			return result.ToString();
		}
		
		public string Concat(string delimiter, XPathNodeIterator nodes)
		{
			if (delimiter == null) {
				throw new ArgumentNullException("delimiter");
			}
			if (nodes == null) {
				throw new ArgumentNullException("nodes");
			}
			
			return Escape(string.Join(delimiter, nodes.Cast<XPathNavigator>().Select(n => n.Value)));
		}
	}
}