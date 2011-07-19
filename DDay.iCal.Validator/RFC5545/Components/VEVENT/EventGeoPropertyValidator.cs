using System;
using System.Collections.Generic;
using System.Text;

namespace DDay.iCal.Validator.RFC5545
{
	public class EventGeoPropertyValidator :
        EventValidation
	{
        #region Constructors

        public EventGeoPropertyValidator(IResourceManager mgr, IICalendarCollection calendars) :
            base(mgr, "eventGeoProperty", calendars)
        {            
        }

        #endregion

        #region Overrides

        protected override IValidationResultCollection ValidateEvent(IEvent evt)
        {
            return ValidationResult.GetCompositeResults(
                ResourceManager,
                new PropertyCountValidation(ResourceManager, evt, "VEVENT", "GEO")
            );
        }

        #endregion
	}
}
