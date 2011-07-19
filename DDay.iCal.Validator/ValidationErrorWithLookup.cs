using System;
using System.Collections.Generic;
using System.Text;

namespace DDay.iCal.Validator
{
    public class ValidationErrorWithLookup :
        ValidationError
    {
        public ValidationErrorWithLookup(IResourceManager mgr) : base(mgr)
        {
        }

        public ValidationErrorWithLookup(IResourceManager mgr, string name) : base(mgr, name)
        {
            Message = ResourceManager.GetError(name);
            Resolutions = ResourceManager.GetResolutions(name);
        }

        public ValidationErrorWithLookup(IResourceManager mgr, string name, ValidationResultInfoType type) :
            this(mgr, name)
        {
            Type = type;
        }

        public ValidationErrorWithLookup(IResourceManager mgr, string name, ValidationResultInfoType type, int line) :
            this(mgr, name, type)
        {
            Line = line;
        }

        public ValidationErrorWithLookup(IResourceManager mgr, string name, ValidationResultInfoType type, int line, int col) :
            this(mgr, name, type, line)
        {
            Col = col;
        }
    }
}
