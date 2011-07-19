using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace DDay.iCal.Validator
{
    public class Debug
    {
        static Stopwatch _Stopwatch;

        [Conditional("DEBUG")]
        public void Initialize()
        {
            _Stopwatch = new Stopwatch();
            _Stopwatch.Start();
        }

        [Conditional("DEBUG")]
        public void WriteWithTimestamp(string msg)
        {            
            System.Diagnostics.Debug.Write(string.Format("{0:ssss.fffff}: {1}", _Stopwatch.Elapsed, msg));
        }

        [Conditional("DEBUG")]
        public void WriteLineWithTimestamp(string msg)
        {
            System.Diagnostics.Debug.WriteLine(string.Format("{0:ssss.fffff}: {1}", _Stopwatch.Elapsed, msg));
        }

        [Conditional("DEBUG")]
        public void Write(string msg)
        {
            System.Diagnostics.Debug.Write(msg);
        }

        [Conditional("DEBUG")]
        public void WriteLine(string msg)
        {
            System.Diagnostics.Debug.WriteLine(msg);
        }

        [Conditional("DEBUG")]
        public void Flush()
        {
            System.Diagnostics.Debug.Flush();
        }
    }
}
