using System;
using UnityEngine;

public class Stats : BaseStats
{
	public static StatsHunger Hunger = new StatsHunger
	{
		id = 0
	};

	public static StatsBurden Burden = new StatsBurden
	{
		id = 1
	};

	public static StatsStamina Stamina = new StatsStamina
	{
		id = 3
	};

	public static StatsSleepiness Sleepiness = new StatsSleepiness
	{
		id = 4
	};

	public static Stats Depression = new Stats
	{
		id = 5
	};

	public static Stats Bladder = new Stats
	{
		id = 6
	};

	public static StatsHygiene Hygiene = new StatsHygiene
	{
		id = 7
	};

	public static StatsMana Mana = new StatsMana
	{
		id = 8
	};

	public static StatsSAN SAN = new StatsSAN
	{
		id = 13
	};

	public int rawIndex;

	public int[] raw;

	public virtual int value
	{
		get
		{
			return raw[rawIndex];
		}
		set
		{
			raw[rawIndex] = value;
		}
	}

	public virtual int max
	{
		get
		{
			return 100;
		}
		set
		{
		}
	}

	public string name => base.source.GetText();

	public virtual bool TrackPhaseChange => false;

	public virtual int min => 0;

	public override int GetValue()
	{
		return value;
	}

	public override string ToString()
	{
		return name + " " + value + " " + GetPhase();
	}

	public override string GetText()
	{
		string text = base.source.GetTextArray("strPhase")[GetPhase()];
		if (!(text == "#"))
		{
			return text;
		}
		return "";
	}

	public override Color GetColor(Gradient g)
	{
		return g.Evaluate((float)value / (float)max);
	}

	public Stats Set(int[] _raw, int _rawIndex, Chara _CC)
	{
		raw = _raw;
		rawIndex = _rawIndex;
		BaseStats.CC = _CC;
		return this;
	}

	public override int GetPhase()
	{
		return base.source.phase[(int)Mathf.Clamp(10f * (float)value / (float)max, 0f, 9f)];
	}

	public virtual void Set(int a)
	{
		value = a;
		if (value < min)
		{
			value = min;
		}
		else if (value > max)
		{
			value = max;
		}
	}

	public virtual void Mod(int a)
	{
		if (BaseStats.CC.IsAgent)
		{
			return;
		}
		if (TrackPhaseChange)
		{
			int phase = GetPhase();
			value += a;
			if (value < min)
			{
				value = min;
			}
			else if (value > max)
			{
				value = max;
			}
			int phase2 = GetPhase();
			if (phase2 != phase)
			{
				OnChangePhase(phase2, phase);
			}
		}
		else
		{
			value += a;
			if (value < min)
			{
				value = min;
			}
			else if (value > max)
			{
				value = max;
			}
		}
	}

	public virtual void OnChangePhase(int phase, int lastPhase)
	{
		bool flag = phase > lastPhase;
		if (base.source.invert)
		{
			flag = !flag;
		}
		string[] array = (flag ? base.source.GetText("textPhase") : base.source.GetText("textPhase2")).Split(Environment.NewLine.ToCharArray());
		if (array.Length > phase)
		{
			if (flag)
			{
				Msg.SetColor("negative");
			}
			BaseStats.CC.Say(array[phase].Split('|').RandomItem(), BaseStats.CC);
			if (BaseStats.CC.ShouldShowMsg)
			{
				PopText();
			}
		}
	}
}
