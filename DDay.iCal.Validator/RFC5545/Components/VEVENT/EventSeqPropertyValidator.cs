using System;
using System.Collections.Generic;
using System.Text;

namespace DDay.iCal.Validator.RFC5545
{
	public class EventSeqPropertyValidator :
        EventValidation
	{
        #region Constructors

        public EventSeqPropertyValidator(IResourceManager mgr, IICalendarCollection calendars) :
            base(mgr, "eventSeqProperty", calendars)
        {
        }

        #endregion

        #region Overrides

        protected override IValidationResultCollection ValidateEvent(IEvent evt)
        {
            return ValidationResult.GetCompositeResults(         
                ResourceManager,
                new PropertyCountValidation(ResourceManager, evt, "VEVENT", "SEQUENCE")
            );
        }

        #endregion
	}
}
