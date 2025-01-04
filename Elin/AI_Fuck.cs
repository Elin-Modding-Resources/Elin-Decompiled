using System.Collections.Generic;
using UnityEngine;

public class AI_Fuck : AIAct
{
	public enum FuckType
	{
		fuck,
		tame
	}

	public Chara target;

	public bool sell;

	public bool bitch;

	public bool succubus;

	public bool ntr;

	public int maxProgress;

	public int progress;

	public int totalAffinity;

	public virtual FuckType Type => FuckType.fuck;

	public override bool PushChara => false;

	public override bool IsAutoTurn => true;

	public override TargetType TargetType => TargetType.Chara;

	public override int MaxProgress => maxProgress;

	public override int CurrentProgress => progress;

	public override bool CancelOnAggro => !ntr;

	public override bool CancelWhenDamaged => !ntr;

	public override IEnumerable<Status> Run()
	{
		if (target == null)
		{
			foreach (Chara chara in EClass._map.charas)
			{
				if (!chara.IsHomeMember() && !chara.IsDeadOrSleeping && chara.Dist(owner) <= 5)
				{
					target = chara;
					break;
				}
			}
		}
		if (target == null)
		{
			yield return Cancel();
		}
		Chara cc = (sell ? target : owner);
		Chara tc = (sell ? owner : target);
		int destDist = ((Type == FuckType.fuck) ? 1 : 1);
		yield return DoGoto(target.pos, destDist);
		cc.Say(Type.ToString() + "_start", cc, tc);
		isFail = () => !tc.IsAliveInCurrentZone || tc.Dist(owner) > 3;
		if (Type == FuckType.tame)
		{
			cc.SetTempHand(1104, -1);
		}
		maxProgress = 25;
		if (succubus)
		{
			cc.Talk("seduce");
		}
		for (int i = 0; i < maxProgress; i++)
		{
			progress = i;
			yield return DoGoto(target.pos, destDist);
			switch (Type)
			{
			case FuckType.fuck:
				cc.LookAt(tc);
				tc.LookAt(cc);
				switch (i % 4)
				{
				case 0:
					cc.renderer.PlayAnime(AnimeID.Attack, tc);
					if (EClass.rnd(3) == 0 || sell)
					{
						cc.Talk("tail");
					}
					break;
				case 2:
					tc.renderer.PlayAnime(AnimeID.Shiver);
					if (EClass.rnd(3) == 0)
					{
						tc.Talk("tailed");
					}
					break;
				}
				if (EClass.rnd(3) == 0 || sell)
				{
					target.AddCondition<ConWait>(50, force: true);
				}
				break;
			case FuckType.tame:
			{
				int num = 100;
				if (!tc.race.IsAnimal)
				{
					num += 50;
				}
				if (tc.race.IsHuman)
				{
					num += 50;
				}
				if (tc.IsInCombat)
				{
					num += 100;
				}
				if (tc == cc)
				{
					num = 50;
				}
				else if (tc.affinity.CurrentStage < Affinity.Stage.Intimate && EClass.rnd(6 * num / 100) == 0)
				{
					tc.AddCondition<ConFear>(60);
				}
				tc.interest -= (tc.IsPCFaction ? 20 : (2 * num / 100));
				if (i == 0 || i == 10)
				{
					cc.Talk("goodBoy");
				}
				if (i % 5 == 0)
				{
					tc.PlaySound("brushing");
					int num2 = cc.CHA / 2 + cc.Evalue(237) - tc.CHA * 2;
					int num3 = ((EClass.rnd(cc.CHA / 2 + cc.Evalue(237)) <= EClass.rnd(tc.CHA * num / 100)) ? (-5 + ((!tc.IsPCFaction) ? Mathf.Clamp(num2 / 10, -30, 0) : 0)) : (5 + Mathf.Clamp(num2 / 20, 0, 20)));
					int a = 20;
					if (tc.IsPCFactionOrMinion && tc.affinity.CurrentStage >= Affinity.Stage.Love)
					{
						num3 = ((EClass.rnd(3) == 0) ? 4 : 0);
						a = 10;
					}
					totalAffinity += num3;
					tc.ModAffinity(EClass.pc, num3, show: true, showOnlyEmo: true);
					cc.elements.ModExp(237, a);
					if (EClass.rnd(4) == 0)
					{
						cc.stamina.Mod(-1);
					}
				}
				break;
			}
			}
		}
		Finish();
	}

	public void Finish()
	{
		Chara chara = (sell ? target : owner);
		Chara chara2 = (sell ? owner : target);
		if (chara.isDead || chara2.isDead)
		{
			return;
		}
		bool flag = EClass.rnd(2) == 0;
		switch (Type)
		{
		case FuckType.fuck:
		{
			for (int i = 0; i < 2; i++)
			{
				Chara chara3 = ((i == 0) ? chara : chara2);
				chara3.RemoveCondition<ConDrunk>();
				if (EClass.rnd(15) == 0 && !chara3.HasElement(1216))
				{
					chara3.AddCondition<ConDisease>(200);
				}
				chara3.ModExp(77, 250);
				chara3.ModExp(71, 250);
				chara3.ModExp(75, 250);
			}
			if (!chara2.HasElement(1216))
			{
				if (EClass.rnd(5) == 0)
				{
					chara2.AddCondition<ConParalyze>(500);
				}
				if (EClass.rnd(3) == 0)
				{
					chara2.AddCondition<ConInsane>(100 + EClass.rnd(100));
				}
			}
			int num3 = CalcMoney.Whore(chara2, chara);
			chara.Talk("tail_after");
			bool flag3 = false;
			if (succubus)
			{
				chara.ShowEmo(Emo.love);
				chara2.ShowEmo(Emo.love);
				EClass.player.forceTalk = true;
				chara2.Talk("seduced");
			}
			else if (chara != EClass.pc)
			{
				Chara chara4 = chara;
				Chara chara5 = chara2;
				if (bitch)
				{
					chara = chara5;
					chara2 = chara4;
				}
				Debug.Log("buyer:" + chara.Name + " seller:" + chara2.Name + " money:" + num3);
				if (!chara.IsPC)
				{
					chara.ModCurrency(EClass.rndHalf(num3));
				}
				if (!chara2.IsPC && chara.GetCurrency() < num3 && EClass.rnd(2) == 0)
				{
					num3 = chara.GetCurrency();
				}
				Debug.Log("money:" + num3 + " buyer:" + chara.GetCurrency());
				if (chara.GetCurrency() >= num3)
				{
					chara.Talk("tail_pay");
				}
				else
				{
					chara.Talk("tail_nomoney");
					num3 = chara.GetCurrency();
					chara2.Say("angry", chara2);
					chara2.Talk("angry");
					flag = (sell ? true : false);
					if (EClass.rnd(chara.IsPC ? 2 : 20) == 0)
					{
						flag3 = true;
					}
				}
				chara.ModCurrency(-num3);
				if (chara2 == EClass.pc)
				{
					if (num3 > 0)
					{
						EClass.player.DropReward(ThingGen.Create("money").SetNum(num3));
						EClass.player.ModKarma(-1);
					}
				}
				else
				{
					int num4 = (chara2.CHA * 10 + 100) / ((chara2.IsPCFaction && chara2.memberType == FactionMemberType.Default) ? 1 : 10);
					if (chara2.GetCurrency() - num4 > 0)
					{
						chara2.c_allowance += num3;
					}
					else
					{
						chara2.ModCurrency(num3);
					}
				}
				chara = chara4;
				chara2 = chara5;
			}
			if (flag3)
			{
				chara2.DoHostileAction(chara);
			}
			if (chara.IsPCParty || chara2.IsPCParty)
			{
				chara.stamina.Mod(-5 - EClass.rnd(chara.stamina.max / 10 + (succubus ? StaminaCost(chara2, chara) : 0) + 1));
				chara2.stamina.Mod(-5 - EClass.rnd(chara2.stamina.max / 20 + (succubus ? StaminaCost(chara, chara2) : 0) + 1));
			}
			SuccubusExp(chara, chara2);
			SuccubusExp(chara2, chara);
			chara2.ModAffinity(chara, flag ? 10 : (-5));
			break;
		}
		case FuckType.tame:
		{
			int num = ((!chara2.IsPCFaction) ? (chara2.IsHuman ? 10 : 5) : (chara2.IsHuman ? 5 : 0));
			Msg.Say("tame_end", target);
			target.PlaySound("groomed");
			target.PlayEffect("heal_tick");
			target.hygiene.Mod(15);
			if (target == owner)
			{
				break;
			}
			if (totalAffinity > 0)
			{
				chara.Say("brush_success", target, owner);
			}
			else
			{
				chara.Say("brush_fail", target, owner);
				num *= 5;
			}
			bool num2 = TraitToolBrush.IsTamePossible(target.Chara);
			bool flag2 = num2 && chara2.affinity.CanInvite() && chara2.GetBestAttribute() < EClass.pc.CHA;
			if (num2)
			{
				if (flag2)
				{
					chara.Say("tame_success", owner, target);
					chara2.MakeAlly();
				}
				else
				{
					chara.Say("tame_fail", chara, chara2);
				}
			}
			if (num > EClass.rnd(100))
			{
				chara2.DoHostileAction(chara);
				chara2.calmCheckTurn *= 3;
			}
			break;
		}
		}
		static int StaminaCost(Chara c1, Chara c2)
		{
			return (int)Mathf.Max(10f * (float)c1.END / (float)Mathf.Max(c2.END, 1), 0f);
		}
		static void SuccubusExp(Chara c, Chara tg)
		{
			if (!c.HasElement(1216))
			{
				return;
			}
			foreach (Element item in tg.elements.ListBestAttributes())
			{
				if (c.elements.ValueWithoutLink(item.id) < item.ValueWithoutLink)
				{
					c.elements.ModTempPotential(item.id, 1 + EClass.rnd(item.ValueWithoutLink - c.elements.ValueWithoutLink(item.id) / 5 + 1));
					c.Say("succubus_exp", c, item.Name.ToLower());
					break;
				}
			}
		}
	}
}
