using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Affinity : EClass
{
	public static Chara CC;

	public int value;

	public int difficulty;

	public static List<Affinity> list => EClass.gamedata.affinities;

	public string Name => Lang.GetList("affinity")[list.IndexOf(this)];

	public static Affinity Get(Chara c)
	{
		CC = c;
		foreach (Affinity item in list)
		{
			if (c._affinity < item.value)
			{
				return item;
			}
		}
		return list.LastItem();
	}

	public bool CanForceTradeEquip()
	{
		return list.IndexOf(this) >= 6;
	}

	public bool CanInvite()
	{
		if (!EClass.debug.inviteAnytime)
		{
			return list.IndexOf(this) >= 6;
		}
		return true;
	}

	public bool CanMarry()
	{
		if (!EClass.debug.marryAnytime)
		{
			return list.IndexOf(this) >= 8;
		}
		return true;
	}

	public bool CanSleepBeside()
	{
		if (!EClass.debug.marryAnytime)
		{
			return list.IndexOf(this) >= 8;
		}
		return true;
	}

	public Thing OnGift(Thing t)
	{
		t.CheckJustCooked();
		Thing result = CC.AddThing(t.Thing);
		EClass.pc.PlaySound("build_resource");
		int num = 0;
		bool num2 = t.HasTag(CTAG.gift);
		bool flag = t.category.IsChildOf(CC.GetFavCat());
		bool flag2 = t.id == CC.GetFavFood().id;
		if (EClass.debug.alwaysFavFood && t.trait is TraitFood)
		{
			flag2 = true;
		}
		num = Mathf.Clamp(t.GetPrice() / (flag2 ? 10 : (flag ? 20 : 200)), 0, 50) + (flag2 ? 20 : (flag ? 5 : 0));
		num = num * (100 + (t.HasElement(757) ? 50 : 0)) / (100 + CC.LV * 10);
		if (num2)
		{
			num += 100;
			CC.Say("give_ring", CC);
			CC.Talk("thanks3");
		}
		else if (flag2 || num > 20)
		{
			CC.Talk("thanks3");
		}
		else if (flag || num > 10)
		{
			CC.Talk("thanks");
		}
		else
		{
			CC.Talk("thanks2");
		}
		CC.ModAffinity(EClass.pc, num);
		return result;
	}

	public void OnTalkRumor()
	{
		bool flag = EClass.rnd(60 + EClass.pc.CHA * 2 + EClass.pc.Evalue(291) * 3) > 50 + difficulty + EClass.rnd(CC.CHA + 1);
		CC.ModAffinity(EClass.pc, flag ? (EClass.rnd(4) + 1) : (-EClass.rnd(4) - 1), show: false);
		if (!EClass.debug.unlimitedInterest)
		{
			CC.interest -= 10 + EClass.rnd(10);
		}
		EClass.pc.ModExp(291, 20);
	}

	public int Mod(int a)
	{
		if (a < 0)
		{
			CC._affinity += a;
			return a;
		}
		int num = 0;
		for (int i = 0; i < a; i++)
		{
			Affinity affinity = Get(CC);
			if (EClass.rnd(100 + affinity.difficulty) < 100)
			{
				CC._affinity++;
				num++;
			}
		}
		return num;
	}
}
