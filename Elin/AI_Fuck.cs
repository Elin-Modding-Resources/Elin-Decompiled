using System.Collections.Generic;
using UnityEngine;

public class AI_Fuck : AIAct
{
	public enum FuckType
	{
		fuck,
		tame
	}

	public enum Variation
	{
		Normal,
		Bitch,
		Succubus,
		NTR,
		Bloodsuck,
		Slime,
		Tentacle,
		MotherMilk
	}

	public Variation variation;

	public Chara target;

	public bool sell;

	public bool succubus;

	public int maxProgress;

	public int progress;

	public int fails;

	public int totalAffinity;

	public virtual FuckType Type => FuckType.fuck;

	public bool IsSacredLovemaking
	{
		get
		{
			if (variation == Variation.Normal && owner != null && target != null && target != owner && (owner == EClass.pc || owner.IsMarried))
			{
				if (target != EClass.pc)
				{
					return target.IsMarried;
				}
				return true;
			}
			return false;
		}
	}

	public override bool PushChara => false;

	public override bool IsAutoTurn => true;

	public override TargetType TargetType => TargetType.Chara;

	public override int MaxProgress => maxProgress;

	public override int CurrentProgress => progress;

	public override bool CancelOnAggro
	{
		get
		{
			if (variation != Variation.NTR && variation != Variation.Bloodsuck)
			{
				return variation != Variation.Slime;
			}
			return false;
		}
	}

	public override bool CancelWhenDamaged => CancelOnAggro;

	public override bool ShouldAllyAttack(Chara tg)
	{
		return tg != target;
	}

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
		if (owner.host != target)
		{
			yield return DoGoto(target.pos, destDist, ignoreConnection: true);
		}
		cc.Say((this.variation == Variation.Slime) ? "slime_start" : ((this.variation == Variation.Bloodsuck) ? "suck_start" : (Type.ToString() + "_start")), cc, tc);
		isFail = () => !tc.IsAliveInCurrentZone || tc.Dist(owner) > 3;
		maxProgress = ((this.variation == Variation.NTR || this.variation == Variation.Bloodsuck) ? 10 : 25);
		if (Type == FuckType.tame)
		{
			cc.SetTempHand(1104, -1);
		}
		Variation variation = this.variation;
		if ((uint)(variation - 3) <= 2u)
		{
			maxProgress = maxProgress * 100 / (100 + owner.Evalue(1664) * 50);
		}
		switch (this.variation)
		{
		case Variation.Succubus:
			cc.Talk("seduce");
			break;
		case Variation.Bloodsuck:
			cc.PlaySound("bloodsuck");
			break;
		case Variation.Slime:
			cc.PlaySound("slime");
			target.AddCondition<ConEntangle>(500, force: true);
			break;
		}
		int i;
		for (i = 0; i < maxProgress; i++)
		{
			progress = i;
			if (owner.host != target)
			{
				yield return DoGoto(target.pos, destDist, ignoreConnection: true);
			}
			switch (Type)
			{
			case FuckType.fuck:
			{
				if (this.variation == Variation.NTR)
				{
					cc.Say("ntr", cc, tc);
				}
				cc.LookAt(tc);
				tc.LookAt(cc);
				switch (i % 4)
				{
				case 0:
					cc.renderer.PlayAnime(AnimeID.Attack, tc);
					if (EClass.rnd(3) == 0 || sell)
					{
						if (cc.IsPC && this.variation == Variation.MotherMilk)
						{
							EClass.player.forceTalk = true;
						}
						cc.Talk((this.variation == Variation.MotherMilk) ? "play_baby" : "tail");
					}
					break;
				case 2:
					tc.renderer.PlayAnime(AnimeID.Shiver);
					if (EClass.rnd(3) == 0)
					{
						if (tc.IsPC && this.variation == Variation.MotherMilk)
						{
							EClass.player.forceTalk = true;
						}
						tc.Talk((this.variation == Variation.MotherMilk) ? "play_mama" : "tailed");
					}
					break;
				}
				int num = ((cc.HasElement(1216) || tc.HasElement(1216)) ? 100 : 20);
				if (this.variation == Variation.MotherMilk)
				{
					num *= 5;
				}
				if (num > EClass.rnd(100))
				{
					((EClass.rnd(2) == 0) ? cc : tc).PlayEffect("love2");
				}
				if (this.variation == Variation.Slime)
				{
					owner.DoHostileAction(target);
				}
				if (EClass.rnd(3) == 0 || sell)
				{
					if (this.variation == Variation.Slime)
					{
						target.AddCondition<ConSupress>(200, force: true);
						if (!owner.IsPC)
						{
							int num2 = EClass.rndHalf(target.MaxHP / (target.IsPowerful ? 100 : 10)) + 1;
							owner.HealHP(num2);
							target.DamageHP(num2, AttackSource.Condition, owner);
						}
					}
					else
					{
						target.AddCondition<ConWait>(50, force: true);
					}
				}
				if (this.variation == Variation.Bloodsuck || this.variation == Variation.Slime)
				{
					owner.pos.TryWitnessCrime(cc, tc, 4, (Chara c) => EClass.rnd(cc.HasCondition<ConTransmuteBat>() ? 50 : 20) == 0);
				}
				break;
			}
			case FuckType.tame:
				if (i == 0 || i == 10)
				{
					cc.Talk("goodBoy");
				}
				if (i % 5 == 0)
				{
					tc.PlaySound("brushing");
				}
				if (cc.Tool != null && cc.Tool.Evalue(770) > 0)
				{
					foreach (Chara item in cc.pos.ListCharasInRadius(cc, 2 + cc.Tool.Evalue(770) / 10, (Chara c) => true))
					{
						if (cc == tc || item != cc)
						{
							Brush(item);
							if (cc.isDead)
							{
								break;
							}
						}
					}
				}
				else
				{
					Brush(tc);
				}
				break;
			}
		}
		Finish();
		void Brush(Chara c)
		{
			if (c.interest >= 0)
			{
				int num3 = 100;
				if (!c.IsAnimal)
				{
					num3 += 50;
				}
				if (c.IsHuman)
				{
					num3 += 50;
				}
				if (c.IsInCombat)
				{
					num3 += 100;
				}
				if (c == cc)
				{
					num3 = 50;
				}
				else if (c.affinity.CurrentStage < Affinity.Stage.Respected && EClass.rnd((c.IsPCFaction ? 30 : 10) * 100 / num3) == 0)
				{
					tc.AddCondition<ConFear>(60);
				}
				c.interest -= (c.IsPCFaction ? 20 : (2 * num3 / 100));
				if (i % 5 == 0)
				{
					int num4 = 0;
					int num5 = cc.CHA / 2 + cc.Evalue(237) - c.CHA * 2;
					if (EClass.rnd(cc.CHA / 2 + cc.Evalue(237)) > EClass.rnd(c.CHA * num3 / 100))
					{
						num4 = 5 + Mathf.Clamp(num5 / 20, 0, 20);
					}
					else
					{
						num4 = -5 + ((!tc.IsPCFaction) ? Mathf.Clamp(num5 / 10, -30, 0) : 0);
						fails++;
					}
					int num6 = 20;
					if (c.IsPCFactionOrMinion && c.affinity.CurrentStage >= Affinity.Stage.Love)
					{
						num4 = ((EClass.rnd(3) == 0) ? 4 : 0);
						num6 = 10;
					}
					totalAffinity += num4;
					c.ModAffinity(EClass.pc, num4, show: true, showOnlyEmo: true);
					cc.elements.ModExp(237, num6);
					if (EClass.rnd(4) == 0)
					{
						cc.stamina.Mod(-1);
					}
				}
				if (c.interest < 0 && c != tc)
				{
					Msg.Say("tame_end", c);
					c.PlaySound("groomed");
					c.PlayEffect("heal_tick");
					c.hygiene.Mod(15);
					if (cc == EClass.pc)
					{
						EClass.player.stats.brush++;
					}
				}
			}
		}
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
			switch (variation)
			{
			case Variation.Bloodsuck:
			case Variation.Slime:
				if (EClass.rnd(2) == 0)
				{
					chara2.AddCondition<ConConfuse>(500);
				}
				if (EClass.rnd(2) == 0)
				{
					chara2.AddCondition<ConDim>(500);
				}
				if (EClass.rnd(2) == 0)
				{
					chara2.AddCondition<ConParalyze>(500);
				}
				if (EClass.rnd(10) == 0)
				{
					chara2.AddCondition<ConInsane>(100 + EClass.rnd(100));
				}
				break;
			default:
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
				break;
			}
			case Variation.MotherMilk:
				break;
			}
			chara.Talk("tail_after");
			bool flag3 = false;
			if (variation == Variation.Succubus)
			{
				chara.ShowEmo(Emo.love);
				chara2.ShowEmo(Emo.love);
				EClass.player.forceTalk = true;
				chara2.Talk("seduced");
			}
			else if (variation != Variation.NTR && variation != Variation.Bloodsuck && variation != Variation.Slime && variation != Variation.MotherMilk && chara != EClass.pc)
			{
				if (IsSacredLovemaking)
				{
					flag = true;
				}
				else
				{
					int num3 = CalcMoney.Whore(chara2, chara);
					Chara chara4 = chara;
					Chara chara5 = chara2;
					if (variation == Variation.Bitch)
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
			}
			if (flag3)
			{
				chara2.DoHostileAction(chara);
			}
			if (variation == Variation.Bloodsuck)
			{
				int value = chara.hunger.value;
				Thing food = CraftUtil.MakeBloodMeal(chara, chara2);
				FoodEffect.Proc(chara, food, consume: false);
				chara2.AddCondition<ConBleed>(EClass.rndHalf(value * 10));
			}
			else
			{
				if (chara.IsPCParty || chara2.IsPCParty)
				{
					chara.stamina.Mod(-5 - EClass.rnd(chara.stamina.max / 10 + ((variation == Variation.Succubus || variation == Variation.MotherMilk) ? StaminaCost(chara2, chara) : 0) + 1));
					chara2.stamina.Mod(-5 - EClass.rnd(chara2.stamina.max / 20 + ((variation == Variation.Succubus || variation == Variation.MotherMilk) ? StaminaCost(chara, chara2) : 0) + 1));
				}
				SuccubusExp(chara, chara2);
				SuccubusExp(chara2, chara);
			}
			chara2.ModAffinity(chara, (flag || (chara.IsPC && chara2.affinity.CanSleepBeside() && EClass.rnd(10) != 0)) ? 10 : (-5));
			if (chara == EClass.pc || chara2 == EClass.pc)
			{
				if (variation == Variation.MotherMilk)
				{
					EClass.player.stats.mama++;
					if (EClass.player.stats.mama >= 20 && !EClass.pc.HasElement(1291) && EClass.pc.faith != EClass.game.religions.MoonShadow)
					{
						EClass.pc.SetFeat(1291, 1, msg: true);
					}
				}
				else
				{
					EClass.player.stats.kimo++;
				}
			}
			switch (variation)
			{
			case Variation.NTR:
			{
				Thing thing2 = chara2.things.Find<TraitDreamBug>();
				if (thing2 != null)
				{
					thing2.ModNum(-1);
					if (chara.IsPC)
					{
						Msg.Say("dream_spell", EClass.sources.elements.map[9156].GetName().ToLowerInvariant());
						EClass.pc.GainAbility(9156, EClass.rnd(2) + 1);
					}
				}
				if (!chara.HasElement(1239) || chara2.HasElement(1216))
				{
					break;
				}
				if (chara2.HasElement(758))
				{
					if (chara.ExistsOnMap)
					{
						chara.stamina.Mod(-1000000);
					}
				}
				else if (chara2.ExistsOnMap)
				{
					chara2.stamina.Mod((!chara2.IsPCFaction) ? (-10000) : (chara2.IsPC ? (-25) : (-50)));
				}
				break;
			}
			case Variation.Bloodsuck:
				if (chara2.HasElement(758) && chara.ExistsOnMap)
				{
					chara.stamina.Mod(-1000000);
				}
				break;
			case Variation.Slime:
			{
				Thing thing = null;
				for (int j = 0; j < 10; j++)
				{
					for (int k = 0; k < 20; k++)
					{
						thing = target.MakeGene((j < 3) ? DNA.Type.Superior : DNA.Type.Default);
						if (EClass.rnd(10) < chara.body.slots.Count - 2)
						{
							break;
						}
						if (thing.c_DNA.GetBodySlot() != -1)
						{
							if (chara.body.GetSlot(35, onlyEmpty: false) == null && EClass.rnd(2) == 0)
							{
								thing.c_DNA.ReplaceBodySlot(35);
							}
							else if (chara.body.GetSlot(32, onlyEmpty: false) == null && EClass.rnd(2) == 0)
							{
								thing.c_DNA.ReplaceBodySlot(32);
							}
							break;
						}
					}
					thing.c_DNA.MakeSlimeFood(chara);
					if (thing.c_DNA.GetInvalidAction(chara) != null || thing.c_DNA.GetInvalidFeat(chara) != null)
					{
						thing.c_DNA.vals.Clear();
						thing.c_DNA.type = DNA.Type.Inferior;
						continue;
					}
					thing.MakeFoodFrom(target);
					thing.elements.ModBase(10, 20);
					thing.elements.ModBase(18, 100);
					break;
				}
				FoodEffect.Proc(chara, thing, consume: false);
				chara.elements.ModExp(6608, 1000f);
				break;
			}
			}
			if (IsSacredLovemaking)
			{
				chara.Say("tender_hug", chara, chara2);
			}
			break;
		}
		case FuckType.tame:
		{
			int num = ((!chara2.IsPCFaction) ? (chara2.IsHuman ? 10 : 5) : (chara2.IsHuman ? 5 : 0));
			Msg.Say("tame_end", target);
			target.PlaySound("groomed");
			target.PlayEffect("heal_tick");
			target.hygiene.Mod(15);
			if (chara == EClass.pc)
			{
				EClass.player.stats.brush++;
			}
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
			if (fails > 0 && num > EClass.rnd(100))
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
		void SuccubusExp(Chara c, Chara tg)
		{
			if (!c.HasElement(1216) || c.isDead)
			{
				return;
			}
			foreach (Element item in tg.elements.ListBestAttributes())
			{
				if (c.elements.ValueWithoutLink(item.id) < item.ValueWithoutLink)
				{
					c.elements.ModTempPotential(item.id, Mathf.Min(EClass.rnd(item.ValueWithoutLink - c.elements.ValueWithoutLink(item.id) / 5), (variation == Variation.MotherMilk) ? 100 : 500));
					c.Say("succubus_exp", c, item.Name.ToLower());
					break;
				}
			}
		}
	}
}
