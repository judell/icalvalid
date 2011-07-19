<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet
    version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:fn="http://www.w3.org/2005/02/xpath-functions"
    xmlns:i="http://icalvalid.wikidot.com/validation">
    <xsl:output
        indent="yes"
        doctype-public="-//W3C//DTD HTML 4.01//EN"
        doctype-system="http://www.w3.org/TR/html4/strict.dtd"/>

    <xsl:key name="errorKey" match="//i:info" use="@name"/>    

    <xsl:template match="/i:results">
        <html>
            <head>
                <link rel="stylesheet"
                      type="text/css"
                      href="results.css" />
                <link type="text/css"
                      href="css/smoothness/jquery-ui-1.7.2.custom.css"
                      rel="stylesheet" />
                <script type="text/javascript" src="js/jquery-1.3.2.min.js"></script>
                <script type="text/javascript" src="js/jquery-ui-1.7.2.custom.min.js"></script>
                <script type="text/javascript" src="js/jquery.qtip-1.0.0-rc3.min.js"></script>
                <script type="text/javascript" src="js/jquery.dimensions.min.js"></script>
                <script type="text/javascript">
                    // Create the tooltips on document load
                    $(document).ready(function()
                    {
                        $('a[href][title]').not('a[href][title].right, a[href][title].titleright').qtip({
                            content: {
                                text: false // Use each elements title attribute
                            },
                            style:
                            {                                
                                name: 'light', // inherit from 'light'
                            },
                            width: { max: 350 },
                            position:
                            {
                                corner:
                                {
                                    target:'bottomLeft',
                                    tooltip:'topLeft'
                                }
                            },
                            show:
                            {
                                delay: 300
                            }
                        });
                        
                        $('a[href][title].right, a[href][title].titleright').qtip({
                            content: {
                                text: false // Use each elements title attribute
                            },
                            style:
                            {                                
                                name: 'light', // inherit from 'light'
                            },
                            width: { max: 350 },
                            position:
                            {
                                corner:
                                {
                                    target:'bottomRight',
                                    tooltip:'topRight'
                                }
                            },
                            show:
                            {
                                delay: 300
                            }
                        });

                        $('#tabs').tabs();
                        
                        // Hide items that display differently when javascript is enabled.
                        $('.noscript').css('display', 'none');
                        
                        // Show items that aren't displayed unless javascript is enabled.
                        $('.javascript').css('display', 'block');
                    });
                    
                    function toggle(obj)
                    {
                        var elm = obj[0];
                        if (elm.style.display == 'none')
                            obj.show();
                        else
                            obj.hide();
                    }
                </script>
            </head>
            <body>
                <div class="results prespaced font">
                    <div class="title">Calendar Validation Results</div>
                    <div class="border prespaced postspaced mainBackground">
                        <div class="small centered">
                            <xsl:if test="i:validationResults/@validationPath">
                                <div>The following calendar was validated:</div>
                                <div class="big bold highlight"><xsl:value-of select="i:validationResults/@validationPath" /></div>
                            </xsl:if>
                            This calendar was validated with the<br />
                            <a href="#"
                               onclick="return false;"
                               class="anchor bold italic big">
                                <xsl:attribute name="title">
                                    <xsl:value-of select="i:validationResults/@rulesetDescription"/>
                                </xsl:attribute>
                                <xsl:value-of select="i:validationResults/@rulesetString"/>
                            </a> ruleset.
                        </div>
                    </div>

                  <p style="text-align:center">
                    <a href="/">Do Another</a>
                  </p>

                  <xsl:if test="count(i:validationResults/i:validationResult[@result='none'])>0">
                        <div class="warning subtitle bold">
                            Due to major errors, some validation was not performed.  Please correct the errors and validate again.
                        </div>
                    </xsl:if>

                    <div id="tabs">
                        <ul class="javascript tabs" style="display:none;">
                            <xsl:call-template name="tabheaders" />
                        </ul>
                        <xsl:call-template name="tabcontents" />
                    </div>

                    <xsl:if test="count(i:using)>0">
                        <div class="tiny prespaced"
                             style="text-align:right;">
                            This report was created using:
                            <xsl:variable name="usingCount" select="count(i:using)" />
                            <xsl:for-each select="i:using">
                                <xsl:choose>
                                    <xsl:when test="position()>1 and position()=$usingCount">
                                        <xsl:if test="$usingCount > 2">,</xsl:if>
                                        <xsl:text xml:space="preserve"> and </xsl:text>
                                    </xsl:when>
                                    <xsl:when test="position()>1">
                                        <xsl:text xml:space="preserve">, </xsl:text>
                                    </xsl:when>
                                </xsl:choose>
                                <a class="titleright">
                                    <xsl:attribute name="title">
                                        <xsl:value-of select="@name" />
                                        <xsl:if test="@version">
                                            <xsl:text xml:space="preserve"> </xsl:text>
                                            <xsl:value-of select="@version" />
                                        </xsl:if>
                                    </xsl:attribute>
                                    <xsl:if test="@url">
                                        <xsl:attribute name="href">
                                            <xsl:value-of select="@url" />
                                        </xsl:attribute>
                                    </xsl:if>
                                    <xsl:value-of select="@name" />
                                </a>
                            </xsl:for-each>.
                        </div>                        
                    </xsl:if>
                </div>
            </body>
        </html>
    </xsl:template>

    <xsl:template name="summary">
        <xsl:param name="header" select="false()" />
        <xsl:variable name="score" select="number(i:validationResults/@score)" />
        
        <xsl:choose>
            <xsl:when test="$header = true()">
                <li>
                    <a href="#tab-1" class="bold">Summary</a>
                </li>
            </xsl:when>
            <xsl:otherwise>
                <div id="tab-1">
                    <xsl:choose>                        
                        <xsl:when test="count(i:validationResults/i:calendarInfos/i:calendarInfo)>0">
                            <div class="border postspaced offBackground">
                                <div class="small centered">
                                    This calendar scored
                                </div>
                                <div class="big bold centered">
                                    <span>
                                        <xsl:choose>
                                            <xsl:when test="$score &lt;= 50">
                                                <xsl:attribute name="class">fatal</xsl:attribute>
                                            </xsl:when>
                                            <xsl:when test="$score &lt; 80">
                                                <xsl:attribute name="class">error</xsl:attribute>
                                            </xsl:when>
                                            <xsl:when test="$score &lt; 95">
                                                <xsl:attribute name="class">warning</xsl:attribute>
                                            </xsl:when>
                                            <xsl:otherwise>
                                                <xsl:attribute name="class">good</xsl:attribute>
                                            </xsl:otherwise>
                                        </xsl:choose>
                                        <xsl:choose>
                                            <xsl:when test="$score &gt; 0">
                                                <xsl:value-of select="format-number($score, '0.#')" />
                                            </xsl:when>
                                            <xsl:otherwise>
                                                0
                                            </xsl:otherwise>
                                        </xsl:choose>                                        
                                    </span>
                                    out of <span class="good">100</span>
                                </div>
                                <div class="small prespaced">
                                    <div class="centered">
                                        <xsl:choose>
                                            <xsl:when test="$score &lt;= 50">
                                                <xsl:attribute name="class">fatal centered</xsl:attribute>
                                                This calendar has severe problems; very few (if any) applications will accept this calendar.
                                            </xsl:when>
                                            <xsl:when test="$score &lt; 80">
                                                <xsl:attribute name="class">error centered</xsl:attribute>
                                                This calendar has major problems; many applications will reject this calendar.
                                            </xsl:when>
                                            <xsl:when test="$score &lt; 95">
                                                <xsl:attribute name="class">warning centered</xsl:attribute>
                                                This calendar has moderate problems, but may work correctly in some major calendar applications.
                                            </xsl:when>
                                            <xsl:when test="$score &lt;= 99.9">
                                                <xsl:attribute name="class">warning centered</xsl:attribute>
                                                This calendar has minor problems, but will likely work correctly in most major calendar applications.
                                            </xsl:when>
                                            <xsl:otherwise>
                                                <xsl:attribute name="class">good centered</xsl:attribute>
                                                This calendar has no known problems: most applications will accept this calendar. Hooray for you!
                                            </xsl:otherwise>
                                        </xsl:choose>
                                    </div>
                                </div>
                            </div>                            
                            <table style="width:100%;">
                                <xsl:if test="string-length(/i:results/i:calendarLines/@path)>0">
                                    <xsl:call-template name="tableRow">
                                        <xsl:with-param name="c1">Calendar Path:</xsl:with-param>
                                        <xsl:with-param name="c2"
                                                        select="i:calendarLines/@path" />
                                    </xsl:call-template>
                                </xsl:if>
                                <xsl:call-template name="tableRow">
                                    <xsl:with-param name="c1">Calendar Size:</xsl:with-param>
                                    <xsl:with-param name="c2">
                                        <xsl:call-template name="byteCountConverter">
                                            <xsl:with-param name="count"
                                                            select="number(/i:results/i:calendarLines/@byteCount)" />
                                        </xsl:call-template>
                                    </xsl:with-param>
                                </xsl:call-template>
                                <xsl:for-each select="i:validationResults/i:calendarInfos/i:calendarInfo">
                                    <tr>
                                        <td class="vtop even50">
                                            <table class="font tiny">
                                                <caption>Calendar Info</caption>                                                
                                                <xsl:if test="string-length(@calendarVersion)>0">
                                                    <xsl:call-template name="tableRow">
                                                        <xsl:with-param name="c1">Calendar Version:</xsl:with-param>
                                                        <xsl:with-param name="c2"
                                                                        select="@calendarVersion" />
                                                    </xsl:call-template>
                                                </xsl:if>
                                                <xsl:if test="string-length(@prodID)>0">
                                                    <xsl:call-template name="tableRow">
                                                        <xsl:with-param name="c1">Product ID:</xsl:with-param>
                                                        <xsl:with-param name="c2"
                                                                        select="@prodID" />
                                                    </xsl:call-template>
                                                </xsl:if>
                                            </table>
                                        </td>
                                        <td class="vtop even50">
                                            <table class="font tiny">
                                                <caption>Statistics</caption>
                                                <xsl:call-template name="tableRow">
                                                    <xsl:with-param name="c1">Time Zones:</xsl:with-param>
                                                    <xsl:with-param name="c2"
                                                                    select="@timeZoneCount" />
                                                </xsl:call-template>
                                                <xsl:call-template name="tableRow">
                                                    <xsl:with-param name="c1">Events:</xsl:with-param>
                                                    <xsl:with-param name="c2"
                                                                    select="@eventCount" />
                                                </xsl:call-template>
                                                <xsl:call-template name="tableRow">
                                                    <xsl:with-param name="c1">Todos:</xsl:with-param>
                                                    <xsl:with-param name="c2"
                                                                    select="@todoCount" />
                                                </xsl:call-template>
                                                <xsl:call-template name="tableRow">
                                                    <xsl:with-param name="c1">Journal Entries:</xsl:with-param>
                                                    <xsl:with-param name="c2"
                                                                    select="@journalCount" />
                                                </xsl:call-template>
                                                <xsl:call-template name="tableRow">
                                                    <xsl:with-param name="c1">Free/Busy Entries:</xsl:with-param>
                                                    <xsl:with-param name="c2"
                                                                    select="@freeBusyCount" />
                                                </xsl:call-template>
                                            </table>
                                        </td>
                                    </tr>
                                </xsl:for-each>
                            </table>
                        </xsl:when>
                        <xsl:otherwise>
                            <div class="small border postspaced offBackground centered">
                                Due to a serious error, some information about the calendar could not be collected.
                                <div class="innerBorder mainBackground error bold centered">
                                    See 'Errors' and 'Serious Errors' for more information.
                                </div>
                            </div>
                        </xsl:otherwise>
                    </xsl:choose>                    
                </div>
            </xsl:otherwise>
        </xsl:choose>        
    </xsl:template>

    <xsl:template name="tableRow">
        <xsl:param name="c1" />
        <xsl:param name="c2" />
        <tr>
            <td align="right" class="even50 vtop">
                <xsl:value-of select="$c1"/>
            </td>
            <td class="even50 vtop">
                <xsl:value-of select="$c2"/>
            </td>
        </tr>
    </xsl:template>

    <xsl:template name="byteCountConverter">
        <xsl:param name="count" />

        <xsl:choose>
            <xsl:when test="$count &gt; 1073741824">
                <xsl:value-of select="format-number($count div 1073741824, '0.0#')"/>Gb
            </xsl:when>
            <xsl:when test="$count &gt; 1048576">
                <xsl:value-of select="format-number($count div 1048576, '0.0#')"/>Mb
            </xsl:when>
            <xsl:when test="$count &gt; 1024">
                <xsl:value-of select="format-number($count div 1024, '0.0#')"/>Kb
            </xsl:when>
            <xsl:otherwise>
                <xsl:value-of select="$count"/> bytes
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template>

    <xsl:template name="tabheaders">
        <xsl:call-template name="summary">
            <xsl:with-param name="header" select="true()" />
        </xsl:call-template>
        <xsl:apply-templates select="i:calendarLines">
            <xsl:with-param name="header"
                            select="true()" />
        </xsl:apply-templates>
        <xsl:apply-templates select="i:validationResults">
            <xsl:with-param name="header"
                            select="true()" />
        </xsl:apply-templates>        
    </xsl:template>

    <xsl:template name="tabcontents">
        <xsl:call-template name="summary" />
        <xsl:apply-templates select="i:calendarLines" />
        <xsl:apply-templates select="i:validationResults" />        
    </xsl:template>

    <xsl:template name="errorList"
                  match="i:validationResults">
        <xsl:param name="header"
                   select="false()" />
        <xsl:param name="type"
                   select="error" />
        <xsl:param name="typeCaps"
                   select="Error" />
        <xsl:param name="info" />

        <xsl:choose>
            <xsl:when test="$header = true()">
                <li>
                    <a
                        href="#tab-{$type}"
                        class="bold">
                        <span class="{$type} font">
                            <xsl:value-of select="count(i:validationResult/i:details/i:info[@type=$type])"/>
                            <xsl:text xml:space="preserve"> </xsl:text>
                            <xsl:value-of select="$typeCaps"/>
                            <xsl:if test="count(i:validationResult/i:details/i:info[@type=$type])!=1">s</xsl:if>
                        </span>
                    </a>
                </li>
            </xsl:when>
            <xsl:otherwise>
                <div id="tab-{$type}">
                    <xsl:choose>
                        <xsl:when test="count(i:validationResult/i:details/i:info[@type=$type])>0">                            
                            <div class="border postspaced offBackground">
                                <table class="font">
                                    <tr>
                                        <td class="vtop">
                                            <img
                                                src="images/{$type}.png"
                                                class="{$type}" />
                                        </td>
                                        <td class="small vtop">
                                            <xsl:value-of select="$info" />
                                        </td>
                                    </tr>
                                </table>
                            </div>

                            <table
                                class="errorList mainBackground font small"
                                cellspacing="0">
                                <tr>
                                    <th>Line</th>
                                    <th>Column</th>
                                    <th>Message</th>
                                    <th>Resolution</th>
                                </tr>

                                <xsl:for-each select="//i:info[@type=$type and generate-id()=generate-id(key('errorKey',@name)[1])]">
                                    <xsl:sort select="@line" data-type="number"/>
                                    <xsl:variable name="name" select="@name" />
                                    <xsl:variable name="key" select="generate-id()" />
                                    <xsl:variable name="othersLikeThisCount" select="count(//i:info[@type=$type and @name=$name])-1" />
                                    <xsl:apply-templates select=".">
                                        <xsl:with-param name="name"
                                                        select="$name" />
                                        <xsl:with-param name="othersLikeThisCount"
                                                        select="$othersLikeThisCount" />
                                        <xsl:with-param name="key"
                                                        select="$key" />
                                    </xsl:apply-templates>
                                </xsl:for-each>
                            </table>
                        </xsl:when>
                        <xsl:otherwise>
                            There are no <xsl:value-of select="$typeCaps"/>s.
                        </xsl:otherwise>
                    </xsl:choose>
                </div>
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template>

    <xsl:template match="i:validationResults">
        <xsl:param name="header"
                   select="false()" />
        <xsl:if test="count(i:validationResult[@result='fail'])>0">
            <xsl:call-template name="errorList">
                <xsl:with-param name="header"
                                select="$header" />                
                <xsl:with-param name="type">warning</xsl:with-param>
                <xsl:with-param name="typeCaps">Warning</xsl:with-param>
                <xsl:with-param name="info">
                    Warnings are items that have compatibility issues.  Some applications may accept this iCalendar, while others won't.  For maximum compatibility, it is recommended to correct all warnings.
                </xsl:with-param>
            </xsl:call-template>

            <xsl:call-template name="errorList">
                <xsl:with-param name="header"
                                select="$header" />                
                <xsl:with-param name="type">error</xsl:with-param>
                <xsl:with-param name="typeCaps">Error</xsl:with-param>
                <xsl:with-param name="info">
                    Errors are items that &quot;break&quot; most systems.  All errors should be corrected.
                </xsl:with-param>
            </xsl:call-template>

            <xsl:if test="count(i:validationResult/i:details/i:info[@type='fatal'])>0">
                <xsl:call-template name="errorList">
                    <xsl:with-param name="header"
                                    select="$header" />
                    <xsl:with-param name="type">fatal</xsl:with-param>
                    <xsl:with-param name="typeCaps">Serious Error</xsl:with-param>
                    <xsl:with-param name="info">
                        Serious errors are errors that must be corrected for the calendar to work.  Very few systems will accept a calendar that contains serious errors.
                    </xsl:with-param>
                </xsl:call-template>
            </xsl:if>
        </xsl:if>
    </xsl:template>

    <xsl:template match="i:info">
        <xsl:param name="name" />
        <xsl:param name="othersLikeThisCount" />
        <xsl:param name="key" />
        <xsl:param name="type" select="@type" />
        
        <tr>
            <td class="centered vtop">
                <a href="#line{@line}"
                   onclick="$('#tabs').tabs('select', 'tablinebyline');">
                    <xsl:value-of select="@line"/>
                </a>
            </td>
            <td class="centered vtop">
                <xsl:value-of select="@column"/>
            </td>
            <td class="vtop">
                <div>
                    <xsl:attribute name="class">
                        <xsl:value-of select="@type"/>
                    </xsl:attribute>
                    <xsl:if test="@isFatal='true'">
                        <xsl:attribute name="class">bold error</xsl:attribute>
                    </xsl:if>
                    <xsl:value-of select="@message"/>
                </div>

                <xsl:if test="$othersLikeThisCount &gt; 0">
                    <div id="{$name}-show-more-errors" class="border prespaced postspaced centered offBackground bold small javascript" style="display:none;">
                        <div>
                            <a class="highlight"
                               href="#"
                               onclick="$('#{$name}-more-errors').slideToggle('slow'); return false;">
                                There 
                                <xsl:choose>
                                    <xsl:when test="$othersLikeThisCount=1">is</xsl:when>
                                    <xsl:otherwise>are</xsl:otherwise>
                                </xsl:choose><xsl:text xml:space="preserve"> </xsl:text>
                                <span class="{@type}"><xsl:value-of select="$othersLikeThisCount" /> </span>
                                similar <xsl:value-of select="@type" /><xsl:if test="$othersLikeThisCount!=1">s</xsl:if>.
                            </a>
                        </div>
                    </div>

                    <table
                        id="{$name}-more-errors"
                        class="noscript errorList mainBackground font small"                        
                        cellspacing="0">
                        <tr>
                            <th>Line</th>
                            <th>Column</th>
                            <th>Message</th>
                            <th>Resolution</th>
                        </tr>

                        <!-- Show similar errors, sorted by line number -->
                        <xsl:for-each select="//i:info[generate-id()!=$key and @name=$name and @type=$type]">
                            <xsl:sort select="@line" data-type="number"/>
                            <xsl:apply-templates select="." />                                
                        </xsl:for-each>
                    </table>
                </xsl:if>
            </td>
            <td class="centered">
                <xsl:for-each select="i:resolution">
                    <xsl:call-template name="resolutionLink" />
                </xsl:for-each>
            </td>
        </tr>
    </xsl:template>

    <xsl:template match="i:calendarLines">
        <xsl:param name="header" select="false()" />
        <xsl:param name="limit" select="@limit" />

        <xsl:choose>
            <xsl:when test="$header = true()">
                <li>
                    <a class="bold font"
                       href="#tablinebyline">Line-by-Line</a>
                </li>
            </xsl:when>
            <xsl:otherwise>
                <div
                    id="tablinebyline"
                    class="content">
                    <xsl:if test="count(i:line)&gt;=$limit">
                        <div class="border postspaced offBackground error">
                            Your calendar contains more than <xsl:value-of select="$limit" /> lines.
                            Only the first <xsl:value-of select="$limit" /> lines are displayed.
                        </div>
                    </xsl:if>

                    <table
                        align="center"
                        class="calendarLines mainBackground font">
                        <xsl:apply-templates select="i:line[position()&lt;=$limit]">
                            <xsl:sort select="@n" data-type="number" />
                        </xsl:apply-templates>
                    </table>
                </div>
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template>

    <xsl:template match="i:line">
        <xsl:variable name="lineNumber"
                      select="@n" />
        <tr>
            <td class="vcentered vtop pre" valign="top">
                <xsl:for-each select="//i:info[@line=$lineNumber]">
                    <xsl:call-template name="errorLink" />
                </xsl:for-each>
            </td>
            <td class="centered vtop" valign="top">
                <xsl:if test="//i:info[@type='warning' and @line=$lineNumber]">
                    <xsl:attribute name="class">centered warning</xsl:attribute>
                </xsl:if>
                <xsl:if test="//i:info[@type='error' and @line=$lineNumber]">
                    <xsl:attribute name="class">centered error</xsl:attribute>
                </xsl:if>
                <xsl:if test="//i:info[@type='fatal' and @line=$lineNumber]">
                    <xsl:attribute name="class">centered fatal</xsl:attribute>
                </xsl:if>
                <a name="line{@n}"
                   class="anchor">
                    <xsl:value-of select="@n"/>
                </a>
            </td>            
            <td class="vcentered vtop" valign="top">
                <xsl:if test="//i:info[@type='warning' and @line=$lineNumber]">
                    <xsl:attribute name="class">warning</xsl:attribute>
                </xsl:if>
                <xsl:if test="//i:info[@type='error' and @line=$lineNumber]">
                    <xsl:attribute name="class">error</xsl:attribute>
                </xsl:if>
                <xsl:if test="//i:info[@type='fatal' and @line=$lineNumber]">
                    <xsl:attribute name="class">fatal</xsl:attribute>
                </xsl:if>
                <pre>
                    <xsl:value-of select="." />
                </pre>
            </td>            
        </tr>
    </xsl:template>

    <xsl:template name="errorLink">
        <a href="#" onclick="return false;">
            <xsl:attribute name="title">
                <xsl:value-of select="@message" />
            </xsl:attribute>
            <img
                src="images/{@type}.png"
                border="0"
                class="{@type}" />
        </a>
    </xsl:template>

    <xsl:template name="resolutionLink">
        <a href="#" class="titleright" onclick="return false;">
            <xsl:attribute name="title">
                <xsl:value-of select="." />
            </xsl:attribute>
            <img
                src="images/info.png"
                border="0" 
                class="resolution" />
        </a>
    </xsl:template>
</xsl:stylesheet>
