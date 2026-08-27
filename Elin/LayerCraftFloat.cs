using System.Collections.Generic;
using UnityEngine;

public class LayerCraftFloat : ELayer
{
	public static LayerCraftFloat Instance;

	public UIDynamicList list;

	public Recipe recipe;

	public UIButton moldButtonContext;

	public int maxSize;

	public int paddingY;

	public int paddingY2;

	public string idLastRecipe;

	public override void OnInit()
	{
		Instance = this;
	}

	public override void OnSwitchContent(Window window)
	{
		if (ELayer.core.IsGameStarted)
		{
			ELayer.game.updater.recipe.Build(ELayer.pc.pos, RecipeUpdater.Mode.Immediate);
		}
	}

	public void Update()
	{
		if (EInput.middleMouse.down)
		{
			ButtonGrid componentOf = InputModuleEX.GetComponentOf<ButtonGrid>();
			if ((bool)componentOf && componentOf.transform.IsChildOf(base.transform))
			{
				Recipe recipe = componentOf.recipe;
				if (recipe != null && windows[0].idTab == 0)
				{
					ShowContextMenu(recipe);
				}
			}
		}
		if (Input.GetMouseButton(0))
		{
			ButtonGrid componentOf2 = InputModuleEX.GetComponentOf<ButtonGrid>();
			if ((bool)componentOf2 && componentOf2.transform.IsChildOf(base.transform) && ELayer.player.waitingInput && ELayer.pc.HasNoGoal && componentOf2.recipe.id == idLastRecipe)
			{
				list.callbacks.OnClick(componentOf2.recipe, componentOf2);
			}
		}
		else
		{
			idLastRecipe = null;
		}
	}

	public void OnCompleteCraft()
	{
	}

	public void RefreshCraft()
	{
		list.callbacks = new UIList.Callback<Recipe, ButtonGrid>
		{
			onClick = delegate(Recipe a, ButtonGrid b)
			{
				ELayer.ui.RemoveLayers();
				ELayer.game.updater.recipe.Build(ELayer.pc.pos, RecipeUpdater.Mode.Validate);
				if (!RecipeUpdater.recipes.Contains(a))
				{
					SE.BeepSmall();
					ELayer.game.updater.recipe.Build(ELayer.pc.pos, RecipeUpdater.Mode.Immediate);
				}
				else
				{
					a.OnChangeIngredient();
					recipe = a;
					idLastRecipe = recipe.id;
					if (!ELayer.player.waitingInput)
					{
						SE.BeepSmall();
					}
					else
					{
						TaskCraft taskCraft = new TaskCraft
						{
							recipe = recipe,
							num = 1,
							repeat = false,
							floatMode = true
						};
						taskCraft.ResetReq();
						if (!taskCraft.IsIngredientsValid(destoryResources: false, taskCraft.num))
						{
							SE.Beep();
						}
						else
						{
							ELayer.pc.SetAI(taskCraft);
							ActionMode.Adv.SetTurbo();
						}
					}
				}
			},
			onRedraw = delegate(Recipe a, ButtonGrid b, int i)
			{
				b.SetCraftRecipe(a, ButtonGrid.Mode.RecipeGrid, tooltip: true);
				if (!ELayer.player.recipes.hoveredRecipes.Contains(a.id))
				{
					b.Attach("recipe_new", rightAttach: false);
				}
				b.onRightClick = delegate
				{
					ShowContextMenu(a);
				};
			},
			onList = delegate
			{
				foreach (Recipe recipe in RecipeUpdater.recipes)
				{
					list.Add(recipe);
				}
			}
		};
		list.List();
		RefreshSize();
		list.dsv.OnResize();
	}

	public void ShowContextMenu(Recipe a)
	{
		UIContextMenu m = ELayer.ui.CreateContextMenuInteraction();
		Point point = ELayer.pc.pos.Copy();
		foreach (Recipe.Ingredient ing in a.ingredients)
		{
			UIContextMenu uIContextMenu = ((a.ingredients.Count == 1) ? m : m.AddChild(ing.GetName() + "x" + ing.req));
			List<Thing> list = new List<Thing>();
			foreach (Thing thing in ELayer.pc.things)
			{
				thing.GetIngredients(ing, list);
			}
			for (int i = point.x - 1; i < point.x + 2; i++)
			{
				if (i < 0 || i >= ELayer._map.Size)
				{
					continue;
				}
				for (int j = point.z - 1; j < point.z + 2; j++)
				{
					if (j < 0 || j >= ELayer._map.Size)
					{
						continue;
					}
					Cell cell = ELayer._map.cells[i, j];
					if (cell.detail == null || cell.detail.things.Count == 0)
					{
						continue;
					}
					foreach (Thing thing2 in cell.detail.things)
					{
						thing2.GetIngredients(ing, list);
					}
				}
			}
			foreach (Thing t in list)
			{
				UIButton uIButton = Util.Instantiate(moldButtonContext);
				uIContextMenu.AddGameObject(uIButton);
				t.SetImage(uIButton.icon);
				uIButton.mainText.text = t.Name + ((t.parent == ELayer.pc) ? "" : ("(" + ((t.parent == ELayer._zone) ? "ground".lang() : (t.parent as Card)?.Name) + ")"));
				uIButton.onClick.AddListener(delegate
				{
					ing.thing = t;
					SE.ClickOk();
					this.list.List();
					m.Hide();
				});
			}
		}
		m.Show();
	}

	public void RefreshDisassemble()
	{
		list.callbacks = new UIList.Callback<Thing, ButtonGrid>
		{
			onClick = delegate(Thing a, ButtonGrid b)
			{
				if (!ELayer.player.waitingInput)
				{
					SE.BeepSmall();
				}
				else
				{
					a.Disassemble();
					ELayer.player.EndTurn();
				}
			},
			onRedraw = delegate(Thing a, ButtonGrid b, int i)
			{
				b.SetCard(a, ButtonGrid.Mode.RecipeGrid, delegate(UINote n)
				{
					RecipeSource recipeSource = RecipeManager.Get(a.id);
					if (recipeSource != null)
					{
						n.Space();
						Recipe.Create(recipeSource).WriteReqSkill(n);
					}
				});
				b.mainText.SetActive(enable: false);
			},
			onList = delegate
			{
				foreach (Thing item in RecipeUpdater.listD)
				{
					list.Add(item);
				}
			}
		};
		list.List();
		RefreshSize();
		list.dsv.OnResize();
	}

	public void RefreshSize()
	{
		list.dsv.GetComponent<UIScrollView>().enabled = list.ItemCount > 0;
		RectTransform rectTransform = windows[0].Rect();
		float x = rectTransform.sizeDelta.x;
		float num = list.Rect().sizeDelta.y;
		if (num > (float)maxSize)
		{
			num = maxSize;
		}
		rectTransform.sizeDelta = new Vector2(x, num + (float)((list.ItemCount > 0) ? paddingY : paddingY2));
		rectTransform.RebuildLayout();
	}
}
