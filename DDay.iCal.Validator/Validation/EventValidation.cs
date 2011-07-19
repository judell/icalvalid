using System;
using System.Collections.Generic;
using System.Text;

namespace DDay.iCal.Validator
{
    public abstract class EventValidation :
        Validation
    {
        #region Public Properties

        public string Rule { get; set; }
        public IICalendarCollection Calendars { get; set; }

        #endregion

        #region Constructors

        public EventValidation(IResourceManager mgr, string rule, IICalendarCollection calendars) :
            base(mgr)
        {
            Rule = rule;
            Calendars = calendars;
        }

        #endregion

        #region Protected Methods

        protected abstract IValidationResultCollection ValidateEvent(IEvent evt);

        #endregion

        #region IValidator Members
                
        public override IValidationResultCollection Validate()
        {
            ValidationResultCollection result = new ValidationResultCollection(ResourceManager, Rule);
            result.Passed = true;

            if (Calendars != null)
            {
                foreach (IICalendar calendar in Calendars)
                {
                    foreach (IEvent evt in calendar.Events)
                        result.Add(ValidateEvent(evt));
                }
            }

            return result;
        }

        #endregion
    }
}
