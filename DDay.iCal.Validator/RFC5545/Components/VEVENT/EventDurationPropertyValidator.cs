using System;
using System.Collections.Generic;
using System.Text;

namespace DDay.iCal.Validator.RFC5545
{
	public class EventDurationPropertyValidator :
        EventValidation
	{
        #region Constructors

        public EventDurationPropertyValidator(IResourceManager mgr, IICalendarCollection calendars) :
            base(mgr, "eventDurationProperty", calendars)
        {
        }

        #endregion

        #region Overrides

        protected override IValidationResultCollection ValidateEvent(IEvent evt)
        {
            return ValidationResult.GetCompositeResults(
                ResourceManager,
                new PropertyCountValidation(ResourceManager, evt, "VEVENT", "DURATION")
            );
        }

        #endregion
	}
}
