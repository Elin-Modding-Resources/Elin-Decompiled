public class TaskDrawWater : TaskDesignation
{
	public TraitToolWaterPot pot;

	public override int destDist => 1;

	public override bool CanPressRepeat => true;

	public override bool Loop
	{
		get
		{
			if (CanProgress())
			{
				return !owner.HasCondition<ConSuffocation>();
			}
			return false;
		}
	}

	public override CursorInfo CursorIcon => CursorSystem.Hand;

	public override bool CanProgress()
	{
		if (base.CanProgress() && pos.cell.IsTopWater && pot.owner.c_charges < pot.MaxCharge)
		{
			return owner.Tool == pot.owner;
		}
		return false;
	}

	public override bool CanManualCancel()
	{
		return true;
	}

	public override void OnCreateProgress(Progress_Custom p)
	{
		p.textHint = Name;
		p.maxProgress = 6;
		p.onProgressBegin = delegate
		{
			if (owner.Tool != null)
			{
				owner.Say("drawWater_start", owner, owner.Tool);
			}
		};
		p.onProgress = delegate
		{
			SourceMaterial.Row row = (pos.cell.HasBridge ? pos.cell.matBridge : pos.cell.matFloor);
			row.PlayHitEffect(pos);
			owner.PlaySound(row.GetSoundImpact());
		};
		p.onProgressComplete = delegate
		{
			Effect.Get("mine").Play(pos).SetParticleColor(pos.cell.HasBridge ? pos.matBridge.GetColor() : pos.matFloor.GetColor())
				.Emit(10 + EClass.rnd(10));
			pos.Animate(AnimeID.Dig, animeBlock: true);
			owner.PlaySound("water");
			pot.owner.Dye(pos.HasBridge ? pos.matBridge : pos.matFloor);
			switch ((pos.HasBridge ? pos.sourceBridge : pos.sourceFloor).alias)
			{
			case "floor_water_shallow":
				ChangeFloor("floor_water_shallow2");
				break;
			case "floor_water":
				ChangeFloor("floor_water_shallow");
				break;
			case "floor_water_deep":
				ChangeFloor("floor_water");
				break;
			default:
				ChangeFloor("floor_raw3");
				break;
			}
			pot.owner.ModCharge(1);
			owner.elements.ModExp(286, 5);
			if (EClass.rnd(3) == 0)
			{
				owner.stamina.Mod(-1);
			}
		};
		void ChangeFloor(string id)
		{
			SourceFloor.Row row2 = EClass.sources.floors.alias[id];
			if (pos.HasBridge)
			{
				pos.cell._bridge = (byte)row2.id;
				if (id == "floor_raw3")
				{
					pos.cell._bridgeMat = 45;
				}
			}
			else
			{
				pos.cell._floor = (byte)row2.id;
				if (id == "floor_raw3")
				{
					pos.cell._floorMat = 45;
				}
			}
			EClass._map.SetLiquid(pos.x, pos.z);
			pos.RefreshNeighborTiles();
		}
	}

	public override HitResult GetHitResult()
	{
		if (!pos.cell.IsTopWater || pos.HasObj || pos.cell.HasFullBlock)
		{
			return HitResult.Invalid;
		}
		return HitResult.Valid;
	}
}
