using System;
using System.Collections.Generic;
using System.Text;

namespace DDay.iCal.Validator
{
    public class ValidationError : 
        IValidationResultInfo
    {
        #region Public Properties

        public IResourceManager ResourceManager { get; protected set; }

        #endregion

        #region IValidationError Members

        public string Name { get; set; }
        public ValidationResultInfoType Type { get; set; }
        public string Message { get; set; }
        public bool IsFatal { get; set; }
        public int Line { get; set; }
        public int Col { get; set; }
        public string[] Resolutions { get; set; }

        #endregion

        #region Constructors
        
        public ValidationError(IResourceManager mgr)
        {
            ResourceManager = mgr;
        }

        public ValidationError(IResourceManager mgr, string name) : this(mgr)
        {
            Name = name;
        }

        public ValidationError(IResourceManager mgr, string name, string msg) :
            this(mgr, name)
        {
            Message = msg;
        }

        public ValidationError(IResourceManager mgr, string name, string msg, ValidationResultInfoType type) :
            this(mgr, name, msg)
        {
            Type = type;
        }

        public ValidationError(IResourceManager mgr, string name, string msg, ValidationResultInfoType type, bool isFatal) :
            this(mgr, name, msg, type)
        {
            IsFatal = isFatal;
        }

        public ValidationError(IResourceManager mgr, string name, string msg, ValidationResultInfoType type, bool isFatal, int line) :
            this(mgr, name, msg, type, isFatal)
        {
            Line = line;
        }

        public ValidationError(IResourceManager mgr, string name, string msg, ValidationResultInfoType type, bool isFatal, int line, int col) :
            this(mgr, name, msg, type, isFatal, line)
        {
            Col = col;
        }

        public ValidationError(IResourceManager mgr, string name, string msg, ValidationResultInfoType type, bool isFatal, int line, int col, string[] resolutions) :
            this(mgr, name, msg, type, isFatal, line, col)
        {
            Resolutions = resolutions;
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(ResourceManager.GetString(Type.ToString()));

            if (!string.IsNullOrEmpty(Message))
            {
                sb.Append(": ");
                sb.Append(Message);
            }
            
            if (Line != default(int))
            {
                sb.Append(" (" + ResourceManager.GetString("Line") + " ");
                sb.Append(Line);
                if (Col != default(int))
                {
                    sb.Append(" " + ResourceManager.GetString("Column") + " ");
                    sb.Append(Col);
                }
                sb.Append(")");
            }

            return sb.ToString();
        }

        #endregion
    }
}
