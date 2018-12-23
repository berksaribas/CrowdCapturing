namespace Util
{
	public static class TimeHelper
	{
		public static string ConvertSecondsToString(float sec)
		{
			var intSeconds = (int) sec;
			
			var hours = intSeconds / 3600;
			var minutes = (intSeconds - hours * 3600) / 60;
			var seconds = intSeconds - hours * 3600 - minutes * 60;

			return hours.ToString("00") + ":" + minutes.ToString("00") + ":" + seconds.ToString("00");
		}
	}
}