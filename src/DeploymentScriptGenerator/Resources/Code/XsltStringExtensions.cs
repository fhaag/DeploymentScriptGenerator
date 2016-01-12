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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.XPath;

namespace XsltExtensions
{
	public class XsltStringExtensions
	{
		private static string ReplaceCustomTokens(string str, IDictionary<string, string> tokenMap)
		{
			if (str == null) {
				throw new ArgumentNullException("str");
			}
			if (tokenMap == null) {
				throw new ArgumentNullException("tokenMap");
			}
			
			var result = new StringBuilder(str.Length);
			var parts = str.Split('%');
			
			bool omitLastDelimiter = false;
			for (int i = 0; i < parts.Length; i++) {
				if (i <= 0) {
					result.Append(parts[0]);
				} else if (i >= parts.Length - 1) {
					if (!omitLastDelimiter) {
						result.Append('%');
					}
					result.Append(parts[parts.Length - 1]);
				} else {
					string newValue;
					if (tokenMap.TryGetValue(parts[i], out newValue)) {
						result.Append(newValue);
						omitLastDelimiter = true;
					} else {
						if (!omitLastDelimiter) {
							result.Append('%');
						}
						result.Append(parts[i]);
					}
				}
			}
			return result.ToString();
		}
		
		public string ReplaceTokens(string str, string filename, string version, string date)
		{
			var tokenMap = new Dictionary<string, string>();
			tokenMap["FILE"] = filename;
			tokenMap["VERSION"] = version;
			tokenMap["DATE"] = date;
			
			return ReplaceCustomTokens(str, tokenMap);
		}
		
		public string ReplaceUrlTokens(string url, string version, string date, string projectId, string prettyProjectId)
		{
			var tokenMap = new Dictionary<string, string>();
			tokenMap["VERSION"] = version;
			tokenMap["DATE"] = date;
			tokenMap["PROJECT"] = projectId;
			tokenMap["PROJECTFORMATTED"] = prettyProjectId;
			
			return ReplaceCustomTokens(str, tokenMap);
		}
		
		public string Underline(int length, string underlineChar)
		{
			if (underlineChar == null) {
				throw new ArgumentNullException("underlineChar");
			}
			if (underlineChar.Length != 1) {
				throw new ArgumentException("The underlineChar argument must be a string of length 1.");
			}
			
			var result = new StringBuilder(length);
			result.Append(underlineChar[0], length);
			return result.ToString();
		}
	}
}