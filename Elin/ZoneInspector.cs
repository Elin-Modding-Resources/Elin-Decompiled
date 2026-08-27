using UnityEngine;

public class ZoneInspector : EMono
{
	public class FillUtil
	{
		public int flatHeight;

		public int fillFloor = 5;

		public int fillBlock = 1;

		public bool randomFillDir;

		public void FlattenHeight()
		{
			EMono._map.ForeachCell(delegate(Cell c)
			{
				c.height = (byte)flatHeight;
			});
			EMono._map.RefreshAllTiles();
		}

		public void FillFloor()
		{
			EMono._map.ForeachCell(delegate(Cell c)
			{
				int dir = (randomFillDir ? EMono.rnd(4) : 0);
				EMono._map.SetFloor(c.x, c.z, EMono.sources.floors[fillFloor].DefaultMaterial.id, fillFloor, dir);
			});
		}

		public void FillBlock()
		{
			EMono._map.ForeachCell(delegate(Cell c)
			{
				int dir = (randomFillDir ? EMono.rnd(4) : 0);
				EMono._map.SetBlock(c.x, c.z, EMono.sources.blocks[fillBlock].DefaultMaterial.id, fillBlock, dir);
			});
		}

		public void ClearBlock()
		{
			EMono._map.ForeachCell(delegate(Cell c)
			{
				EMono._map.SetBlock(c.x, c.z);
			});
		}

		public void ClearObj()
		{
			EMono._map.ForeachCell(delegate(Cell c)
			{
				EMono._map.SetObj(c.x, c.z);
			});
		}

		public void ClearBridge()
		{
			EMono._map.ForeachCell(delegate(Cell c)
			{
				EMono._map.SetObj(c.x, c.z);
			});
		}
	}

	public class ResizeUtil
	{
		public int newSize;

		public Vector2Int offset;

		public void Apply()
		{
			if (EMono._map.Size != newSize)
			{
				EMono._map.Resize(newSize);
			}
			if (offset.x != 0 || offset.y != 0)
			{
				EMono._map.Shift(offset);
			}
		}
	}

	public static ZoneInspector Instance;

	public ResizeUtil resize = new ResizeUtil();

	public FillUtil fill = new FillUtil();

	public Zone zone;

	public string idExport => zone?.idExport;

	public MapConfig mapConfig
	{
		get
		{
			return zone?.map.config;
		}
		set
		{
			zone.map.config = value;
		}
	}

	public MapBounds bounds
	{
		get
		{
			return zone?.map.bounds;
		}
		set
		{
			zone.map.bounds = value;
		}
	}

	private void Awake()
	{
		Instance = this;
	}

	public void RefreshAll()
	{
		EMono._map.RefreshAllTiles();
	}

	public void RefreshScreen()
	{
		if (EMono.core.IsGameStarted)
		{
			EMono.scene.profile = SceneProfile.Load(EMono._map.config.idSceneProfile.IsEmpty("default"));
			EMono.screen.Deactivate();
			if (EMono.player.zone is Region)
			{
				ActionMode.Region.Activate();
			}
			else
			{
				ActionMode.Adv.Activate();
			}
			EMono.screen.SetZoom(0.5f);
			EMono.screen.RefreshScreenSize();
		}
	}

	public void SetAllPlayerCreation()
	{
		foreach (Thing thing in EMono._map.things)
		{
			thing.isPlayerCreation = true;
		}
		foreach (Chara chara in EMono._map.charas)
		{
			chara.isPlayerCreation = true;
		}
	}

	public void Export()
	{
		EMono._zone.Export();
	}

	private void OnValidate()
	{
		if (Application.isPlaying)
		{
			RefreshScreen();
		}
	}
}
