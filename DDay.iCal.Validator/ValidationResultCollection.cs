using System;
using System.Collections.Generic;
using System.Text;

namespace DDay.iCal.Validator
{
    public class ValidationResultCollection :
        List<IValidationResult>,
        IValidationResultCollection
    {
        #region Private Fields

        IResourceManager _ResourceManager;
        bool? _LocalPassed = null;
        List<IValidationResultInfo> _LocalDetails = new List<IValidationResultInfo>();
        long _ByteCount = 0;        
        string _CalendarText = null;
        string _CalendarPath = null;

        #endregion

        #region Constructors

        public ValidationResultCollection(IResourceManager mgr)
        {
            _ResourceManager = mgr;
        }

        public ValidationResultCollection(IResourceManager mgr, string rule) : this(mgr)
        {
            Rule = rule;
        }

        public ValidationResultCollection(IResourceManager mgr, string rule, IValidationResultCollection validationResults)
            : this(mgr, rule)
        {
            foreach (IValidationResult result in validationResults)
                Add(result);
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Rule);
            sb.Append(":");
            if (Passed != null)
            {
                if (BoolUtil.IsTrue(Passed))
                    sb.Append(_ResourceManager.GetString("pass"));
                else
                    sb.Append(_ResourceManager.GetString("fail"));
            }
            else sb.Append(_ResourceManager.GetString("didNotRun"));

            return sb.ToString();
        }

        #endregion

        #region IValidationResult Members

        virtual public long ByteCount
        {
            get { return _ByteCount; }
            set { _ByteCount = value; }
        }

        virtual public string CalendarText
        {
            get { return _CalendarText; }
            set { _CalendarText = value; }
        }

        virtual public string CalendarPath
        {
            get { return _CalendarPath; }
            set { _CalendarPath = value; }
        }

        virtual public bool IsFatal
        {
            get
            {
                if (Details != null)
                {
                    foreach (IValidationResultInfo info in Details)
                    {
                        if (info.Type == ValidationResultInfoType.Fatal)
                            return true;
                    }
                }
                return false;
            }
        }

        virtual public string Rule { get; set; }

        virtual public bool? Passed 
        {
            get
            {
                if (_LocalPassed != null && _LocalPassed.HasValue && !_LocalPassed.Value)
                    return false;

                bool? passed = _LocalPassed;
                foreach (IValidationResult result in this)
                {
                    if (result.Passed != null && result.Passed.HasValue)
                    {
                        if (passed == null)
                            passed = result.Passed;
                        else if (!BoolUtil.IsTrue(result.Passed))
                        {
                            passed = false;
                            break;
                        }
                    }
                }
                
                return passed;
            }
            set
            {
                LocalPassed = value;
            }
        }

        virtual public IValidationResultInfo[] Details
        {
            get
            {
                List<IValidationResultInfo> details = new List<IValidationResultInfo>(_LocalDetails);
                foreach (IValidationResult result in this)
                {
                    if (result.Details != null)
                        details.AddRange(result.Details);
                }
                return details.ToArray();
            }            
        }

        virtual public IValidationResultInfo[] LocalDetails
        {
            get
            {
                return _LocalDetails.ToArray();
            }
            set
            {
                _LocalDetails.Clear();
                if (value != null)
                    _LocalDetails.AddRange(value);
            }
        }

        virtual public bool? LocalPassed
        {
            get { return _LocalPassed; }
            set { _LocalPassed = value; }
        }

        virtual public new void Add(IValidationResult result)
        {
            if (result != null)
            {
                base.Add(result);

                IValidationResultCollection collection = result as IValidationResultCollection;
                if (collection != null)
                {
                    if (Rule == null)
                        Rule = collection.Rule;
                }
            }
        }

        virtual public void Add(IValidationResultInfo info)
        {
            if (info != null)
                _LocalDetails.Add(info);
        }

        #endregion
    }
}
