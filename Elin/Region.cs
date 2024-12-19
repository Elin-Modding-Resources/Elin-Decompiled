using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class Region : Zone
{
	public EloMap elomap = new EloMap();

	[JsonProperty]
	public int dateCheckSites;

	public override bool WillAutoSave => false;

	public override ActionMode DefaultActionMode => ActionMode.Region;

	public override bool IsRegion => true;

	public override int DangerLv => 1;

	public override bool BlockBorderExit => true;

	public override Point RegionPos => _regionPos.Set(EClass.pc.pos);

	public override void OnActivate()
	{
		children.ForeachReverse(delegate(Spatial _z)
		{
			Zone zone = _z as Zone;
			if (zone.CanDestroy())
			{
				zone.Destroy();
			}
		});
		children.ForeachReverse(delegate(Spatial _z)
		{
			if (_z.destryoed)
			{
				children.Remove(_z);
			}
		});
		children.ForEach(delegate(Spatial a)
		{
			Zone zone2 = a as Zone;
			if (!zone2.IsInstance && (zone2.IsPCFaction || !(zone2 is Zone_Field)) && !(zone2 is Zone_SisterHouse))
			{
				elomap.SetZone(zone2.x, zone2.y, zone2);
			}
		});
		CheckRandomSites();
	}

	public void CheckRandomSites()
	{
		if (EClass.world.date.IsExpired(dateCheckSites))
		{
			dateCheckSites = EClass.world.date.GetRaw() + 1440;
			UpdateRandomSites();
		}
		if (FindZone("foxtown_nefu") == null)
		{
			SpatialGen.Create("foxtown_nefu", this, register: true);
		}
		if (FindZone("little_garden") == null)
		{
			SpatialGen.Create("little_garden", this, register: true);
		}
		if (FindZone("cave_yeek") == null)
		{
			SpatialGen.Create("cave_yeek", this, register: true);
		}
		elomap.objmap.UpdateMeshImmediate();
	}

	public void RenewRandomSites()
	{
		dateCheckSites = 0;
		Msg.Say("renewNefia");
		Debug.Log(ListRandomSites().Count);
		foreach (Zone item in ListRandomSites())
		{
			item.dateExpire = 1;
		}
	}

	public void UpdateRandomSites()
	{
		List<Zone> list = ListRandomSites();
		int num = 50 - list.Count;
		if (num > 0)
		{
			for (int i = 0; i < num; i++)
			{
				CreateRandomSite(GetRandomPoint(), null, updateMesh: false);
			}
		}
	}

	public void InitElomap()
	{
		EClass.scene.elomapActor.Initialize(elomap);
	}

	public Zone CreateRandomSite(Zone center, int radius = 8, string idSource = null, bool updateMesh = true, int lv = 0)
	{
		InitElomap();
		return CreateRandomSite(GetRandomPoint(center.IsRegion ? (EClass.pc.pos.x + EClass.scene.elomap.minX) : center.x, center.IsRegion ? (EClass.pc.pos.z + EClass.scene.elomap.minY) : center.y, radius), idSource, updateMesh, lv);
	}

	private Zone CreateRandomSite(Point pos, string idSource, bool updateMesh, int lv = 0)
	{
		if (pos == null)
		{
			return null;
		}
		if (idSource.IsEmpty())
		{
			idSource = GetRandomSiteSource().id;
		}
		Zone zone = SpatialGen.Create(idSource, this, register: true, pos.x, pos.z) as Zone;
		if (lv <= 0)
		{
			if (EClass.player.CountKeyItem("license_adv") == 0 && !EClass.debug.enable)
			{
				lv = ((EClass.rnd(3) == 0) ? EClass.pc.LV : EClass.pc.FameLv) * (75 + EClass.rnd(50)) / 100 + EClass.rnd(EClass.rnd(10) + 1) - 3;
				if (lv >= 50)
				{
					lv = EClass.rndHalf(50);
				}
			}
			else
			{
				lv = EClass.pc.FameLv * 100 / (100 + EClass.rnd(50)) + EClass.rnd(EClass.rnd(10) + 1) - 3;
				if (EClass.rnd(10) == 0)
				{
					lv = lv * 3 / 2;
				}
				if (EClass.rnd(10) == 0)
				{
					lv /= 2;
				}
			}
		}
		zone._dangerLv = Mathf.Max(1, lv);
		zone.isRandomSite = true;
		zone.dateExpire = EClass.world.date.GetRaw() + 10080;
		if (elomap.IsSnow(zone.x, zone.y))
		{
			zone.icon++;
		}
		elomap.SetZone(zone.x, zone.y, zone);
		if (updateMesh)
		{
			elomap.objmap.UpdateMeshImmediate();
		}
		return zone;
	}

	public SourceZone.Row GetRandomSiteSource()
	{
		return EClass.sources.zones.rows.Where((SourceZone.Row a) => a.tag.Contains("random") && (EClass.debug.enable || !a.tag.Contains("debug"))).ToList().RandomItemWeighted((SourceZone.Row a) => a.chance);
	}

	public Point GetRandomPoint()
	{
		Point point = new Point();
		for (int i = 0; i < 1000; i++)
		{
			point = map.bounds.GetRandomPoint();
			point.x += elomap.minX;
			point.z += elomap.minY;
			if (elomap.CanBuildSite(point.x, point.z, 1))
			{
				return point;
			}
		}
		return null;
	}

	public Point GetRandomPoint(int orgX, int orgY, int radius = 8, bool increaseRadius = false)
	{
		Point point = new Point();
		for (int i = 0; i < 1000; i++)
		{
			point.x = orgX + Rand.Range(-radius / 2, radius / 2);
			point.z = orgY + Rand.Range(-radius / 2, radius / 2);
			if (i % 100 == 0 && increaseRadius)
			{
				radius++;
			}
			if (elomap.CanBuildSite(point.x, point.z))
			{
				return point;
			}
		}
		return null;
	}

	public Point GetRandomPoint(int orgX, int orgY, int minRadius, int maxRadius)
	{
		Point point = new Point();
		Point p = new Point(orgX, orgY);
		for (int i = 0; i < 1000; i++)
		{
			point.x = orgX + Rand.Range(-maxRadius, maxRadius);
			point.z = orgY + Rand.Range(-maxRadius, maxRadius);
			if (point.Distance(p) >= minRadius && !point.IsBlocked && elomap.CanBuildSite(point.x + elomap.minX, point.z + elomap.minY, 0, ElomapSiteType.Mob))
			{
				return point;
			}
		}
		return null;
	}

	public bool CanCreateZone(Point pos)
	{
		EClass.scene.elomapActor.Initialize(EClass.world.region.elomap);
		EloMap.TileInfo tileInfo = EClass.scene.elomapActor.elomap.GetTileInfo(pos.x, pos.z);
		if (!tileInfo.idZoneProfile.IsEmpty())
		{
			return !tileInfo.blocked;
		}
		return false;
	}

	public Zone CreateZone(Point pos)
	{
		return SpatialGen.Create("field", this, register: true, pos.x, pos.z) as Zone;
	}

	public List<Zone> ListTowns()
	{
		List<Zone> list = new List<Zone>();
		foreach (Spatial value in EClass.game.spatials.map.Values)
		{
			if (value.CanSpawnAdv)
			{
				list.Add(value as Zone);
			}
		}
		return list;
	}

	public Zone GetRandomTown()
	{
		List<Zone> list = ListTowns();
		Zone zone = null;
		for (int i = 0; i < 5; i++)
		{
			zone = list.RandomItem();
			_ = zone is Zone_SubTown;
		}
		return zone;
	}

	public List<Zone> ListRandomSites()
	{
		return (from Zone a in children
			where a.isRandomSite
			select a).ToList();
	}

	public List<Zone> ListZonesInRadius(Zone center, int radius = 10)
	{
		List<Zone> list = new List<Zone>();
		foreach (Zone zone in EClass.game.spatials.Zones)
		{
			if (zone.Dist(center) <= radius && zone != center && !(zone.source.parent != base.source.id))
			{
				list.Add(zone);
			}
		}
		return list;
	}

	public List<Zone> ListTravelZones(int radius = 100)
	{
		bool isRegion = EClass.pc.currentZone.IsRegion;
		List<Zone> list = new List<Zone>();
		if (!isRegion)
		{
			new Point(EClass.pc.currentZone.x - EClass.scene.elomap.minX, EClass.pc.currentZone.y - EClass.scene.elomap.minY);
		}
		else
		{
			_ = EClass.pc.pos;
		}
		foreach (Zone zone in EClass.game.spatials.Zones)
		{
			if (zone.CanFastTravel && !zone.IsInstance && (zone.isKnown || EClass.debug.returnAnywhere) && zone.parent == this)
			{
				list.Add(zone);
				zone.tempDist = zone.Dist(EClass.pc.pos);
			}
		}
		return list;
	}

	public override void OnAdvanceHour()
	{
		if (EClass.world.date.hour == 0)
		{
			foreach (Chara item in ListMobs())
			{
				if (item.Dist(EClass.pc) > 20 && EClass.rnd(2) == 0)
				{
					item.Destroy();
				}
			}
		}
		if (ListMobs().Count >= (EClass.debug.enable ? 100 : 6) || EClass.rnd(3) != 0)
		{
			return;
		}
		Point randomPoint = GetRandomPoint(EClass.pc.pos.x, EClass.pc.pos.z, 8, 14);
		if (randomPoint != null)
		{
			RegionPoint regionPoint = new RegionPoint(EClass.pc.pos);
			BiomeProfile biome = regionPoint.biome;
			SpawnList list = ((biome.spawn.chara.Count <= 0) ? SpawnList.Get(biome.name, "chara", new CharaFilter
			{
				ShouldPass = (SourceChara.Row s) => s.biome == biome.name || s.biome.IsEmpty()
			}) : SpawnList.Get(biome.spawn.GetRandomCharaId()));
			Chara chara = CharaGen.CreateFromFilter(list, regionPoint.dangerLv);
			if (chara != null)
			{
				AddCard(chara, randomPoint);
			}
		}
	}

	public List<Chara> ListMobs()
	{
		List<Chara> list = new List<Chara>();
		foreach (Chara chara in EClass._map.charas)
		{
			if (!chara.IsPCFactionOrMinion && !chara.IsGlobal)
			{
				list.Add(chara);
			}
		}
		return list;
	}
}
