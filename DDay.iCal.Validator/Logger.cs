using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace DDay.iCal.Validator
{

	public class ConsoleLogger : ILogger
	{

		public void LogMessage(string message)
		{
			Console.WriteLine(message);
		}

		public void SetStatus(string status)
		{
			Console.WriteLine("status: " + status);
		}

	}

	public class NullLogger : ILogger
	{

		public void LogMessage(string message)
		{
		}

		public void SetStatus(string status)
		{
		}

	}

}
