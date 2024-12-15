using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIRecipeInfo : EMono
{
	public class RecipeVariation
	{
		public Recipe recipe;

		public int dir;
	}

	public UIText textName;

	public UIText textDetail;

	public UIText textCost;

	public UIText textInfo;

	public UIText textQuality;

	public UIText textReqSkill;

	public UIText textCostSP;

	public Thing factory;

	public Recipe recipe;

	public Area area;

	public UIList listIngredients;

	public UIList listVariations;

	public Image imageArea;

	public Image imageIcon;

	public UINote note;

	public Color colorCost;

	public Color colorPredict;

	public GameObject goMoney;

	public GameObject goIngredients;

	public GameObject goInfo;

	public bool main;

	public bool CraftMode;

	public StockSearchMode searchMode;

	public DropdownGrid ddList;

	public DropdownGrid ddFactory;

	public ButtonGrid buttonProduct;

	[NonSerialized]
	public Dictionary<string, int> lastMats = new Dictionary<string, int>();

	[NonSerialized]
	public bool hideMode = true;

	[NonSerialized]
	public List<DropdownRecipe> dds = new List<DropdownRecipe>();

	public HitSummary summary => EMono.screen.tileSelector.summary;

	public void Init()
	{
		DropdownRecipe.colorCost = colorCost;
		DropdownRecipe.colorPredict = colorPredict;
		SetRecipe(null);
	}

	public void SetArea(Area a)
	{
		if (a == null)
		{
			Clear();
			return;
		}
		if (main)
		{
			goInfo.SetActive(value: true);
			goIngredients.SetActive(value: false);
		}
		if (area != a)
		{
			area = a;
			recipe = null;
			textName.text = a.Name;
			textDetail.SetActive(enable: true);
			textDetail.SetText(a.source.GetDetail());
			note.Clear();
			note.Build();
			if (!main)
			{
				listIngredients.SetActive(enable: false);
			}
			RefreshList();
			if ((bool)imageArea)
			{
				imageArea.SetActive(enable: true);
			}
			if ((bool)listVariations)
			{
				listVariations.SetActive(enable: false);
			}
			this.SetActive(enable: true);
			this.RebuildLayout(recursive: true);
		}
	}

	public void SetRecipe(Recipe r)
	{
		if ((bool)imageArea)
		{
			imageArea.SetActive(enable: false);
		}
		area = null;
		if (r == null)
		{
			textDetail.SetActive(enable: false);
			Clear();
			return;
		}
		if (main)
		{
			goInfo.SetActive(value: true);
			goIngredients.SetActive(r.ingredients.Count > 0 && !r.UseStock);
		}
		if (recipe != r)
		{
			recipe = r;
			Refresh();
		}
	}

	public void Clear()
	{
		recipe = null;
		area = null;
		if (main)
		{
			goIngredients.SetActive(value: false);
			goInfo.SetActive(value: false);
			if ((bool)listVariations)
			{
				listVariations.SetActive(enable: false);
			}
		}
		if (textName.text != "")
		{
			UIText uIText = textName;
			string text2 = (textDetail.text = "");
			uIText.text = text2;
			RefreshList();
			RefreshBalance();
			if ((bool)imageArea)
			{
				imageArea.SetActive(enable: false);
			}
			note.Clear();
			note.Build();
			this.RebuildLayout(recursive: true);
		}
	}

	public void RefreshBalance()
	{
		if (!main)
		{
			return;
		}
		string text = Lang._Number(EMono.pc.GetCurrency());
		if (summary.money != 0)
		{
			text = text + "<color=" + colorCost.ToHex() + ">  -" + summary.money + "</color>";
		}
		textCost.SetText(text);
		foreach (DropdownRecipe dd in dds)
		{
			dd.RefreshLabel();
		}
		if ((bool)ddList)
		{
			ddList.Redraw();
		}
		if ((bool)textCostSP)
		{
			int countValid = summary.countValid;
			int num = ((countValid == 0) ? (-1) : (recipe.source.GetSPCost(summary.factory) * countValid));
			text = ((num == -1) ? "-" : (num.ToString() ?? ""));
			DOTweenAnimation component = textCostSP.GetComponent<DOTweenAnimation>();
			Transform transform = textCostSP.Find("nerun");
			bool flag = num >= EMono.pc.stamina.value;
			if ((bool)component && !flag)
			{
				component.DOPause();
			}
			textCostSP.SetText(text, (num == -1) ? FontColor.Bad : ((num < EMono.pc.stamina.value) ? FontColor.Good : FontColor.Warning));
			if ((bool)component && flag)
			{
				component.tween.Restart();
			}
			if ((bool)transform)
			{
				transform.SetActive(flag);
			}
		}
	}

	public void RefreshImages()
	{
		Recipe recipe = this.recipe;
		if ((bool)listVariations)
		{
			listVariations.Redraw();
		}
		if ((bool)imageIcon)
		{
			if (recipe.UseStock && !recipe.VirtualBlock)
			{
				recipe.ingredients[0].thing.SetImage(imageIcon, recipe._dir, recipe.ingredients[0].thing.trait.IdSkin);
			}
			else
			{
				recipe.renderRow.SetImage(imageIcon, null, recipe.renderRow.GetColorInt(recipe.GetColorMaterial()), setNativeSize: true, recipe._dir, recipe.idSkin);
			}
		}
	}

	public void Refresh()
	{
		if (recipe == null)
		{
			return;
		}
		if (recipe.UseStock || recipe.IngAsProduct)
		{
			Thing thing = recipe.ingredients[0].thing;
			if (thing == null || thing.isDestroyed || thing.ExistsOnMap)
			{
				return;
			}
		}
		Recipe r = recipe;
		r.OnSelected();
		RefreshList();
		RefreshBalance();
		if (r.UseStock && r.ingredients[0].thing == null)
		{
			return;
		}
		textName.text = r.GetName();
		string text = "";
		int[] tiles = recipe.renderRow.tiles;
		if (EMono.debug.showExtra)
		{
			text = text + "(" + recipe.id + "  " + recipe.renderRow.idRenderData + "/" + ((tiles.Length != 0) ? tiles[0] : (-1)) + ")" + Environment.NewLine;
		}
		textDetail.SetActive(enable: true);
		if (CraftMode)
		{
			if (main)
			{
				EMono.pc.things.GetThingStack(r.GetIdThing(), r.GetRefVal());
				int num = EMono.player.recipes.knownRecipes.TryGetValue(r.id, 0);
				textInfo.SetText("recipe_lv".lang(num.ToString() ?? ""));
				text += r.GetDetail();
			}
			else
			{
				text += r.GetDetail();
				r.WriteNote(note);
			}
		}
		else
		{
			string detail = r.GetDetail();
			if (!detail.IsEmpty())
			{
				text = text + detail + "\n\n";
			}
			text = text + "( " + r.tileType.LangPlaceType.lang() + " ) " + (r.tileType.LangPlaceType + "_hint").lang();
			if (r.UseStock && r.ingredients[0].thing != null)
			{
				r.ingredients[0].thing.WriteNote(note, null, IInspect.NoteMode.Recipe);
			}
			else
			{
				r.WriteNote(note);
			}
			if (!r.UseStock && r.source.NeedFactory)
			{
				PropSet propSet = EMono._map.Installed.cardMap.TryGetValue(r.source.idFactory);
				if ((propSet == null || propSet.Count == 0) && EMono.pc.things.Find((Thing t) => t.id == r.source.idFactory) == null)
				{
					note.Space(8);
					note.AddText("noFactory".lang(r.source.NameFactory), FontColor.Bad);
					note.Build();
				}
			}
		}
		textDetail.SetText(text);
		if ((bool)goMoney)
		{
			goMoney.transform.SetAsLastSibling();
		}
		if (main)
		{
			if ((bool)listVariations)
			{
				UIList uIList = listVariations;
				bool flag = r.CanRotate && r.renderRow != null;
				int[] array = (flag ? r.renderRow._tiles : null);
				if (flag && array.Length <= 1)
				{
					flag = false;
				}
				uIList.SetActive(flag);
				if (flag)
				{
					uIList.Clear();
					uIList.callbacks = new UIList.Callback<RecipeVariation, ButtonGrid>
					{
						onClick = delegate(RecipeVariation a, ButtonGrid b)
						{
							r.SetDir(a.dir);
						},
						onInstantiate = delegate(RecipeVariation a, ButtonGrid b)
						{
							b.SetRecipeVariation(a);
						},
						onRedraw = delegate(RecipeVariation a, ButtonGrid b, int i)
						{
							b.SetRecipeVariation(a);
						}
					};
					for (int j = 0; j < array.Length; j++)
					{
						uIList.Add(new RecipeVariation
						{
							recipe = r,
							dir = j
						});
					}
					uIList.Refresh();
					uIList.group.Init(r._dir);
				}
			}
			if ((bool)goMoney)
			{
				goMoney.SetActive(!CraftMode);
			}
		}
		else
		{
			listIngredients.SetActive(!recipe.UseStock || recipe.VirtualBlock);
			if ((bool)goMoney)
			{
				goMoney.SetActive(r.CostMoney != 0);
			}
		}
		RefreshImages();
		this.SetActive(enable: true);
		this.RebuildLayout(recursive: true);
	}

	public void OnRotate()
	{
		if (listVariations.gameObject.activeSelf)
		{
			listVariations.group.Select(recipe._dir);
		}
		RefreshImages();
	}

	public void RefreshQuality()
	{
		if ((bool)textQuality)
		{
			recipe.SetTextDifficulty(textQuality);
		}
	}

	public void RefreshList()
	{
		if ((bool)textReqSkill)
		{
			string str = "";
			FontColor c = FontColor.Good;
			if (recipe != null)
			{
				Element reqSkill = recipe.source.GetReqSkill();
				int value = EMono.pc.elements.GetOrCreateElement(reqSkill).Value;
				_ = reqSkill.Name + " " + reqSkill.Value;
				str = "reqSkill2".lang(reqSkill.Name, reqSkill.Value.ToString() ?? "", value.ToString() ?? "");
				if (value < reqSkill.Value)
				{
					c = FontColor.Warning;
				}
			}
			textReqSkill.SetText(str.IsEmpty("noReqSkill".lang()), c);
		}
		if ((bool)ddFactory)
		{
			List<GridItem> list = new List<GridItem>();
			if (recipe.source.NeedFactory)
			{
				GridItemCardSource gridItemCardSource = new GridItemCardSource
				{
					source = EMono.sources.cards.map[recipe.source.idFactory]
				};
				if (factory != null && recipe.source.idFactory == factory.id)
				{
					gridItemCardSource.thing = factory;
				}
				list.Add(gridItemCardSource);
			}
			ddFactory.Build(list);
		}
		RefreshQuality();
		if ((bool)ddList)
		{
			ddList.SetActive(recipe != null && !recipe.UseStock);
			if (recipe != null && !recipe.UseStock)
			{
				ddList.recipeInfo = this;
				ddList.lastMats = lastMats;
				ddList.BuildIngredients(recipe, imageIcon, RefreshQuality, searchMode);
			}
			return;
		}
		Recipe r = recipe;
		UIList uIList = listIngredients;
		uIList.Clear();
		dds.Clear();
		if (r == null || r.UseStock)
		{
			return;
		}
		uIList.callbacks = new UIList.Callback<Recipe.Ingredient, UIItem>
		{
			onClick = delegate
			{
			},
			onInstantiate = delegate(Recipe.Ingredient ingredient, UIItem b)
			{
				DropdownRecipe dropdownRecipe = b.dd as DropdownRecipe;
				dropdownRecipe.recipe = r;
				dds.Add(dropdownRecipe);
				ThingStack thingStack = EMono._map.Stocked.ListThingStack(ingredient, searchMode);
				int num = thingStack.count;
				Color color = ((num >= ingredient.req) ? SkinManager.CurrentColors.textGood : SkinManager.CurrentColors.textBad);
				dropdownRecipe.SetList(0, new List<string> { ingredient.id }, (string n, int i) => "<color=" + color.ToHex() + ">" + ingredient.GetName() + " x " + ingredient.req + "  (" + num + ")</color>", delegate
				{
				});
				dropdownRecipe.textLabel.text = dropdownRecipe.orgLabel.text;
				if ((bool)textCost)
				{
					textCost.text = r.CostMoney.ToString() ?? "";
				}
			}
		};
		foreach (Recipe.Ingredient ingredient in r.ingredients)
		{
			uIList.Add(ingredient);
		}
		uIList.Refresh();
	}
}
