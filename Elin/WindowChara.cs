using System.Collections.Generic;
using System.Linq;
using PrimitiveUI.Examples;
using UnityEngine;
using UnityEngine.UI;

public class WindowChara : WindowController
{
	public static WindowChara Instance;

	public Chara chara;

	public Portrait portrait;

	public UIText textMood;

	public UIText textHealth;

	public UIText textName;

	public UIText textAlias;

	public UIText textTitle;

	public UIText textBio;

	public UIText textBio2;

	public UIText textBio3;

	public UIText textBirthday;

	public UIText textFaction;

	public UIText textFaith;

	public UIText textHome;

	public UIText textMom;

	public UIText textDad;

	public UIText textBirthplace;

	public UIText textLike;

	public UIText textHobby;

	public UIText textKarma;

	public UIText textSAN;

	public UIText textFame;

	public UIText textMoney;

	public UIText textTax;

	public UIText textTerritory;

	public UIText textIncome;

	public UIText textAssets;

	public UIText textDeposit;

	public UIText textDeepest;

	public UIText textCurrentZone;

	public UIText textHomeZone;

	public UIList listAttaribute;

	public UIList listEquipment;

	public UIList listEquipment2;

	public UINote note;

	public PUIExampleRPGStats graph;

	public UIText textStability;

	public UIText textLaw;

	public UIText textAffection;

	public UIText textDominance;

	public UIText textExtroversion;

	public UIText textFavArmor;

	public UIText textDV;

	public UIText textStyle;

	public LayoutGroup layoutNeeds;

	public LayoutGroup layoutStatus;

	public GridLayoutGroup gridEquip;

	public UIScrollView scrollSkill;

	public ButtonElement buttonMana;

	public ButtonElement buttonLife;

	public ButtonElement buttonVigor;

	public ButtonElement buttonSpeed;

	public UIItem moldStats;

	public UIItem moldNeeds;

	public UIHeader moldHeader;

	public UIHeader moldHeader2;

	public UIHeader headerEquip;

	public UIList moldListSkill;

	public UIList moldListResist;

	public UIList moldListFeat;

	public UIList moldListFeatPurchase;

	public Transform contentList;

	public RectTransform rectEquip;

	public UIButton buttonFeatMode;

	public UIButton buttonFeatOption;

	public Image imageView;

	public Sprite mask;

	public Sprite maskResist;

	public Material matItem;

	public Color colorUnequipped;

	public Vector2 equipPos;

	public bool featMode;

	public float barColor1;

	public float barColor2;

	private UIHeader _header;

	public Biography bio => chara.bio;

	public void SetChara(Chara c)
	{
		Instance = this;
		chara = c;
		portrait.interactable = chara.IsPC;
		OnSwitchContent(window);
		if (window.setting.tabs.Count > 6)
		{
			window.setting.tabs[6].button.SetActive(chara.IsPC);
		}
		if (window.idTab == 6 && !chara.IsPC)
		{
			window.SwitchContent(0);
		}
	}

	private void Update()
	{
		rectEquip.anchoredPosition = new Vector2(scrollSkill.content.anchoredPosition.x + equipPos.x, equipPos.y);
	}

	public override void OnSwitchContent(Window window)
	{
		if (chara != null)
		{
			RefreshStatic();
			int idTab = window.idTab;
			if (idTab == 0)
			{
				RefreshInfo();
				RefreshProfile();
			}
			else
			{
				RefreshSkill(idTab);
			}
			bool flag = idTab == 3 || idTab == 4;
			scrollSkill.horizontal = flag;
			scrollSkill.content.anchoredPosition = new Vector2(0f, 0f);
			imageView.sprite = (flag ? maskResist : mask);
			if (flag)
			{
				RefreshEquipment(listEquipment2);
			}
			listEquipment2.transform.parent.SetActive(flag);
			buttonFeatMode.SetActive(idTab == 2 && chara.IsPC);
			buttonFeatOption.SetActive(idTab == 2 && chara.IsPC && featMode);
		}
	}

	public void SetPortraitBG(int a)
	{
		string currentId = chara.GetStr(23) ?? "BG_1";
		currentId = Portrait.modPortraitBGs.GetNextID(currentId, a);
		if (currentId == "BG_1")
		{
			currentId = null;
		}
		chara.SetStr(23, currentId);
		RefreshStatic();
	}

	public void SetPortraitFrame(int a)
	{
		string currentId = chara.GetStr(24) ?? "BGF_1";
		currentId = Portrait.modPortraitBGFs.GetNextID(currentId, a);
		if (currentId == "BGF_1")
		{
			currentId = null;
		}
		chara.SetStr(24, currentId);
		RefreshStatic();
	}

	public void ToggleFeatMode()
	{
		featMode = !featMode;
		RefreshSkill(window.idTab);
		buttonFeatOption.SetActive(featMode);
		buttonFeatOption.SetToggle(EClass.game.config.showAllFeat, delegate(bool a)
		{
			EClass.game.config.showAllFeat = a;
			RefreshSkill(window.idTab);
		});
	}

	public void RefreshFeatMode()
	{
		buttonFeatMode.mainText.text = "featMode".lang(EClass.pc.feat.ToString() ?? "");
		ColorBlock colors = buttonFeatMode.colors;
		colors.normalColor = colors.normalColor.SetAlpha(featMode ? 1 : 0);
		buttonFeatMode.colors = colors;
	}

	public void Refresh()
	{
		OnSwitchContent(window);
	}

	public void RefreshStatic()
	{
		textHealth.text = chara.hp + "/" + chara.MaxHP;
		textMood.text = chara.mana.value + "/" + chara.mana.max;
		portrait.SetChara(chara);
		Sprite @object = Portrait.modPortraitBGs.GetItem(chara.GetStr(23) ?? "BG_1").GetObject();
		@object.texture.filterMode = FilterMode.Bilinear;
		portrait.image.sprite = @object;
		@object = Portrait.modPortraitBGFs.GetItem(chara.GetStr(24) ?? "BGF_1").GetObject();
		@object.texture.filterMode = FilterMode.Bilinear;
		portrait.imageFrame.sprite = @object;
		string text = Lang.GetList("difficulties_title")[EClass.game.idDifficulty];
		window.SetCaption(((text.IsEmpty() || !chara.IsPC) ? "" : (text + " - ")) + chara.NameBraced);
		listAttaribute.Clear();
		listAttaribute.callbacks = new UIList.Callback<Element, ButtonElement>
		{
			onClick = delegate(Element a, ButtonElement b)
			{
				WidgetTracker.Toggle(a);
				RefreshStatic();
			},
			onInstantiate = delegate(Element a, ButtonElement b)
			{
				b.SetElement(a, chara.elements, ButtonElement.Mode.Attribute);
			}
		};
		List<Element> list = chara.elements.ListElements((Element a) => a.HasTag("primary"));
		list.Sort((Element a, Element b) => a.source.sort - b.source.sort);
		foreach (Element item in list)
		{
			listAttaribute.Add(item);
		}
		listAttaribute.Refresh();
	}

	public void RefreshStatus()
	{
		layoutStatus.DestroyChildren();
		foreach (Stats item in new List<Stats> { chara.hunger, chara.burden, chara.stamina, chara.depression, chara.bladder, chara.hygiene })
		{
			AddStatus(item);
		}
		foreach (Condition condition in chara.conditions)
		{
			AddStatus(condition);
		}
		if (layoutStatus.transform.childCount == 1)
		{
			AddStatus(null);
		}
		layoutStatus.RebuildLayout();
	}

	public void AddNeeds(Stats st)
	{
		UIItem uIItem = Util.Instantiate(moldNeeds, layoutNeeds);
		uIItem.text1.SetText(st.source.GetName());
		if (st.GetText().IsEmpty())
		{
			uIItem.text2.SetActive(enable: false);
		}
		else
		{
			st.SetText(uIItem.text2);
		}
		UIBar componentInChildren = uIItem.GetComponentInChildren<UIBar>();
		componentInChildren.Refresh(st.value, st.max);
		componentInChildren.image.color = st.GetColor().Multiply(barColor1, barColor2);
	}

	public void AddStatus(BaseStats st)
	{
		string text = st?.GetText() ?? "noItem".lang();
		if (text.IsEmpty())
		{
			return;
		}
		UIItem uIItem = Util.Instantiate(moldStats, layoutStatus);
		if (st == null)
		{
			uIItem.text1.SetText(text, FontColor.Passive);
			uIItem.button1.interactable = false;
		}
		else if (text.IsEmpty())
		{
			uIItem.text1.SetText(st.source.GetName());
		}
		else
		{
			st.SetText(uIItem.text1);
			uIItem.image1.sprite = st.GetSprite();
			uIItem.image1.SetNativeSize();
		}
		if (st != null)
		{
			uIItem.button1.SetTooltip(delegate(UITooltip t)
			{
				st.WriteNote(t.note);
			});
		}
	}

	public void RefreshProfile()
	{
		RefreshStatus();
		textName.text = chara.NameSimple;
		textAlias.text = chara.Aka;
		textTitle.text = (chara.IsPC ? EClass.player.title : "-");
		textCurrentZone.text = ((chara.currentZone == null) ? "???" : chara.currentZone.Name);
		textHomeZone.text = ((chara.homeZone == null) ? "???" : chara.homeZone.Name);
		textBio.text = chara.job.GetText().ToTitleCase();
		textBio2.text = bio.TextBio(chara);
		textBio3.text = bio.TextBio2(chara);
		textDV.text = "_DV".lang(chara.DV.ToString() ?? "", chara.PV.ToString() ?? "");
		textStyle.text = "_style".lang(Lang._weight(chara.body.GetWeight(armorOnly: true)) ?? "", chara.elements.GetOrCreateElement(chara.GetArmorSkill()).Name, ("style" + chara.body.GetAttackStyle()).lang());
		textKarma.text = (chara.IsPC ? (EClass.player.karma.ToString() ?? "") : "???");
		textSAN.text = chara.SAN.value.ToString() ?? "";
		textFame.text = (chara.IsPC ? (EClass.player.fame.ToString() ?? "") : "???");
		textMoney.text = Lang._currency(chara.GetCurrency(), showUnit: true);
		textDeposit.text = (chara.IsPC ? Lang._currency(EClass.game.cards.container_deposit.GetCurrency(), showUnit: true) : "???");
		string text = "deepestLv2".lang((chara.IsPCFaction ? EClass.player.stats.deepest : chara.LV).ToString() ?? "");
		if (chara.IsPCFaction && EClass.player.CountKeyItem("license_void") > 0)
		{
			text = text + " " + "deepestLv3".lang(Mathf.Abs(EClass.game.spatials.Find("void").GetDeepestLv()).ToString() ?? "");
		}
		textDeepest.text = text;
		textAssets.text = (chara.IsPC ? "tGameTime".lang(EClass.player.stats.days.ToFormat(), EClass.player.stats.turns.ToFormat()) : "???");
		textTerritory.text = (chara.IsPC ? (EClass.pc.faction.CountTerritories().ToString() ?? "") : "???");
		buttonLife.SetElement(chara.elements.GetOrCreateElement(60), chara.elements, ButtonElement.Mode.OnlyValue);
		buttonMana.SetElement(chara.elements.GetOrCreateElement(61), chara.elements, ButtonElement.Mode.OnlyValue);
		buttonVigor.SetElement(chara.elements.GetOrCreateElement(62), chara.elements, ButtonElement.Mode.OnlyValue);
		buttonSpeed.SetElement(chara.elements.GetOrCreateElement(79), chara.elements, ButtonElement.Mode.OnlyValue);
		RefreshEquipment(listEquipment);
	}

	public void RefreshEquipment(UIList list, bool sort = true)
	{
		string text = chara.GetFavWeaponSkill()?.Name ?? Element.Get(100).GetText();
		text = text + "/" + (chara.GetFavArmorSkill()?.Name ?? Element.Get(120).GetText());
		textFavArmor.SetText(text);
		list.Clear();
		list.callbacks = new UIList.Callback<BodySlot, UIItem>
		{
			onClick = delegate
			{
			},
			onInstantiate = delegate(BodySlot a, UIItem b)
			{
				if (a.elementId == 0)
				{
					b.button1.interactable = false;
				}
				else
				{
					Thing thing = (Thing)(b.refObj = a.thing);
					if (thing != null)
					{
						b.button1.icon.material = matItem;
						thing.SetImage(b.button1.icon);
					}
					else
					{
						b.button1.icon.material = null;
						b.button1.icon.sprite = SpriteSheet.Get("Media/Graphics/Icon/Element/", "eq_" + a.element.alias);
						b.button1.icon.color = colorUnequipped;
						b.button1.icon.SetNativeSize();
					}
					b.button1.SetTooltip(delegate(UITooltip tt)
					{
						if (a.thing != null)
						{
							a.thing.WriteNote(tt.note);
						}
						else
						{
							tt.note.Clear();
							tt.note.AddHeader(a.name);
							tt.note.AddText("noEQ".lang());
							if (a.elementId == 35)
							{
								Thing.AddAttackEvaluation(tt.note, chara);
							}
						}
						tt.note.Build();
					});
				}
			},
			onSort = (BodySlot a, UIList.SortMode m) => (a.element.id != 0) ? chara.body.GetSortVal(a) : (-99999)
		};
		foreach (BodySlot slot in chara.body.slots)
		{
			if (slot.elementId != 44)
			{
				list.Add(slot);
			}
		}
		if (list.items.Count < 12)
		{
			int num = 12 - list.items.Count;
			for (int i = 0; i < num; i++)
			{
				list.Add(new BodySlot());
			}
		}
		if (list.items.Count > 18)
		{
			gridEquip.cellSize = new Vector2(52f, 44f);
			gridEquip.spacing = new Vector2(-6f, -12f);
		}
		if (sort)
		{
			list.Sort();
		}
		list.Refresh();
	}

	public void RefreshInfo()
	{
		textBirthday.text = bio.TextBirthDate(chara);
		textMom.text = chara.bio.nameMom.ToTitleCase();
		textDad.text = chara.bio.nameDad.ToTitleCase();
		textBirthplace.text = chara.bio.nameBirthplace.ToTitleCase();
		textLike.text = EClass.sources.cards.map.TryGetValue(bio.idLike, EClass.sources.cards.map["ash"]).GetName();
		textHobby.text = EClass.sources.elements.map[bio.idHobby].GetText();
		textFaction.text = ((chara.faction == null) ? "???" : chara.faction.name.ToTitleCase());
		textFaith.text = chara.faith.Name.ToTitleCase();
	}

	public void OnClickPortrait()
	{
		if (chara.IsPC)
		{
			EClass.ui.AddLayer<LayerEditPCC>().Activate(chara, UIPCC.Mode.Body, null, delegate
			{
				portrait.SetChara(chara);
			});
		}
		else
		{
			SE.Beep();
		}
	}

	public void RefreshSkill(int idTab)
	{
		contentList.DestroyChildren();
		note.Clear();
		note.RebuildLayout();
		UIList list = default(UIList);
		if (idTab == 1)
		{
			List("skillsGeneral", "general");
			List("skillsCraft", "craft");
			List("skillsCombat", "combat");
			List("skillsWeapon", "weapon");
		}
		else if (idTab == 2)
		{
			RefreshFeatMode();
			if (featMode)
			{
				List("availableFeats", "general");
				List("availableFeats_special", "special");
				List("availableFeats_skill", "skill");
				List("availableFeats_attribute", "attribute");
				return;
			}
			Header("mutation", null);
			ListFeat();
			list.callbacks = new UIList.Callback<Feat, ButtonElement>
			{
				onClick = delegate(Feat a, ButtonElement b)
				{
					WidgetTracker.Toggle(a);
					b.SetElement(a, chara.elements, ButtonElement.Mode.Feat);
				},
				onInstantiate = delegate(Feat a, ButtonElement b)
				{
					b.SetElement(a, chara.elements, ButtonElement.Mode.Feat);
				},
				onList = delegate
				{
					foreach (Element item in chara.elements.ListElements((Element a) => a.source.category == "mutation" && a.Value != 0))
					{
						list.Add(item);
					}
				}
			};
			list.List();
			if (list.items.Count == 0)
			{
				_header.SetActive(enable: false);
				list.SetActive(enable: false);
			}
			Header("etherDisease", null);
			ListFeat();
			list.callbacks = new UIList.Callback<Feat, ButtonElement>
			{
				onClick = delegate(Feat a, ButtonElement b)
				{
					WidgetTracker.Toggle(a);
					b.SetElement(a, chara.elements, ButtonElement.Mode.Feat);
				},
				onInstantiate = delegate(Feat a, ButtonElement b)
				{
					b.SetElement(a, chara.elements, ButtonElement.Mode.Feat);
				},
				onList = delegate
				{
					foreach (Element item2 in chara.elements.ListElements((Element a) => a.source.category == "ether" && a.Value != 0))
					{
						list.Add(item2);
					}
				}
			};
			list.List();
			if (list.items.Count == 0)
			{
				_header.SetActive(enable: false);
				list.SetActive(enable: false);
			}
			Header("innateFeats", null);
			ListFeat();
			list.callbacks = new UIList.Callback<Feat, ButtonElement>
			{
				onClick = delegate(Feat a, ButtonElement b)
				{
					WidgetTracker.Toggle(a);
					b.SetElement(a, chara.elements, ButtonElement.Mode.Feat);
				},
				onInstantiate = delegate(Feat a, ButtonElement b)
				{
					b.SetElement(a, chara.elements, ButtonElement.Mode.Feat);
				},
				onList = delegate
				{
					foreach (Element item3 in chara.elements.ListElements((Element a) => a.source.category == "feat" && a.HasTag("innate") && a.Value != 0))
					{
						list.Add(item3);
					}
				}
			};
			list.List();
			Header("feats", null);
			ListFeat();
			list.callbacks = new UIList.Callback<Feat, ButtonElement>
			{
				onClick = delegate(Feat a, ButtonElement b)
				{
					WidgetTracker.Toggle(a);
					b.SetElement(a, chara.elements, ButtonElement.Mode.Feat);
				},
				onInstantiate = delegate(Feat a, ButtonElement b)
				{
					b.SetElement(a, chara.elements, ButtonElement.Mode.Feat);
				},
				onList = delegate
				{
					foreach (Element item4 in chara.elements.ListElements((Element a) => a.source.category == "feat" && !a.HasTag("innate") && a.Value != 0))
					{
						list.Add(item4);
					}
				},
				onSort = (Feat a, UIList.SortMode m) => a.GetSortVal(m)
			};
			list.ChangeSort(UIList.SortMode.ByID);
			list.List();
		}
		else if (idTab == 3)
		{
			Header("resistance", null);
			headerEquip.SetText("resistance".lang());
			ListResist();
			list.callbacks = new UIList.Callback<Element, ButtonElement>
			{
				onInstantiate = delegate(Element a, ButtonElement b)
				{
					b.SetGrid(a, chara);
				},
				onList = delegate
				{
					foreach (SourceElement.Row item5 in EClass.sources.elements.rows.Where((SourceElement.Row a) => a.category == "resist" && ((!a.tag.Contains("hidden") && !a.tag.Contains("high")) || chara.Evalue(a.id) != 0)))
					{
						list.Add(chara.elements.GetOrCreateElement(item5.id));
					}
				}
			};
			list.List();
		}
		else if (idTab == 4)
		{
			chara.elements.GetOrCreateElement(78);
			List<Element> eles = chara.elements.ListElements(delegate(Element a)
			{
				if (a.source.tag.Contains("godAbility") || a.source.categorySub == "god")
				{
					return false;
				}
				if (a.IsFlag)
				{
					if (a.Value == 0)
					{
						return false;
					}
				}
				else
				{
					switch (a.id)
					{
					case 55:
					case 56:
					case 57:
					case 68:
					case 90:
					case 91:
					case 93:
						if (a.vSource != 0)
						{
							return true;
						}
						break;
					}
					if ((a.owner == chara.elements && a.vLink == 0 && !a.IsFactionElement(chara)) || a.source.category == "resist")
					{
						return false;
					}
				}
				return true;
			});
			eles.Sort((Element a, Element b) => a.SortVal(charaSheet: true) - b.SortVal(charaSheet: true));
			string[] skillCats = new string[7] { "general", "labor", "mind", "stealth", "combat", "craft", "weapon" };
			Header("enchant", null);
			headerEquip.SetText("enchant".lang());
			ListResist();
			list.callbacks = new UIList.Callback<Element, ButtonElement>
			{
				onInstantiate = delegate(Element a, ButtonElement b)
				{
					b.SetGrid(a, chara);
				},
				onList = delegate
				{
					foreach (Element item6 in eles)
					{
						if (!skillCats.Contains(item6.source.categorySub))
						{
							list.Add(item6);
						}
					}
				}
			};
			list.List();
			Header("skill", null);
			ListResist();
			list.callbacks = new UIList.Callback<Element, ButtonElement>
			{
				onInstantiate = delegate(Element a, ButtonElement b)
				{
					b.SetGrid(a, chara);
				},
				onList = delegate
				{
					foreach (Element item7 in eles)
					{
						if (skillCats.Contains(item7.source.categorySub))
						{
							list.Add(item7);
						}
					}
				}
			};
			list.List();
		}
		else if (idTab == 5)
		{
			Header("note", null);
			note.transform.SetAsLastSibling();
			RefreshNote(chara, note);
		}
		void Header(string lang, string lang2)
		{
			_header = Util.Instantiate(moldHeader, contentList);
			_header.SetText(lang);
			if (lang2 != null)
			{
				Util.Instantiate(moldHeader2, contentList).SetText(lang2);
			}
		}
		void List(string lang, string idSubCat)
		{
			List<string> cats = new List<string>();
			cats.Add(idSubCat);
			if (idSubCat == "general")
			{
				cats.Add("labor");
				cats.Add("mind");
				cats.Add("stealth");
			}
			Header(lang, null);
			ListSkill();
			list.callbacks = new UIList.Callback<Element, ButtonElement>
			{
				onClick = delegate(Element a, ButtonElement b)
				{
					WidgetTracker.Toggle(a);
					b.SetElement(a, chara.elements);
				},
				onInstantiate = delegate(Element a, ButtonElement b)
				{
					b.SetElement(a, chara.elements);
				},
				onList = delegate
				{
					foreach (Element item8 in chara.elements.ListElements((Element a) => (a.Value != 0 || a.ValueWithoutLink != 0 || a.vSource > 0) && a.source.category == "skill" && cats.Contains(a.source.categorySub)))
					{
						list.Add(item8);
					}
				},
				onSort = (Element c, UIList.SortMode m) => EClass.sources.elements.alias[c.source.aliasParent].id * -10000 - c.id
			};
			list.List(UIList.SortMode.ByElementParent);
		}
		void List(string lang, string idSubCat)
		{
			Header(lang, "cost");
			ListFeatPurchase();
			UIList _list = list;
			_list.callbacks = new UIList.Callback<Element, ButtonElement>
			{
				onClick = delegate(Element a, ButtonElement b)
				{
					if (!a.IsPurchaseFeatReqMet(chara.elements) && !EClass.debug.enable)
					{
						SE.BeepSmall();
					}
					else if (EClass.pc.feat < a.CostLearn)
					{
						SE.BeepSmall();
						Msg.Say("notEnoughFeatPoint");
					}
					else
					{
						Dialog.YesNo("dialogBuyFeat".lang(a.CostLearn.ToString() ?? "", a.FullName), delegate
						{
							EClass.pc.feat -= a.CostLearn;
							EClass.pc.SetFeat(a.id, a.Value, msg: true);
							RefreshSkill(idTab);
							RefreshFeatMode();
							RefreshStatic();
							this.RebuildLayout(recursive: true);
						});
					}
				},
				onInstantiate = delegate(Element a, ButtonElement b)
				{
					b.SetElement(a, chara.elements, ButtonElement.Mode.FeatPurchase);
				},
				onList = delegate
				{
					foreach (Element item9 in chara.ListAvailabeFeats(pet: false, EClass.game.config.showAllFeat))
					{
						if (item9.source.categorySub == idSubCat)
						{
							_list.Add(item9);
						}
					}
				}
			};
			_list.List();
		}
		void ListFeat()
		{
			list = Util.Instantiate(moldListFeat, contentList);
		}
		void ListFeatPurchase()
		{
			list = Util.Instantiate(moldListFeatPurchase, contentList);
		}
		void ListResist()
		{
			list = Util.Instantiate(moldListResist, contentList);
		}
		void ListSkill()
		{
			list = Util.Instantiate(moldListSkill, contentList);
		}
	}

	private void OnApplicationFocus(bool focus)
	{
		if (focus && window.idTab == 5)
		{
			RefreshNote(chara, note);
		}
	}

	public static void RefreshNote(Chara chara, UINote n, bool shortMode = false)
	{
		n.Clear();
		Biography biography = chara.bio;
		if (shortMode)
		{
			n.AddText(biography.TextBio(chara) + " " + biography.TextBio2(chara));
			n.Space();
		}
		else
		{
			UIItem uIItem = n.AddItem("ItemBackground");
			if (chara.IsPC)
			{
				uIItem.text1.SetText(EClass.player.GetBackgroundText());
				uIItem.button1.SetOnClick(delegate
				{
					EClass.player.EditBackgroundText();
				});
			}
			else
			{
				uIItem.text1.SetText("???");
				uIItem.button1.SetActive(enable: false);
			}
			n.Space(16);
			n.AddTopic("TopicDomain", "profile".lang(), biography.TextBio(chara) + " " + biography.TextBio2(chara));
		}
		string text = "";
		ElementContainer elementContainer = (chara.IsPC ? EClass.player.GetDomains() : new ElementContainer().ImportElementMap(chara.job.domain));
		foreach (Element value in elementContainer.dict.Values)
		{
			text = text + ((value == elementContainer.dict.Values.First()) ? "" : ", ") + value.Name;
		}
		n.AddTopic("TopicDomain", "domain".lang(), text).button1.SetActive(enable: false);
		string text2 = chara.GetFavCat().GetName();
		string @ref = chara.GetFavFood().GetName();
		Add("favgift".lang(text2.ToLower().ToTitleCase(), @ref));
		Add(chara.GetTextHobby());
		Add(chara.GetTextWork());
		if (chara.IsPC)
		{
			n.AddTopic("TopicDomain", "totalFeat".lang(), EClass.player.totalFeat.ToString() ?? "");
		}
		text = chara.GetFavWeaponSkill()?.Name ?? Element.Get(100).GetText();
		text = text + " / " + ("style" + chara.GetFavAttackStyle()).lang();
		n.AddTopic("TopicDomain", "attackStyle".lang(), text);
		n.AddTopic("TopicDomain", "armorStyle".lang(), chara.GetFavArmorSkill()?.Name ?? Element.Get(120).GetText());
		if (chara.IsPC && EClass.pc.c_daysWithGod > 0)
		{
			AddText("info_daysWithGod".lang(EClass.pc.c_daysWithGod.ToString() ?? "", EClass.pc.faith.Name));
		}
		if (chara.ride != null)
		{
			AddText("info_ride".lang(chara.ride.NameBraced));
		}
		if (chara.parasite != null)
		{
			AddText("info_parasite".lang(chara.parasite.NameBraced));
		}
		if (EClass.player.IsCriminal)
		{
			AddText("info_criminal".lang());
		}
		if (EClass.debug.showExtra)
		{
			n.AddText("LV:" + chara.LV + "  exp:" + chara.exp + " next:" + chara.ExpToNext);
			n.AddText("Luck:" + chara.Evalue(78));
		}
		n.Build();
		void Add(string s)
		{
			string[] array = s.Split(':');
			n.AddTopic("TopicDomain", array[0], (array.Length >= 2) ? array[1].TrimStart(' ') : "").button1.SetActive(enable: false);
		}
		void AddText(string s)
		{
			n.AddText(" ・ " + s);
		}
	}
}
