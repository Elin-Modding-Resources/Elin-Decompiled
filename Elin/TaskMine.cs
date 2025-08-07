public class TaskMine : BaseTaskHarvest
{
	public enum Mode
	{
		Default,
		Ramp
	}

	public Mode mode;

	public int ramp = 3;

	public bool mined;

	public override HarvestType harvestType => HarvestType.Block;

	public override int destDist => 1;

	public override bool isBlock => true;

	public override int RightHand => 1004;

	public override bool destIgnoreConnection => true;

	public override bool ShowMapHighlightBlock => true;

	public override CursorInfo CursorIcon => CursorSystem.Mine;

	public override bool IsHostileAct => true;

	public static bool CanMine(Point pos, Card t)
	{
		if (t != null && pos.HasBlock && (!pos.HasObj || !pos.sourceObj.tileType.IsBlockMount))
		{
			if (t.HasElement(220))
			{
				return true;
			}
			if ((pos.matBlock.category == "wood" || pos.matBlock.category == "grass") && t.HasElement(225))
			{
				return true;
			}
		}
		return false;
	}

	public override string GetTextSmall(Card c)
	{
		if (!pos.cell.HasBlock)
		{
			return pos.cell.GetFloorName();
		}
		return pos.cell.GetBlockName();
	}

	public override void OnCreateProgress(Progress_Custom p)
	{
		SetTarget(owner);
		p.textHint = pos.cell.GetBlockName();
		p.maxProgress = maxProgress;
		p.onProgressBegin = delegate
		{
			if (!CanMine(pos, owner.Tool))
			{
				p.Cancel();
			}
			else if (base.IsTooHard)
			{
				owner.Say("tooHardToMine", owner, pos.cell.GetBlockName());
				p.Cancel();
			}
			else if (owner.Tool != null)
			{
				owner.Say("mine_start", owner, owner.Tool);
			}
		};
		p.onProgress = delegate
		{
			owner.LookAt(pos);
			owner.PlaySound(pos.matBlock.GetSoundImpact());
			pos.Animate(AnimeID.HitObj, animeBlock: true);
			pos.matBlock.PlayHitEffect(pos);
			pos.matBlock.AddBlood(pos);
			owner.renderer.NextFrame();
			owner.elements.ModExp(220, 5f);
			if (EClass._zone.IsCrime(owner, this))
			{
				owner.pos.TryWitnessCrime(owner);
			}
		};
	}

	public override void DrawMarker(int x, int z, RenderParam p)
	{
		if (ActionMode.Mine.IsRoofEditMode() && pos.HasWallOrFence)
		{
			EClass.screen.guide.DrawWall(pos, Working ? EClass.Colors.blockColors.ActiveOpacity : EClass.Colors.blockColors.InactiveOpacity);
		}
		else
		{
			base.DrawMarker(x, z, p);
		}
	}

	public override HitResult GetHitResult()
	{
		if (ActionMode.Mine.IsRoofEditMode())
		{
			if (pos.cell._roofBlock == 0)
			{
				return HitResult.Default;
			}
			return HitResult.Valid;
		}
		if (pos.cell.HasBlock || !pos.cell.isSeen)
		{
			if (pos.sourceBlock.tileType.Invisible && !ActionMode.Mine.IsActive)
			{
				return HitResult.Default;
			}
			if (mode == Mode.Ramp && (pos.cell.HasRamp || EClass._map.GetRampDir(pos.x, pos.z, EClass.sources.blocks.rows[ramp].tileType) == -1))
			{
				return HitResult.Default;
			}
			return HitResult.Valid;
		}
		if (!mined && !pos.HasObj && owner != null && !owner.IsAgent && pos.Equals(owner.pos) && pos.Installed == null && EClass._zone.CanDigUnderground)
		{
			return HitResult.Valid;
		}
		return HitResult.Default;
	}

	public override void OnProgressComplete()
	{
		string recipeID = pos.sourceBlock.RecipeID;
		int hardness = pos.matBlock.hardness;
		switch (mode)
		{
		case Mode.Default:
			if (pos.HasBlock || ActionMode.Mine.IsRoofEditMode())
			{
				if (owner.IsPC)
				{
					EClass.player.stats.digs++;
				}
				EClass._map.MineBlock(pos, recoverBlock: false, owner);
			}
			else if (pos.Installed == null)
			{
				EClass._zone.AddThing("stairsDown_cave", pos.x, pos.z).Install();
				return;
			}
			break;
		case Mode.Ramp:
			EClass._map.MineRamp(pos, (ramp == 3) ? pos.matBlock.ramp : ramp);
			break;
		}
		if (!owner.IsAgent)
		{
			owner.elements.ModExp(220, 20 + hardness);
			if (EClass.rnd(10) == 0)
			{
				EClass._map.TrySmoothPick(pos, ThingGen.Create("pebble"), owner);
			}
			if (EClass.rnd(10) == 0)
			{
				EClass._map.TrySmoothPick(pos, ThingGen.Create("stone"), owner);
			}
			if (EClass._zone.IsCrime(owner, this))
			{
				EClass.player.ModKarma(-1);
			}
			if (EClass.rnd(2) == 0)
			{
				owner.stamina.Mod(-1);
			}
		}
		if (owner != null && owner.IsPC)
		{
			EClass.player.recipes.ComeUpWithRecipe(recipeID, 30);
		}
		mined = true;
	}
}
