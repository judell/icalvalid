using System;
using System.Collections.Generic;
using System.Text;
using System.Resources;
using System.Reflection;
using System.IO;

namespace DDay.iCal.Validator.RFC5545
{
    public class EmptyLineValidator : 
        Validation
    {
        #region Public Properties

        public string iCalText { get; set; }

        #endregion

        #region Constructors

        public EmptyLineValidator(IResourceManager mgr, string cal_text) :
            base(mgr)
        {
            iCalText = cal_text;
        }

        #endregion

        #region IValidator Members

        public override IValidationResultCollection Validate()
        {
            ValidationResultCollection result = new ValidationResultCollection(ResourceManager, "emptyLine");
            result.Passed = true;

            // Convert all variations of newlines to a simple "\n" so
            // we can easily detect empty lines.
            string simpleNewlineCalendar = iCalText.Replace("\r\n", "\n");
            simpleNewlineCalendar = simpleNewlineCalendar.Replace("\r", "\n");

            StringReader sr = new StringReader(simpleNewlineCalendar);
            string line = sr.ReadLine();
            int lineNumber = 0;
            while (line != null)
            {
                lineNumber++;
                if (string.IsNullOrEmpty(line))
                {
                    Warning(result, "emptyLineError", lineNumber, 0, null, null);
                    break;
                }
                line = sr.ReadLine();
            }

            return result;
        }

        #endregion
    }
}
