using System;
using System.Collections.Generic;
using System.Text;

namespace DDay.iCal.Validator.RFC5545
{
	public class EventDTEndPropertyValidator :
        EventValidation
	{
        #region Constructors

        public EventDTEndPropertyValidator(IResourceManager mgr, IICalendarCollection calendars) :
            base(mgr, "eventDTEndProperty", calendars)
        {
        }

        #endregion

        #region Overrides

        protected override IValidationResultCollection ValidateEvent(IEvent evt)
        {
            return ValidationResult.GetCompositeResults(
                ResourceManager,
                new PropertyCountValidation(ResourceManager, evt, "VEVENT", new string[] { "DTEND", "DURATION" }, true),
                new iCalDateTimePropertyValidation(ResourceManager, evt, "DTEND")
            );
        }

        #endregion
	}
}
