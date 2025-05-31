using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class DramaCustomSequence : EClass
{
	public Card destCard;

	public Chara destChara;

	public Quest destQuest;

	public Thing destThing;

	public string idDefault;

	public string idCustom;

	public DramaSetup setup;

	public DramaSequence sequence;

	public List<DramaEvent> events;

	public DramaManager manager;

	public string StepDefault => idDefault;

	public string StepEnd => "end";

	public void Build(Chara c)
	{
		bool flag = idCustom == "Unique";
		bool flag2 = c.bio.IsUnderAge || EClass.pc.bio.IsUnderAge;
		bool isInGuild = Guild.Fighter.IsCurrentZone || Guild.Mage.IsCurrentZone || Guild.Thief.IsCurrentZone || Guild.Merchant.IsCurrentZone;
		string bird = (flag2 ? "bird" : "tail");
		_ = c.Name;
		string rumor = (c.IsPCParty ? GetTalk("sup") : GetRumor(c));
		Layer layer = null;
		bool flag3 = c.IsHumanSpeak || EClass.pc.HasElement(1640);
		bool num = !c.IsUnique || (c.IsPCFaction && c.trait is TraitOlderYoungerSister);
		if (!flag)
		{
			Step("Resident");
			_Talk("tg", () => rumor);
		}
		if (num && flag3)
		{
			DramaChoice choice = Choice2("letsTalk", StepDefault);
			choice.SetOnClick(delegate
			{
				sequence.firstTalk.funcText = () => rumor;
				List<Hobby> list2 = c.ListHobbies();
				Hobby hobby = ((list2.Count > 0) ? list2[0] : null);
				if (EClass.rnd(20) == 0 || EClass.debug.showFav)
				{
					if (EClass.rnd(2) == 0 || hobby == null)
					{
						GameLang.refDrama1 = c.GetFavCat().GetName().ToLower();
						GameLang.refDrama2 = c.GetFavFood().GetName();
						rumor = GetText(c, "general", "talk_fav");
						c.knowFav = true;
					}
					else
					{
						GameLang.refDrama1 = hobby.Name.ToLower();
						rumor = GetText(c, "general", "talk_hobby");
					}
				}
				else
				{
					rumor = GetRumor(c);
				}
				c.affinity.OnTalkRumor();
				choice.forceHighlight = true;
			}).SetCondition(() => c.interest > 0);
		}
		bool flag4 = false;
		if (!c.IsPCFaction && c.affinity.CanInvite() && !EClass._zone.IsInstance && c.c_bossType == BossType.none)
		{
			if ((c.trait.IsUnique || c.IsGlobal) && c.GetInt(111) == 0 && !c.IsPCFaction)
			{
				Choice2("daBout", "_bout");
				flag4 = true;
			}
			else
			{
				Choice2("daInvite", "_invite");
			}
		}
		foreach (Quest item in EClass.game.quests.list)
		{
			Quest _quest = item;
			if (!item.CanDeliverToClient(c))
			{
				continue;
			}
			QuestDeliver questDeliver = _quest as QuestDeliver;
			foreach (Thing item2 in questDeliver.ListDestThing())
			{
				Thing _t4 = item2;
				Choice2("daDeliver".lang(item.GetTitle() ?? "", _t4.GetName(NameStyle.Full, questDeliver.num)), "_deliver").SetOnClick(delegate
				{
					destThing = _t4;
					destQuest = _quest;
				}).SetOnTooltip(delegate(UITooltip a)
				{
					_t4.WriteNote(a.note);
				});
			}
		}
		if (c.IsPCParty && !c.isSummon)
		{
			if (EClass._zone.IsPCFaction && c.homeBranch != EClass._zone.branch)
			{
				Choice2("daMakeHome", "_makeHome");
			}
			if (c.host == null && c.homeZone != null)
			{
				Choice2("daLeaveParty".lang(c.homeZone.Name), "_leaveParty");
			}
		}
		if (c.memberType != FactionMemberType.Livestock && !c.IsGuest())
		{
			Choice2("daQuest".lang(c.quest?.GetTitle() ?? ""), "_quest").SetCondition(() => c.quest != null);
			if (c.trait is TraitGuard)
			{
				EClass.pc.things.Foreach(delegate(Thing _t)
				{
					if (_t.isLostProperty)
					{
						Choice2("daLostProperty".lang(_t.Name), "_lostProperty").SetOnClick(delegate
						{
							destThing = _t;
						});
					}
				});
			}
			if (!c.IsPCParty || EClass._zone.IsPCFaction)
			{
				if (c.trait.CanGuide)
				{
					foreach (Quest item3 in EClass.game.quests.list)
					{
						if (!item3.IsRandomQuest)
						{
							continue;
						}
						Chara dest = ((item3.chara != null && item3.chara.IsAliveInCurrentZone) ? item3.chara : null);
						if (dest != null)
						{
							Choice2("daGoto".lang(dest.Name, item3.GetTitle() ?? ""), "_goto").SetOnClick(delegate
							{
								destCard = dest;
							});
						}
						if (!(item3 is QuestDeliver { IsDeliver: not false } questDeliver2) || questDeliver2.DestZone != EClass._zone || !EClass._zone.dictCitizen.ContainsKey(questDeliver2.uidTarget))
						{
							continue;
						}
						Chara dest2 = EClass._zone.FindChara(questDeliver2.uidTarget);
						if (dest2 != null)
						{
							Choice2("daGoto".lang(dest2.Name, item3.GetTitle() ?? ""), "_goto").SetOnClick(delegate
							{
								destCard = dest2;
							});
						}
					}
					if (GetListGuide().Count > 0)
					{
						Choice2("daGuide", "_Guide");
					}
				}
				if (c.trait is TraitGM_Mage && Guild.Mage.relation.rank >= 4)
				{
					Choice2("daChangeDomain", "_changeDomain").DisableSound();
				}
				if (c.trait.ShopType != 0)
				{
					Choice2(c.trait.TextNextRestock, "_buy").DisableSound();
				}
				if (!EClass._zone.IsUserZone)
				{
					if (c.trait.SlaverType != 0)
					{
						Choice2(c.trait.TextNextRestockPet, "_buySlave").DisableSound();
					}
					if (c.trait.CopyShop != 0)
					{
						Choice2(("daCopy" + c.trait.CopyShop).lang(c.trait.NumCopyItem.ToString() ?? ""), "_copyItem").DisableSound();
					}
				}
				if (c.trait.HaveNews && c.GetInt(33) + 10080 < EClass.world.date.GetRaw())
				{
					Choice2("daNews", "_news");
				}
				if (!flag4 && !EClass._zone.IsInstance && !c.IsPCFaction && c.trait.CanBout && c.IsGlobal && c.GetInt(59) + 10080 < EClass.world.date.GetRaw())
				{
					Choice2("daBout", "_bout");
				}
				if (c.isDrunk || EClass.debug.enable)
				{
					Choice2(flag2 ? "daBird" : "daTail", "_tail");
				}
				if (c.trait.CanRevive)
				{
					Choice2("daRevive", "_revive").DisableSound();
				}
				if (!c.trait.IDTrainer.IsEmpty() && !EClass._zone.IsUserZone && (Guild.GetCurrentGuild() == null || Guild.GetCurrentGuild().relation.IsMember()))
				{
					Choice2("daTrain", "_train").DisableSound();
				}
				if (c.trait.CanWhore)
				{
					Choice2(flag2 ? "daBirdBuy" : "daTailBuy", "_whore");
				}
				if (c.trait.CanHeal)
				{
					Choice2("daHeal", "_heal");
				}
				if (c.trait.CanServeFood)
				{
					Choice2("daFood", "_food");
				}
				if (c.trait is TraitInformer)
				{
					Choice2("daSellFame", "_sellFame");
				}
				if (EClass._zone.AllowInvest)
				{
					if (c.trait.CanInvestTown && Guild.GetCurrentGuild() == null)
					{
						Choice2("daInvest", "_investZone");
					}
					if (c.trait.CanInvest)
					{
						Choice2("daInvest", "_investShop");
					}
				}
				if (c.trait.CanIdentify)
				{
					Choice2("daIdentify", "_identify").DisableSound();
					Choice2("daIdentifyAll", "_identifyAll");
					Choice2("daIdentifySP", "_identifySP").DisableSound();
				}
				if (c.trait.CanPicklock)
				{
					if (c.Evalue(280) < 20)
					{
						c.elements.SetBase(280, 20);
					}
					foreach (Thing item4 in EClass.pc.things.List((Thing a) => a.c_lockLv > 0, onlyAccessible: true))
					{
						Thing _t3 = item4;
						Choice2("daPicklock".lang(_t3.Name), "_picklock").SetOnClick(delegate
						{
							destThing = _t3;
						});
					}
				}
				if (c.trait is TraitBanker)
				{
					Choice2("daDeposit", "_deposit");
				}
				if (c.IsMaid || (c.trait.CanInvestTown && (EClass._zone.source.faction == "mysilia" || EClass._zone.IsPCFaction)))
				{
					Choice2("daExtraTax", "_extraTax");
				}
				if ((c.trait is TraitMiko_Mifu || c.trait is TraitMiko_Nefu || c.trait is TraitEureka) && EClass.world.date.IsExpired(c.c_dateStockExpire))
				{
					Choice2("daBlessing", "_blessing");
				}
			}
		}
		if (c.IsHomeMember())
		{
			if (c.IsMaid)
			{
				if (EClass.Branch.meetings.CanStartMeeting)
				{
					Choice2("daMeeting".lang(EClass.Branch.meetings.list.Count.ToString() ?? ""), "_meeting");
				}
				Choice2("daBuyLand", "_buyLand");
				Choice2("daChangeTitle", "_changeTitle");
			}
			if (c.noMove)
			{
				Choice2("enableMove", "_enableMove");
			}
			if (!c.IsPCParty && c.memberType != FactionMemberType.Livestock && c.trait.CanJoinParty)
			{
				Choice2("daJoinParty", "_joinParty");
			}
			Choice2("daFactionOther", "_factionOther");
		}
		if (c.trait is TraitLoytel && EClass.game.quests.Get<QuestDebt>() != null)
		{
			Choice2("daGreatDebt", "_greatDebt");
		}
		if (!flag)
		{
			Choice2("bye", "_bye");
			EnableCancel();
		}
		Step("_factionOther");
		Talk("what", StepDefault);
		if (c.trait is TraitLoytel)
		{
			QuestDebt questDebt = EClass.game.quests.Get<QuestDebt>();
			if (questDebt != null && questDebt.gaveBill)
			{
				Choice("daGreatDebt2", "_greatDebt2");
			}
		}
		if (c.IsPCParty)
		{
			if (!c.isSummon)
			{
				Choice((c.GetInt(106) == 0) ? "daShutup" : "daShutup2", "_shutup");
				if (c.CanInsult() || c.GetInt(108) == 1)
				{
					Choice((c.GetInt(108) == 0) ? "daInsult" : "daInsult2", "_insult");
				}
			}
		}
		else if (!c.noMove)
		{
			Choice("disableMove", "_disableMove");
		}
		Choice((c.GetInt(123) == 0) ? "daSleepBeside" : "daSleepBeside2", "_sleepBeside");
		if (c.GetInt(113) == 0)
		{
			Choice("daEquipSharedOff", "_toggleSharedEquip");
		}
		else
		{
			Choice("daEquipSharedOn", "_toggleSharedEquip");
		}
		if (!c.IsMaid && c.homeBranch == EClass.Branch)
		{
			Choice("daMakeMaid", "_daMakeMaid");
		}
		if (c.trait.CanBeBanished && !c.IsPCParty)
		{
			Choice("daBanish", "_depart");
		}
		Choice("daNothing", StepDefault, cancel: true);
		Step("_toggleSharedEquip");
		Method(delegate
		{
			c.SetInt(113, (c.GetInt(113) == 0) ? 1 : 0);
		});
		_Talk("tg", GetTopic(c, (c.GetInt(113) == 0) ? "shutup" : "shutup2"));
		End();
		Step("_daMakeMaid");
		Method(delegate
		{
			EClass.Branch.uidMaid = c.uid;
		});
		_Talk("tg", GetTopic(c, "becomeMaid"));
		End();
		Step("_joinParty");
		Method(delegate
		{
			if (!c.trait.CanJoinPartyResident)
			{
				GameLang.refDrama1 = c.GetBestAttribute().ToString() ?? "";
				TempTalkTopic("invite3", StepDefault);
			}
			else
			{
				EClass.pc.party.AddMemeber(c, showMsg: true);
			}
		});
		Talk("hired", StepEnd);
		Step("_leaveParty");
		Method(delegate
		{
			EClass.pc.party.RemoveMember(c);
			if (EClass.game.activeZone != c.homeZone)
			{
				EClass.pc.Say("tame_send", c, c.homeZone.Name);
				c.MoveZone(c.homeZone);
			}
		});
		Goto("_bye");
		Step("_banish");
		Goto("_bye");
		Step("_makeLivestock");
		Method(delegate
		{
			c.memberType = FactionMemberType.Livestock;
		});
		Talk("becomeLivestock", StepEnd);
		Step("_makeResident");
		Method(delegate
		{
			c.memberType = FactionMemberType.Default;
		});
		Talk("becomeResident", StepEnd);
		Step("_depart");
		Talk("depart_choice", StepDefault);
		Choice("depart1", "_depart1");
		Choice("depart2", "_depart2");
		Step("_depart1");
		Method(delegate
		{
			LayerDrama.Instance.SetOnKill(delegate
			{
				c.homeBranch.BanishMember(c);
			});
		});
		Talk("depart1", StepEnd);
		Step("_depart2");
		Talk("depart2", StepDefault);
		Step("_gift");
		Talk("gift_good", null);
		End();
		Step("_goto");
		Method(delegate
		{
			GameLang.refDrama1 = destCard.Name;
			if (destCard == c)
			{
				TempTalkTopic("goto2", StepDefault);
			}
			else
			{
				TempTalkTopic("goto", "_goto2");
			}
		});
		Step("_goto2");
		Method(delegate
		{
			if (destCard.isChara && !PathManager.Instance.RequestPathImmediate(EClass.pc.pos, destCard.pos, EClass.pc).HasPath)
			{
				destCard.Teleport(EClass.pc.pos.GetNearestPoint(allowBlock: false, allowChara: false, allowInstalled: true, ignoreCenter: true) ?? EClass.pc.pos, silent: true, force: true);
			}
			EClass.pc.SetAIImmediate(new AI_Goto(destCard, 1));
			EInput.Consume(consumeAxis: false, 20);
		});
		End();
		Step("_rumor");
		Talk("rumor", StepDefault);
		Step("_lostProperty");
		Method(delegate
		{
			GameLang.refDrama1 = destThing.Name;
			destThing.Destroy();
			EClass.player.ModKarma(5);
		});
		_Talk("tg", GetTopic(c, "deliver_purse"), StepEnd);
		_ = c.quest;
		string text = ((!setup.forceJump.IsEmpty()) ? StepEnd : StepDefault);
		Step("_deliver");
		Method(delegate
		{
			GameLang.refDrama1 = destQuest.NameDeliver;
			destQuest.Deliver(c, destThing);
		});
		_Talk("tg", () => (destQuest != null) ? destQuest.GetTalkComplete().IsEmpty(GetTopic(c, (destQuest.bonusMoney > 0) ? "questCompleteDeliverExtra" : "questCompleteDeliver")) : "", StepEnd);
		Step("_quest");
		_Talk("tg", delegate
		{
			if (c.quest == null)
			{
				return "";
			}
			GameLang.refDrama1 = c.quest.RefDrama1;
			GameLang.refDrama2 = c.quest.RefDrama2;
			GameLang.refDrama3 = c.quest.RefDrama3;
			return (!taken()) ? c.quest.GetDetail() : c.quest.GetTalkProgress().IsEmpty(GetTopic(c, "questInProgress"));
		}, text);
		string text2 = "daAccept".lang();
		if (c.quest != null && c.quest.deadline != 0)
		{
			text2 += "hintDeadline".lang(c.quest.TextDeadline).ToLower();
		}
		Choice(text2, (c.quest != null && c.quest.UseInstanceZone) ? "_questAccept_instance" : "_questAccept").SetOnClick(delegate
		{
			EClass.game.quests.Start(c.quest);
		}).SetCondition(() => !taken() && EClass.game.quests.CountRandomQuest() < 5);
		Choice(text2, "_questFull").SetOnClick(delegate
		{
		}).SetCondition(() => !taken() && EClass.game.quests.CountRandomQuest() >= 5);
		Choice("daDecline", text).SetOnClick(RumorChill).SetCondition(() => !taken());
		if (c.quest != null && EClass.game.quests.Get(c.quest.uid) == null)
		{
			QuestSupply supply = c.quest as QuestSupply;
			if (supply != null)
			{
				foreach (Thing item5 in supply.ListDestThing())
				{
					Thing _t2 = item5;
					Choice("daDeliver".lang(supply.GetTitle() ?? "", _t2.GetName(NameStyle.Full, supply.num)), "_deliver").SetOnClick(delegate
					{
						EClass.game.quests.Start(c.quest);
						destThing = _t2;
						destQuest = supply;
					}).SetOnTooltip(delegate(UITooltip a)
					{
						_t2.WriteNote(a.note);
					});
				}
			}
		}
		EnableCancel(text);
		Step("_questAccept");
		_Talk("tg", GetTopic(c, "questAccept"), StepEnd);
		Step("_questAccept_instance");
		_Talk("tg", GetTopic(c, "questAccept"));
		Method(delegate
		{
			Zone z2 = c.quest.CreateInstanceZone(c);
			EClass.pc.MoveZone(z2, ZoneTransition.EnterState.Center);
		}, null, StepEnd);
		Step("_questFull");
		_Talk("tg", GetTopic(c, "questFull"), text);
		Step("_greatDebt");
		Method(delegate
		{
			QuestDebt questDebt3 = EClass.game.quests.Get<QuestDebt>();
			if (!questDebt3.CanGiveBill())
			{
				TempTalkTopic("loytel_bill_give_wait", StepDefault);
			}
			else if (questDebt3.gaveBill)
			{
				TempTalkTopic("loytel_bill_give_given", StepDefault);
			}
			else
			{
				TempTalkTopic(questDebt3.GetIdTalk_GiveBill(), StepEnd);
				questDebt3.GiveBill();
			}
		});
		Step("_greatDebt2");
		Method(delegate
		{
			QuestDebt questDebt2 = EClass.game.quests.Get<QuestDebt>();
			TempTalkTopic("loytel_bill_give_lost", StepEnd);
			questDebt2.GiveBill();
		});
		Step("_shutup");
		Method(delegate
		{
			c.SetInt(106, (c.GetInt(106) == 0) ? 1 : 0);
		});
		_Talk("tg", GetTopic(c, (c.GetInt(106) == 0) ? "shutup" : "shutup2"));
		End();
		Step("_sleepBeside");
		Method(delegate
		{
			if (c.affinity.CanSleepBeside() || c.GetInt(123) == 1)
			{
				c.SetInt(123, (c.GetInt(123) == 0) ? 1 : 0);
			}
			else
			{
				TempTalkTopic("refuse", StepEnd);
			}
		});
		_Talk("tg", GetTopic(c, (c.GetInt(123) == 0) ? "ok" : "shutup"));
		End();
		Step("_insult");
		Method(delegate
		{
			c.SetInt(108, (c.GetInt(108) == 0) ? 1 : 0);
		});
		_Talk("tg", GetTopic(c, (c.GetInt(108) == 0) ? "insult" : "insult2"));
		Method(delegate
		{
			if (c.GetInt(108) == 1)
			{
				c.Talk("insult");
			}
		});
		End();
		Step("_makeHome");
		Method(delegate
		{
			EClass._zone.branch.AddMemeber(c);
		});
		_Talk("tg", GetTopic(c, "ok"));
		End();
		Step("_hire");
		Talk("rumor", StepDefault);
		Choice("daAccept", StepDefault).SetOnClick(delegate
		{
		});
		Choice("daDecline", StepDefault).SetOnClick(delegate
		{
		});
		Step("_invite");
		Method(delegate
		{
			if (!c.trait.CanInvite)
			{
				TempTalkTopic("invite2", StepDefault);
			}
			else if (c.GetBestAttribute() > EClass.pc.CHA && !EClass.debug.godMode)
			{
				GameLang.refDrama1 = c.GetBestAttribute().ToString() ?? "";
				TempTalkTopic("invite3", StepDefault);
			}
			else
			{
				TempTalkTopic("invite", null);
				Choice("yes", delegate
				{
					TempTalk("hired", StepEnd);
					EClass.Sound.Play("good");
					c.MakeAlly();
				});
				Choice("no", StepDefault, cancel: true).SetOnClick(RumorChill);
			}
		});
		Step("_Guide");
		Method(delegate
		{
			TempTalkTopic("guide", null);
			foreach (Card guide in GetListGuide())
			{
				Choice("daGotoGuide".lang(guide.Name, ""), "_goto").SetOnClick(delegate
				{
					destCard = guide;
				});
			}
			Choice("no", StepDefault, cancel: true).SetOnClick(RumorChill);
			TempCancel();
		});
		BackChill();
		Step("_tail");
		Method(delegate
		{
			TempTalkTopic(bird + "1", null);
			Choice("yes2", delegate
			{
				TempTalkTopic(bird + "2", StepEnd);
				EClass.pc.SetAI(new AI_Fuck
				{
					target = c,
					sell = true
				});
			});
			Choice("no2", StepDefault, cancel: true).SetOnClick(RumorChill);
		});
		Step("_whore");
		Method(delegate
		{
			int costWhore = CalcMoney.Whore(c, EClass.pc);
			GameLang.refDrama1 = costWhore.ToString() ?? "";
			TempTalkTopic(bird + "3", null);
			Choice("yes2", delegate
			{
				if (EClass.pc.GetCurrency() < costWhore)
				{
					TempTalkTopic("nomoney", StepDefault);
				}
				else
				{
					SE.Pay();
					EClass.pc.ModCurrency(-costWhore);
					TempTalkTopic(bird + "2", StepEnd);
					EClass.pc.SetAI(new AI_Fuck
					{
						target = c
					});
				}
			});
			Choice("no2", StepDefault, cancel: true).SetOnClick(RumorChill);
		});
		Step("_picklock");
		Method(delegate
		{
			int cost8 = CalcMoney.Picklock(EClass.pc, destThing);
			GameLang.refDrama1 = cost8.ToString() ?? "";
			TempTalkTopic("bird3", null);
			Choice("yes2", delegate
			{
				if (destThing.c_lockedHard)
				{
					TempTalkTopic("lockTooHard", StepDefault);
				}
				else if (EClass.pc.GetCurrency() < cost8)
				{
					TempTalkTopic("nomoney", StepDefault);
				}
				else
				{
					SE.Pay();
					EClass.pc.ModCurrency(-cost8);
					manager.layer.SetOnKill(delegate
					{
						c.PlaySound("lock_open");
						c.Say("lockpick_success", c, destThing);
						c.ModExp(280, 200 + destThing.c_lockLv * 20);
						destThing.c_lockLv = 0;
						if (destThing.isLostProperty)
						{
							EClass.player.ModKarma(-8);
						}
						destThing.isLostProperty = false;
					});
					TempTalkTopic(destThing.isLostProperty ? "lockpick_purse" : "bird2", StepEnd);
				}
			});
			Choice("no2", StepDefault, cancel: true).SetOnClick(RumorChill);
		});
		Step("_buy");
		Method(delegate
		{
			if (c.id == "miral")
			{
				SE.Play("click_chat");
				if (EClass.pc.GetCurrency("medal") > 0)
				{
					TempTalkTopic("miral_medal", null);
				}
				else
				{
					TempTalkTopic("miral_medal2", StepDefault);
				}
			}
		});
		Method(delegate
		{
			if (EClass.player.IsCriminal && !EClass._zone.AllowCriminal && !EClass._zone.IsPCFaction && !c.trait.AllowCriminal)
			{
				SE.Play("click_chat");
				TempTalkTopic("shop_criminal", StepEnd);
			}
			else
			{
				sequence.Exit();
				manager.layer.Close();
				c.trait.OnBarter();
				if ((bool)WidgetFeed.Instance)
				{
					WidgetFeed.Instance.Talk(c, "barter");
				}
				layer = EClass.ui.AddLayer(LayerInventory.CreateBuy(c, c.trait.CurrencyType, c.trait.PriceType));
			}
		}, () => !layer, StepDefault);
		Step("_buyPlan");
		Method(delegate
		{
			List<ResearchPlan> plans = new List<ResearchPlan>();
			foreach (SourceResearch.Row row in EClass.sources.researches.rows)
			{
				if (EClass.BranchOrHomeBranch.researches.IsListBarter(row.id))
				{
					plans.Add(ResearchPlan.Create(row.id));
				}
			}
			EClass.ui.AddLayer<LayerList>().ManualList(delegate(UIList list, LayerList l)
			{
				list.moldItem = Resources.Load<ItemGeneral>("UI/Element/Item/ItemGeneralBarter").transform;
				list.callbacks = new UIList.Callback<ResearchPlan, ItemGeneral>
				{
					onInstantiate = delegate(ResearchPlan a, ItemGeneral b)
					{
						b.button1.mainText.text = a.Name;
						UIItem uIItem = Util.Instantiate<UIItem>("UI/Element/Item/Extra/costBarter", b.layout);
						HomeResource.Cost c2 = new HomeResource.Cost(EClass.BranchOrHomeBranch.resources.money, a.source.money);
						uIItem.text1.SetText(c2.cost.ToString() ?? "", (c2.resource.value >= c2.cost) ? FontColor.Good : FontColor.Bad);
						uIItem.image1.sprite = c2.resource.Sprite;
						b.button1.SetTooltip(delegate(UITooltip t)
						{
							a.WriteNote(t.note);
						});
						b.button1.onClick.AddListener(delegate
						{
							if (c2.resource.value < c2.cost)
							{
								SE.Beep();
							}
							else
							{
								c2.resource.Mod(-c2.cost);
								plans.Remove(a);
								EClass.BranchOrHomeBranch.researches.AddPlan(a);
								SE.Pay();
								list.List(refreshHighlight: true);
							}
						});
						b.RebuildLayout(recursive: true);
					},
					onList = delegate
					{
						foreach (ResearchPlan item6 in plans)
						{
							list.Add(item6);
						}
					}
				};
			}).SetSize()
				.windows[0].AttachCurrency().Build(new UICurrency.Options
			{
				branchMoney = true
			});
		}, () => !layer, StepDefault);
		Step("_upgradeHearth");
		Method(delegate
		{
			int cost7 = EClass.Branch.GetUpgradeCost();
			GameLang.refDrama1 = Lang._currency(cost7, "money");
			GameLang.refDrama2 = (EClass.Branch.lv + 1).ToString() ?? "";
			GameLang.refDrama3 = "hearth_dialog".lang(EClass.Branch.GetHearthHint(EClass.Branch.lv + 1));
			TempTalkTopic("upgrade_heath1", null);
			Choice("yes", delegate
			{
				if (EClass.pc.GetCurrency() < cost7)
				{
					TempTalkTopic("nomoney", StepDefault);
				}
				else
				{
					EClass.pc.ModCurrency(-cost7);
					SE.Pay();
					LayerDrama.Instance.SetOnKill(delegate
					{
						EClass.Branch.Upgrade();
					});
					TempTalkTopic("upgrade_heath2", StepEnd);
				}
			});
			Choice("no", StepDefault, cancel: true).SetOnClick(RumorChill);
		});
		Step("_sellFame");
		Method(delegate
		{
			int cost6 = EClass.player.fame / 5;
			GameLang.refDrama1 = cost6.ToString() ?? "";
			if (cost6 == 0)
			{
				TempTalkTopic("goto2", StepDefault);
			}
			else
			{
				TempTalkTopic("sellFame1", null);
				Choice("yes", delegate
				{
					EClass.pc.ModCurrency(cost6);
					SE.Pay();
					EClass.player.ModFame(-cost6);
					TempTalkTopic("sellFame2", StepDefault);
				});
				Choice("no", StepDefault, cancel: true).SetOnClick(RumorChill);
			}
		});
		Step("_investZone");
		Method(delegate
		{
			int cost5 = CalcMoney.InvestZone(EClass.pc);
			GameLang.refDrama1 = cost5.ToString() ?? "";
			GameLang.refDrama2 = ((EClass._zone.investment < 0) ? int.MaxValue : EClass._zone.investment).ToString() ?? "";
			TempTalkTopic("invest1", null);
			Choice("yes", delegate
			{
				Invest(quick: false);
			});
			Choice("no", StepDefault, cancel: true).SetOnClick(RumorChill);
			Choice("quickInvest", delegate
			{
				Invest(quick: true);
			});
			void Invest(bool quick)
			{
				if (EClass.pc.GetCurrency() < cost5)
				{
					TempTalkTopic("nomoney", StepDefault);
				}
				else
				{
					EClass.pc.ModCurrency(-cost5);
					SE.Pay();
					EClass._zone.investment += cost5;
					EClass._zone.ModDevelopment(5 + EClass.rnd(5));
					EClass._zone.ModInfluence(2);
					EClass.pc.ModExp(292, 100 + EClass._zone.development * 2);
					if (quick)
					{
						TempGoto("_investZone");
					}
					else
					{
						TempTalkTopic("invest2", StepDefault);
					}
				}
			}
		});
		Step("_investShop");
		Method(delegate
		{
			int cost4 = CalcMoney.InvestShop(EClass.pc, c);
			GameLang.refDrama1 = cost4.ToString() ?? "";
			GameLang.refDrama2 = c.trait.ShopLv.ToString() ?? "";
			TempTalkTopic("invest_shop1", null);
			Choice("yes", delegate
			{
				Invest(quick: false);
			});
			Choice("no", StepDefault, cancel: true).SetOnClick(RumorChill);
			Choice("quickInvest", delegate
			{
				Invest(quick: true);
			});
			void Invest(bool quick)
			{
				if (EClass.pc.GetCurrency() < cost4)
				{
					TempTalkTopic("nomoney", StepDefault);
				}
				else
				{
					EClass.pc.ModCurrency(-cost4);
					SE.Pay();
					c.c_invest++;
					EClass._zone.ModInfluence(1);
					EClass.pc.ModExp(292, 50 + c.c_invest * 20);
					Guild.Merchant.AddContribution(5 + c.c_invest);
					if (quick)
					{
						TempGoto("_investShop");
					}
					else
					{
						TempTalkTopic("invest_shop2", StepDefault);
					}
				}
			}
		});
		Step("_changeTitle");
		Method(delegate
		{
			EClass.player.title = WordGen.Get("title");
			GameLang.refDrama1 = EClass.player.title;
			TempTalk("changeTitle", StepDefault);
		}, null, StepDefault);
		Step("_buyLand");
		Method(delegate
		{
			bool num5 = EClass._map.bounds.CanExpand(1);
			int costLand = CalcGold.ExpandLand();
			GameLang.refDrama1 = "";
			GameLang.refDrama2 = costLand.ToString() ?? "";
			if (!num5)
			{
				TempTalkTopic("expand3", StepDefault);
			}
			else
			{
				TempTalkTopic("expand1", StepDefault);
				Choice("yes", delegate
				{
					if (EClass.pc.GetCurrency("money2") < costLand)
					{
						TempTalkTopic("nomoney", StepDefault);
					}
					else
					{
						SE.Pay();
						EClass.pc.ModCurrency(-costLand, "money2");
						TempTalkTopic("expand2", StepDefault);
						EClass._map.bounds.Expand(1);
						SE.Play("good");
						EClass._map.RefreshAllTiles();
						WidgetMinimap.UpdateMap();
						ScreenEffect.Play("Firework");
					}
				});
				Choice("no", StepDefault, cancel: true).SetOnClick(RumorChill);
			}
		});
		Step("_meeting");
		Method(delegate
		{
			sequence.Exit();
			manager.layer.Close();
			EClass.Branch.meetings.Start();
		});
		End();
		Step("_give");
		Method(delegate
		{
			manager.Hide();
			layer = LayerDragGrid.CreateGive(c);
			layer.SetOnKill(manager.Show);
		}, () => !layer, StepDefault);
		Step("_blessing");
		Method(delegate
		{
			bool flag6 = c.trait is TraitMiko_Mifu;
			TempTalkTopic("blessing", StepEnd);
			LayerDrama.Instance.SetOnKill(delegate
			{
				foreach (Chara member in EClass.pc.party.members)
				{
					if (c.trait is TraitMiko_Mifu)
					{
						member.AddCondition<ConHolyVeil>()?.SetPerfume();
					}
					else if (c.trait is TraitMiko_Nefu)
					{
						member.AddCondition<ConEuphoric>()?.SetPerfume();
					}
					else
					{
						member.AddCondition<ConNightVision>()?.SetPerfume();
					}
					member.Say("blessing", member);
					member.PlaySound("pray");
					member.PlayEffect("holyveil");
				}
				c.isRestocking = true;
			});
			c.c_dateStockExpire = EClass.world.date.GetRaw() + (flag6 ? 180 : 180) * 1440;
		});
		Step("_train");
		Method(delegate
		{
			layer = EClass.ui.AddLayer<LayerList>().ManualList(delegate(UIList list, LayerList l)
			{
				list.moldItem = Resources.Load<ButtonElement>("UI/Element/Button/ButtonElementTrain").transform;
				list.callbacks = new UIList.Callback<Element, ButtonElement>
				{
					onClick = delegate(Element a, ButtonElement b)
					{
						int num4 = (EClass.pc.elements.HasBase(a.id) ? CalcPlat.Train(EClass.pc, a) : CalcPlat.Learn(EClass.pc, a));
						if (num4 == 0)
						{
							SE.Beep();
						}
						else if (EClass.pc.TryPay(num4, "plat"))
						{
							if (EClass.pc.elements.HasBase(a.id))
							{
								EClass.pc.elements.Train(a.id);
							}
							else
							{
								EClass.pc.elements.Learn(a.id);
							}
							list.Redraw();
							UIButton.TryShowTip();
						}
					},
					onRedraw = delegate(Element a, ButtonElement b, int i)
					{
						bool flag5 = EClass.pc.elements.HasBase(a.id);
						b.imagePotential.enabled = flag5;
						b.SetElement(EClass.pc.elements.GetElement(a.id) ?? a, EClass.pc.elements);
						int plat = EClass.pc.GetCurrency("plat");
						int cost3 = (EClass.pc.elements.HasBase(a.id) ? CalcPlat.Train(EClass.pc, a) : CalcPlat.Learn(EClass.pc, a));
						b.mainText.text = b.mainText.text + " " + (flag5 ? "" : ("notLearned".lang() + " "));
						b.subText2.text = ((cost3 == 0) ? "-" : (cost3.ToString() ?? "")).TagColor(() => plat >= cost3 && cost3 != 0);
						b.RebuildLayout();
					},
					onInstantiate = delegate
					{
					},
					onList = delegate
					{
						foreach (SourceElement.Row item7 in EClass.sources.elements.rows.Where(delegate(SourceElement.Row a)
						{
							if (a.tag.Contains("unused"))
							{
								return false;
							}
							if (a.tag.Contains("guild"))
							{
								if (a.tag.Contains("fighter") && Guild.Fighter.IsCurrentZone)
								{
									return true;
								}
								if (a.tag.Contains("mage") && Guild.Mage.IsCurrentZone)
								{
									return true;
								}
								if (a.tag.Contains("thief") && Guild.Thief.IsCurrentZone)
								{
									return true;
								}
								if (a.tag.Contains("merchant") && Guild.Merchant.IsCurrentZone)
								{
									return true;
								}
								return false;
							}
							if (isInGuild)
							{
								return false;
							}
							return a.category == "skill" && a.categorySub == c.trait.IDTrainer;
						}).ToList())
						{
							list.Add(Element.Create(item7.id));
						}
					}
				};
			}).SetSize()
				.SetTitles("wTrain")
				.SetOnKill(SE.PopDrama);
			layer.windows[0].AttachCurrency().Build(new UICurrency.Options
			{
				plat = true
			});
		}, () => !layer, StepDefault);
		Step("_changeDomain");
		Method(delegate
		{
			layer = EClass.player.SelectDomain(SE.PopDrama);
		}, () => !layer, StepDefault);
		Step("_revive");
		Method(delegate
		{
			layer = EClass.ui.AddLayer(LayerPeople.Create<ListPeopleRevive>("h_revive"));
			layer.SetOnKill(SE.PopDrama);
			layer.windows[0].AttachCurrency().Build(new UICurrency.Options
			{
				money = true
			});
		}, () => !layer, StepDefault);
		Step("_buySlave");
		Method(delegate
		{
			LayerPeople.slaveToBuy = null;
			layer = EClass.ui.AddLayer(LayerPeople.Create<ListPeopleBuySlave>("h_invBuy", c));
			layer.SetOnKill(SE.PopDrama);
			layer.windows[0].AttachCurrency().Build(new UICurrency.Options
			{
				money = true
			});
			manager.Load();
		}, () => !layer, "_buySlaveConfirm");
		Step("_buySlaveConfirm");
		Method(delegate
		{
			Chara tc = LayerPeople.slaveToBuy;
			if (tc == null)
			{
				RumorChill();
			}
			else
			{
				int cost2 = CalcMoney.BuySlave(tc);
				GameLang.refDrama1 = cost2.ToString() ?? "";
				GameLang.refDrama2 = tc.Name;
				TempTalkTopic("slave_buy", null);
				Choice("yes", delegate
				{
					if (!EClass.pc.TryPay(cost2))
					{
						TempTalkTopic("nomoney", StepDefault);
					}
					else
					{
						GameLang.refDrama1 = tc.Name;
						EClass._zone.AddCard(tc, EClass.pc.pos.GetNearestPoint());
						tc.MakeAlly();
						c.GetObj<SlaverData>(5).list.Remove(tc);
						TempTalkTopic("slave_buy2", StepEnd);
					}
				}).DisableSound();
				Choice("no", StepDefault, cancel: true).SetOnClick(RumorChill);
			}
		});
		Goto(StepDefault);
		Step("_trade");
		Method(delegate
		{
			layer = EClass.ui.AddLayer(LayerInventory.CreateContainer(c));
		}, () => !layer, StepDefault);
		Step("_identify");
		Method(delegate
		{
			manager.Hide();
			c.trait.OnBarter();
			layer = LayerDragGrid.CreateIdentify(EClass.pc, superior: false, BlessedState.Normal, CalcMoney.Identify(EClass.pc, superior: false), -1);
			layer.SetOnKill(manager.Show);
		}, () => !layer, StepDefault);
		Step("_identifyAll");
		Method(delegate
		{
			int costIdentify = 0;
			int numIdentify = 0;
			int numSuperior = 0;
			EClass.pc.things.Foreach(delegate(Thing t)
			{
				if (!t.IsIdentified && t.c_IDTState != 1)
				{
					numIdentify++;
					costIdentify += CalcMoney.Identify(EClass.pc, superior: false);
				}
			});
			GameLang.refDrama1 = costIdentify.ToString() ?? "";
			GameLang.refDrama2 = numIdentify.ToString() ?? "";
			if (numIdentify == 0)
			{
				TempTalkTopic("appraise3", StepDefault);
			}
			else
			{
				TempTalkTopic("appraise1", StepDefault);
				Choice("yes", delegate
				{
					if (EClass.pc.GetCurrency() < costIdentify)
					{
						TempTalkTopic("nomoney", StepDefault);
					}
					else
					{
						SE.Pay();
						EClass.pc.ModCurrency(-costIdentify);
						foreach (Thing item8 in EClass.pc.things.List((Thing t) => !t.IsIdentified, onlyAccessible: true))
						{
							item8.Thing.Identify(show: false);
							if (!item8.IsInstalled)
							{
								numSuperior++;
							}
						}
						TempTalkTopic("appraise2", StepDefault);
					}
				});
				Choice("no", StepDefault, cancel: true).SetOnClick(RumorChill);
			}
		});
		Step("_identifySP");
		Method(delegate
		{
			manager.Hide();
			c.trait.OnBarter();
			layer = LayerDragGrid.CreateIdentify(EClass.pc, superior: true, BlessedState.Normal, CalcMoney.Identify(EClass.pc, superior: true), -1);
			layer.SetOnKill(manager.Show);
		}, () => !layer, StepDefault);
		Step("_bout");
		Method(delegate
		{
			TempTalkTopic("bout1", StepDefault);
			Choice("yes", delegate
			{
				Zone z = SpatialGen.CreateInstance("field", new ZoneInstanceBout
				{
					uidTarget = c.uid,
					targetX = c.pos.x,
					targetZ = c.pos.z
				});
				c.SetGlobal();
				z.events.AddPreEnter(new ZonePreEnterBout
				{
					target = c
				});
				c.SetInt(59, EClass.world.date.GetRaw());
				LayerDrama.Instance.SetOnKill(delegate
				{
					EClass.pc.MoveZone(z, ZoneTransition.EnterState.Center);
				});
				TempTalkTopic("bout2", StepEnd);
			});
			Choice("no", StepDefault, cancel: true).SetOnClick(RumorChill);
		}, null, StepDefault);
		Step("_news");
		Method(delegate
		{
			Zone zone = EClass.world.region.CreateRandomSite(EClass._zone, 5);
			if (zone == null)
			{
				TempTalkTopic("news2", null);
			}
			else
			{
				zone.isKnown = true;
				Msg.Say("discoverZone", zone.Name);
				GameLang.refDrama1 = zone.Name;
				TempTalkTopic("news1", null);
			}
			c.SetInt(33, EClass.world.date.GetRaw());
		});
		Method(delegate
		{
			manager.Load();
		}, null, StepDefault);
		Step("_heal");
		Method(delegate
		{
			int costHeal = CalcMoney.Heal(EClass.pc);
			GameLang.refDrama1 = costHeal.ToString() ?? "";
			TempTalkTopic("healer1", null);
			Choice("yes", delegate
			{
				if (EClass.pc.GetCurrency() < costHeal)
				{
					TempTalkTopic("nomoney", StepDefault);
				}
				else
				{
					SE.Pay();
					foreach (Chara member2 in EClass.pc.party.members)
					{
						ActEffect.Proc(EffectId.HealComplete, member2);
					}
					EClass.pc.ModCurrency(-costHeal);
					TempTalkTopic("healer2", StepEnd);
				}
			});
			Choice("no", StepDefault, cancel: true).SetOnClick(RumorChill);
		});
		Step("_food");
		Method(delegate
		{
			int cost = CalcMoney.Meal(EClass.pc);
			GameLang.refDrama1 = cost.ToString() ?? "";
			TempTalkTopic("food1", null);
			Choice("yes", delegate
			{
				if (EClass.pc.hunger.GetPhase() <= 0)
				{
					TempTalkTopic("alreadyFull", StepDefault);
				}
				else if (EClass.pc.GetCurrency() < cost)
				{
					TempTalkTopic("nomoney", StepDefault);
				}
				else
				{
					SE.Pay();
					EClass.pc.ModCurrency(-cost);
					TempTalkTopic("food2", StepDefault);
					FoodEffect.Proc(EClass.pc, ThingGen.Create("dish_lunch"));
					EClass.pc.hunger.value = 0;
				}
			});
			Choice("no", StepDefault, cancel: true).SetOnClick(RumorChill);
		});
		int[] bankTier = new int[5] { 100, 1000, 10000, 100000, 1000000 };
		Step("_deposit");
		Method(delegate
		{
			sequence.Exit();
			manager.layer.Close();
			if ((bool)WidgetFeed.Instance)
			{
				WidgetFeed.Instance.Talk(c, "barter");
			}
			SE.Play("shop_open");
			layer = LayerInventory.CreateContainer(EClass.game.cards.container_deposit);
		}, () => !layer, StepDefault);
		End();
		Step("_copyItem");
		Method(delegate
		{
			sequence.Exit();
			manager.layer.Close();
			if ((bool)WidgetFeed.Instance)
			{
				WidgetFeed.Instance.Talk(c, "barter");
			}
			SE.Play("shop_open");
			c.trait.OnBarter();
			if (c.c_copyContainer == null)
			{
				c.c_copyContainer = ThingGen.Create("container_deposit");
			}
			int numCopyItem = c.trait.NumCopyItem;
			c.c_copyContainer.things.SetSize(numCopyItem, 1);
			layer = LayerInventory.CreateContainer<InvOwnerCopyShop>(c, c.c_copyContainer);
		}, () => !layer, StepDefault);
		End();
		int[] taxTier = new int[6] { 0, 1000, 2000, 5000, 10000, 20000 };
		Step("_extraTax");
		Method(delegate
		{
			GameLang.refDrama1 = Lang._currency(EClass.player.extraTax, "money");
			TempTalkTopic("extraTax", null);
			int[] array2 = taxTier;
			foreach (int num3 in array2)
			{
				int _i2 = num3;
				Choice(Lang._currency(_i2, showUnit: true), delegate
				{
					EClass.player.extraTax = _i2;
					GameLang.refDrama1 = Lang._currency(_i2, "money");
					TempTalkTopic("extraTax2", StepDefault);
				});
			}
			Choice("no2", StepDefault, cancel: true).SetOnClick(RumorChill);
		});
		Step("_withdraw");
		Method(delegate
		{
			int bankMoney = EClass.player.bankMoney;
			GameLang.refDrama1 = bankMoney.ToString() ?? "";
			TempTalkTopic("banker2", null);
			int[] array = bankTier;
			foreach (int num2 in array)
			{
				int _i = num2;
				if (EClass.player.bankMoney >= _i)
				{
					Choice(Lang._currency(_i, showUnit: true), delegate
					{
						SE.Pay();
						EClass.pc.ModCurrency(_i);
						EClass.player.bankMoney -= _i;
						TempTalkTopic("banker4", StepDefault);
					});
				}
			}
			Choice("no2", StepDefault, cancel: true).SetOnClick(RumorChill);
		});
		Step("_disableMove");
		Method(delegate
		{
			c.noMove = true;
			c.orgPos = new Point(c.pos);
			c.Talk("ok");
		});
		End();
		Step("_enableMove");
		Method(delegate
		{
			c.noMove = false;
			c.orgPos = null;
			c.Talk("thanks");
		});
		End();
		Step("_bye");
		Method(delegate
		{
			c.Talk("bye");
		});
		End();
		void BackChill()
		{
			Method(RumorChill, null, StepDefault);
		}
		string GetTalk(string id)
		{
			return c.GetTalkText(id);
		}
		void RumorChill()
		{
			rumor = GetTalk("chill");
		}
		void Talk(string idTalk, string idJump)
		{
			_Talk("tg", GetTalk(idTalk), idJump);
		}
		void TempCancel()
		{
			EnableCancel("back");
		}
		void TempTalk(string idTalk, string idJump)
		{
			_TempTalk("tg", GetTalk(idTalk), idJump);
		}
		void TempTalkTopic(string idTopc, string idJump)
		{
			_TempTalk("tg", GetTopic(c, idTopc), idJump);
		}
		bool taken()
		{
			if (c.quest != null)
			{
				return EClass.game.quests.list.Contains(c.quest);
			}
			return false;
		}
	}

	public string GetRumor(Chara c)
	{
		if (c.interest <= 0)
		{
			return GetText(c, "rumor", "bored");
		}
		if (HasTopic("unique", c.id))
		{
			manager.enableTone = false;
			return GetText(c, "unique", c.id);
		}
		if (EClass.rnd(2) == 0 && !c.trait.IDRumor.IsEmpty())
		{
			return GetText(c, "rumor", c.trait.IDRumor);
		}
		if (EClass.rnd(2) == 0 && HasTopic("zone", EClass._zone.id))
		{
			return GetText(c, "zone", EClass._zone.id);
		}
		if (EClass.rnd(2) == 0)
		{
			return GetText(c, "rumor", "interest_" + c.bio.idInterest.ToEnum<Interest>());
		}
		if (EClass.rnd(2) == 0)
		{
			return c.GetTalkText("rumor");
		}
		if (EClass.rnd(4) == 0)
		{
			return GetText(c, "rumor", "hint");
		}
		return GetText(c, "rumor", "default");
	}

	public bool HasTopic(string idSheet, string idTopic)
	{
		string path = CorePath.CorePackage.TextDialog + "dialog.xlsx";
		if (!File.Exists(path))
		{
			return false;
		}
		ExcelData excelData = new ExcelData();
		excelData.path = path;
		excelData.BuildMap(idSheet);
		return excelData.sheets[idSheet].map.ContainsKey(idTopic);
	}

	public string GetText(Chara c, string idSheet, string idTopic)
	{
		if (!idTopic.IsEmpty() && manager.customTalkTopics.ContainsKey(idTopic))
		{
			return manager.customTalkTopics[idTopic];
		}
		string[] dialog = Lang.GetDialog(idSheet, idTopic);
		if (!manager.enableTone && !(idSheet == "general"))
		{
			return dialog.RandomItem();
		}
		return c.ApplyTone(dialog.RandomItem());
	}

	public string GetTopic(Chara c, string idTopic = null)
	{
		return GetText(c, "general", idTopic);
	}

	public void BuildTextData()
	{
		Step("import");
		string[] array = setup.textData.Split(new string[1] { Environment.NewLine + Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
		foreach (string text in array)
		{
			if (!(text == Environment.NewLine))
			{
				_Talk("", text.TrimStart(Environment.NewLine.ToCharArray()).TrimEnd(Environment.NewLine.ToCharArray()));
			}
		}
		End();
	}

	public DramaEvent Event(DramaEvent e)
	{
		events.Add(e);
		e.sequence = sequence;
		return e;
	}

	public void Step(string step)
	{
		Event(new DramaEvent
		{
			step = step
		});
	}

	public void Method(Action action, Func<bool> endFunc = null, string idJump = null)
	{
		Event(new DramaEventMethod(action)
		{
			endFunc = endFunc
		});
		if (!idJump.IsEmpty())
		{
			Event(new DramaEventGoto(idJump));
		}
	}

	public void End()
	{
		Event(new DramaEventGoto("end"));
	}

	public void Goto(string idJump)
	{
		Event(new DramaEventGoto(idJump));
	}

	public void GotoDefault()
	{
		Event(new DramaEventGoto(StepDefault));
	}

	public void _Talk(string idActor, string text, string idJump = null)
	{
		manager.lastTalk = Event(new DramaEventTalk
		{
			idActor = idActor,
			idJump = idJump,
			text = text
		}) as DramaEventTalk;
	}

	public void _Talk(string idActor, Func<string> funcText, string idJump = null)
	{
		manager.lastTalk = Event(new DramaEventTalk
		{
			idActor = idActor,
			idJump = idJump,
			funcText = funcText
		}) as DramaEventTalk;
	}

	public void _TempTalk(string idActor, string text, string idJump = null)
	{
		manager.lastTalk = Event(new DramaEventTalk
		{
			idActor = idActor,
			idJump = idJump,
			text = text,
			temp = true
		}) as DramaEventTalk;
		sequence.tempEvents.Add(manager.lastTalk);
	}

	public void TempGoto(string idJump = null)
	{
		sequence.tempEvents.Clear();
		sequence.Play(idJump);
	}

	public DramaChoice Choice(string lang, string idJump, bool cancel = false)
	{
		DramaChoice dramaChoice = new DramaChoice(lang.lang(), idJump);
		manager.lastTalk.AddChoice(dramaChoice);
		if (cancel)
		{
			EnableCancel(idJump);
		}
		return dramaChoice;
	}

	public DramaChoice Choice(string lang, Action onJump)
	{
		DramaChoice dramaChoice = new DramaChoice(lang.lang(), null);
		dramaChoice.onJump = onJump;
		manager.lastTalk.AddChoice(dramaChoice);
		return dramaChoice;
	}

	public DramaChoice Choice2(string lang, string idJump)
	{
		DramaChoice dramaChoice = new DramaChoice(lang.lang(), idJump);
		manager.lastTalk.AddChoice(dramaChoice);
		sequence.manager._choices.Add(dramaChoice);
		return dramaChoice;
	}

	public void EnableCancel(string idCancelJump = null)
	{
		manager.lastTalk.canCancel = true;
		manager.lastTalk.idCancelJump = idCancelJump;
	}

	public List<Card> GetListGuide()
	{
		List<Card> list = new List<Card>();
		foreach (Thing thing in EClass._map.things)
		{
			if (thing.IsInstalled && thing.trait.GuidePriotiy > 0)
			{
				list.Add(thing);
			}
		}
		foreach (Chara chara in EClass._map.charas)
		{
			if (!chara.IsPCParty && chara.trait.GuidePriotiy > 0)
			{
				list.Add(chara);
			}
		}
		list.Sort((Card a, Card b) => b.trait.GuidePriotiy - a.trait.GuidePriotiy);
		return list;
	}
}
