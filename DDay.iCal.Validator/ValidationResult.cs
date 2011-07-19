using System;
using System.Collections.Generic;
using System.Text;

namespace DDay.iCal.Validator
{
    public class ValidationResult :
        IValidationResult
    {
        #region Static Public Methods

        static public IValidationResultCollection GetCompositeResults(IResourceManager mgr, params IValidator[] validators)
        {
            return GetCompositeResultsPrivate(mgr, null, validators);
        }

        static public IValidationResultCollection GetCompositeResults(IResourceManager mgr, string rule, params IValidator[] validators)
        {
            return GetCompositeResultsPrivate(mgr, rule, validators);
        }

        static private IValidationResultCollection GetCompositeResultsPrivate(IResourceManager mgr, string rule, IValidator[] validators)
        {
            ValidationResultCollection results = new ValidationResultCollection(mgr, rule);
            foreach (IValidator validator in validators)
                results.Add(validator.Validate());
            return results;
        }

        #endregion

        #region Protected Properties

        protected IResourceManager ResourceManager { get; set; }

        #endregion

        #region Constructors

        public ValidationResult(IResourceManager mgr)
        {
            ResourceManager = mgr;
        }

        public ValidationResult(IResourceManager mgr, bool? passed, IValidationResultInfo[] details) :
            this(mgr)
        {
            Passed = passed;
            Details = details;
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (Passed != null)
            {
                if (BoolUtil.IsTrue(Passed))
                    sb.Append(ResourceManager.GetString("pass"));
                else
                    sb.Append(ResourceManager.GetString("fail"));
            }
            else sb.Append(ResourceManager.GetString("didNotRun"));

            return sb.ToString();
        }

        #endregion

        #region IValidationResult Members

        virtual public string Rule { get; set; }
        virtual public bool? Passed { get; set; }
        virtual public IValidationResultInfo[] Details { get; set; }

        #endregion
    }
}
