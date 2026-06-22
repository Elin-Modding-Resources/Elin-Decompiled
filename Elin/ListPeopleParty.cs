using System.Linq;

public class ListPeopleParty : BaseListPeople
{
	public bool CanJoinParty(Chara c)
	{
		if (!c.IsPC)
		{
			if (!c.isDead && !c.HasCondition<ConSuspend>() && c.currentZone != null && c.trait.CanJoinParty)
			{
				return c.trait.CanJoinPartyResident;
			}
			return false;
		}
		return true;
	}

	public override void OnRefreshMenu()
	{
		base.OnRefreshMenu();
		if (!main)
		{
			return;
		}
		WindowMenu menuLeft = window.menuLeft;
		menuLeft.AddSpace();
		menuLeft.AddHeader("party");
		menuLeft.AddSpace(13f);
		if (EClass.pc.party.members.Count > 1)
		{
			menuLeft.AddButton("disband", delegate
			{
				EClass.pc.party.Disband();
				SE.Trash();
				RefreshAll();
			});
		}
		for (int j = 0; j < 5; j++)
		{
			int i = j;
			Player.PartySetup setup = EClass.player.partySetups[i];
			if (setup == null)
			{
				menuLeft.AddButton("party_empty".lang((i + 1).ToString() ?? ""), delegate
				{
					if (EClass.pc.party.members.Count == 1)
					{
						SE.Beep();
					}
					else
					{
						SE.Equip();
						EClass.pc.party.RegisterSetup(i);
						RefreshAll();
					}
				});
				continue;
			}
			UIButton uIButton = menuLeft._AddButton("Party", null);
			uIButton.SetOnClick(delegate
			{
				EClass.pc.party.Disband();
				foreach (int uid in setup.uids)
				{
					Chara chara3 = EClass.game.cards.globalCharas.Find(uid);
					if (chara3 != null && !chara3.IsPC && CanJoinParty(chara3) && chara3.IsPCFaction)
					{
						JoinParty(chara3);
						if (chara3.uid == setup.ride)
						{
							ActRide.Ride(EClass.pc, chara3, parasite: false, talk: false);
						}
						else if (chara3.uid == setup.parasite)
						{
							ActRide.Ride(EClass.pc, chara3, parasite: true, talk: false);
						}
					}
				}
				SE.Equip();
				RefreshAll();
			});
			bool flag = true;
			foreach (int uid2 in setup.uids)
			{
				Chara chara = EClass.game.cards.globalCharas.Find(uid2);
				if (chara == null || !CanJoinParty(chara))
				{
					flag = false;
				}
			}
			uIButton.mainText.SetText("party_load".lang((i + 1).ToString() ?? ""), flag ? FontColor.Good : FontColor.Warning);
			uIButton.SetTooltip(delegate(UITooltip n)
			{
				n.note.Clear();
				n.note.AddHeader("faction_Member");
				foreach (int uid3 in setup.uids)
				{
					Chara chara2 = EClass.game.cards.globalCharas.Find(uid3);
					string text = ((chara2 == null) ? "???" : chara2.NameBraced);
					if (chara2 != null)
					{
						if (chara2.uid == setup.ride)
						{
							text = text + " " + "party_ride".lang();
						}
						else if (chara2.uid == setup.parasite)
						{
							text = text + " " + "party_parasite".lang();
						}
					}
					n.note.AddText(text, (chara2 == null || chara2.isDead || !chara2.IsPCFaction) ? FontColor.Bad : ((!CanJoinParty(chara2)) ? FontColor.Warning : FontColor.Good));
				}
				n.note.Build();
			});
			uIButton.GetComponentInDirectChildren<UIButton>().SetOnClick(delegate
			{
				SE.Trash();
				EClass.player.partySetups[i] = null;
				RefreshAll();
			});
		}
	}

	public override void OnInstantiate(Chara c, ItemGeneral i)
	{
		Zone zone = (main ? c.currentZone : c.homeZone);
		if (main)
		{
			i.SetSubText((zone == null) ? "???" : zone.Name, 240);
		}
		UIButton uIButton = i.AddSubButton(EClass.core.refs.icons.fav, delegate
		{
			SE.ClickGeneral();
			c.isFav = !c.isFav;
			RefreshAll();
		}, null, null, "fav");
		uIButton.icon.SetAlpha(c.isFav ? 1f : 0.3f);
		uIButton.icon.SetNativeSize();
	}

	public override void OnClick(Chara c, ItemGeneral i)
	{
		if (main)
		{
			if (!CanJoinParty(c))
			{
				SE.Beep();
				if (!c.trait.CanJoinPartyResident)
				{
					Msg.Say("ride_req");
				}
				return;
			}
			JoinParty(c);
		}
		else
		{
			if (c.IsPC)
			{
				SE.Beep();
				return;
			}
			EClass.pc.party.RemoveMember(c);
			if (c.homeZone != EClass._zone)
			{
				c.MoveZone(c.homeZone);
			}
		}
		MoveToOther(c);
		base.Main.OnRefreshMenu();
	}

	public void JoinParty(Chara c)
	{
		if (c.currentZone != EClass._zone)
		{
			c.MoveZone(EClass._zone);
			c.MoveImmediate(EClass.pc.pos.GetNearestPoint(allowBlock: false, allowChara: false) ?? EClass.pc.pos);
		}
		EClass.pc.party.AddMemeber(c);
	}

	public override void OnList()
	{
		if (main)
		{
			foreach (Chara value in EClass.game.cards.globalCharas.Values)
			{
				if (value.IsPCFaction && !value.IsPCParty && value.memberType == FactionMemberType.Default)
				{
					list.Add(value);
				}
			}
			return;
		}
		foreach (Chara member in EClass.pc.party.members)
		{
			list.Add(member);
		}
		list.sortMode = UIList.SortMode.ByPartyOrder;
	}

	public HireInfo GetInfo(Chara c)
	{
		return EClass.Home.listReserve.First((HireInfo a) => a.chara == c);
	}
}
