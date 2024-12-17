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

	public int maxProgress;

	public int progress;

	public virtual FuckType Type => FuckType.fuck;

	public override bool PushChara => false;

	public override bool IsAutoTurn => true;

	public override TargetType TargetType => TargetType.Chara;

	public override int MaxProgress => maxProgress;

	public override int CurrentProgress => progress;

	public virtual bool CanTame()
	{
		return false;
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
		cc.Say(Type.ToString() + "_start", cc, tc);
		isFail = () => !tc.IsAliveInCurrentZone || tc.Dist(owner) > 3;
		if (Type == FuckType.tame)
		{
			cc.SetTempHand(1104, -1);
		}
		int destDist = ((Type == FuckType.fuck) ? 1 : 1);
		maxProgress = ((!EClass.debug.enable) ? 25 : 0);
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
				if (EClass.rnd(8) == 0)
				{
					tc.AddCondition<ConFear>(50);
				}
				if (i == 0 || i == 10)
				{
					cc.Talk("goodBoy");
				}
				cc.elements.ModExp(237, 10);
				break;
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
				chara3.SAN.Mod(10);
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
			int num = CalcMoney.Whore(chara2, chara);
			chara.Talk("tail_after");
			bool flag2 = false;
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
				Debug.Log("buyer:" + chara.Name + " seller:" + chara2.Name + " money:" + num);
				if (!chara.IsPC)
				{
					chara.ModCurrency(EClass.rndHalf(num));
				}
				if (!chara2.IsPC && chara.GetCurrency() < num && EClass.rnd(2) == 0)
				{
					num = chara.GetCurrency();
				}
				Debug.Log("money:" + num + " buyer:" + chara.GetCurrency());
				if (chara.GetCurrency() >= num)
				{
					chara.Talk("tail_pay");
				}
				else
				{
					chara.Talk("tail_nomoney");
					num = chara.GetCurrency();
					chara2.Say("angry", chara2);
					chara2.Talk("angry");
					flag = (sell ? true : false);
					if (EClass.rnd(chara.IsPC ? 2 : 20) == 0)
					{
						flag2 = true;
					}
				}
				chara.ModCurrency(-num);
				if (chara2 == EClass.pc)
				{
					if (num > 0)
					{
						EClass.player.DropReward(ThingGen.Create("money").SetNum(num));
						EClass.player.ModKarma(-1);
					}
				}
				else
				{
					int num2 = (chara2.CHA * 10 + 100) / ((chara2.IsPCFaction && chara2.memberType == FactionMemberType.Default) ? 1 : 10);
					if (chara2.GetCurrency() - num2 > 0)
					{
						chara2.c_allowance += num;
					}
					else
					{
						chara2.ModCurrency(num);
					}
				}
				chara = chara4;
				chara2 = chara5;
			}
			if (flag2)
			{
				chara2.DoHostileAction(chara);
			}
			if (chara.IsPCParty || chara2.IsPCParty)
			{
				chara.stamina.Mod(-5 - EClass.rnd(chara.stamina.max / 10 + (succubus ? chara2.LV : 0) + 1));
				chara2.stamina.Mod(-5 - EClass.rnd(chara2.stamina.max / 20 + (succubus ? chara.LV : 0) + 1));
			}
			SuccubusExp(chara, chara2);
			SuccubusExp(chara2, chara);
			break;
		}
		case FuckType.tame:
			if (CanTame())
			{
				if (flag)
				{
					chara.Say("tame_success", owner, target);
					chara2.MakeAlly();
					chara.elements.ModExp(237, 200);
				}
				else
				{
					chara.Say("tame_fail", chara, chara2);
				}
			}
			else
			{
				chara.Say("tame_invalid", chara2);
			}
			break;
		}
		chara2.ModAffinity(chara, flag ? 10 : (-5));
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
