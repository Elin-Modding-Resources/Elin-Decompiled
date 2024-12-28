using System.Collections.Generic;
using UnityEngine;

public class AI_Fish : AIAct
{
	public class ProgressFish : AIProgress
	{
		public int hit = -1;

		public Point posWater;

		public override bool ShowProgress => false;

		public override int MaxProgress => 100;

		public override int LeftHand => -1;

		public override int RightHand => 1107;

		public override void OnStart()
		{
			if (!shouldCancel)
			{
				owner.PlaySound("fish_cast");
				if (owner.Tool != null)
				{
					owner.Say("fish_start", owner, owner.Tool);
				}
				else
				{
					owner.Say("fish_start2", owner);
				}
			}
		}

		public override void OnProgress()
		{
			if (owner.IsPC && (owner.Tool == null || !owner.Tool.HasElement(245)))
			{
				Cancel();
			}
			else if (hit >= 0)
			{
				owner.renderer.PlayAnime(AnimeID.Fishing);
				owner.PlaySound("fish_fight");
				Ripple();
				int a = Mathf.Clamp(10 - EClass.rnd(owner.Evalue(245) + 1) / 10, 5, 10);
				if (hit > EClass.rnd(a))
				{
					hit = 100;
					progress = MaxProgress;
				}
				hit++;
			}
			else
			{
				if (EClass.rnd(Mathf.Clamp(10 - EClass.rnd(owner.Evalue(245) + 1) / 5, 2, 10)) == 0 && progress >= 10)
				{
					hit = 0;
				}
				if (progress == 2 || (progress >= 8 && progress % 6 == 0 && EClass.rnd(3) == 0))
				{
					owner.renderer.PlayAnime(AnimeID.Shiver);
					Ripple();
				}
			}
		}

		public void Ripple()
		{
			if (posWater != null)
			{
				Effect.Get("ripple").Play(posWater, -0.04f);
				posWater.PlaySound("fish_splash");
			}
		}

		public override void OnProgressComplete()
		{
			owner.renderer.PlayAnime(AnimeID.Fishing);
			if (hit < 100)
			{
				Fail();
				return;
			}
			if (owner.IsPC && !EClass.debug.enable)
			{
				if (EClass.player.eqBait == null || EClass.player.eqBait.isDestroyed)
				{
					Msg.Say("noBait");
					return;
				}
				EClass.player.eqBait.ModNum(-1);
			}
			Thing thing = Makefish(owner);
			if (thing == null)
			{
				Fail();
				return;
			}
			int num = thing.Num;
			EClass._zone.AddCard(thing, owner.pos);
			thing.renderer.PlayAnime(AnimeID.Jump);
			owner.Say("fish_get", owner, thing);
			owner.PlaySound("fish_get");
			owner.elements.ModExp(245, 100);
			if (thing.id == "medal")
			{
				thing.isHidden = false;
			}
			if (owner.IsPC)
			{
				if (EClass.game.config.preference.pickFish)
				{
					if (StatsBurden.GetPhase((EClass.pc.ChildrenWeight + thing.ChildrenAndSelfWeight) * 100 / EClass.pc.WeightLimit) >= 3)
					{
						EClass.pc.Say("tooHeavy", thing);
						shouldCancel = true;
					}
					else
					{
						owner.Pick(thing);
					}
				}
			}
			else
			{
				foreach (Thing item in owner.things.List((Thing t) => t.source._origin == "fish"))
				{
					item.Destroy();
				}
				if (owner.things.IsFull())
				{
					thing.Destroy();
				}
			}
			if (EClass.rnd(2) == 0 || num > 1)
			{
				owner.stamina.Mod(-1 * num);
			}
		}

		public void Fail()
		{
			if (owner.IsPC)
			{
				owner.Say("fish_miss", owner);
			}
			owner.stamina.Mod(-1);
		}
	}

	public Point pos;

	public static bool shouldCancel;

	public override int MaxRestart => 9999;

	public override TargetType TargetType => TargetType.Ground;

	public override bool CanManualCancel()
	{
		return true;
	}

	public override bool CanPerform()
	{
		return Act.TP.cell.IsTopWaterAndNoSnow;
	}

	public override AIProgress CreateProgress()
	{
		return new ProgressFish
		{
			posWater = pos
		};
	}

	public override IEnumerable<Status> Run()
	{
		if (!owner.IsPC)
		{
			owner.TryPickGroundItem();
		}
		if (pos != null)
		{
			if (!pos.cell.IsTopWaterAndNoSnow)
			{
				yield return Cancel();
			}
			yield return DoGoto(pos, 1);
			owner.LookAt(pos);
			if (owner.IsPC)
			{
				EClass.player.TryEquipBait();
				if (EClass.player.eqBait == null)
				{
					Msg.Say("noBait");
					yield return Cancel();
				}
			}
			if (!pos.cell.IsTopWaterAndNoSnow || owner.Dist(pos) > 1)
			{
				yield return Cancel();
			}
			Status status = DoProgress();
			if (shouldCancel)
			{
				shouldCancel = false;
				yield return Cancel();
			}
			if (status == Status.Running)
			{
				yield return Status.Running;
			}
			if (owner == EClass.pc)
			{
				yield return Restart();
			}
			yield return DoWait(2);
			if (owner != null)
			{
				if (!owner.IsPC)
				{
					owner.TryPickGroundItem();
				}
				if (owner.IsPCFaction && !owner.IsPCParty)
				{
					owner.ClearInventory(ClearInventoryType.Purge);
				}
			}
			yield return Success();
		}
		if (owner.fov == null)
		{
			yield return Cancel();
		}
		List<Point> list = owner.fov.ListPoints();
		foreach (Point p in list)
		{
			if (p.cell.IsTopWaterAndNoSnow)
			{
				continue;
			}
			for (int _x = p.x - 1; _x <= p.x + 1; _x++)
			{
				for (int _z = p.z - 1; _z <= p.z + 1; _z++)
				{
					Point.shared.Set(_x, _z);
					if (Point.shared.IsValid && Point.shared.cell.IsTopWaterAndNoSnow)
					{
						Point dest = Point.shared.Copy();
						yield return DoGoto(dest);
						owner.LookAt(dest);
						yield return DoProgress();
						yield return Status.Success;
					}
				}
			}
		}
	}

	public static Point GetFishingPoint(Point p)
	{
		Point point = new Point();
		if (p.cell.IsTopWaterAndNoSnow)
		{
			return Point.Invalid;
		}
		for (int i = p.x - 1; i <= p.x + 1; i++)
		{
			for (int j = p.z - 1; j <= p.z + 1; j++)
			{
				point.Set(i, j);
				if (point.IsValid && point.cell.IsTopWaterAndNoSnow)
				{
					return point;
				}
			}
		}
		return Point.Invalid;
	}

	public static Thing Makefish(Chara c)
	{
		int num = c.Evalue(245);
		if (EClass.rnd(3 + num) == 0)
		{
			return null;
		}
		int[] array = new int[15]
		{
			233, 235, 236, 236, 236, 1170, 1143, 1144, 727, 728,
			237, 869, 1178, 1179, 1180
		};
		Thing thing = null;
		int num2 = 1;
		string text = "";
		if (c.IsPC || EClass.rnd(20) == 0)
		{
			if (EClass.rnd(30) == 0)
			{
				text = "book_ancient";
			}
			if (EClass.rnd(35) == 0 || EClass.debug.enable)
			{
				text = "plat";
				if (EClass.rnd(2) == 0)
				{
					text = "scratchcard";
				}
				if (EClass.rnd(3) == 0)
				{
					text = "casino_coin";
				}
				if (EClass.rnd(3) == 0)
				{
					text = "gacha_coin";
				}
				if (EClass.rnd(50) == 0 || EClass.debug.enable)
				{
					text = new string[7] { "659", "758", "759", "806", "828", "1190", "1191" }.RandomItem();
				}
			}
			if (EClass.rnd(40) == 0 && EClass.rnd(40) < num / 3 + 10)
			{
				text = "medal";
			}
		}
		if (text != "")
		{
			thing = ThingGen.Create(text, -1, EClass._zone.ContentLv);
		}
		else if (EClass.rnd(5 + num / 3) == 0)
		{
			thing = ThingGen.Create(array.RandomItem().ToString() ?? "");
		}
		else
		{
			int num3 = EClass.rnd(num * 2) + 1;
			thing = ThingGen.Create("fish", -1, num3);
			num2 = EClass.rnd(num / (num3 + 10)) + 1;
			int num4 = 5;
			if (EClass.Branch != null)
			{
				num4 += EClass.Branch.Evalue(3604) * 20 + EClass.Branch.Evalue(3605) * 20 + EClass.Branch.Evalue(3706) * 25;
			}
			if (num4 >= EClass.rnd(100))
			{
				num2++;
			}
		}
		if (thing != null)
		{
			thing.SetNum(num2);
			thing.SetBlessedState(BlessedState.Normal);
		}
		return thing;
	}
}
