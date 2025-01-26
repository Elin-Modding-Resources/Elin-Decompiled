using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class TraitMoongateEx : TraitMoongate
{
	public override bool OnUse(Chara c)
	{
		if (EClass.core.version.demo)
		{
			Msg.SayNothingHappen();
			return false;
		}
		Core.TryWarnMod(delegate
		{
			_OnUse();
		}, warn: false);
		return false;
	}

	public void _OnUse()
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
		if (list.Count == 0)
		{
			EClass.pc.SayNothingHappans();
			return;
		}
		list.Sort((MapMetaData a, MapMetaData b) => DateTime.Compare(a.date, b.date));
		LayerList layer = null;
		bool skipDialog = false;
		layer = EClass.ui.AddLayer<LayerList>().SetList2(list, (MapMetaData a) => a.name, delegate(MapMetaData a, ItemGeneral b)
		{
			LoadMap(a);
		}, delegate(MapMetaData a, ItemGeneral b)
		{
			b.AddSubButton(EClass.core.refs.icons.trash, delegate
			{
				if (skipDialog)
				{
					func();
				}
				else
				{
					Dialog.Choice("dialogDeleteGame", delegate(Dialog d)
					{
						d.AddButton("yes".lang(), delegate
						{
							func();
						});
						d.AddButton("yesAndSkip".lang(), delegate
						{
							func();
							skipDialog = true;
						});
						d.AddButton("no".lang());
					});
				}
			});
			void func()
			{
				IO.DeleteFile(a.path);
				list.Remove(a);
				layer.list.List();
				SE.Trash();
			}
		}).SetSize(500f)
			.SetTitles("wMoongate") as LayerList;
	}
}
