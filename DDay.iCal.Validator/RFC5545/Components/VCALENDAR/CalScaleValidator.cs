using System;
using System.Collections.Generic;
using System.Text;
using System.Resources;
using System.Reflection;
using System.IO;

namespace DDay.iCal.Validator.RFC5545
{
    public class CalScaleValidator : 
        Validation
    {
        #region Public Properties

        public IICalendarCollection Calendars { get; set; }

        #endregion

        #region Constructors

        public CalScaleValidator(IResourceManager mgr, IICalendarCollection calendars) :
            base(mgr)
        {
            Calendars = calendars;
        }

        #endregion

        #region IValidator Members

        public override IValidationResultCollection Validate()
        {
            ValidationResultCollection results = new ValidationResultCollection(ResourceManager);
            results.Passed = true;

            if (Calendars != null)
            {
                foreach (IICalendar calendar in Calendars)
                {
                    results.Add(ValidationResult.GetCompositeResults(
                        ResourceManager,
                        "calScale",
                        new PropertyCountValidation(ResourceManager, calendar, "VCALENDAR", "CALSCALE")
                    ));
                }
            }

            return results;
        }

        #endregion
    }
}
