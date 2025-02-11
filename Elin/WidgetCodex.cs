using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WidgetCodex : Widget
{
	public enum SearchType
	{
		Codex,
		Search
	}

	public InputField field;

	public WordSearch search = new WordSearch();

	public UIDynamicList list;

	public HashSet<Recipe> recipes = new HashSet<Recipe>();

	public string lastSearch;

	public UIButton buttonClear;

	public Transform transResult;

	private int count;

	public virtual SearchType type => SearchType.Codex;

	public override void OnActivate()
	{
		field.onValueChanged.AddListener(Search);
		field.onSubmit.AddListener(Search);
		lastSearch = null;
		Search("");
		if (EMono._zone.isStarted)
		{
			field.Select();
		}
	}

	public void Clear()
	{
		field.text = "";
	}

	public virtual void Search(string s)
	{
		s = s.ToLower();
		buttonClear.SetActive(field.text != "");
		if (s == lastSearch)
		{
			return;
		}
		RecipeManager.BuildList();
		HashSet<Recipe> hashSet = new HashSet<Recipe>();
		if (!s.IsEmpty())
		{
			foreach (RecipeSource item in RecipeManager.list)
			{
				if (!item.isChara && !item.noListing && !item.isBridgePillar && (item.row.GetSearchName(jp: false).Contains(s) || item.row.GetSearchName(jp: true).Contains(s)) && (EMono.player.recipes.knownRecipes.ContainsKey(item.id) || item.alwaysKnown))
				{
					hashSet.Add(Recipe.Create(item));
				}
			}
		}
		else
		{
			foreach (RecipeSource item2 in RecipeManager.list)
			{
				if (!item2.isChara && !item2.noListing && (EMono.player.recipes.knownRecipes.ContainsKey(item2.id) || item2.alwaysKnown))
				{
					hashSet.Add(Recipe.Create(item2));
				}
			}
		}
		if (!hashSet.SetEquals(recipes))
		{
			recipes = hashSet;
			RefreshList();
		}
		lastSearch = s;
	}

	public virtual void RefreshList()
	{
		list.callbacks = new UIList.Callback<Recipe, ButtonGrid>
		{
			onClick = delegate(Recipe a, ButtonGrid b)
			{
				SE.ClickGeneral();
				a.ToggleTrack(a.GetQuestTrack());
			},
			onRedraw = delegate(Recipe a, ButtonGrid b, int i)
			{
				b.SetCraftRecipe(a, ButtonGrid.Mode.RecipeGrid, tooltip: true);
			},
			onList = delegate
			{
				foreach (Recipe recipe in recipes)
				{
					list.Add(recipe);
				}
			}
		};
		list.List();
		list.dsv.OnResize();
	}

	protected void Update()
	{
		count++;
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Close();
			EInput.Consume(consumeAxis: false, 5);
			return;
		}
		int functionkeyDown = EInput.GetFunctionkeyDown();
		if (count > 10 && functionkeyDown != -1)
		{
			WidgetHotbar hotbarExtra = WidgetHotbar.HotbarExtra;
			if ((bool)hotbarExtra && hotbarExtra.GetItem(functionkeyDown) is HotItemWidget hotItemWidget && hotItemWidget.id == base.ID)
			{
				Close();
				EInput.Consume(consumeAxis: false, 5);
				return;
			}
		}
		if (CheckClose())
		{
			Close();
			EInput.Consume();
		}
	}

	public virtual bool CheckClose()
	{
		return false;
	}
}
