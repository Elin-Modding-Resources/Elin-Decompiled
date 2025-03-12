using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class UIBook : MonoBehaviour
{
	public enum Mode
	{
		Help,
		Announce,
		Book,
		Parchment,
		Gallery,
		About
	}

	public class Item
	{
		public string id;

		public string idFile;

		public string title;

		public List<Item> items = new List<Item>();
	}

	public class Page
	{
		public UIBook book;

		public int idx;

		public List<string> lines = new List<string>();

		public virtual void BuildNote(UINote n, string idTopic)
		{
			string text = "";
			bool center = false;
			bool hasSearchWord = !searchWord.IsEmpty();
			bool skip = false;
			UINote current = n;
			ParseLines(lines, idTopic);
			void Flush()
			{
				if (!text.IsEmpty())
				{
					UIItem uIItem = current.AddText(STR(text));
					text = "";
					if (center)
					{
						uIItem.text1.alignment = TextAnchor.UpperCenter;
					}
					LayoutElement component = uIItem.GetComponent<LayoutElement>();
					if ((bool)component)
					{
						component.enabled = false;
					}
				}
			}
			void ParseLines(List<string> lines, string idTopic)
			{
				char c = 'a';
				bool flag = false;
				foreach (string line in lines)
				{
					string text2 = Lang.Parse(line, "test1", "test2");
					if (text2.Length == 0)
					{
						if (!skip && (idTopic.IsEmpty() || flag))
						{
							text += Environment.NewLine;
						}
					}
					else
					{
						c = text2[0];
						if (!idTopic.IsEmpty())
						{
							if (c == '$')
							{
								if (flag)
								{
									break;
								}
								if (text2.Split('$')[1] == idTopic)
								{
									flag = true;
									continue;
								}
							}
							if (!flag)
							{
								continue;
							}
						}
						if (book.mode == Mode.Announce)
						{
							switch (c)
							{
							case '■':
								Flush();
								current.AddHeader("HeaderPatch_header", line);
								continue;
							case '[':
								text = text.TrimEnd(Environment.NewLine.ToCharArray());
								Flush();
								current.AddHeader("HeaderPatch_category", line);
								continue;
							case '*':
								Flush();
								text = line.TrimStart('*').TrimStart(' ').TrimEnd(Environment.NewLine.ToCharArray());
								current.AddText("NoteText_list", text);
								text = "";
								continue;
							}
						}
						if (c == '{')
						{
							Flush();
							string[] array = text2.Split('}');
							text = array[1];
							string[] array2 = array[0].TrimStart('{').Split(array[0].Contains('|') ? '|' : ',');
							if (!skip || !(array2[0] != "endif"))
							{
								switch (array2[0])
								{
								case "if":
								{
									string text5 = array2[1];
									if (!(text5 == "cn"))
									{
										if (text5 == "!cn" && Lang.langCode == "CN")
										{
											skip = true;
										}
									}
									else if (Lang.langCode != "CN")
									{
										skip = true;
									}
									break;
								}
								case "endif":
									skip = false;
									break;
								case "include":
								case "load":
								{
									string text3 = "";
									string text4 = "";
									if (array2[0] == "include")
									{
										text3 = array2[1];
										text4 = "include";
									}
									else
									{
										text4 = array2[1];
										text3 = array2[2];
									}
									text4 += ".txt";
									string[] array3 = IO.LoadTextArray(CorePath.CorePackage.Help + text4);
									if (array3.IsEmpty())
									{
										array3 = IO.LoadTextArray(CorePath.CorePackage.Text + text4);
									}
									if (array3.IsEmpty())
									{
										array3 = IO.LoadTextArray(CorePath.CorePackage.TextCommon + text4);
									}
									if (!array3.IsEmpty())
									{
										ParseLines(array3.ToList(), text3);
									}
									break;
								}
								case "center":
									center = true;
									break;
								case "/center":
									center = false;
									break;
								case "layout":
									if (array2.Length == 1)
									{
										current = n;
									}
									else
									{
										current = n.AddNote(STR(array2[1]));
									}
									break;
								case "nerun":
									current.AddText("NoteText_nerun", STR(text));
									break;
								case "topic":
									current.AddHeader("HeaderHelpTopic", array2[1]);
									break;
								case "Q":
									current.AddText("NoteText_Q", STR(array2[1]));
									break;
								case "A":
									current.AddText("NoteText_A", STR(array2[1]));
									break;
								case "pair":
									current.AddTopic("TopicPair", STR(array2[1]), STR(array2[2]));
									break;
								case "image":
									current.AddImage(array2[1]);
									break;
								case "link":
									current.AddButtonLink(array2[1], array2[2]);
									break;
								case "button":
									if (array2[1] == "close")
									{
										book.AddButtonClose();
									}
									break;
								}
								text = "";
							}
						}
						else if (!skip)
						{
							text = text + text2 + Environment.NewLine;
						}
					}
				}
				if (!text.IsEmpty())
				{
					text = text.Replace("#pc", str_pc);
					UIItem uIItem2 = current.AddText("NoteText_help", STR(text));
					if (center)
					{
						uIItem2.text1.alignment = TextAnchor.UpperCenter;
					}
				}
			}
			string STR(string s)
			{
				if (!hasSearchWord)
				{
					return s;
				}
				return Regex.Replace(s, searchWord, searchWord.TagColor(book.colorSearchHit), RegexOptions.IgnoreCase);
			}
		}
	}

	public static string str_pc;

	public static string searchWord;

	public static Dictionary<string, string> helpTitles = new Dictionary<string, string>();

	public Mode mode;

	[NonSerialized]
	public int currentPage;

	public List<Page> pages = new List<Page>();

	public UINote note;

	public UINote note2;

	public UINote noteSearch;

	public UIList list;

	public UIDynamicList listSearchResult;

	public UIText textTitle;

	public UIText textPage;

	public UIText textPage2;

	[NonSerialized]
	public string idFile;

	[NonSerialized]
	public string idTopic;

	[NonSerialized]
	public string idFirstFile;

	[NonSerialized]
	public string idFirstTopic;

	public SkinRootStatic skin;

	public LayoutGroup layoutButton;

	public UIButton moldButton;

	public BookList.Item bookItem;

	public Scrollbar scrollbar;

	public Window window;

	public UIButton buttonPrev;

	public UIButton buttonNext;

	public InputField inputSearch;

	public UIButton buttonClear;

	public UIScrollView scrollView;

	public Transform transSearchResult;

	public string lastSearch;

	public bool showSearchResult;

	public float searchResultHeight;

	public float marginSearchResult;

	public Color colorSearchHit;

	public static SearchContext searchContext;

	private void Awake()
	{
		if (searchContext == null || Application.isEditor)
		{
			searchContext = new SearchContext();
			searchContext.Init();
		}
		if ((bool)inputSearch)
		{
			inputSearch.onValueChanged.AddListener(Search);
			inputSearch.onSubmit.AddListener(Search);
			lastSearch = null;
		}
	}

	private void Update()
	{
		if ((bool)inputSearch)
		{
			if (inputSearch.isFocused)
			{
				showSearchResult = true;
			}
			else if (Input.GetMouseButtonDown(0) && !InputModuleEX.IsPointerOver(scrollView) && !InputModuleEX.IsPointerOver(inputSearch))
			{
				showSearchResult = false;
			}
			buttonClear.SetActive(inputSearch.text != "");
			transSearchResult.SetActive((inputSearch.isFocused || showSearchResult) && inputSearch.text != "");
			if (Input.GetKeyDown(KeyCode.Escape) && inputSearch.text == "")
			{
				window.layer.Close();
				EInput.Consume(consumeAxis: false, 5);
			}
		}
	}

	public void Search(string s)
	{
		if (s == lastSearch)
		{
			return;
		}
		lastSearch = s;
		if (s.IsEmpty())
		{
			return;
		}
		transSearchResult.SetActive(enable: true);
		listSearchResult.Clear();
		listSearchResult.callbacks = new UIList.Callback<SearchContext.Item, UIButton>
		{
			onClick = delegate(SearchContext.Item a, UIButton b)
			{
				if (!a.system)
				{
					SE.Play("click_recipe");
					searchWord = s;
					Show(a.idFile, a.idTopic);
				}
			},
			onRedraw = delegate(SearchContext.Item a, UIButton b, int i)
			{
				if (a.system)
				{
					b.mainText.text = "";
					b.subText.text = a.text;
				}
				else
				{
					string text = helpTitles.TryGetValue(a.idFile + "-") ?? "";
					string text2 = helpTitles.TryGetValue(a.idFile + "-" + a.idTopic) ?? a.idTopic;
					b.mainText.text = (text.IsEmpty() ? "" : (text + " - ")) + text2;
					string text3 = a.text;
					if (!text3.Contains("</color>"))
					{
						int num = a.textSearch.IndexOf(s.ToLower());
						if (num != -1)
						{
							text3 = text3.Substring(0, num) + text3.Substring(num, s.Length).TagColor(colorSearchHit) + text3.Substring(num + s.Length);
						}
					}
					b.subText.text = text3;
				}
			},
			onList = delegate
			{
				foreach (SearchContext.Item item in searchContext.Search(s))
				{
					listSearchResult.Add(item);
				}
			},
			onRefresh = null
		};
		listSearchResult.List();
		scrollView.content.RebuildLayout(recursive: true);
		RectTransform rectTransform = transSearchResult.Rect();
		rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, Mathf.Min(searchResultHeight, scrollView.content.sizeDelta.y + marginSearchResult));
	}

	public void ClearSearch()
	{
		inputSearch.text = "";
		lastSearch = null;
	}

	public void ModPage(int a)
	{
		SE.Play("read_paper");
		if ((bool)note2)
		{
			a *= 2;
		}
		currentPage += a;
		if (currentPage < 0)
		{
			currentPage = 0;
		}
		if (currentPage >= pages.Count)
		{
			currentPage = pages.Count - ((!note2) ? 1 : 2);
		}
		ShowPage(currentPage);
	}

	public void Show(string _idFile = null, string _idTopic = null, string title = null, BookList.Item _bookItem = null)
	{
		if (!title.IsEmpty() && (bool)window)
		{
			window.textCaption.SetText(title);
		}
		idFile = _idFile;
		idTopic = _idTopic;
		bookItem = _bookItem;
		BuildPages();
		Show();
	}

	public void Show()
	{
		note.Clear();
		if ((bool)note2)
		{
			note2.Clear();
		}
		if ((bool)layoutButton)
		{
			moldButton = layoutButton.CreateMold<UIButton>();
		}
		if (mode == Mode.Help)
		{
			idFirstFile = idFile;
			idFirstTopic = idTopic;
			RefreshTopics();
		}
		else
		{
			ShowPage(currentPage);
		}
	}

	public void BuildPages()
	{
		pages.Clear();
		string[] array = IO.LoadTextArray(CorePath.CorePackage.Help + idFile);
		if (idFile == "version" && Lang.langCode == "CN")
		{
			array = IO.LoadTextArray(CorePath.CorePackage.TextEN + idFile);
		}
		if (array.IsEmpty())
		{
			array = IO.LoadTextArray(CorePath.CorePackage.Text + idFile);
		}
		if (array.IsEmpty())
		{
			array = IO.LoadTextArray(CorePath.CorePackage.TextCommon + idFile);
		}
		if (array.IsEmpty())
		{
			array = IO.LoadTextArray(CorePath.CorePackage.HelpEN + idFile);
		}
		if (array.IsEmpty())
		{
			array = IO.LoadTextArray(CorePath.CorePackage.TextEN + idFile);
		}
		Page page = new Page();
		int num = 0;
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (bookItem != null && num == 0)
			{
				num++;
				continue;
			}
			if (text == "{p}")
			{
				AddPage(page);
				page = new Page();
			}
			page.lines.Add(text);
			num++;
		}
		AddPage(page);
	}

	public void AddPage(Page page)
	{
		page.idx = pages.Count;
		pages.Add(page);
		page.book = this;
	}

	public void RefreshTopics()
	{
		string[] array = IO.LoadTextArray(CorePath.CorePackage.Help + "_topics.txt");
		List<UIList> lists = new List<UIList>();
		lists.Add(list);
		list.Clear();
		list.callbacks = new UIList.Callback<Item, ButtonCategory>
		{
			onClick = delegate(Item a, ButtonCategory b)
			{
				if (a.items.Count > 0)
				{
					b.buttonFold.onClick.Invoke();
				}
				else
				{
					idFile = a.idFile;
					idTopic = a.id;
					textTitle.SetText(a.title);
					BuildPages();
					ShowPage();
				}
			},
			onInstantiate = delegate(Item a, ButtonCategory b)
			{
				b.mainText.text = a.title;
				bool flag = false;
				if (a.items.Count > 0)
				{
					foreach (Item item4 in a.items)
					{
						if (item4.idFile == idFile && item4.id == idTopic)
						{
							flag = true;
						}
					}
				}
				b.SetFold(a.items.Count > 0, !flag, delegate(UIList l)
				{
					lists.Add(l);
					foreach (Item item5 in a.items)
					{
						l.Add(item5);
					}
				});
			},
			onRefresh = null
		};
		string oldValue = '\t'.ToString();
		Item item = null;
		Item item2 = null;
		helpTitles.Clear();
		string[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			string[] array3 = array2[i].Replace(oldValue, "").Split(',');
			string[] array4 = array3[0].Split('-');
			Item item3 = new Item
			{
				idFile = array4[0],
				id = array4[1],
				title = array3[1]
			};
			helpTitles[array3[0]] = item3.title.Replace("$", "");
			if (item3.title.StartsWith("$"))
			{
				item = item3;
				item3.title = item3.title.TrimStart('$');
				list.Add(item3);
			}
			else if (item != null)
			{
				item.items.Add(item3);
			}
			else
			{
				list.Add(item3);
			}
			if (item3.idFile == idFirstFile && item3.id == idFirstTopic)
			{
				item2 = item3;
				idFirstFile = null;
			}
		}
		list.Refresh();
		if (item2 != null)
		{
			foreach (UIList item6 in lists)
			{
				item6.Select(item2, invoke: true);
			}
		}
		else if (list.children.Count > 0)
		{
			list.children.FirstItem().Select(0, invoke: true);
		}
		else
		{
			list.Select(0, invoke: true);
		}
		SkinManager.tempSkin = null;
	}

	public void ShowPage(int idx = 0)
	{
		if ((bool)note2)
		{
			ShowPage(idx, note, textPage, buttonPrev);
			ShowPage(idx + 1, note2, textPage2, buttonNext);
		}
		else
		{
			ShowPage(idx, note, textPage);
		}
	}

	public void ShowPage(int idx, UINote n, UIText textPage = null, UIButton button = null)
	{
		if ((bool)transSearchResult)
		{
			transSearchResult.SetActive(enable: false);
			showSearchResult = false;
		}
		bool flag = pages.Count > idx;
		n.Clear();
		if ((bool)textPage)
		{
			textPage.SetActive(flag);
		}
		if ((bool)button)
		{
			button.SetActive(flag);
		}
		if (flag)
		{
			idx = Mathf.Clamp(idx, 0, pages.Count - 1);
			Page page = pages[idx];
			if (page.idx == 0 && bookItem != null)
			{
				n.AddHeader("HeaderNoteBook", bookItem.title).text2.SetText(bookItem.author);
			}
			page.BuildNote(n, idTopic);
			n.RebuildLayout(recursive: true);
			n.Build();
			n.RebuildLayoutTo<Layer>();
			if ((bool)textPage)
			{
				textPage.SetText(page.idx + 1 + " / " + pages.Count);
			}
			LayoutGroup[] componentsInChildren = n.transform.parent.GetComponentsInChildren<LayoutGroup>();
			foreach (LayoutGroup obj in componentsInChildren)
			{
				obj.CalculateLayoutInputHorizontal();
				obj.CalculateLayoutInputVertical();
				obj.SetLayoutHorizontal();
				obj.SetLayoutVertical();
			}
			this.RebuildLayout(recursive: true);
			if ((bool)layoutButton)
			{
				layoutButton.RebuildLayout(recursive: true);
			}
			if ((bool)scrollbar)
			{
				scrollbar.value = 1f;
			}
			searchWord = "";
		}
	}

	public void AddButtonClose()
	{
		if ((bool)window)
		{
			AddButton("close", window.layer.Close);
		}
	}

	public UIButton AddButton(string lang, Action action)
	{
		UIButton uIButton = Util.Instantiate(moldButton, layoutButton);
		uIButton.mainText.SetText(lang.lang());
		uIButton.onClick.AddListener(delegate
		{
			action();
		});
		return uIButton;
	}
}
