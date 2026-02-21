using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class Season : EClass
{
	public enum ID
	{
		Spring,
		Summer,
		Autumn,
		Winter
	}

	public GameDate date => EClass.world.date;

	public ID Current
	{
		get
		{
			int month = date.month;
			if (month >= 3 && month <= 5)
			{
				return ID.Spring;
			}
			if (month >= 6 && month <= 8)
			{
				return ID.Summer;
			}
			if (month >= 9 && month <= 11)
			{
				return ID.Autumn;
			}
			return ID.Winter;
		}
	}

	public bool isSpring
	{
		get
		{
			if (date.month >= 3)
			{
				return date.month <= 5;
			}
			return false;
		}
	}

	public bool isSummer
	{
		get
		{
			if (date.month >= 6)
			{
				return date.month <= 8;
			}
			return false;
		}
	}

	public bool isAutumn
	{
		get
		{
			if (date.month >= 9)
			{
				return date.month <= 11;
			}
			return false;
		}
	}

	public bool isWinter
	{
		get
		{
			if (date.month < 12)
			{
				return date.month <= 2;
			}
			return true;
		}
	}

	public Weather.Condition GetRandomWeather(Date date, Weather.Condition current)
	{
		if (EClass.rnd(3) == 0)
		{
			return Weather.Condition.Cloudy;
		}
		if (EClass.rnd(4) == 0)
		{
			return Weather.Condition.Rain;
		}
		if (EClass.rnd(5) == 0)
		{
			return Weather.Condition.RainHeavy;
		}
		if (EClass.rnd(6) == 0)
		{
			return Weather.Condition.Snow;
		}
		return Weather.Condition.Fine;
	}

	public void Next()
	{
		if (isSpring)
		{
			date.month = 6;
		}
		else if (isSummer)
		{
			date.month = 9;
		}
		else if (isAutumn)
		{
			date.month = 12;
		}
		else
		{
			date.month = 3;
		}
	}
}
