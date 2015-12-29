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
<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:local="http://local/">
	
	<xsl:output method="xml" indent="no" encoding="UTF-8" media-type="text/html" omit-xml-declaration="yes"/>
	<xsl:namespace-alias stylesheet-prefix="local" result-prefix="#default"/>
	<xsl:param name="ReleaseVersion"/>
	<xsl:param name="ReleaseDate"/>
	<xsl:param name="ProjectUrl"/>
	<xsl:param name="TextChangeLogUrl"/>
	
	<xsl:template match="/local:project">
<xsl:text disable-output-escaping='yes'>&lt;!DOCTYPE html&gt;
</xsl:text><html><xsl:text>
</xsl:text><head><xsl:text>
  </xsl:text><meta charset="utf-8"/><xsl:text>
  </xsl:text><link rel="stylesheet" href="changelog.css"/><xsl:text>
  </xsl:text><title><xsl:value-of select="local:title"/> Version History</title><xsl:text>
</xsl:text></head><xsl:text>
  </xsl:text><header><xsl:text>
    </xsl:text><h1><xsl:value-of select="local:title"/> Version History</h1><xsl:text>
    </xsl:text><a href="$ProjectUrl">back to project website</a><xsl:text>
  </xsl:text></header><xsl:text>
  </xsl:text><main><xsl:text>
    </xsl:text><table><xsl:text>
      </xsl:text><thead><xsl:text>
        </xsl:text><tr><xsl:text>
          </xsl:text><td class="Version">Version</td><xsl:text>
          </xsl:text><td class="Date">Release date</td><xsl:text>
          </xsl:text><td class="Filler"><xsl:text> </xsl:text></td><xsl:text>
        </xsl:text></tr><xsl:text>
      </xsl:text></thead><xsl:text>
      </xsl:text><tbody><xsl:text>
        </xsl:text><tr class="EntryHead"><xsl:text>
          </xsl:text><td class="Version"><xsl:value-of select="$ReleaseVersion"/></td><xsl:text>
          </xsl:text><td class="Date"><xsl:value-of select="$ReleaseDate"/></td><xsl:text>
          </xsl:text><td class="Filler"><xsl:text> </xsl:text></td><xsl:text>
        </xsl:text></tr><xsl:text>
        </xsl:text><tr class="EntryBody"><xsl:text>
          </xsl:text><td colspan="3"><ul><xsl:apply-templates select="local:history/local:current/local:change"/><xsl:text>
          </xsl:text></ul></td><xsl:text>
        </xsl:text></tr><xsl:for-each select="local:history/local:previous"><xsl:text>
        </xsl:text><tr class="EntryHead"><xsl:text>
          </xsl:text><td class="Version"><xsl:value-of select="local:version/text()"/></td><xsl:text>
          </xsl:text><td class="Date"><xsl:value-of select="local:userFriendlyDate/text()"/></td><xsl:text>
          </xsl:text><td class="Filler"><xsl:text> </xsl:text></td><xsl:text>
        </xsl:text></tr><xsl:text>
        </xsl:text><tr class="EntryBody"><xsl:text>
          </xsl:text><td colspan="3"><ul><xsl:apply-templates select="local:change"/><xsl:text>
          </xsl:text></ul></td><xsl:text>
        </xsl:text></tr></xsl:for-each><xsl:text>
      </xsl:text></tbody><xsl:text>
    </xsl:text></table><xsl:text>
  </xsl:text></main><xsl:text>
  </xsl:text><footer><xsl:text>
    </xsl:text><a href="$TextChangeLogUrl">download text version of this changelog</a><xsl:text>
  </xsl:text></footer><xsl:text>
</xsl:text></html>
	</xsl:template>
	
	<xsl:template match="local:change"><xsl:text>
            </xsl:text><li><xsl:value-of select="text()"/></li></xsl:template>
	
</xsl:transform>