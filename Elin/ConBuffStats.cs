using System.Collections.Generic;
using UnityEngine;

public class ConBuffStats : Condition
{
	public override string Name => (isDebuff ? "debuff" : "buff").lang();

	public override ConditionType Type
	{
		get
		{
			if (!isDebuff)
			{
				return ConditionType.Buff;
			}
			return ConditionType.Debuff;
		}
	}

	public bool isDebuff => base.refVal2 == 222;

	public override bool WillOverride => true;

	public override bool AllowMultipleInstance => true;

	public override bool UseElements => true;

	public override int EvaluateTurn(int p)
	{
		if (base.refVal2 == 267)
		{
			return 7;
		}
		return base.EvaluateTurn(p) * ((base.refVal == 79) ? 50 : 100) / 100;
	}

	public override int GetPhase()
	{
		return 0;
	}

	public override Color GetColor(SkinColorProfile c)
	{
		if (!isDebuff)
		{
			return c.textGood;
		}
		return c.textBad;
	}

	public override string GetPhaseStr()
	{
		return GetText();
	}

	public override string GetText()
	{
		string[] list = Lang.GetList("buff_" + EClass.sources.elements.map[base.refVal].alias);
		if (list != null)
		{
			if (!isDebuff)
			{
				return list[0];
			}
			return list[1];
		}
		return base.GetText();
	}

	public override void PlayEffect()
	{
		if (!Condition.ignoreEffect)
		{
			owner.PlaySound(isDebuff ? "debuff" : "buff");
			owner.PlayEffect(isDebuff ? "debuff" : "buff");
			owner.Say(isDebuff ? "buffStats_curse" : "buffStats", owner, EClass.sources.elements.map[base.refVal].GetName().ToLower());
		}
	}

	public override bool CanStack(Condition c)
	{
		if (c is ConBuffStats conBuffStats)
		{
			return conBuffStats.refVal == base.refVal;
		}
		return true;
	}

	public override void SetOwner(Chara _owner, bool onDeserialize = false)
	{
		base.SetOwner(_owner);
		elements.SetBase(base.refVal, CalcValue() * ((!isDebuff) ? 1 : (-1)));
		elements.SetParent(owner);
	}

	public int CalcValue()
	{
		if (base.refVal2 == 267)
		{
			return 100 + (int)Mathf.Sqrt(base.power) * 2;
		}
		if (base.refVal == 79)
		{
			return (int)Mathf.Max(5f, Mathf.Sqrt(base.power) * 1.5f + 20f);
		}
		return (int)Mathf.Max(5f, Mathf.Sqrt(base.power) * 2f - 15f);
	}

	public override void OnWriteNote(List<string> list)
	{
		list.Add((isDebuff ? "hintDebuffStats" : "hintBuffStats").lang(base.sourceElement.GetName(), CalcValue().ToString() ?? ""));
	}
}
