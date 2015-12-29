<?xml version="1.0" encoding="utf-8"?>
<!--
This source file is a part of DeploymentScriptGenerator.

Copyright (c) 2012 - 2015 Florian Haag

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
-->
<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns="http://schemas.microsoft.com/developer/msbuild/2003" xmlns:ms="http://schemas.microsoft.com/developer/msbuild/2003">
	
	<xsl:output method="xml" indent="yes" encoding="UTF-8" omit-xml-declaration="yes"/>
	<xsl:namespace-alias stylesheet-prefix="ms" result-prefix="#default"/>
	
	<xsl:param name="ProjectPrefix"/>
	
	<xsl:template match="@*|node()">
		<xsl:copy>
			<xsl:apply-templates select="@*|node()"/>
		</xsl:copy>
	</xsl:template>
	
	<!-- ================ Library References ================ -->
	
	<xsl:template match="/ms:Project/ms:ItemGroup/ms:ProjectReference[starts-with(ms:Name, $ProjectPrefix)]">
		<Reference>
			<xsl:attribute name="Include">
				<xsl:value-of select="ms:Name"/>
			</xsl:attribute>
			<HintPath>..\bin\<xsl:value-of select="ms:Name"/>.dll</HintPath>
		</Reference>
	</xsl:template>
	
	<!-- ================ Version.cs ================ -->
	
	<xsl:template match="/ms:Project/ms:ItemGroup/ms:Compile[ms:Link = 'Properties\Version.cs'][contains(@ms:Include, 'Version.cs')]">
		<Compile Include="Properties\Version.cs"/>
	</xsl:template>
	
</xsl:transform>