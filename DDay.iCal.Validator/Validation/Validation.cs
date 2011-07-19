using System;
using System.Collections.Generic;
using System.Text;

namespace DDay.iCal.Validator
{
    public abstract class Validation :
        IValidator
    {
        #region Constructors

        public Validation(IResourceManager mgr)
        {
            ResourceManager = mgr;
        }

        #endregion

        #region Protected Methods

        protected void Warning(ValidationResultCollection results, string errorName)
        {
            ErrorInternal(results, errorName, ValidationResultInfoType.Warning, 0, 0, null, null);
        }

        protected void Warning(ValidationResultCollection results, string errorName, int line, int col, params object[] objects)
        {
            ErrorInternal(results, errorName, ValidationResultInfoType.Warning, line, col, objects);
        }

        protected void Error(ValidationResultCollection results, string errorName)
        {
            ErrorInternal(results, errorName, ValidationResultInfoType.Error, 0, 0, null, null);
        }

        protected void Error(ValidationResultCollection results, string errorName, int line, int col, params object[] objects)
        {
            ErrorInternal(results, errorName, ValidationResultInfoType.Error, line, col, objects);
        }

        protected void Fatal(ValidationResultCollection results, string errorName)
        {
            ErrorInternal(results, errorName, ValidationResultInfoType.Fatal, 0, 0, null, null);
        }

        protected void Fatal(ValidationResultCollection results, string errorName, int line, int col, params object[] objects)
        {
            ErrorInternal(results, errorName, ValidationResultInfoType.Fatal, line, col, objects);
        }

        #endregion

        #region Private Methods

        private void ErrorInternal(
            ValidationResultCollection results, 
            string errorName, 
            ValidationResultInfoType type, 
            int line,
            int col,
            params object[] objects)
        {
            results.Passed = false;
            ValidationErrorWithLookup error = new ValidationErrorWithLookup(ResourceManager, errorName, type, line, col);
            error.Message = string.Format(error.Message, objects);
            if (error.Resolutions != null)
            {
                // Format each resolution as well.
                for (int i = 0; i < error.Resolutions.Length; i++)
                    error.Resolutions[i] = string.Format(error.Resolutions[i], objects);
            }
            results.Add(error);
        }

        #endregion

        #region IValidator Members

        public IResourceManager ResourceManager { get; protected set; }
        public abstract IValidationResultCollection Validate();

        #endregion
    }
}
