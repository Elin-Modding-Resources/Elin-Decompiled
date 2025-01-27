using Newtonsoft.Json;

public class Date : EClass
{
	public enum TextFormat
	{
		Default,
		Log,
		Widget,
		Schedule,
		Travel,
		YearMonthDay,
		LogPlusYear
	}

	public const int ShippingHour = 5;

	[JsonProperty]
	public int[] raw = new int[6];

	public const int HourToken = 60;

	public const int DayToken = 1440;

	public const int MonthToken = 43200;

	public const int YearToken = 518400;

	public const int HourTokenReal = 60;

	public const int DayTokenReal = 1440;

	public const int MonthTokenReal = 46080;

	public const int YearTokenReal = 552960;

	public int year
	{
		get
		{
			return raw[0];
		}
		set
		{
			raw[0] = value;
		}
	}

	public int month
	{
		get
		{
			return raw[1];
		}
		set
		{
			raw[1] = value;
		}
	}

	public int day
	{
		get
		{
			return raw[2];
		}
		set
		{
			raw[2] = value;
		}
	}

	public int hour
	{
		get
		{
			return raw[3];
		}
		set
		{
			raw[3] = value;
		}
	}

	public int min
	{
		get
		{
			return raw[4];
		}
		set
		{
			raw[4] = value;
		}
	}

	public int sec
	{
		get
		{
			return raw[5];
		}
		set
		{
			raw[5] = value;
		}
	}

	public bool IsDay => !IsNight;

	public bool IsNight
	{
		get
		{
			if (hour < 19)
			{
				return hour <= 5;
			}
			return true;
		}
	}

	public PeriodOfDay periodOfDay
	{
		get
		{
			if (hour >= 5 && hour <= 6)
			{
				return PeriodOfDay.Dawn;
			}
			if (hour >= 7 && hour <= 17)
			{
				return PeriodOfDay.Day;
			}
			if (hour >= 18 && hour <= 19)
			{
				return PeriodOfDay.Dusk;
			}
			return PeriodOfDay.Night;
		}
	}

	public string NameMonth => month.ToString() ?? "";

	public string NameMonthShort => month.ToString() ?? "";

	public string NameTime => periodOfDay.ToString().lang();

	public bool IsSpring
	{
		get
		{
			if (month >= 3)
			{
				return month <= 5;
			}
			return false;
		}
	}

	public bool IsSummer
	{
		get
		{
			if (month >= 6)
			{
				return month <= 8;
			}
			return false;
		}
	}

	public bool IsAutumn
	{
		get
		{
			if (month >= 9)
			{
				return month <= 11;
			}
			return false;
		}
	}

	public bool IsWinter
	{
		get
		{
			if (month < 12)
			{
				return month <= 2;
			}
			return true;
		}
	}

	public Date Copy()
	{
		return new Date
		{
			year = year,
			month = month,
			day = day,
			hour = hour,
			min = min,
			sec = sec
		};
	}

	public override string ToString()
	{
		return GetText(TextFormat.Log);
	}

	public void AddHour(int a)
	{
		hour += a;
		while (hour >= 24)
		{
			hour -= 24;
			AddDay(1);
		}
	}

	public void AddDay(int a)
	{
		day += a;
		while (day > 30)
		{
			day -= 30;
			AddMonth(1);
		}
	}

	public void AddMonth(int a)
	{
		month += a;
		while (month > 12)
		{
			month -= 12;
			year++;
		}
	}

	public string GetText(TextFormat format)
	{
		switch (format)
		{
		case TextFormat.LogPlusYear:
			return year + ", " + month + "/" + day + " " + ((hour < 10) ? "0" : "") + hour + ":" + ((min < 10) ? "0" : "") + min;
		case TextFormat.Log:
			return month + "/" + day + " " + hour + ":" + min;
		case TextFormat.Widget:
			return "dateYearMonthDay".lang(year.ToString() ?? "", month.ToString() ?? "", day.ToString() ?? "");
		case TextFormat.Schedule:
			return "dateSchedule".lang(NameMonth, day.ToString() ?? "");
		case TextFormat.Travel:
		{
			string text = "_short";
			if (format == TextFormat.Travel)
			{
				return "travelDate".lang(year.ToString() ?? "", month.ToString() ?? "", day.ToString() ?? "");
			}
			string text2 = "";
			if (day != 0)
			{
				text2 = text2 + day + Lang.Get("wDay" + text);
			}
			if (hour != 0)
			{
				text2 = text2 + hour + Lang.Get("wHour" + text);
			}
			if (min != 0)
			{
				text2 = text2 + min + Lang.Get("wMin" + text);
			}
			return text2 + sec + Lang.Get("wSec" + text);
		}
		case TextFormat.YearMonthDay:
			return "dateYearMonthDay".lang(year.ToString() ?? "", month.ToString() ?? "", day.ToString() ?? "");
		default:
			return "Day " + day + " " + hour + ":" + min;
		}
	}

	public static string GetText(int raw, TextFormat format)
	{
		return ToDate(raw).GetText(format);
	}

	public static string GetText(int hour)
	{
		if (hour < 0)
		{
			return "dateDayVoid".lang();
		}
		if (hour > 24)
		{
			return "dateDay".lang((hour / 24).ToString() ?? "");
		}
		return "dateHour".lang(hour.ToString() ?? "");
	}

	public static string GetText2(int hour)
	{
		if (hour < 0)
		{
			return "-";
		}
		if (hour > 24)
		{
			return hour / 24 + "d";
		}
		return hour + "h";
	}

	public int GetRawReal(int offsetHours = 0)
	{
		return min + (hour + offsetHours) * 60 + day * 1440 + month * 46080 + year * 552960;
	}

	public int GetRaw(int offsetHours = 0)
	{
		return min + (hour + offsetHours) * 60 + day * 1440 + month * 43200 + year * 518400;
	}

	public int GetRawDay()
	{
		return day * 1440 + month * 43200 + year * 518400;
	}

	public bool IsExpired(int time)
	{
		return time - GetRaw() < 0;
	}

	public int GetRemainingHours(int rawDeadLine)
	{
		return (rawDeadLine - GetRaw()) / 60;
	}

	public int GetRemainingSecs(int rawDeadLine)
	{
		return rawDeadLine - GetRaw();
	}

	public int GetElapsedMins(int rawDate)
	{
		return GetRaw() - rawDate;
	}

	public int GetElapsedHour(int rawDate)
	{
		if (rawDate != 0)
		{
			return (GetRaw() - rawDate) / 60;
		}
		return 0;
	}

	public static string SecToDate(int sec)
	{
		return sec / 60 / 60 + "時間 " + sec / 60 % 60 + "分 " + sec % 60 + "秒";
	}

	public static string MinToDayAndHour(int min)
	{
		int num = min / 60;
		int num2 = num / 24;
		if (num == 0)
		{
			return min + "分";
		}
		if (num2 != 0)
		{
			return num2 + "日と" + num % 24 + "時間";
		}
		return num + "時間";
	}

	public static int[] GetDateArray(int raw)
	{
		return new int[5]
		{
			raw % 60,
			raw / 60 % 24,
			raw / 60 / 24 % 30,
			raw / 60 / 24 / 30 % 12,
			raw / 60 / 24 / 30 / 12
		};
	}

	public static Date ToDate(int raw)
	{
		int[] dateArray = GetDateArray(raw);
		int num = dateArray[4];
		int num2 = dateArray[3];
		int num3 = dateArray[2];
		if (num2 == 0)
		{
			num2 = 12;
			num--;
		}
		if (num3 == 0)
		{
			num3 = 30;
			num2--;
		}
		if (num2 == 0)
		{
			num2 = 12;
			num--;
		}
		return new Date
		{
			min = dateArray[0],
			hour = dateArray[1],
			day = num3,
			month = num2,
			year = num
		};
	}
}
