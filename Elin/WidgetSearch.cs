using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class WidgetSearch : WidgetCodex
{
	public class Extra
	{
		[JsonProperty]
		public string lastSearch;

		[JsonProperty]
		public List<Word> words;
	}

	public class Word
	{
		[JsonProperty]
		public string text;

		[JsonProperty]
		public int type;
	}

	public static WidgetSearch Instance;

	public HashSet<Card> cards = new HashSet<Card>();

	public static Card selected;

	public CanvasGroup cgResult;

	public UIList listWords;

	public Zone lastZone;

	private bool isDirty;

	public Extra extra => base.config.extra as Extra;

	public override SearchType type => SearchType.Search;

	public override object CreateExtra()
	{
		return new Extra();
	}

	public override void OnActivate()
	{
		base.OnActivate();
		Instance = this;
		selected = null;
		if (extra.words == null)
		{
			extra.words = new List<Word>();
			string[] array = Lang.GetList("search_words");
			foreach (string text in array)
			{
				extra.words.Add(new Word
				{
					text = text
				});
			}
		}
		RefreshWords();
		RefreshList();
		if (!extra.lastSearch.IsEmpty())
		{
			field.text = extra.lastSearch;
		}
	}

	public override bool CheckClose()
	{
		if (Input.GetKey(KeyCode.LeftControl))
		{
			return Input.GetKeyDown(KeyCode.F);
		}
		return false;
	}

	private new void Update()
	{
		if (EMono.scene.actionMode.IsFuncPressed(CoreConfig.GameFunc.PropertySearch))
		{
			EMono.ui.widgets.DeactivateWidget(this);
			return;
		}
		base.Update();
		if (lastZone != EMono._zone || isDirty)
		{
			lastSearch = "";
			Search(field.text);
			isDirty = false;
		}
	}

	public void RefreshSearch()
	{
		SE.Tab();
		lastSearch = "";
		Search(field.text);
	}

	public override void Search(string s)
	{
		if (!s.IsEmpty())
		{
			extra.lastSearch = s;
		}
		s = s.ToLower();
		if (!buttonClear)
		{
			return;
		}
		buttonClear.SetActive(field.text != "");
		buttonRefresh.SetActive(field.text != "");
		if (s == lastSearch || s.Length == 0)
		{
			return;
		}
		lastSearch = s;
		lastZone = EMono._zone;
		bool encSearch = s.Length >= 2 && (s[0] == '@' || s[0] == '＠');
		if (encSearch)
		{
			s = s.Substring(1);
		}
		HashSet<Card> newCards = new HashSet<Card>();
		if (!s.IsEmpty())
		{
			if (!encSearch && (EMono._zone.IsTown || EMono._zone.HasLaw || EMono._zone.IsPCFaction || EMono._zone is Zone_Tent))
			{
				foreach (Chara chara in EMono._map.charas)
				{
					if (chara.IsNeutralOrAbove() && (chara.Name.ToLower().Contains(s) || chara.sourceCard.GetSearchName(jp: false).Contains(s)))
					{
						newCards.Add(chara);
					}
				}
			}
			if (EMono._zone.IsPCFaction || EMono._zone is Zone_Tent)
			{
				foreach (Thing thing2 in EMono._map.things)
				{
					if (encSearch)
					{
						if (thing2.MatchEncSearch(s))
						{
							newCards.Add(thing2);
						}
					}
					else if (thing2.Name.ToLower().Contains(s) || thing2.sourceCard.GetSearchName(jp: false).Contains(s))
					{
						newCards.Add(thing2);
					}
				}
				foreach (Card item in EMono._map.props.stocked.all)
				{
					if (!(item.parent is Thing thing) || !thing.trait.CanSearchContent)
					{
						continue;
					}
					if (encSearch)
					{
						if (item.MatchEncSearch(s))
						{
							newCards.Add(item);
						}
					}
					else if (item.Name.ToLower().Contains(s) || item.sourceCard.GetSearchName(jp: false).Contains(s))
					{
						newCards.Add(item);
					}
				}
			}
			for (int i = 0; i < 2; i++)
			{
				foreach (Chara chara2 in EMono._map.charas)
				{
					if ((i == 0 && chara2 == EMono.pc) || (i == 1 && chara2 != EMono.pc) || !chara2.IsPCFactionOrMinion)
					{
						continue;
					}
					chara2.things.Foreach(delegate(Thing t)
					{
						if (!((t.parent as Card)?.trait is TraitChestMerchant))
						{
							if (encSearch)
							{
								if (t.MatchEncSearch(s))
								{
									newCards.Add(t);
								}
							}
							else if (t.Name.ToLower().Contains(s) || t.source.GetSearchName(jp: false).Contains(s))
							{
								newCards.Add(t);
							}
						}
					});
				}
			}
		}
		if (!newCards.SetEquals(cards))
		{
			cards = newCards;
			RefreshList();
		}
		cgResult.alpha = ((list.ItemCount > 0) ? 1f : 0f);
	}

	public void RefreshWords()
	{
		listWords.callbacks = new UIList.Callback<Word, UIItem>
		{
			onClick = delegate(Word a, UIItem b)
			{
				if (a.type == 1)
				{
					if (field.text.IsEmpty())
					{
						SE.BeepSmall();
					}
					else
					{
						SE.ClickOk();
						extra.words.Add(new Word
						{
							text = field.text
						});
						listWords.List();
					}
				}
				else
				{
					field.text = a.text;
					SE.Tab();
				}
			},
			onInstantiate = delegate(Word a, UIItem b)
			{
				b.button1.mainText.SetText(a.text);
				b.button2.SetActive(a.type != 1);
				b.button2.SetOnClick(delegate
				{
					extra.words.Remove(a);
					SE.Trash();
					listWords.List();
				});
			},
			onList = delegate
			{
				foreach (Word word in extra.words)
				{
					listWords.Add(word);
				}
				if (extra.words.Count < 10)
				{
					listWords.Add(new Word
					{
						text = "add_search_word".lang(),
						type = 1
					});
				}
			}
		};
		listWords.List();
	}

	public override void RefreshList()
	{
		LayerInventory.SetDirtyAll();
		selected = null;
		list.callbacks = new UIList.Callback<Card, ButtonGrid>
		{
			onClick = delegate(Card a, ButtonGrid b)
			{
				Card rootCard = a.GetRootCard();
				if (rootCard == EMono.pc)
				{
					LayerInventory.SetDirtyAll();
					selected = a;
					SE.Click();
				}
				else if (EMono.pc.ai.IsNoGoal)
				{
					if (a.isDestroyed || !rootCard.pos.IsValid)
					{
						SE.Beep();
						Search(field.text);
					}
					else
					{
						SE.Click();
						selected = null;
						EMono.pc.SetAIImmediate(new AI_Goto(rootCard.pos.Copy(), 0));
						ActionMode.Adv.SetTurbo(3);
					}
				}
				else
				{
					SE.Beep();
				}
			},
			onRedraw = delegate(Card a, ButtonGrid b, int i)
			{
				if (a.isDestroyed)
				{
					isDirty = true;
				}
				b.SetCard(a, ButtonGrid.Mode.Search);
			},
			onList = delegate
			{
				foreach (Card card in cards)
				{
					list.Add(card);
				}
			}
		};
		list.List();
		list.dsv.OnResize();
	}

	public override void OnDeactivate()
	{
		base.OnDeactivate();
		if (field.text.IsEmpty())
		{
			extra.lastSearch = "";
		}
		LayerInventory.SetDirtyAll();
	}
}
