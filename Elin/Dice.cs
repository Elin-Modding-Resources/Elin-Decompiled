using UnityEngine;

public class Dice
{
	public static Dice Null = new Dice();

	public int num;

	public int sides;

	public int bonus;

	public Card card;

	public static int Roll(int num, int sides, int bonus = 0, Card card = null)
	{
		int a = 1;
		bool flag = true;
		int num2 = 0;
		if (card != null)
		{
			int num3 = card.Evalue(78);
			flag = num3 >= 0;
			a = 1 + Mathf.Abs(num3 / 100) + ((Mathf.Abs(num3 % 100) > rnd(100)) ? 1 : 0);
		}
		for (int i = 0; i < Mathf.Min(a, 20); i++)
		{
			int num4 = Roll();
			if (i == 0 || (flag && num4 > num2) || (!flag && num4 < num2))
			{
				num2 = num4;
			}
		}
		return num2;
		int Roll()
		{
			int num5 = 0;
			for (int j = 0; j < num; j++)
			{
				num5 += rnd(sides) + 1;
			}
			return num5 + bonus;
		}
	}

	public static int RollMax(int num, int sides, int bonus = 0)
	{
		return num * sides + bonus;
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

	public int Roll()
	{
		return Roll(num, sides, bonus, card);
	}

	public int RollMax()
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
		if (!EClass.sources.calc.map.ContainsKey(id))
		{
			Debug.Log(id);
			return null;
		}
		SourceCalc.Row row = EClass.sources.calc.map[id];
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
			return new Dice(Mathf.Max(1, row.num.Calc(power2, ele)), Mathf.Max(1, row.sides.Calc(power2, ele)), row.bonus.Calc(power2, ele), c);
		}
		catch
		{
			return new Dice();
		}
	}
}
