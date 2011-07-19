using System;
using System.Collections.Generic;
using System.Text;

namespace DDay.iCal.Validator
{
    public class RecurringComponentDTStartRequiredValidation :
        PropertyCountValidation
    {
        public RecurringComponentDTStartRequiredValidation(IResourceManager mgr, IRecurringComponent rc, string componentName)
            : base(mgr, rc, componentName, "DTSTART", 0, 1)
        {
        }

        public override IValidationResultCollection Validate()
        {
            IRecurringComponent rc = Container as IRecurringComponent;
            if (rc != null)
            {
                MinCount = 0;

                // See RFC 5545 Seciton 3.8.2.4
                // 1. This property is REQUIRED in all types of recurring calendar components
                //    that specify the "RRULE" property.
                // 2. This property is also REQUIRED in "VEVENT" calendar components contained
                //    in iCalendar objects that don't specify the "METHOD" property.
                if (rc.RecurrenceRules != null &&
                    rc.RecurrenceRules.Count > 0)
                    MinCount = 1;
                else if (
                    rc is IEvent &&
                    (
                        // NOTE: if an iCalendar component cannot be found, then let's default
                        // to requiring the DTSTART property.
                        rc.iCalendar == null ||
                        rc.iCalendar.Method == null
                    ))
                {
                    MinCount = 1;
                }
            }

            return base.Validate();
        }
    }
}
