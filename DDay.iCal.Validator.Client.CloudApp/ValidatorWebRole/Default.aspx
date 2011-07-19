<%@ Page Title="" ValidateRequest="false" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="ValidatorWebRole._Default" %>

<asp:Content ID="Title" ContentPlaceHolderID="Title" runat="server">
    iCalendar Validation
</asp:Content>
<asp:Content ID="Content" ContentPlaceHolderID="Content" runat="server">
    <div class="centered">
        <span class="title">i<span class="smallcaps">Calendar Validator</span></span><div class="right tiny smallcaps highlight bold">Beta</div>
    </div>
    <div class="centered tiny">
        Based on <a href="http://www.ddaysoftware.com/">DDay.iCal</a> version
        <%= DDayICalVersion %>
    </div>
    
    <noscript>
        <div class="border offBackground">
            <div class="centered error big bold">
                This site requires Javascript to work properly.
            </div>
            <div class="prespaced">
                This site makes extensive use of Javascript, and may behave erratically with it disabled.  
                Please enable Javascript to enjoy the full functionality of this service.
            </div>
        </div>
    </noscript>
    <div id="tabs" class="prespaced">
        <ul class="javascript tabs" style="display:none;">
            <li><a href="#tab-validation">Validation</a></li>
            <li><a href="#tab-about">About These Tools</a></li>
        </ul>
        <div id="tab-about" class="small">
            The iCalendar Validator is used to validate calendaring data against the iCalendar (RFC 5545) standard.
            <div class="prespaced">
                These tools can be used by producers of iCalendar data to help ensure your iCalendars
                are standards-compliant, and by consumers of iCalendar data to check for possible
                errors in the iCalendars they receive.
            </div>
            <div class="prespaced">
                The iCalendar Validator currently consists of a web service and this Web page, which
                conveniently submits data to that web service. This service uses the validation
                routines of <a href="http://www.ddaysoftware.com">DDay.iCal</a> to check iCalendar
                data for potential problems. Technically speaking, the word "validation" is somewhat
                incorrect, as there are no definitive rules we can follow to <span class="italic">guarantee</span>
                a calendar is valid (or invalid). Perhaps a better description would be "proofing"
                tools; however, for practical reasons I call it "validation".
            </div>
            <div class="prespaced">
                These tools were inspired by many things, namely:
            </div>
            <ul>
                <li>The <a href="http://severinghaus.org/projects/icv/">iCalendar Validator</a> by Steven
                    N. Severinghaus.</li>
                <li>The persistent efforts of Jon Udell at Microsoft.</li>
                <li>Aron Roberts' <a href="http://seaotter.berkeley.edu/calconnect/icalproofing/">mockup
                    validator.</a></li>
                <li>The general need for a more in-depth solution.</li>
                <li>Many others, including <a href="ical4j.sourceforge.net">Ben Fortuna</a>, DDay.iCal
                    developers and contributors, and the CalConnect group.</li>
            </ul>
        </div>
        <div id="tab-validation" class="small">
            <table>
                <tr>
                <td colspan="2">
                <p id="progress">&nbsp;</p>
                </td>
                </tr>
                <tr>
                    <td align="right" class="vtop bold">
                        <span class="u">U</span>RL:
                    </td>
                    <td class="vtop">
                        <div class="left">
                            <asp:TextBox AccessKey="u" ID="tbUrl" size="44" class="text" runat="server"></asp:TextBox>
                        </div>
                        <div class="left">
                            <asp:Button ID="btnValidateUrl" runat="server" Text="Validate" OnClick="btnValidateUrl_Click" />
                        </div>
                    </td>
                </tr>
                <tr>
                    <td align="right" class="vtop bold">
                        <span class="u">F</span>ile:
                    </td>
                    <td class="vtop">
                        <div class="left">
                            <asp:FileUpload AccessKey="f" ID="fileUpload" size="50" runat="server" class="multi" maxlength="1" />
                        </div>
                        <div class="left">
                            <asp:Button ID="btnUpload" runat="server" Text="Validate" OnClick="btnUpload_Click" />
                        </div>
                    </td>
                </tr>
                <tr>
                    <td align="right" class="vtop bold">
                        <span class="u">S</span>nippet:
                    </td>
                    <td class="vtop">
                        <div class="left small">
                            <asp:TextBox AccessKey="s" Rows="12" Columns="70" class="code" ID="tbSnippet" TextMode="MultiLine" runat="server" />
                        </div>
                        <div class="left">
                            <asp:Button ID="btnValidateSnippet" runat="server" Text="Validate" OnClick="btnValidateSnippet_Click" />
                        </div>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <table style="width:100%;" class="prespaced">
        <tr>            
            <td class="even50 bold centered tiny">
                Copyright © 2010 Douglas Day
            </td>
            <td class="even50 centered tiny">
                <div>For feedback, bug reports, or other comments,</div>
                <div>please e-mail <a href="doug(at)ddaysoftware.com" class="email">doug(at)ddaysoftware.com</a>.</div>
            </td>
        </tr>
    </table>

</asp:Content>
