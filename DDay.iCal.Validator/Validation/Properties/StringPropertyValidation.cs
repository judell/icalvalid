using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace DDay.iCal.Validator
{
    public class StringPropertyValidation :
        PropertyValidation
    {
        #region Constructors

        public StringPropertyValidation(IResourceManager mgr, ICalendarPropertyListContainer obj, string propertyName) :
            base(mgr, obj, propertyName)
        {
        }

        #endregion

        #region Overrides

        protected override IValidationResultCollection ValidateProperty(ICalendarProperty p)
        {
            ValidationResultCollection result = new ValidationResultCollection(ResourceManager);
            if (p != null)
            {
                result.Passed = true;

                // Get the escaped value for this property.
                // This is stored during deserialization when 
                // SerializationSettings.StoreExtraSerializationData is true.
                object escapedValue = p.GetService("EscapedValue");
                
                List<string> values = new List<string>();
                if (escapedValue is string)
                    values.Add((string)escapedValue);
                else if (escapedValue is IList<string>)
                    values.AddRange((IList<string>)escapedValue);

                // Validate the encoding
                EncodableDataType dt = new EncodableDataType();
                dt.AssociatedObject = p;
                EncodableDataTypeValidation validation = new EncodableDataTypeValidation(ResourceManager, dt);
                result.Add(validation.Validate());
                                
                foreach (string value in values)
                {
                    // Find single commas
                    if (Regex.IsMatch(value, @"(?<!\\),"))
                    {
                        Error(result, "textEscapeCommasError", p.Line, p.Column, p.Name);
                    }

                    // Find single semicolons
                    if (Regex.IsMatch(value, @"(?<!\\);"))
                    {
                        Error(result, "textEscapeSemicolonsError", p.Line, p.Column, p.Name);
                    }

                    // Find backslashes that are not escaped
                    if (Regex.IsMatch(value, @"\\([^\\nN;,])"))
                    {
                        Error(result, "textEscapeBackslashesError", p.Line, p.Column, p.Name);
                    }
                }
            }

            return result;
        }

        #endregion
    }
}
