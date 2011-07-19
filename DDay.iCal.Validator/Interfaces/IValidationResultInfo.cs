using System;
using System.Collections.Generic;
using System.Text;

namespace DDay.iCal.Validator
{
    public interface IValidationResultInfo
    {
        /// <summary>
        /// The name of the validation error, as per
        /// the icalvalid spec.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the type of validation error.
        /// </summary>
        ValidationResultInfoType Type { get; }
        
        /// <summary>
        /// Gets a message describing the validation error.
        /// </summary>
        string Message { get; }

        /// <summary>
        /// The line number in the code that is related
        /// to this validation error.
        /// </summary>
        int Line { get; }

        /// <summary>
        /// The column number in the code that is related
        /// to this validation error.
        /// </summary>
        int Col { get; }

        /// <summary>
        /// A list of possible resolutions for this problem.
        /// </summary>
        string[] Resolutions { get; }
    }

    public enum ValidationResultInfoType
    {
        Warning,
        Error,
        Fatal
    }
}
