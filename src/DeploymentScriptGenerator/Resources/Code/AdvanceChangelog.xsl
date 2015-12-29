<?xml version="1.0" encoding="utf-8"?>
<!--
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
-->
<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns="http://local/" xmlns:local="http://local/" xmlns:str="urn:str" exclude-result-prefixes="str">
	
	<xsl:output method="xml" indent="yes" encoding="UTF-8" omit-xml-declaration="no" standalone="yes"/>
	<xsl:namespace-alias stylesheet-prefix="local" result-prefix="#default"/>
	<xsl:param name="ReleaseDateTime"/>
	<xsl:param name="ReleaseVersion"/>
	<xsl:param name="ReleaseState"/>
	<xsl:param name="ReleaseDownload"/>
	<xsl:param name="ReleaseDate"/>
	
	<xsl:template match="@*|node()">
		<xsl:copy>
			<xsl:apply-templates select="@*|node()"/>
		</xsl:copy>
	</xsl:template>
	
	<xsl:template match="/local:project/local:history/local:current">
		<local:current>
			<local:type></local:type><xsl:comment>minor, major, bugfix, feature, security, documentation, cleanup</xsl:comment>
			<xsl:comment>Add as many change elements as appropriate.</xsl:comment>
			<local:change></local:change>
		</local:current>
		<local:previous>
			<local:version><xsl:value-of select="$ReleaseVersion"/></local:version>
			<local:state><xsl:value-of select="$ReleaseState"/></local:state>
			<xsl:copy-of select="local:type"/>
			<xsl:copy-of select="local:change"/>
			<local:downloads>
			<xsl:for-each select="/local:project/local:downloads/local:file">
				<local:file>
					<xsl:attribute name="type"><xsl:value-of select="@type"/></xsl:attribute>
					<xsl:attribute name="title"><xsl:value-of select="@title"/></xsl:attribute>
					<xsl:variable name="filename" select="str:ReplaceTokens(text(), '', $ReleaseVersion, $ReleaseDate)"/>
					<xsl:attribute name="file"><xsl:value-of select="$filename"/></xsl:attribute>
					<xsl:attribute name="url"><xsl:value-of select="str:ReplaceTokens(/local:project/local:downloads/@baseUrl, $filename, $ReleaseVersion, $ReleaseDate)"/></xsl:attribute>
				</local:file>
			</xsl:for-each>
			</local:downloads>
			<local:timestamp><xsl:value-of select="$ReleaseDateTime"/></local:timestamp>
			<local:userFriendlyDate><xsl:value-of select="$ReleaseDate"/></local:userFriendlyDate>
		</local:previous>
	</xsl:template>
	
</xsl:transform>