using System;
using System.Collections.Generic;
using System.Text;

namespace DDay.iCal.Validator.RFC5545
{
	public class EventDTStartPropertyValidator :
        EventValidation
	{
        #region Constructors

        public EventDTStartPropertyValidator(IResourceManager mgr, IICalendarCollection calendars) :
            base(mgr, "eventDTStartProperty", calendars)
        {
        }

        #endregion

        #region Overrides

        protected override IValidationResultCollection ValidateEvent(IEvent evt)
        {
            return ValidationResult.GetCompositeResults(
                ResourceManager,
                new RecurringComponentDTStartRequiredValidation(ResourceManager, evt, "VEVENT"),
                new iCalDateTimePropertyValidation(ResourceManager, evt, "DTSTART")
            );
        }

        #endregion
	}
}
