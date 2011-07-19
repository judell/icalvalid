using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DDay.iCal.Validator.RFC5545
{
    public class LineFoldingValidator :
        Validation
    {
        #region Public Properties

        public string iCalText { get; set; }

        #endregion

        #region Constructors

        public LineFoldingValidator(IResourceManager mgr, string cal_text) :
            base(mgr)
        {
            iCalText = cal_text;
        }

        #endregion

        #region IValidator Members

        public override IValidationResultCollection Validate()
        {
            ValidationResultCollection result = new ValidationResultCollection(ResourceManager, "lineFolding");

            try
            {
                StringReader sr = new StringReader(iCalText);
                IICalendarCollection calendar = iCalendar.LoadFromStream(sr);

                result.Passed = true;
            }
            catch (antlr.MismatchedTokenException ex)
            {
                if (ex.expecting == 6 && // COLON = 6
                    ex.mismatchType == antlr.MismatchedTokenException.TokenTypeEnum.TokenType)
                    Fatal(result, "lineFoldingWithoutSpaceError", ex.line, ex.column, ex.Message, null);
            }

            return result;
        }

        #endregion
    }
}
