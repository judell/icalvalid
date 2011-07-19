using System;
using System.Collections.Generic;
using System.Text;

namespace DDay.iCal.Validator.RFC5545
{
	public class EventRelatedPropertyValidator :
        EventValidation
	{
        #region Constructors

        public EventRelatedPropertyValidator(IResourceManager mgr, IICalendarCollection calendars) :
            base(mgr, "eventRelatedProperty", calendars)
        {
        }

        #endregion

        #region Overrides

        protected override IValidationResultCollection ValidateEvent(IEvent evt)
        {
            return new StringValidation(ResourceManager, evt, evt.RelatedComponents, "RELATED-TO").Validate();
        }

        #endregion
	}
}
