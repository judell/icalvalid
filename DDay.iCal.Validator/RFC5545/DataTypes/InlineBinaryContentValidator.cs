using System;
using System.Collections.Generic;
using System.Text;
using System.Resources;
using System.Reflection;
using System.IO;

namespace DDay.iCal.Validator.RFC5545
{
    public class InlineBinaryContentValidator : 
        Validation
    {
        #region Public Properties

        public IICalendarCollection Calendars { get; set; }

        #endregion

        #region Constructors

        public InlineBinaryContentValidator(IResourceManager mgr, IICalendarCollection calendars) :
            base(mgr)
        {
            Calendars = calendars;
        }

        #endregion

        #region IValidator Members

        public override IValidationResultCollection Validate()
        {
            ValidationResultCollection result = new ValidationResultCollection(ResourceManager, "inlineBinaryContent");
            result.Passed = true;
            
            if (Calendars != null)
            {
                foreach (IICalendar calendar in Calendars)
                {
                    foreach (IUniqueComponent uc in calendar.UniqueComponents)
                    {
                        IRecurringComponent rc = uc as IRecurringComponent;
                        if (rc != null && rc.Attachments != null)
                        {
                            foreach (IAttachment a in rc.Attachments)
                            {
                                // Inline binary content (i.e. BASE64-encoded attachments) is not
                                // recommended, and should only be used when absolutely necessary.
                                if (string.Equals(a.Encoding, "BASE64", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    Warning(result, "inlineBinaryContentError");
                                }
                            }

                            if (result.Passed != null)
                                break;
                        }
                    }
                }
            }

            return result;
        }

        #endregion
    }
}
