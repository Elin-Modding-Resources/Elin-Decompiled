using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMagicChest : EMono
{
	public LayoutGroup layoutPage;

	public LayoutGroup layoutCat;

	public LayoutGroup layoutBottom;

	public UIButton itemNum;

	public UIButton itemElec;

	public UIButton buttonClearSearch;

	public UIButton moldCat;

	public UISelectableGroup groupPage;

	public List<UIButton> buttonsPage;

	public UIInventory uiInventory;

	public InputField inputSearch;

	public float intervalSearch;

	public int page;

	public int pageMax;

	public List<Thing> filteredList = new List<Thing>();

	public Color colorCat;

	public Color colorCatSelected;

	public HashSet<string> cats = new HashSet<string>();

	public Dictionary<string, UIButton> catButton = new Dictionary<string, UIButton>();

	public Dictionary<string, int> catCount = new Dictionary<string, int>();

	public string idCat = "";

	private bool firstSearch = true;

	private float timerSearch;

	public string lastSearch = "";

	public HashSet<Recipe> searchRecipes = new HashSet<Recipe>();

	public Card container => uiInventory.owner.Container;

	public int GridSize => container.things.GridSize;

	public void Init()
	{
		UIButton t = layoutPage.CreateMold<UIButton>();
		moldCat = layoutCat.CreateMold<UIButton>();
		for (int i = 0; i < 9; i++)
		{
			UIButton b = Util.Instantiate(t, layoutPage);
			b.mainText.text = (i + 1).ToString() ?? "";
			buttonsPage.Add(b);
			int _i = i;
			b.SetOnClick(delegate
			{
				if (!UIContextMenu.Current)
				{
					page = _i;
					groupPage.Select(b);
					SE.Tab();
					Redraw();
				}
			});
		}
		groupPage.Init();
		inputSearch.onValueChanged.AddListener(Search);
		inputSearch.onSubmit.AddListener(Search);
		RefreshBottom();
		itemNum.RebuildLayout();
		itemElec.RebuildLayout();
		layoutBottom.RebuildLayout();
	}

	public void OnAfterRedraw()
	{
		RefreshBottom();
		for (int i = 0; i < buttonsPage.Count; i++)
		{
			buttonsPage[i].interactable = i <= pageMax;
		}
		groupPage.selected = null;
		groupPage.Select(page);
	}

	public void RefreshCats()
	{
		foreach (string cat in cats)
		{
			if (catButton.ContainsKey(cat))
			{
				continue;
			}
			UIButton uIButton = Util.Instantiate(moldCat, layoutCat);
			catButton[cat] = uIButton;
			string _c = cat;
			uIButton.SetOnClick(delegate
			{
				if (!UIContextMenu.Current)
				{
					SE.Tab();
					if (idCat == _c)
					{
						idCat = "";
					}
					else
					{
						idCat = _c;
					}
					Redraw();
				}
			});
		}
		foreach (KeyValuePair<string, UIButton> item in catButton)
		{
			bool flag = cats.Contains(item.Key);
			UIButton value = item.Value;
			value.SetActive(flag);
			if (flag)
			{
				value.mainText.text = EMono.sources.categories.map[item.Key].GetName() + " (" + catCount[item.Key] + ")";
				value.image.color = ((item.Key == idCat) ? colorCatSelected : colorCat);
			}
		}
	}

	public void RefreshBottom()
	{
		itemNum.mainText.text = container.things.Count + " / " + container.things.MaxCapacity;
		itemElec.mainText.SetText(Mathf.Abs(container.trait.Electricity) + " " + "mw".lang(), (container.trait.Electricity == 0 || container.isOn) ? FontColor.Good : FontColor.Bad);
	}

	private void LateUpdate()
	{
		if (timerSearch > 0f)
		{
			timerSearch -= Core.delta;
			if (timerSearch <= 0f)
			{
				Search(inputSearch.text);
			}
		}
		if (EInput.wheel != 0 && !UIContextMenu.Current)
		{
			SE.Tab();
			page -= EInput.wheel;
			if (page < 0)
			{
				page = pageMax;
			}
			if (page > pageMax)
			{
				page = 0;
			}
			Redraw();
		}
	}

	public void Search(string s)
	{
		s = s.ToLower();
		if (s.IsEmpty())
		{
			s = "";
		}
		buttonClearSearch.SetActive(inputSearch.text != "");
		if (s == lastSearch)
		{
			return;
		}
		if (firstSearch)
		{
			firstSearch = false;
			foreach (Thing thing in container.things)
			{
				thing.tempName = thing.GetName(NameStyle.Full, 1).ToLower();
			}
		}
		timerSearch = intervalSearch;
		lastSearch = s;
		Redraw();
	}

	public void ClearSearch()
	{
		inputSearch.text = "";
		timerSearch = 0f;
		lastSearch = "";
		Redraw();
	}

	public void Redraw()
	{
		uiInventory.list.Redraw();
	}
}
