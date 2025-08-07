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
		return "AI_Slaughter".lang() + "(" + text + ")";
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
		if (target != owner)
		{
			target.Chara.AddCondition<ConWait>(1000, force: true);
		}
		Progress_Custom seq = new Progress_Custom
		{
			canProgress = () => IsValidTC(target),
			onProgressBegin = delegate
			{
				target.PlaySound("slaughter");
				target.SetCensored(enable: true);
				owner.Say("disassemble_start", owner, owner.Tool, target.Name);
			},
			onProgress = delegate(Progress_Custom p)
			{
				owner.LookAt(target);
				target.renderer.PlayAnime(AnimeID.Shiver);
				if (target != owner)
				{
					target.Chara.AddCondition<ConWait>(1000, force: true);
				}
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
				bool num = target.HasElement(1237) || target.Chara.race.id == "cat";
				target.pos.PlayEffect("revive");
				target.Chara.ModAffinity(owner, -50);
				owner.ShowEmo(Emo.love);
				target.SetCensored(enable: false);
				if (target.HaveFur())
				{
					Thing fur = AI_Shear.GetFur(target.Chara, 500);
					EClass._zone.AddCard(fur, target.pos);
				}
				slaughtering = true;
				target.SetSale(sale: false);
				if (target.IsPCParty && !target.IsPC)
				{
					if (target.Chara.host != null)
					{
						ActRide.Unride(target.Chara.host, target.Chara.host.parasite == target.Chara, talk: false);
					}
					EClass.pc.party.RemoveMember(target.Chara);
				}
				target.Die();
				Msg.Say("goto_heaven", target);
				slaughtering = false;
				if (!target.IsPC)
				{
					if (target.Chara.trait.IsUnique)
					{
						target.c_dateDeathLock = EClass.world.date.GetRaw() + 86400;
					}
					else
					{
						target.Chara.c_uniqueData = null;
						target.Chara.homeBranch.BanishMember(target.Chara, skipMsg: true);
					}
				}
				if (owner != null)
				{
					owner.elements.ModExp(290, 200f);
				}
				if (!EClass.pc.isDead)
				{
					EClass.pc.stamina.Mod(-3);
				}
				if (num)
				{
					Msg.Say("killcat");
					EClass.player.ModKarma(-3);
				}
			}
		}.SetDuration(6000 / (100 + owner.Tool.material.hardness * 2), 3);
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
