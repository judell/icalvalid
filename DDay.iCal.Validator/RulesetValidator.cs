using System;
using System.Collections.Generic;
using System.Text;
using DDay.iCal.Serialization;
using System.IO;
using System.Xml;
using System.Reflection;
using DDay.iCal.Serialization.iCalendar;

namespace DDay.iCal.Validator.RFC5545
{
    public class RulesetValidator :
        Validation,
        ITestable
    {
        #region Public Events

        #region TestCompleted Event

        public event EventHandler<TestCompletedEventArgs> TestCompleted;

        protected void OnTestCompleted(ITestResult result)
        {
            if (TestCompleted != null)
                TestCompleted(this, new TestCompletedEventArgs(result));
        }

        #endregion

        #region TestProgress Event

        public event EventHandler<TestProgressEventArgs> TestProgress;
        
        TestProgressEventArgs _CurrentTestProgress;

        protected void OnTestStart(int totalTests)
        {
            _CurrentTestProgress = new TestProgressEventArgs(totalTests, 0, 0, 0);
            if (TestProgress != null)
                TestProgress(this, _CurrentTestProgress);
        }

        protected void OnTestPassed(ITestResult result)
        {
            OnTestCompleted(result);

            _CurrentTestProgress.Passed++;
            if (TestProgress != null)
                TestProgress(this, _CurrentTestProgress);
        }

        protected void OnTestFailed(ITestResult result)
        {
            OnTestCompleted(result);

            _CurrentTestProgress.Failed++;
            if (TestProgress != null)
                TestProgress(this, _CurrentTestProgress);
        }

        protected void OnTestNotRun(ITestResult result)
        {
            OnTestCompleted(result);

            _CurrentTestProgress.NotRun++;
            if (TestProgress != null)
                TestProgress(this, _CurrentTestProgress);
        }

        #endregion

        #endregion

        #region Private Fields

        private string _iCalendarText;
        private List<IValidationResultInfo> _MissingValidators = new List<IValidationResultInfo>();
        private IValidationRuleset _Ruleset;

        // FIXME: should we create this here?
        private Debug Debug = new Debug();

        #endregion

        #region Public Properties

        public string iCalendarText
        {
            get { return _iCalendarText; }
            set
            {
                if (!object.Equals(_iCalendarText, value))
                {
                    _iCalendarText = value;

                    try
                    {
                        Calendars = DDay.iCal.iCalendar.LoadFromStream(new StringReader(_iCalendarText));
                    }
                    catch (antlr.RecognitionException ex)
                    {                        
                    }
                }
            }            
        }
        public IICalendarCollection Calendars { get; set; }
        public IValidationRuleset Ruleset { get { return _Ruleset; } }

        #endregion

        #region Constructors

        public RulesetValidator(IResourceManager mgr, IValidationRuleset ruleset) :
            base(mgr)
        {
            _Ruleset = ruleset;
        }

        public RulesetValidator(IResourceManager mgr, IValidationRuleset ruleset, string text) :
            this(mgr, ruleset, text, false)
        {
        }

        public RulesetValidator(IResourceManager mgr, IValidationRuleset ruleset, string text, bool verifyValidatorsExist) :
            this(mgr, ruleset)
        {
            iCalendarText = text;

            if (verifyValidatorsExist)
                VerifyValidatorsExist();
        }

        #endregion

        #region Private Methods

        void VerifyValidatorsExist()
        {
            _MissingValidators.Clear();

            Debug.WriteLineWithTimestamp("Verifying each rule has a corresponding validator...");
            // We've passed a basic parsing test, let's move on...
            // Let's ensure each validator for the ruleset can be loaded.
            // We've passed a basic parsing test, let's move
            // on to the more complex tests!                
            foreach (IValidationRule rule in Ruleset.Rules)
            {
                IValidator validator = null;

                Type validatorType = rule.ValidatorType;
                if (validatorType != null)
                    validator = ValidatorActivator.Create(validatorType, ResourceManager, Calendars, iCalendarText);

                if (validator == null)
                {
                    _MissingValidators.Add(
                        new ValidationError(
                            ResourceManager, 
                            null,
                            "Validator for rule '" + rule.Name + "' could not be determined!",
                            ValidationResultInfoType.Error,
                            true
                        )
                    );
                }
            }
            Debug.WriteLineWithTimestamp("Done.");
        }

        #endregion

        #region IValidator Members

        public override IValidationResultCollection Validate()
        {
            if (Ruleset != null)
            {
                Debug.WriteLineWithTimestamp("Validating...");
                ValidationResultCollection results = new ValidationResultCollection(ResourceManager, "all");
                foreach (IValidationRule rule in Ruleset.Rules)
                {
                    IValidator validator = null;

                    Type validatorType = rule.ValidatorType;
                    if (validatorType != null)
                        validator = ValidatorActivator.Create(validatorType, ResourceManager, Calendars, iCalendarText);

                    if (validator != null)
                    {
                        IValidationResultCollection currentResults = validator.Validate();
                        results.Add(currentResults);

                        // Determine if there were any fatal errors in the results.
                        // If there are, then we need to abort any further processing!
                        if (results.IsFatal)
                            break;
                    }
                }
                Debug.WriteLineWithTimestamp("Done.");

                // We at least gave the validators a chance to handle the error in
                // a different manner.  If they haven't produced a more specific
                // error, then we'll fall back to the basic "parsing" error.
                if (BoolUtil.IsTrue(results.Passed))
                {
                    if (_MissingValidators.Count > 0)
                    {
                        results.Passed = false;
                        foreach (IValidationResultInfo error in _MissingValidators)
                            results.Add(error);
                    }
                }
                                
                Debug.Flush();
                return results;
            }
            else return null;
        }

        #endregion

        #region ICalendarTestable Members

        public ITest[] Tests
        {
            get
            {
                if (Ruleset != null)
                {
                    List<ITest> tests = new List<ITest>();
                    foreach (IValidationRule rule in Ruleset.Rules)
                    {
                        foreach (ITest test in rule.Tests)
                            tests.Add(test);
                    }

                    return tests.ToArray();
                }
                return new ITest[0];
            }
        }

        public ITestResult[] Test()
        {
            if (Ruleset != null)
            {
                List<ITestResult> results = new List<ITestResult>();

                int testsToRun = 0;
                foreach (IValidationRule rule in Ruleset.Rules)
                {
                    if (rule.Tests != null)
                        testsToRun += rule.Tests.Length;
                }

                OnTestStart(testsToRun);
                
                bool validatorsVerified = false;
                foreach (IValidationRule rule in Ruleset.Rules)
                {
                    if (rule.Tests != null)
                    {
                        foreach (ITest test in rule.Tests)
                        {
                            Debug.WriteLineWithTimestamp("Loading validators for test...");

                            // Load the ruleset validator using the test data.
                            RulesetValidator validator = new RulesetValidator(ResourceManager, Ruleset, test.iCalendarText, !validatorsVerified);
                            validatorsVerified = true;

                            Debug.WriteLineWithTimestamp("Done.");

                            // Validate the calendar!
                            IValidationResultCollection validationResults = validator.Validate();
                            TestResult result = new TestResult(ResourceManager, rule.Name, test, false);

                            Debug.WriteLineWithTimestamp("Interpreting test results...");

                            // Interpret the results...
                            if (test.Type == TestType.Fail)
                            {
                                if (BoolUtil.IsTrue(validationResults.Passed))
                                {
                                    // The validation passed, but the test expected it to fail.
                                    TestError error = new TestError(ResourceManager, "failExpectedError", rule.Name, validationResults);
                                    error.Message = string.Format(error.Message, test.ExpectedError);
                                    result.Error = error;
                                }
                                else
                                {
                                    IValidationResultInfo[] details = validationResults.Details;
                                    if (details.Length == 1 && !string.Equals(details[0].Name, test.ExpectedError))
                                    {
                                        // Validation failed (as expected), however, it failed with the incorrect error.
                                        TestError error = new TestError(ResourceManager, "failWithIncorrectError", rule.Name, validationResults);
                                        error.Message = string.Format(error.Message, details[0].Name, test.ExpectedError);
                                        result.Error = error;
                                    }
                                    else if (details.Length > 1)
                                    {
                                        // Validation failed (as expected), however, it failed with more than one error.
                                        TestError error = new TestError(ResourceManager, "failWithMoreThanOneError", rule.Name, validationResults);
                                        error.Message = string.Format(error.Message, test.ExpectedError);
                                        result.Error = error;
                                    }
                                    else
                                    {
                                        // The test passed, meaning the result was exactly as expected.
                                        result.Passed = true;
                                    }
                                }
                            }
                            else
                            {
                                if (BoolUtil.IsTrue(validationResults.Passed))
                                {
                                    // The test passed, meaning the result was exactly as expected.
                                    result.Passed = true;
                                }
                                else
                                {
                                    // The validation was expected to succeed, but one or more errors occurred.
                                    result.Passed = false;
                                    result.Error = new TestError(ResourceManager, "passExpectedError", rule.Name, validationResults);
                                }
                            }

                            Debug.WriteLineWithTimestamp("Done.");

                            // Notify via event the result of the test.
                            if (result.Passed == null)
                                OnTestNotRun(result);
                            else if (BoolUtil.IsTrue(result.Passed))
                                OnTestPassed(result);
                            else
                                OnTestFailed(result);

                            results.Add(result);

                            // If we encounter a fatal error, then we cannot continue processing
                            // other errors. In other words, even though a single error was caused,
                            // the fact that it was fatal may have side effects and cause errors
                            // in almost every validator (i.e. a calendar with parsing errors).
                            if (validationResults.IsFatal)
                                break;
                        }
                    }
                }

                return results.ToArray();
            }
            // FIXME: else throw exception?
            else return new ITestResult[0];
        }

        #endregion
    }
}
