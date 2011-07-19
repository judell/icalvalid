using System;
using System.Collections.Generic;
using System.Text;

namespace DDay.iCal.Validator.RFC5545
{
	public class EventDescriptionPropertyValidator :
        EventValidation
	{
        #region Constructors

        public EventDescriptionPropertyValidator(IResourceManager mgr, IICalendarCollection calendars) :
            base(mgr, "eventDescriptionProperty", calendars)
        {
        }

        #endregion

        #region Overrides

        protected override IValidationResultCollection ValidateEvent(IEvent evt)
        {
            return ValidationResult.GetCompositeResults(
                ResourceManager,
                 new PropertyCountValidation(ResourceManager, evt, "VEVENT", "DESCRIPTION"),
                 new StringValidation(ResourceManager, evt, evt.Description, "DESCRIPTION")
            );
        }

        #endregion
	}
}
