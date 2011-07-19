using System;
using System.Collections.Generic;
using System.Text;

namespace DDay.iCal.Validator.RFC5545
{
	public class EventCommentPropertyValidator :
        EventValidation
	{
        #region Constructors

        public EventCommentPropertyValidator(IResourceManager mgr, IICalendarCollection calendars) :
            base(mgr, "eventCommentProperty", calendars)
        {
        }

        #endregion

        #region Overrides

        protected override IValidationResultCollection ValidateEvent(IEvent evt)
        {
            return new StringValidation(ResourceManager, evt, evt.Comments, "COMMENT").Validate();
        }

        #endregion
	}
}
