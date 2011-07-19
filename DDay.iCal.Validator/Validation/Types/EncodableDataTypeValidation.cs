using System;
using System.Collections.Generic;
using System.Text;

namespace DDay.iCal.Validator
{
    public class EncodableDataTypeValidation :
        Validation
    {
        #region Public Properties

        public EncodableDataType Object { get; set; }

        #endregion

        #region Constructors

        public EncodableDataTypeValidation(IResourceManager mgr, EncodableDataType obj) :
            base(mgr)
        {
            Object = obj;
        }

        #endregion

        #region IValidator Members

        public override IValidationResultCollection Validate()
        {
            ValidationResultCollection results = new ValidationResultCollection(ResourceManager);
            results.Passed = true;

            if (Object != null)
            {
                if (Object.Encoding != null)
                {
                    int line = 0;
                    int col = 0;
                    if (Object.AssociatedObject != null)
                    {
                        line = Object.AssociatedObject.Line;
                        col = Object.AssociatedObject.Column;
                    }

                    switch (Object.Encoding)
                    {
                        case "7BIT":
                        case "8BIT":
                        case "BASE64":
                            break;
                        case "QUOTED-PRINTABLE":
                            Warning(results, "deprecatedEncodingError", line, col, Object.Encoding);
                            break;
                        default:
                            Warning(results, "invalidEncodingError", line, col, Object.Encoding);
                            break;
                    }
                }
            }

            return results;
        }

        #endregion
    }
}
