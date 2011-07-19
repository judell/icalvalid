using System;
using System.Collections.Generic;
using System.Text;

namespace DDay.iCal.Validator.RFC5545
{
	public class EventCategoriesPropertyValidator :
        EventValidation
	{
        #region Constructors

        public EventCategoriesPropertyValidator(IResourceManager mgr, IICalendarCollection calendars) :
            base(mgr, "eventCategoriesProperty", calendars)
        {
        }

        #endregion

        #region Overrides

        protected override IValidationResultCollection ValidateEvent(IEvent evt)
        {
            return new StringValidation(ResourceManager, evt, evt.Categories, "CATEGORIES").Validate();
        }

        #endregion
	}
}
