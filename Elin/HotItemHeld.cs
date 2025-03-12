using UnityEngine;

public class HotItemHeld : HotItemThing
{
	public static Thing lastHeld;

	public static TaskBuild taskBuild;

	public static Recipe recipe;

	public static bool disableTool;

	public override Act act => GetAct();

	public static bool CanChangeHeightByWheel()
	{
		if (EClass.core.config.input.altChangeHeight && !EClass.scene.actionMode.IsBuildMode && !EInput.isShiftDown)
		{
			return false;
		}
		if (!EClass._zone.IsRegion && EClass.pc.held != null && EClass.pc.held.trait.CanChangeHeight && taskBuild != null && recipe.MaxAltitude != 0)
		{
			return taskBuild.CanPerform();
		}
		return false;
	}

	public static bool CanRotate()
	{
		if (EClass._zone.IsRegion || EClass.pc.held == null)
		{
			return false;
		}
		if (!(EClass._zone is Zone_Tent) && !EClass._zone.IsPCFactionOrTent && EClass.pc.held.trait.CanBeOnlyBuiltInHome)
		{
			return false;
		}
		if (EClass._zone.RestrictBuild && !EClass.pc.held.trait.CanBuildInTown)
		{
			return false;
		}
		if (taskBuild == null)
		{
			return false;
		}
		if (EClass.pc.held.trait is TraitTile)
		{
			return true;
		}
		if (!taskBuild.CanPerform())
		{
			return false;
		}
		ActionMode.Adv.planAll.Update(EClass.scene.mouseTarget);
		return !ActionMode.Adv.planAll.HasAct;
	}

	public HotItemHeld()
	{
	}

	public HotItemHeld(Thing t)
	{
		thing = t;
	}

	public override void OnSetCurrentItem()
	{
		ActionMode.Build.altitude = 0;
		disableTool = false;
		if (EClass.pc.held != thing)
		{
			EClass.pc.HoldCard(thing);
		}
		taskBuild = null;
		RecipeManager.BuildList();
		recipe = thing.trait.GetRecipe();
		if (thing.trait is TraitCatalyst)
		{
			_act = (thing.trait as TraitCatalyst).CreateAct();
		}
		if (lastHeld != thing)
		{
			thing.trait.OnHeld();
		}
		lastHeld = thing;
		if (thing.trait is TraitToolRange)
		{
			EClass.pc.ranged = thing;
		}
	}

	public override void OnUnselect()
	{
		taskBuild = null;
		recipe = null;
		if (EClass.pc.held == thing)
		{
			EClass.pc.PickHeld();
		}
	}

	public Act GetAct()
	{
		if (thing.trait is TraitRod)
		{
			return new ActZap();
		}
		if (thing.trait is TraitToolRange)
		{
			return ACT.Ranged;
		}
		if (thing.HasElement(241))
		{
			return new AI_PlayMusic();
		}
		if (thing.HasElement(225))
		{
			return new TaskCut();
		}
		if (thing.HasElement(220))
		{
			return new TaskMine();
		}
		if (thing.HasElement(230))
		{
			return new TaskDig
			{
				mode = TaskDig.Mode.RemoveFloor
			};
		}
		if (thing.HasElement(286))
		{
			return new TaskPlow();
		}
		if (thing.HasElement(245))
		{
			return new AI_Fish();
		}
		if (thing.HasElement(237))
		{
			return new AI_TendAnimal();
		}
		return _act;
	}

	public Act GetSelfAct()
	{
		if (lost)
		{
			return null;
		}
		if (thing.trait.CanDrink(EClass.pc))
		{
			return new AI_Drink
			{
				target = thing
			};
		}
		if (thing.trait.CanEat(EClass.pc))
		{
			return new AI_Eat
			{
				cook = false
			};
		}
		if (thing.trait.CanRead(EClass.pc))
		{
			return new AI_Read
			{
				target = thing
			};
		}
		return null;
	}

	public override bool TrySetAct(ActPlan p)
	{
		taskBuild = null;
		if (!disableTool)
		{
			if (p.IsSelf && thing.trait.CanUse(EClass.pc))
			{
				return p.TrySetAct(thing.trait.LangUse, () => thing.trait.OnUse(p.cc), thing, null, -1);
			}
			if (EClass.scene.mouseTarget.target is Card && thing.trait.CanUse(EClass.pc, EClass.scene.mouseTarget.target as Card))
			{
				return p.TrySetAct(thing.trait.LangUse, () => thing.trait.OnUse(p.cc, EClass.scene.mouseTarget.target as Card), thing, null, -1);
			}
			if (thing.trait.CanUse(EClass.pc, p.pos))
			{
				return p.TrySetAct(thing.trait.LangUse, () => thing.trait.OnUse(p.cc, p.pos), thing, null, -1);
			}
			if (thing.trait.IsTool)
			{
				if (TrySetToolAct(p))
				{
					return true;
				}
			}
			else
			{
				thing.trait.TrySetHeldAct(p);
			}
			if (p.HasAct || thing.trait.IsTool)
			{
				return true;
			}
		}
		if (p.pos.Equals(EClass.pc.pos))
		{
			Act selfAct = GetSelfAct();
			if (selfAct != null)
			{
				p.TrySetAct(selfAct, thing);
				return true;
			}
		}
		bool flag = true;
		if (thing.trait is TraitThrown && !disableTool)
		{
			flag = false;
		}
		if (thing.trait is TraitTent && EClass._zone is Zone_Tent)
		{
			flag = false;
		}
		Chara tg;
		if (flag)
		{
			if (p.IsSelfOrNeighbor)
			{
				_ = p.pos.Installed?.trait;
				tg = p.pos.FirstVisibleChara();
				if (tg != null && tg != EClass.pc && !tg.IsDisabled && tg.IsNeutralOrAbove() && EClass.pc.held != null && tg.CanAcceptGift(EClass.pc, EClass.pc.held))
				{
					string lang = "actGive";
					if (tg.Evalue(1232) > 0 && EClass.pc.held.trait is TraitDrinkMilkMother)
					{
						lang = "actMilk";
					}
					p.TrySetAct(lang, delegate
					{
						if (!tg.IsValidGiftWeight(EClass.pc.held, 1))
						{
							tg.Talk("tooHeavy");
							return true;
						}
						if (EClass.core.config.game.confirmGive)
						{
							Dialog.YesNo("dialogGive".lang(EClass.pc.held.GetName(NameStyle.Full, 1)), func);
						}
						else
						{
							func();
						}
						return true;
					}, thing);
				}
			}
			if (p.HasAct)
			{
				return true;
			}
			if (!thing.c_isImportant && (p.IsSelfOrNeighbor || thing.trait.CanExtendBuild))
			{
				Chara chara = p.pos.FirstVisibleChara();
				if ((chara == null || chara == EClass.pc || chara.IsNeutralOrAbove()) && recipe != null && (!thing.trait.IsThrowMainAction || EInput.isShiftDown || disableTool))
				{
					taskBuild = new TaskBuild
					{
						recipe = recipe,
						held = EClass.pc.held,
						pos = p.pos.Copy()
					};
					AM_Build build = ActionMode.Build;
					build.bridgeHeight = -1;
					build.recipe = taskBuild.recipe;
					build.mold = taskBuild;
					build.SetAltitude(recipe.tileType.AltitudeAsDir ? recipe._dir : build.altitude);
					p.TrySetAct(taskBuild);
				}
			}
		}
		if (p.HasAct)
		{
			return true;
		}
		if (ActThrow.CanThrow(EClass.pc, thing, null, p.pos) && p.TrySetAct(new ActThrow
		{
			target = thing,
			pcTarget = p.pos.FindAttackTarget()?.Chara
		}, thing))
		{
			return true;
		}
		return false;
		void func()
		{
			EClass.pc.GiveGift(tg, EClass.pc.SplitHeld(1) as Thing);
		}
	}

	public bool TrySetToolAct(ActPlan p)
	{
		thing.trait.TrySetHeldAct(p);
		if (p.HasAct)
		{
			return true;
		}
		Cell cell = p.pos.cell;
		Point pos = p.pos;
		if (cell.HasBlock && (!cell.HasObj || cell.sourceObj.tileType.IsMountBlock) && TaskMine.CanMine(pos, thing) && p.TrySetAct(new TaskMine
		{
			pos = pos.Copy()
		}))
		{
			return true;
		}
		if (pos.IsHidden)
		{
			return true;
		}
		if (thing.trait is TraitToolRange)
		{
			if (!EClass.pc.CanSeeSimple(pos))
			{
				return true;
			}
			Card tc = null;
			foreach (Chara item in pos.ListVisibleCharas())
			{
				if (item.isSynced && item.IsAliveInCurrentZone)
				{
					tc = item;
					break;
				}
			}
			EClass.pc.ranged = thing;
			p.TrySetAct(ACT.Ranged, tc);
			return true;
		}
		if (cell.IsTopWaterAndNoSnow && thing.HasElement(245) && p.TrySetAct(new AI_Fish
		{
			pos = pos.Copy()
		}))
		{
			return true;
		}
		if (p.IsNeighborBlocked)
		{
			return true;
		}
		TaskHarvest taskHarvest = TaskHarvest.TryGetAct(EClass.pc, pos);
		if (taskHarvest != null && p.TrySetAct(taskHarvest))
		{
			return true;
		}
		TraitToolWaterPot traitToolWaterPot = thing.trait as TraitToolWaterPot;
		if (cell.IsTopWaterAndNoSnow)
		{
			if (traitToolWaterPot != null && traitToolWaterPot.owner.c_charges < traitToolWaterPot.MaxCharge && p.TrySetAct(new TaskDrawWater
			{
				pot = traitToolWaterPot,
				pos = pos.Copy()
			}))
			{
				return true;
			}
		}
		else
		{
			if (traitToolWaterPot != null && !p.pos.HasBridge && traitToolWaterPot.owner.c_charges > 0 && pos.cell.sourceSurface.tag.Contains("soil") && p.TrySetAct(new TaskPourWater
			{
				pot = traitToolWaterPot,
				pos = pos.Copy()
			}))
			{
				return true;
			}
			if (thing.HasElement(230) && p.TrySetAct(new TaskDig
			{
				pos = pos.Copy(),
				mode = TaskDig.Mode.RemoveFloor
			}))
			{
				return true;
			}
			if (thing.HasElement(286) && p.TrySetAct(new TaskPlow
			{
				pos = pos.Copy()
			}))
			{
				return true;
			}
		}
		if (TaskMine.CanMine(pos, thing) && p.TrySetAct(new TaskMine
		{
			pos = pos.Copy()
		}))
		{
			return true;
		}
		if (thing.HasElement(225) && p.TrySetAct(new TaskChopWood
		{
			pos = pos.Copy()
		}))
		{
			return true;
		}
		if (pos.HasChara)
		{
			_ = pos.FirstChara;
			_ = EClass.pc;
		}
		if (thing.HasTag(CTAG.throwWeapon) && ActThrow.CanThrow(EClass.pc, thing, null, p.pos) && p.TrySetAct(new ActThrow
		{
			target = thing,
			pcTarget = p.pos.FindAttackTarget()?.Chara
		}, thing))
		{
			return true;
		}
		return false;
	}

	public override void OnRenderTile(Point point, HitResult result, int dir)
	{
		if (EClass.game.config.showGuideGrid)
		{
			for (int i = point.z - 1; i < point.z + 2; i++)
			{
				for (int j = point.x - 1; j < point.x + 2; j++)
				{
					EClass.screen.guide.passGuideFloor.Add(Point.shared.Set(j, i), 1f);
				}
			}
		}
		if (taskBuild == null || !taskBuild.IsRunning || (!recipe.MultiSize && !taskBuild.CanProgress()) || !taskBuild.CanPerform(EClass.pc, null, point))
		{
			return;
		}
		EClass.screen.guide.isActive = false;
		ActionMode.Build.OnRenderTile(point, HitResult.Valid, taskBuild.recipe._dir);
		EClass.screen.guide.isActive = true;
		if (recipe.MultiSize)
		{
			point.ForeachMultiSize(recipe.W, recipe.H, delegate(Point pos, bool main)
			{
				Vector3 v = pos.Position();
				v.z -= 0.01f;
				EClass.screen.guide.passGuideFloor.Add(ref v);
			});
		}
	}
}
