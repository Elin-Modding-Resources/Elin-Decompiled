public class TaskDig : BaseTaskHarvest
{
	public enum Mode
	{
		Default,
		Ramp,
		RemoveFloor
	}

	public Mode mode;

	public int ramp = 3;

	public override HarvestType harvestType => HarvestType.Floor;

	public override int RightHand => 1101;

	public override bool IsHostileAct => true;

	public override bool LocalAct => false;

	public override int destDist
	{
		get
		{
			if (!EClass._zone.IsSkyLevel)
			{
				if (!EClass._zone.IsRegion)
				{
					return 1;
				}
				return 0;
			}
			return 1;
		}
	}

	public override bool destIgnoreConnection => true;

	public override CursorInfo CursorIcon => CursorSystem.Dig;

	public override string GetTextSmall(Card c)
	{
		if (!pos.cell.HasBridge)
		{
			return pos.cell.GetFloorName();
		}
		return pos.cell.GetBridgeName();
	}

	public override void OnCreateProgress(Progress_Custom p)
	{
		SetTarget(owner);
		p.textHint = pos.cell.GetFloorName();
		p.maxProgress = maxProgress;
		p.onProgressBegin = delegate
		{
			if (base.IsTooHard)
			{
				owner.Say("tooHardToDig", owner, pos.cell.HasBridge ? pos.cell.GetBridgeName() : pos.cell.GetFloorName());
				p.Cancel();
			}
			else if (owner.Tool != null)
			{
				owner.Say("dig_start", owner, owner.Tool);
			}
		};
		p.onProgress = delegate
		{
			SourceMaterial.Row row = (pos.cell.HasBridge ? pos.cell.matBridge : pos.cell.matFloor);
			owner.PlaySound(row.GetSoundImpact());
			row.PlayHitEffect(pos);
			row.AddBlood(pos);
			owner.elements.ModExp(230, 5f);
			owner.renderer.NextFrame();
			if (EClass._zone.IsCrime(owner, this))
			{
				owner.pos.TryWitnessCrime(owner);
			}
		};
	}

	public override HitResult GetHitResult()
	{
		if (EClass._zone.IsRegion && GetTreasureMap() != null)
		{
			return HitResult.Valid;
		}
		if (mode == Mode.RemoveFloor)
		{
			if (EClass._zone.IsRegion)
			{
				if (pos.matFloor.category == "soil")
				{
					return HitResult.Valid;
				}
				return HitResult.Default;
			}
			if (EClass._zone.IsSkyLevel && (pos.IsSky || pos.Installed != null || pos.Charas.Count >= 2 || (pos.HasChara && (pos.FirstChara != EClass.pc || EClass.game.IsSurvival))))
			{
				return HitResult.Invalid;
			}
			if (!pos.IsInBounds || pos.IsWater || pos.HasObj || (!EClass._zone.IsPCFaction && pos.HasBlock))
			{
				return HitResult.Invalid;
			}
			if (!pos.HasBridge && !EClass._zone.IsSkyLevel && pos.sourceFloor.id == 40)
			{
				return HitResult.Invalid;
			}
			return HitResult.Valid;
		}
		if (pos.HasBridge)
		{
			if (pos.HasObj)
			{
				return HitResult.Warning;
			}
			return HitResult.Valid;
		}
		return HitResult.Default;
	}

	public Thing GetTreasureMap()
	{
		foreach (Thing item in EClass.pc.things.List((Thing t) => t.trait is TraitScrollMapTreasure))
		{
			TraitScrollMapTreasure traitScrollMapTreasure = item.trait as TraitScrollMapTreasure;
			if (pos.Equals(traitScrollMapTreasure.GetDest(fix: true)))
			{
				return item;
			}
		}
		return null;
	}

	public override void OnProgressComplete()
	{
		string idRecipe = (pos.HasBridge ? pos.sourceBridge.RecipeID : pos.sourceFloor.RecipeID);
		int num = (pos.HasBridge ? pos.matBridge.hardness : pos.matFloor.hardness);
		if (EClass._zone.IsRegion)
		{
			Thing map = GetTreasureMap();
			if (map != null || EClass.debug.enable)
			{
				if (map == null)
				{
					map = ThingGen.Create("map_treasure", -1, EClass.pc.LV);
				}
				SE.Play("ding_skill");
				Msg.Say("digTreasure");
				Rand.UseSeed(map.refVal, delegate
				{
					Thing thing = ThingGen.CreateTreasure("chest_treasure", map.LV);
					EClass._zone.AddCard(thing, EClass.pc.pos);
					ThingGen.TryLickChest(thing);
				});
				map.Destroy();
				EClass.player.willAutoSave = true;
				return;
			}
		}
		switch (mode)
		{
		case Mode.Default:
			EClass._map.SetBridge(pos.x, pos.z);
			break;
		case Mode.RemoveFloor:
			EClass._map.MineFloor(pos, owner);
			pos.Animate(AnimeID.Dig, animeBlock: true);
			if (!owner.IsAgent)
			{
				owner.elements.ModExp(230, 20 + num / 2);
			}
			break;
		case Mode.Ramp:
			EClass._map.MineFloor(pos, owner);
			break;
		}
		if (EClass._zone.IsCrime(owner, this))
		{
			EClass.player.ModKarma(-1);
		}
		if (EClass.rnd(2) == 0)
		{
			owner.stamina.Mod(-1);
		}
		if (owner != null)
		{
			if (owner.IsPC)
			{
				EClass.player.recipes.ComeUpWithRecipe(idRecipe, 30);
			}
			if (owner.IsPC && owner.IsAliveInCurrentZone && EClass._zone.IsSkyLevel && !EClass.game.IsSurvival && owner.pos.IsSky)
			{
				EClass.pc.FallFromZone();
			}
		}
	}
}
