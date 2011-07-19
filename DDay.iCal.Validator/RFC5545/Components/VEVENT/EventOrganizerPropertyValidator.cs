using System;
using System.Collections.Generic;
using System.Text;

namespace DDay.iCal.Validator.RFC5545
{
	public class EventOrganizerPropertyValidator :
        EventValidation
	{
        #region Constructors

        public EventOrganizerPropertyValidator(IResourceManager mgr, IICalendarCollection calendars) :
            base(mgr, "eventOrganizerProperty", calendars)
        {
        }

        #endregion

        #region Overrides

        protected override IValidationResultCollection ValidateEvent(IEvent evt)
        {
            return ValidationResult.GetCompositeResults(
                ResourceManager,
                new PropertyCountValidation(ResourceManager, evt, "VEVENT", "ORGANIZER")
            );
        }

        #endregion
	}
}
