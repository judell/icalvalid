using System;
using System.Collections.Generic;
using System.Text;

namespace DDay.iCal.Validator
{
    public interface IValidationResultCollection :
        ICollection<IValidationResult>,
        IValidationResult
    {
        bool IsFatal { get; }
        bool? LocalPassed { get; set; }
        IValidationResultInfo[] LocalDetails { get; set; }
        long ByteCount { get; set; }
        string CalendarText { get; set; }
        string CalendarPath { get; set; }

        void Add(IValidationResult results);
        void Add(IValidationResultInfo details);
    }
}
