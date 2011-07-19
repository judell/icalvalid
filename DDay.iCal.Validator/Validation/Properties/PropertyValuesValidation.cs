using System;
using System.Collections.Generic;
using System.Text;

namespace DDay.iCal.Validator
{
    public class PropertyValuesValidation :
        Validation
    {
        #region Public Properties

        public ICalendarPropertyListContainer Container { get; set; }
        public string PropertyName { get; set; }
        public string ComponentName { get; set; }
        public List<string> PossibleValues { get; set; }

        #endregion

        #region Constructors

        public PropertyValuesValidation(IResourceManager mgr, ICalendarPropertyListContainer container, string componentName, string propertyName, params string[] possibleValues) :
            base(mgr)
        {
            Container = container;
            ComponentName = componentName;
            PropertyName = propertyName;
            PossibleValues = new List<string>(possibleValues);
        }

        public PropertyValuesValidation(IResourceManager mgr, ICalendarPropertyListContainer container, string componentName, string propertyName, List<string> possibleValues) :
            base(mgr)
        {
            Container = container;
            ComponentName = componentName;
            PropertyName = propertyName;
            PossibleValues = possibleValues;
        }

        #endregion

        #region IValidator Members

        public override IValidationResultCollection Validate()
        {
            ValidationResultCollection result = new ValidationResultCollection(ResourceManager);

            if (Container.Properties.ContainsKey(PropertyName))
            {
                ICalendarProperty p = Container.Properties[PropertyName];
                string value = p.Value != null ? p.Value.ToString() : null;
                if (value != null && !PossibleValues.Contains(value))
                {
                    Error(result, "propertyInvalidValueWarning", p.Line, p.Column, PropertyName, ComponentName, p.Value, string.Join(", ", PossibleValues.ToArray()));
                }
            }

            return result;
        }

        #endregion
    }
}
