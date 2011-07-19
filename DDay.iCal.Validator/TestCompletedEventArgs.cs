using System;
using System.Collections.Generic;
using System.Text;

namespace DDay.iCal.Validator
{
    public class TestCompletedEventArgs :
        EventArgs
    {
        public ITestResult Result { get; set; }

        public TestCompletedEventArgs(ITestResult result)
        {
            Result = result;
        }
    }
}
