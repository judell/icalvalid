using System;
using System.Collections.Generic;
using System.Text;

namespace DDay.iCal.Validator.RFC5545
{
	public class EventSummaryPropertyValidator :
        EventValidation
	{
        #region Constructors

        public EventSummaryPropertyValidator(IResourceManager mgr, IICalendarCollection calendars) :
            base(mgr, "eventSummaryProperty", calendars)
        {
        }

        #endregion

        #region Overrides

        protected override IValidationResultCollection ValidateEvent(IEvent evt)
        {
            return ValidationResult.GetCompositeResults(
                ResourceManager,
                new PropertyCountValidation(ResourceManager, evt, "VEVENT", "SUMMARY"),
                new StringValidation(ResourceManager, evt, evt.Summary, "SUMMARY")
            );
        }

        #endregion
	}
}
