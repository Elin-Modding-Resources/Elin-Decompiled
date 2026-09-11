using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class TileManager : EClass
{
	private struct TileBounds
	{
		public int left;

		public int width;

		public int up;

		public int down;
	}

	private class Grid
	{
		public bool[] used;

		public int cols;

		public int rows;

		public void SetUsedSequence(int linear, int span)
		{
			for (int i = 0; i < span; i++)
			{
				int num = linear + i;
				if (num >= 0 && num < used.Length)
				{
					used[num] = true;
				}
			}
		}

		public void AddRows(int n)
		{
			Array.Resize(ref used, cols * (rows + n));
			rows += n;
		}
	}

	private class Entry
	{
		public string source;

		public string alias;

		public TileRow row;

		public FileInfo file;

		public FileInfo snowFile;

		public int[] tiles;

		public string[] growth;
	}

	private static readonly (string source, Func<IEnumerable<TileRow>> rows)[] _sourceData = new(string, Func<IEnumerable<TileRow>>)[5]
	{
		("Block", () => EClass.sources.blocks.rows),
		("Floor", () => EClass.sources.floors.rows),
		("Obj", () => EClass.sources.objs.rows),
		("Deco", () => EClass.sources.decos.rows),
		("CellEffect", () => EClass.sources.cellEffects.rows)
	};

	private static readonly List<Entry> _allocated = new List<Entry>();

	private static bool _applied;

	private const int GrowChunkRows = 8;

	private static readonly Dictionary<TextureData, (int w, int h)> _grownTex = new Dictionary<TextureData, (int, int)>();

	private static readonly Dictionary<ProceduralMesh, Vector2> _grownMesh = new Dictionary<ProceduralMesh, Vector2>();

	private static readonly Dictionary<Material, Vector4> _grownMat = new Dictionary<Material, Vector4>();

	private static readonly int _tiling = Shader.PropertyToID("_Tiling");

	public static void Apply()
	{
		if (!_applied)
		{
			_applied = true;
			AllocateTiles();
		}
	}

	private static void AllocateTiles()
	{
		Dictionary<MeshPass, TextureData> dictionary = BuildPassData();
		Dictionary<TextureData, TextureData> dictionary2 = new Dictionary<TextureData, TextureData>();
		foreach (TextureData value2 in EClass.core.textures.texMap.Values)
		{
			foreach (MeshPass item in value2.listPass)
			{
				if ((bool)item.snowPass && dictionary.TryGetValue(item.snowPass, out var value) && value != value2)
				{
					dictionary2[value2] = value;
					break;
				}
			}
		}
		Dictionary<TextureData, Grid> grids = new Dictionary<TextureData, Grid>();
		(string, Func<IEnumerable<TileRow>>)[] sourceData = _sourceData;
		for (int i = 0; i < sourceData.Length; i++)
		{
			var (text, func) = sourceData[i];
			foreach (Entry entry in ParseTiles(text, func()))
			{
				if (_allocated.Any((Entry e) => e.source == entry.source && string.Equals(e.alias, entry.alias, StringComparison.OrdinalIgnoreCase)))
				{
					continue;
				}
				int[] tiles = entry.row.tiles;
				if (tiles != null && tiles.Length != 0 && ModUtil.FindSourceRowPackage(entry.row) == null)
				{
					Debug.LogWarning("#tile " + text + "/" + entry.alias + " is a base game row; to reskin it put the sprite in 'Texture Replace' as <texture_sheet>_<cell>.png instead of " + entry.file.FullName.ShortPath() + "; or use in game Texture Viewer");
				}
				else
				{
					try
					{
						Allocate(entry, dictionary, dictionary2, grids);
					}
					catch (Exception ex)
					{
						Debug.LogError($"#tile failed to allocate {text}/{entry.alias} ({entry.file.FullName.ShortPath()})\n{ex}");
					}
				}
			}
		}
		sourceData = _sourceData;
		for (int i = 0; i < sourceData.Length; i++)
		{
			(string, Func<IEnumerable<TileRow>>) tuple2 = sourceData[i];
			var (text2, _) = tuple2;
			foreach (TileRow row in tuple2.Item2())
			{
				int[] tiles2 = row.tiles;
				if (tiles2 != null && tiles2.Length == 0 && _allocated.All((Entry e) => e.row != row))
				{
					Debug.LogWarning("#tile " + text2 + "/" + row.alias + " has empty tiles and no Texture/" + text2 + "/" + row.alias + ".png");
				}
			}
		}
	}

	public static void ReapplyRows()
	{
		foreach (Entry entry in _allocated)
		{
			TileRow tileRow = _sourceData.First(((string source, Func<IEnumerable<TileRow>> rows) t) => t.source == entry.source).rows().FirstOrDefault((TileRow r) => string.Equals(r.alias, entry.alias, StringComparison.OrdinalIgnoreCase));
			if (tileRow != null)
			{
				if (entry.growth != null && tileRow is SourceObj.Row { growth: not null } row)
				{
					InitGrowth(row, entry.growth);
				}
				ApplyRow(tileRow, entry.tiles);
			}
		}
		if (_applied)
		{
			AllocateTiles();
		}
	}

	private static List<Entry> ParseTiles(string source, IEnumerable<TileRow> rows)
	{
		string text = source + "/";
		Dictionary<string, FileInfo> dictionary = new Dictionary<string, FileInfo>(StringComparer.OrdinalIgnoreCase);
		string key2;
		foreach (KeyValuePair<string, string> dictModItem in SpriteReplacer.dictModItems)
		{
			dictModItem.Deconstruct(out var key, out key2);
			string text2 = key;
			string text3 = key2;
			if (text2.StartsWith(text, StringComparison.OrdinalIgnoreCase))
			{
				string text4 = text2[text.Length..];
				if (text4.IndexOf('/') < 0)
				{
					dictionary[text4] = new FileInfo(text3 + ".png");
				}
			}
		}
		List<TileRow> source2 = rows.ToList();
		Dictionary<string, Entry> dictionary2 = new Dictionary<string, Entry>(StringComparer.OrdinalIgnoreCase);
		FileInfo value;
		foreach (KeyValuePair<string, FileInfo> item in dictionary)
		{
			item.Deconstruct(out key2, out value);
			string name = key2;
			FileInfo file = value;
			TileRow tileRow = source2.FirstOrDefault((TileRow r) => string.Equals(r.alias, name, StringComparison.OrdinalIgnoreCase));
			if (tileRow != null)
			{
				dictionary2[name] = new Entry
				{
					source = source,
					alias = name,
					row = tileRow,
					file = file
				};
			}
		}
		foreach (KeyValuePair<string, FileInfo> item2 in dictionary)
		{
			item2.Deconstruct(out key2, out value);
			string text5 = key2;
			FileInfo fileInfo = value;
			if (!dictionary2.ContainsKey(text5))
			{
				if (text5.EndsWith("_snow", StringComparison.OrdinalIgnoreCase) && dictionary2.TryGetValue(text5[..^5], out var value2))
				{
					value2.snowFile = fileInfo;
					continue;
				}
				Debug.LogWarning("#tile no " + source + " row with alias '" + text5 + "' for " + fileInfo.FullName.ShortPath());
			}
		}
		return dictionary2.Values.ToList();
	}

	private static TileBounds GetTileBounds(RenderRow row, int frames = 1)
	{
		TileBounds result = new TileBounds
		{
			width = Math.Max(1, frames)
		};
		int[] anime = GetAnime(row);
		if (anime != null && anime.Length != 0)
		{
			result.width = Math.Max(result.width, anime[0]);
		}
		if (!(row is SourceCellEffect.Row))
		{
			if (row is SourceObj.Row row2)
			{
				if (row2.autoTile)
				{
					result.width = Math.Max(result.width, 16);
				}
				if (row2.growth is GrowSystemWeed)
				{
					result.left = 2;
					result.width = Math.Max(Math.Max(1, frames - 2), 3);
				}
				if (row2.idRoof != 0)
				{
					result.width = Math.Max(result.width, 12);
				}
			}
		}
		else
		{
			result.width = Math.Max(result.width, 8);
		}
		if ((bool)row.renderData && row.renderData.multiSize)
		{
			result.up = 1;
		}
		if (row is TileRow tileRow && row.tileType != null)
		{
			result.down = tileRow.tileType.blockRenderMode switch
			{
				BlockRenderMode.WallOrFence => 3, 
				BlockRenderMode.Pillar => Math.Max(result.down, 1), 
				_ => result.down, 
			};
		}
		return result;
	}

	private static void Allocate(Entry entry, Dictionary<MeshPass, TextureData> passData, Dictionary<TextureData, TextureData> snowOf, Dictionary<TextureData, Grid> grids)
	{
		TileRow row = entry.row;
		string text = entry.source + "/" + entry.alias + " (" + entry.file.FullName.ShortPath() + ")";
		RenderData renderData = row.renderData;
		if (!renderData || !renderData.pass || !passData.TryGetValue(renderData.pass, out var value))
		{
			Debug.LogWarning("#tile " + text + " has invalid render data");
			return;
		}
		if (!TryMeasure(entry.file, value, out var frames, out var rows))
		{
			Debug.LogWarning($"#tile {text} sprite must be n*{value.tileW} x m*{value.tileH} (n < 100, m <= 4)");
			return;
		}
		TextureData textureData = snowOf.TryGetValue(value);
		if (entry.snowFile != null)
		{
			int frames2;
			int rows2;
			if (textureData == null)
			{
				Debug.LogWarning("#tile " + text + " ignore snow");
				entry.snowFile = null;
			}
			else if (!TryMeasure(entry.snowFile, value, out frames2, out rows2) || frames2 != frames || rows2 != rows)
			{
				Debug.LogWarning("#tile " + text + " base and snow sprites size mismatch");
				entry.snowFile = null;
			}
		}
		TileBounds tileBounds = GetTileBounds(row, frames);
		if (tileBounds.width > frames)
		{
			Debug.LogWarning($"#tile {text} uses {tileBounds.width} cells but sprite has {frames}");
		}
		if (tileBounds.up > 0 && rows < 1 + tileBounds.up)
		{
			Debug.LogWarning($"#tile {text} multiSize needs {1 + tileBounds.up} cells tall");
		}
		if (tileBounds.down > 0 && rows < 2)
		{
			Debug.LogWarning("#tile " + text + " wall/fence should include the pillar rows below the base row");
		}
		if (row is SourceObj.Row row2 && row2.growth is GrowSystemWeed && frames != 5)
		{
			Debug.LogWarning($"#tile {text} crop is 5 frames (stages 0..4, the 3rd is the base tile), got {frames}");
		}
		if (!grids.TryGetValue(value, out var value2))
		{
			value2 = (grids[value] = BuildGrid(value, textureData, passData));
		}
		int num = Math.Max(1 + tileBounds.up + tileBounds.down, rows);
		int num2 = tileBounds.left + tileBounds.width;
		int num3 = AllocateRect(value2, num2, num, tileBounds.up, tileBounds.left);
		if (num3 < 0 && num2 <= value2.cols && TryAddRows(value, textureData, value2, num, text))
		{
			num3 = AllocateRect(value2, num2, num, tileBounds.up, tileBounds.left);
		}
		if (num3 < 0)
		{
			Debug.LogError($"#tile no free {num2}x{num} region left in '{value.id}' for {text}");
			return;
		}
		int index = num3 - tileBounds.left - Math.Min(tileBounds.up, rows - 1) * 100;
		Blit(value, entry.file, index);
		if (entry.snowFile != null)
		{
			Blit(textureData, entry.snowFile, index);
		}
		entry.tiles = GetTiles(entry, row, num3, frames);
		ApplyRow(row, entry.tiles);
		_allocated.Add(entry);
		Debug.Log($"#tile {entry.source}/{entry.alias} -> {value.id}#{num3} x{frames}");
	}

	private static int[] GetTiles(Entry entry, RenderRow row, int baseIndex, int frames)
	{
		int[] anime = GetAnime(row);
		if (anime != null && anime.Length != 0)
		{
			return new int[1] { baseIndex };
		}
		if (row is SourceObj.Row row2)
		{
			if (row2.autoTile)
			{
				return new int[1] { baseIndex };
			}
			GrowSystem growth = row2.growth;
			if (growth != null && !(growth is GrowSystemDeco) && !(growth is GrowSystemTreeCoralwood))
			{
				if (!(growth is GrowSystemWeed))
				{
					return new int[1] { MakeGrowthTile(entry, row2, baseIndex, frames) };
				}
				return new int[1] { baseIndex };
			}
		}
		return Enumerable.Range(baseIndex, frames).ToArray();
	}

	private static int MakeGrowthTile(Entry entry, SourceObj.Row obj, int baseIndex, int frames)
	{
		string tag = entry.source + "/" + entry.alias;
		string[] array = (string[])obj._growth.Clone();
		if (array.Length > 1)
		{
			array[1] = string.Join("/", from v in array[1].Split('/')
				select Cell(v, "stage").ToString());
		}
		int num = ((array.Length > 2 && array[2].ToInt() > 0) ? Cell(array[2], "harvest") : 0);
		if (array.Length > 2)
		{
			array[2] = num.ToString();
		}
		entry.growth = array;
		InitGrowth(obj, array);
		int num2 = obj.renderData.ConvertTile(baseIndex);
		int num3 = obj.growth.stages.Where((GrowSystem.Stage st) => st.renderData == obj.renderData).SelectMany((GrowSystem.Stage st) => st.tiles).DefaultIfEmpty(num2)
			.Max();
		if (num > 0)
		{
			num3 = Math.Max(num3, obj.growth.harvestTile + ((obj.growth is GrowSystemWheat) ? 1 : 0));
		}
		if (num3 - num2 + 1 > frames)
		{
			Debug.LogWarning($"#tile {tag} the growth stages has {num3 - num2 + 1} frames but the sprite has {frames}");
		}
		if (!(obj.growth is GrowSystemTree))
		{
			if (num <= 0)
			{
				return baseIndex + Math.Max(0, frames - 2);
			}
			return num;
		}
		return baseIndex + Math.Min(2, frames - 1);
		int Cell(string s, string what)
		{
			int num4 = s.ToInt();
			if (num4 < 0 || num4 >= frames)
			{
				Debug.LogWarning($"#tile {tag} _growth {what} offset {num4} is outside {frames}-frame");
				num4 = Mathf.Clamp(num4, 0, frames - 1);
			}
			return baseIndex + num4;
		}
	}

	private static void InitGrowth(SourceObj.Row obj, string[] resolved)
	{
		string[] growth = obj._growth;
		obj._growth = resolved;
		try
		{
			obj.growth.Init(obj);
		}
		finally
		{
			obj._growth = growth;
		}
	}

	private static void ApplyRow(RenderRow row, int[] tiles)
	{
		row.tiles = tiles;
		row._tiles = Array.Empty<int>();
		row.sprites = null;
		row.SetTiles();
	}

	private static void Blit(TextureData data, FileInfo file, int index)
	{
		TextureReplace textureReplace = new TextureReplace
		{
			file = file,
			index = index,
			data = data,
			source = TextureReplace.Source.Mod
		};
		data.AddReplace(textureReplace);
		textureReplace.TryRefresh(force: false);
	}

	private static bool TryMeasure(FileInfo file, TextureData data, out int frames, out int rows)
	{
		frames = (rows = 0);
		Texture2D texture2D = IO.LoadPNG(file.FullName);
		if (!texture2D)
		{
			return false;
		}
		frames = texture2D.width / data.tileW;
		rows = texture2D.height / data.tileH;
		int num = frames;
		int result;
		if (num >= 1 && num < 100)
		{
			num = rows;
			if (num >= 1 && num <= 4 && texture2D.width % data.tileW == 0)
			{
				result = ((texture2D.height % data.tileH == 0) ? 1 : 0);
				goto IL_0076;
			}
		}
		result = 0;
		goto IL_0076;
		IL_0076:
		UnityEngine.Object.Destroy(texture2D);
		return (byte)result != 0;
	}

	private static void CheckUsedPx(Grid grid, Texture2D tex, int tileW, int tileH)
	{
		Color32[] px = tex.GetPixels32();
		int num = Math.Min(grid.rows, tex.height / tileH);
		int num2 = Math.Min(grid.cols, tex.width / tileW);
		for (int i = 0; i < num; i++)
		{
			int y = tex.height - (i + 1) * tileH;
			for (int j = 0; j < num2; j++)
			{
				int num3 = i * grid.cols + j;
				if (!grid.used[num3])
				{
					grid.used[num3] = HasAnyPx(tex.width, j * tileW, y, tileW, tileH);
				}
			}
		}
		bool HasAnyPx(int texW, int x0, int num4, int w, int h)
		{
			for (int k = num4; k < num4 + h; k++)
			{
				int num5 = k * texW + x0;
				for (int l = 0; l < w; l++)
				{
					if (px[num5 + l].a != 0)
					{
						return true;
					}
				}
			}
			return false;
		}
	}

	private static Grid BuildGrid(TextureData data, TextureData snow, Dictionary<MeshPass, TextureData> passData)
	{
		Grid grid = new Grid
		{
			cols = data.tex.width / data.tileW,
			rows = data.tex.height / data.tileH
		};
		grid.used = new bool[grid.cols * grid.rows];
		CheckUsedPx(grid, data.tex, data.tileW, data.tileH);
		if (snow != null)
		{
			CheckUsedPx(grid, snow.tex, data.tileW, data.tileH);
		}
		foreach (RenderRow allTileRow in GetAllTileRows())
		{
			RenderData renderData = allTileRow.renderData;
			if (!renderData || !renderData.pass || passData.TryGetValue(renderData.pass) != data || allTileRow._tiles == null)
			{
				continue;
			}
			TileBounds tileBounds = GetTileBounds(allTileRow);
			int[] tiles = allTileRow._tiles;
			for (int i = 0; i < tiles.Length; i++)
			{
				int num = Math.Abs(tiles[i]) - tileBounds.left;
				for (int j = -tileBounds.up; j <= tileBounds.down; j++)
				{
					grid.SetUsedSequence(num + j * grid.cols, tileBounds.left + tileBounds.width);
				}
			}
			if (allTileRow is SourceObj.Row { growth: { stages: var stages } growth })
			{
				foreach (GrowSystem.Stage stage in stages)
				{
					if (!stage.renderData || passData.TryGetValue(stage.renderData.pass) != data)
					{
						continue;
					}
					tiles = stage.tiles;
					foreach (int num2 in tiles)
					{
						for (int l = -tileBounds.up; l <= 0; l++)
						{
							grid.SetUsedSequence(num2 + l * grid.cols, 1);
						}
					}
				}
				if (growth.harvestTile != 0 && (bool)growth.RenderHarvest && passData.TryGetValue(growth.RenderHarvest.pass) == data)
				{
					grid.SetUsedSequence(growth.harvestTile, 2);
				}
			}
			if (allTileRow.snowTile <= 0)
			{
				continue;
			}
			RenderRow renderRow = allTileRow;
			if (!(renderRow is SourceBlock.Row))
			{
				if (renderRow is SourceObj.Row { idRoof: not 0 })
				{
					grid.SetUsedSequence(renderData.ConvertTile(allTileRow.snowTile), 12);
				}
			}
			else
			{
				grid.SetUsedSequence(renderData.ConvertTile(allTileRow.snowTile), 16);
			}
		}
		SetBuiltinSequence(grid, data, passData);
		return grid;
	}

	private static void SetBuiltinSequence(Grid grid, TextureData data, Dictionary<MeshPass, TextureData> passData)
	{
		if (grid.cols != 32)
		{
			return;
		}
		List<SourceFloor.Row> list = EClass.sources.floors.rows.Where((SourceFloor.Row r) => (bool)r.renderData && (bool)r.renderData.pass && passData.TryGetValue(r.renderData.pass) == data).ToList();
		if (list.Count == 0)
		{
			return;
		}
		grid.SetUsedSequence(24 * grid.cols, 16);
		foreach (SourceFloor.Row item in list)
		{
			if (item.edge >= 0)
			{
				grid.SetUsedSequence((24 + item.edge / 2) * grid.cols + item.edge % 2 * 16, 16);
			}
			if (item.autotile > 0)
			{
				grid.SetUsedSequence((26 + item.autotile / 2) * grid.cols + item.autotile % 2 * 16, 16);
			}
		}
		foreach (SourceDeco.Row row in EClass.sources.decos.rows)
		{
			if (row.autotile > 0 && (bool)row.renderData && (bool)row.renderData.pass && passData.TryGetValue(row.renderData.pass) == data)
			{
				grid.SetUsedSequence((26 + row.autotile / 2) * grid.cols + row.autotile % 2 * 16, 16);
			}
		}
	}

	private static int AllocateRect(Grid grid, int cols, int rows, int baseRowOffset, int baseColOffset)
	{
		for (int i = 0; i + rows <= grid.rows; i++)
		{
			for (int j = 0; j + cols <= grid.cols; j++)
			{
				if (IsRectFree(grid, i, j, cols, rows))
				{
					for (int k = 0; k < rows; k++)
					{
						grid.SetUsedSequence((i + k) * grid.cols + j, cols);
					}
					return (i + baseRowOffset) * 100 + j + baseColOffset;
				}
			}
		}
		return -1;
	}

	private static bool IsRectFree(Grid grid, int r, int c, int cols, int rows)
	{
		for (int i = 0; i < rows; i++)
		{
			int num = (r + i) * grid.cols + c;
			for (int j = 0; j < cols; j++)
			{
				if (grid.used[num + j])
				{
					return false;
				}
			}
		}
		return true;
	}

	private static bool TryAddRows(TextureData data, TextureData snow, Grid grid, int rowsNeeded, string tag)
	{
		int cols = grid.cols;
		int rows = grid.rows;
		int num = Math.Min(10000 / cols, SystemInfo.maxTextureSize / data.tileH);
		if (rows + rowsNeeded > num)
		{
			Debug.LogError($"#tile '{data.id}' has no empty rows ({rows} rows of {cols}, max {num}), cannot add {tag}");
			return false;
		}
		int num2 = Math.Min(Math.Max(rowsNeeded, 8), num - rows);
		List<MeshPass> passes = GetPasses(data, snow);
		TextureData[] array = new TextureData[2] { data, snow };
		foreach (TextureData textureData in array)
		{
			if (textureData != null)
			{
				ReinitTextureH(textureData, num2 * textureData.tileH);
			}
		}
		foreach (MeshPass item in passes)
		{
			ProceduralMesh pmesh = item.pmesh;
			if ((bool)pmesh && Mathf.RoundToInt(pmesh.tiling.x) == cols && Mathf.RoundToInt(pmesh.tiling.y) == rows)
			{
				if (!_grownMesh.ContainsKey(pmesh))
				{
					_grownMesh[pmesh] = pmesh.tiling;
				}
				pmesh.tiling.y = rows + num2;
				pmesh.Create();
			}
			Material mat = item.mat;
			if (!mat || !mat.HasProperty(_tiling))
			{
				continue;
			}
			Vector4 vector = mat.GetVector(_tiling);
			if (Mathf.RoundToInt(vector.x) == cols && Mathf.RoundToInt(vector.y) == rows)
			{
				_grownMat.TryAdd(mat, vector);
				float num3 = 1f / (float)rows;
				if (vector.z > 0f && vector.z < num3)
				{
					vector.z *= (float)rows / (float)(rows + num2);
				}
				if (vector.w > 0f && vector.w < num3)
				{
					vector.w *= (float)rows / (float)(rows + num2);
				}
				vector.y = rows + num2;
				mat.SetVector(_tiling, vector);
			}
		}
		foreach (MeshPass item2 in passes)
		{
			if ((bool)item2.pmesh)
			{
				item2.mesh = null;
				item2._Refresh();
			}
		}
		grid.AddRows(num2);
		Debug.Log($"#tile '{data.id}' added {num2} rows to {grid.rows}x{cols} ({data.tex.width}x{data.tex.height}) for {tag}");
		return true;
	}

	private static void ReinitTextureH(TextureData d, int addH)
	{
		Texture2D tex = d.tex;
		int width = tex.width;
		int height = tex.height;
		Color32[] pixels = tex.GetPixels32();
		if (!_grownTex.ContainsKey(d))
		{
			_grownTex[d] = (width, height);
		}
		bool flag = tex.mipmapCount > 1;
		if (!tex.Reinitialize(width, height + addH, tex.format, flag))
		{
			throw new InvalidOperationException($"could not reinitialize '{d.id}' to {width}x{height + addH}");
		}
		tex.SetPixels32(0, addH, width, height, pixels);
		tex.SetPixels32(0, 0, width, addH, new Color32[width * addH]);
		tex.Apply(flag, makeNoLongerReadable: false);
		d.extraRows += addH / d.tileH;
	}

	private static List<MeshPass> GetPasses(TextureData data, TextureData snow)
	{
		HashSet<MeshPass> hashSet = new HashSet<MeshPass>();
		TextureData[] array = new TextureData[2] { data, snow };
		foreach (TextureData textureData in array)
		{
			if (textureData == null)
			{
				continue;
			}
			foreach (MeshPass item in textureData.listPass.Where((MeshPass p) => p))
			{
				hashSet.Add(item);
				if ((bool)item.subPass)
				{
					hashSet.Add(item.subPass);
				}
				if ((bool)item.snowPass)
				{
					hashSet.Add(item.snowPass);
				}
				if ((bool)item.shadowPass)
				{
					hashSet.Add(item.shadowPass);
				}
			}
		}
		return hashSet.ToList();
	}

	private static Dictionary<MeshPass, TextureData> BuildPassData()
	{
		Dictionary<MeshPass, TextureData> dictionary = new Dictionary<MeshPass, TextureData>();
		foreach (TextureData value in EClass.core.textures.texMap.Values)
		{
			foreach (MeshPass item in value.listPass)
			{
				dictionary[item] = value;
			}
		}
		return dictionary;
	}

	private static IEnumerable<RenderRow> GetAllTileRows()
	{
		foreach (SourceBlock.Row row in EClass.sources.blocks.rows)
		{
			yield return row;
		}
		foreach (SourceFloor.Row row2 in EClass.sources.floors.rows)
		{
			yield return row2;
		}
		foreach (SourceObj.Row row3 in EClass.sources.objs.rows)
		{
			yield return row3;
		}
		foreach (SourceDeco.Row row4 in EClass.sources.decos.rows)
		{
			yield return row4;
		}
		foreach (SourceCellEffect.Row row5 in EClass.sources.cellEffects.rows)
		{
			yield return row5;
		}
		foreach (SourceThing.Row row6 in EClass.sources.things.rows)
		{
			yield return row6;
		}
	}

	private static int[] GetAnime(RenderRow row)
	{
		if (!(row is SourceBlock.Row { anime: var anime }))
		{
			if (!(row is SourceFloor.Row { anime: var anime2 }))
			{
				if (!(row is SourceDeco.Row { anime: var anime3 }))
				{
					if (!(row is SourceCellEffect.Row { anime: var anime4 }))
					{
						if (!(row is SourceThing.Row { anime: var anime5 }))
						{
							return null;
						}
						return anime5;
					}
					return anime4;
				}
				return anime3;
			}
			return anime2;
		}
		return anime;
	}
}
