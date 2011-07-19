using System;
using System.Collections.Generic;
using System.Text;

namespace DDay.iCal.Validator.RFC5545
{
    public class EventUrlPropertyValidator :
        EventValidation
    {
        #region Constructors

        public EventUrlPropertyValidator(IResourceManager mgr, IICalendarCollection calendars) :
            base(mgr, "eventUrlProperty", calendars)
        {
        }

        #endregion

        #region Overrides

        protected override IValidationResultCollection ValidateEvent(IEvent evt)
        {
            return ValidationResult.GetCompositeResults(
                ResourceManager,
                new PropertyCountValidation(ResourceManager, evt, "VEVENT", "URL"),
                new UriPropertyValidation(ResourceManager, evt, "URL")
            );
        }

        #endregion
    }
}
