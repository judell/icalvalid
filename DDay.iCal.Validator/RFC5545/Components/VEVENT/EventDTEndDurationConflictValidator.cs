using System;
using System.Collections.Generic;
using System.Text;
using System.Resources;
using System.Reflection;
using System.IO;

namespace DDay.iCal.Validator.RFC5545
{
    public class EventDTEndDurationConflictValidator : 
        Validation
    {
        #region Public Properties

        public string iCalText { get; set; }

        #endregion

        #region Constructors

        public EventDTEndDurationConflictValidator(IResourceManager mgr, string cal_text) :
            base(mgr)
        {
            iCalText = cal_text;
        }

        #endregion

        #region IValidator Members

        public override IValidationResultCollection Validate()
        {
            bool started = false;
            bool hasDuration = false;
            bool hasEnd = false;
            bool errorAdded = false;
            int lineNumber = 0;

            ValidationResultCollection result = new ValidationResultCollection(ResourceManager, "eventDTEndDurationConflict");
            result.Passed = true;
            
            StringReader sr = new StringReader(iCalText);
            string line = sr.ReadLine();
            while (line != null)
            {
                lineNumber++;

                if (!started)
                {
                    if (line.StartsWith("BEGIN:VEVENT", StringComparison.InvariantCultureIgnoreCase))
                        started = true;
                }
                else
                {
                    if (line.StartsWith("END:VEVENT", StringComparison.InvariantCultureIgnoreCase))
                    {
                        started = false;
                        hasDuration = false;
                        hasEnd = false;
                        errorAdded = false;
                    }
                    else if (
                        line.StartsWith("DTEND;", StringComparison.InvariantCultureIgnoreCase) ||
                        line.StartsWith("DTEND:", StringComparison.InvariantCultureIgnoreCase))
                    {
                        hasEnd = true;
                    }
                    else if (
                        line.StartsWith("DURATION;", StringComparison.InvariantCultureIgnoreCase) ||
                        line.StartsWith("DURATION:", StringComparison.InvariantCultureIgnoreCase))
                    {
                        hasDuration = true;
                    }

                    if (!errorAdded && hasEnd && hasDuration)
                    {
                        Error(result, "eventDTEndDurationConflictError", lineNumber, 0, null, null);
                        errorAdded = true;
                    }
                }
                line = sr.ReadLine();
            }

            return result;
        }

        #endregion
    }
}
