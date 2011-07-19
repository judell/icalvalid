﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DDay.iCal.Validator.RFC5545
{
	public class EventExRulePropertyValidator :
        EventValidation
	{
        #region Constructors

        public EventExRulePropertyValidator(IResourceManager mgr, IICalendarCollection calendars) :
            base(mgr, "eventExRuleProperty", calendars)
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
