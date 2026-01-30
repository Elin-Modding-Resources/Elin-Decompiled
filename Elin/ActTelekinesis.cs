using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActTelekinesis : Spell
{
	public override bool ShowMapHighlight => true;

	public override int PerformDistance => 20;

	public override bool IsHostileAct
	{
		get
		{
			if (Act.TC != null)
			{
				return Act.TC.isNPCProperty;
			}
			return false;
		}
	}

	public override string GetText(string str = "")
	{
		string text = " (" + Lang.GetList("skillDiff")[GetDifficulty()] + ")";
		return base.GetText(str) + text;
	}

	public override void OnMarkMapHighlights()
	{
		if (!EClass.scene.mouseTarget.pos.IsValid || EClass.scene.mouseTarget.card == null)
		{
			return;
		}
		Point pos = EClass.scene.mouseTarget.pos;
		List<Point> list = ListPath(EClass.pc.pos, pos);
		foreach (Point item in list)
		{
			item.SetHighlight(list.LastItem().Equals(item) ? 8 : 2);
		}
	}

	public override bool CanPerform()
	{
		if (!Act.CC.IsPC)
		{
			return false;
		}
		Act.TC = EClass.scene.mouseTarget.card;
		if (Act.TC == null || Act.TC.pos.Equals(Act.CC.pos))
		{
			return false;
		}
		if (!Act.TC.trait.CanBeHeld || !Act.TC.trait.CanBeTeleported || Act.TC.IsMultisize)
		{
			return false;
		}
		if (ListPath(Act.CC.pos, Act.TC.pos).Count <= 1)
		{
			return false;
		}
		Act.TP.Set(Act.TC.pos);
		return base.CanPerform();
	}

	public List<Point> ListPath(Point start, Point end)
	{
		List<Point> list = new List<Point>();
		List<Point> list2 = EClass._map.ListPointsInLine(start, end, 20, returnOnBlocked: false);
		bool flag = start.Distance(end) != 1;
		if (flag)
		{
			list2.Reverse();
		}
		foreach (Point item in list2)
		{
			if (!item.Equals(start) && (!flag || start.Distance(item) <= start.Distance(end)) && item.IsInBounds)
			{
				if (!item.Equals(end) && (item.IsBlocked || (item.HasChara && item.FirstChara.IsMultisize)))
				{
					break;
				}
				list.Add(item);
			}
		}
		return list;
	}

	public int GetWeightLimit()
	{
		long num = GetPower(Act.CC);
		if (num * num <= int.MaxValue)
		{
			return (int)(num * num);
		}
		return int.MaxValue;
	}

	public int GetDifficulty()
	{
		int num = Act.TC.SelfWeight / GetWeightLimit();
		ConATField condition = Act.TC.GetCondition<ConATField>();
		if (condition != null)
		{
			num += condition.value;
		}
		return Mathf.Clamp(num / 33, 0, 3);
	}

	public void DoDamage(Card c, int dist = 1, int mtp = 100)
	{
		long num = Dice.Create("SpTelekinesis", GetPower(Act.CC), Act.CC, this).Roll();
		num = num * (50 + dist * 25) / 100;
		if (Act.TC.SelfWeight > 1000)
		{
			num = num * (100 + (int)Mathf.Sqrt(Act.TC.SelfWeight / 10) / 2) / 100;
		}
		num = num * mtp / 100;
		Act.CC.DoHostileAction(c);
		c.DamageHP(num, 925, 100, AttackSource.Throw, Act.CC);
	}

	public override bool Perform()
	{
		if (GetDifficulty() * 33 > EClass.rnd(100))
		{
			Act.TC.PlayEffect("Element/ball_Gravity");
			Act.TC.PlaySound("gravity");
			Act.CC.Say("tooHeavy", Act.TC);
			return true;
		}
		bool flag = Act.TC.Dist(Act.CC) != 1;
		List<Point> list = ListPath(Act.CC.pos, Act.TC.pos);
		Act.CC.PlaySound("telekinesis");
		Act.CC.PlayEffect("telekinesis");
		if (EClass._zone.IsCrime(Act.CC, this))
		{
			Act.CC.pos.TryWitnessCrime(Act.CC, Act.TC.Chara, 5, (Chara c) => EClass.rnd(3) == 0);
		}
		Point point = list.Last();
		for (int i = 0; i < list.Count; i++)
		{
			Point point2 = list[i];
			Effect.Get("telekinesis2").Play(0.1f * (float)i, point2);
			if (point2.Equals(Act.TC.pos) || !point2.HasChara)
			{
				continue;
			}
			foreach (Chara item in point2.ListCharas())
			{
				Act.TC.Kick(item, ignoreSelf: true, karmaLoss: false);
				Act.TC.pos.PlayEffect("vanish");
				if (!flag && (item.isChara || item.trait.CanBeAttacked))
				{
					DoDamage(item, i, 50);
				}
			}
		}
		if (point.HasChara)
		{
			point.FirstChara.MoveImmediate(Act.TC.pos);
		}
		Act.TC.MoveImmediate(point, focus: true, cancelAI: false);
		Act.TC.pos.PlayEffect("telekinesis");
		if (!flag)
		{
			Act.TC.pos.PlayEffect("vanish");
			Act.TC.pos.PlaySound("rush");
			if (Act.TC.isChara || Act.TC.trait.CanBeAttacked)
			{
				DoDamage(Act.TC, list.Count);
			}
		}
		if (!flag && Act.TC.isChara && Act.TC.IsAliveInCurrentZone)
		{
			Act.TC.Chara.AddCondition<ConATField>();
		}
		return true;
	}
}
