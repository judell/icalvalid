using System;
using System.Collections.Generic;
using System.Text;
using System.Resources;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;

namespace DDay.iCal.Validator.RFC5545
{
    public class CRLFPairsValidator : 
        Validation
    {
        #region Public Properties

        public string iCalText { get; set; }

        #endregion

        #region Constructors

        public CRLFPairsValidator(IResourceManager mgr, string cal_text) :
            base(mgr)
        {
            iCalText = cal_text;
        }

        #endregion

        #region IValidator Members

        public override IValidationResultCollection Validate()
        {
            ValidationResultCollection result = new ValidationResultCollection(ResourceManager, "crlfPairs");
            result.Passed = true;

            MatchCollection allLineBreaks = Regex.Matches(iCalText, @"((\r?\n)|(\r\n?))");
            GC.KeepAlive(allLineBreaks);

            MatchCollection matches = Regex.Matches(iCalText, @"((\r(?=[^\n]))|((?<=[^\r])\n))");
            if (matches.Count > 0)
                Warning(result, "crlfPairError");

            return result;
        }

        #endregion
    }
}
