using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class ConStrife : BaseBuff
{
	[JsonProperty]
	public int exp;

	[JsonProperty]
	public int lv;

	[JsonProperty]
	public int turn;

	public override string TextDuration => "Lv." + lv;

	public override bool WillOverride => false;

	public int ExpToNext => (lv + 1) * (lv + 1);

	public void AddKill(Chara c)
	{
		if (c.IsPCFactionOrMinion)
		{
			if (c.IsMinion)
			{
				exp += 2;
			}
			else
			{
				exp += 30;
			}
		}
		else
		{
			exp++;
		}
		while (exp >= ExpToNext)
		{
			exp -= ExpToNext;
			lv++;
		}
		SetTurn();
	}

	public Dice GetDice()
	{
		return new Dice(1, 1 + Mathf.Min(lv, 10) * 2);
	}

	public void SetTurn()
	{
		turn = Mathf.Max(100 - lv * 10, 10);
	}

	public override void Tick()
	{
		turn--;
		if (turn < 0)
		{
			lv--;
			if (lv >= 1)
			{
				SetTurn();
				exp = ExpToNext / 2;
			}
			else
			{
				Kill();
			}
		}
	}

	public override void OnWriteNote(List<string> list)
	{
		list.Add("hintStrife".lang(lv.ToString() ?? "", exp + "/" + ExpToNext, GetDice().ToString()));
	}
}
