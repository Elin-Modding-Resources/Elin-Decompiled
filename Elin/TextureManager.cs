using System;
using System.Collections.Generic;
using System.IO;

public class TextureManager
{
	public Dictionary<string, TextureData> texMap = new Dictionary<string, TextureData>();

	public string pathTex => CorePath.packageCore + "Texture/";

	public void Init()
	{
		BaseTileMap tileMap = EClass.scene.screenElin.tileMap;
		CoreRef.TextureDatas textureData = EClass.core.refs.textureData;
		AddBase(textureData.block, "blocks.png", tileMap.passBlock);
		AddBase(textureData.floor, "floors.png", tileMap.passFloor);
		AddBase(textureData.objs_SS, "objs_SS.png", tileMap.passObjSS);
		AddBase(textureData.objs_S, "objs_S.png", tileMap.passObjS);
		AddBase(textureData.objs, "objs.png", tileMap.passObj);
		AddBase(textureData.objs_L, "objs_L.png", tileMap.passObjL);
		AddBase(textureData.roofs, "roofs.png", tileMap.passRoof);
		AddBase(textureData.shadows, "shadows.png", tileMap.passShadow);
		AddBase(textureData.fov, "fov.png", tileMap.passFov);
		AddBase(textureData.objs_C, "objs_C.png", tileMap.passChara);
		AddBase(textureData.objs_CL, "objs_CL.png", tileMap.passCharaL);
		AddBase(textureData.objs_CLL, "objs_CLL.png", tileMap.passCharaLL);
		AddBase(textureData.block_snow, "blocks_snow.png", tileMap.passBlock.snowPass);
		AddBase(textureData.floor_snow, "floors_snow.png", tileMap.passFloor.snowPass);
		AddBase(textureData.objs_S_snow, "objs_S_snow.png", tileMap.passObjS.snowPass);
		AddBase(textureData.objs_snow, "objs_snow.png", tileMap.passObj.snowPass);
		AddBase(textureData.objs_L_snow, "objs_L_snow.png", tileMap.passObjL.snowPass);
		Add(tileMap.passRamp, textureData.block);
		Add(tileMap.passWaterBlock, textureData.block);
		Add(tileMap.passFog, textureData.block);
		Add(tileMap.passLiquid, textureData.block);
		Add(tileMap.passBlockEx, textureData.block);
		Add(tileMap.passInner, textureData.block);
		Add(tileMap.passBlockMarker, textureData.block);
		Add(tileMap.passEdge, textureData.floor);
		Add(tileMap.passFloorEx, textureData.floor);
		Add(tileMap.passFloorWater, textureData.floor);
		Add(tileMap.passAutoTile, textureData.floor);
		Add(tileMap.passShore, textureData.floor);
		Add(tileMap.passAutoTileWater, textureData.floor);
		Add(tileMap.passFloorMarker, textureData.floor);
		Add(tileMap.passCharaLW, textureData.objs_CL);
		Add(tileMap.passIcon, textureData.objs_S);
		AddList(textureData.world, "world.png");
		AddList(textureData.bird, "bird1.png");
		FileInfo[] files = new DirectoryInfo(CorePath.user + "/Texture Replace").GetFiles();
		foreach (FileInfo fileInfo in files)
		{
			if (fileInfo.Name.EndsWith(".png"))
			{
				TryAddReplace(fileInfo, TextureReplace.Source.User);
			}
		}
		foreach (FileInfo replaceFile in EClass.core.mods.replaceFiles)
		{
			TryAddReplace(replaceFile);
		}
	}

	public void AddBase(TextureData data, string path, MeshPass pass, string texName = "_MainTex")
	{
		texMap[data.id] = data;
		data.path = pathTex + path;
		data.texName = texName;
		data.dictReplace.Clear();
		data.tileW = data.tex.width / (int)pass.pmesh.tiling.x;
		data.tileH = data.tex.height / (int)pass.pmesh.tiling.y;
		texMap[data.id].listPass.Add(pass);
	}

	public void Add(MeshPass to, TextureData from, string texName = "_MainTex")
	{
		texMap[from.id].listPass.Add(to);
	}

	public void AddList(TextureData data, string path)
	{
		data.path = pathTex + path;
		texMap[data.id] = data;
	}

	public void RefreshTextures()
	{
		foreach (TextureData value in texMap.Values)
		{
			value.TryRefresh();
		}
		if (EClass.core.IsGameStarted && EClass._zone != null && EClass._zone.isStarted)
		{
			EClass.scene.ApplyZoneConfig();
		}
	}

	public void OnDropFile(List<string> paths)
	{
		int num = 0;
		foreach (string path in paths)
		{
			FileInfo fileInfo = new FileInfo(path);
			if (!TryAddReplace(fileInfo, TextureReplace.Source.User, add: false))
			{
				continue;
			}
			string text = CorePath.user + "/Texture Replace/" + fileInfo.Name;
			try
			{
				if (File.Exists(text))
				{
					File.Delete(text);
				}
				fileInfo.CopyTo(text, overwrite: true);
			}
			catch (Exception)
			{
			}
			TryAddReplace(new FileInfo(text), TextureReplace.Source.User, add: true, refresh: true);
			num++;
		}
		Msg.Say("Imported " + num + "image(s)");
	}

	public bool TryAddReplace(FileInfo file, TextureReplace.Source source = TextureReplace.Source.Mod, bool add = true, bool refresh = false)
	{
		string[] array = Path.GetFileNameWithoutExtension(file.Name).Split('_');
		if (array.Length == 1)
		{
			return false;
		}
		string key = array[0];
		if (!int.TryParse(array[1], out var result))
		{
			return false;
		}
		if (!texMap.ContainsKey(key))
		{
			return false;
		}
		TextureData textureData = texMap[key];
		if (textureData.listPass.Count == 0)
		{
			return false;
		}
		if (add)
		{
			textureData.AddReplace(new TextureReplace
			{
				file = file,
				index = result,
				data = textureData,
				source = source
			});
			if (refresh)
			{
				textureData.TryRefresh();
			}
		}
		return true;
	}

	public void ApplyLocalReplace(string path)
	{
		foreach (TextureData value in texMap.Values)
		{
			if (value.listReplaceLocal.Count > 0)
			{
				value.forceRefresh = true;
				foreach (TextureReplace item in value.listReplaceLocal)
				{
					item.DestoryTex();
					if (item.original != null)
					{
						value.dictReplace[item.index] = item.original;
					}
					else
					{
						value.dictReplace.Remove(item.index);
					}
				}
			}
			value.listReplaceLocal.Clear();
		}
		string path2 = path + "Texture Replace";
		if (Directory.Exists(path2))
		{
			FileInfo[] files = new DirectoryInfo(path2).GetFiles();
			foreach (FileInfo file in files)
			{
				TryAddReplace(file, TextureReplace.Source.Local);
			}
		}
		RefreshTextures();
	}
}
