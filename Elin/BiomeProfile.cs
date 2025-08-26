using System;
using System.Collections.Generic;
using System.Linq;
using NoiseSystem;
using UnityEngine;

public class BiomeProfile : EScriptable
{
	public enum DoorStyle
	{
		simpleWood,
		FirmWood,
		Stone,
		Jail,
		Rune
	}

	public enum StairsStyle
	{
		Soil,
		Wood,
		Stone,
		Rock
	}

	public class BaseTile
	{
		public enum SubType
		{
			None = 0,
			Rnd5 = 10,
			Rnd10 = 11,
			Rnd20 = 12,
			Pattern = 20
		}

		public enum DirType
		{
			Random8,
			RandomSqrt8,
			RandomSqrtSqrt8
		}

		protected string _GetThing(string id)
		{
			if (id.IsEmpty())
			{
				return "-";
			}
			SourceThing.Row row = EClass.editorSources.things.rows.First((SourceThing.Row a) => a.id == id);
			if (row != null)
			{
				return row.id + "-(" + row.name_JP + ")";
			}
			return "-";
		}

		protected void _SetThing(ref string id, string value)
		{
			id = EClass.editorSources.things.rows.First((SourceThing.Row a) => a.id == value.Split('-')[0]).id;
		}

		protected IEnumerable<string> ThingRows()
		{
			return EClass.editorSources.things.GetListString();
		}

		protected string _GetObj(int id)
		{
			SourceObj.Row row = EClass.editorSources.objs.rows.First((SourceObj.Row a) => a.id == id);
			if (row != null)
			{
				return row.id + "-" + row.alias + "(" + row.name_JP + ")";
			}
			return "-";
		}

		protected void _SetObj(ref int id, string value)
		{
			id = EClass.editorSources.objs.rows.First((SourceObj.Row a) => a.id == int.Parse(value.Split('-')[0])).id;
		}

		protected IEnumerable<string> ObjRows()
		{
			return EClass.editorSources.objs.GetListString();
		}

		protected string _GetFloor(int id)
		{
			SourceFloor.Row row = EClass.editorSources.floors.rows.First((SourceFloor.Row a) => a.id == id);
			if (row != null)
			{
				return row.id + "-" + row.alias + "(" + row.name_JP + ")";
			}
			return "-";
		}

		protected void _SetFloor(ref int id, string value)
		{
			id = EClass.editorSources.floors.rows.First((SourceFloor.Row a) => a.id == int.Parse(value.Split('-')[0])).id;
		}

		protected IEnumerable<string> FloorRows()
		{
			return EClass.editorSources.floors.GetListString();
		}

		protected string _GetBlock(int id)
		{
			SourceBlock.Row row = EClass.editorSources.blocks.rows.First((SourceBlock.Row a) => a.id == id);
			if (row != null)
			{
				return row.id + "-" + row.alias + "(" + row.name_JP + ")";
			}
			return "-";
		}

		protected void _SetBlock(ref int id, string value)
		{
			id = EClass.editorSources.blocks.rows.First((SourceBlock.Row a) => a.id == int.Parse(value.Split('-')[0])).id;
		}

		protected IEnumerable<string> BlockRows()
		{
			return EClass.editorSources.blocks.GetListString();
		}

		protected string _GetMat(int id)
		{
			if (id == -1)
			{
				id = 0;
			}
			SourceMaterial.Row row = EClass.editorSources.materials.rows.First((SourceMaterial.Row a) => a.id == id);
			if (row != null)
			{
				return row.id + "-" + row.alias + "(" + row.name_JP + ")";
			}
			return "-";
		}

		protected void _SetMat(ref int id, string value)
		{
			id = EClass.editorSources.materials.rows.First((SourceMaterial.Row a) => a.id == int.Parse(value.Split('-')[0])).id;
		}

		protected IEnumerable<string> MatRows()
		{
			return EClass.editorSources.materials.GetListString();
		}

		protected string _GetSpawnList(string id)
		{
			if (id.IsEmpty() || !EClass.editorSources.spawnLists.map.ContainsKey(id))
			{
				return "-";
			}
			SourceSpawnList.Row row = EClass.editorSources.spawnLists.rows.First((SourceSpawnList.Row a) => a.id == id);
			if (row != null)
			{
				return row.id;
			}
			return "-";
		}

		protected void _SetSpawnList(ref string id, string value)
		{
			id = ((value == null) ? "" : EClass.editorSources.spawnLists.rows.First((SourceSpawnList.Row a) => a.id == value).id);
		}

		protected IEnumerable<string> _SpawnList()
		{
			return EClass.editorSources.spawnLists.GetListString();
		}
	}

	[Serializable]
	public class Tile : BaseTile
	{
		[HideInInspector]
		public int id;

		[HideInInspector]
		public int mat;

		[HideInInspector]
		public int idSub;

		[HideInInspector]
		public int matSub;

		public SubType subType;

		public DirType dirType;

		public string _mat
		{
			get
			{
				return _GetMat(mat);
			}
			set
			{
				_SetMat(ref mat, value);
			}
		}

		public string _matSub
		{
			get
			{
				return _GetMat(matSub);
			}
			set
			{
				_SetMat(ref matSub, value);
			}
		}

		public int GetDir()
		{
			return dirType switch
			{
				DirType.Random8 => EScriptable.rnd(8), 
				DirType.RandomSqrt8 => EScriptable.rnd(EScriptable.rnd(8) + 1), 
				DirType.RandomSqrtSqrt8 => EScriptable.rnd(EScriptable.rnd(EScriptable.rnd(8) + 1) + 1), 
				_ => EScriptable.rnd(8), 
			};
		}
	}

	[Serializable]
	public class TileFloor : Tile
	{
		public string _id
		{
			get
			{
				return _GetFloor(id);
			}
			set
			{
				_SetFloor(ref id, value);
			}
		}

		public string _idSub
		{
			get
			{
				return _GetFloor(idSub);
			}
			set
			{
				_SetFloor(ref idSub, value);
			}
		}
	}

	[Serializable]
	public class TileBlock : Tile
	{
		public string _id
		{
			get
			{
				return _GetBlock(id);
			}
			set
			{
				_SetBlock(ref id, value);
			}
		}

		public string _idSub
		{
			get
			{
				return _GetBlock(idSub);
			}
			set
			{
				_SetBlock(ref idSub, value);
			}
		}
	}

	[Serializable]
	public class TileGroup
	{
		public TileFloor floor;

		public TileBlock block;
	}

	[Serializable]
	public class Style : BaseTile
	{
		public float doorChance = 0.9f;

		public DoorStyle doorStyle;

		[HideInInspector]
		public int matDoor;

		public StairsStyle stairsStyle;

		[HideInInspector]
		public int matStairs;

		public List<Cluster.ItemThing> lights;

		public string _matDoor
		{
			get
			{
				return _GetMat(matDoor);
			}
			set
			{
				_SetMat(ref matDoor, value);
			}
		}

		public string _matStairs
		{
			get
			{
				return _GetMat(matStairs);
			}
			set
			{
				_SetMat(ref matStairs, value);
			}
		}

		public string GetIdLight(bool wall)
		{
			if (lights.Count > 0)
			{
				foreach (Cluster.ItemThing light in lights)
				{
					if (EClass.sources.cards.map[light.id].tileType.UseMountHeight == wall)
					{
						return light.id;
					}
				}
			}
			if (!wall)
			{
				return "torch";
			}
			return "torch_wall";
		}

		public string GetIdStairs(bool upstairs)
		{
			switch (stairsStyle)
			{
			case StairsStyle.Wood:
				if (!upstairs)
				{
					return "381";
				}
				return "376";
			case StairsStyle.Stone:
				if (!upstairs)
				{
					return "932";
				}
				return "379";
			case StairsStyle.Rock:
				if (!upstairs)
				{
					return "380";
				}
				return "377";
			default:
				if (!upstairs)
				{
					return "stairsDown_cave";
				}
				return "stairs_cave";
			}
		}

		public string GetIdDoor()
		{
			return doorStyle switch
			{
				DoorStyle.FirmWood => "46", 
				DoorStyle.Stone => "42", 
				DoorStyle.Jail => "40", 
				DoorStyle.Rune => "43", 
				_ => "45", 
			};
		}
	}

	[Serializable]
	public class Spawns
	{
		public List<SpawnListChara> chara;

		public List<SpawnListThing> thing;

		public string GetRandomCharaId()
		{
			if (chara.Count != 0)
			{
				return chara.RandomItemWeighted((SpawnListChara a) => a.chance).id;
			}
			return "c_dungeon";
		}

		public string GetRandomThingId()
		{
			if (thing.Count != 0)
			{
				return thing.RandomItemWeighted((SpawnListThing a) => a.chance).id;
			}
			return "dungeon";
		}
	}

	[Serializable]
	public class SpawnList : BaseTile
	{
		public float chance = 1f;

		[HideInInspector]
		public string id;

		public string _id
		{
			get
			{
				return _GetSpawnList(id);
			}
			set
			{
				_SetSpawnList(ref id, value);
			}
		}
	}

	[Serializable]
	public class SpawnListChara : SpawnList
	{
	}

	[Serializable]
	public class SpawnListThing : SpawnList
	{
	}

	[Serializable]
	public class Clusters
	{
		public List<ClusterObj> obj;

		public List<ClusterThing> thing;
	}

	[Serializable]
	public class Cluster
	{
		public enum Type
		{
			Scatter = 0,
			ScatterExterior = 1,
			ScatterInterior = 2,
			ScatterNonObstacle = 3,
			ByWall = 10
		}

		[Serializable]
		public class BaseItem : BaseTile
		{
			public float chance = 1f;

			public virtual bool IsSpawnOnBlock => false;

			public virtual bool IsSpawnOnWater => false;
		}

		[Serializable]
		public class Item : BaseItem
		{
			public int idObj;

			public override bool IsSpawnOnBlock => EClass.sources.objs.rows[idObj].tileType.IsBlockMount;

			public override bool IsSpawnOnWater => EClass.sources.objs.rows[idObj].tileType.CanSpawnOnWater;

			public string obj
			{
				get
				{
					return _GetObj(idObj);
				}
				set
				{
					_SetObj(ref idObj, value);
				}
			}
		}

		[Serializable]
		public class ItemThing : BaseItem
		{
			public string id;

			public string _id
			{
				get
				{
					return _GetThing(id);
				}
				set
				{
					_SetThing(ref id, value);
				}
			}
		}

		public Type type;

		public float density;

		public virtual bool TryCreate(Point p)
		{
			return false;
		}
	}

	[Serializable]
	public class ClusterObj : Cluster
	{
		public List<Item> items;

		public override bool TryCreate(Point p)
		{
			Item item = items.RandomItem();
			if (Rand.Range(0f, 1f) > item.chance)
			{
				return false;
			}
			Cell cell = p.cell;
			if (cell.HasBlock)
			{
				if (!item.IsSpawnOnBlock)
				{
					return false;
				}
			}
			else if (item.IsSpawnOnBlock)
			{
				return false;
			}
			p.SetObj(item.idObj);
			cell.objDir = EScriptable.rnd(8);
			if (cell.sourceObj.HasGrowth && !EClass._zone.IsPCFactionOrTent && cell.sourceObj.id != 103)
			{
				cell.growth.SetRandomStage();
			}
			return true;
		}
	}

	[Serializable]
	public class ClusterThing : Cluster
	{
		public List<ItemThing> items;

		public override bool TryCreate(Point p)
		{
			ItemThing itemThing = items.RandomItem();
			if (Rand.Range(0f, 1f) > itemThing.chance)
			{
				return false;
			}
			Cell cell = p.cell;
			if (cell.HasBlock)
			{
				if (!itemThing.IsSpawnOnBlock)
				{
					return false;
				}
			}
			else if (itemThing.IsSpawnOnBlock)
			{
				return false;
			}
			if (cell.IsTopWater)
			{
				if (!itemThing.IsSpawnOnWater)
				{
					return false;
				}
			}
			else if (itemThing.IsSpawnOnWater)
			{
				return false;
			}
			Thing thing = ThingGen.Create(itemThing.id);
			int desiredDir = thing.TileType.GetDesiredDir(p, 0);
			if (desiredDir != -1)
			{
				thing.dir = desiredDir;
			}
			EClass._zone.AddCard(thing, p).Install();
			return true;
		}
	}

	public static bool forceInitialize;

	public NoiseLayer layerBlock;

	public BiomeID id;

	public Color color;

	public FowProfile fowProfile;

	public TileGroup _exterior;

	public TileGroup _interior;

	public Style style;

	public Spawns spawn;

	public Clusters cluster;

	public string tags;

	public int floor_height;

	[NonSerialized]
	private SourceMaterial.Row _mat;

	[NonSerialized]
	private SourceMaterial.Row _matSub;

	public TileGroup exterior => _exterior;

	public TileGroup interior
	{
		get
		{
			if (_interior.floor.id != 0)
			{
				return _interior;
			}
			return exterior;
		}
	}

	public SourceMaterial.Row MatFloor => _mat ?? (_mat = EClass.sources.materials.map[exterior.floor.mat]);

	public SourceMaterial.Row MatSub => _matSub ?? (_matSub = ((exterior.floor.matSub == 0) ? MatFloor : EClass.sources.materials.map[exterior.floor.matSub]));

	public void Populate(Point point, bool interior = false, float mtpDensity = 1f)
	{
		Cell cell = point.cell;
		if (cell.obj != 0 || cell.Things.Count > 0)
		{
			return;
		}
		int count = this.cluster.obj.Count;
		for (int i = 0; i < count + this.cluster.thing.Count; i++)
		{
			Cluster cluster = ((i >= count) ? ((Cluster)this.cluster.thing[i - count]) : ((Cluster)this.cluster.obj[i]));
			if (cluster.density == 0f)
			{
				break;
			}
			if (Rand.Range(0f, 1f) > cluster.density * mtpDensity)
			{
				continue;
			}
			switch (cluster.type)
			{
			case Cluster.Type.ScatterExterior:
				if (interior)
				{
					continue;
				}
				break;
			case Cluster.Type.ScatterInterior:
				if (!interior)
				{
					continue;
				}
				break;
			case Cluster.Type.ScatterNonObstacle:
				if ((cell.Left.HasObstacle() ? 1 : 0) + (cell.Right.HasObstacle() ? 1 : 0) + (cell.Front.HasObstacle() ? 1 : 0) + (cell.Back.HasObstacle() ? 1 : 0) > 0)
				{
					continue;
				}
				break;
			case Cluster.Type.ByWall:
				if (cell.Left.hasDoor || cell.Right.hasDoor || cell.Front.hasDoor || cell.Back.hasDoor || (!cell.Left.HasBlock && !cell.Right.HasBlock && !cell.Front.HasBlock && !cell.Back.HasBlock))
				{
					continue;
				}
				break;
			}
			if (cluster.TryCreate(point))
			{
				break;
			}
		}
	}

	public static void Init()
	{
		if (!forceInitialize)
		{
			return;
		}
		Debug.Log("Initializing Clusters");
		foreach (BiomeProfile value in EClass.core.refs.biomes.dict.Values)
		{
			value.Reset();
		}
		forceInitialize = false;
	}

	public void Reset()
	{
		_mat = null;
		_matSub = null;
	}
}
