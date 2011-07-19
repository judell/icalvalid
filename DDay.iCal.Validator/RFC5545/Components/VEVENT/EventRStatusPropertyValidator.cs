using System;
using System.Collections.Generic;
using System.Text;

namespace DDay.iCal.Validator.RFC5545
{
	public class EventRStatusPropertyValidator :
        EventValidation
	{
        #region Constructors

        public EventRStatusPropertyValidator(IResourceManager mgr, IICalendarCollection calendars) :
            base(mgr, "eventRStatusProperty", calendars)
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
