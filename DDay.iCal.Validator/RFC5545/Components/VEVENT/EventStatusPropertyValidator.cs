using System;
using System.Collections.Generic;
using System.Text;

namespace DDay.iCal.Validator.RFC5545
{
	public class EventStatusPropertyValidator :
        EventValidation
	{
        #region Constructors

        public EventStatusPropertyValidator(IResourceManager mgr, IICalendarCollection calendars) :
            base(mgr, "eventStatusProperty", calendars)
        {
        }

        #endregion

        #region Overrides

        protected override IValidationResultCollection ValidateEvent(IEvent evt)
        {
            return ValidationResult.GetCompositeResults(
                ResourceManager,
                new PropertyCountValidation(ResourceManager, evt, "VEVENT", "STATUS"),
                new PropertyValuesValidation(ResourceManager, evt, "VEVENT", "STATUS", "Tentative", "Confirmed", "Cancelled")
            );
        }

        #endregion
	}
}
