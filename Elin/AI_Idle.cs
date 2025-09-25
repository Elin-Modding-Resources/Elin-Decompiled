using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AI_Idle : AIAct
{
	public enum Behaviour
	{
		Default,
		NoMove
	}

	public int maxRepeat = 10;

	public int moveFailCount;

	private static List<BaseArea> _listRoom = new List<BaseArea>();

	public override bool IsIdle => !base.IsChildRunning;

	public override bool InformCancel => false;

	public override int MaxRestart => maxRepeat;

	public override void OnStart()
	{
		owner.SetTempHand(-1, -1);
		owner.ShowEmo();
	}

	public override IEnumerable<Status> Run()
	{
		while (true)
		{
			if (owner.held != null)
			{
				owner.PickHeld();
			}
			if (owner.nextUse != null)
			{
				Thing nextUse = owner.nextUse;
				owner.nextUse = null;
				if (nextUse.parent == owner && !nextUse.isDestroyed)
				{
					owner.TryUse(nextUse);
				}
				yield return KeepRunning();
			}
			if (owner.pos.cell.HasFire)
			{
				if (EClass.rnd(5) == 0)
				{
					owner.Talk("onFire");
				}
				if (owner.MoveNeighborDefinitely())
				{
					yield return Restart();
				}
			}
			if (EClass.rnd(owner.IsPCParty ? 10 : 100) == 0 && owner.hunger.GetPhase() >= 3)
			{
				Thing thing = (owner.IsPCFaction ? owner.FindBestFoodToEat() : owner.things.Find((Thing a) => owner.CanEat(a, owner.IsPCFaction) && !a.c_isImportant, recursive: false));
				if (thing == null && owner.IsPCFaction && EClass._zone.IsPCFaction)
				{
					thing = EClass._zone.branch.GetMeal(owner);
					if (thing != null)
					{
						owner.Pick(thing);
					}
				}
				if (thing == null && !owner.IsPCParty)
				{
					if (!owner.IsPCFaction && EClass.rnd(8) != 0)
					{
						owner.hunger.Mod(-30);
					}
					else if (!owner.things.IsFull())
					{
						thing = ThingGen.CreateFromCategory("food", EClass.rnd(EClass.rnd(60) + 1) + 10);
						if (thing.trait.CanEat(owner))
						{
							thing.isNPCProperty = true;
							thing = owner.AddThing(thing);
						}
						else
						{
							thing = null;
						}
					}
				}
				if (thing != null)
				{
					if (EClass._zone.IsRegion)
					{
						owner.InstantEat(thing, sound: false);
						yield return Restart();
					}
					else if (thing.Num == 1 || !owner.things.IsFull())
					{
						yield return Do(new AI_Eat
						{
							target = thing
						});
					}
				}
			}
			if (!EClass._zone.IsRegion)
			{
				if (EClass.rnd(10) == 0 && owner.ability.Has(6627) && ((float)owner.hp < (float)owner.MaxHP * 0.8f || EClass.rnd(10) == 0) && owner.GetNearbyCatToSniff() != null && !owner.HasCondition<ConHOT>())
				{
					owner.Sniff(owner.GetNearbyCatToSniff());
					yield return KeepRunning();
				}
				if (EClass.rnd(3) == 0 && owner.mana.value > 0)
				{
					Act act = null;
					Act actRevive = null;
					foreach (ActList.Item item in owner.ability.list.items)
					{
						Act act2 = item.act;
						if (act2.id == 8430)
						{
							actRevive = act2;
						}
						string[] abilityType = act2.source.abilityType;
						if (!abilityType.IsEmpty() && (abilityType[0] == "heal" || abilityType[0] == "hot"))
						{
							act = item.act;
						}
					}
					if (act != null)
					{
						List<Chara> list = (owner.IsPCParty ? EClass.pc.party.members : new List<Chara> { owner });
						foreach (Chara item2 in list)
						{
							if (!((float)item2.hp > (float)item2.MaxHP * 0.75f) && owner.CanSeeLos(item2) && (!(act.source.abilityType[0] == "hot") || !item2.HasCondition<ConHOT>()))
							{
								owner.UseAbility(act, item2);
								yield return KeepRunning();
								break;
							}
						}
					}
					if (actRevive != null && owner.IsPCFaction && EClass.game.cards.globalCharas.Where((KeyValuePair<int, Chara> a) => a.Value.isDead && a.Value.faction == EClass.pc.faction && !a.Value.isSummon && a.Value.c_wasInPcParty).ToList().Count > 0 && owner.UseAbility(actRevive.source.alias, owner))
					{
						yield return KeepRunning();
					}
				}
			}
			if (owner.IsPCFaction && EClass._zone.IsPCFaction)
			{
				owner.sharedCheckTurn--;
				if (owner.sharedCheckTurn < 0 && EClass.rnd(EClass.debug.enable ? 2 : 20) == 0)
				{
					owner.TryTakeSharedItems();
					owner.TryPutSharedItems();
					owner.sharedCheckTurn += (EClass.debug.enable ? 20 : 200);
				}
			}
			if ((EClass._zone is Zone_Civilized || EClass._zone.IsPCFaction) && (owner.IsPCParty ? 10 : (owner.IsPCFaction ? 2 : 0)) > EClass.rnd(100))
			{
				Thing thing2 = owner.things.Find("polish_powder");
				if (thing2 != null && EClass._map.props.installed.Find<TraitGrindstone>() != null)
				{
					foreach (Thing thing8 in owner.things)
					{
						if (!thing8.IsEquipment || thing8.encLV >= 0)
						{
							continue;
						}
						for (int j = 0; j < 5; j++)
						{
							if (thing8.encLV >= 0)
							{
								break;
							}
							owner.Say("polish", owner, thing8);
							thing8.ModEncLv(1);
							thing2.ModNum(-1);
							if (thing2.isDestroyed)
							{
								break;
							}
						}
						if (thing2.isDestroyed)
						{
							break;
						}
					}
				}
			}
			if (owner.IsPCParty)
			{
				if (owner.IsRestrainedResident && owner.stamina.value > owner.stamina.max / 2)
				{
					TraitShackle traitShackle = owner.pos.FindThing<TraitShackle>();
					if (traitShackle != null && traitShackle.AllowTraining)
					{
						owner.SetAI(new AI_Torture
						{
							shackle = traitShackle
						});
					}
					yield return Restart();
				}
				if (EClass.rnd(20) == 0)
				{
					Thing thing3 = owner.things.Find((Thing a) => a.parent == owner && a.isGifted && (a.category.id == "skillbook" || a.category.id == "ancientbook"));
					if (thing3 != null && thing3.trait.CanRead(owner) && (thing3.Num == 1 || !owner.things.IsFull()))
					{
						yield return Do(new AI_Read
						{
							target = thing3
						});
					}
				}
				if (EClass.rnd(100) == 0 && !EClass._zone.IsRegion && owner.HasElement(1227))
				{
					List<Chara> list2 = new List<Chara>();
					foreach (Chara member in EClass.pc.party.members)
					{
						if (member.Evalue(1227) > 0)
						{
							list2.Add(member);
						}
					}
					if (list2.Count > 2)
					{
						list2.Remove(owner);
						owner.SetEnemy(list2.RandomItem());
						yield return Success();
					}
				}
				if (EClass.rnd(150) == 0 && owner.host != null && owner.host.parasite == owner && owner.GetInt(108) == 1)
				{
					owner.host.PlaySound("whip");
					owner.host.Say("use_whip3", owner, owner.host);
					owner.Talk("insult");
					owner.host.PlayAnime(AnimeID.Shiver);
					owner.host.DamageHP(5 + EClass.rndHalf(owner.host.MaxHP / 5), 919, 100, AttackSource.Condition);
					owner.host.OnInsulted();
					yield return KeepRunning();
				}
				if (EClass.rnd(EClass.debug.enable ? 2 : 20) == 0 && owner.CanSee(EClass.pc) && !(EClass.pc.ai is AI_Eat))
				{
					owner.TryTakeSharedItems(EClass.pc.things.List((Thing t) => t.IsSharedContainer));
				}
				if (owner.isSynced && EClass.rnd((owner.host == null) ? 200 : 150) == 0 && owner.GetInt(106) == 0)
				{
					if (EClass.rnd(2) == 0 && owner.GetInt(108) == 1)
					{
						owner.Talk("insult");
					}
					else
					{
						owner.TalkTopic();
					}
				}
				if (EClass.rnd(100) == 0 && EClass._zone.IsTown)
				{
					owner.ClearInventory(ClearInventoryType.SellAtTown);
				}
				if ((EClass.rnd(20) == 0 || EClass.debug.enable) && owner.GetCurrency() >= 500)
				{
					bool flag = EClass._zone.IsTown;
					if (EClass._zone.IsPCFaction)
					{
						foreach (Chara member2 in EClass._zone.branch.members)
						{
							if (member2.ExistsOnMap && member2.trait is TraitTrainer)
							{
								flag = true;
							}
						}
					}
					if (flag)
					{
						bool flag2 = false;
						foreach (Element value in owner.elements.dict.Values)
						{
							if (!(value.source.category != "skill") && value.vTempPotential < 900)
							{
								flag2 = true;
								break;
							}
						}
						if (flag2)
						{
							int num = owner.GetCurrency();
							if (num >= 20000)
							{
								num = 20000;
							}
							owner.PlaySound("pay");
							int num2 = num / 200;
							foreach (Element value2 in owner.elements.dict.Values)
							{
								if (!(value2.source.category != "skill"))
								{
									int num3 = num2 * 100 / (100 + (100 + value2.vTempPotential / 2 + value2.ValueWithoutLink) * (100 + value2.vTempPotential / 2 + value2.ValueWithoutLink) / 100);
									num3 += 1 + EClass.rnd(3);
									owner.elements.ModTempPotential(value2.id, Mathf.Max(1, num3), 9999);
								}
							}
							Msg.Say("party_train", owner, Lang._currency(num));
							owner.PlaySound("ding_potential");
							owner.ModCurrency(-num);
						}
					}
				}
			}
			if (owner.c_uidMaster != 0)
			{
				Chara chara = owner.master;
				if (chara == null || !chara.IsAliveInCurrentZone)
				{
					chara = owner.FindMaster();
				}
				if (chara != null && chara.IsAliveInCurrentZone)
				{
					if (owner.enemy == null)
					{
						owner.SetEnemy(chara.enemy);
					}
					int num4 = owner.Dist(chara.pos);
					if (owner.source.aiIdle != "root" && num4 > EClass.game.config.tactics.AllyDistance(owner) && EClass._zone.PetFollow && owner.c_minionType == MinionType.Default)
					{
						if (owner.HasAccess(chara.pos))
						{
							owner.TryMoveTowards(chara.pos);
						}
						yield return KeepRunning();
						continue;
					}
				}
			}
			Party party = owner.party;
			if (party == null || party.leader == owner || !party.leader.IsAliveInCurrentZone || owner.host != null || !EClass._zone.PetFollow)
			{
				break;
			}
			if (owner.source.aiIdle == "root")
			{
				yield return KeepRunning();
				continue;
			}
			if (owner.Dist(party.leader.pos) <= EClass.game.config.tactics.AllyDistance(owner))
			{
				yield return KeepRunning();
				continue;
			}
			if (owner.HasAccess(party.leader.pos) && owner.TryMoveTowards(party.leader.pos) == Card.MoveResult.Fail && owner.Dist(party.leader) > 4)
			{
				moveFailCount++;
				bool flag3 = (EClass._zone is Zone_Civilized || EClass._zone.IsPCFaction) && (EClass.pc.enemy == null || !EClass.pc.enemy.IsAliveInCurrentZone);
				if (moveFailCount >= (flag3 ? 100 : 10))
				{
					owner.Teleport(party.leader.pos.GetNearestPoint(allowBlock: false, allowChara: false, allowInstalled: true, ignoreCenter: true), silent: false, force: true);
					moveFailCount = 0;
				}
			}
			else
			{
				moveFailCount = 0;
			}
			yield return KeepRunning();
		}
		if (EClass._zone.IsNefia && EClass._zone.Boss == owner && EClass.rnd(20) == 0)
		{
			owner.SetEnemy(EClass.pc);
		}
		if (EClass._zone.IsRegion && EClass.rnd(10) != 0)
		{
			yield return Restart();
		}
		if (((owner.homeBranch != null && owner.homeBranch == EClass.Branch && EClass.rnd(100) == 0) || (owner.IsGuest() && EClass.rnd(50) == 0)) && owner.FindBed() == null)
		{
			owner.TryAssignBed();
		}
		string text;
		if (!EClass._zone.IsRegion)
		{
			text = owner.id;
			if (!(text == "geist"))
			{
				if (text == "mech_scarab" && EClass.rnd(20) == 0)
				{
					int i = 0;
					owner.pos.ForeachNeighbor(delegate(Point p)
					{
						if (p.HasChara && p.FirstChara.id == "mech_scarab")
						{
							i++;
						}
					});
					if (i <= 2)
					{
						EClass._zone.AddCard(owner.Duplicate(), owner.pos.GetNearestPoint(allowBlock: false, allowChara: false));
					}
				}
			}
			else if (EClass.rnd(20) == 0)
			{
				Point nearestPoint = EClass.pc.pos.GetNearestPoint(allowBlock: false, allowChara: false);
				if (nearestPoint != null)
				{
					foreach (Chara item3 in nearestPoint.ListCharasInRadius(owner, 6, (Chara _c) => _c != owner && !_c.IsPCFactionOrMinion))
					{
						item3.Teleport(nearestPoint.GetNearestPoint(allowBlock: false, allowChara: false) ?? nearestPoint);
					}
					if (owner != null)
					{
						if (!owner.IsPCFactionOrMinion)
						{
							EClass.pc.ai.Cancel();
						}
						owner.Teleport(nearestPoint);
					}
					yield return Success();
				}
			}
		}
		if (EClass._zone.IsPCFaction)
		{
			Room room = owner.pos.cell.room;
			if (room != null)
			{
				Point point = null;
				if (owner.memberType == FactionMemberType.Guest && room.data.accessType != 0)
				{
					point = FindMovePoint(BaseArea.AccessType.Public);
				}
				else if (owner.memberType == FactionMemberType.Default && room.data.accessType == BaseArea.AccessType.Private)
				{
					point = FindMovePoint(BaseArea.AccessType.Resident) ?? FindMovePoint(BaseArea.AccessType.Public);
				}
				if (point != null)
				{
					yield return DoGoto(point);
				}
			}
		}
		if (owner.isSynced && !owner.IsPCParty)
		{
			if (owner.IsPCFaction && owner.GetInt(32) + 4320 < EClass.world.date.GetRaw())
			{
				if (owner.GetInt(32) != 0 && Zone.okaerinko < 10)
				{
					owner.Talk("welcomeBack");
					Zone.okaerinko++;
				}
				owner.SetInt(32, EClass.world.date.GetRaw());
			}
			else if (EClass.player.stats.turns > owner.turnLastSeen + 50 && Los.IsVisible(EClass.pc, owner) && owner.CanSee(EClass.pc))
			{
				if (EClass.rnd(5) == 0 && owner.hostility >= Hostility.Neutral && EClass.pc.IsPCC && EClass.pc.pccData.state == PCCState.Undie && !EClass.pc.pos.cell.IsTopWaterAndNoSnow)
				{
					owner.Talk("pervert3");
				}
				else if (EClass.rnd(15) == 0 && EClass._zone.IsTown && owner.hostility >= Hostility.Neutral && !owner.IsPCFaction && !EClass.pc.HasCondition<ConIncognito>())
				{
					bool flag4 = EClass._zone is Zone_Derphy;
					string text2 = ((EClass.player.karma > 10) ? ((EClass.player.karma < 90) ? "" : (flag4 ? "rumor_bad" : "rumor_good")) : (flag4 ? "rumor_good" : "rumor_bad"));
					if (!text2.IsEmpty())
					{
						owner.Talk(text2);
					}
					if ((flag4 ? (EClass.player.karma >= 90) : (EClass.player.karma <= 10)) && EClass.rnd(10) == 0)
					{
						Thing t2 = ThingGen.Create("stone");
						AI_PlayMusic.ignoreDamage = true;
						ActThrow.Throw(owner, EClass.pc.pos, t2, ThrowMethod.Punish);
						AI_PlayMusic.ignoreDamage = false;
					}
				}
				else
				{
					owner.TalkTopic("fov");
				}
				owner.turnLastSeen = EClass.player.stats.turns;
			}
		}
		if (EClass.rnd(25) == 0 && owner.IsInMutterDistance())
		{
			if (owner.isRestrained)
			{
				owner.PlayAnime(AnimeID.Shiver);
			}
			TCText tC = owner.HostRenderer.GetTC<TCText>();
			if (tC == null || tC.pop.items.Count == 0)
			{
				if (owner.noMove)
				{
					foreach (Thing thing9 in owner.pos.Things)
					{
						if (thing9.IsInstalled && thing9.trait is TraitGeneratorWheel)
						{
							owner.Talk("labor");
							owner.PlayAnime(AnimeID.Shiver);
							yield return Restart();
						}
					}
				}
				if (owner.isDrunk && (owner.race.id == "cat" || owner.id == "sailor"))
				{
					owner.Talk("drunk_cat");
				}
				else if (owner.isRestrained)
				{
					owner.Talk("restrained");
				}
				else if (owner.GetInt(106) == 0 && !owner.IsPCParty)
				{
					if (owner.HasElement(1232) && EClass.rnd(4) == 0)
					{
						owner.Talk("baby");
					}
					else if (EClass.rnd((owner.host == null) ? 2 : 10) == 0 && owner.isSynced && owner.TalkTopic().IsEmpty())
					{
						owner.Talk(owner.pos.IsHotSpring ? "hotspring" : "idle");
					}
				}
			}
		}
		if (EClass.rnd(8) == 0 && owner.race.id == "chicken")
		{
			owner.PlaySound("Animal/Chicken/chicken");
		}
		if (EClass.rnd(80) == 0 && owner.race.id == "cat")
		{
			owner.PlaySound("Animal/Cat/cat");
		}
		if (owner.trait.IdAmbience != null && owner.IsInMutterDistance(15))
		{
			float mtp = 1f;
			Room room2 = owner.Cell.room;
			Room room3 = EClass.pc.Cell.room;
			if (room2 != room3 && room3 != null)
			{
				mtp = ((room2?.lot != room3?.lot) ? 0.4f : 0.7f);
			}
			EClass.Sound.PlayAmbience(owner.trait.IdAmbience, owner.pos.Position(), mtp);
		}
		if (EClass.rnd(2000) == 0 && owner.IsHuman && (owner.host == null || owner.host.ride != owner))
		{
			Thing thing4 = owner.things.Find((Thing a) => !a.IsNegativeGift && a.trait.CanDrink(owner), recursive: false);
			if (thing4 != null && thing4.trait is TraitPotion && owner.IsPCParty)
			{
				thing4 = null;
			}
			bool flag5 = EClass.Branch != null && EClass.Branch.policies.IsActive(2503);
			if (owner.homeBranch != null && owner.homeBranch.policies.IsActive(2503))
			{
				flag5 = true;
			}
			if (thing4 == null && !flag5)
			{
				thing4 = ThingGen.Create("crimAle");
				owner.Drink(thing4);
			}
			if (thing4 != null && !thing4.isDestroyed)
			{
				owner.TryUse(thing4);
				yield return Restart();
			}
		}
		if (EClass.rnd(owner.IsPCParty ? 1000 : 200) == 0 && owner.isDrunk && (owner.isSynced || EClass.rnd(5) == 0))
		{
			DoSomethingToCharaInRadius(3, null, delegate(Chara c)
			{
				owner.Say("drunk_mess", owner, c);
				owner.Talk("drunk_mess");
				bool flag6 = EClass.rnd(5) == 0 && !c.IsPC;
				if (c.IsPCParty && owner.hostility >= Hostility.Friend)
				{
					flag6 = false;
				}
				if (flag6)
				{
					owner.Say("drunk_counter", c, owner);
					c.Talk("drunk_counter");
					c.DoHostileAction(owner);
				}
			});
		}
		if (EClass.rnd(100) == 0 && owner.trait.CanFish && owner.stamina.value > 0)
		{
			Point fishingPoint = AI_Fish.GetFishingPoint(owner.pos);
			if (fishingPoint.IsValid)
			{
				yield return Do(new AI_Fish
				{
					pos = fishingPoint
				});
			}
		}
		string idAct = owner.source.actIdle.RandomItem();
		if (EClass.rnd(EClass.world.date.IsNight ? 1500 : 15000) == 0 && !owner.IsPCFaction && !owner.noMove)
		{
			owner.AddCondition<ConSleep>(1000 + EClass.rnd(1000), force: true);
		}
		if (EClass.rnd(100) == 0 && !owner.noMove && (owner.HasHobbyOrWork("Pet") || owner.HasHobbyOrWork("Fluffy")))
		{
			yield return Do(new AI_Mofu());
		}
		if (EClass.rnd((owner.host != null && owner.GetInt(106) != 0) ? 1000 : 40) == 0 && owner.IsHuman)
		{
			DoSomethingToNearChara((Chara c) => (!c.IsPCParty || EClass.rnd(5) == 0) && c.IsMofuable && !owner.IsHostile(c) && !c.IsInCombat, delegate(Chara c)
			{
				owner.Cuddle(c);
			});
			yield return KeepRunning();
		}
		if (EClass.rnd(100) == 0 && owner.trait is TraitBitch)
		{
			Chara chara2 = DoSomethingToNearChara((Chara c) => c.IsIdle && !c.IsPCParty && !(c.trait is TraitBitch) && c.Evalue(418) <= 0);
			if (chara2 != null)
			{
				yield return Do(new AI_Fuck
				{
					target = chara2,
					bitch = true
				});
			}
		}
		if (EClass.rnd(50) == 0 && owner.trait is TraitBard)
		{
			yield return Do(new AI_PlayMusic());
		}
		if (EClass.rnd(4) == 0 && TryPerformIdleUse())
		{
			yield return Restart();
		}
		if (EClass.rnd(20) == 0 && owner.trait.IdleAct())
		{
			yield return Restart();
		}
		if (idAct == "janitor" && EClass.rnd(5) == 0)
		{
			DoSomethingToCharaInRadius(4, null, delegate(Chara c)
			{
				if (c.HasElement(1211) && !(EClass._zone is Zone_Casino))
				{
					owner.Talk("snail");
					Thing t4 = ThingGen.Create("1142");
					ActThrow.Throw(owner, c.pos, t4);
				}
			});
			yield return Restart();
		}
		if (owner.IsRestrainedResident && owner.stamina.value > owner.stamina.max / 2)
		{
			TraitShackle traitShackle2 = owner.pos.FindThing<TraitShackle>();
			if (traitShackle2 != null && traitShackle2.AllowTraining)
			{
				owner.SetAI(new AI_Torture
				{
					shackle = traitShackle2
				});
				yield return Restart();
			}
		}
		if (!owner.IsPCFactionOrMinion && EClass.rnd(owner.isSynced ? 50 : 2000) == 0 && owner.hostility == Hostility.Neutral && EClass.pc.party.HasElement(1563) && !owner.race.tag.Contains("animal") && EClass._zone.IsTown && !EClass._zone.IsPCFaction && !owner.HasCondition<ConIncognito>())
		{
			EClass.pc.DoHostileAction(owner);
		}
		if (EClass.rnd(200) == 0 && !owner.noMove)
		{
			Point cleanPoint = AI_Clean.GetCleanPoint(owner, 4);
			if (cleanPoint != null)
			{
				yield return Do(new AI_Clean
				{
					pos = cleanPoint
				});
			}
		}
		if (EClass.rnd(35) == 0 && owner.id == "child" && owner.pos.cell.IsSnowTile)
		{
			foreach (Chara chara3 in EClass._map.charas)
			{
				if (EClass.rnd(3) != 0 && chara3 != owner && chara3.pos.cell.IsSnowTile && chara3.Dist(owner) <= 6 && Los.IsVisible(chara3, owner))
				{
					Thing t3 = ThingGen.Create("snow");
					ActThrow.Throw(owner, chara3.pos, t3);
					break;
				}
			}
		}
		if (EClass.rnd(EClass.debug.enable ? 3 : 30) == 0)
		{
			Thing thing5 = owner.things.Find<TraitBall>();
			if (thing5 == null)
			{
				owner.pos.ForeachNeighbor(delegate(Point p)
				{
					Card card2 = p.FindThing<TraitBall>()?.owner;
					if (card2 != null)
					{
						owner.Pick(card2.Thing);
					}
				});
			}
			else
			{
				foreach (Chara chara4 in EClass._map.charas)
				{
					if (EClass.rnd(3) != 0 && chara4 != owner && chara4.Dist(owner) <= 6 && chara4.Dist(owner) >= 3 && Los.IsVisible(chara4, owner))
					{
						ActThrow.Throw(owner, chara4.pos, thing5);
						break;
					}
				}
			}
		}
		if (EClass.rnd(20) == 0 && AI_Shopping.TryShop(owner, realtime: true))
		{
			yield return Restart();
		}
		if (EClass.rnd(20) == 0 && owner.IsPCFaction && AI_Shopping.TryRestock(owner, realtime: true))
		{
			yield return Restart();
		}
		owner.idleActTimer--;
		if (owner.idleActTimer <= 0 && !owner.source.actIdle.IsEmpty())
		{
			owner.idleActTimer = 10 + EClass.rnd(50);
			switch (idAct)
			{
			case "torture_snail":
				DoSomethingToNearChara((Chara c) => c.race.id == "snail", delegate(Chara c)
				{
					owner.Say("use_whip3", owner, c);
					owner.PlaySound("whip");
					owner.Talk("insult");
					c.PlayAnime(AnimeID.Shiver);
					c.OnInsulted();
				});
				break;
			case "buffMage":
				if (EClass.rnd(2) == 0)
				{
					TryCast<ConHolyVeil>(EffectId.HolyVeil, 300 + EClass.rnd(300));
				}
				else
				{
					TryCast<ConLevitate>(EffectId.Levitate, 300 + EClass.rnd(300));
				}
				break;
			case "buffThief":
				TryCast<ConNightVision>(EffectId.CatsEye, 100 + EClass.rnd(100));
				break;
			case "buffGuildWatch":
				TryCast<ConGravity>(EffectId.Gravity, 300 + EClass.rnd(300));
				break;
			case "buffHealer":
				TryCast(EffectId.Heal);
				break;
			case "readBook":
			{
				if (EClass.rnd(2) == 0 || (owner.IsPCParty && EClass.rnd(20) != 0))
				{
					break;
				}
				List<Thing> list3 = owner.things.List((Thing a) => a.parent == owner && (a.category.id == "spellbook" || a.category.id == "ancientbook" || a.category.id == "skillbook"), onlyAccessible: true);
				Thing thing6 = null;
				if (list3.Count > 0)
				{
					thing6 = list3.RandomItem();
					if (!thing6.trait.CanRead(owner))
					{
						thing6 = null;
					}
				}
				if (thing6 == null)
				{
					if (owner.things.IsFull())
					{
						break;
					}
					thing6 = ThingGen.CreateFromCategory((EClass.rnd(5) != 0) ? "spellbook" : "ancientbook");
					thing6.isNPCProperty = true;
				}
				if (!(thing6.id == "1084") || !owner.IsPCFaction)
				{
					if (!owner.HasElement(285))
					{
						owner.elements.ModBase(285, 1);
					}
					yield return Do(new AI_Read
					{
						target = thing6
					});
				}
				break;
			}
			default:
				if (LangGame.Has("idle_" + idAct))
				{
					IdleActText(idAct);
				}
				break;
			}
			yield return Restart();
		}
		if (owner.host != null)
		{
			yield return Restart();
		}
		if (owner.HasEditorTag(EditorTag.AINoMove) || owner.trait.IdleBehaviour == Behaviour.NoMove || owner.noMove)
		{
			if (owner.orgPos != null && !owner.pos.Equals(owner.orgPos) && !owner.orgPos.IsBlocked && !owner.orgPos.HasChara)
			{
				yield return DoGoto(owner.orgPos);
			}
			yield return Restart();
		}
		if (owner.HasEditorTag(EditorTag.AIFollow) && owner.pos.Distance(EClass.pc.GetDestination()) > 1)
		{
			yield return DoGoto(EClass.pc);
		}
		if (EClass.rnd(100) == 0 && !owner.IsPCFaction)
		{
			if (owner.id == "ashland" || owner.id == "fiama")
			{
				Card card = EClass._map.Installed.traits.restSpots.RandomItem();
				if (card != null)
				{
					yield return DoGotoSpot(card);
				}
				else
				{
					Room room4 = owner.FindRoom();
					if (room4 != null)
					{
						yield return DoGoto(room4.GetRandomPoint().GetNearestPoint());
					}
				}
			}
			else if (owner.orgPos != null && !owner.pos.Equals(owner.orgPos) && !owner.orgPos.IsBlocked && !owner.orgPos.HasChara && owner.orgPos.IsInBounds)
			{
				yield return DoGoto(owner.orgPos, 0, ignoreConnection: false, delegate
				{
					if (!EClass._zone.IsPCFaction)
					{
						owner.Teleport(owner.orgPos, silent: false, force: true);
					}
					return Status.Success;
				});
			}
		}
		if (EClass.rnd(100) == 0 && owner.id == "bee")
		{
			Thing thing7 = EClass._map.ListThing<TraitBeekeep>()?.RandomItem();
			if (thing7 != null)
			{
				yield return DoGoto(thing7.pos);
			}
		}
		if (EClass.rnd(10) == 0 && !EClass._zone.IsUnderwater && (owner.race.tag.Contains("water") || owner.source.tag.Contains("water")) && !owner.pos.IsDeepWater)
		{
			for (int k = 0; k < 100; k++)
			{
				Point randomPoint = EClass._map.GetRandomPoint();
				if (randomPoint.IsDeepWater && !randomPoint.IsBlocked)
				{
					yield return DoGoto(randomPoint);
					break;
				}
			}
		}
		text = owner.source.aiIdle;
		if (!(text == "stand") && !(text == "root"))
		{
			if (EClass.rnd(15) == 0)
			{
				owner.MoveRandom();
			}
			if (owner == null)
			{
				yield return Cancel();
			}
		}
		if (EClass._zone.IsPCFaction && owner.IsPCFaction && !owner.IsPCParty && (owner.GetWork("Clean") != null || owner.GetWork("Chore") != null) && !(EClass.pc.ai is AI_UseCrafter))
		{
			AI_Haul aI_Haul = AI_Haul.TryGetAI(owner);
			if (aI_Haul != null)
			{
				yield return Do(aI_Haul);
			}
		}
		yield return Restart();
		Point FindMovePoint(BaseArea.AccessType type)
		{
			for (int l = 0; l < 20; l++)
			{
				Point randomPoint2 = owner.pos.GetRandomPoint(5 + l, requireLos: false);
				if (randomPoint2 != null && randomPoint2.IsInBounds && (randomPoint2.cell.room == null || randomPoint2.cell.room.data.accessType == type))
				{
					return randomPoint2;
				}
			}
			return null;
		}
	}

	public void IdleActText(string id)
	{
		string text = "idle_" + id;
		owner.PlaySound(text);
		if (Lang.Game.map.ContainsKey(text))
		{
			owner.Say(text, owner);
		}
	}

	public void TryCast<T>(EffectId id, int power = 100) where T : Condition
	{
		if (!owner.HasCondition<T>())
		{
			TryCast(id, power);
		}
	}

	public void TryCast(EffectId id, int power = 100)
	{
		owner.Say("idle_cast", owner);
		ActEffect.Proc(id, power, BlessedState.Normal, owner);
	}

	public BaseArea GetRandomAssignedRoom()
	{
		_listRoom.Clear();
		foreach (BaseArea item in ((IEnumerable<BaseArea>)EClass._map.rooms.listRoom).Concat((IEnumerable<BaseArea>)EClass._map.rooms.listArea))
		{
			if (item.type != null && item.type.uidCharas.Contains(owner.uid))
			{
				_listRoom.Add(item);
			}
		}
		return _listRoom.RandomItem();
	}

	public Chara DoSomethingToNearChara(Func<Chara, bool> funcPickChara, Action<Chara> action = null)
	{
		List<Chara> list = owner.pos.ListCharasInNeighbor(delegate(Chara c)
		{
			if (c == owner || !owner.CanSee(c))
			{
				return false;
			}
			return funcPickChara == null || funcPickChara(c);
		});
		if (list.Count > 0)
		{
			Chara chara = list.RandomItem();
			action?.Invoke(chara);
			return chara;
		}
		return null;
	}

	public Chara DoSomethingToCharaInRadius(int radius, Func<Chara, bool> funcPickChara, Action<Chara> action = null)
	{
		List<Chara> list = owner.pos.ListCharasInRadius(owner, radius, delegate(Chara c)
		{
			if (c == owner || !owner.CanSee(c))
			{
				return false;
			}
			return funcPickChara == null || funcPickChara(c);
		});
		if (list.Count > 0)
		{
			Chara chara = list.RandomItem();
			action?.Invoke(chara);
			return chara;
		}
		return null;
	}

	public bool TryPerformIdleUse()
	{
		for (int i = 0; i < 10; i++)
		{
			Point randomPoint = owner.pos.GetRandomPoint(7, requireLos: true, allowChara: true, allowBlocked: true);
			if (randomPoint == null || randomPoint.detail == null)
			{
				continue;
			}
			foreach (Thing thing in randomPoint.detail.things)
			{
				if (thing.IsInstalled)
				{
					int num = owner.Dist(thing);
					if (EClass.rnd((owner.memberType == FactionMemberType.Guest) ? 5 : 50) == 0 && thing.HasTag(CTAG.tourism) && num <= 2)
					{
						owner.LookAt(thing);
						owner.Talk("nice_statue");
						return true;
					}
					if (EClass.rnd(thing.trait.IdleUseChance) == 0 && thing.trait.IdleUse(owner, num))
					{
						owner.LookAt(thing);
						return true;
					}
				}
			}
		}
		return false;
	}
}
