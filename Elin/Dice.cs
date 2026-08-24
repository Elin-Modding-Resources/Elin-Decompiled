using System;
using UnityEngine;

public class Dice
{
	public static Dice Null = new Dice();

	public int num;

	public int sides;

	public int bonus;

	public Card card;

	public static int MaxValue => 214748364;

	public static long Roll_Normal(long num, long sides, int bonus = 0)
	{
		double num2 = num;
		double num3 = sides;
		double num4 = num2 * (num3 + 1.0) / 2.0;
		double num5 = Math.Sqrt(num2 * (num3 * num3 - 1.0) / 12.0);
		double d = (double)(rnd(1000000) + 1) / 1000001.0;
		double num6 = (double)(rnd(1000000) + 1) / 1000001.0;
		double num7 = Math.Sqrt(-2.0 * Math.Log(d)) * Math.Cos(Math.PI * 2.0 * num6);
		double num8 = Math.Floor(num4 + num5 * num7 + 0.5);
		long num9 = ((sides != 0L && num > long.MaxValue / sides) ? long.MaxValue : (num * sides));
		if (num8 < (double)num)
		{
			num8 = num;
		}
		if (num8 > (double)num9)
		{
			num8 = num9;
		}
		long num10 = (long)num8;
		if (bonus > 0 && num10 > long.MaxValue - bonus)
		{
			return long.MaxValue;
		}
		if (bonus < 0 && num10 < long.MinValue - bonus)
		{
			return long.MinValue;
		}
		return num10 + bonus;
	}

	public static long Roll_Precise(int num, int sides, int bonus = 0)
	{
		long num2 = 0L;
		for (int i = 0; i < num; i++)
		{
			num2 += rnd(sides) + 1;
		}
		return num2 + bonus;
	}

	public static long Roll(int num, int sides, int bonus = 0, Card card = null)
	{
		int a = 1;
		bool flag = true;
		long num2 = 0L;
		if (card != null)
		{
			int num3 = card.Evalue(78);
			flag = num3 >= 0;
			a = 1 + Mathf.Abs(num3 / 100) + ((Mathf.Abs(num3 % 100) > rnd(100)) ? 1 : 0);
		}
		for (int i = 0; i < Mathf.Min(a, 20); i++)
		{
			long num4 = ((num >= 10) ? Roll_Normal(num, sides, bonus) : Roll_Precise(num, sides, bonus));
			if (i == 0 || (flag && num4 > num2) || (!flag && num4 < num2))
			{
				num2 = num4;
			}
		}
		return num2;
	}

	public static long RollMax(int num, int sides, int bonus = 0)
	{
		return (long)num * (long)sides + bonus;
	}

	public static int rnd(int a)
	{
		return Rand.Range(0, a);
	}

	public Dice(int _num = 0, int _sides = 0, int _bonus = 0, Card _card = null)
	{
		num = _num;
		sides = _sides;
		bonus = _bonus;
		card = _card;
	}

	public static Dice Parse(string raw)
	{
		Dice dice = new Dice();
		string[] array = raw.Split(',');
		if (array.Length != 0)
		{
			string[] array2 = array[0].Split('d');
			dice.num = int.Parse(array2[0]);
			dice.sides = int.Parse(array2[1]);
		}
		if (array.Length > 1)
		{
			dice.bonus = int.Parse(array[1]);
		}
		return dice;
	}

	public long Roll()
	{
		return Roll(num, sides, bonus, card);
	}

	public long RollMax()
	{
		return RollMax(num, sides, bonus);
	}

	public override string ToString()
	{
		return num + "d" + sides + ((bonus > 0) ? ("+" + bonus) : ((bonus < 0) ? (bonus.ToString() ?? "") : ""));
	}

	public static Dice Create(Element ele, Card c)
	{
		string key = ele.source.alias;
		if (!EClass.sources.calc.map.ContainsKey(key) && !ele.source.aliasRef.IsEmpty())
		{
			key = ele.source.alias.Split('_')[0] + "_";
		}
		if (!EClass.sources.calc.map.ContainsKey(key))
		{
			return null;
		}
		SourceCalc.Row row = EClass.sources.calc.map[key];
		int power = ele.GetPower(c);
		int ele2 = ((!ele.source.aliasParent.IsEmpty()) ? c.Evalue(ele.source.aliasParent) : 0);
		try
		{
			return new Dice(Mathf.Max(1, row.num.Calc(power, ele2)), Mathf.Max(1, row.sides.Calc(power, ele2)), row.bonus.Calc(power, ele2), c);
		}
		catch
		{
			Debug.Log(ele.id);
			return new Dice();
		}
	}

	public static Dice Create(string id, int power, Card c = null, Act act = null)
	{
		if (!EClass.sources.calc.map.TryGetValue(id, out var value) && (string.IsNullOrEmpty(act?.ID) || !EClass.sources.calc.map.TryGetValue(act.ID, out value)))
		{
			Debug.Log(id);
			return null;
		}
		int power2 = power;
		int ele = power / 10;
		if (act != null)
		{
			Element orCreateElement = c.elements.GetOrCreateElement(act.source.id);
			power2 = orCreateElement.GetPower(c);
			ele = ((!orCreateElement.source.aliasParent.IsEmpty()) ? c.Evalue(orCreateElement.source.aliasParent) : 0);
		}
		try
		{
			return new Dice(Mathf.Max(1, value.num.Calc(power2, ele)), Mathf.Max(1, value.sides.Calc(power2, ele)), value.bonus.Calc(power2, ele), c);
		}
		catch
		{
			return new Dice();
		}
	}
}
