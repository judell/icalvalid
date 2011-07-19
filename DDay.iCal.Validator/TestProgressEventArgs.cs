using System;
using System.Collections.Generic;
using System.Text;

namespace DDay.iCal.Validator
{
    public class TestProgressEventArgs :
        EventArgs
    {
        public int TotalTests { get; set; }
        public int Passed { get; set; }
        public int Failed { get; set; }
        public int NotRun { get; set; }

        public TestProgressEventArgs()
        {
        }

        public TestProgressEventArgs(int totalTests, int passed, int failed, int notRun)
        {
            TotalTests = totalTests;
            Passed = passed;
            Failed = failed;
            NotRun = notRun;
        }
    }
}
