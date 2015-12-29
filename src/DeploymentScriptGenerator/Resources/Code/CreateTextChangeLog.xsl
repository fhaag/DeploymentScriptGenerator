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
<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:local="http://local/" xmlns:str="urn:str">
	
	<xsl:output method="text" indent="no" encoding="UTF-8" media-type="text/plain"/>
	<xsl:param name="ReleaseVersion"/>
	<xsl:param name="ReleaseDate"/>
	
	<xsl:template match="/local:project"><xsl:value-of select="$ReleaseVersion"/> (<xsl:value-of select="$ReleaseDate"/>)
<xsl:value-of select="str:Underline(string-length($ReleaseVersion) + string-length($ReleaseDate) + 3, '=')"/>
<xsl:apply-templates select="local:history/local:current/local:change"/><xsl:for-each select="local:history/local:previous"><xsl:text>

</xsl:text><xsl:value-of select="local:version/text()"/> (<xsl:value-of select="local:userFriendlyDate/text()"/>)
<xsl:value-of select="str:Underline(string-length(local:version/text()) + string-length(local:userFriendlyDate/text()) + 3, '=')"/>
<xsl:apply-templates select="local:change"/>
</xsl:for-each></xsl:template>
	
	<xsl:template match="local:change">
- <xsl:value-of select="text()"/></xsl:template>
	
</xsl:transform>