using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BaseListPeople : ListOwner<Chara, ItemGeneral>
{
	public Chara owner;

	public FactionMemberType memberType;

	public new LayerPeople layer => base.layer as LayerPeople;

	public override string IdHeaderRow => "HeaderRowPeople";

	public virtual bool ShowCharaSheet => false;

	public virtual bool ShowShowMode => false;

	public virtual LayerPeople.ShowMode ShowMode => layer.showMode;

	public virtual bool ShowGoto => false;

	public virtual bool ShowHome => false;

	public virtual bool IsDisabled(Chara c)
	{
		if (!c.isDead && c.memberType != FactionMemberType.Guest)
		{
			return !c.IsInHomeZone();
		}
		return false;
	}

	public override void List()
	{
		list.callbacks = new UIList.Callback<Chara, ItemGeneral>
		{
			onInstantiate = delegate(Chara a, ItemGeneral b)
			{
				b.SetChara(a, (this is ListPeopleBuySlave) ? ItemGeneral.Mode.Slave : ItemGeneral.Mode.Default);
				OnInstantiate(a, b);
				b.Build();
			},
			onClick = delegate(Chara c, ItemGeneral i)
			{
				OnClick(c, i);
			},
			onSort = delegate(Chara a, UIList.SortMode m)
			{
				a.SetSortVal(m);
				return -a.sortVal;
			},
			onList = delegate
			{
				OnList();
			},
			onRefresh = null
		};
		list.List();
	}

	public override void OnInstantiate(Chara a, ItemGeneral b)
	{
		SetSubText(a, b);
		if (ShowHome)
		{
			if (a.IsPCFaction)
			{
				Sprite icon = a.affinity.GetIcon();
				if ((bool)icon)
				{
					Util.Instantiate<UIItem>("UI/Element/Grid/Attach/affinity", b).image1.sprite = icon;
				}
			}
			AddSubButtonWork(b, a);
			Room room = a.FindRoom();
			TraitBed bed = a.FindBed();
			if (a.memberType == FactionMemberType.Default)
			{
				b.AddSubButton(EClass.core.refs.icons.home, delegate
				{
					if (room == null)
					{
						SE.BeepSmall();
					}
					else
					{
						EClass.pc.SetAI(new AI_Goto(room.GetRandomFreePos(), 1));
						layer.Close();
					}
				}, null, delegate(UITooltip t)
				{
					t.note.Clear();
					t.note.AddHeader("infoHome".lang((room != null) ? room.Name : "none".lang()));
					t.note.AddTopic("TopicLeft", "infoBed".lang(), (bed != null) ? bed.Name.ToTitleCase() : "none".lang());
					t.note.Build();
					if (room != null)
					{
						EClass.core.actionsNextFrame.Add(delegate
						{
						});
					}
				}).icon.SetAlpha((bed != null) ? 1f : 0.4f);
			}
		}
		if (ShowCharaSheet && EClass.debug.showExtra)
		{
			b.AddSubButton(EClass.core.refs.icons.inspect, delegate
			{
				SE.Play("pop_paper");
				LayerChara layerChara = EClass.ui.AddLayerDontCloseOthers<LayerChara>();
				layerChara.windows[0].SetRect(EClass.core.refs.rects.center);
				layerChara.SetChara(a);
			}, "charaInfo");
		}
		if (IsDisabled(a))
		{
			b.gameObject.AddComponent<CanvasGroup>().alpha = 0.6f;
		}
	}

	public void AddSubButtonWork(ItemGeneral b, Chara a)
	{
		BaseArea roomWork = a.GetRoomWork();
		bool flag = true;
		foreach (Hobby item in a.ListWorks().Concat(a.ListHobbies()))
		{
			if (item.GetEfficiency(a) <= 0)
			{
				flag = false;
				break;
			}
		}
		b.AddSubButton(EClass.core.refs.icons.work, delegate
		{
			if (roomWork == null)
			{
				SE.BeepSmall();
			}
			else
			{
				EClass.pc.SetAI(new AI_Goto(roomWork.GetRandomFreePos(), 1));
				layer.Close();
			}
		}, null, delegate(UITooltip t)
		{
			WriteHobbies(t, a, roomWork);
		}).icon.SetAlpha(flag ? 1f : 0.4f);
	}

	public void WriteHobbies(UITooltip t, Chara a, BaseArea roomWork)
	{
		t.note.Clear();
		t.note.AddHeader("infoWork".lang((roomWork != null) ? roomWork.Name : "none".lang()));
		foreach (Hobby item in a.ListWorks())
		{
			AddText(item, "work", a.source.works.Contains(item.source.alias));
		}
		foreach (Hobby item2 in a.ListHobbies())
		{
			AddText(item2, "hobby", a.source.hobbies.Contains(item2.source.alias));
		}
		if (a.IsPCParty)
		{
			t.note.Space(8);
			t.note.AddText("workNotActive_party".lang(), FontColor.Warning);
		}
		if (a.IsPCFactionOrMinion && !a.trait.IsCountAsResident)
		{
			t.note.AddText("notCountResident".lang(), FontColor.Good);
		}
		t.note.Build();
		void AddText(Hobby h, string lang, bool fix)
		{
			int efficiency = h.GetEfficiency(a);
			string text = h.Name;
			if (fix)
			{
				text += "*";
			}
			text = text.TagColor((efficiency == 0) ? FontColor.Warning : FontColor.Good);
			string[] array = Lang.GetList("work_lv");
			string text2 = array[Mathf.Clamp(efficiency / 50, (efficiency != 0) ? 1 : 0, array.Length - 1)];
			t.note.AddTopic("TopicLeft", lang.lang(), text + " (" + text2 + ")");
			if (!h.source.destTrait.IsEmpty())
			{
				bool flag = EClass._map.FindThing(Type.GetType("Trait" + h.source.destTrait + ", Elin"), a) != null;
				List<CardRow> obj = EClass.sources.cards.rows.Where((CardRow t) => t.trait.Length != 0 && Type.GetType("Trait" + h.source.destTrait).IsAssignableFrom(Type.GetType("Trait" + t.trait[0]))).ToList();
				obj.Sort((CardRow a, CardRow b) => a.LV - b.LV);
				CardRow cardRow = obj[0];
				t.note.AddText("NoteText_small", "・ " + "workDestTrait".lang(cardRow.GetName().ToTitleCase().TagColor(flag ? FontColor.Good : FontColor.Warning)));
			}
			if (efficiency == 0)
			{
				t.note.AddText("NoteText_small", "・ " + "workNotActive".lang());
			}
			else
			{
				for (int i = 0; i < h.source.things.Length; i += 2)
				{
					float num = Mathf.Max(1f, (long)h.source.things[i + 1].ToInt() * (long)efficiency * a.homeBranch.GetProductBonus(a) / 100 / 1000);
					string text3 = h.source.things[i];
					string s = (text3.StartsWith("#") ? EClass.sources.categories.map[text3.Replace("#", "")].GetName() : EClass.sources.cards.map[h.source.things[i]].GetName());
					if (h.source.alias == "Breeding")
					{
						num = a.race.breeder * 100 / 2500;
						num = num * (float)((!(text3 == "_egg")) ? 1 : 2) / 3f;
						if (a.race.breeder > 0 && num <= 0f)
						{
							num = 1f;
						}
					}
					t.note.AddText("NoteText_small", "・ " + "work_produce".lang(s.ToTitleCase(), num.ToString() ?? ""));
				}
				if (!h.source.elements.IsEmpty())
				{
					for (int j = 0; j < h.source.elements.Length; j += 2)
					{
						SourceElement.Row row = EClass.sources.elements.map[h.source.elements[j]];
						int num2 = h.source.elements[j + 1];
						float num3 = ((num2 < 0 || row.id == 2115 || row.id == 2207) ? ((float)(num2 / 10)) : Mathf.Max((h.GetEfficiency(a) > 0) ? 1 : 0, (long)num2 * (long)h.GetEfficiency(a) * a.homeBranch.efficiency / 100 / 1000));
						t.note.AddText("NoteText_small", "・ " + "workBonus_skill".lang(row.GetName().ToTitleCase(), ((num2 > 0) ? "+" : "") + num3) + ((row.id == 2115 || row.id == 2207) ? (" " + "fixedFactionSkill".lang()) : ""), (num2 >= 0) ? FontColor.Default : FontColor.Bad);
					}
				}
				string[] array2 = h.source.GetDetail().SplitNewline();
				foreach (string text4 in array2)
				{
					if (!text4.IsEmpty())
					{
						t.note.AddText("NoteText_small", "・ " + text4);
					}
				}
			}
			t.note.Space(1);
		}
	}

	public virtual void SetSubText(Chara a, ItemGeneral b)
	{
		switch (ShowMode)
		{
		case LayerPeople.ShowMode.Race:
			b.SetSubText(a.race.GetName().ToTitleCase(wholeText: true), 300);
			break;
		case LayerPeople.ShowMode.Job:
			b.SetSubText(a.job.GetName().ToTitleCase(wholeText: true), 300);
			break;
		case LayerPeople.ShowMode.Work:
		{
			string text = a.GetTextWork(simple: true) + "," + a.GetTextHobby(simple: true);
			b.SetSubText(text.TrimEnd(','), 300);
			break;
		}
		}
	}

	public override void OnClick(Chara c, ItemGeneral i)
	{
		if (!c.IsAliveInCurrentZone)
		{
			if (c.currentZone != EClass._zone)
			{
				Msg.Say("isIn", c, (c.currentZone == null) ? "???" : c.currentZone.Name);
			}
			SE.BeepSmall();
			return;
		}
		UIContextMenu uIContextMenu = EClass.ui.CreateContextMenuInteraction().SetHighlightTarget(i);
		if (c.IsGuest())
		{
			uIContextMenu.AddButton("findMember", delegate
			{
				EClass.pc.SetAI(new AI_Goto(c, 1));
				layer.Close();
			});
		}
		else if (c.IsHomeMember())
		{
			if (!c.IsPC)
			{
				uIContextMenu.AddButton("findMember", delegate
				{
					EClass.pc.SetAI(new AI_Goto(c, 1));
					layer.Close();
				});
			}
			uIContextMenu.AddButton("changeName", delegate
			{
				Dialog.InputName("dialogChangeName", c.c_altName.IsEmpty(c.NameSimple), delegate(bool cancel, string text)
				{
					if (!cancel)
					{
						if (text == "*r")
						{
							text = NameGen.getRandomName();
						}
						c.c_altName = text;
						layer.OnSwitchContent(layer.windows[0]);
					}
				});
			});
			if (c != EClass.pc)
			{
				if (c.sourceCard.idActor.IsEmpty() && c.host == null)
				{
					if (c.pccData == null)
					{
						uIContextMenu.AddButton("editPCC", delegate
						{
							EClass.ui.AddLayer<LayerEditPortrait>().Activate(c);
						});
						if (!c.IsPC)
						{
							uIContextMenu.AddButton("editSkin", delegate
							{
								LayerEditSkin layerEditSkin = EClass.ui.AddLayer<LayerEditSkin>();
								layerEditSkin.SetOnKill(delegate
								{
									list.Refresh();
								});
								layerEditSkin.Activate(c);
							});
						}
						uIContextMenu.AddButton("togglePCC", delegate
						{
							bool isSynced2 = c.isSynced;
							c.pccData = PCCData.Create(c.IDPCCBodySet);
							c.pccData.Randomize(c.IDPCCBodySet);
							if (isSynced2)
							{
								c.renderer.OnLeaveScreen();
							}
							EClass.scene.syncList.Remove(c.renderer);
							c._CreateRenderer();
							if (isSynced2)
							{
								EClass.scene.syncList.Add(c.renderer);
								c.renderer.OnEnterScreen();
							}
							list.Refresh();
							SE.Click();
							WidgetRoster.SetDirty();
						});
					}
					else
					{
						uIContextMenu.AddButton("editPCC", delegate
						{
							EClass.ui.AddLayer<LayerEditPCC>().Activate(c, UIPCC.Mode.Full);
						});
						uIContextMenu.AddButton("togglePCC", delegate
						{
							bool isSynced = c.isSynced;
							if (isSynced)
							{
								c.renderer.OnLeaveScreen();
							}
							EClass.scene.syncList.Remove(c.renderer);
							c.pccData = null;
							c._CreateRenderer();
							if (isSynced)
							{
								EClass.scene.syncList.Add(c.renderer);
								c.renderer.OnEnterScreen();
							}
							list.Refresh();
							WidgetRoster.SetDirty();
						});
					}
				}
				if (c.homeBranch == EClass.Branch)
				{
					uIContextMenu.AddButton("makeMaid", delegate
					{
						if (EClass.Branch.uidMaid == c.uid)
						{
							EClass.Branch.uidMaid = 0;
						}
						else
						{
							EClass.Branch.uidMaid = c.uid;
						}
						list.Refresh();
						SE.Click();
					});
				}
				int @int = c.GetInt(36);
				bool isLivestockTimerOn = memberType == FactionMemberType.Default && !EClass.world.date.IsExpired(@int);
				int remainingHours = EClass.world.date.GetRemainingHours(@int);
				if (!c.IsPCParty)
				{
					uIContextMenu.AddButton((c.memberType == FactionMemberType.Livestock) ? "daMakeResident" : (isLivestockTimerOn ? "daMakeLivestock2".lang(Date.GetText(remainingHours)) : "daMakeLivestock"), delegate
					{
						if (isLivestockTimerOn)
						{
							SE.Beep();
						}
						else
						{
							if (c.memberType == FactionMemberType.Livestock)
							{
								c.SetInt(36, EClass.world.date.GetRaw() + 14400);
							}
							EClass.Branch.ChangeMemberType(c, (c.memberType != FactionMemberType.Livestock) ? FactionMemberType.Livestock : FactionMemberType.Default);
							foreach (ListOwner owner in layer.multi.owners)
							{
								owner.RefreshTab();
							}
							List();
							SE.Click();
						}
					});
				}
				uIContextMenu.AddButton("addToReserve".lang() + " (" + EClass.Home.listReserve.Count + "/" + EClass.Home.GetMaxReserve() + ")", delegate
				{
					if (EClass.Home.listReserve.Count >= EClass.Home.GetMaxReserve())
					{
						SE.Beep();
						Msg.Say("reserveLimit");
						return;
					}
					SE.MoveZone();
					EClass.Home.AddReserve(c);
					list.List();
					foreach (ListOwner owner2 in layer.multi.owners)
					{
						owner2.RefreshTab();
					}
				});
			}
		}
		uIContextMenu.Show();
	}

	public override void OnList()
	{
		if (memberType == FactionMemberType.Guest)
		{
			foreach (Chara chara in EClass._map.charas)
			{
				if (chara.IsGuest())
				{
					list.Add(chara);
				}
			}
			return;
		}
		foreach (Chara member in EClass.Branch.members)
		{
			if (member.memberType == memberType && !member.isSummon)
			{
				list.Add(member);
			}
		}
		foreach (Chara member2 in EClass.pc.party.members)
		{
			if (member2.memberType == memberType && !list.items.Contains(member2))
			{
				list.Add(member2);
			}
		}
	}

	public override void OnRefreshMenu()
	{
		WindowMenu menuLeft = window.menuLeft;
		menuLeft.Clear();
		if (!main)
		{
			return;
		}
		menuLeft.AddButton2Line("sort", () => list.sortMode.ToString(), delegate
		{
			list.NextSort();
		});
		if (ShowShowMode)
		{
			Lang.GetList("info_people");
			menuLeft.AddButton2Line("info", () => "show" + layer.showMode, delegate
			{
				layer.showMode = layer.showMode.NextEnum();
				List();
			});
		}
	}
}
