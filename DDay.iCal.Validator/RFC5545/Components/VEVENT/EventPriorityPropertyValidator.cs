using System;
using System.Collections.Generic;
using System.Text;

namespace DDay.iCal.Validator.RFC5545
{
	public class EventPriorityPropertyValidator :
        EventValidation
	{
        #region Constructors

        public EventPriorityPropertyValidator(IResourceManager mgr, IICalendarCollection calendars) :
            base(mgr, "eventPriorityProperty", calendars)
        {
        }

        #endregion

        #region Overrides

        protected override IValidationResultCollection ValidateEvent(IEvent evt)
        {
            return ValidationResult.GetCompositeResults(
                ResourceManager,
                new PropertyCountValidation(ResourceManager, evt, "VEVENT", "PRIORITY")
            );
        }

        #endregion
	}
}
