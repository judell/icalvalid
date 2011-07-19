using System;
using System.Collections.Generic;
using System.Text;

namespace DDay.iCal.Validator.RFC5545
{
	public class EventAttachPropertyValidator :
        EventValidation
	{
        #region Constructors

        public EventAttachPropertyValidator(IResourceManager mgr, IICalendarCollection calendars) :
            base(mgr, "eventAttachProperty", calendars)
        {            
        }

        #endregion

        #region Overrides

        protected override IValidationResultCollection ValidateEvent(IEvent evt)
        {
            // FIXME: do some validation here
            return null;
        }

        #endregion
	}
}
