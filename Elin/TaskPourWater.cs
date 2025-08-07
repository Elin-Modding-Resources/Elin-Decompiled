public class TaskPourWater : TaskDesignation
{
	public TraitToolWaterPot pot;

	public override int destDist => 1;

	public override bool CanPressRepeat => true;

	public override bool Loop => CanProgress();

	public override bool CanProgress()
	{
		if (base.CanProgress() && !pos.HasBridge && pos.cell.sourceSurface.alias != "floor_water_deep" && pot.owner.c_charges > 0)
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
				owner.Say("pourWater_start", owner, owner.Tool);
			}
		};
		p.onProgress = delegate
		{
			(pos.cell.HasBridge ? pos.cell.matBridge : pos.cell.matFloor).PlayHitEffect(pos);
			owner.PlaySound(MATERIAL.sourceWaterSea.GetSoundImpact());
		};
		p.onProgressComplete = delegate
		{
			if (pot.owner.DyeMat == null)
			{
				pot.owner.Dye(MATERIAL.sourceWaterSea);
			}
			switch ((pos.HasBridge ? pos.sourceBridge : pos.sourceFloor).alias)
			{
			case "floor_water_shallow2":
				ChangeFloor("floor_water_shallow");
				break;
			case "floor_water_shallow":
				ChangeFloor("floor_water");
				break;
			case "floor_water":
				ChangeFloor("floor_water_deep");
				break;
			default:
				ChangeFloor("floor_water_shallow2");
				break;
			}
			Effect.Get("mine").Play(pos).SetParticleColor(pos.cell.HasBridge ? pos.matBridge.GetColor() : pos.matFloor.GetColor())
				.Emit(10 + EClass.rnd(10));
			pos.Animate(AnimeID.Dig, animeBlock: true);
			owner.PlaySound("water_farm");
			pot.owner.ModCharge(-1);
			if (pot.owner.c_charges <= 0)
			{
				pot.owner.Dye(EClass.sources.materials.alias["void"]);
			}
			owner.elements.ModExp(286, 5f);
			if (EClass.rnd(3) == 0)
			{
				owner.stamina.Mod(-1);
			}
		};
		void ChangeFloor(string id)
		{
			SourceFloor.Row row = EClass.sources.floors.alias[id];
			if (pos.HasBridge)
			{
				pos.cell._bridge = (byte)row.id;
				pos.cell._bridgeMat = (byte)pot.owner.DyeMat.id;
			}
			else
			{
				pos.cell._floor = (byte)row.id;
				pos.cell._floorMat = (byte)pot.owner.DyeMat.id;
			}
			EClass._map.SetLiquid(pos.x, pos.z);
			pos.RefreshNeighborTiles();
		}
	}

	public override HitResult GetHitResult()
	{
		if (pos.HasBridge || pos.HasObj || pos.cell.HasFullBlock)
		{
			return HitResult.Invalid;
		}
		return HitResult.Valid;
	}
}
