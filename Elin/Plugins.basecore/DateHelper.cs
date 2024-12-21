using System;

public class DateHelper
{
	private const int SECOND = 1;

	private const int MINUTE = 60;

	private const int HOUR = 3600;

	private const int DAY = 86400;

	private const int MONTH = 2592000;

	public static string GetLastTime(long time)
	{
		TimeSpan timeSpan = new TimeSpan(DateTime.UtcNow.Ticks - time);
		double num = Math.Abs(timeSpan.TotalSeconds);
		if (num < 60.0)
		{
			if (timeSpan.Seconds != 1)
			{
				return timeSpan.Seconds + " seconds ago";
			}
			return "one second ago";
		}
		if (num < 120.0)
		{
			return "a minute ago";
		}
		if (num < 2700.0)
		{
			return timeSpan.Minutes + " minutes ago";
		}
		if (num < 5400.0)
		{
			return "an hour ago";
		}
		if (num < 86400.0)
		{
			return timeSpan.Hours + " hours ago";
		}
		if (num < 172800.0)
		{
			return "yesterday";
		}
		if (num < 2592000.0)
		{
			return timeSpan.Days + " days ago";
		}
		if (num < 31104000.0)
		{
			int num2 = Convert.ToInt32(Math.Floor((double)timeSpan.Days / 30.0));
			if (num2 > 1)
			{
				return num2 + " months ago";
			}
			return "one month ago";
		}
		int num3 = Convert.ToInt32(Math.Floor((double)timeSpan.Days / 365.0));
		if (num3 > 1)
		{
			return num3 + " years ago";
		}
		return "one year ago";
	}
}
