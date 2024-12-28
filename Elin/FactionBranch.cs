using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class FactionBranch : EClass
{
	public class Statistics : EClass
	{
		[JsonProperty]
		public int ship;

		[JsonProperty]
		public int inn;

		[JsonProperty]
		public int tourism;

		[JsonProperty]
		public int shop;

		[JsonProperty]
		public int numSold;

		[JsonProperty]
		public int visitor;

		[JsonProperty]
		public int tax;
	}

	[JsonProperty]
	public int lv = 1;

	[JsonProperty]
	public int rank;

	[JsonProperty]
	public int exp;

	[JsonProperty]
	public int seedPlan;

	[JsonProperty]
	public int temper;

	[JsonProperty]
	public int uidMaid;

	[JsonProperty]
	public int lastUpdateReqruit;

	[JsonProperty]
	public int incomeShop;

	[JsonProperty]
	public GStability stability = new GStability
	{
		value = 1
	};

	[JsonProperty]
	public HomeResourceManager resources = new HomeResourceManager();

	[JsonProperty]
	public ResearchManager researches = new ResearchManager();

	[JsonProperty]
	public PolicyManager policies = new PolicyManager();

	[JsonProperty]
	public HappinessManager happiness = new HappinessManager();

	[JsonProperty]
	public MeetingManager meetings = new MeetingManager();

	[JsonProperty]
	public Thing stash;

	[JsonProperty]
	public MsgLog log = new MsgLog
	{
		id = "log"
	};

	[JsonProperty]
	public Religion faith;

	[JsonProperty]
	public ExpeditionManager expeditions = new ExpeditionManager();

	[JsonProperty]
	public List<HireInfo> listRecruit = new List<HireInfo>();

	[JsonProperty]
	public int dateFound;

	[JsonProperty]
	public int tourism;

	[JsonProperty]
	public Statistics statistics = new Statistics();

	[JsonProperty]
	public Statistics lastStatistics = new Statistics();

	public Zone owner;

	public int incomeInn;

	public int incomeTourism;

	public int efficiency = 100;

	public List<Chara> members = new List<Chara>();

	public int Money => resources.money.value;

	public int Worth => resources.worth.value;

	public int NumHeirloom => 4 + Evalue(2120);

	public int MaxAP => 4 + Evalue(2115) + Evalue(3900) * 5 + Evalue(3709) * lv;

	public int MaxPopulation => 5 + Evalue(2204);

	public int MaxSoil => ((int)(Mathf.Sqrt(EClass._map.bounds.Width * EClass._map.bounds.Height) * 3f) + Evalue(2200) * 5) * (100 + Evalue(3700) * 25) / 100;

	public int ContentLV => Mathf.Max(1, lv * 4 + EClass.scene.elomap.GetRoadDist(EClass._zone.x, EClass._zone.y) - 4);

	public int DangerLV => Mathf.Max(1, ContentLV - (int)Mathf.Sqrt(Evalue(2704)) * 2 + (int)Mathf.Sqrt(Evalue(2706)) * 4);

	public bool HasItemProtection => lv >= 3;

	public bool HasNetwork => lv >= 5;

	public bool IsTaxFree => policies.IsActive(2514);

	public string RankText => (0.01f * (float)exp + (float)rank).ToString("F2");

	public string TextLv => lv + " (" + (100f * (float)exp / (float)GetNextExp()).ToString("F1") + ")";

	public bool IsStartBranch => owner.id == "startSite";

	public ElementContainerZone elements => owner.elements;

	public int MaxLv => 7;

	public int GetNextExp(int _lv = -1)
	{
		if (_lv == -1)
		{
			_lv = lv;
		}
		return _lv * _lv * 100 + 100;
	}

	public int Evalue(int ele)
	{
		SourceElement.Row row = EClass.sources.elements.map[ele];
		if (row.category == "policy")
		{
			return policies.GetValue(ele);
		}
		if (HasNetwork && row.tag.Contains("network"))
		{
			return EClass.pc.faction.elements.Value(ele) + elements.Value(ele);
		}
		return elements.Value(ele);
	}

	public int GetProductBonus(Chara c)
	{
		if (c.isDead || c.IsPCParty)
		{
			return 0;
		}
		return Mathf.Max(1, (90 + Evalue(2116) / 2) * efficiency / 100);
	}

	public void RefreshEfficiency()
	{
		int num = 100;
		int num2 = CountMembers(FactionMemberType.Default);
		int ration = 0;
		foreach (Chara member in members)
		{
			if (member.memberType != 0)
			{
				continue;
			}
			if (member.IsPCParty || member.homeBranch == null || member.homeBranch.owner == null)
			{
				return;
			}
			foreach (Hobby item in member.ListHobbies())
			{
				TryAdd(item);
			}
			foreach (Hobby item2 in member.ListWorks())
			{
				TryAdd(item2);
			}
		}
		if (num2 > MaxPopulation)
		{
			num -= (num2 - MaxPopulation) * 20 * 100 / (100 + 20 * (int)Mathf.Sqrt(ration));
		}
		efficiency = num;
		void TryAdd(Hobby h)
		{
			if (!h.source.elements.IsEmpty())
			{
				for (int i = 0; i < h.source.elements.Length; i += 2)
				{
					if (h.source.elements[i] == 2207)
					{
						ration += h.source.elements[i + 1];
					}
				}
			}
		}
	}

	public void Abandon()
	{
	}

	public void OnCreate(Zone zone)
	{
		SetOwner(zone);
		stash = ThingGen.Create("container_salary");
		stash.RemoveThings();
		faith = EClass.game.religions.Eyth;
		dateFound = EClass.world.date.GetRaw();
	}

	public void SetOwner(Zone zone)
	{
		owner = zone;
		resources.SetOwner(this);
		researches.SetOwner(this);
		policies.SetOwner(this);
		happiness.SetOwner(this);
		expeditions.SetOwner(this);
		meetings.SetOwner(this);
		foreach (Chara value in EClass.game.cards.globalCharas.Values)
		{
			if (value == null)
			{
				Debug.LogError("exception: c==null");
			}
			else if (EClass.pc == null)
			{
				Debug.LogError("exception: pc==null");
			}
			else if (value.homeZone == zone && value.faction == EClass.pc.faction)
			{
				members.Add(value);
				EClass.pc.faction.charaElements.OnAddMemeber(value);
			}
		}
		RefreshEfficiency();
	}

	public void OnActivateZone()
	{
		incomeInn = 0;
		incomeTourism = 0;
		foreach (Chara member in members)
		{
			if (member.IsAliveInCurrentZone && !member.pos.IsInBounds)
			{
				member.MoveImmediate(EClass._map.GetCenterPos().GetNearestPoint());
			}
			member.RefreshWorkElements(elements);
		}
		ValidateUpgradePolicies();
	}

	public void OnAfterSimulate()
	{
		if (!EClass.game.isLoading)
		{
			GetDailyIncome();
		}
		foreach (Chara chara in EClass._map.charas)
		{
			if (!chara.IsPCParty && !chara.noMove && (chara.pos.cell.HasBlock || chara.pos.cell.hasDoor) && !chara.isRestrained && !chara.HasCondition<ConSuspend>())
			{
				chara.MoveImmediate(chara.pos.GetNearestPoint(allowBlock: false, allowChara: false) ?? chara.pos);
			}
		}
	}

	public void OnUnloadMap()
	{
	}

	public void OnSimulateHour(VirtualDate date)
	{
		int num = CountMembers(FactionMemberType.Default, onlyAlive: true);
		int starveChance = Mathf.Max(num - MaxPopulation, 0);
		members.ForeachReverse(delegate(Chara c)
		{
			if (!c.IsPCParty && c.IsAliveInCurrentZone && !c.noMove)
			{
				if (EClass.rnd(24 * members.Count) < starveChance)
				{
					starveChance--;
				}
				else
				{
					c.TickWork(date);
				}
				if (!date.IsRealTime && EClass.rnd(3) != 0 && c.memberType == FactionMemberType.Default)
				{
					c.hunger.Mod(1);
					if (c.hunger.GetPhase() >= 3)
					{
						Thing meal = GetMeal(c);
						if (meal != null)
						{
							FoodEffect.Proc(c, meal);
							meal.Destroy();
						}
						else
						{
							c.hunger.Mod(-50);
						}
					}
				}
			}
		});
		expeditions.OnSimulateHour();
		meetings.OnSimulateHour(date);
		policies.OnSimulateHour(date);
		int num2 = Evalue(3707);
		int num3 = Evalue(3705) + Evalue(3710) * 2;
		int num4 = 4 + num3 * 4;
		int num5 = Mathf.Max(1, (100 + Evalue(2205) * 5 + Evalue(3703) * 100 - num2 * 80) / 10);
		int num6 = 10;
		if (policies.IsActive(2709))
		{
			num6 = 0;
		}
		if (EClass.world.date.IsNight)
		{
			num5 /= 2;
		}
		if ((date.IsRealTime || num2 > 0) && !EClass.debug.enable && EClass.rnd(num5) == 0 && !EClass.pc.IsDeadOrSleeping && EClass._map.CountHostile() < num4)
		{
			int num7 = 1 + EClass.rnd(num2 + num3 + 1);
			for (int i = 0; i < num7; i++)
			{
				EClass._zone.SpawnMob(null, SpawnSetting.HomeEnemy(DangerLV));
			}
		}
		if ((date.IsRealTime || num2 == 0) && num6 != 0 && EClass.rnd(num6) == 0 && EClass._map.CountWildAnimal() < num4)
		{
			EClass._zone.SpawnMob(null, SpawnSetting.HomeWild(5));
		}
		if (EClass.rnd(5) == 0 && policies.IsActive(2810))
		{
			int num8 = 3 + lv + Evalue(2206) / 5 + Evalue(3702) * 2 + Evalue(2202) / 2;
			num8 = num8 * (100 + Evalue(3702) * 20 + Evalue(2206)) / 100;
			num8 = num8 * (100 + (int)Mathf.Sqrt(Evalue(2811)) * 3) / 100;
			if (EClass._map.CountGuest() < num8)
			{
				Chara chara;
				if (policies.IsActive(2822) && Mathf.Sqrt(Evalue(2822) / 2) + 5f >= (float)EClass.rnd(100))
				{
					chara = CharaGen.CreateWealthy(ContentLV);
					EClass._zone.AddCard(chara, EClass._zone.GetSpawnPos(SpawnPosition.Random) ?? EClass._map.GetRandomSurface());
				}
				else
				{
					chara = EClass._zone.SpawnMob(null, SpawnSetting.HomeGuest(ContentLV));
				}
				if (chara != null && (chara.id == "nun_mother" || chara.id == "prostitute") && policies.IsActive(2710))
				{
					chara.Destroy();
					chara = null;
				}
				if (chara != null)
				{
					statistics.visitor++;
					chara.memberType = FactionMemberType.Guest;
					chara.SetInt(34, EClass.world.date.GetRaw());
					chara.c_allowance = chara.LV * 100;
					if (chara.IsWealthy)
					{
						chara.c_allowance *= 10;
					}
					if (date.IsRealTime)
					{
						Msg.Say("guestArrive", chara.Name);
					}
					else
					{
						chara.TryAssignBed();
					}
				}
			}
		}
		foreach (Chara chara3 in EClass._map.charas)
		{
			if (chara3.memberType == FactionMemberType.Guest && ((chara3.c_allowance <= 0 && EClass.rnd(2) == 0) || chara3.GetInt(34) + 10080 < EClass.world.date.GetRaw()))
			{
				if (!chara3.IsGlobal)
				{
					chara3.Destroy();
				}
				chara3.ClearBed();
				break;
			}
		}
		if (date.hour == 5)
		{
			DailyOutcome(date);
			GenerateGarbage(date);
			if (!date.IsRealTime)
			{
				AutoClean();
			}
			CalcTourismIncome();
			CalcInnIncome();
			if (date.IsRealTime)
			{
				GetDailyIncome();
			}
		}
		if (!EClass.player.simulatingZone && date.hour == 6)
		{
			ReceivePackages(date);
		}
		if (!date.IsRealTime && date.hour % 8 == 0)
		{
			foreach (Chara item in EClass._map.charas.Where((Chara c) => c.memberType == FactionMemberType.Guest).ToList())
			{
				for (int j = 0; j < 3; j++)
				{
					AI_Shopping.TryShop(item, realtime: false);
				}
			}
			AI_Shopping.TryRestock(EClass.pc, realtime: false);
		}
		if (EClass.rnd(24) == 0 || EClass.debug.enable)
		{
			foreach (Thing thing in EClass._map.things)
			{
				if (EClass.rnd(20) == 0 && thing.trait is TraitFoodEggFertilized && thing.pos.FindThing<TraitBed>() != null)
				{
					Chara chara2 = TraitFoodEggFertilized.Incubate(thing, thing.pos);
					thing.ModNum(-1);
					if (date.IsRealTime)
					{
						chara2.PlaySound("egg");
					}
					break;
				}
			}
		}
		resources.SetDirty();
	}

	public void AutoClean()
	{
		foreach (Chara member in members)
		{
			if (member.IsPCParty || !member.ExistsOnMap || member.memberType != 0)
			{
				continue;
			}
			Hobby hobby = member.GetWork("Clean") ?? member.GetWork("Chore");
			if (hobby == null)
			{
				continue;
			}
			int num = (5 + EClass.rnd(5)) * hobby.GetEfficiency(member) / 100;
			for (int i = 0; i < num; i++)
			{
				Thing thingToClean = AI_Haul.GetThingToClean(member);
				if (thingToClean == null)
				{
					break;
				}
				EClass._zone.TryAddThingInSharedContainer(thingToClean);
			}
		}
	}

	public void OnSimulateDay(VirtualDate date)
	{
		if (owner == null || owner.mainFaction != EClass.Home)
		{
			return;
		}
		researches.OnSimulateDay();
		resources.OnSimulateDay();
		happiness.OnSimulateDay();
		if (!date.IsRealTime)
		{
			BranchMap branchMap = date.GetBranchMap();
			foreach (Chara member in members)
			{
				if (!member.IsPCParty && member.memberType != FactionMemberType.Livestock && !member.faith.IsEyth && !member.c_isPrayed && branchMap.altarMap.Contains(member.faith.id))
				{
					AI_Pray.Pray(member, silent: true);
				}
			}
		}
		foreach (Chara member2 in members)
		{
			member2.c_isPrayed = false;
			member2.c_isTrained = false;
		}
	}

	public void OnAdvanceDay()
	{
		int num = 0;
		foreach (Chara member in members)
		{
			if (!member.IsPC && !member.isDead && member.memberType == FactionMemberType.Default)
			{
				if (EClass.rnd(3) == 0)
				{
					num++;
				}
				if (member.GetWork("Pioneer") != null)
				{
					num += 3;
				}
			}
		}
		ModExp(num);
	}

	public void OnSimulateMonth(VirtualDate date)
	{
		lastStatistics = statistics;
		statistics = new Statistics();
	}

	public void GenerateGarbage(VirtualDate date)
	{
		int sortChance = 40 + GetCivility();
		int unsortedCount = 0;
		foreach (Chara member in members)
		{
			if (!member.IsPCParty && member.ExistsOnMap)
			{
				Hobby hobby = member.GetWork("Clean") ?? member.GetWork("Chore");
				if (hobby != null)
				{
					sortChance += 20 * hobby.GetEfficiency(member) / 100;
				}
			}
		}
		int num = Evalue(2702);
		int num2 = EClass.rnd(2);
		for (int i = 0; i < num2; i++)
		{
			if (num == 0 || EClass.rnd(num + (EClass.debug.enable ? 100000 : 10)) == 0)
			{
				Generate(null);
			}
		}
		foreach (Chara member2 in members)
		{
			if (!member2.IsPCParty && member2.ExistsOnMap && (num == 0 || EClass.rnd(num + (EClass.debug.enable ? 100000 : 10)) == 0))
			{
				Generate(member2);
			}
		}
		void Generate(Chara c)
		{
			if (c == null || EClass.rnd(5) == 0)
			{
				if (EClass.rnd(80) == 0)
				{
					Add(ThingGen.CreateFromCategory("junk"));
				}
				else if (EClass.rnd(40) == 0)
				{
					Thing thing;
					Add(thing = ThingGen.CreateFromTag("garbage"));
				}
				else
				{
					string id = "trash2";
					if (EClass.rnd(3) == 0)
					{
						id = "trash1";
					}
					if (EClass.rnd(10) == 0)
					{
						id = ((EClass.rnd(3) == 0) ? "529" : "1170");
					}
					if (c != null && (!c.IsUnique || c.memberType == FactionMemberType.Livestock) && EClass.rnd((c.memberType == FactionMemberType.Livestock) ? 2 : 50) == 0)
					{
						id = "_poop";
					}
					if (EClass.rnd(200000) == 0)
					{
						id = "goodness";
					}
					Add(ThingGen.Create(id));
				}
			}
			void Add(Thing t)
			{
				if (sortChance < EClass.rnd(100))
				{
					EClass._zone.TryAddThing(t, (c != null) ? (c.pos.GetRandomPoint(2) ?? c.pos).GetNearestPoint() : EClass._zone.bounds.GetRandomSurface(), destroyIfFail: true);
				}
				else
				{
					if (t.id == "_poop" && EClass.rnd(150) == 0)
					{
						t.ChangeMaterial((EClass.rnd(2) == 0) ? "silver" : "gold");
					}
					if (!TryTrash(t))
					{
						unsortedCount++;
						if (unsortedCount >= 5)
						{
							Tutorial.Reserve("garbage");
						}
					}
				}
			}
		}
	}

	public bool TryTrash(Thing t)
	{
		Thing thing = ((t.id == "_poop" || t.source._origin == "dish") ? EClass._map.props.installed.FindEmptyContainer<TraitContainerCompost>(t) : (t.isFireproof ? EClass._map.props.installed.FindEmptyContainer<TraitContainerUnburnable>(t) : EClass._map.props.installed.FindEmptyContainer<TraitContainerBurnable>(t)));
		if (thing != null)
		{
			thing.AddCard(t);
			return true;
		}
		return EClass._zone.TryAddThingInSpot<TraitSpotGarbage>(t, useContainer: false);
	}

	public void ReceivePackages(VirtualDate date)
	{
		if (EClass.player.simulatingZone)
		{
			return;
		}
		List<Thing> listPackage = EClass.game.cards.listPackage;
		if (EClass.player.stats.days >= 2 && !EClass.player.flags.elinGift && Steam.HasDLC(ID_DLC.BackerReward))
		{
			listPackage.Add(ThingGen.Create("gift"));
			EClass.player.flags.elinGift = true;
		}
		if (listPackage.Count == 0)
		{
			return;
		}
		SE.Play("notice");
		int num = 0;
		foreach (Thing item in listPackage)
		{
			if (item.id != "bill")
			{
				num++;
			}
			PutInMailBox(item, item.id == "cardboard_box" || item.id == "gift");
			WidgetPopText.Say("popDeliver".lang(item.Name));
		}
		Msg.Say("deliver_arrive", num.ToString() ?? "");
		listPackage.Clear();
	}

	public void DailyOutcome(VirtualDate date)
	{
		Thing thing = null;
		foreach (Chara member in members)
		{
			Chara i = member;
			if (EClass.rnd(EClass.debug.enable ? 2 : (360 * members.Count)) == 0 && !i.IsPC && EClass.pc.faction.IsGlobalPolicyActive(2712) && i.things.Find((Thing t) => t.id == "panty" && t.c_idRefCard == i.id) == null && !i.things.IsFull())
			{
				Thing thing2 = ThingGen.Create("panty");
				thing2.c_idRefCard = i.id;
				i.AddThing(thing2);
			}
			if (i.IsPCParty || !i.ExistsOnMap)
			{
				continue;
			}
			i.RefreshWorkElements(elements);
			if (i.memberType == FactionMemberType.Livestock)
			{
				if (thing == null)
				{
					thing = EClass._map.Stocked.Find("pasture", -1, -1, shared: true) ?? EClass._map.Installed.Find("pasture");
				}
				if (thing == null)
				{
					continue;
				}
				if (i.race.breeder >= EClass.rnd(2500 - (int)Mathf.Sqrt(Evalue(2827) * 100)))
				{
					if (EClass.rnd(3) != 0)
					{
						Thing t2 = i.MakeEgg(date.IsRealTime, 1, date.IsRealTime);
						if (!date.IsRealTime)
						{
							i.TryPutShared(t2);
						}
					}
					else
					{
						Thing t3 = i.MakeMilk(date.IsRealTime, 1, date.IsRealTime);
						if (!date.IsRealTime)
						{
							i.TryPutShared(t3);
						}
					}
				}
				if (i.HaveFur())
				{
					i.c_fur++;
				}
				thing.ModNum(-1);
				if (thing.isDestroyed || thing.Num == 0)
				{
					thing = null;
				}
				continue;
			}
			foreach (Hobby item in i.ListHobbies())
			{
				GetOutcome(item);
			}
			foreach (Hobby item2 in i.ListWorks())
			{
				GetOutcome(item2);
			}
			void GetOutcome(Hobby h)
			{
				int num3 = h.GetEfficiency(i) * GetProductBonus(i) / 100;
				int num4 = h.GetLv(i);
				int id = EClass.sources.elements.alias[h.source.skill].id;
				if (!i.elements.HasBase(id))
				{
					i.elements.SetBase(id, 1);
				}
				i.ModExp(id, 100);
				for (int j = 0; j < h.source.things.Length; j += 2)
				{
					string text = h.source.things[j];
					int num5 = Mathf.Max(1, h.source.things[j + 1].ToInt() * num3 / 1000);
					int num6 = num5 / 1000;
					if (num5 % 1000 > EClass.rnd(1000))
					{
						num6++;
					}
					if (num6 != 0)
					{
						if (!(text == "_egg"))
						{
							if (text == "milk")
							{
								i.MakeMilk(date.IsRealTime, num6);
							}
							else
							{
								Thing thing4 = ((!text.StartsWith("#")) ? ThingGen.Create(h.source.things[j], -1, num4) : ThingGen.CreateFromCategory(text.Replace("#", ""), num4));
								if (thing4 != null)
								{
									if (!thing4.trait.CanStack)
									{
										num6 = 1;
									}
									thing4.SetNum(thing4.trait.CraftNum * num6);
									thing4.SetBlessedState(BlessedState.Normal);
									thing4.TryMakeRandomItem(num4);
									if (thing4.IsAmmo)
									{
										thing4.ChangeMaterial("iron");
										thing4.c_IDTState = 0;
									}
									bool flag = thing4.category.id == "garbage";
									if (thing4.trait is TraitFoodMeal)
									{
										if (thing4.HasTag(CTAG.dish_fail))
										{
											flag = true;
										}
										else
										{
											CraftUtil.MakeDish(thing4, num4 + 10, i);
										}
									}
									if (flag)
									{
										TryTrash(thing4);
									}
									else
									{
										i.TryPutShared(thing4);
									}
								}
							}
						}
						else
						{
							i.MakeEgg(date.IsRealTime, num6);
						}
					}
				}
				switch (h.source.alias)
				{
				case "Nurse":
				{
					foreach (Chara member2 in members)
					{
						if (!member2.IsPCParty)
						{
							if (member2.isDead && EClass.rnd(num3) > EClass.rnd(100))
							{
								Log("bNurse", i, member2);
								member2.Revive(member2.pos, msg: true);
								break;
							}
							if (EClass.rnd(num3) > EClass.rnd(100))
							{
								member2.CureHost(CureType.HealComplete);
							}
						}
					}
					break;
				}
				case "Chore":
				case "Clean":
				{
					for (int k = 0; k < num3 / 2; k++)
					{
						Point randomPoint = EClass._map.bounds.GetRandomPoint();
						if (randomPoint.HasDecal)
						{
							EClass._map.SetDecal(randomPoint.x, randomPoint.z);
						}
					}
					break;
				}
				case "TreasureHunt":
					if (EClass.rnd(num3) > EClass.rnd(EClass.debug.enable ? 100 : 5000))
					{
						Thing thing5 = EClass._zone.TryGetThingFromSharedContainer((Thing t) => t.trait is TraitScrollMapTreasure);
						if (thing5 != null)
						{
							Thing thing6 = ThingGen.CreateTreasure("chest_treasure", thing5.LV);
							i.TryPutShared(thing6);
							thing5.Destroy();
							WidgetPopText.Say("foundTreasure".lang(thing6.Name));
						}
					}
					break;
				}
			}
		}
		int soilCost = EClass._zone.GetSoilCost();
		int num = Mathf.Min(100, 70 + (MaxSoil - soilCost));
		int flower = 5;
		int lv = 1;
		EClass._map.bounds.ForeachCell(delegate(Cell cell)
		{
			if (cell.obj != 0 && cell.sourceObj.tag.Contains("flower"))
			{
				PlantData plantData = cell.TryGetPlant();
				if (plantData != null && plantData.seed != null)
				{
					lv += plantData.seed.encLV + 1;
				}
				else
				{
					lv++;
				}
				flower++;
			}
		});
		lv /= flower;
		int num2 = 0;
		foreach (Thing thing7 in EClass._map.things)
		{
			if (thing7.IsInstalled && thing7.trait is TraitBeekeep && !thing7.things.IsFull())
			{
				flower -= 3 + EClass.rnd(5 + num2 * 4);
				num2++;
				if (flower < 0)
				{
					break;
				}
				if (EClass.rnd(100) <= num)
				{
					Thing thing3 = ThingGen.Create("honey");
					thing3.SetEncLv(lv / 10);
					thing3.elements.SetBase(2, EClass.curve(lv, 50, 10, 80));
					thing7.AddThing(thing3);
				}
			}
		}
	}

	public void PutInMailBox(Thing t, bool outside = false, bool install = true)
	{
		if (!outside)
		{
			Thing mailBox = GetMailBox();
			if (mailBox != null)
			{
				mailBox.AddCard(t);
				return;
			}
		}
		Point mailBoxPos = GetMailBoxPos();
		EClass._zone.AddCard(t, mailBoxPos);
		if (install)
		{
			t.Install();
		}
	}

	public Thing GetMailBox()
	{
		return EClass._map.props.installed.FindEmptyContainer<TraitMailPost>();
	}

	public Point GetMailBoxPos()
	{
		Thing thing = GetMailBox();
		if (thing == null)
		{
			thing = EClass._map.props.installed.Find<TraitCoreZone>();
		}
		if (thing != null)
		{
			return thing.pos.GetNearestPoint(allowBlock: false, allowChara: true, allowInstalled: false, ignoreCenter: true).Clamp(useBounds: true);
		}
		return EClass._map.GetCenterPos();
	}

	public int GetResidentTax()
	{
		if (!policies.IsActive(2512, 30))
		{
			return 0;
		}
		int num = 0;
		int num2 = (policies.IsActive(2500, 30) ? Evalue(2500) : 0);
		int num3 = (policies.IsActive(2501, 30) ? Evalue(2501) : 0);
		int num4 = 50 + (int)Mathf.Sqrt(Evalue(2512)) * 5;
		foreach (Chara member in members)
		{
			if (member.IsPC || member.memberType != 0)
			{
				continue;
			}
			bool isWealthy = member.IsWealthy;
			int num5 = 0;
			foreach (Hobby item in member.ListWorks().Concat(member.ListHobbies()))
			{
				int num6 = item.source.tax * 100 / 100;
				if (num6 > num5)
				{
					num5 = num6;
				}
			}
			int num7 = ((isWealthy ? 50 : 10) + member.LV * 2) * num5 / 100 * num4 / 100;
			if (isWealthy && num2 > 0)
			{
				num7 = num7 * (150 + (int)Mathf.Sqrt(num2) * 5) / 100;
			}
			if (num3 > 0)
			{
				num7 += (80 + (int)Mathf.Sqrt(num3) * 5) * member.faith.source.tax / 100;
			}
			num7 = num7 * efficiency / (IsTaxFree ? 100 : 1000);
			num += num7;
			if (num7 > 0)
			{
				Log("bTax", num7.ToString() ?? "", member.Name);
			}
		}
		statistics.tax += num;
		return num;
	}

	public void CalcInnIncome()
	{
		int num = CountGuests();
		if (num == 0)
		{
			return;
		}
		int num2 = 0;
		int num3 = 0;
		foreach (Thing thing in EClass._map.things)
		{
			if (thing.IsInstalled && thing.trait is TraitBed traitBed && traitBed.owner.c_bedType == BedType.guest)
			{
				int maxHolders = traitBed.MaxHolders;
				num2 += maxHolders;
				num3 += traitBed.owner.LV * (100 + traitBed.owner.Quality / 2 + traitBed.owner.Evalue(750) / 2) / 100 * maxHolders;
			}
		}
		num = Mathf.Min(num, num2);
		if (num != 0)
		{
			num3 /= num2;
			num3 = num3 * (100 + 5 * (int)Mathf.Sqrt(Evalue(2812))) / 100;
			float num4 = 10f + Mathf.Sqrt(num) * 10f;
			if (policies.IsActive(2813))
			{
				num4 += Mathf.Sqrt(CountWealthyGuests()) * 50f * (80f + 5f * Mathf.Sqrt(Evalue(2813))) / 100f;
			}
			if (policies.IsActive(2812))
			{
				num4 = num4 * (float)(100 + num3) / 100f;
			}
			num4 = Mathf.Min(num4, Mathf.Sqrt(Worth) / 15f + 5f);
			incomeInn += EClass.rndHalf((int)num4) + EClass.rndHalf((int)num4);
		}
	}

	public void CalcTourismIncome()
	{
		int num = CountGuests();
		if (num != 0)
		{
			float num2 = 10f + Mathf.Sqrt(num) * 10f;
			if (policies.IsActive(2815))
			{
				num2 += Mathf.Sqrt(CountWealthyGuests()) * 50f * (80f + 5f * Mathf.Sqrt(Evalue(2815))) / 100f;
			}
			num2 = Mathf.Min(num2, Mathf.Sqrt(tourism) / 5f);
			incomeTourism += EClass.rndHalf((int)num2) + EClass.rndHalf((int)num2);
		}
	}

	public void GetDailyIncome()
	{
		GetIncome(ref incomeShop, ref statistics.shop, "getIncomeShop", tax: false);
		GetIncome(ref incomeInn, ref statistics.inn, "getIncomeInn", tax: true);
		GetIncome(ref incomeTourism, ref statistics.tourism, "getIncomeTourism", tax: true);
		void GetIncome(ref int n, ref int stat, string lang, bool tax)
		{
			if (tax && !IsTaxFree)
			{
				n = n * 10 / 100;
			}
			if (n != 0)
			{
				Msg.Say(lang, Lang._currency(n, "money"), owner.Name);
				Thing t = ThingGen.Create("money").SetNum(n);
				EClass.pc.Pick(t);
				stat += n;
				n = 0;
			}
		}
	}

	public Thing GetMeal(Chara c)
	{
		if (c.things.IsFull())
		{
			return null;
		}
		Thing thing = EClass._zone.TryGetThingFromSharedContainer((Thing t) => c.CanEat(t, shouldEat: true) && !t.c_isImportant);
		if (thing != null)
		{
			thing = thing.Split(1);
		}
		return thing;
	}

	public void OnClaimZone()
	{
		if (EClass.debug.allHomeSkill)
		{
			foreach (SourceElement.Row item in EClass.sources.elements.rows.Where((SourceElement.Row a) => a.category == "tech"))
			{
				elements.SetBase(item.id, 1);
			}
			foreach (SourceElement.Row item2 in EClass.sources.elements.rows.Where((SourceElement.Row a) => a.category == "policy" && !a.tag.Contains("hidden")))
			{
				policies.AddPolicy(item2.id);
			}
		}
		else
		{
			elements.SetBase(2003, 1);
			elements.SetBase(4002, 1);
			elements.SetBase(2115, 1);
			elements.SetBase(2204, 1);
			elements.SetBase(2120, 1);
			policies.AddPolicy(2512);
			policies.AddPolicy(2702);
			policies.AddPolicy(2703);
			policies.AddPolicy(2516);
			policies.AddPolicy(2515);
			policies.AddPolicy(2514);
		}
		if (EClass.pc.faction.CountTaxFreeLand() < 3)
		{
			policies.Activate(2514);
		}
		Element element = EClass._zone.ListLandFeats()[0];
		elements.SetBase(element.id, 1);
		switch (element.id)
		{
		case 3604:
			elements.SetBase(2206, 10);
			break;
		case 3602:
			elements.SetBase(2206, 15);
			break;
		}
		if (EClass.game.StartZone == owner || owner is Zone_Vernis)
		{
			AddMemeber(EClass.pc);
		}
		if (EClass.debug.allPolicy)
		{
			foreach (SourceElement.Row item3 in EClass.sources.elements.rows.Where((SourceElement.Row a) => a.category == "policy"))
			{
				policies.AddPolicy(item3.id);
			}
		}
		ValidateUpgradePolicies();
	}

	public void OnUnclaimZone()
	{
	}

	public void ValidateUpgradePolicies()
	{
		if (lv >= 2)
		{
			if (!policies.HasPolicy(2705))
			{
				policies.AddPolicy(2705);
				EClass.pc.faction.SetGlobalPolicyActive(2705, EClass.pc.faction.IsGlobalPolicyActive(2705));
			}
			if (!policies.HasPolicy(2708))
			{
				policies.AddPolicy(2708);
				EClass.pc.faction.SetGlobalPolicyActive(2708, EClass.pc.faction.IsGlobalPolicyActive(2708));
			}
			if (!policies.HasPolicy(2707))
			{
				policies.AddPolicy(2707);
			}
			if (!policies.HasPolicy(2709))
			{
				policies.AddPolicy(2709);
			}
		}
		foreach (int globalPolicy in EClass.pc.faction.globalPolicies)
		{
			if (!policies.HasPolicy(globalPolicy))
			{
				policies.AddPolicy(globalPolicy, show: false);
				EClass.pc.faction.SetGlobalPolicyActive(globalPolicy, EClass.pc.faction.IsGlobalPolicyActive(globalPolicy));
			}
		}
	}

	public void Upgrade()
	{
		List<Element> list = owner.ListLandFeats();
		if (owner.IsActiveZone)
		{
			TraitCoreZone traitCoreZone = EClass._map.FindThing<TraitCoreZone>();
			if (traitCoreZone != null)
			{
				SE.Play("godbless");
				traitCoreZone.owner.PlayEffect("aura_heaven");
			}
		}
		lv++;
		exp = 0;
		int admin = 4;
		int food = 4;
		Set(4, 3);
		switch (list[0].id)
		{
		case 3500:
			Set(5, 3);
			break;
		case 3604:
			Set(4, 3);
			break;
		case 3600:
			Set(5, 3);
			break;
		case 3601:
			Set(4, 4);
			break;
		case 3603:
			Set(6, 2);
			break;
		case 3602:
			Set(5, 2);
			break;
		}
		elements.SetBase(2003, Mathf.Min((lv - 1) * 2 + 1, 10));
		elements.SetBase(2115, (lv - 1) * admin + 1);
		elements.SetBase(2204, (lv - 1) * food + 1);
		elements.GetElement(2003).CheckLevelBonus(elements);
		ValidateUpgradePolicies();
		if (lv == 4)
		{
			elements.SetBase(list[1].id, 1);
		}
		if (lv == 7)
		{
			elements.SetBase(list[2].id, 1);
		}
		if (lv >= 5)
		{
			List<Element> list2 = elements.dict.Values.Where((Element a) => a.source.category == "landfeat" && a.HasTag("network")).ToList();
			foreach (Element item in list2)
			{
				EClass.pc.faction.elements.ModBase(item.id, item.Value);
			}
			foreach (Element item2 in list2)
			{
				elements.Remove(item2.id);
			}
		}
		Msg.Say("upgrade_hearth", lv.ToString() ?? "", owner.Name);
		LogRaw("upgrade_hearth".langGame(lv.ToString() ?? "", owner.Name), "Good");
		Tutorial.Reserve("stone");
		void Set(int a, int f)
		{
			admin = a;
			food = f;
		}
	}

	public bool CanUpgrade()
	{
		if (lv < MaxLv)
		{
			if (exp < GetNextExp())
			{
				return EClass.debug.enable;
			}
			return true;
		}
		return false;
	}

	public int GetUpgradeCost()
	{
		return lv * lv * lv * 1000;
	}

	public void ModExp(int a)
	{
		if (policies.IsActive(2515))
		{
			return;
		}
		if (policies.IsActive(2516))
		{
			a = a * 150 / 100 + EClass.rnd(2);
		}
		exp += a;
		if (exp >= GetNextExp() && CanUpgrade())
		{
			if (EClass.core.version.demo && lv >= 3)
			{
				exp = 0;
				Msg.Say("demoLimit2");
			}
			else
			{
				Upgrade();
			}
		}
	}

	public string GetHearthHint(int a)
	{
		if (a <= 1)
		{
			return "hearth1".lang();
		}
		string text = "";
		for (int i = 1; i < a; i++)
		{
			string text2 = ("hearth" + (i + 1)).lang();
			if (!text2.IsEmpty())
			{
				text = text + text2 + Environment.NewLine;
			}
		}
		return text.TrimEnd(Environment.NewLine.ToCharArray());
	}

	public void AddFeat(int ele, int v)
	{
		elements.ModBase(ele, v);
		WidgetPopText.Say("rewardElement".lang(EClass.sources.elements.map[ele].GetName()));
	}

	public void AddMemeber(Chara c)
	{
		if (!members.Contains(c))
		{
			EClass.Home.FindBranch(c)?.RemoveMemeber(c);
			EClass.Home.RemoveReserve(c);
			c.SetGlobal();
			c.SetFaction(EClass.Home);
			c.SetHomeZone(owner);
			if (c.OriginalHostility <= Hostility.Ally)
			{
				c.c_originalHostility = Hostility.Ally;
			}
			c.hostility = Hostility.Ally;
			c.enemy = null;
			c.orgPos = null;
			if (c.memberType != 0 && c.memberType != FactionMemberType.Livestock)
			{
				c.memberType = FactionMemberType.Default;
			}
			if (c.hp > c.MaxHP)
			{
				c.hp = c.MaxHP;
			}
			if (c.mana.value > c.mana.max)
			{
				c.mana.value = c.mana.max;
			}
			if (c.stamina.value > c.stamina.max)
			{
				c.stamina.value = c.stamina.max;
			}
			members.Add(c);
			EClass.pc.faction.charaElements.OnAddMemeber(c);
			RefreshEfficiency();
			c.RefreshWorkElements(elements);
			if (uidMaid == 0 && c.id == "maid")
			{
				uidMaid = c.uid;
			}
		}
	}

	public void ChangeMemberType(Chara c, FactionMemberType type)
	{
		c.ClearBed();
		c.memberType = type;
		RefreshEfficiency();
		c.RefreshWorkElements(elements);
		policies.Validate();
	}

	public void BanishMember(Chara c, bool sell = false)
	{
		RemoveMemeber(c);
		if (!sell)
		{
			Msg.Say("banish", c, EClass._zone.Name);
		}
		if (c.IsGlobal)
		{
			c.OnBanish();
		}
		else
		{
			c.Destroy();
		}
	}

	public void RemoveMemeber(Chara c)
	{
		c.isSale = false;
		if (c.currentZone != null && c.currentZone.map != null)
		{
			c.ClearBed(c.currentZone.map);
			c.currentZone.map.props.sales.Remove(c);
		}
		c.RefreshWorkElements();
		c.RemoveGlobal();
		members.Remove(c);
		EClass.pc.faction.charaElements.OnRemoveMember(c);
		c.SetFaction(EClass.game.factions.Wilds);
		policies.Validate();
		RefreshEfficiency();
	}

	public int GetHappiness(FactionMemberType type)
	{
		float num = 0f;
		if (members.Count == 0)
		{
			return 0;
		}
		foreach (Chara member in members)
		{
			num += (float)member.happiness;
		}
		return (int)(num / (float)members.Count);
	}

	public bool IsAllDead()
	{
		foreach (Chara member in members)
		{
			if (!member.isDead)
			{
				return false;
			}
		}
		return true;
	}

	public void Recruit(Chara c)
	{
		RemoveRecruit(c);
		AddMemeber(c);
		if (c.currentZone != EClass._zone && !c.isDead)
		{
			Point point = EClass._map.Installed.traits.GetTraitSet<TraitSpotExit>().GetRandom()?.GetPoint() ?? EClass.pc.pos;
			EClass._zone.AddCard(c, point);
		}
		RefreshEfficiency();
		c.RefreshWorkElements();
		Msg.Say("hire".langGame(c.Name));
	}

	public Chara GetMaid()
	{
		foreach (Chara member in members)
		{
			if (member.IsAliveInCurrentZone && member.IsMaid)
			{
				return member;
			}
		}
		return null;
	}

	public string GetRandomName()
	{
		if (EClass.rnd(4) == 0 || members.Count == 0)
		{
			return EClass.player.title;
		}
		return members.RandomItem().Name;
	}

	public int CountMembers(FactionMemberType type, bool onlyAlive = false)
	{
		int num = 0;
		foreach (Chara member in members)
		{
			if (member.memberType == type && member.trait.IsCountAsResident && (!onlyAlive || !member.isDead))
			{
				num++;
			}
		}
		return num;
	}

	public int CountGuests()
	{
		int num = 0;
		foreach (Chara chara in EClass._map.charas)
		{
			if (chara.memberType == FactionMemberType.Guest)
			{
				num++;
			}
		}
		return num;
	}

	public int CountWealthyGuests()
	{
		int num = 0;
		foreach (Chara chara in EClass._map.charas)
		{
			if (chara.memberType == FactionMemberType.Guest && chara.IsWealthy)
			{
				num++;
			}
		}
		return num;
	}

	public void UpdateReqruits(bool clear = false)
	{
		resources.Refresh();
		if (clear)
		{
			listRecruit.ForeachReverse(delegate(HireInfo i)
			{
				if (!i.chara.IsGlobal)
				{
					i.chara.Destroy();
				}
				if (i.chara.isDestroyed)
				{
					listRecruit.Remove(i);
				}
			});
			listRecruit.Clear();
			lastUpdateReqruit = -1;
		}
		else
		{
			listRecruit.ForeachReverse(delegate(HireInfo i)
			{
				if (i.Hours < 0)
				{
					if (!i.chara.IsGlobal)
					{
						i.chara.Destroy();
					}
					listRecruit.Remove(i);
				}
			});
		}
		if (lastUpdateReqruit == EClass.world.date.day)
		{
			return;
		}
		lastUpdateReqruit = EClass.world.date.day;
		int num = 2 + (int)Mathf.Sqrt(Evalue(2513)) / 2;
		int num2 = EClass.rnd(3 + (int)Mathf.Sqrt(Evalue(2513)) / 2) + num - listRecruit.Count;
		if (num2 <= 0)
		{
			return;
		}
		new List<Chara>(EClass.game.cards.globalCharas.Values).Shuffle();
		for (int j = 0; j < num2; j++)
		{
			Chara chara = CharaGen.CreateFromFilter("c_neutral", ContentLV + Mathf.Min(EClass.player.stats.days, 10));
			if (chara.isBackerContent)
			{
				j--;
			}
			else
			{
				AddRecruit(chara);
			}
		}
	}

	public void AddRecruit(Chara c)
	{
		HireInfo hireInfo = new HireInfo
		{
			chara = c,
			isNew = true
		};
		hireInfo.deadline = EClass.world.date.GetRaw() + (EClass.rnd(5) + 1) * 1440 + EClass.rnd(24) * 60;
		listRecruit.Add(hireInfo);
	}

	public void RemoveRecruit(Chara c)
	{
		listRecruit.ForeachReverse(delegate(HireInfo i)
		{
			if (i.chara == c)
			{
				listRecruit.Remove(i);
			}
		});
	}

	public int CountNewRecruits()
	{
		int num = 0;
		foreach (HireInfo item in listRecruit)
		{
			if (item.isNew)
			{
				num++;
			}
		}
		return num;
	}

	public void ClearNewRecruits()
	{
		foreach (HireInfo item in listRecruit)
		{
			item.isNew = false;
		}
	}

	public bool IsRecruit(Chara c)
	{
		foreach (HireInfo item in listRecruit)
		{
			if (item.chara == c)
			{
				return true;
			}
		}
		return false;
	}

	public int CountPasture()
	{
		return EClass._map.Stocked.GetNum("pasture", onlyShared: true) + EClass._map.Installed.GetNum("pasture");
	}

	public int GetPastureCost()
	{
		return CountMembers(FactionMemberType.Livestock);
	}

	public int GetCivility()
	{
		int num = 10 + Evalue(2203);
		if (Evalue(2701) > 0)
		{
			num += 10 + (int)Mathf.Sqrt(Evalue(2701));
		}
		if (Evalue(2702) > 0)
		{
			num -= 10 + (int)Mathf.Sqrt(Evalue(2702));
		}
		return num;
	}

	public float GetHearthIncome()
	{
		float num = 0f;
		foreach (Element value in elements.dict.Values)
		{
			if (value.source.category == "culture")
			{
				num += GetHearthIncome(value.source.alias);
			}
		}
		return num;
	}

	public float GetHearthIncome(string id)
	{
		int num = 0;
		foreach (KeyValuePair<int, Element> item in elements.dict)
		{
			if (item.Value.source.aliasParent == id)
			{
				num++;
			}
		}
		return 0.2f * (float)num;
	}

	public int GetTechUpgradeCost(Element e)
	{
		int valueWithoutLink = e.ValueWithoutLink;
		valueWithoutLink += e.CostLearn;
		if (e.source.max != 0 && e.ValueWithoutLink >= e.source.max)
		{
			return 0;
		}
		return valueWithoutLink;
	}

	public string Log(string idLang, string ref1 = null, string ref2 = null, string ref3 = null, string ref4 = null)
	{
		Msg.alwaysVisible = true;
		return LogRaw(Msg.GetRawText(idLang, ref1, ref2, ref3, ref4), EClass.sources.langGame.map[idLang].logColor);
	}

	public string Log(string idLang, Card c1, Card c2, string ref1 = null, string ref2 = null)
	{
		Msg.alwaysVisible = true;
		return LogRaw(Msg.GetRawText(idLang, c1, c2, ref1, ref2), EClass.sources.langGame.map[idLang].logColor);
	}

	public string Log(string idLang, Card c1, string ref1 = null, string ref2 = null, string ref3 = null)
	{
		Msg.alwaysVisible = true;
		return LogRaw(Msg.GetRawText(idLang, c1, ref1, ref2, ref3), EClass.sources.langGame.map[idLang].logColor);
	}

	public string LogRaw(string text, string col = null)
	{
		log.Add(text, col.IsEmpty() ? null : col);
		Msg.alwaysVisible = false;
		Msg.SetColor();
		return text;
	}
}
