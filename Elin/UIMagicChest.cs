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

	public int pageJump = 7;

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
			buttonsPage.Add(b);
			b.SetOnClick(delegate
			{
				if (!UIContextMenu.Current)
				{
					page = b.refInt;
					SE.Tab();
					Redraw();
				}
			});
		}
		groupPage.selectOnClick = false;
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
		int count = buttonsPage.Count;
		bool flag = pageMax >= count;
		int num = ((!flag) ? 1 : Mathf.Clamp(page - count / 2 + 1, 1, pageMax - count + 2));
		for (int i = 0; i < count; i++)
		{
			UIButton uIButton = buttonsPage[i];
			int num2 = ((i != 0) ? ((flag && i == count - 1) ? pageMax : (num + i - 1)) : 0);
			bool flag2 = (i == 1 && num2 > 1) || (i == count - 2 && num2 < pageMax - 1);
			uIButton.refInt = (flag2 ? Mathf.Clamp(page + ((i == 1) ? (-pageJump) : pageJump), 0, pageMax) : num2);
			uIButton.mainText.text = (flag2 ? "…" : ((num2 + 1).ToString() ?? ""));
			uIButton.interactable = num2 <= pageMax;
		}
		groupPage.selected = null;
		groupPage.Select((UIButton a) => a.refInt == page);
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
