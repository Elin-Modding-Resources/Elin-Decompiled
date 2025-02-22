using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TraitMoongate : Trait
{
	public UniTask<bool> test;

	public override bool CanUse(Chara c)
	{
		if (EClass._zone.IsInstance || EClass._zone.dateExpire != 0 || EClass._zone.IsRegion || !owner.IsInstalled)
		{
			return false;
		}
		return true;
	}

	public override bool OnUse(Chara c)
	{
		if (EClass.core.version.demo)
		{
			Msg.SayNothingHappen();
			return false;
		}
		Core.TryWarnMod(delegate
		{
			LayerProgress.StartAsync("Loading", UseMoongate());
		}, warn: false);
		return false;
	}

	public async UniTask<bool> UseMoongate()
	{
		string lang = Lang.langCode;
		if (lang != "JP" && lang != "CN" && lang != "EN")
		{
			lang = "EN";
		}
		if (EClass.core.config.backer.filter == 0)
		{
			lang = ((EClass.rnd(10) != 0) ? ((EClass.rnd(2) == 0) ? "JP" : "EN") : "CN");
		}
		Debug.Log(lang);
		try
		{
			List<Net.DownloadMeta> list = (await Net.GetFileList(lang)).Where((Net.DownloadMeta m) => m.IsValidVersion()).ToList();
			if (list == null || list.Count == 0)
			{
				EClass.pc.SayNothingHappans();
				return false;
			}
			List<MapMetaData> list2 = ListSavedUserMap();
			IList<Net.DownloadMeta> list3 = list.Copy();
			foreach (MapMetaData item2 in list2)
			{
				foreach (Net.DownloadMeta item3 in list3)
				{
					if (item3.id == item2.id && item3.version == item2.version)
					{
						list3.Remove(item3);
						break;
					}
				}
			}
			Debug.Log(list3.Count);
			if (list3.Count == 0)
			{
				list3 = list.Copy();
			}
			Net.DownloadMeta item = list3.RandomItem();
			Zone_User zone_User = EClass.game.spatials.Find((Zone_User z) => z.id == item.id);
			if (zone_User != null)
			{
				MoveZone(zone_User);
				return true;
			}
			FileInfo fileInfo = await Net.DownloadFile(item, CorePath.ZoneSaveUser, lang);
			Debug.Log(item?.ToString() + "/" + item.title + item.id + "/" + item.path + "/");
			Debug.Log(fileInfo?.ToString() + "/" + item.name + "/" + item.path);
			if (Zone.IsImportValid(fileInfo.FullName))
			{
				Debug.Log("valid");
				LoadMap(Map.GetMetaData(fileInfo.FullName));
			}
			else
			{
				Debug.Log("invalid");
				EClass.pc.SayNothingHappans();
			}
		}
		catch (Exception ex)
		{
			EClass.ui.Say(ex.Message);
			return false;
		}
		return true;
	}

	public void LoadMap(MapMetaData m)
	{
		if (EClass.pc.burden.GetPhase() == 4)
		{
			Msg.Say("returnOverweight");
			return;
		}
		Debug.Log("loading:" + m.name + "/" + m.path);
		Zone_User zone_User = EClass.game.spatials.Find((Zone_User z) => z.idUser == m.id);
		if (zone_User == null)
		{
			zone_User = SpatialGen.Create("user", EClass.world.region, register: true) as Zone_User;
			zone_User.path = m.path;
			zone_User.idUser = m.id;
			zone_User.dateExpire = EClass.world.date.GetRaw(1);
			zone_User.name = m.name;
		}
		Debug.Log(zone_User);
		MoveZone(zone_User);
	}

	public void MoveZone(Zone zone)
	{
		zone.instance = new ZoneInsstanceMoongate
		{
			uidZone = EClass._zone.uid,
			x = EClass.pc.pos.x,
			z = EClass.pc.pos.z
		};
		EClass.pc.MoveZone(zone, ZoneTransition.EnterState.Moongate);
	}

	public List<MapMetaData> ListSavedUserMap()
	{
		List<MapMetaData> list = new List<MapMetaData>();
		foreach (FileInfo item in new DirectoryInfo(CorePath.ZoneSaveUser).GetFiles().Concat(MOD.listMaps))
		{
			if (!(item.Extension != ".z"))
			{
				MapMetaData metaData = Map.GetMetaData(item.FullName);
				if (metaData != null && metaData.IsValidVersion())
				{
					metaData.path = item.FullName;
					metaData.date = item.LastWriteTime;
					list.Add(metaData);
				}
			}
		}
		return list;
	}
}
