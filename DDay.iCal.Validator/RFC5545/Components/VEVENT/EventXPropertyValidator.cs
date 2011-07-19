using System;
using System.Collections.Generic;
using System.Text;

namespace DDay.iCal.Validator.RFC5545
{
	public class EventXPropertyValidator :
        EventValidation
	{
        #region Constructors

        public EventXPropertyValidator(IResourceManager mgr, IICalendarCollection calendars) :
            base(mgr, "eventXProperty", calendars)
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
