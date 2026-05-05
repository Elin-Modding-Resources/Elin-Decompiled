public class SpatialGen : EClass
{
	public static Spatial CreateRecursive(string id, Spatial parent = null)
	{
		Spatial spatial = Create(id, parent, register: true);
		foreach (SourceZone.Row row in EClass.sources.zones.rows)
		{
			if ((!row.tag.Contains("debug") || EClass.debug.enable) && row.parent == id)
			{
				CreateRecursive(row.id, spatial);
			}
		}
		return spatial;
	}

	public static Spatial Create(string id, Spatial parent, bool register, int x = -99999, int y = -99999, int icon = 0)
	{
		SourceZone.Row row = EClass.sources.zones.map[id];
		Spatial spatial = ClassCache.Create<Spatial>(row.type, "Elin") ?? new Zone();
		if (x == -99999)
		{
			x = ((row.pos.Length != 0) ? row.pos[0] : (-1000));
			y = ((row.pos.Length != 0) ? row.pos[1] : (-1000));
		}
		if (icon == 0 && row.pos.Length != 0)
		{
			icon = row.pos[2];
		}
		spatial.Create(id, x, y, icon);
		if (register)
		{
			spatial.Register();
		}
		parent?.AddChild(spatial);
		spatial.OnAfterCreate();
		return spatial;
	}

	public static Zone CreateInstance(string id, ZoneInstance instance)
	{
		Zone topZone = EClass._zone.GetTopZone();
		Zone obj = Create(id, EClass._zone.Region, register: true, topZone.x, topZone.y) as Zone;
		obj.instance = instance;
		instance.x = EClass.pc.pos.x;
		instance.z = EClass.pc.pos.z;
		instance.uidZone = EClass._zone.uid;
		obj.dateExpire = EClass.world.date.GetRaw() + 1440;
		return obj;
	}
}
