using System;
using System.Collections.Generic;
using System.Text;

namespace DDay.iCal.Validator
{
    public class StringValidation :
        Validation
    {
        #region Public Properties

        public ICalendarPropertyListContainer Container { get; set; }
        public string PropertyName { get; set; }
        public IList<string> StringList { get; set; }
        public string String { get; set; }

        #endregion

        #region Constructors

        public StringValidation(IResourceManager mgr, ICalendarPropertyListContainer obj, string str, string propertyName) :
            base(mgr)
        {
            Container = obj;
            String = str;
            PropertyName = propertyName;
        }

        public StringValidation(IResourceManager mgr, ICalendarPropertyListContainer obj, IList<string> stringList, string propertyName) :
            base(mgr)
        {
            Container = obj;
            StringList = stringList;
            PropertyName = propertyName;
        }

        #endregion

        #region IValidator Members

        public override IValidationResultCollection Validate()
        {
            ValidationResultCollection results = new ValidationResultCollection(ResourceManager);
            results.Add(
                new StringPropertyValidation(
                    ResourceManager, 
                    Container,
                    PropertyName
                ).Validate()
            );

            return results;
        }

        #endregion
    }
}
