using System;
using System.Collections.Generic;
using System.Text;

namespace DDay.iCal.Validator.RFC5545
{
	public class EventTranspPropertyValidator :
        EventValidation
	{
        #region Constructors

        public EventTranspPropertyValidator(IResourceManager mgr, IICalendarCollection calendars) :
            base(mgr, "eventTranspProperty", calendars)
        {
        }

        #endregion

        #region Overrides

        protected override IValidationResultCollection ValidateEvent(IEvent evt)
        {
            return ValidationResult.GetCompositeResults(
                ResourceManager,
                new PropertyCountValidation(ResourceManager, evt, "VEVENT", "TRANSP")
            );
        }

        #endregion
	}
}
