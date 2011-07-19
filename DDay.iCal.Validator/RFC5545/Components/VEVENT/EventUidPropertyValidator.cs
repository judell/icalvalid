using System;
using System.Collections.Generic;
using System.Text;

namespace DDay.iCal.Validator.RFC5545
{
	public class EventUidPropertyValidator :
        EventValidation
	{
        #region Constructors

        public EventUidPropertyValidator(IResourceManager mgr, IICalendarCollection calendars) :
            base(mgr, "eventUidProperty", calendars)
        {            
        }

        #endregion

        #region Overrides

        protected override IValidationResultCollection ValidateEvent(IEvent evt)
        {
            return ValidationResult.GetCompositeResults(
                ResourceManager,
                new PropertyCountValidation(ResourceManager, evt, "VEVENT", "UID"),
                new StringValidation(ResourceManager, evt, evt.UID, "UID")
            );
        }

        #endregion
	}
}
