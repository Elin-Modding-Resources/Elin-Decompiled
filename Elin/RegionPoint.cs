using UnityEngine;

public class RegionPoint : Point
{
	public int dangerLv;

	public EloMap.TileInfo tile;

	public BiomeProfile biome;

	public RegionPoint(Point p)
	{
		SetRegionPoint(p.x, p.z);
	}

	public void SetRegionPoint(int _x, int _z)
	{
		Set(_x + EClass.scene.elomap.minX, _z + EClass.scene.elomap.minY);
		EClass._zone.Region.GetZoneAt(x, z);
		int roadDist = EClass.scene.elomap.GetRoadDist(x, z);
		int num = ((EClass.pc.homeBranch != null) ? EClass.pc.pos.Distance(EClass.game.StartZone.mapX, EClass.game.StartZone.mapY) : 0);
		tile = EClass.scene.elomapActor.elomap.GetTileInfo(x, z);
		string key = tile.source.idBiome.IsEmpty("Plain");
		biome = EClass.core.refs.biomes.dict[key];
		int num2 = Mathf.Clamp(roadDist - 4, 0, 200) + Mathf.Clamp(num / 4, 0, 10);
		if (roadDist > 20)
		{
			num2 += roadDist - 20;
		}
		if (num2 >= 100)
		{
			num2 = 100;
		}
		Mathf.Min(num2 / 10, 4);
		EClass.rnd(3 + Mathf.Min(num2 / 10, 4));
		_ = 5;
		dangerLv = num2 + 5;
	}
}
