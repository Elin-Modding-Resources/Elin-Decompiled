using System.Collections.Generic;
using UnityEngine;

public class AI_Shear : AI_TargetCard
{
	public override bool ShouldAllyAttack(Chara tg)
	{
		return tg != target;
	}

	public override string GetText(string str = "")
	{
		string[] list = Lang.GetList("fur");
		string text = list[Mathf.Clamp(target.c_fur / 10, 0, list.Length - 1)];
		return "AI_Shear".lang() + "(" + text + ")";
	}

	public override bool IsValidTC(Card c)
	{
		return c?.CanBeSheared() ?? false;
	}

	public override bool Perform()
	{
		target = Act.TC;
		return base.Perform();
	}

	public override IEnumerable<Status> Run()
	{
		yield return DoGoto(target);
		int furLv = GetFurLv(target.Chara);
		Progress_Custom seq = new Progress_Custom
		{
			canProgress = () => IsValidTC(target),
			onProgressBegin = delegate
			{
				owner.Say("shear_start", owner, target);
				if (EClass.rnd(5) == 0)
				{
					owner.Talk("goodBoy");
				}
			},
			onProgress = delegate(Progress_Custom p)
			{
				owner.LookAt(target);
				owner.PlaySound("shear");
				target.renderer.PlayAnime(AnimeID.Shiver);
				if (owner.Dist(target) > 1)
				{
					owner.TryMoveTowards(target.pos);
					if (owner == null)
					{
						p.Cancel();
					}
					else if (owner.Dist(target) > 1)
					{
						owner.Say("targetTooFar");
						p.Cancel();
					}
				}
			},
			onProgressComplete = delegate
			{
				Thing fur = GetFur(target.Chara);
				owner.Say("shear_end", owner, target, fur.Name);
				owner.Pick(fur, msg: false);
				owner.elements.ModExp(237, 50 * furLv);
				owner.stamina.Mod(-1);
				target.Chara.ModAffinity(owner, 1);
			}
		}.SetDuration((6 + furLv * 6) * 100 / (100 + owner.Tool.material.hardness * 2), 3);
		yield return Do(seq);
	}

	public static int GetFurLv(Chara c)
	{
		return Mathf.Clamp(c.c_fur / 10 + 1, 1, 5);
	}

	public static Thing GetFur(Chara c, int mod = 100)
	{
		int furLv = GetFurLv(c);
		string text = "fiber";
		string idMat = "wool";
		string text2 = c.id;
		if (!(text2 == "putty_snow"))
		{
			if (text2 == "putty_snow_gold")
			{
				idMat = "gold";
			}
			else if (!c.Chara.race.fur.IsEmpty())
			{
				string[] array = c.Chara.race.fur.Split('/');
				text = array[0];
				idMat = array[1];
			}
		}
		else
		{
			idMat = "cashmere";
		}
		Thing thing = ThingGen.Create(text, idMat);
		int num = mod * furLv + furLv * furLv * 10;
		int num2 = c.LV;
		if (c.IsInCombat || c.IsMinion)
		{
			Msg.Say("shear_penalty");
			num /= 2;
			num2 /= 2;
		}
		int num3 = 20 + thing.material.tier * 20;
		thing.SetNum(Mathf.Max(num / num3, 1) + EClass.rnd(furLv + 1));
		thing.SetEncLv(EClass.curve(num2, 30, 10) / 10);
		thing.elements.ModBase(2, EClass.curve(num2 / 10 * 10, 30, 10));
		c.c_fur = -5;
		return thing;
	}
}
