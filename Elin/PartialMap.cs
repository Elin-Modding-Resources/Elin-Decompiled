using System.Collections.Generic;
using System.IO;
using Ionic.Zip;
using Newtonsoft.Json;
using SFB;
using UnityEngine;

public class PartialMap : EClass
{
	public enum ApplyMode
	{
		Render,
		HitTest,
		Apply,
		Evaluate
	}

	public class Result
	{
		public bool hasLight;

		public List<int> ruined = new List<int>();
	}

	public static bool relative;

	[JsonProperty]
	public string name;

	[JsonProperty]
	public int offsetX;

	[JsonProperty]
	public int offsetZ;

	[JsonProperty]
	public int w;

	[JsonProperty]
	public int h;

	[JsonProperty]
	public int dir;

	[JsonProperty]
	public int baseHeight;

	[JsonProperty]
	public int baseHeightWater;

	[JsonProperty]
	public int value;

	[JsonProperty]
	public bool allowRotate;

	[JsonProperty]
	public bool ignoreBlock;

	public int localOffsetX;

	public int localOffsetZ;

	public int destX;

	public int destZ;

	public string path;

	public bool editMode;

	public bool procedural;

	public float ruinChance;

	public Map map;

	public ZoneExportData exportData;

	public List<Cell> cellsMarked = new List<Cell>();

	public HashSet<int> validPoints = new HashSet<int>();

	public Result result = new Result();

	public static string PathTemp => CorePath.MapPieceSave + "_temp.mp";

	public bool altDirPosition => true;

	public string ID => new FileInfo(path).Name.Replace(".mp", "");

	public void Rotate()
	{
		dir++;
		if (dir > 3)
		{
			dir = 0;
		}
		SE.Rotate();
		localOffsetX = (localOffsetZ = 0);
	}

	public void ClearMarkedCells()
	{
		foreach (Cell item in cellsMarked)
		{
			item.skipRender = false;
		}
		cellsMarked.Clear();
	}

	public void Apply(Point _point, ApplyMode mode)
	{
		byte height = _point.cell.height;
		int num = (_point.cell.IsFloorWater ? baseHeightWater : baseHeight);
		Point point = new Point(_point);
		point.x += localOffsetX;
		point.z += localOffsetZ;
		if (altDirPosition)
		{
			switch (dir)
			{
			case 1:
				point.z += w - 1;
				break;
			case 2:
				point.x += w - 1;
				point.z += h - 1;
				break;
			case 3:
				point.x += h - 1;
				break;
			}
		}
		destX = point.x;
		destZ = point.z;
		Point p = new Point();
		if (mode == ApplyMode.Render)
		{
			ClearMarkedCells();
		}
		if (mode == ApplyMode.Evaluate)
		{
			value = 0;
		}
		int num2 = 0;
		int num3 = 0;
		int minX = point.x;
		int minZ = point.z;
		int maxX = point.x + w;
		int maxZ = point.z + h;
		switch (dir)
		{
		case 1:
			minX = point.x;
			maxX = point.x + h;
			minZ = point.z - w + 1;
			maxZ = point.z + 1;
			break;
		case 2:
			minX = point.x - w + 1;
			maxX = point.x + 1;
			minZ = point.z - h + 1;
			maxZ = point.z + 1;
			break;
		case 3:
			minX = point.x - h + 1;
			maxX = point.x + 1;
			minZ = point.z;
			maxZ = point.z + w;
			break;
		}
		EClass.pc.GetCurrency();
		BiomeProfile.TileGroup interior = EClass._zone.biome.interior;
		BiomeProfile.TileGroup exterior = EClass._zone.biome.exterior;
		this.result.ruined.Clear();
		Cell c1;
		HitResult result;
		for (int i = 0; i < w; i++)
		{
			for (int j = 0; j < h; j++)
			{
				Cell cell = map.cells[i, j];
				p.Set(point.x + num2, point.z + num3);
				if (EClass._map.Contains(p))
				{
					c1 = EClass._map.cells[p.x, p.z];
					result = HitResult.Valid;
					if (c1.isFloating)
					{
						result = HitResult.Invalid;
					}
					if (result == HitResult.Valid)
					{
						validPoints.Add(p.index);
					}
					if (mode == ApplyMode.Render)
					{
						c1.skipRender = true;
						cellsMarked.Add(c1);
						if (result != HitResult.Valid)
						{
							MeshPass guidePass = ActionMode.Copy.GetGuidePass(point);
							int num4 = (int)result;
							Vector3 vector = p.Position();
							guidePass.Add(vector.x, vector.y, vector.z - 0.01f, num4, 0.3f);
						}
						else
						{
							Render(cell.sourceFloor, cell.floorDir, 0);
							Render(cell.sourceBlock, cell.HasWall ? FixWall(cell) : cell.blockDir, 0);
							Render(cell.sourceObj, cell.objDir, 0);
							Render(cell.sourceBridge, cell.floorDir, (cell.bridgeHeight != 0) ? (cell.bridgeHeight - cell.height) : 0);
							foreach (SerializedCards.Data card in exportData.serializedCards.cards)
							{
								Recipe orCreate = Recipe.GetOrCreate(EClass.sources.cards.map[card.id].RecipeID);
								if (card.x - offsetX == i && card.z - offsetZ == j)
								{
									if (relative)
									{
										orCreate?.OnRenderMarker(p, active: true, result, main: true, card.dir, height + cell.height - num);
									}
									else
									{
										orCreate?.OnRenderMarker(p, active: true, result, main: true, card.dir, cell.height);
									}
								}
							}
							Critter.RebuildCritter(c1);
						}
					}
					if (mode == ApplyMode.Apply && result == HitResult.Valid)
					{
						if (Rand.Range(0f, 1f) < ruinChance)
						{
							this.result.ruined.Add(p.index);
						}
						else if (cell._floor != 77 || editMode)
						{
							if (cell._block == 149 && !editMode)
							{
								c1._block = interior.block.id;
								c1._blockMat = interior.block.mat;
							}
							else if (cell._block == 150 && !editMode)
							{
								c1._block = exterior.block.id;
								c1._blockMat = exterior.block.mat;
							}
							else
							{
								c1._block = cell._block;
								c1._blockMat = cell._blockMat;
							}
							if (cell._floor == 2 && !editMode)
							{
								c1._floor = interior.floor.id;
								c1._floorMat = interior.floor.mat;
							}
							else if (cell._floor == 3 && !editMode)
							{
								c1._floor = exterior.floor.id;
								c1._floorMat = exterior.floor.mat;
							}
							else
							{
								c1._floor = cell._floor;
								c1._floorMat = cell._floorMat;
							}
							c1._roofBlock = cell._roofBlock;
							c1._roofBlockMat = cell._roofBlockMat;
							c1.obj = cell.obj;
							c1.objMat = cell.objMat;
							c1.objVal = cell.objVal;
							c1.decal = cell.decal;
							c1._dirs = cell._dirs;
							c1._bridge = cell._bridge;
							c1._bridgeMat = cell._bridgeMat;
							c1.bridgePillar = cell.bridgePillar;
							c1.hidePillar = cell.hidePillar;
							c1.toggleOcclusion = cell.toggleOcclusion;
							c1.isModified = true;
							if (c1.HasWall)
							{
								c1.blockDir = FixWall(c1);
							}
							if (relative || procedural)
							{
								c1.height = (byte)Mathf.Clamp(height + cell.height - num, 0, 255);
								c1.bridgeHeight = (byte)Mathf.Clamp((cell.bridgeHeight != 0) ? (c1.height + (cell.bridgeHeight - cell.height)) : 0, 0, 255);
							}
							else
							{
								c1.height = cell.height;
								c1.bridgeHeight = cell.bridgeHeight;
							}
						}
					}
				}
				switch (dir)
				{
				case 0:
					num3++;
					if (num3 >= h)
					{
						num3 = 0;
						num2++;
					}
					break;
				case 1:
					num2++;
					if (num2 >= h)
					{
						num2 = 0;
						num3--;
					}
					break;
				case 2:
					num3--;
					if (num3 <= -h)
					{
						num3 = 0;
						num2--;
					}
					break;
				case 3:
					num2--;
					if (num2 <= -h)
					{
						num2 = 0;
						num3++;
					}
					break;
				}
			}
		}
		if (mode != ApplyMode.Apply)
		{
			return;
		}
		p.Set(point.x, point.z);
		EClass._map.SetReference();
		EClass._map.things.ForeachReverse(delegate(Thing t)
		{
			if (validPoints.Contains(t.pos.index) && t.pos.x >= minX && t.pos.z >= minZ && t.pos.x < maxX && t.pos.z < maxZ && t.trait.CanBeDestroyed)
			{
				t.Destroy();
			}
		});
		if (exportData != null)
		{
			exportData.serializedCards.Restore(EClass._map, null, addToZone: true, this);
		}
		EClass._map.RefreshAllTiles();
		EClass._map.RefreshAllTiles();
		void Render(TileRow row, int dir, int bridgeHeight)
		{
			if (row.id != 0)
			{
				Recipe orCreate2 = Recipe.GetOrCreate(row.RecipeID);
				if (relative)
				{
					orCreate2?.OnRenderMarker(p, active: true, result, main: true, dir, c1.height + bridgeHeight);
				}
				else
				{
					orCreate2?.OnRenderMarker(p, active: true, result, main: true, dir, c1.height + bridgeHeight);
				}
			}
		}
	}

	public int FixWall(Cell c)
	{
		switch (dir)
		{
		case 1:
			if (c.blockDir == 0)
			{
				return 1;
			}
			if (c.blockDir == 1)
			{
				return 0;
			}
			break;
		case 3:
			if (c.blockDir == 0)
			{
				return 1;
			}
			if (c.blockDir == 1)
			{
				return 0;
			}
			if (c.blockDir == 2)
			{
				return 0;
			}
			break;
		}
		return c.blockDir;
	}

	public void Update()
	{
		if (!path.IsEmpty())
		{
			Map.UpdateMetaData(path, this);
		}
	}

	public void Save(int _x, int _z, int _w, int _h)
	{
		offsetX = _x;
		offsetZ = _z;
		w = _w;
		h = _h;
		value = 0;
		for (int i = _x; i < _x + w; i++)
		{
			for (int j = _z; j < _z + h; j++)
			{
				Cell cell = EClass._map.cells[i, j];
				value += Card.GetTilePrice(cell.sourceBlock, cell.matBlock);
				value += Card.GetTilePrice(cell.sourceFloor, cell.matFloor);
				value += Card.GetTilePrice(cell.sourceObj, cell.matObj);
				value += Card.GetTilePrice(cell.sourceBridge, cell.matBridge);
				value += Card.GetTilePrice(cell.sourceRoofBlock, cell.matRoofBlock);
			}
		}
		foreach (Thing thing in EClass._map.things)
		{
			if (thing.trait.CanCopyInBlueprint && thing.pos.Within(_x, _z, _w, _h) && thing.IsInstalled)
			{
				value += thing.GetPrice() * thing.Num;
			}
		}
		EClass._zone.Export(PathTemp, this);
	}

	public static PartialMap Apply(string path, Point pos)
	{
		PartialMap partialMap = Load(CorePath.MapPieceSave + path);
		partialMap.ruinChance = 0f;
		partialMap.Apply(pos, ApplyMode.Apply);
		return partialMap;
	}

	public static PartialMap Load(string path = null)
	{
		bool flag = path.IsEmpty();
		if (flag)
		{
			path = PathTemp;
		}
		if (!File.Exists(path))
		{
			return null;
		}
		MapMetaData metaData = Map.GetMetaData(path);
		if (metaData == null || metaData.partial == null)
		{
			return null;
		}
		PartialMap partial = metaData.partial;
		if (!flag)
		{
			partial.path = path;
		}
		partial.exportData = EClass._zone.Import(path);
		partial._Load();
		return partial;
	}

	public void _Load()
	{
		map = GameIO.LoadFile<Map>(EClass._zone.pathTemp + "map");
		map.Load(EClass._zone.pathTemp, import: false, this);
		if (map == null)
		{
			Debug.Log("Map is null:" + name + "/" + PathTemp);
			return;
		}
		EClass._map.SetReference();
		baseHeight = 255;
		baseHeightWater = 255;
		for (int i = 0; i < w; i++)
		{
			for (int j = 0; j < h; j++)
			{
				Cell cell = map.cells[i, j];
				if (cell.height < baseHeight)
				{
					baseHeightWater = cell.height;
				}
				if (cell.height < baseHeight && !cell.IsFloorWater)
				{
					baseHeight = cell.height;
				}
			}
		}
		if (baseHeight == 255)
		{
			baseHeight = 0;
		}
	}

	public static void Delete(string path)
	{
		string fullFileNameWithoutExtension = path.GetFullFileNameWithoutExtension();
		IO.DeleteFile(path);
		IO.DeleteFile(fullFileNameWithoutExtension + ".jpg");
		IO.DeleteFile(fullFileNameWithoutExtension + ".txt");
	}

	public static void ExportDialog(string dir = null)
	{
		EClass.core.WaitForEndOfFrame(delegate
		{
			string pathDest = StandaloneFileBrowser.SaveFilePanel("Export Map Piece", dir ?? CorePath.MapPieceSaveUser, "new map piece", "mp");
			if (!string.IsNullOrEmpty(pathDest))
			{
				SavePreview(PathTemp, pathDest);
				Msg.SayRaw("Exported Zone");
			}
		});
	}

	public static void SavePreview(string path, string pathDest)
	{
		EClass.core.actionsNextFrame.Add(delegate
		{
			string fileName = CorePath.Temp + "preview.jpg";
			Texture2D texture2D = ScreenCapture.CaptureScreenshotAsTexture();
			int num = 200;
			int num2 = 100;
			RenderTexture renderTexture = new RenderTexture(num, num2, 0);
			renderTexture.Create();
			RenderTexture active = RenderTexture.active;
			RenderTexture.active = renderTexture;
			Graphics.Blit(texture2D, renderTexture);
			Texture2D texture2D2 = new Texture2D(num, num2, texture2D.format, mipChain: false);
			texture2D2.ReadPixels(new Rect(0f, 0f, num, num2), 0, 0);
			texture2D2.Apply();
			RenderTexture.active = active;
			renderTexture.Release();
			File.WriteAllBytes(fileName, texture2D2.EncodeToJPG());
			ZipFile zipFile = ZipFile.Read(path);
			zipFile.ExtractExistingFile = ExtractExistingFileAction.OverwriteSilently;
			zipFile.AddFile(fileName, "");
			zipFile.Save(pathDest);
			zipFile.Dispose();
			IO.DeleteFile(fileName);
			Object.Destroy(texture2D2);
			Object.Destroy(texture2D);
			Object.Destroy(renderTexture);
		});
	}
}
