using System;
using UnityEngine;

public class Check : EClass
{
	public enum Result
	{
		CriticalPass,
		Pass,
		Fail,
		CriticalFail
	}

	public enum Output
	{
		None,
		Default
	}

	public SourceCheck.Row _source;

	public string id;

	public float dcMod;

	public SourceCheck.Row source => _source ?? (_source = EClass.sources.checks.map[id]);

	public string mainElementName => EClass.sources.elements.map[source.element].GetName();

	public int baseDC => source.baseDC;

	public static Check Get(string id, float dcMod = 1f)
	{
		return new Check
		{
			id = id,
			dcMod = dcMod
		};
	}

	public string GetText(Chara p, Card tg, bool inDialog = false)
	{
		string text = "";
		int finalDC = GetFinalDC(p, tg);
		string text2 = ((finalDC > 20) ? "絶望的" : ((finalDC > 17) ? "ほぼ絶望的" : ((finalDC > 14) ? "とても難しい" : ((finalDC > 11) ? "難しい" : ((finalDC > 8) ? "普通" : ((finalDC > 5) ? "簡単" : ((finalDC > 2) ? "とても簡単" : "とても余裕")))))));
		if (finalDC <= 20 && finalDC <= 15 && finalDC <= 10)
		{
			_ = 5;
		}
		text = text + "DC " + GetDC(tg) + " " + mainElementName + " " + text2;
		return text + " " + source.element + "/" + EClass.sources.elements.map[source.element].alias;
	}

	public int GetDC(Card tg = null)
	{
		float num = (float)baseDC * dcMod;
		if (tg == null)
		{
			return (int)num;
		}
		num += source.lvMod * (float)tg.LV;
		if (source.targetElement != 0)
		{
			Element element = tg.elements.GetElement(source.element);
			if (element != null)
			{
				num += (float)element.Value;
				if (source.targetSubFactor > 0f && !element.source.aliasParent.IsEmpty())
				{
					Element element2 = tg.elements.GetElement(element.source.aliasParent);
					if (element2 != null)
					{
						num += source.targetSubFactor * (float)element2.Value;
					}
				}
			}
		}
		return (int)num;
	}

	public int GetFinalDC(Chara p, Card tg = null)
	{
		int num = GetDC(tg);
		if (source.element != 0)
		{
			Element element = p.elements.GetElement(source.element);
			if (element != null)
			{
				num -= element.Value;
				if (source.subFactor > 0f && !element.source.aliasParent.IsEmpty())
				{
					Element element2 = p.elements.GetElement(element.source.aliasParent);
					if (element2 != null)
					{
						num -= (int)(source.subFactor * (float)element2.Value);
					}
				}
			}
		}
		if (num > 14)
		{
			num = 14 + (int)Mathf.Sqrt(num - 14);
		}
		else if (num < 6)
		{
			num = 6 - (int)Mathf.Sqrt(6 - num);
		}
		return num;
	}

	public Result Perform(Chara p, Card tg = null)
	{
		int finalDC = GetFinalDC(p, tg);
		long num = Dice.Roll(1, 20);
		if (EClass.debug.logDice)
		{
			string text = "Check:" + source.id + " dc=" + finalDC + " roll=" + num + "  ";
			Debug.Log(num switch
			{
				20L => text + "CriticalPass", 
				1L => text + "CriticalFail", 
				_ => (num < finalDC) ? (text + "Fail") : (text + "Pass"), 
			});
		}
		switch (num)
		{
		case 20L:
			return Result.CriticalPass;
		case 1L:
			return Result.CriticalFail;
		default:
			if (num >= finalDC)
			{
				return Result.Pass;
			}
			return Result.Fail;
		}
	}

	public Result Perform(Chara p, Card tg, Action<Result> action)
	{
		Result result = Perform(p, tg);
		action?.Invoke(result);
		return result;
	}
}
