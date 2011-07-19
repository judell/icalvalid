using System;
using System.Collections.Generic;
using System.Text;

namespace DDay.iCal.Validator.RFC5545
{
	public class EventContactPropertyValidator :
        EventValidation
	{
        #region Constructors

        public EventContactPropertyValidator(IResourceManager mgr, IICalendarCollection calendars) :
            base(mgr, "eventContactProperty", calendars)
        {
        }

        #endregion

        #region Overrides

        protected override IValidationResultCollection ValidateEvent(IEvent evt)
        {
            return new StringValidation(ResourceManager, evt, evt.Contacts, "CONTACT").Validate();            
        }

        #endregion
    }
}
