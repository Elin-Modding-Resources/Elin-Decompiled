using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildMenu : EMono
{
	public enum Mode
	{
		Build,
		None,
		Hide,
		Area,
		PartialMap
	}

	public GridLayoutGroup gridTab;

	public UISelectableGroup groupTab;

	public UIDynamicList list;

	public UIDynamicList listArea;

	public Dictionary<string, int> catDic = new Dictionary<string, int>();

	public UIRecipeInfo info1;

	public UIRecipeInfo info2;

	public RectTransform transOption;

	public RectTransform transMain;

	public Vector2 posInfo2;

	public Vector2 posInfo1;

	public Vector2 posOption;

	public CanvasGroup cg;

	public float hideSpeed;

	public float intervalSearch;

	public bool moveInfo1;

	public TerrainMenu terrainMenu;

	public InputField inputSearch;

	public string currentCat;

	public string lastCat;

	public UIButton buttonClearSearch;

	[NonSerialized]
	public string[] categories = new string[20]
	{
		"wall", "floor", "foundation", "door", "furniture", "storage", "spot", "mount", "facility", "tool",
		"deco", "mech", "light", "junk", "ext", "goods", "food", "obj", "other", "area"
	};

	[NonSerialized]
	public ButtonRecipe lastButtonRecipe;

	public Dictionary<string, int> counts = new Dictionary<string, int>();

	public Dictionary<string, UIButton> tabs = new Dictionary<string, UIButton>();

	private Vector2 orgPos;

	private Vector2 orgPosOption;

	public static BuildMenu Instance;

	public UIButton buttonUndo;

	public static bool dirtyCat;

	private float timerSearch;

	public string lastSearch;

	public HashSet<Recipe> searchRecipes = new HashSet<Recipe>();

	private bool firstRefresh = true;

	public Mode mode => EMono.scene.actionMode.buildMenuMode;

	public static void Toggle()
	{
		EMono.player.hotbars.ResetHotbar(3);
		EMono.player.hotbars.bars[3].dirty = true;
		EMono.player.hotbars.ResetHotbar(4);
		EMono.player.hotbars.bars[4].dirty = true;
		if (ActionMode.LastBuildMode == ActionMode.Build || ActionMode.LastBuildMode == ActionMode.Picker)
		{
			ActionMode.Inspect.Activate();
		}
		else if (ActionMode.LastBuildMode != null)
		{
			ActionMode.LastBuildMode.Activate();
		}
		else if (EMono.debug.godBuild || (EMono.Branch != null && EMono.Branch.elements.Has(4006)))
		{
			ActionMode.Inspect.Activate();
		}
		else
		{
			ActionMode.Terrain.Activate();
		}
	}

	public static void Activate()
	{
		if (!Instance)
		{
			Instance = Util.Instantiate<BuildMenu>("UI/BuildMenu/BuildMenu", EMono.ui);
			Instance.Init();
		}
		if (EMono.debug.godBuild)
		{
			EMono._map.RevealAll();
		}
		RecipeManager.BuildList();
		Instance.OnActivate();
		EMono.screen.focusPos = null;
	}

	public void OnActivate()
	{
		if (EMono.debug.godBuild && EMono.debug.GetDebugContainer() == null)
		{
			EMono.debug.EnableDebugResource();
		}
		EMono.ui.HideFloats();
		this.SetActive(mode != Mode.None);
		transMain.SetActive(mode == Mode.Build && (EMono.debug.godBuild || EMono._zone.elements.Has(4005) || (EMono._zone is Zone_Tent && EMono.pc.homeBranch.elements.Has(4005))));
		switch (mode)
		{
		case Mode.Build:
		{
			string cat = EMono.player.pref.lastBuildCategory.IsEmpty("wall");
			SelectCategory(cat);
			Refresh();
			cg.GetComponentInChildren<UIScrollView>()?.RebuildLayout(recursive: true);
			buttonUndo.onClick.RemoveAllListeners();
			buttonUndo.onClick.AddListener(delegate
			{
				EMono._map.tasks.undo.Perform();
			});
			buttonUndo.SetTooltip("note", delegate(UITooltip tp)
			{
				EMono._map.tasks.undo.WriteNote(tp.note);
			});
			break;
		}
		case Mode.Area:
			RefreshCategoryArea();
			Refresh();
			break;
		}
		if (EMono.debug.enable)
		{
			EMono._map.ResetEditorPos();
		}
		EMono.screen.RefreshAll();
	}

	public static void Deactivate()
	{
		if ((bool)Instance)
		{
			UnityEngine.Object.DestroyImmediate(Instance.gameObject);
			if (EMono.game.altUI)
			{
				EMono._zone.RefreshBGM();
			}
			EMono.ui.ShowFloats();
			EMono.screen.RefreshAll();
		}
	}

	public void Init()
	{
		info1.Init();
		info2.Init();
		info2.transform.position = info1.transform.position;
		orgPos = info1.Rect().anchoredPosition;
		orgPosOption = transOption.anchoredPosition;
		for (int i = 0; i < categories.Length; i++)
		{
			if (!categories[i].IsEmpty())
			{
				catDic.Add(categories[i], i);
			}
		}
		UIButton t = gridTab.CreateMold<UIButton>();
		for (int j = 0; j < categories.Length - 1; j++)
		{
			int _i = j;
			UIButton uIButton = Util.Instantiate(t, gridTab);
			uIButton.onClick.AddListener(delegate
			{
				ClearSearch(refresh: false);
				SelectCategory(categories[_i]);
			});
			uIButton.mainText.SetText(("cat_" + categories[j]).lang());
			tabs[categories[j]] = uIButton;
			if (categories[j].IsEmpty())
			{
				uIButton.mainText.SetActive(enable: false);
				uIButton.interactable = false;
			}
		}
		groupTab.Init(-1);
		EMono.ui.hud.frame.SetActive(EMono.game.altUI);
		inputSearch.onValueChanged.AddListener(Search);
		inputSearch.onSubmit.AddListener(Search);
		if (EMono.game.altUI)
		{
			EMono.Sound.SwitchPlaylist(EMono.Sound.playlistBuild);
		}
	}

	public void Search(string s)
	{
		s = s.ToLower();
		buttonClearSearch.SetActive(inputSearch.text != "");
		if (!(s == lastSearch))
		{
			timerSearch = intervalSearch;
			lastSearch = s;
		}
	}

	private void _Search()
	{
		string s = lastSearch;
		RecipeManager.BuildList();
		HashSet<Recipe> newRecipes = new HashSet<Recipe>();
		if (!s.IsEmpty())
		{
			foreach (RecipeSource item in RecipeManager.list)
			{
				if (item.noListing || item.isBridgePillar || (!EMono.debug.godBuild && !item.alwaysKnown && !EMono.player.recipes.knownRecipes.ContainsKey(item.id)) || (!item.row.GetSearchName(jp: false).Contains(s) && !item.row.GetSearchName(jp: true).Contains(s) && !item.id.Contains(s) && !item.row.category.Contains(s)))
				{
					continue;
				}
				for (int i = 0; i < ((item.row.skins == null) ? 1 : (item.row.skins.Length + 1)); i++)
				{
					Recipe recipe = Recipe.Create(item);
					recipe.idSkin = i;
					if (recipe == null)
					{
						Debug.Log(item.Name + "/" + item.id);
					}
					else
					{
						newRecipes.Add(recipe);
					}
				}
			}
			foreach (Thing thing in EMono._map.Stocked.Things)
			{
				if (EMono._map.Stocked.ShouldListAsResource(thing) && (thing.source.name.Contains(s) || thing.source.name_JP.Contains(s) || thing.NameOne.Contains(s)))
				{
					Recipe recipe2 = Recipe.Create(thing);
					if (recipe2 == null)
					{
						Debug.Log(thing.Name + "/" + thing.id);
					}
					else
					{
						newRecipes.Add(recipe2);
					}
				}
			}
			EMono.pc.things.Foreach(delegate(Thing t)
			{
				if (t.trait.CanBeDropped && !t.trait.CanOnlyCarry && (t.source.name.Contains(s) || t.source.name_JP.Contains(s) || t.NameOne.Contains(s)))
				{
					Recipe recipe3 = Recipe.Create(t);
					if (recipe3 == null)
					{
						Debug.Log(t.Name + "/" + t.id);
					}
					else
					{
						newRecipes.Add(recipe3);
					}
				}
			});
		}
		if (!newRecipes.SetEquals(searchRecipes))
		{
			searchRecipes = newRecipes;
			RefreshCategory(currentCat);
		}
		lastSearch = s;
	}

	public void ClearSearch()
	{
		ClearSearch(refresh: true);
	}

	public void ClearSearch(bool refresh)
	{
		SE.Click();
		inputSearch.text = "";
		timerSearch = 0f;
		lastSearch = "";
		if (refresh)
		{
			RefreshCategory(currentCat);
		}
	}

	public static void Show()
	{
		if ((bool)Instance)
		{
			Instance.SetActive(enable: true);
		}
	}

	public static void Hide()
	{
		if ((bool)Instance)
		{
			Instance.SetActive(enable: false);
		}
	}

	private void LateUpdate()
	{
		Refresh();
		if (timerSearch > 0f)
		{
			timerSearch -= Core.delta;
			if (timerSearch <= 0f)
			{
				_Search();
			}
		}
	}

	public void Refresh()
	{
		ButtonGrid buttonGrid = InputModuleEX.GetComponentOf<ButtonGrid>();
		if ((bool)buttonGrid && buttonGrid.recipe == null && buttonGrid.area == null)
		{
			buttonGrid = null;
		}
		bool flag = EMono.scene.actionMode.ShouldHideBuildMenu || (EMono.scene.actionMode != ActionMode.Build && EMono.scene.actionMode != ActionMode.Inspect && EMono.scene.actionMode != ActionMode.EditArea);
		cg.SetActive(!flag, delegate(bool enabled)
		{
			if (enabled)
			{
				list.RefreshHighlight();
				groupTab.RefreshButtons();
			}
		});
		groupTab.SetActive(EMono.scene.actionMode != ActionMode.EditArea);
		transOption.anchoredPosition = (groupTab.gameObject.activeInHierarchy ? orgPosOption : posOption);
		if (info1.hideMode != flag || firstRefresh)
		{
			info1.hideMode = flag;
			if (moveInfo1)
			{
				info1.Rect().anchoredPosition = (flag ? posInfo1 : orgPos);
			}
		}
		if ((bool)buttonGrid && !flag)
		{
			info2.transform.position = buttonGrid.transform.position;
			info2.Rect().anchoredPosition = new Vector2(info2.Rect().anchoredPosition.x + posInfo2.x, posInfo2.y);
			if (buttonGrid.area != null)
			{
				info2.SetArea(buttonGrid.area);
			}
			else
			{
				info2.SetRecipe(buttonGrid.recipe);
			}
			info2.SetActive(enable: true);
		}
		else
		{
			info2.SetActive(enable: false);
		}
		firstRefresh = false;
	}

	public void SelectCategory(string cat)
	{
		if (dirtyCat || !(currentCat == cat))
		{
			dirtyCat = false;
			groupTab.Select(catDic[cat]);
			RefreshCategory(cat);
		}
	}

	public void Select(AM_Picker.Result r)
	{
		EInput.Consume(consumeAxis: false, 10);
		Debug.Log(EInput.skipFrame);
		ClearSearch(refresh: false);
		RefreshCategory(r.source.recipeCat);
		foreach (object @object in list.objects)
		{
			Recipe recipe = @object as Recipe;
			if (r.source == recipe.source && (r.thing == null || r.thing.idSkin == recipe.idSkin))
			{
				int index = list.GetIndex(recipe);
				if (index != -1)
				{
					list.dsv.scrollByItemIndex(index);
					list.Refresh();
				}
				break;
			}
		}
		ButtonGrid[] componentsInChildren = list.GetComponentsInChildren<ButtonGrid>();
		foreach (ButtonGrid buttonGrid in componentsInChildren)
		{
			if (buttonGrid.recipe.source == r.source && (r.thing == null || r.thing.idSkin == buttonGrid.recipe.idSkin))
			{
				info1.lastMats[buttonGrid.recipe.id] = r.mat.id;
				buttonGrid.onClick.Invoke();
				break;
			}
		}
	}

	public void Unselect()
	{
		info1.SetRecipe(null);
		list.Select(null);
	}

	public void RefreshCategoryArea()
	{
		UIDynamicList uIDynamicList = listArea;
		List<Area> list = new List<Area>();
		foreach (SourceArea.Row row in EMono.sources.areas.rows)
		{
			Area area = Area.Create(row.id);
			area.data.name = null;
			list.Add(area);
		}
		uIDynamicList.Clear();
		object selected = null;
		uIDynamicList.callbacks = new UIList.Callback<Area, ButtonGrid>
		{
			onClick = delegate(Area a, ButtonGrid b)
			{
				SE.Click();
				ActionMode.CreateArea.SetArea(a);
				if (EMono.scene.actionMode != ActionMode.CreateArea)
				{
					ActionMode.CreateArea.Activate();
				}
				EMono.ui.hud.hint.Refresh();
				info1.SetArea(a);
			},
			onRedraw = delegate(Area a, ButtonGrid b, int i)
			{
				b.SetRecipe(a);
				if (info1.area != null && a.source == info1.area.source)
				{
					selected = a;
				}
			}
		};
		foreach (Area item in list)
		{
			if (item.isListable)
			{
				uIDynamicList.Add(item);
			}
		}
		uIDynamicList.List();
	}

	public void RefreshCategory(string cat)
	{
		cat = cat.IsEmpty(categories[0]);
		EMono.player.pref.lastBuildCategory = cat;
		currentCat = cat;
		list.Clear();
		object selected = null;
		counts.Clear();
		list.callbacks = new UIList.Callback<Recipe, ButtonGrid>
		{
			onClick = delegate(Recipe a, ButtonGrid b)
			{
				SE.Click();
				EMono.ui.hud.hint.Refresh();
				EMono.player.pref.lastBuildRecipe = a.source.id;
				if (a.UseStock && (a.ingredients[0].uid == 0 || a.ingredients[0].thing == null))
				{
					Debug.LogError("expection: invalid ingredient:" + a.ingredients[0].uid);
				}
				if (info1.recipe != a && !a.UseStock)
				{
					foreach (Recipe.Ingredient ingredient in a.ingredients)
					{
						ingredient.SetThing();
					}
				}
				if (a.UseStock && (a.ingredients[0].uid == 0 || a.ingredients[0].thing == null))
				{
					Debug.LogError("expection: invalid ingredient:" + a.ingredients[0].uid);
				}
				info1.SetRecipe(a);
				if (a.UseStock && (a.ingredients[0].uid == 0 || a.ingredients[0].thing == null))
				{
					Debug.LogError("expection: invalid ingredient:" + a.ingredients[0].uid);
				}
				a.OnChangeIngredient();
				ActionMode.Build.StartBuild(a, () => list.GetComp<ButtonGrid>(a));
				info1.SetRecipe(a);
				list.Select(a);
				if (a.UseStock && (a.ingredients[0].uid == 0 || a.ingredients[0].thing == null))
				{
					Debug.LogError("expection: invalid ingredient:" + a.ingredients[0].uid);
				}
			},
			onRedraw = delegate(Recipe a, ButtonGrid b, int i)
			{
				b.SetActive(a != null);
				if (a != null)
				{
					b.SetRecipe(a);
					if (info1.recipe != null && a.id == info1.recipe.id)
					{
						selected = a;
					}
					a.BuildIngredientList();
				}
			},
			onList = delegate
			{
				string[] array = categories;
				foreach (string key in array)
				{
					counts[key] = 0;
				}
				if (inputSearch.text != "")
				{
					foreach (Recipe searchRecipe in searchRecipes)
					{
						if (searchRecipe.UseStock)
						{
							Thing thing = searchRecipe.ingredients[0].thing;
							if (thing == null || thing.isDestroyed || thing.ExistsOnMap || (thing.parentCard != null && thing.parentCard.c_lockLv > 0))
							{
								continue;
							}
						}
						list.Add(searchRecipe);
					}
					return;
				}
				foreach (RecipeSource item in RecipeManager.list)
				{
					if (EMono.debug.godBuild || !(item.row.GetAlias != "block_invisi") || (!item.noListing && !item.row.tileType.EditorTile && EMono.player.recipes.IsKnown(item.id) && (item.row.factory.Length == 0 || !(item.row.factory[0] == "none"))))
					{
						counts[item.recipeCat]++;
						if (!(item.recipeCat != cat))
						{
							for (int k = 0; k < ((item.row.skins != null) ? item.row.skins.Length : 0) + 1; k++)
							{
								Recipe recipe = Recipe.Create(item);
								recipe.idSkin = k;
								list.Add(recipe);
							}
						}
					}
				}
				if (!EMono.debug.hidePCItemsInBuild)
				{
					foreach (Thing thing2 in EMono._map.Stocked.Things)
					{
						if (EMono._map.Stocked.ShouldListAsResource(thing2))
						{
							counts[thing2.trait.RecipeCat]++;
							if (thing2.trait.RecipeCat == cat)
							{
								Card rootCard = thing2.GetRootCard();
								if (rootCard != null && rootCard.c_lockLv == 0)
								{
									list.Add(thing2.trait.GetBuildModeRecipe());
								}
							}
						}
					}
					EMono.pc.things.Foreach(delegate(Thing t)
					{
						if (!t.trait.CanOnlyCarry && t.trait.CanBeDropped && !t.c_isImportant)
						{
							counts[t.trait.RecipeCat]++;
							if (t.trait.RecipeCat == cat && t.invY != 1)
							{
								list.Add(t.trait.GetBuildModeRecipe());
							}
						}
					});
				}
				if (cat == "other")
				{
					list.objects.Sort((object a, object b) => string.Compare((a as Recipe).id, (b as Recipe).id));
				}
				else if (cat == "wall" || cat == "floor" || cat == "foundation" || cat == "obj")
				{
					list.objects.Sort((object a, object b) => (a as Recipe).renderRow.sort - (b as Recipe).renderRow.sort);
				}
				else
				{
					list.objects.Sort(delegate(object a, object b)
					{
						Recipe recipe2 = a as Recipe;
						Recipe recipe3 = b as Recipe;
						int num = recipe2.renderRow.sort - recipe3.renderRow.sort;
						if (num == 0)
						{
							RecipeCard recipeCard = recipe2 as RecipeCard;
							RecipeCard recipeCard2 = recipe3 as RecipeCard;
							num = ((recipeCard == null || recipeCard2 == null) ? string.Compare(recipe2.id, recipe3.id) : string.Compare(recipeCard.sourceCard._origin.IsEmpty(recipeCard.sourceCard.id), recipeCard2.sourceCard._origin.IsEmpty(recipeCard2.sourceCard.id)));
							if (num == 0)
							{
								num = (recipe2.IngAsProduct ? recipe2.ingredients[0].uid : 0) - (recipe3.IngAsProduct ? recipe3.ingredients[0].uid : 0);
							}
						}
						return num;
					});
				}
				array = categories;
				foreach (string text in array)
				{
					if (!(text == "area") && !(text == ""))
					{
						if (counts[text] == 0)
						{
							tabs[text].subText.text = "";
						}
						else
						{
							tabs[text].subText.text = counts[text].ToString() ?? "";
						}
					}
				}
			}
		};
		list.List();
		if (lastCat != currentCat)
		{
			list.Scroll();
		}
		groupTab.Select(catDic[cat]);
		if (selected != null)
		{
			list.Select(selected);
		}
		lastCat = currentCat;
		list.RebuildLayoutTo<BuildMenu>();
	}

	public void OnClickPicker()
	{
		ActionMode.Picker.Activate();
	}
}
