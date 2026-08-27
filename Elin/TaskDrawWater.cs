using System.Collections.Generic;

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
			owner.PlaySound("water");
			int num = owner.Tool?.Evalue(770) ?? 0;
			num = ((num <= 0) ? 1 : (2 + num / 10));
			if (num > 1)
			{
				List<Point> list = EClass._map.ListPointsInSquare(pos, num - 1, mustBeWalkable: false);
				list.Sort((Point a, Point b) => a.Distance(pos) - b.Distance(pos));
				{
					foreach (Point item in list)
					{
						if (owner == null || owner.isDead)
						{
							break;
						}
						Draw(item);
					}
					return;
				}
			}
			Draw(pos);
		};
		void Draw(Point point)
		{
			if (pot.owner.c_charges < pot.MaxCharge && GetHitResult(point) != HitResult.Invalid)
			{
				Effect.Get("mine").Play(point).SetParticleColor(point.cell.HasBridge ? point.matBridge.GetColor() : point.matFloor.GetColor())
					.Emit(10 + EClass.rnd(10));
				point.Animate(AnimeID.Dig, animeBlock: true);
				pot.owner.Dye(point.HasBridge ? point.matBridge : point.matFloor);
				switch ((point.HasBridge ? point.sourceBridge : point.sourceFloor).alias)
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
				owner.elements.ModExp(286, 5f);
				if (EClass.rnd(3) == 0)
				{
					owner.stamina.Mod(-1);
				}
			}
			void ChangeFloor(string id)
			{
				SourceFloor.Row row = EClass.sources.floors.alias[id];
				if (point.HasBridge)
				{
					point.cell._bridge = (byte)row.id;
					if (id == "floor_raw3")
					{
						point.cell._bridgeMat = 45;
					}
				}
				else
				{
					point.cell._floor = (byte)row.id;
					if (id == "floor_raw3")
					{
						point.cell._floorMat = 45;
					}
				}
				EClass._map.SetLiquid(point.x, point.z);
				point.RefreshNeighborTiles();
			}
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
