using System;
using System.Collections.Generic;
using System.Text;

namespace DDay.iCal.Validator.RFC5545
{
	public class EventDTStampPropertyValidator :
        EventValidation
	{
        #region Constructors

        public EventDTStampPropertyValidator(IResourceManager mgr, IICalendarCollection calendars) :
            base(mgr, "eventDTStampProperty", calendars)
        {
        }

        #endregion

        #region Overrides

        protected override IValidationResultCollection ValidateEvent(IEvent evt)
        {
            return ValidationResult.GetCompositeResults(
                ResourceManager,
                new PropertyCountValidation(ResourceManager, evt, "VEVENT", "DTSTAMP"),
                new iCalDateTimePropertyValidation(ResourceManager, evt, "DTSTAMP")
            );
        }

        #endregion
	}
}
