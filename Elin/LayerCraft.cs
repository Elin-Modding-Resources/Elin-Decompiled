using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LayerCraft : LayerBaseCraft
{
	public static LayerCraft Instance;

	public UIDynamicList list;

	public UIRecipeInfo info1;

	public UIRecipeInfo info2;

	public Vector3 posInfo;

	public Thing factory;

	public Thing product;

	public Recipe recipe;

	public UIButton toggleRepeat;

	public UIButton toggleCraftTo;

	public UIButton buttonTrack;

	public UIInputText inputNum;

	public UIInputText moldInputNum;

	public List<RecipeSource> newRecipes = new List<RecipeSource>();

	public Window windowList;

	public bool showInfo;

	private bool pickaxeCrafted;

	private bool workbenchCrafted;

	public UIButton buttonRefuel;

	public UIButton buttonAutoRefuel;

	public Transform transFuel;

	public UIText textFuel;

	public UIText textQualityInvalid;

	public UIText textQualityValid;

	public Image imageQualityReq;

	public Image imageQualityCurrent;

	public float waitComplete;

	public float qualityBarWidth;

	private int wcount;

	public ButtonGrid lastB;

	private bool tabBuilt;

	public override void RefreshCurrentGrid()
	{
		RefreshRecipe(first: true);
	}

	public override void ClearButtons()
	{
		RefreshRecipe(first: true);
	}

	public override string GetTextHeader(Window window)
	{
		return null;
	}

	public override void OnAfterInit()
	{
		info1.searchMode = StockSearchMode.AroundPC;
		info1.ddList.pivot.SetActive(enable: true);
		Instance = this;
	}

	public override void OnKill()
	{
		if (workbenchCrafted)
		{
			Tutorial.Play("middle_click");
		}
		if (pickaxeCrafted)
		{
			Tutorial.Play("hardness");
		}
		string key = ((factory == null) ? "hand" : factory.id);
		ELayer.player.lastRecipes[key] = recipe.id;
	}

	public void SetFactory(Thing t)
	{
		factory = t;
		RefreshCategory("all", first: true);
		bool flag = t != null && t.trait.IsRequireFuel;
		transFuel.SetActive(flag);
		if (!flag)
		{
			return;
		}
		buttonRefuel.SetOnClick(delegate
		{
			foreach (Window window in windows)
			{
				window.SetInteractable(enable: false, 0f);
			}
			LayerDragGrid.Create(new InvOwnerRefuel(t)).SetOnKill(delegate
			{
				if (!isDestroyed)
				{
					OnEndCraft();
				}
			});
		});
		buttonAutoRefuel.SetOnClick(delegate
		{
			SE.Click();
			t.autoRefuel = !t.autoRefuel;
			RefreshProduct();
		});
	}

	public void OnClickCraft()
	{
		Dictionary<Thing, int> dictionary = new Dictionary<Thing, int>();
		foreach (Recipe.Ingredient ingredient in recipe.ingredients)
		{
			if (ingredient.thing != null)
			{
				if (!dictionary.ContainsKey(ingredient.thing))
				{
					dictionary.Add(ingredient.thing, 0);
				}
				dictionary[ingredient.thing] += ingredient.req * inputNum.Num;
			}
		}
		foreach (KeyValuePair<Thing, int> item in dictionary)
		{
			if (item.Key.Num < item.Value)
			{
				SE.Beep();
				Msg.Say("craftDupError");
				return;
			}
		}
		if (inputNum.Num == 0)
		{
			SE.Beep();
			return;
		}
		TraitCrafter traitCrafter = factory?.trait as TraitCrafter;
		if (traitCrafter == null)
		{
			traitCrafter = Trait.SelfFactory;
			traitCrafter.owner = ELayer.pc;
		}
		(traitCrafter as TraitFactory).recipe = recipe;
		switch (recipe.id)
		{
		case "workbench":
			workbenchCrafted = true;
			break;
		case "axe":
		case "hammer":
		case "pickaxe":
			pickaxeCrafted = true;
			break;
		}
		recipe.SaveLastIngredients();
		ELayer.pc.SetAI(new AI_UseCrafter
		{
			crafter = traitCrafter,
			layer = this,
			recipe = recipe,
			num = inputNum.Num
		});
		ActionMode.Adv.SetTurbo();
		base.gameObject.SetActive(value: false);
	}

	public override List<Thing> GetTargets()
	{
		List<Thing> list = new List<Thing>();
		foreach (Recipe.Ingredient ingredient in recipe.ingredients)
		{
			if (!ingredient.optional || (ingredient.thing != null && !ingredient.thing.isDestroyed))
			{
				list.Add(ingredient.thing);
			}
		}
		return list;
	}

	public override int GetReqIngredient(int index)
	{
		return recipe.ingredients[index].req * inputNum.Num;
	}

	public override void OnEndCraft()
	{
		OnCompleteCraft();
	}

	public void OnCompleteCraft()
	{
		CancelInvoke("WaitUntilIdle");
		EInput.haltInput = true;
		TweenUtil.Tween(waitComplete, null, delegate
		{
			EInput.haltInput = false;
			base.gameObject.SetActive(value: true);
			foreach (Window window in windows)
			{
				window.SetInteractable(enable: true);
			}
			list.Redraw();
			RefreshRecipe(first: true);
			windowList.groupTab.RefreshButtons();
			list.Select((Recipe r) => recipe.id == r.id);
		});
	}

	public void WaitUntilIdle()
	{
		if (!ActionMode.Adv.IsActive || ELayer.pc.HasNoGoal)
		{
			CancelInvoke();
			Close();
		}
	}

	private void Update()
	{
		float axis = Input.GetAxis("Mouse ScrollWheel");
		if (Input.GetMouseButton(0))
		{
			wcount = 2;
		}
		if (axis != 0f)
		{
			wcount = 2;
		}
		else if (wcount > 0)
		{
			wcount--;
		}
		else
		{
			RefreshInfo();
		}
	}

	public void RefreshInfo()
	{
		ButtonGrid buttonGrid = InputModuleEX.GetComponentOf<ButtonGrid>();
		if ((bool)buttonGrid && buttonGrid.recipe == null)
		{
			buttonGrid = null;
		}
		if (!(buttonGrid == lastB))
		{
			lastB = buttonGrid;
		}
	}

	public void RefreshCategory(string cat, bool first = false)
	{
		Dictionary<string, int> cats = new Dictionary<string, int>();
		RecipeManager.BuildList();
		list.callbacks = new UIList.Callback<Recipe, ButtonGrid>
		{
			onClick = delegate(Recipe a, ButtonGrid b)
			{
				if (recipe != null)
				{
					recipe.SaveLastIngredients();
				}
				recipe = a;
				SE.Play("click_recipe");
				RefreshRecipe(first: true);
				ELayer.player.recipes.hoveredRecipes.Add(a.id);
				b.Dettach("recipe_new");
				list.Select(a);
			},
			onRedraw = delegate(Recipe a, ButtonGrid b, int i)
			{
				if (a.ingredients.Count == 0)
				{
					a.BuildIngredientList();
				}
				b.SetCraftRecipe(a, ButtonGrid.Mode.Recipe);
				if (!ELayer.player.recipes.hoveredRecipes.Contains(a.id))
				{
					b.Attach("recipe_new", rightAttach: false);
				}
				if (a.HasFirstTimeBonus())
				{
					b.Attach("recipe_firstTime", rightAttach: false).GetComponent<Image>().SetAlpha((a.idSkin == 0) ? 1f : 0.5f);
				}
			},
			onList = delegate
			{
				newRecipes.Clear();
				foreach (RecipeSource item in ELayer.player.recipes.ListSources(factory, newRecipes))
				{
					if (cat == "all" || item.row.Category.IsChildOf(cat))
					{
						Recipe o2 = Recipe.Create(item);
						list.Add(o2);
						if (item.row.skins != null && item.row.skins.Length != 0)
						{
							for (int j = 0; j < item.row.skins.Length; j++)
							{
								o2 = Recipe.Create(item);
								o2.idSkin = j + 1;
								list.Add(o2);
							}
						}
					}
					SourceCategory.Row row = item.row.Category.GetSecondRoot();
					if (row.id != "lightsource" && row.IsChildOf("armor"))
					{
						row = ELayer.sources.categories.map["armor"];
					}
					if (!cats.ContainsKey(row.id))
					{
						cats.Add(row.id, 1);
					}
					else
					{
						cats[row.id]++;
					}
				}
				list.objects.Sort((object a, object b) => (b as Recipe).GetSortVal() - (a as Recipe).GetSortVal());
				_ = newRecipes.Count;
			},
			onSort = (Recipe a, UIList.SortMode m) => a.GetSortVal()
		};
		list.sortMode = UIList.SortMode.ByLevel;
		list.List();
		if (cats.Count > 1 && !tabBuilt)
		{
			windowList.AddTab("all", null, delegate
			{
				RefreshCategory("all");
			});
			foreach (KeyValuePair<string, int> c in cats)
			{
				windowList.AddTab(ELayer.sources.categories.map[c.Key].GetName() + "(" + c.Value + ")", null, delegate
				{
					RefreshCategory(c.Key);
				});
			}
			tabBuilt = true;
			windowList.BuildTabs(0);
		}
		list.Scroll();
		if (list.rows.Count <= 0)
		{
			return;
		}
		object o = list.rows[0].objects[0];
		if (first)
		{
			string key = ((factory == null) ? "hand" : factory.id);
			if (ELayer.player.lastRecipes.ContainsKey(key))
			{
				string text = ELayer.player.lastRecipes[key];
				foreach (object @object in list.objects)
				{
					if ((@object as Recipe).id == text)
					{
						o = @object;
						break;
					}
				}
			}
			list.Scroll(o);
			if (!list.Select(o, invoke: true))
			{
				list.Scroll();
				list.Select(list.rows[0].objects[0]);
			}
		}
		else
		{
			list.Select(o, invoke: true);
		}
	}

	public void OnChangeIngredient()
	{
		RefreshRecipe(first: true);
	}

	public void RefreshRecipe(bool first)
	{
		foreach (Recipe.Ingredient ingredient in recipe.ingredients)
		{
			if (info1.recipe != recipe || (!ingredient.optional && (ingredient.thing == null || ingredient.thing.isDestroyed)))
			{
				ingredient.SetThing();
			}
		}
		if (first)
		{
			toggleCraftTo.SetToggle(isOn: false);
			toggleRepeat.SetToggle(isOn: false);
			inputNum.Num = 1;
			info1.recipe = null;
		}
		info1.factory = factory;
		info1.SetRecipe(recipe);
		recipe.OnChangeIngredient();
		RefreshProduct();
		RefreshTrackButton();
	}

	public void RefreshTrackButton()
	{
		QuestTrackCraft quest = recipe.GetQuestTrack();
		buttonTrack.SetOnClick(delegate
		{
			recipe.ToggleTrack(quest);
			RefreshTrackButton();
		});
		buttonTrack.icon.SetActive(quest != null && quest.idRecipe == recipe.id);
	}

	public void RefreshInputNum()
	{
		inputNum.SetMinMax(1, recipe.GetMaxCount());
		inputNum.onValueChanged = delegate
		{
			RefreshProduct();
		};
		inputNum.Validate();
	}

	public void RefreshProduct()
	{
		RefreshInputNum();
		ELayer.screen.tileSelector.summary.countValid = inputNum.Num;
		ELayer.screen.tileSelector.summary.factory = factory;
		info1.RefreshBalance();
		if (factory != null)
		{
			textFuel.text = "craftFuel".lang(((int)((float)factory.c_charges / (float)factory.trait.MaxFuel * 100f)).ToString() ?? "");
			buttonAutoRefuel.mainText.text = (factory.autoRefuel ? "On" : "Off");
			buttonAutoRefuel.icon.SetAlpha(factory.autoRefuel ? 1f : 0.4f);
		}
		RefreshQuality();
		List<Thing> list = new List<Thing>();
		foreach (Recipe.Ingredient ingredient in recipe.ingredients)
		{
			if (ingredient.thing != null)
			{
				list.Add(ingredient.thing);
			}
		}
		Thing thing = recipe.Craft(BlessedState.Normal, sound: false, list, null, model: true);
		thing.SetNum(inputNum.Num);
		if (thing.sockets != null)
		{
			thing.sockets.Clear();
		}
		if (thing.IsEquipmentOrRangedOrAmmo)
		{
			foreach (Element item in thing.elements.dict.Values.ToList())
			{
				_ = item;
			}
		}
		info1.buttonProduct.SetCard(thing);
		thing.WriteNote(info1.note, null, IInspect.NoteMode.Product, recipe);
		product = thing;
	}

	public bool IsQualityMet()
	{
		return true;
	}

	public void RefreshQuality()
	{
	}

	public void OnClickExamine()
	{
		ELayer.ui.AddLayer<LayerInfo>().Set(product);
	}
}
