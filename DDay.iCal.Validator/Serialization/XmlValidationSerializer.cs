using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using DDay.iCal.Validator.Xml;
using System.Reflection;

namespace DDay.iCal.Validator.Serialization
{
    public class XmlValidationSerializer :
        IValidationSerializer
    {
        #region Constructors

        public XmlValidationSerializer(
            IResourceManager mgr,
            IXmlDocumentProvider docProvider)
        {
            ResourceManager = mgr;
            XmlDocProvider = docProvider;
        }

        public XmlValidationSerializer(
            IResourceManager mgr,
            IXmlDocumentProvider docProvider,
            IValidationRuleset ruleset,
            ITestResult[] testResults) :
            this(mgr, docProvider)
        {
            Ruleset = ruleset;
            TestResults = testResults;
        }

        public XmlValidationSerializer(
            IResourceManager mgr,
            IXmlDocumentProvider docProvider,
            IValidationRuleset ruleset,
            IValidationResultCollection validationResults) :
            this(mgr, docProvider)
        {
            Ruleset = ruleset;
            ValidationResults = validationResults;
        }

        #endregion

        #region IValidationSerializer Members

        public IResourceManager ResourceManager { get; protected set; }
        public string ApplicationName { get; set; }
        public string ApplicationUrl { get; set; }
        public Version ApplicationVersion { get; set; }

        public string DefaultExtension
        {
            get { return "xml"; }
        }

        public Encoding DefaultEncoding
        {
            get { return Encoding.UTF8; }
        }

        virtual public IXmlDocumentProvider XmlDocProvider { get; set; }
        virtual public IValidationRuleset Ruleset { get; set; }
        virtual public ITestResult[] TestResults { get; set; }
        virtual public IValidationResultCollection ValidationResults { get; set; }

        virtual public void Serialize(Stream stream, Encoding encoding)
        {
            XmlWriter xw = null;
            try
            {
                if (XmlDocProvider != null)
                {
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.CloseOutput = false;
                    settings.ConformanceLevel = ConformanceLevel.Document;
                    settings.Encoding = encoding;
                    settings.Indent = true;
                    settings.IndentChars = "    ";
                    settings.NewLineChars = "\r\n";
                    settings.NewLineOnAttributes = true;

                    xw = XmlWriter.Create(stream, settings);
                    xw.WriteProcessingInstruction("xml-stylesheet", "type=\"text/xsl\" href=\"results.xslt\"");

                    XmlDocument doc = new XmlDocument();
                    XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
                    nsmgr.AddNamespace("i", "http://icalvalid.wikidot.com/validation");

                    // FIXME: add a schema!
                    // doc.Schemas.Add("http://icalvalid.wikidot.com/validation", "schema.xsd");

                    DateTime now = DateTime.Now;

                    XmlElement results = doc.CreateElement("results", "http://icalvalid.wikidot.com/validation");
                    results.SetAttribute("language", ResourceManager.CurrentLanguageIdentifier);
                    results.SetAttribute("datetime", now.ToString("yyyy-MM-dd") + "T" + now.ToString("hh:mm:ss"));

                    // Indicate what version of DDay.iCal we're using...
                    XmlElement @using = doc.CreateElement("using", "http://icalvalid.wikidot.com/validation");
                    @using.SetAttribute("name", "DDay.iCal");
                    @using.SetAttribute("url", "http://www.ddaysoftware.com/Pages/Projects/DDay.iCal/");
                    try
                    {
                        Assembly dday_ical = Assembly.Load("DDay.iCal");
                        @using.SetAttribute("version", dday_ical.GetName().Version.ToString());
                    }
                    catch
                    {                       
                    }
                    results.AppendChild(@using);

                    // Indicate what version of our current application we're using
                    if (ApplicationName != null)
                    {
                        @using = doc.CreateElement("using", "http://icalvalid.wikidot.com/validation");
                        @using.SetAttribute("name", ApplicationName);
                        if (ApplicationUrl != null)
                            @using.SetAttribute("url", ApplicationUrl);
                        if (ApplicationVersion != null)
                            @using.SetAttribute("version", ApplicationVersion.ToString());
                        results.AppendChild(@using);
                    }

                    if (TestResults != null &&
                        TestResults.Length > 0)
                    {
                        XmlElement testResults = doc.CreateElement("testResults", "http://icalvalid.wikidot.com/validation");
                        testResults.SetAttribute("ruleset", Ruleset.Name);

                        foreach (ITestResult result in TestResults)
                        {
                            XmlElement testResult = doc.CreateElement("testResult", "http://icalvalid.wikidot.com/validation");
                            testResult.SetAttribute("rule", result.Rule);
                            testResult.SetAttribute("expected", result.Test.Type == TestType.Pass ? "pass" : "fail");
                            if (result.Passed != null && result.Passed.HasValue)
                            {
                                string pass = (result.Test.Type == TestType.Pass ? "pass" : "fail");
                                string fail = (result.Test.Type == TestType.Pass ? "fail" : "pass");
                                testResult.SetAttribute("actual", BoolUtil.IsTrue(result.Passed) ? pass : fail);
                            }
                            else
                            {
                                testResult.SetAttribute("actual", "none");
                            }
                            
                            if (!string.IsNullOrEmpty(result.Test.ExpectedError))
                                testResult.SetAttribute("errorName", result.Test.ExpectedError);
                            if (result.Error != null)
                                testResult.SetAttribute("error", result.Error.ToString());

                            testResults.AppendChild(testResult);
                        }

                        results.AppendChild(testResults);
                    }

                    if (ValidationResults != null)
                    {
                        if (!string.IsNullOrEmpty(ValidationResults.CalendarText))
                        {
                            XmlElement calTextElm = doc.CreateElement("calendarLines", "http://icalvalid.wikidot.com/validation");
                            calTextElm.SetAttribute("path", ValidationResults.CalendarPath);
                            calTextElm.SetAttribute("byteCount", ValidationResults.ByteCount.ToString());
                            calTextElm.SetAttribute("limit", "500");

                            StringReader sr = new StringReader(ValidationResults.CalendarText);
                            string line = sr.ReadLine();
                            int lineNumber = 0;                            
                            while (line != null)
                            {
                                XmlElement calLineElm = doc.CreateElement("line", "http://icalvalid.wikidot.com/validation");                                
                                calLineElm.SetAttribute("n", (++lineNumber).ToString());                                
                                calLineElm.InnerText = line;
                                calTextElm.AppendChild(calLineElm);
                                line = sr.ReadLine();

                                // Limit to 500 lines, for speed purposes.
                                if (lineNumber >= 500)
                                    break;
                            }

                            results.AppendChild(calTextElm);
                        }

                        if (ValidationResults.Count > 0)
                        {
                            XmlElement validationResults = doc.CreateElement("validationResults", "http://icalvalid.wikidot.com/validation");
                            validationResults.SetAttribute("ruleset", Ruleset.Name);
                            validationResults.SetAttribute("rulesetString", ResourceManager.GetString("ruleset_" + Ruleset.Name));
                            validationResults.SetAttribute("rulesetDescription", ResourceManager.GetString("ruleset_" + Ruleset.Name + "_Description"));
                            validationResults.SetAttribute("score", ScoreCalculator.CalculateScore(ValidationResults).ToString());

                            try
                            {
                                // Attempt to gather some general information about the calendar...
                                IICalendarCollection calendars = iCalendar.LoadFromStream(new StringReader(ValidationResults.CalendarText));
                                if (calendars != null)
                                {
                                    XmlElement calendarInfos = doc.CreateElement("calendarInfos", "http://icalvalid.wikidot.com/validation");
                                    foreach (IICalendar iCal in calendars)
                                    {
                                        XmlElement calendarInfo = doc.CreateElement("calendarInfo", "http://icalvalid.wikidot.com/validation");
                                        calendarInfo.SetAttribute("timeZoneCount", iCal.TimeZones.Count.ToString());
                                        calendarInfo.SetAttribute("eventCount", iCal.Events.Count.ToString());
                                        calendarInfo.SetAttribute("todoCount", iCal.Todos.Count.ToString());
                                        calendarInfo.SetAttribute("journalCount", iCal.Journals.Count.ToString());
                                        calendarInfo.SetAttribute("freeBusyCount", iCal.FreeBusy.Count.ToString());
                                        calendarInfo.SetAttribute("calendarVersion", iCal.Version);
                                        calendarInfo.SetAttribute("prodID", iCal.ProductID);

                                        calendarInfos.AppendChild(calendarInfo);
                                    }

                                    validationResults.AppendChild(calendarInfos);
                                }
                            }
                            catch
                            {
                                // fail gracefully
                            }

                            List<string> errorNames = new List<string>();
                            foreach (IValidationResult result in ValidationResults)
                            {                                
                                XmlElement validationResult = doc.CreateElement("validationResult", "http://icalvalid.wikidot.com/validation");
                                validationResult.SetAttribute("rule", result.Rule);
                                if (result.Passed != null && result.Passed.HasValue)
                                    validationResult.SetAttribute("result", result.Passed.Value ? "pass" : "fail");
                                else
                                    validationResult.SetAttribute("result", "none");

                                if (result.Details != null &&
                                    result.Details.Length > 0)
                                {
                                    XmlElement validationDetails = doc.CreateElement("details", "http://icalvalid.wikidot.com/validation");
                                    foreach (IValidationResultInfo info in result.Details)
                                    {
                                        // Add the error to our list of errors
                                        if (!errorNames.Contains(info.Name))
                                            errorNames.Add(info.Name);

                                        XmlElement validationInfo = doc.CreateElement("info", "http://icalvalid.wikidot.com/validation");
                                        validationInfo.SetAttribute("name", info.Name);
                                        validationInfo.SetAttribute("type", info.Type.ToString().ToLower());
                                        validationInfo.SetAttribute("message", info.Message);
                                        if (info.Line != default(int))
                                            validationInfo.SetAttribute("line", info.Line.ToString());
                                        if (info.Col != default(int))
                                            validationInfo.SetAttribute("col", info.Col.ToString());
                                        if (info.Resolutions != null)
                                        {
                                            foreach (string resolution in info.Resolutions)
                                            {
                                                XmlElement resolutionNode = doc.CreateElement("resolution", "http://icalvalid.wikidot.com/validation");
                                                resolutionNode.InnerText = resolution;
                                                validationInfo.AppendChild(resolutionNode);
                                            }
                                        }

                                        validationDetails.AppendChild(validationInfo);
                                    }

                                    validationResult.AppendChild(validationDetails);
                                }

                                validationResults.AppendChild(validationResult);
                            }

                            results.AppendChild(validationResults);
                        }
                    }

                    doc.AppendChild(results);

                    doc.Save(xw);
                }
            }
            finally
            {
                if (xw != null)
                {
                    xw.Close();
                    xw = null;
                }
            }
        }

        #endregion
    }
}
