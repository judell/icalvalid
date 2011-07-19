namespace DDay.iCal.Validator
{
	public interface ILogger
	{
		void LogMessage(string message);

		void SetStatus(string status);
	}
}
