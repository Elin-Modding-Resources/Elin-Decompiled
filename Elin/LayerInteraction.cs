using System;
using System.Collections.Generic;
using UnityEngine;

public class LayerInteraction : ELayer
{
	public enum Mode
	{
		Map,
		Inventory,
		EloMap,
		Area,
		Custom
	}

	public class Page
	{
		public UIButton button;

		public Area area;

		public List<Item> items = new List<Item>();

		public Item Add(object target, string text, string idSprite, Action action, int priority = 0, bool auto = false)
		{
			Item item = new Item
			{
				target = target,
				text = text,
				idSprite = idSprite,
				action = action,
				priority = priority,
				auto = auto
			};
			items.Add(item);
			return item;
		}

		public Item Add(object target, string text, Func<bool> valueFunc, Action<bool> action)
		{
			Action action2 = delegate
			{
				SE.Click();
				action(!valueFunc());
			};
			Item item = new Item
			{
				target = target,
				textFunc = () => text.lang() + " (" + (valueFunc() ? "on" : "off") + ")",
				action = action2,
				reload = true
			};
			items.Add(item);
			return item;
		}
	}

	public class Item
	{
		public object target;

		public string text;

		public string idSprite;

		public Action action;

		public int priority;

		public bool auto;

		public bool reload;

		public Func<string> textFunc;

		public bool IsArea => target is Area;
	}

	public static LayerInteraction Instance;

	public static IInspect Target;

	public Mode mode;

	public Point point;

	public List<Page> pages = new List<Page>();

	private bool first = true;

	public InteractionMenu menu;

	public WindowChara windowChara;

	public override void OnInit()
	{
		Instance = this;
		first = true;
	}

	public override void OnKill()
	{
		Target = null;
	}

	private void Update()
	{
		if (mode == Mode.Map)
		{
			_ = ELayer.screen.tileSelector;
			MeshPass obj = ((point.HasBlock || point.cell.liquidLv > 0) ? ELayer.screen.guide.passGuideBlock : ELayer.screen.guide.passGuideFloor);
			int num = 0;
			Vector3 vector = point.Position();
			obj.Add(vector.x, vector.y, vector.z, num, 0.3f);
		}
	}

	public override void OnUpdateInput()
	{
	}

	public static void Show(IInspect newTarget)
	{
		if (Target != newTarget)
		{
			Target = newTarget;
			Page page = GetPage(Target);
			((Instance != null) ? Instance : ELayer.ui.AddLayer<LayerInteraction>()).ShowPage(page);
		}
	}

	public static bool TryShow(bool quick)
	{
		if (!Scene.HitPoint.IsValid)
		{
			return false;
		}
		Show(ELayer.scene.mouseTarget.pos.ListInspectorTargets().NextItem(Target));
		return true;
	}

	public static Page GetPage(IInspect o)
	{
		Page page = new Page();
		_ = page.items;
		if (o is Area)
		{
			Area area = o as Area;
			page.area = area;
		}
		else if (o is Chara)
		{
			Chara t = o as Chara;
			string text = "charaInfo".lang();
			if (!t.IsHomeMember())
			{
				text = text + "(" + "unidentified".lang() + ")";
			}
			if (t.IsHomeMember())
			{
				page.Add(t, "tTalk".lang(), "", delegate
				{
					t.ShowDialog();
				});
			}
			else
			{
				page.Add(t, text, "", delegate
				{
					if (!t.IsHomeMember())
					{
						SE.Beep();
					}
				});
			}
		}
		else if (o is Thing)
		{
			Thing t2 = o as Thing;
			page.Add(t2, "objInfo", "", delegate
			{
				ELayer.ui.AddLayer<LayerInfo>().Set(t2);
			});
			if (t2.trait is TraitQuestBoard)
			{
				page.Add(t2, "quest", "", delegate
				{
					ELayer.ui.AddLayer<LayerQuestBoard>();
				}, 20, auto: true);
				page.Add(t2, "hire", "", delegate
				{
					ELayer.ui.AddLayer<LayerHire>();
				}, 20, auto: true);
			}
			if (t2.trait is TraitGacha)
			{
				page.Add(t2, "gacha", "", delegate
				{
					ELayer.ui.AddLayer<LayerGacha>();
				}, 10, auto: true);
			}
			if (t2.trait.IsFactory)
			{
				page.Add(t2, "craft", "icon_Inspect", delegate
				{
					ELayer.ui.AddLayer<LayerCraft>().SetFactory(t2);
				}, 100, auto: true);
			}
			if (t2.IsInstalled)
			{
				page.Add(t2, "move", "", delegate
				{
					ActionMode.Inspect.Activate(t2);
				});
			}
		}
		return page;
	}

	public void Show(List<Page> _pages, Mode _mode)
	{
		pages = _pages;
		mode = _mode;
		switch (mode)
		{
		case Mode.Map:
			point.Set(Scene.HitPoint);
			break;
		case Mode.EloMap:
		{
			Page page = new Page();
			pages = new List<Page>();
			pages.Add(page);
			page.Add(null, "test1", "", delegate
			{
			});
			page.Add(null, "test2", "", delegate
			{
			});
			break;
		}
		}
		foreach (Page page2 in pages)
		{
			ButtonGrid buttonGrid = (ButtonGrid)(page2.button = menu.Add() as ButtonGrid);
			buttonGrid.SetObject(page2.items[0].target);
			Page _page = page2;
			buttonGrid.onClick.AddListener(delegate
			{
				ShowPage(_page);
				SE.Click();
			});
			buttonGrid.RebuildLayout(recursive: true);
		}
		menu.Show();
		ShowPage(0);
	}

	public void ShowPage(int index)
	{
		ShowPage(pages[index]);
	}

	public void ShowPage(Page page)
	{
		menu.Clear();
		foreach (Item item in page.items)
		{
			UIButton uIButton = menu.Add();
			uIButton.icon.sprite = SpriteSheet.Get(item.idSprite.IsEmpty("icon_" + item.text)) ?? uIButton.icon.sprite;
			uIButton.mainText.SetText((item.textFunc != null) ? item.textFunc() : item.text.lang());
			uIButton.onClick.AddListener(delegate
			{
				item.action();
				if (item.reload)
				{
					Reload();
					ShowPage(page);
				}
			});
			uIButton.RebuildLayout(recursive: true);
		}
		Chara chara = Target as Chara;
		bool flag = chara?.IsHomeMember() ?? false;
		windowChara.SetActive(flag);
		if (flag)
		{
			windowChara.SetChara(chara);
		}
		menu.Show();
	}

	public void Reload()
	{
	}
}
