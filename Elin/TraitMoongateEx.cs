using System;
using System.Collections.Generic;

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
		List<MapMetaData> list = ListSavedUserMap();
		if (list.Count == 0)
		{
			EClass.pc.SayNothingHappans();
			return;
		}
		foreach (MapMetaData item in list)
		{
			bool flag = false;
			foreach (string item2 in EClass.player.favMoongate)
			{
				_ = item2;
				if (EClass.player.favMoongate.Contains(item.id))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				EClass.player.favMoongate.Remove(item.id);
			}
		}
		Sort();
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
			UIButton uIButton = b.AddSubButton(EClass.core.refs.icons.fav, delegate
			{
				SE.ClickGeneral();
				EClass.player.ToggleFavMoongate(a.id);
				Sort();
				EClass.ui.FreezeScreen(0.1f);
				layer.list.List();
			});
			uIButton.icon.SetAlpha(EClass.player.favMoongate.Contains(a.id) ? 1f : 0.3f);
			uIButton.icon.SetNativeSize();
			void func()
			{
				IO.DeleteFile(a.path);
				list.Remove(a);
				EClass.ui.FreezeScreen(0.1f);
				layer.list.List();
				SE.Trash();
			}
		}).SetSize(500f)
			.SetTitles("wMoongate") as LayerList;
		static DateTime GetDate(MapMetaData meta)
		{
			int num = EClass.player.favMoongate.IndexOf(meta.id);
			if (num == -1)
			{
				return meta.date;
			}
			return meta.date + new TimeSpan(-3650 - num, 0, 0, 0, 0);
		}
		void Sort()
		{
			list.Sort((MapMetaData a, MapMetaData b) => DateTime.Compare(GetDate(a), GetDate(b)));
		}
	}
}
