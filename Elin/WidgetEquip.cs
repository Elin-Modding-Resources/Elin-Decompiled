using UnityEngine;
using UnityEngine.UI;

public class WidgetEquip : Widget
{
	public class Extra
	{
		public bool showIndex;
	}

	public static WidgetEquip Instance;

	public static bool dirty;

	public static Thing dragEquip;

	public UIList listMain;

	public UIList listEtc;

	public Chara chara;

	public RectTransform transLayer;

	public GridLayoutGroup grid;

	public Extra extra => base.config.extra as Extra;

	public override object CreateExtra()
	{
		return new Extra();
	}

	public static void SetDirty()
	{
		dirty = true;
	}

	public override void OnActivate()
	{
		chara = EMono.pc;
		Instance = this;
		foreach (Thing thing in EMono.pc.things)
		{
			if (thing.trait is TraitToolBelt)
			{
				AddLayer(LayerInventory.CreateContainer(thing), transLayer);
			}
		}
		Rebuild();
	}

	public static void OnChangeBodyPart()
	{
		if ((bool)Instance)
		{
			Instance.Rebuild();
		}
	}

	public void Rebuild()
	{
		RefreshEquipment(listMain);
		RefreshEquipment(listEtc, etc: true);
		dirty = true;
		this.RebuildLayout(recursive: true);
	}

	public void CheckDirty()
	{
		if (dirty)
		{
			_Redraw();
		}
	}

	public static void Redraw()
	{
		if ((bool)Instance)
		{
			Instance._Redraw();
		}
	}

	public void _Redraw()
	{
		dirty = false;
		listMain.Redraw();
		listEtc.Redraw();
	}

	public void RefreshEquipment(UIList list, bool etc = false)
	{
		list.Clear();
		list.callbacks = new UIList.Callback<BodySlot, ButtonGridDrag>
		{
			onInstantiate = delegate(BodySlot a, ButtonGridDrag b)
			{
				b.SetBodySlot(a, new InvOwnerEquip(chara, a), extra.showIndex);
			},
			onRedraw = delegate(BodySlot a, ButtonGridDrag b, int i)
			{
				b.SetBodySlot(a, new InvOwnerEquip(chara, a), extra.showIndex);
			},
			onSort = (BodySlot a, UIList.SortMode b) => chara.body.GetSortVal(a),
			onList = delegate
			{
				int num = 0;
				foreach (BodySlot slot in chara.body.slots)
				{
					if (slot.elementId != 44)
					{
						if (slot.elementId == 36 || slot.elementId == 31 || slot.elementId == 45)
						{
							if (!etc)
							{
								continue;
							}
						}
						else if (etc)
						{
							continue;
						}
						list.Add(slot);
						num++;
					}
				}
				if (list == listMain)
				{
					grid.constraintCount = num / 12 + 1;
				}
			}
		};
		list.onAfterRedraw = delegate
		{
			LayerInventory.TryShowGuide(list);
		};
		list.sortMode = UIList.SortMode.ByNumber;
		list.List();
	}

	public override bool CanShowContextMenu()
	{
		ButtonGrid componentOf = InputModuleEX.GetComponentOf<ButtonGrid>();
		if ((bool)componentOf && componentOf.card != null)
		{
			return false;
		}
		return base.CanShowContextMenu();
	}

	public override void OnSetContextMenu(UIContextMenu m)
	{
		m.AddChild("setting").AddToggle("showWeaponIndex", extra.showIndex, delegate(bool a)
		{
			extra.showIndex = a;
			Rebuild();
			SE.ClickOk();
		});
		SetBaseContextMenu(m);
	}
}
