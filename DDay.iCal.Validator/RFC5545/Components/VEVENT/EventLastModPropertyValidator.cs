using System;
using System.Collections.Generic;
using System.Text;

namespace DDay.iCal.Validator.RFC5545
{
	public class EventLastModPropertyValidator :
        EventValidation
	{
        #region Constructors

        public EventLastModPropertyValidator(IResourceManager mgr, IICalendarCollection calendars) :
            base(mgr, "eventLastModProperty", calendars)
        {
        }

        #endregion

        #region Overrides

        protected override IValidationResultCollection ValidateEvent(IEvent evt)
        {
            return ValidationResult.GetCompositeResults(
                ResourceManager,
                new PropertyCountValidation(ResourceManager, evt, "VEVENT", "LAST-MODIFIED")
            );
        }

        #endregion
	}
}
