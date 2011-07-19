using System;
using System.Collections.Generic;
using System.Text;

namespace DDay.iCal.Validator.RFC5545
{
	public class EventLocationPropertyValidator :
        EventValidation
	{
        #region Constructors

        public EventLocationPropertyValidator(IResourceManager mgr, IICalendarCollection calendars) :
            base(mgr, "eventLocationProperty", calendars)
        {
        }

        #endregion

        #region Overrides

        protected override IValidationResultCollection ValidateEvent(IEvent evt)
        {
            return ValidationResult.GetCompositeResults(
                ResourceManager,
                new PropertyCountValidation(ResourceManager, evt, "VEVENT", "LOCATION"),
                new StringValidation(ResourceManager, evt, evt.Location, "LOCATION")
            );
        }

        #endregion
	}
}
