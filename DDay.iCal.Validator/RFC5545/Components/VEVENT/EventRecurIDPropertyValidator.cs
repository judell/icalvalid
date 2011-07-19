using System;
using System.Collections.Generic;
using System.Text;

namespace DDay.iCal.Validator.RFC5545
{
	public class EventRecurIDPropertyValidator :
        EventValidation
	{
        #region Constructors

        public EventRecurIDPropertyValidator(IResourceManager mgr, IICalendarCollection calendars) :
            base(mgr, "eventRecurIDProperty", calendars)
        {
        }

        #endregion

        #region Overrides

        protected override IValidationResultCollection ValidateEvent(IEvent evt)
        {
            return ValidationResult.GetCompositeResults(
                ResourceManager,
                new PropertyCountValidation(ResourceManager, evt, "VEVENT", "RECURRENCE-ID"),
                new iCalDateTimePropertyValidation(ResourceManager, evt, "RECURRENCE-ID")
            );
        }

        #endregion
	}
}
