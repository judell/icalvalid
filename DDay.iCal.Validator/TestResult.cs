using System;
using System.Collections.Generic;
using System.Text;

namespace DDay.iCal.Validator
{
    public class TestResult :
        ITestResult
    {
        #region Private Fields

        IResourceManager _ResourceManager;

        #endregion

        #region Constructors

        public TestResult(IResourceManager mgr, string rule, ITest test)
        {
            _ResourceManager = mgr;
            Rule = rule;
            Test = test;
        }

        public TestResult(IResourceManager mgr, string rule, ITest test, bool? passed)
            : this(mgr, rule, test)
        {
            Passed = passed;
        }

        public TestResult(IResourceManager mgr, string rule, ITest test, bool? passed, ITestError error) :
            this(mgr, rule, test, passed)
        {
            Error = error;
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Rule ?? "Unknown");
            sb.Append(": ");

            if (Passed != null && Passed.HasValue)
            {
                sb.Append(Passed.Value ? _ResourceManager.GetString("pass") : _ResourceManager.GetString("fail"));
            }
            else
            {
                sb.Append(_ResourceManager.GetString("didNotRun"));
            }

            if (Error != null)
                sb.Append(Environment.NewLine + Error.ToString());

            return sb.ToString();
        }

        #endregion

        #region ICalendarTestResult Members

        virtual public string Rule { get; protected set; }
        virtual public bool? Passed { get; set; }
        virtual public ITest Test { get; protected set; }
        virtual public ITestError Error { get; set; }

        #endregion
    }
}
