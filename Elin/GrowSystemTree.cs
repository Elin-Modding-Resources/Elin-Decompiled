using UnityEngine;

public class GrowSystemTree : GrowSystem
{
	public static int[] shadows = new int[8] { 31, 31, 32, 33, 34, 1, 1, 1 };

	public override RenderData RenderHarvest => EClass.core.refs.renderers.objL_harvest;

	public override int Step => 3;

	public override int StageLength => 6;

	public override float MtpProgress
	{
		get
		{
			if (!Bud && !Baby)
			{
				if (!Grown)
				{
					if (!Mature)
					{
						return 1f;
					}
					return 3f;
				}
				return 2f;
			}
			return 0.5f;
		}
	}

	public override int HarvestStage => 4;

	protected override bool DrawHarvestOnTop => true;

	public override bool IsMature
	{
		get
		{
			if (!Grown)
			{
				return Mature;
			}
			return true;
		}
	}

	public override bool IsTree => true;

	public override int ShadowStage => 0;

	public bool Bud => base.stage.idx == 0;

	public bool Baby => base.stage.idx == 1;

	public bool Young => base.stage.idx == 2;

	public bool Grown => base.stage.idx == 3;

	public bool Mature => base.stage.idx == 4;

	public bool Withered => base.stage.idx == 5;

	public bool IsPalulu => source.id == 17;

	public bool IsBamboo => source.id == 103;

	protected override bool UseGenericFirstStageTile => false;

	public override bool BlockPass(Cell cell)
	{
		return cell.objVal / 30 > 1;
	}

	public override bool BlockSight(Cell cell)
	{
		return cell.objVal / 30 > 2;
	}

	public override int GetShadow(int index)
	{
		if (source.id == 103)
		{
			return source.pref.shadow;
		}
		return shadows[index];
	}

	public override void SetGenericFirstStageTile(Stage s)
	{
		s.renderData = EClass.core.refs.renderers.objS_flat;
		s.SetTile(0, 103);
	}

	public override void SetStageTile(Stage s)
	{
		s.renderData = source.renderData;
		s.SetTile(s.idx + ((s.idx != 0) ? (-1) : 0), baseTiles);
	}

	public override void OnHitFail(Chara c)
	{
		if (c != null && (Young || Grown || Mature || Withered))
		{
			Chara chara = CharaGen.Create("putty");
			EClass._zone.AddCard(chara, GrowSystem.cell.GetPoint().GetNearestPoint(allowBlock: false, allowChara: false));
			chara.renderer.PlayAnime(AnimeID.Fall, force: true);
			c.Say("mobFromTree", chara);
		}
	}

	public override int GetHp()
	{
		if (base.stage.idx > HarvestStage)
		{
			return (int)((float)base.GetHp() * 1.5f);
		}
		return (int)((float)base.GetHp() * 0.5f);
	}

	public int GetPlantBonus(PlantData plant)
	{
		if (plant == null || plant.seed == null || EClass._zone.IsUserZone)
		{
			return 0;
		}
		int num = plant.seed.encLV / 10;
		if (EClass.rnd(10) > plant.seed.encLV % 10)
		{
			num++;
		}
		return Mathf.Min(EClass.rnd(num + 1), 4);
	}

	public override void OnMineObj(Chara c = null)
	{
		Point point = GrowSystem.cell.GetPoint();
		int id = GrowSystem.cell.matObj_fixed.id;
		PlantData plant = EClass._map.TryGetPlant(GrowSystem.cell);
		if (Bud || Baby)
		{
			if (EClass.rnd(2) == 0 || IsPalulu)
			{
				TryPick(GrowSystem.cell, IsPalulu ? "leaf_palulu" : "leaf");
			}
			if (IsBamboo)
			{
				PopHarvest(c, "bamboo_shoot");
			}
			TryPick(GrowSystem.cell, "branch", id, 1 + PlantBonus());
			return;
		}
		if (Young)
		{
			if (IsBamboo)
			{
				PopHarvest(c, "bamboo_shoot");
			}
			TryPick(GrowSystem.cell, "branch", id, 2 + PlantBonus());
			if (IsPalulu)
			{
				TryPick(GrowSystem.cell, "leaf_palulu");
			}
			TryPick(GrowSystem.cell, "log", id, EClass.rnd(2) + PlantBonus() / 2, applySeed: true);
			return;
		}
		if (Grown || Mature)
		{
			TryPick(GrowSystem.cell, IsPalulu ? "leaf_palulu" : "bark", -1, 1 + PlantBonus() / 2);
			TryPick(GrowSystem.cell, "log", id, 1 + EClass.rnd(3) + PlantBonus(), applySeed: true);
			TryPick(GrowSystem.cell, "resin", -1, 1 + PlantBonus() / 2);
			if ((!EClass._zone.IsUserZone || !EClass.game.principal.disableUsermapBenefit) && !EClass.player.isAutoFarming)
			{
				if (EClass.rnd(500) == 0)
				{
					TryPick(GrowSystem.cell, "throw_putit");
				}
				if (point.IsFarmField)
				{
					TryPick(GrowSystem.cell, TraitSeed.MakeSeed(point.sourceObj, plant), c);
				}
			}
			return;
		}
		TryPick(GrowSystem.cell, "bark", -1, EClass.rnd(3) + PlantBonus());
		TryPick(GrowSystem.cell, "resin", -1, 1 + PlantBonus() / 2);
		if ((!EClass._zone.IsUserZone || !EClass.game.principal.disableUsermapBenefit) && !EClass.player.isAutoFarming)
		{
			TryPick(GrowSystem.cell, TraitSeed.MakeSeed(GrowSystem.cell.sourceObj, plant).SetNum(1 + EClass.rnd(3)), c);
			if (EClass.rnd(100) == 0)
			{
				TryPick(GrowSystem.cell, "throw_putit");
			}
		}
		int PlantBonus()
		{
			return GetPlantBonus(plant);
		}
	}
}
