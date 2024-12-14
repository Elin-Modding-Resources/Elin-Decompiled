using UnityEngine;

public class GrowSystemPasture : GrowSystemWheat
{
	public override int HarvestStage => -1;

	public override bool CanReapSeed()
	{
		return base.stage.idx >= 1;
	}

	public override void OnMineObj(Chara c = null)
	{
		if (IsWithered() || base.stage.idx == 0)
		{
			base.OnMineObj(c);
			return;
		}
		int num = 1 + EClass.rnd(base.stage.idx);
		Thing thing = EClass._map.TryGetPlant(GrowSystem.cell)?.seed;
		if (thing != null && thing.encLV > 1 && !EClass._zone.IsUserZone)
		{
			num += EClass.rndHalf((int)Mathf.Sqrt(thing.encLV) + 1);
		}
		PopHarvest(c, "pasture", num);
	}
}
