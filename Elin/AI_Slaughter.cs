using System.Collections.Generic;
using UnityEngine;

public class AI_Slaughter : AI_TargetCard
{
	public static bool slaughtering;

	public override bool CanManualCancel()
	{
		return true;
	}

	public override string GetText(string str = "")
	{
		string[] list = Lang.GetList("fur");
		string text = list[Mathf.Clamp(target.c_fur / 10, 0, list.Length - 1)];
		return "AI_Shear".lang() + "(" + text + ")";
	}

	public override bool IsValidTC(Card c)
	{
		return c?.ExistsOnMap ?? false;
	}

	public override bool Perform()
	{
		target = Act.TC;
		return base.Perform();
	}

	public override IEnumerable<Status> Run()
	{
		yield return DoGoto(target);
		target.Chara.AddCondition<ConWait>(1000, force: true);
		Progress_Custom seq = new Progress_Custom
		{
			canProgress = () => IsValidTC(target),
			onProgressBegin = delegate
			{
				target.PlaySound("slaughter");
				target.SetCensored(enable: true);
				owner.Say("disassemble_start", owner, target);
			},
			onProgress = delegate(Progress_Custom p)
			{
				owner.LookAt(target);
				target.renderer.PlayAnime(AnimeID.Shiver);
				target.Chara.AddCondition<ConWait>(1000, force: true);
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
				target.Chara.ModAffinity(owner, 1);
				target.SetCensored(enable: false);
				if (target.HaveFur())
				{
					Thing fur = AI_Shear.GetFur(target.Chara, 500);
					EClass._zone.AddCard(fur, target.pos);
				}
				slaughtering = true;
				target.Die();
				slaughtering = false;
				if (target.Chara.trait.IsUnique)
				{
					target.c_dateDeathLock = EClass.world.date.GetRaw() + 86400;
				}
				else
				{
					target.Chara.homeBranch.BanishMember(target.Chara, skipMsg: true);
				}
				owner.elements.ModExp(237, 250);
				owner.elements.ModExp(290, 100);
				EClass.pc.stamina.Mod(-3);
			}
		}.SetDuration(5000 / (100 + owner.Tool.material.hardness * 2), 3);
		yield return Do(seq);
	}

	public override void OnCancelOrSuccess()
	{
		if (target.ExistsOnMap)
		{
			target.Chara.RemoveCondition<ConWait>();
			target.SetCensored(enable: false);
		}
	}
}
