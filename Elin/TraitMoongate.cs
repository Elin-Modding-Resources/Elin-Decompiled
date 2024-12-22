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
			Net.DownloadMeta item = list.RandomItem();
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
}
