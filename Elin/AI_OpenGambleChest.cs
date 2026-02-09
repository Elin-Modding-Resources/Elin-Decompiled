using System.Collections.Generic;
using UnityEngine;

public class AI_OpenGambleChest : AIAct
{
	public Thing target;

	public override CursorInfo CursorIcon => CursorSystem.Container;

	public bool IsValid()
	{
		if (owner == null || owner.isDead || target.isDestroyed)
		{
			return false;
		}
		if (!target.IsChildOf(owner))
		{
			return owner.Dist(target) <= 1;
		}
		return true;
	}

	public override IEnumerable<Status> Run()
	{
		owner.Say("lockpick_start", owner, target);
		owner.PlaySound("lock_pick");
		while (target.Num > 0 && IsValid())
		{
			owner.PlaySound("lock_open_small");
			owner.LookAt(target);
			target.renderer.PlayAnime(AnimeID.Shiver);
			yield return KeepRunning();
			EClass.player.stats.gambleChest++;
			Rand.SetSeed(EClass.game.seed + EClass.player.stats.gambleChest);
			bool flag = owner.Evalue(280) + 5 >= EClass.rnd(target.c_lockLv + 10);
			if (EClass.rnd(20) == 0)
			{
				flag = true;
			}
			if (EClass.rnd(20) == 0)
			{
				flag = false;
			}
			int num = 20 + target.c_lockLv / 3;
			if (flag)
			{
				num *= 3;
				EClass.player.stats.gambleChestOpen++;
				Rand.SetSeed(EClass.game.seed + EClass.player.stats.gambleChestOpen);
				bool flag2 = 100 + owner.LUC > EClass.rnd(10000);
				if (EClass.debug.enable && EClass.rnd(2) == 0)
				{
					flag2 = true;
				}
				if (flag2)
				{
					owner.PlaySound("money");
					owner.PlayAnime(AnimeID.Jump);
					Thing thing = ThingGen.Create("money").SetNum(EClass.rndHalf(50 * (100 + Mathf.Min(target.c_lockLv, 10000000) * 10)));
					owner.Pick(thing, msg: false);
					owner.Say("gambleChest_win", thing);
				}
				else
				{
					owner.Say("gambleChest_loss");
				}
				Rand.SetSeed();
			}
			else
			{
				owner.Say("gambleChest_broke", target.GetName(NameStyle.Full, 1));
				owner.PlaySound("rock_dead");
			}
			target.ModNum(-1);
			owner.ModExp(280, num);
			if (EClass.rnd(2) == 0)
			{
				owner.stamina.Mod(-1);
			}
		}
	}

	public static Thing MakeReward()
	{
		int num = 1;
		string str = "";
		if (str.IsEmpty())
		{
			str = "money";
		}
		return ThingGen.Create(str).SetNum(num);
	}
}
