using CreativeSpore.SuperTilemapEditor;
using UnityEngine;

public class MapGenRegion : BaseMapGen
{
	private static MapGenRegion _Instance;

	public static MapGenRegion Instance => _Instance ?? (_Instance = new MapGenRegion());

	protected override bool OnGenerateTerrain()
	{
		EClass.scene.elomapActor.Initialize(zone.Region.elomap);
		EloMap elomap = EClass.scene.elomapActor.elomap;
		SetSize(Mathf.Max(elomap.w, elomap.h), 10);
		map.CreateNew(Size);
		for (int i = 0; i < elomap.h; i++)
		{
			for (int j = 0; j < elomap.w; j++)
			{
				_ = elomap.cells[j, i];
				int gridX = elomap.minX + j;
				int gridY = elomap.minY + i;
				map.cells[j, i].isSeen = true;
				SetFloor(j, i, 66, 43);
				foreach (STETilemap tilemap in elomap.group.Tilemaps)
				{
					if (tilemap == elomap.fogmap || tilemap == elomap.cloudmap)
					{
						continue;
					}
					int tileIdFromTileData = Tileset.GetTileIdFromTileData(tilemap.GetTileData(gridX, gridY));
					SourceGlobalTile.Row row = EClass.sources.globalTiles.tileAlias.TryGetValue(tileIdFromTileData);
					if (row != null)
					{
						map.cells[j, i].impassable = row.attribs[0] == 0;
						if (row.floor != 0)
						{
							SetFloor(j, i, EClass.sources.floors[row.floor].DefaultMaterial.id, row.floor);
						}
					}
				}
				if (Tileset.GetTileIdFromTileData(elomap.cloudmap.GetTileData(gridX, gridY)) < 1000)
				{
					map.cells[j, i].impassable = true;
				}
			}
		}
		map.poiMap.Reset();
		map.SetZone(zone);
		map.config.idSceneProfile = "region";
		map.RefreshAllTiles();
		return true;
	}
}
