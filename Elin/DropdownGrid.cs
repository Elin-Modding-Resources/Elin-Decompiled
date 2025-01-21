using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownGrid : EMono
{
	public static bool IsActive;

	public static DropdownGrid Instance;

	public UIList list;

	public UIList listDrop;

	public RectTransform rectDrop;

	public RectTransform rectDropContent;

	public RectTransform pivot;

	public Recipe recipe;

	public Image icon;

	public SoundData soundPop;

	public Action onValueChange;

	public int slots = 4;

	public Vector3 fixPos;

	public StockSearchMode searchMode;

	public UIRecipeInfo recipeInfo;

	[NonSerialized]
	public Dictionary<string, int> lastMats = new Dictionary<string, int>();

	private void Awake()
	{
		rectDrop.SetActive(enable: false);
	}

	private void Update()
	{
		rectDrop.SetActive(IsActive);
	}

	public void Build(List<GridItem> items)
	{
		list.Clear();
		list.callbacks = new UIList.Callback<GridItem, ButtonGrid>
		{
			onInstantiate = delegate(GridItem a, ButtonGrid b)
			{
				a.SetButton(b);
			},
			onClick = delegate(GridItem a, ButtonGrid b)
			{
				a.OnClick(b);
			},
			onRedraw = delegate(GridItem a, ButtonGrid b, int i)
			{
				a.SetButton(b);
			}
		};
		for (int j = 0; j < slots; j++)
		{
			if (j < items.Count)
			{
				list.Add(items[j]);
			}
			else
			{
				list.Add(new GridItem());
			}
		}
		list.Refresh();
	}

	public List<Thing> ListIngredients(Recipe.Ingredient ingredient, StockSearchMode searchMode)
	{
		return EMono._map.Stocked.ListThingStack(ingredient, searchMode).list;
	}

	public void BuildIngredients(Recipe _recipe, Image _icon, Action _onValueChange, StockSearchMode _searchMode)
	{
		recipe = _recipe;
		icon = _icon;
		list.Clear();
		onValueChange = _onValueChange;
		searchMode = _searchMode;
		if (recipe == null)
		{
			return;
		}
		list.callbacks = new UIList.Callback<Recipe.Ingredient, ButtonGrid>
		{
			onInstantiate = delegate(Recipe.Ingredient ingredient, ButtonGrid b)
			{
				List<Thing> things;
				if (ingredient.id.IsEmpty())
				{
					b.SetIngredient(recipe, ingredient);
				}
				else
				{
					bool flag = false;
					List<int> list = EMono.player.recipes.lastIngredients.TryGetValue(recipe.id);
					int num = recipe.ingredients.IndexOf(ingredient);
					if (num != -1 && list != null && list.Count > num)
					{
						int uid = list[num];
						if (uid != 0)
						{
							Thing thing = EMono.pc.things.Find((Thing t) => t.uid == uid) ?? EMono._map.Stocked.Find(uid);
							if (thing != null && ingredient.IsValidIngredient(thing) && (!(thing.parent is Card card) || card.GetWindowSaveData() == null || !card.GetWindowSaveData().excludeCraft))
							{
								ingredient.SetThing(thing);
								flag = true;
							}
						}
					}
					if (!flag)
					{
						things = ListIngredients(ingredient, searchMode);
						if (things.Count == 0)
						{
							if (EMono.debug.godBuild && ingredient.thing == null)
							{
								Thing thing2 = ThingGen.Create(ingredient.IdThing, recipe.DefaultMaterial.alias).SetNum(99);
								things.Add(thing2);
								ingredient.SetThing(thing2);
							}
							else
							{
								ingredient.SetThing();
								things.Insert(0, null);
							}
						}
						else if (!ingredient.optional)
						{
							if (ingredient.thing == null)
							{
								int num2 = lastMats.TryGetValue(recipe.id, -1);
								if (num2 != -1)
								{
									foreach (Thing item in things)
									{
										if (item.material.id == num2 && item.Num >= ingredient.req)
										{
											ingredient.SetThing(item);
											break;
										}
									}
								}
								if (ingredient.thing == null)
								{
									SourceMaterial.Row defaultMaterial = recipe.DefaultMaterial;
									foreach (Thing item2 in things)
									{
										if (item2.material.id == defaultMaterial.id && item2.Num >= ingredient.req)
										{
											ingredient.SetThing(item2);
											break;
										}
									}
									if (EMono.debug.godBuild && ingredient.thing == null)
									{
										Thing thing3 = (ingredient.useCat ? ThingGen.CreateFromCategory(ingredient.id) : ThingGen.Create(ingredient.id, defaultMaterial.alias)).SetNum(99);
										things.Add(thing3);
										ingredient.SetThing(thing3);
									}
								}
								if (ingredient.thing == null)
								{
									ingredient.SetThing(FindMax());
								}
							}
							else
							{
								bool flag2 = true;
								foreach (Thing item3 in things)
								{
									if (ingredient.thing == item3)
									{
										flag2 = false;
										break;
									}
								}
								if (flag2 && ingredient.thing == null)
								{
									ingredient.SetThing(FindMax());
								}
							}
						}
					}
					b.SetIngredient(recipe, ingredient);
					b.onClick.RemoveAllListeners();
					b.onClick.AddListener(delegate
					{
						List<Thing> list2 = ListIngredients(ingredient, searchMode);
						if (list2.Count == 0 || list2[0] == null)
						{
							SE.Beep();
						}
						else if ((bool)rectDrop)
						{
							Activate(ingredient, list2);
						}
					});
				}
				Thing FindMax()
				{
					if (things.Count == 0)
					{
						return null;
					}
					int num3 = 0;
					Thing result = null;
					foreach (Thing item4 in things)
					{
						if (item4.Num > num3)
						{
							num3 = item4.Num;
							result = item4;
						}
					}
					return result;
				}
			},
			onRedraw = delegate(Recipe.Ingredient a, ButtonGrid b, int i)
			{
				b.SetIngredient(recipe, a);
			}
		};
		for (int j = 0; j < slots; j++)
		{
			if (j < recipe.ingredients.Count)
			{
				list.Add(recipe.ingredients[j]);
				continue;
			}
			list.Add(new Recipe.Ingredient
			{
				id = ""
			});
		}
		list.Refresh();
	}

	public void Redraw()
	{
		list.Redraw();
	}

	public void Activate(Recipe.Ingredient ingredient, List<Thing> things)
	{
		IsActive = true;
		Instance = this;
		rectDrop.SetActive(enable: true);
		rectDrop.SetParent(EMono.ui.transform);
		rectDrop.sizeDelta = new Vector2(0f, 0f);
		rectDrop.anchoredPosition = Vector3.zero;
		listDrop.callbacks = new UIList.Callback<Thing, ButtonGrid>
		{
			onClick = delegate(Thing a, ButtonGrid b)
			{
				SE.Resource();
				ingredient.SetThing(a);
				recipe.SaveLastIngredients();
				OnChangeIngredient();
				if (onValueChange != null)
				{
					onValueChange();
				}
				Deactivate();
				if ((bool)LayerCraft.Instance)
				{
					LayerCraft.Instance.OnChangeIngredient();
				}
			},
			onInstantiate = delegate(Thing a, ButtonGrid b)
			{
				b.SetIngredient(recipe, a);
				b.SetOnClick(delegate
				{
				});
				b.onRightClick = delegate
				{
				};
			}
		};
		listDrop.Clear();
		foreach (Thing thing in things)
		{
			listDrop.Add(thing);
		}
		listDrop.Refresh();
		rectDropContent.position = list.transform.position + fixPos;
		if (listDrop.items.Count == 0)
		{
			SE.Beep();
			Deactivate();
		}
		else if ((bool)soundPop)
		{
			soundPop.Play();
		}
	}

	public void OnChangeIngredient()
	{
		if (recipe == null)
		{
			return;
		}
		recipe.OnChangeIngredient();
		if ((bool)rectDrop)
		{
			if ((bool)icon)
			{
				recipe.renderRow.SetImage(icon, null, recipe.renderRow.GetColorInt(recipe.GetColorMaterial()), setNativeSize: true, 0, recipe.idSkin);
			}
			list.Redraw();
		}
	}

	public void Deactivate()
	{
		IsActive = false;
		if (rectDrop != null)
		{
			rectDrop.SetActive(enable: false);
		}
	}

	public void OnDestroy()
	{
		IsActive = false;
		UnityEngine.Object.DestroyImmediate(rectDrop.gameObject);
	}

	public void TrySelect(int a)
	{
		ButtonGrid componentOf = InputModuleEX.GetComponentOf<ButtonGrid>();
		if (!componentOf || componentOf.ing == null)
		{
			return;
		}
		Recipe.Ingredient ing = componentOf.ing;
		List<Thing> list = EMono._map.Stocked.ListThingStack(ing, searchMode).list;
		int num = list.IndexOf(ing.thing);
		if (num != -1)
		{
			SE.Resource();
			num += a;
			if (num < 0)
			{
				num = 0;
			}
			if (num >= list.Count)
			{
				num = list.Count - 1;
			}
			ing.SetThing(list[num]);
			OnChangeIngredient();
			TooltipManager.Instance.ResetTooltips();
			componentOf.ShowTooltip();
			if ((bool)recipeInfo)
			{
				recipeInfo.RefreshImages();
			}
			if ((bool)LayerCraft.Instance)
			{
				LayerCraft.Instance.RefreshProduct();
			}
		}
	}

	public void Next()
	{
		TrySelect(1);
	}

	public void Prev()
	{
		TrySelect(-1);
	}
}
