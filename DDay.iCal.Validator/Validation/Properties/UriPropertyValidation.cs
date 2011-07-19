using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace DDay.iCal.Validator
{
    public class UriPropertyValidation :
        PropertyValidation
    {
        #region Constructors

        public UriPropertyValidation(IResourceManager mgr, ICalendarPropertyListContainer obj, string propertyName) :
            base(mgr, obj, propertyName)
        {
        }

        #endregion

        #region Overrides

        protected override IValidationResultCollection ValidateProperty(ICalendarProperty p)
        {
            ValidationResultCollection result = new ValidationResultCollection(ResourceManager);
            if (p != null)
            {
                result.Passed = true;

                // FIXME: is it valid to have an empty URI?
                if (p.Value != null && !(p.Value is Uri))
                {
                    try
                    {
                        Uri uri = new Uri(p.Value.ToString());
                        Error(result, "invalidUriFormatError", p.Line, p.Column, p.Value, "An unknown error occurred.");
                    }
                    catch (Exception ex)
                    {
                        Error(result, "invalidUriFormatError", p.Line, p.Column, p.Value, ex.Message);
                    }                    
                }
            }

            return result;
        }

        #endregion
    }
}
