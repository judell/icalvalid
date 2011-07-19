using System;
using System.Collections.Generic;
using System.Text;

namespace DDay.iCal.Validator.RFC5545
{
    public class ComponentValidator :
        CalendarObjectValidation
    {
        #region Constructors

        public ComponentValidator(IResourceManager mgr, IICalendarCollection calendars) : base(mgr, calendars)
        {
        }

        #endregion

        #region Protected Methods

        protected override IValidationResultCollection ValidateObject(ICalendarObject component)
        {
            ValidationResultCollection result = new ValidationResultCollection(ResourceManager, "component");

            if (component is ICalendarComponent)
            {
                switch (component.Name.ToUpper())
                {
                    case "VALARM": // FIXME: VALARM should only be valid within recurring components.
                    case "VEVENT":
                    case "VTODO":
                    case "VJOURNAL":
                    case "VTIMEZONE":
                    case "VFREEBUSY":
                    case "DAYLIGHT":
                    case "STANDARD":
                        break;
                    case "VNOTE":
                        {
                            Warning(result, "vnoteDeprecatedError", component.Line, component.Column);
                        } break;
                    default:
                        {
                            // FIXME: return an ERROR for nonstandard components without an "X-" prefix, and
                            // a WARNING for "X-" components.
                        } break;
                }

                // Validation of children of the component, for subcomponents.
                // This is necessary for DAYLIGHT, STANDARD, and VALARM.
                foreach (ICalendarObject child in component.Children)
                {
                    IValidationResultCollection childResult = ValidateObject(child);
                    if (childResult.Passed != null && childResult.Passed.HasValue && !childResult.Passed.Value)
                        result.Add(childResult);
                }
            }

            return result;
        }

        #endregion
    }
}
