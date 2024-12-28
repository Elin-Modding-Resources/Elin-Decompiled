using System.Collections.Generic;
using System.Runtime.Serialization;
using Algorithms;
using Newtonsoft.Json;
using UnityEngine;

public class Spatial : EClass
{
	[JsonProperty]
	public Spatial parent;

	[JsonProperty]
	public string id;

	[JsonProperty]
	public string idUser;

	[JsonProperty]
	public string name;

	[JsonProperty]
	public string idMainFaction;

	[JsonProperty]
	public string idProfile;

	[JsonProperty]
	public string idCurrentSubset;

	[JsonProperty]
	public string idHat;

	[JsonProperty]
	public int[] _ints = new int[30];

	[JsonProperty]
	public List<Spatial> children = new List<Spatial>();

	[JsonProperty]
	public List<Spatial> connections = new List<Spatial>();

	[JsonProperty(PropertyName = "Y")]
	public Dictionary<int, int> mapInt = new Dictionary<int, int>();

	public bool destryoed;

	public bool isImported;

	public BitArray32 bits;

	private SourceZone.Row _source;

	private EloMap.TileInfo _tile;

	protected Point _regionPos = new Point();

	public int _bits
	{
		get
		{
			return _ints[0];
		}
		set
		{
			_ints[0] = value;
		}
	}

	public int uid
	{
		get
		{
			return _ints[1];
		}
		set
		{
			_ints[1] = value;
		}
	}

	public int icon
	{
		get
		{
			return _ints[2];
		}
		set
		{
			_ints[2] = value;
		}
	}

	public int x
	{
		get
		{
			return _ints[3];
		}
		set
		{
			_ints[3] = value;
		}
	}

	public int y
	{
		get
		{
			return _ints[4];
		}
		set
		{
			_ints[4] = value;
		}
	}

	public int lastActive
	{
		get
		{
			return _ints[5];
		}
		set
		{
			_ints[5] = value;
		}
	}

	public int idPrefix
	{
		get
		{
			return _ints[6];
		}
		set
		{
			_ints[6] = value;
		}
	}

	public int lv
	{
		get
		{
			return _ints[7];
		}
		set
		{
			_ints[7] = value;
		}
	}

	public int visitCount
	{
		get
		{
			return _ints[8];
		}
		set
		{
			_ints[8] = value;
		}
	}

	public int dateExpire
	{
		get
		{
			return _ints[9];
		}
		set
		{
			_ints[9] = value;
		}
	}

	public int dateRevive
	{
		get
		{
			return _ints[10];
		}
		set
		{
			_ints[10] = value;
		}
	}

	public int _dangerLv
	{
		get
		{
			return _ints[11];
		}
		set
		{
			_ints[11] = value;
		}
	}

	public int dateRegenerate
	{
		get
		{
			return _ints[12];
		}
		set
		{
			_ints[12] = value;
		}
	}

	public int influence
	{
		get
		{
			return _ints[13];
		}
		set
		{
			_ints[13] = value;
		}
	}

	public int investment
	{
		get
		{
			return _ints[14];
		}
		set
		{
			_ints[14] = value;
		}
	}

	public int development
	{
		get
		{
			return _ints[15];
		}
		set
		{
			_ints[15] = value;
		}
	}

	public int electricity
	{
		get
		{
			return _ints[16];
		}
		set
		{
			_ints[16] = value;
		}
	}

	public int dateHat
	{
		get
		{
			return _ints[17];
		}
		set
		{
			_ints[17] = value;
		}
	}

	public int uidBoss
	{
		get
		{
			return _ints[18];
		}
		set
		{
			_ints[18] = value;
		}
	}

	public int dateQuest
	{
		get
		{
			return _ints[19];
		}
		set
		{
			_ints[19] = value;
		}
	}

	public int version
	{
		get
		{
			return _ints[20];
		}
		set
		{
			_ints[20] = value;
		}
	}

	public bool isGenerated
	{
		get
		{
			return bits[0];
		}
		set
		{
			bits[0] = value;
		}
	}

	public bool isShore
	{
		get
		{
			return bits[3];
		}
		set
		{
			bits[3] = value;
		}
	}

	public bool isRandomSite
	{
		get
		{
			return bits[6];
		}
		set
		{
			bits[6] = value;
		}
	}

	public bool isKnown
	{
		get
		{
			return bits[7];
		}
		set
		{
			bits[7] = value;
		}
	}

	public bool isMapSaved
	{
		get
		{
			return bits[8];
		}
		set
		{
			bits[8] = value;
		}
	}

	public bool isExternalZone
	{
		get
		{
			return bits[9];
		}
		set
		{
			bits[9] = value;
		}
	}

	public bool isConquered
	{
		get
		{
			return bits[10];
		}
		set
		{
			bits[10] = value;
		}
	}

	public bool isBeach
	{
		get
		{
			return bits[11];
		}
		set
		{
			bits[11] = value;
		}
	}

	public bool isPeace
	{
		get
		{
			return bits[12];
		}
		set
		{
			bits[12] = value;
		}
	}

	public bool isDeathLocation
	{
		get
		{
			return bits[13];
		}
		set
		{
			bits[13] = value;
		}
	}

	public Faction mainFaction
	{
		get
		{
			return EClass.game.factions.dictAll.TryGetValue(idMainFaction) ?? EClass.Wilds;
		}
		set
		{
			idMainFaction = ((value == null) ? EClass.Wilds.uid : value.uid);
		}
	}

	public SourceZone.Row source => _source ?? (_source = EClass.sources.zones.map[id]);

	public bool IsPlayerFaction => mainFaction == EClass.pc.faction;

	public bool IsClosed => source.tag.Contains("closed");

	public int mapX
	{
		get
		{
			if (!EClass._zone.IsRegion)
			{
				return x;
			}
			return x - EClass.scene.elomap.minX;
		}
	}

	public int mapY
	{
		get
		{
			if (!EClass._zone.IsRegion)
			{
				return y;
			}
			return y - EClass.scene.elomap.minY;
		}
	}

	public virtual int ContentLv => DangerLv;

	public virtual int DangerLv => _dangerLv;

	public virtual int DangerLvFix => 0;

	public virtual float VolumeSea
	{
		get
		{
			if (!isShore)
			{
				return 0f;
			}
			return 1f;
		}
	}

	public virtual bool ShowDangerLv => false;

	public virtual bool CanSpawnAdv => false;

	public string pathSave => GameIO.pathCurrentSave + uid + "/";

	public virtual string Name => ((idPrefix == 0) ? "" : (EClass.sources.zoneAffixes.map[idPrefix].GetName().ToTitleCase() + Lang.space)) + name.IsEmpty(source.GetText()) + NameSuffix;

	public string NameWithDangerLevel => Name + (isRandomSite ? "dangerLv".lang(DangerLv.ToString() ?? "") : "");

	public virtual string NameSuffix => "";

	public virtual bool IsRegion => false;

	public virtual bool CanFastTravel
	{
		get
		{
			if (!isRandomSite)
			{
				return !IsClosed;
			}
			return false;
		}
	}

	public EloMap.TileInfo Tile
	{
		get
		{
			if (_tile == null)
			{
				EClass.scene.elomapActor.Initialize(EClass.world.region.elomap);
				_tile = EClass.scene.elomapActor.elomap.GetTileInfo(x, y);
			}
			return _tile;
		}
	}

	public virtual bool IsSnowZone => Tile.IsSnow;

	public virtual bool IsSnowCovered
	{
		get
		{
			if (!EClass._map.IsIndoor)
			{
				if (!IsSnowZone)
				{
					return EClass.world.season.isWinter;
				}
				return true;
			}
			return false;
		}
	}

	public virtual Point RegionPos => _regionPos.Set(x - EClass.scene.elomap.minX, y - EClass.scene.elomap.minY);

	public virtual bool isClaimable => false;

	public int Seed => EClass.game.seed + Mathf.Abs(x) * 1000 + Mathf.Abs(y);

	public int GetInt(int id, int? defaultInt = null)
	{
		if (mapInt.TryGetValue(id, out var value))
		{
			return value;
		}
		return defaultInt.GetValueOrDefault();
	}

	public void AddInt(int id, int value)
	{
		SetInt(id, GetInt(id) + value);
	}

	public void SetInt(int id, int value = 0)
	{
		if (value == 0)
		{
			if (mapInt.ContainsKey(id))
			{
				mapInt.Remove(id);
			}
		}
		else
		{
			mapInt[id] = value;
		}
	}

	public override string ToString()
	{
		return Name + "(" + uid + ")";
	}

	[OnSerializing]
	private void OnSerializing(StreamingContext context)
	{
		_bits = (int)bits.Bits;
	}

	[OnDeserialized]
	private void OnDeserialized(StreamingContext context)
	{
		bits.Bits = (uint)_bits;
	}

	public void Create(string _id, int _x, int _y, int _icon)
	{
		x = _x;
		y = _y;
		icon = _icon;
		Rand.SetSeed(Seed);
		id = _id;
		development = source.dev;
		_ = source.name == "*r";
		_dangerLv = source.LV;
		if (EClass.debug.travelAnywhere)
		{
			isKnown = true;
		}
		OnCreate();
		Rand.SetSeed();
	}

	public void Register()
	{
		if (uid == 0)
		{
			EClass.game.spatials.AssignUID(this);
		}
	}

	public virtual void OnCreate()
	{
	}

	public virtual void OnAfterCreate()
	{
	}

	public virtual void SetMainFaction(Faction f)
	{
		mainFaction = f;
	}

	public Zone GetFirstZone()
	{
		if (this is Zone)
		{
			return this as Zone;
		}
		foreach (Spatial child in children)
		{
			Zone firstZone = child.GetFirstZone();
			if (firstZone != null)
			{
				return firstZone;
			}
		}
		return null;
	}

	public void OnBeforeSave()
	{
		_OnBeforeSave();
	}

	public virtual void _OnBeforeSave()
	{
	}

	public void _OnLoad()
	{
		foreach (Spatial child in children)
		{
			child._OnLoad();
		}
		OnLoad();
	}

	public virtual void OnLoad()
	{
	}

	public virtual void Destroy()
	{
	}

	public void DeleteMapRecursive()
	{
		foreach (Spatial child in children)
		{
			child.DeleteMapRecursive();
		}
		IO.DeleteDirectory(pathSave);
	}

	public virtual void AddChild(Spatial child)
	{
		child.parent = this;
		children.Add(child);
	}

	public void RemoveChild(Spatial child)
	{
		children.Remove(child);
		child.parent = null;
	}

	public Zone FindDeepestZone()
	{
		Spatial spatial = this;
		foreach (Spatial child in children)
		{
			if (!child.isExternalZone && child.lv < spatial.lv)
			{
				spatial = child;
			}
		}
		return spatial as Zone;
	}

	public Zone FindZone(int lv)
	{
		foreach (Spatial child in children)
		{
			if (!child.isExternalZone && child.lv == lv)
			{
				return child as Zone;
			}
		}
		if (this.lv != lv)
		{
			return null;
		}
		return this as Zone;
	}

	public Zone FindZone(string _id)
	{
		foreach (Spatial child in children)
		{
			Zone zone = child._FindZone(_id);
			if (zone != null)
			{
				return zone;
			}
		}
		return null;
	}

	protected Zone _FindZone(string _id)
	{
		foreach (Spatial child in children)
		{
			Zone zone = child._FindZone(_id);
			if (zone != null)
			{
				return zone;
			}
		}
		if (this is Zone && id == _id)
		{
			return (Zone)this;
		}
		return null;
	}

	public virtual bool CanKill()
	{
		return false;
	}

	public int Dist(Point p)
	{
		PathProgress pathProgress = PathManager.Instance.RequestPathImmediate(new Point(x - EClass.scene.elomap.minX, y - EClass.scene.elomap.minY), p, EClass.pc);
		if (pathProgress.HasPath)
		{
			int num = 0;
			{
				foreach (PathFinderNode node in pathProgress.nodes)
				{
					bool flag = EClass.scene.elomap.GetTileInfo(node.X + EClass.scene.elomap.minX, node.Z + EClass.scene.elomap.minY)?.IsSnow ?? false;
					num += ((!flag) ? 1 : 2);
				}
				return num;
			}
		}
		return Fov.Distance(x, y, p.x + EClass.scene.elomap.minX, p.z + EClass.scene.elomap.minY);
	}

	public int Dist(Spatial s)
	{
		return Fov.Distance(s.x, s.y, x, y);
	}

	public void MakeGameObject(GameObject parentGo)
	{
		GameObject gameObject = new GameObject();
		gameObject.name = ToString();
		gameObject.transform.SetParent(parentGo.transform);
		gameObject.AddComponent<SpatialInspector>().spatial = this;
		foreach (Spatial child in children)
		{
			child.MakeGameObject(gameObject);
		}
	}
}
