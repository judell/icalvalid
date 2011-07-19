using System;
using System.Collections.Generic;
using System.Text;

namespace DDay.iCal.Validator
{
    public abstract class PropertyValidation :
        Validation
    {
        #region Public Properties

        public ICalendarPropertyListContainer Container { get; set; }
        public string PropertyName { get; set; }

        #endregion

        #region Constructors

        public PropertyValidation(IResourceManager mgr, ICalendarPropertyListContainer obj, string propertyName) :
            base(mgr)
        {
            Container = obj;
            PropertyName = propertyName;
        }

        #endregion

        #region Protected Methods

        protected abstract IValidationResultCollection ValidateProperty(ICalendarProperty p);

        #endregion

        #region IValidator Members

        public override IValidationResultCollection Validate()
        {
            ValidationResultCollection results = new ValidationResultCollection(ResourceManager);

            if (Container != null &&
                Container.Properties != null)
            {
                foreach (ICalendarProperty p in Container.Properties)
                {
                    if (p.Name.Equals(PropertyName, StringComparison.InvariantCultureIgnoreCase))
                        results.Add(ValidateProperty(p));                   
                }
            }

            return results;
        }

        #endregion
    }
}
