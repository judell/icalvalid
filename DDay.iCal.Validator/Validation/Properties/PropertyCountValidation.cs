using System;
using System.Collections.Generic;
using System.Text;

namespace DDay.iCal.Validator
{
    public class PropertyCountValidation :
        Validation
    {
        #region Public Properties

        public ICalendarPropertyListContainer Container { get; set; }
        public string[] PropertyNames { get; set; }
        public string ComponentName { get; set; }
        public int MinCount { get; set; }
        public int MaxCount { get; set; }
        public bool MissingInsteadOfRequired { get; set; }

        #endregion

        #region Constructors

        public PropertyCountValidation(IResourceManager mgr, ICalendarPropertyListContainer container, string componentName, string propertyName) :
            this(mgr, container, componentName, new string[] { propertyName }, false)
        {
        }

        public PropertyCountValidation(IResourceManager mgr, ICalendarPropertyListContainer container, string componentName, string propertyName, bool missingInsteadOfRequired) :
            this(mgr, container, componentName, new string[] { propertyName }, missingInsteadOfRequired)
        {
        }

        public PropertyCountValidation(IResourceManager mgr, ICalendarPropertyListContainer container, string componentName, string propertyName, int minOccurrences) :
            this(mgr, container, componentName, propertyName, minOccurrences, int.MaxValue)
        {            
        }

        public PropertyCountValidation(IResourceManager mgr, ICalendarPropertyListContainer container, string componentName, string propertyName, int minOccurrences, int maxOccurrences) :
            this(mgr, container, componentName, propertyName)
        {
            MinCount = minOccurrences;
            MaxCount = maxOccurrences;
        }

        public PropertyCountValidation(IResourceManager mgr, ICalendarPropertyListContainer container, string componentName, string[] propertyNames, bool missingInsteadOfRequired) :
            base(mgr)
        {
            MissingInsteadOfRequired = missingInsteadOfRequired;

            MinCount = 0;
            MaxCount = 1;

            Container = container;
            ComponentName = componentName;
            PropertyNames = propertyNames;
        }

        #endregion

        #region IValidator Members

        public override IValidationResultCollection Validate()
        {
            ValidationResultCollection result = new ValidationResultCollection(ResourceManager);
            result.Passed = true;

            if (Container != null &&
                Container.Properties != null)
            {
                bool containsProperty = false;
                if (PropertyNames != null)
                {
                    foreach (string propName in PropertyNames)
                    {
                        if (Container.Properties.ContainsKey(propName))
                        {
                            containsProperty = true;
                            break;
                        }
                    }

                    if (!containsProperty)
                    {
                        if (MinCount > 0)
                        {
                            Error(result, "propertyRequiredError", Container.Line, Container.Column, PropertyNames[0], ComponentName);
                        }
                        else if (MissingInsteadOfRequired)
                        {
                            Warning(result, "propertyMissingError", Container.Line, Container.Column, PropertyNames[0], ComponentName);
                        }
                    }
                    else
                    {
                        if (MaxCount == 1 && Container.Properties.CountOf(PropertyNames[0]) > 1)
                        {
                            ICalendarProperty p = Container.Properties[PropertyNames[0]];
                            Error(result, "propertyOnlyOnceError", p.Line, p.Column, PropertyNames[0], ComponentName);
                        }
                        else if (Container.Properties.CountOf(PropertyNames[0]) > MaxCount)
                        {
                            ICalendarProperty p = Container.Properties[PropertyNames[0]];
                            Error(result, "propertyCountRangeError", p.Line, p.Column, PropertyNames[0], ComponentName, MinCount, MaxCount);
                        }
                    }
                }                
            }

            return result;
        }

        #endregion
    }
}
