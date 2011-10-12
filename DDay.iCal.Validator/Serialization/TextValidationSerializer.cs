using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using DDay.iCal.Validator.Xml;

namespace DDay.iCal.Validator.Serialization
{
    public class TextValidationSerializer :
        IValidationSerializer
    {
        #region Constructors

        public TextValidationSerializer(IResourceManager mgr)
        {
            ResourceManager = mgr;
        }

        public TextValidationSerializer(
            IResourceManager mgr,
            IValidationRuleset ruleset,
            ITestResult[] testResults) :
            this(mgr)
        {
            Ruleset = ruleset;
            TestResults = testResults;
        }

        public TextValidationSerializer(
            IResourceManager mgr,
            IValidationRuleset ruleset,
            IValidationResultCollection validationResults) :
            this(mgr)
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
            get { return "txt"; }
        }

        public Encoding DefaultEncoding
        {
            get { return Encoding.Unicode; }
        }

        virtual public IValidationRuleset Ruleset { get; set; }
        virtual public ITestResult[] TestResults { get; set; }
        virtual public IValidationResultCollection ValidationResults { get; set; }

        virtual public void Serialize(Stream stream, Encoding encoding, string permalink)
        {
            using (StreamWriter sw = new StreamWriter(stream, encoding))
            {
                sw.WriteLine();
                if (Ruleset != null)
                {
                    if (TestResults != null &&
                        TestResults.Length > 0)
                    {
                        int numTests = TestResults.Length;
                        int numTestsRun = 0;
                        foreach (ITestResult result in TestResults)
                        {
                            if (result.Passed != null && result.Passed.HasValue)
                                numTestsRun++;
                        }

                        if (!object.Equals(numTests, numTestsRun))
                        {
                            sw.WriteLine(string.Format(
                                ResourceManager.GetString("notAllTestsPerformed"),
                                numTests,
                                numTestsRun,
                                numTests - numTestsRun
                            ));
                        }

                        int passed = 0;
                        foreach (ITestResult result in TestResults)
                        {
                            if (BoolUtil.IsTrue(result.Passed))
                                passed++;

                            sw.WriteLine(result.ToString());
                        }

                        sw.WriteLine(string.Format(
                            ResourceManager.GetString("passVsFail"),
                            passed,
                            numTestsRun,
                            string.Format("{0:0.0}", ((double)passed / (double)numTestsRun) * 100)
                        ));
                    }
                    else if (TestResults != null)
                    {
                        sw.WriteLine(ResourceManager.GetString("noTestsPerformed"));
                    }
                    
                    if (ValidationResults != null)
                    {
                        if (BoolUtil.IsTrue(ValidationResults.Passed))
                        {
                            sw.WriteLine(ResourceManager.GetString("calendarValid"));
                        }
                        else
                        {
                            foreach (IValidationResultInfo info in ValidationResults.Details)
                            {
                                sw.WriteLine(info.ToString());
                            }
                        }
                    }
                }
                else
                {
                    sw.WriteLine(ResourceManager.GetString("noValidationRuleset"));
                }
            }
        }

        #endregion
    }
}
