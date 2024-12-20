using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class BaseArea : EClass, IInspect
{
	public enum AccessType
	{
		Public,
		Resident,
		Private
	}

	public class Interaction
	{
		public string text;

		public Action action;
	}

	[JsonProperty]
	public int uid;

	[JsonProperty]
	public PointList points = new PointList();

	[JsonProperty]
	public AreaData data = new AreaData();

	[JsonProperty]
	public AreaType type = new AreaTypeRoom();

	public TraitRoomPlate plate;

	public virtual string Name
	{
		get
		{
			if (!data.name.IsEmpty())
			{
				return data.name;
			}
			return type.source.GetName();
		}
	}

	public bool IsPrivate => data.accessType == AccessType.Private;

	public SourceArea.Row source => type.source;

	public bool CanInspect => true;

	public string InspectName => Name;

	public Point InspectPoint => Point.Invalid;

	public Vector3 InspectPosition => Vector3.zero;

	public Point GetRandomFreePos()
	{
		for (int i = 0; i < 100; i++)
		{
			Point point = points.RandomItem();
			if (!point.IsBlocked)
			{
				return point;
			}
		}
		return null;
	}

	public Thing GetEmptySeat()
	{
		foreach (Point point in points)
		{
			foreach (Thing thing in point.Things)
			{
				if (thing.IsInstalled && !thing.pos.HasChara && thing.trait is TraitChair)
				{
					return thing;
				}
			}
		}
		return null;
	}

	public virtual Point GetRandomPoint(bool walkable = true, bool allowChara = true)
	{
		if (walkable)
		{
			for (int i = 0; i < 100; i++)
			{
				foreach (Point point in points)
				{
					if (!point.IsBlocked && (allowChara || !point.HasChara))
					{
						return point;
					}
				}
			}
		}
		return points.RandomItem();
	}

	public void ChangeType(string _id)
	{
		type = ClassCache.Create<AreaType>("AreaType" + _id, "Elin");
		type.id = _id;
		type.owner = this;
		if (plate != null)
		{
			plate.areaData.type = type;
		}
	}

	public void SetRandomName(int seed = -1)
	{
		data.name = GetRandomName(seed);
	}

	public string GetRandomName(int seed = -1)
	{
		if (seed != -1)
		{
			Rand.SetSeed(seed);
		}
		string combinedName = WordGen.GetCombinedName(EClass.Branch?.GetRandomName(), ListRoomNames().RandomItem(), room: true);
		if (seed != -1)
		{
			Rand.SetSeed();
		}
		return combinedName;
	}

	public HashSet<string> ListRoomNames()
	{
		HashSet<string> hashSet = new HashSet<string>();
		string[] list = Lang.GetList("rooms");
		foreach (string item in list)
		{
			hashSet.Add(item);
		}
		foreach (Point point in points)
		{
			foreach (Thing thing in point.Things)
			{
				if (thing.IsInstalled && !thing.source.roomName.IsEmpty())
				{
					list = thing.source.GetTextArray("roomName");
					foreach (string item2 in list)
					{
						hashSet.Add(item2);
					}
				}
			}
		}
		return hashSet;
	}

	public int GetSortVal(UIList.SortMode m)
	{
		_ = 7;
		return source._index;
	}

	public List<Interaction> ListInteractions()
	{
		return new List<Interaction>
		{
			new Interaction
			{
				text = "accessType".lang(("access_" + data.accessType).lang()),
				action = delegate
				{
					UIContextMenu uIContextMenu = EClass.ui.CreateContextMenuInteraction();
					foreach (AccessType t in Util.EnumToList<AccessType>())
					{
						uIContextMenu.AddButton(((data.accessType == t) ? "context_checker".lang() : "") + ("access_" + t).lang(), delegate
						{
							data.accessType = t;
							if (plate != null)
							{
								plate.areaData.accessType = data.accessType;
							}
							SE.ClickOk();
						});
					}
					CursorSystem.ignoreCount = 5;
					uIContextMenu.Show();
				}
			},
			new Interaction
			{
				text = "changeName",
				action = delegate
				{
					Dialog.InputName("dialogChangeName", data.name.IsEmpty(GetRandomName()), delegate(bool cancel, string text)
					{
						if (!cancel)
						{
							data.name = text;
							if (plate != null)
							{
								plate.areaData.name = text;
							}
						}
					});
				}
			},
			new Interaction
			{
				text = "toggleShowWallItem".lang() + "(" + (data.showWallItem ? "on" : "off").lang() + ")",
				action = delegate
				{
					data.showWallItem = !data.showWallItem;
					SE.ClickOk();
				}
			},
			new Interaction
			{
				text = "toggleAtrium".lang() + "(" + (data.atrium ? "on" : "off").lang() + ")",
				action = delegate
				{
					data.atrium = !data.atrium;
					if (plate != null)
					{
						plate.areaData.atrium = data.atrium;
					}
					SE.ClickOk();
				}
			},
			new Interaction
			{
				text = "limitRoomHeight",
				action = delegate
				{
					List<string> list2 = new List<string>();
					EClass.ui.AddLayer<LayerList>().SetStringList(delegate
					{
						list2.Clear();
						for (int j = 1; j < 10; j++)
						{
							list2.Add(j.ToString() ?? "");
						}
						return list2;
					}, delegate(int a, string b)
					{
						data.maxHeight = a + 1;
						if (plate != null)
						{
							plate.areaData.maxHeight = a + 1;
						}
						EClass._map.rooms.RefreshAll();
					});
				}
			},
			new Interaction
			{
				text = "changeGroup",
				action = delegate
				{
					List<string> list = new List<string>();
					EClass.ui.AddLayer<LayerList>().SetStringList(delegate
					{
						list.Clear();
						for (int i = 0; i < 5; i++)
						{
							list.Add(i.ToString() ?? "");
						}
						return list;
					}, delegate(int a, string b)
					{
						data.group = a;
						if (plate != null)
						{
							plate.areaData.group = a;
						}
						EClass._map.rooms.RefreshAll();
					});
				}
			}
		};
	}

	public void OnInspect()
	{
	}

	public void WriteNote(UINote n, Action<UINote> onWriteNote = null, IInspect.NoteMode mode = IInspect.NoteMode.Default, Recipe recipe = null)
	{
		n.Clear();
		n.AddHeaderCard(Name);
		n.Build();
	}
}
