using System.Collections.Generic;
using UnityEngine;

public class LayerEditPlaylist : ELayer
{
	public enum Mode
	{
		Playlist,
		LotBGM
	}

	public class ListBGM : ListOwner<BGMData, ItemGeneral>
	{
		public List<BGMData> items;

		public string idTitle;

		public bool single;

		public new LayerEditPlaylist layer => base.layer as LayerEditPlaylist;

		public override string IdTitle => idTitle;

		public override void List()
		{
			list.callbacks = new UIList.Callback<BGMData, ItemGeneral>
			{
				onInstantiate = delegate(BGMData a, ItemGeneral b)
				{
					b.SetMainText(((a.id > 1) ? "♪ " : "") + a._name.IsEmpty(a.name).ToTitleCase());
					b.button1.keyText.text = a.id.ToString() ?? "";
					if (a.id != 0 && layer.jukebox == null)
					{
						b.AddSubButton(EClass.core.refs.icons.resume, delegate
						{
							EClass.Sound.PlayBGM(a);
						});
					}
					if (main && layer.mode == Mode.Playlist)
					{
						b.AddSubButton(EClass.core.refs.icons.down, delegate
						{
							BGMData select2 = items.Move(a, 1);
							list.OnMove(a, select2);
						});
						b.AddSubButton(EClass.core.refs.icons.up, delegate
						{
							BGMData select = items.Move(a, -1);
							list.OnMove(a, select);
						});
					}
					b.Build();
					if (!single)
					{
						b.button1.soundClick = null;
					}
				},
				onClick = delegate(BGMData c, ItemGeneral i)
				{
					if (layer.mode == Mode.Playlist)
					{
						items.Remove(c);
						(other as ListBGM).items.Add(c);
						MoveToOther(c);
					}
					else if (layer.board != null)
					{
						layer.board.data.idBGM = ((layer.board.data.idBGM != c.id) ? c.id : 0);
						layer.board.ApplyData();
						layer.Close();
					}
					else
					{
						layer.jukebox.OnSetBGM(c);
						layer.Close();
					}
				},
				onList = delegate
				{
					foreach (BGMData item in items)
					{
						list.Add(item);
					}
				}
			};
			list.List();
		}
	}

	public List<BGMData> itemsDay = new List<BGMData>();

	public List<BGMData> itemsNight = new List<BGMData>();

	public UIMultiList multi;

	public bool day;

	public bool keepPlaying;

	public bool single;

	public bool dayNight;

	public Mode mode;

	public TraitHouseBoard board;

	public TraitJukeBox jukebox;

	public Dictionary<int, BGMData> bgms => ELayer.core.refs.dictBGM;

	public override void OnInit()
	{
	}

	public void Activate(TraitJukeBox _box)
	{
		jukebox = _box;
		single = true;
		Activate(Mode.LotBGM);
	}

	public void Activate(TraitHouseBoard _board)
	{
		board = _board;
		single = true;
		Activate(Mode.LotBGM);
	}

	public void Activate(Mode _mode = Mode.Playlist)
	{
		mode = _mode;
		Msg.Nerun("nPlaylist", "UN_nerun_smile3");
		if (mode == Mode.Playlist)
		{
			if (dayNight)
			{
				windows[0].AddBottomButton("togglePlaylist", TogglePL);
			}
			foreach (int item in ELayer._map._plDay)
			{
				if (bgms.ContainsKey(item))
				{
					itemsDay.Add(bgms[item]);
				}
			}
			foreach (int item2 in ELayer._map._plNight)
			{
				if (bgms.ContainsKey(item2))
				{
					itemsNight.Add(bgms[item2]);
				}
			}
		}
		else
		{
			windows[1].SetActive(enable: false);
			windows[0].SetPosition();
			multi.Double = false;
		}
		Refresh();
		foreach (ListOwner owner in multi.owners)
		{
			owner.OnSwitchContent();
		}
	}

	public void Refresh()
	{
		List<BGMData> list = ((mode == Mode.LotBGM) ? new List<BGMData>() : (day ? itemsDay : itemsNight));
		List<BGMData> list2 = new List<BGMData>();
		foreach (BGMData value in bgms.Values)
		{
			if (!ELayer.debug.allBGM && !ELayer.player.knownBGMs.Contains(value.id))
			{
				continue;
			}
			if (mode == Mode.LotBGM)
			{
				list.Add(value);
				continue;
			}
			bool flag = value.id != 0;
			foreach (BGMData item in list)
			{
				if (item.id == value.id)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				list2.Add(value);
			}
		}
		multi.Clear();
		if (mode == Mode.LotBGM)
		{
			multi.AddOwner(0, new ListBGM
			{
				items = list,
				idTitle = "wBGMLot",
				single = true
			});
		}
		else
		{
			multi.AddOwner(0, new ListBGM
			{
				items = list,
				idTitle = "wPlaylist"
			});
			multi.AddOwner(1, new ListBGM
			{
				items = list2,
				idTitle = "wPlaylist2"
			});
		}
		multi.Build();
	}

	public void TogglePL()
	{
		day = !day;
		Refresh();
		multi.Refresh();
		SE.Click();
	}

	public override void OnKill()
	{
		if (mode == Mode.LotBGM)
		{
			ELayer.Sound.StopBGM();
			ELayer._zone.RefreshBGM();
			return;
		}
		ELayer._map._plDay.Clear();
		ELayer._map._plNight.Clear();
		foreach (BGMData item in itemsDay)
		{
			ELayer._map._plDay.Add(item.id);
		}
		foreach (BGMData item2 in itemsNight)
		{
			ELayer._map._plNight.Add(item2.id);
		}
		Object.DestroyImmediate(ELayer._map.plDay);
		ELayer._map.plDay = null;
		ELayer._zone.RefreshPlaylist();
		if (!keepPlaying)
		{
			ELayer.Sound.StopBGM();
			ELayer._zone.RefreshBGM();
		}
	}

	public override void OnSwitchContent(Window window)
	{
		if (multi.owners.Count > 0)
		{
			multi.owners[window.windowIndex].OnSwitchContent();
		}
	}
}
