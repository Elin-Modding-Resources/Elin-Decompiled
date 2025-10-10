using System.Linq;
using UnityEngine;

public class WindowCharaMini : WindowController
{
	public Chara chara;

	public UIHeader moldHeader;

	public UIHeader moldHeader2;

	public UIText textName;

	public UIText textInfo1;

	public UIText textInfo2;

	public UIList moldListResist;

	public UIList moldListFeat;

	public Transform contentList;

	public UINote note;

	public UIList listEq;

	public Material matItem;

	public Color colorUnequipped;

	private UIHeader _header;

	public void SetChara(Chara c)
	{
		chara = c;
		Refresh(0);
	}

	public override void OnSwitchContent(Window window)
	{
		if (chara != null)
		{
			Refresh(window.idTab);
		}
	}

	public void Refresh(int idTab)
	{
		if (chara == null)
		{
			return;
		}
		contentList.DestroyChildren();
		note.Clear();
		note.SetActive(enable: false);
		textName.SetText(chara.NameSimple);
		textInfo1.text = "_DV".lang(chara.DV.ToString() ?? "", chara.PV.ToString() ?? "");
		textInfo2.text = "_style".lang(Lang._weight(chara.body.GetWeight(armorOnly: true)) ?? "", chara.elements.GetOrCreateElement(chara.GetArmorSkill()).Name, ("style" + chara.body.GetAttackStyle()).lang());
		UIList list = default(UIList);
		if (idTab == 0)
		{
			Header("resistance", null);
			ListResist();
			list.callbacks = new UIList.Callback<Element, ButtonElement>
			{
				onInstantiate = delegate(Element a, ButtonElement b)
				{
					b.SetGrid(a, chara);
				},
				onList = delegate
				{
					foreach (SourceElement.Row item in EClass.sources.elements.rows.Where((SourceElement.Row a) => a.category == "resist" && ((!a.tag.Contains("hidden") && !a.tag.Contains("high")) || chara.Evalue(a.id) != 0)))
					{
						list.Add(chara.elements.GetOrCreateElement(item.id));
					}
				}
			};
			list.List();
		}
		else if (idTab == 1)
		{
			Header("mutation", null);
			ListFeat();
			list.callbacks = new UIList.Callback<Feat, ButtonElement>
			{
				onClick = delegate(Feat a, ButtonElement b)
				{
					WidgetTracker.Toggle(a);
				},
				onInstantiate = delegate(Feat a, ButtonElement b)
				{
					b.SetElement(a, chara.elements, ButtonElement.Mode.FeatMini);
				},
				onList = delegate
				{
					foreach (Element item2 in chara.elements.ListElements((Element a) => a.source.category == "mutation" && a.Value != 0))
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
			Header("etherDisease", null);
			ListFeat();
			list.callbacks = new UIList.Callback<Feat, ButtonElement>
			{
				onClick = delegate(Feat a, ButtonElement b)
				{
					WidgetTracker.Toggle(a);
				},
				onInstantiate = delegate(Feat a, ButtonElement b)
				{
					b.SetElement(a, chara.elements, ButtonElement.Mode.FeatMini);
				},
				onList = delegate
				{
					foreach (Element item3 in chara.elements.ListElements((Element a) => a.source.category == "ether" && a.Value != 0))
					{
						list.Add(item3);
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
				},
				onInstantiate = delegate(Feat a, ButtonElement b)
				{
					b.SetElement(a, chara.elements, ButtonElement.Mode.FeatMini);
				},
				onList = delegate
				{
					foreach (Element item4 in chara.elements.ListElements((Element a) => a.source.category == "feat" && a.HasTag("innate") && a.Value != 0))
					{
						list.Add(item4);
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
				},
				onInstantiate = delegate(Feat a, ButtonElement b)
				{
					b.SetElement(a, chara.elements, ButtonElement.Mode.FeatMini);
				},
				onList = delegate
				{
					foreach (Element item5 in chara.elements.ListElements((Element a) => a.source.category == "feat" && !a.HasTag("innate") && a.Value != 0))
					{
						list.Add(item5);
					}
				},
				onSort = (Feat a, UIList.SortMode m) => a.GetSortVal(m)
			};
			list.ChangeSort(UIList.SortMode.ByID);
			list.List();
		}
		else if (idTab == 2)
		{
			if (chara.c_upgrades == null)
			{
				chara.c_upgrades = new CharaUpgrade();
			}
			CharaUpgrade upgrades = chara.c_upgrades;
			CharaGenes genes = chara.c_genes;
			int num = upgrades.spent;
			if (genes != null)
			{
				num += genes.GetTotalCost();
			}
			note.SetActive(enable: true);
			note.AddText("feat_pet".lang(chara.feat.ToString() ?? "", num.ToString() ?? ""));
			note.AddText("feat_inferior".lang(chara.CurrentGeneSlot.ToString() ?? "", chara.MaxGeneSlot.ToString() ?? "", ((genes != null) ? genes.inferior : 0).ToString() ?? ""));
			note.AddText((chara.isScaled ? "feat_pet_type2" : "feat_pet_type1").lang());
			note.Space(8);
			foreach (CharaUpgrade.Item item6 in upgrades.items)
			{
				if (!Element.List_MainAttributesMajor.Contains(item6.idEle))
				{
					Element element = Element.Create(item6.idEle, item6.value);
					note.AddText("upgraded".lang(element.Name.ToTitleCase(), item6.value.ToString() ?? "", item6.cost.ToString() ?? ""));
				}
			}
			if (chara.c_genes != null)
			{
				foreach (DNA g in genes.items)
				{
					UIItem uIItem = note.AddItem("ItemGene");
					uIItem.button1.mainText.SetText(g.GetText());
					uIItem.button1.SetTooltip(delegate(UITooltip t)
					{
						UINote uINote = t.note;
						uINote.Clear();
						uINote.AddHeader(g.GetText());
						g.WriteNote(uINote);
						t.note.Build();
					});
					uIItem.button2.SetOnClick(delegate
					{
						if (!g.CanRemove())
						{
							SE.Beep();
						}
						else
						{
							Dialog.YesNo("dialog_removeGene", delegate
							{
								genes.inferior--;
								SE.Trash();
								genes.Remove(chara, g);
								Refresh(idTab);
								chara.RemoveAllStances();
							});
						}
					});
					uIItem.button2.SetActive(genes != null && genes.inferior > 0);
				}
			}
			if (EClass.debug.enable)
			{
				note.AddButton("add 20".lang(), delegate
				{
					chara.feat += 20;
					Refresh(idTab);
				});
				note.AddButton("grow 100".lang(), delegate
				{
					chara.feat += 100;
					chara.TryUpgrade();
					Refresh(idTab);
				});
			}
			if (upgrades.count > 0)
			{
				note.AddButton("feat_reset".lang(), delegate
				{
					upgrades.Reset(chara);
					SE.Trash();
					Refresh(idTab);
				});
			}
			note.Build();
		}
		else if (idTab == 3)
		{
			note.SetActive(enable: true);
			WindowChara.RefreshNote(chara, note, shortMode: true);
		}
		RefreshEquipment();
		void Header(string lang, string lang2)
		{
			_header = Util.Instantiate(moldHeader, contentList);
			_header.SetText(lang);
			if (lang2 != null)
			{
				Util.Instantiate(moldHeader2, contentList).SetText(lang2);
			}
		}
		void ListFeat()
		{
			list = Util.Instantiate(moldListFeat, contentList);
		}
		void ListResist()
		{
			list = Util.Instantiate(moldListResist, contentList);
		}
	}

	public void RefreshEquipment()
	{
		UIList uIList = listEq;
		uIList.Clear();
		uIList.callbacks = new UIList.Callback<BodySlot, UIItem>
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
			onSort = (BodySlot a, UIList.SortMode m) => -a.element.sort
		};
		foreach (BodySlot slot in chara.body.slots)
		{
			if (slot.elementId != 44)
			{
				uIList.Add(slot);
			}
		}
		uIList.Sort();
		uIList.Refresh();
	}
}
