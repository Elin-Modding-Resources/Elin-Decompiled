using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TraitMoongate : Trait
{
	public UniTask<bool> test;

	public virtual string AllowedCat
	{
		get
		{
			if (!(EClass._zone is Zone_Tent))
			{
				return "Home,Dungeon,Town";
			}
			return "Tent";
		}
	}

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
		Net.DownloadMeta meta = null;
		try
		{
			List<Net.DownloadMeta> listOrg = await Net.GetFileList(lang);
			listOrg = listOrg.Where((Net.DownloadMeta m) => m.IsValidVersion()).ToList();
			listOrg.ForeachReverse(delegate(Net.DownloadMeta m)
			{
				if (!AllowedCat.Split(',').Contains(m.cat.IsEmpty("Home")))
				{
					listOrg.Remove(m);
				}
			});
			if (listOrg == null || listOrg.Count == 0)
			{
				EClass.pc.SayNothingHappans();
				return false;
			}
			List<MapMetaData> list = ListSavedUserMap();
			IList<Net.DownloadMeta> list2 = listOrg.Copy();
			foreach (MapMetaData item in list)
			{
				foreach (Net.DownloadMeta item2 in list2)
				{
					if (item2.id == item.id && item2.version == item.version)
					{
						list2.Remove(item2);
						break;
					}
				}
			}
			Debug.Log(list2.Count);
			if (list2.Count == 0)
			{
				list2 = listOrg.Copy();
			}
			meta = list2.RandomItem();
			Zone_User zone_User = EClass.game.spatials.Find((Zone_User z) => z.id == meta.id);
			if (zone_User != null)
			{
				MoveZone(zone_User);
				return true;
			}
			FileInfo fileInfo = await Net.DownloadFile(meta, CorePath.ZoneSaveUser, lang);
			Debug.Log(meta?.ToString() + "/" + meta.title + meta.id + "/" + meta.path + "/");
			Debug.Log(fileInfo?.ToString() + "/" + meta.name + "/" + meta.path);
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
			if (meta != null)
			{
				EClass.ui.Say("Invalid Usermap: " + meta.title + "(" + meta.id + ")");
			}
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
