using System;
using System.Collections.Generic;
using System.Text;

namespace DDay.iCal.Validator.RFC5545
{
	public class EventRDatePropertyValidator :
        EventValidation
	{
        #region Constructors

        public EventRDatePropertyValidator(IResourceManager mgr, IICalendarCollection calendars) :
            base(mgr, "eventRDateProperty", calendars)
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
