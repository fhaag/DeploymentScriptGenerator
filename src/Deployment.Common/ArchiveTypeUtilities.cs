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
using System.ComponentModel;

namespace Deployment
{
	public static class ArchiveTypeUtilities
	{
		public static string GetFileExtension(this ArchiveType archiveType)
		{
			switch (archiveType) {
				case ArchiveType.Tar:
					return ".tar";
				case ArchiveType.TarBz2:
					return ".tar.bz";
				case ArchiveType.TarGz:
					return ".tar.gz";
				case ArchiveType.Zip:
					return ".zip";
				default:
					throw new InvalidEnumArgumentException("archiveType", (int)archiveType, typeof(ArchiveType));
			}
		}
		
		public static ArchiveType? StringToArchiveType(string text)
		{
			if (text == null) {
				return null;
			}
			
			switch (text.ToLowerInvariant()) {
				case "zip":
					return ArchiveType.Zip;
				case "tar":
					return ArchiveType.Tar;
				case "tgz":
					return ArchiveType.TarGz;
				case "tbz":
					return ArchiveType.TarBz2;
				case "":
					return null;
				default:
					throw new ArgumentException("Invalid download format: " + text);
			}
		}
		
		public static string ArchiveTypeToString(this ArchiveType archiveType)
		{
			switch (archiveType) {
				case ArchiveType.Tar:
					return "tar";
				case ArchiveType.TarBz2:
					return "tbz";
				case ArchiveType.TarGz:
					return "tgz";
				case ArchiveType.Zip:
					return "zip";
				default:
					throw new InvalidEnumArgumentException("archiveType", (int)archiveType, typeof(ArchiveType));
			}
		}
	}
}
