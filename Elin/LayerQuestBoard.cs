using UnityEngine;

public class LayerQuestBoard : ELayer
{
	public UIList list;

	public UIList listHire;

	public UIDynamicList listRanking;

	public UIText textClient;

	public UIText textTitle;

	public UIText textDetail;

	public UIText textHours;

	public UIText textNote;

	public UIText textReward;

	public UIText textQuestLimit;

	public Portrait portrait;

	public UIButton buttonTalk;

	public UICurrency uiCurrency;

	public Gradient gradientRank;

	public override void OnInit()
	{
		if (ELayer._zone.IsPCFaction)
		{
			ELayer._zone.branch.UpdateReqruits();
		}
		windows[0].setting.tabs[1].disable = !ELayer._zone.IsPCFaction;
	}

	public override void OnSwitchContent(Window window)
	{
		switch (window.idTab)
		{
		case 0:
			RefreshQuest();
			break;
		case 1:
			if (ELayer.debug.enable)
			{
				ELayer.Branch.UpdateReqruits(clear: true);
			}
			RefreshHire();
			break;
		case 2:
			RefreshRanking();
			break;
		}
		WindowMenu menuRight = windows[0].menuRight;
		menuRight.Clear();
		if (window.idTab != 0)
		{
			return;
		}
		UIButton button = null;
		button = menuRight.AddButton("rerollQuest".lang(GetCost().ToString() ?? ""), delegate
		{
			if (ELayer._zone.influence < GetCost())
			{
				SE.Beep();
				Msg.Say("notEnoughInfluence");
			}
			else
			{
				SE.Dice();
				ELayer._zone.UpdateQuests(force: true);
				RefreshQuest();
				ELayer._zone.influence -= GetCost();
				if (ELayer.player.questRerollCost < 14)
				{
					ELayer.player.questRerollCost++;
				}
				button.mainText.SetText("rerollQuest".lang(GetCost().ToString() ?? ""));
			}
		});
		static int GetCost()
		{
			return 1;
		}
	}

	public void RefreshQuest()
	{
		list.Clear();
		list.callbacks = new UIList.Callback<Quest, ItemQuest>
		{
			onInstantiate = delegate(Quest a, ItemQuest b)
			{
				b.SetQuest(a);
				b.button.SetOnClick(delegate
				{
					Close();
					a.OnClickQuest();
				});
			}
		};
		ELayer._zone.UpdateQuests();
		foreach (Quest global in ELayer.game.quests.globalList)
		{
			if (global.IsVisibleOnQuestBoard())
			{
				list.Add(global);
			}
		}
		foreach (Chara chara in ELayer._map.charas)
		{
			if (chara.quest != null && !ELayer.game.quests.list.Contains(chara.quest) && !chara.IsPCParty && chara.memberType == FactionMemberType.Default && chara.quest.IsVisibleOnQuestBoard())
			{
				list.Add(chara.quest);
			}
		}
		list.Refresh();
		textQuestLimit.SetText("questLimit".lang(ELayer.game.quests.CountRandomQuest().ToString() ?? "", 5.ToString() ?? ""));
	}

	public void RefreshHire()
	{
		UIList list = listHire;
		list.Clear();
		list.callbacks = new UIList.Callback<HireInfo, ButtonChara>
		{
			onClick = delegate(HireInfo a, ButtonChara b)
			{
				a.isNew = false;
				GameLang.refDrama1 = "(" + "money2".lang() + ": " + CalcGold.Hire(a.chara) + ")";
				GameLang.refDrama2 = ELayer.pc.things.Find("ticket_resident")?.Name ?? "";
				a.chara.ShowDialog("_chara", "4-1").onKill.AddListener(RefreshHire);
			},
			onInstantiate = delegate(HireInfo a, ButtonChara b)
			{
				Chara chara = a.chara;
				b.SetChara(chara, ButtonChara.Mode.Hire);
				int cost = CalcGold.Hire(chara);
				b.item.text1.text = cost.ToString().TagColorGoodBad(() => ELayer.pc.GetCurrency("money2") >= cost || ELayer.pc.things.Find("ticket_resident") != null);
				b.item.text2.text = ((a.Days == -1) ? "-" : (a.Days.ToString() ?? ""));
				HintIcon[] componentsInChildren = b.layoutTag.GetComponentsInChildren<HintIcon>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].onPointerDown = delegate
					{
						list.callbacks.OnClick(a, b);
					};
				}
				b.textHobby.text = chara.GetTextHobby();
				b.textWork.text = chara.GetTextWork();
				b.textLifeStyle.text = "lifestyle".lang() + ": " + ("lifestyle_" + chara.idTimeTable).lang();
				b.item.image1.SetActive(a.isNew);
			},
			onRefresh = null
		};
		foreach (HireInfo item in ELayer.Branch.listRecruit)
		{
			if (!item.chara.IsHomeMember() && (item.chara.currentZone == null || !item.chara.currentZone.IsPlayerFaction))
			{
				list.Add(item);
			}
		}
		list.Refresh();
		this.RebuildLayout(recursive: true);
	}

	public void RefreshRanking()
	{
		UIDynamicList list = listRanking;
		list.Clear();
		list.callbacks = new UIList.Callback<Chara, ButtonChara>
		{
			onClick = delegate
			{
			},
			onRedraw = delegate(Chara a, ButtonChara b, int i)
			{
				b.SetChara(a, ButtonChara.Mode.Ranking);
				int advRank = a.trait.GetAdvRank();
				b.textAlias.SetText(a.trait.GetAdvRankText());
				b.textAlias.color = gradientRank.Evaluate(0.1f * (float)advRank);
				b.item.image2.SetActive(a.IsPCFaction);
				bool enable = a.GetInt(111) > 0;
				b.transDefeated.SetActive(enable);
			},
			onList = delegate
			{
				foreach (Chara value in ELayer.game.cards.globalCharas.Values)
				{
					if (value.trait.ShowAdvRank)
					{
						list.Add(value);
					}
				}
				list.objects.Sort((object a, object b) => GetSortVal(b as Chara) - GetSortVal(a as Chara));
			},
			onRefresh = null
		};
		list.List();
		int num = list.objects.IndexOf(ELayer.pc);
		if (num != -1)
		{
			list.dsv.scrollByItemIndex(num);
			list.Refresh();
		}
		this.RebuildLayout(recursive: true);
		static int GetSortVal(Chara c)
		{
			return c.trait.GetAdvRank() * 10000 + Mathf.Clamp(c.LV, 0, 1000) + ((c.trait is TraitAdventurerBacker) ? 1000 : 0);
		}
	}

	public override void OnKill()
	{
		if (ELayer.Branch != null)
		{
			ELayer.Branch.ClearNewRecruits();
		}
	}
}
