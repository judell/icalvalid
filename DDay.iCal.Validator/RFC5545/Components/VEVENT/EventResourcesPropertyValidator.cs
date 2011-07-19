using System;
using System.Collections.Generic;
using System.Text;

namespace DDay.iCal.Validator.RFC5545
{
	public class EventResourcesPropertyValidator :
        EventValidation
	{
        #region Constructors

        public EventResourcesPropertyValidator(IResourceManager mgr, IICalendarCollection calendars) :
            base(mgr, "eventResourcesProperty", calendars)
        {
        }

        #endregion

        #region Overrides

        protected override IValidationResultCollection ValidateEvent(IEvent evt)
        {
            return new StringValidation(ResourceManager, evt, evt.Resources, "RESOURCES").Validate();            
        }

        #endregion
	}
}
