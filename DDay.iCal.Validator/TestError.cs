using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DDay.iCal.Validator
{
    public class TestError :
        ITestError
    {
        #region Constructors

        public TestError(IResourceManager mgr, string name, string source, IValidationResultCollection validationResults)
        {
            Name = name;
            Message = mgr.GetError(name);
            Source = source;
            ValidationResults = validationResults;
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();            
            sb.Append(Message);
            int i = 1;
            foreach (IValidationResult result in ValidationResults)
            {
                if (!BoolUtil.IsTrue(result.Passed))
                {
                    if (result.Details != null)
                    {
                        foreach (IValidationResultInfo info in result.Details)
                        {
                            sb.Append(Environment.NewLine + i + ": ");
                            StringReader sr = new StringReader(info.ToString());
                            string s = sr.ReadLine();
                            bool isFirstLine = true;
                            while (!string.IsNullOrEmpty(s))
                            {
                                if (!isFirstLine)
                                    sb.Append(Environment.NewLine);
                                sb.Append("\t" + s);
                                isFirstLine = false;
                                s = sr.ReadLine();
                            }
                            sr.Close();

                            i++;
                        }
                    }
                }
            }

            return sb.ToString();
        }

        #endregion

        #region ICalendarTestError Members

        virtual public string Name { get; protected set; }
        virtual public string Message { get; set; }
        virtual public string Source { get; protected set; }
        virtual public IValidationResultCollection ValidationResults { get; protected set; }

        #endregion
    }
}
