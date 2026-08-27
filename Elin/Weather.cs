using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

public class Weather : EClass
{
	[JsonObject(MemberSerialization.OptOut)]
	public class Forecast
	{
		public int duration;

		public Condition condition;
	}

	public enum Condition
	{
		Fine,
		Cloudy,
		Rain,
		RainHeavy,
		Snow,
		SnowHeavy,
		Ether,
		Blossom,
		None
	}

	public class WeatherForecast
	{
		public Date date;

		public Dictionary<Condition, int> cons = new Dictionary<Condition, int>();

		public void Add(Condition con, int h)
		{
			if (cons.ContainsKey(con))
			{
				cons[con] += h;
			}
			else
			{
				cons[con] = h;
			}
		}

		public void Finish()
		{
			if (cons.Count == 0)
			{
				return;
			}
			int num = cons.Sum((KeyValuePair<Condition, int> c) => c.Value);
			if (num == 0)
			{
				return;
			}
			foreach (Condition item in cons.Keys.ToList())
			{
				cons[item] = cons[item] * 100 / num;
			}
		}
	}

	private const int maxForecasts = 10;

	[JsonProperty]
	public Condition _currentCondition;

	[JsonProperty]
	public int duration = 8;

	[JsonProperty]
	public int lastRain;

	[JsonProperty]
	public List<Forecast> forecasts = new List<Forecast>();

	public Condition CurrentCondition
	{
		get
		{
			if (EClass._map.config.fixedCondition == Condition.None)
			{
				return _currentCondition;
			}
			return EClass._map.config.fixedCondition;
		}
	}

	public Season season => EClass.world.season;

	public bool IsHazard
	{
		get
		{
			if (CurrentCondition != Condition.Rain && CurrentCondition != Condition.RainHeavy && CurrentCondition != Condition.Snow && CurrentCondition != Condition.SnowHeavy)
			{
				return CurrentCondition == Condition.Ether;
			}
			return true;
		}
	}

	public bool IsFineOrCloudy
	{
		get
		{
			if (CurrentCondition != Condition.Fine)
			{
				return CurrentCondition == Condition.Cloudy;
			}
			return true;
		}
	}

	public bool IsRaining
	{
		get
		{
			if (CurrentCondition != Condition.Rain)
			{
				return CurrentCondition == Condition.RainHeavy;
			}
			return true;
		}
	}

	public bool IsSnowing
	{
		get
		{
			if (CurrentCondition != Condition.Snow)
			{
				return CurrentCondition == Condition.SnowHeavy;
			}
			return true;
		}
	}

	public bool IsBlossom
	{
		get
		{
			if (CurrentCondition != Condition.Blossom)
			{
				if (IsFineOrCloudy)
				{
					return EClass._map.config.blossom;
				}
				return false;
			}
			return true;
		}
	}

	public bool IsEther => CurrentCondition == Condition.Ether;

	public string GetName()
	{
		return GetName(CurrentCondition);
	}

	public string GetName(Condition condition)
	{
		return ("weather" + condition).lang();
	}

	public void RefreshWeather()
	{
		if (EClass._map.IsIndoor)
		{
			return;
		}
		bool flag = EClass._zone.IsSnowCovered;
		if (EClass._zone is Region)
		{
			EloMap elomap = EClass.scene.elomapActor.elomap;
			flag = elomap.GetTileInfo(EClass.pc.pos.x + elomap.minX, EClass.pc.pos.z + elomap.minY)?.IsSnow ?? false;
			if (EClass.world.season.isWinter)
			{
				flag = true;
			}
		}
		if (IsSnowing)
		{
			if (!flag)
			{
				_currentCondition = ((_currentCondition == Condition.Snow) ? Condition.Rain : Condition.RainHeavy);
			}
		}
		else if (IsRaining && flag)
		{
			_currentCondition = ((_currentCondition == Condition.Rain) ? Condition.Snow : Condition.SnowHeavy);
		}
	}

	public void OnChangeHour()
	{
		duration--;
		if (duration < 0)
		{
			SetConditionFromForecast();
		}
	}

	public Forecast GetForecast(Date date, Condition current)
	{
		Forecast forecast = new Forecast();
		forecast.condition = ((EClass.rnd(4) == 0) ? season.GetRandomWeather(date, current) : Condition.Fine);
		forecast.duration = EClass.rnd(24) + 10;
		date.AddHour(forecast.duration);
		return forecast;
	}

	public List<WeatherForecast> GetWeatherForecast()
	{
		RefreshForecasts();
		List<WeatherForecast> list = new List<WeatherForecast>();
		Date date = EClass.world.date.Copy();
		WeatherForecast weatherForecast = new WeatherForecast
		{
			date = date.Copy()
		};
		list.Add(weatherForecast);
		Forecast forecast = new Forecast
		{
			condition = _currentCondition,
			duration = duration
		};
		int num = forecast.duration;
		int num2 = -1;
		for (int i = 0; i < 10000; i++)
		{
			num--;
			weatherForecast.Add(forecast.condition, 1);
			if (num < 0)
			{
				num2++;
				if (forecasts.Count <= num2)
				{
					list.Remove(weatherForecast);
					break;
				}
				forecast = forecasts[num2];
				num = forecast.duration;
			}
			date.AddHour(1);
			if (date.day != weatherForecast.date.day)
			{
				weatherForecast.Finish();
				weatherForecast = new WeatherForecast
				{
					date = date.Copy()
				};
				list.Add(weatherForecast);
			}
		}
		return list;
	}

	public void RefreshForecasts()
	{
		if (forecasts.Count >= 10)
		{
			return;
		}
		Date date = EClass.world.date.Copy();
		Condition current = Condition.Fine;
		for (int i = 0; i < 10; i++)
		{
			if (forecasts.Count > i)
			{
				Forecast forecast = forecasts[i];
				date.AddHour(forecast.duration);
				current = forecast.condition;
			}
			else
			{
				Forecast forecast2 = GetForecast(date, current);
				forecasts.Add(forecast2);
				current = forecast2.condition;
			}
		}
	}

	public long GetTimeSinceLastRain()
	{
		if (CurrentCondition == Condition.Rain || CurrentCondition == Condition.RainHeavy)
		{
			return 0L;
		}
		return EClass.world.date.GetElapsedMins(lastRain);
	}

	public void SetRandomCondition()
	{
		SetCondition(Util.RandomEnum<Condition>());
	}

	public void SetConditionFromForecast(bool silent = false)
	{
		RefreshForecasts();
		Forecast forecast = forecasts[0];
		SetCondition(forecast.condition, forecast.duration, silent);
		forecasts.RemoveAt(0);
	}

	public void SetCondition(Condition condition, int _duration = 20, bool silent = false)
	{
		if (condition == Condition.Fine)
		{
			foreach (Chara member in EClass.pc.party.members)
			{
				if (member.HasElement(1558) && EClass.rnd(4) == 0)
				{
					condition = Condition.Rain;
					Msg.Say("drawRain", member);
					break;
				}
			}
		}
		if (EClass.world.season.isWinter)
		{
			switch (condition)
			{
			case Condition.Rain:
				condition = Condition.Snow;
				break;
			case Condition.RainHeavy:
				condition = Condition.SnowHeavy;
				break;
			}
		}
		else if (!EClass._zone.IsSnowZone && !EClass._map.IsIndoor)
		{
			switch (condition)
			{
			case Condition.Snow:
				condition = Condition.Rain;
				break;
			case Condition.SnowHeavy:
				condition = Condition.RainHeavy;
				break;
			}
		}
		if (_currentCondition == Condition.Rain || _currentCondition == Condition.RainHeavy)
		{
			lastRain = EClass.world.date.GetRaw();
		}
		duration = _duration;
		if (condition != _currentCondition)
		{
			_currentCondition = condition;
			RefreshWeather();
			Msg.Say("weather_" + condition);
			if (IsRaining)
			{
				EClass._zone.RainWater();
			}
			EClass.core.screen.RefreshSky();
		}
	}
}
