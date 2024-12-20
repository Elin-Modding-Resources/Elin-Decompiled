using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIDragGridInfo : EMono
{
	public Window window;

	public UIText textHeader;

	public Transform transMold;

	public Transform moldThing;

	public Transform moldPlus;

	public Transform moldEqual;

	public Transform moldOr;

	public Transform moldCat;

	public Transform moldUnknown;

	public UIList list;

	public Card owner;

	private void Awake()
	{
		window.SetActive(enable: false);
		transMold.SetActive(enable: false);
	}

	public void Refresh()
	{
		Init(owner);
	}

	public void Init(Card _owner)
	{
		owner = _owner;
		TraitCrafter crafter = owner.trait as TraitCrafter;
		if (crafter == null)
		{
			return;
		}
		textHeader.text = "knownRecipe".lang();
		List<SourceRecipe.Row> recipes = EMono.sources.recipes.rows.Where((SourceRecipe.Row r) => r.factory == crafter.IdSource).ToList();
		if (recipes.Count == 0)
		{
			return;
		}
		list.callbacks = new UIList.Callback<SourceRecipe.Row, LayoutGroup>
		{
			onClick = delegate
			{
			},
			onInstantiate = delegate(SourceRecipe.Row a, LayoutGroup b)
			{
				for (int i = 0; i < crafter.numIng; i++)
				{
					if (i != 0)
					{
						Util.Instantiate(moldPlus, b);
					}
					string[] array2 = i switch
					{
						1 => a.ing2, 
						0 => a.ing1, 
						_ => a.ing3, 
					};
					if (array2.IsEmpty())
					{
						break;
					}
					string[] array3 = array2;
					foreach (string text2 in array3)
					{
						if (text2 != array2[0])
						{
							Util.Instantiate(moldOr, b);
						}
						AddThing(text2);
					}
				}
				Util.Instantiate(moldEqual, b);
				AddThing(a.thing);
			},
			onList = delegate
			{
				foreach (SourceRecipe.Row item in recipes)
				{
					if (item.tag.Contains("known") || EMono.player.knownCraft.Contains(item.id) || EMono.debug.godCraft)
					{
						list.Add(item);
					}
				}
			}
		};
		list.List();
		window.SetActive(enable: true);
		window.RebuildLayout(recursive: true);
		void AddThing(string id)
		{
			if (id.IsEmpty() || id == "notImplemented" || id == "any")
			{
				Util.Instantiate(moldUnknown, P_1.b).GetComponentInChildren<UIButton>().tooltip.lang = "???";
			}
			else
			{
				id = id.Replace("%", "@");
				string[] array = id.Split('@');
				string text = "";
				id = array[0];
				if (id.StartsWith('#'))
				{
					text = id.Replace("#", "");
					id = EMono.sources.categories.map[text].GetIdThing();
				}
				CardRow cardRow = EMono.sources.cards.map[id];
				SourceMaterial.Row mat = cardRow.DefaultMaterial;
				if (array.Length >= 2)
				{
					mat = ((!(array[1] == "gelatin")) ? EMono.sources.materials.alias[array[1]] : EMono.sources.materials.alias["jelly"]);
				}
				Transform transform = Util.Instantiate(moldThing, P_1.b);
				Image componentInChildren = transform.GetComponentInChildren<Image>();
				UIButton component = componentInChildren.GetComponent<UIButton>();
				cardRow.SetImage(componentInChildren, null, cardRow.GetColorInt(mat));
				string s = cardRow.GetName();
				if (!text.IsEmpty())
				{
					Transform obj = Util.Instantiate(moldCat, transform);
					string @ref = EMono.sources.categories.map[text].GetName();
					obj.GetComponentInChildren<UIText>().SetText("category".lang());
					s = "ingCat".lang(@ref);
				}
				component.tooltip.lang = s.ToTitleCase();
			}
		}
	}

	public void InitFuel(Card _owner)
	{
		owner = _owner;
		textHeader.text = "knownFuel".lang();
		List<SourceThing.Row> fuels = new List<SourceThing.Row>();
		foreach (SourceThing.Row row in EMono.sources.things.rows)
		{
			if (owner.trait.IsFuel(row.id))
			{
				fuels.Add(row);
			}
		}
		list.callbacks = new UIList.Callback<SourceThing.Row, LayoutGroup>
		{
			onClick = delegate
			{
			},
			onInstantiate = delegate(SourceThing.Row a, LayoutGroup b)
			{
				AddThing(a.id);
				Util.Instantiate(moldEqual, b);
				AddThing(owner.id);
			},
			onList = delegate
			{
				foreach (SourceThing.Row item in fuels)
				{
					list.Add(item);
				}
			}
		};
		list.List();
		window.SetActive(enable: true);
		window.RebuildLayout(recursive: true);
		void AddThing(string id)
		{
			if (id.IsEmpty() || id == "notImplemented" || id == "any")
			{
				Util.Instantiate(moldUnknown, P_1.b).GetComponentInChildren<UIButton>().tooltip.lang = "???";
			}
			else
			{
				id = id.Replace("%", "@");
				string[] array = id.Split('@');
				string cat = "";
				id = array[0];
				if (id.StartsWith('#'))
				{
					cat = id.Replace("#", "");
					List<CardRow> obj = EMono.sources.cards.rows.Where((CardRow r) => r.Category.IsChildOf(cat)).ToList();
					obj.Sort((CardRow a, CardRow b) => a.value - b.value);
					id = obj[0].id;
				}
				CardRow cardRow = EMono.sources.cards.map[id];
				SourceMaterial.Row mat = cardRow.DefaultMaterial;
				if (array.Length >= 2)
				{
					mat = ((!(array[1] == "gelatin")) ? EMono.sources.materials.alias[array[1]] : EMono.sources.materials.alias["jelly"]);
				}
				Transform transform = Util.Instantiate(moldThing, P_1.b);
				Image componentInChildren = transform.GetComponentInChildren<Image>();
				UIButton component = componentInChildren.GetComponent<UIButton>();
				cardRow.SetImage(componentInChildren, null, cardRow.GetColorInt(mat));
				string s = cardRow.GetName();
				if (!cat.IsEmpty())
				{
					Transform obj2 = Util.Instantiate(moldCat, transform);
					string @ref = EMono.sources.categories.map[cat].GetName();
					obj2.GetComponentInChildren<UIText>().SetText("(" + "category".lang() + ")");
					s = "ingCat".lang(@ref);
				}
				component.tooltip.lang = s.ToTitleCase();
			}
		}
	}
}
