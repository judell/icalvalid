using System;
using System.Collections.Generic;
using System.Text;
using System.Resources;
using System.Reflection;
using System.IO;

namespace DDay.iCal.Validator.RFC5545
{
    public class UnknownValueTypeParameterValidator : 
        CalendarObjectValidation
    {
        #region Constructors

        public UnknownValueTypeParameterValidator(IResourceManager mgr, IICalendarCollection calendars) : base(mgr, calendars)
        {
        }

        #endregion

        #region Overrides

        protected override IValidationResultCollection ValidateObject(ICalendarObject obj)
        {
            ValidationResultCollection results = new ValidationResultCollection(ResourceManager);
            results.Passed = true;

            ICalendarPropertyListContainer c = obj as ICalendarPropertyListContainer;
            if (c != null)
            {
                List<string> validValues = new List<string>(new string[]
                {
                    "BINARY",
                    "BOOLEAN",
                    "CAL-ADDRESS",
                    "DATE",
                    "DATE-TIME",
                    "DURATION",
                    "FLOAT",
                    "INTEGER",
                    "PERIOD",
                    "RECUR",
                    "TEXT",
                    "TIME",
                    "URI",
                    "UTC-OFFSET"
                });

                foreach (ICalendarProperty p in c.Properties)
                {
                    foreach (ICalendarParameter parm in p.Parameters)
                    {
                        if (parm.Values != null &&
                            parm.Values.Length > 0 &&
                            string.Equals(parm.Name, "VALUE", StringComparison.InvariantCultureIgnoreCase))
                        {
                            if (!validValues.Contains(parm.Values[0].ToUpper()))
                                Warning(results, "unknownValueTypeParameterError", p.Line, p.Column, parm.Values[0], string.Join(", ", validValues.ToArray()));
                        }
                    }
                }
            }

            return results;
        }

        #endregion
    }
}
