using System;
using System.Collections.Generic;
using UnityEngine;

public class GoalCombat : Goal
{
	public class ItemAbility
	{
		public Act act;

		public int priority;

		public int priorityMod;

		public int chance;

		public Chara tg;

		public bool pt;

		public bool aiPt;
	}

	public Chara destEnemy;

	public Chara tc;

	public int idleCount;

	public int moveFail;

	public List<ItemAbility> abilities;

	public List<Chara> charas = new List<Chara>();

	public override CursorInfo CursorIcon => CursorSystem.IconMelee;

	public override bool CancelWhenDamaged => false;

	public override bool CancelOnAggro => false;

	public Tactics tactics => owner.tactics;

	public override bool CanManualCancel()
	{
		return true;
	}

	public override IEnumerable<Status> Run()
	{
		if (destEnemy != null)
		{
			owner.enemy = destEnemy;
			destEnemy = null;
		}
		int count = 0;
		int lostCount = 0;
		bool dontWander = owner.IsPCParty && !owner.IsPC && EClass.game.config.tactics.dontWander;
		while (true)
		{
			bool canSeeLos = false;
			if (EClass.debug.logCombat)
			{
				Debug.Log("■" + owner.Name + "/" + count + "/" + lostCount);
			}
			if (EClass._zone.isPeace && owner.IsPCFactionOrMinion && !owner.IsPC)
			{
				owner.enemy = null;
				owner.ShowEmo(Emo.happy);
				yield return Success();
			}
			count++;
			if (dontWander && owner.enemy != null && !EClass.pc.isBlind && !EClass.pc.CanSeeLos(owner.enemy) && (owner.Dist(EClass.pc) > 4 || owner.Dist(owner.enemy) > 1))
			{
				Point firstStep = owner.GetFirstStep(EClass.pc.pos, PathManager.MoveType.Combat);
				if (firstStep.IsValid && !firstStep.HasChara)
				{
					owner.enemy = null;
				}
			}
			tc = owner.enemy;
			if (tc != null && owner.IsPCFaction)
			{
				if (EClass.pc.isHidden && owner.isHidden)
				{
					owner.enemy = null;
					yield return Success();
				}
				if (!EClass.pc.ai.ShouldAllyAttack(tc))
				{
					tc = (owner.enemy = null);
				}
			}
			if (tc == null || tc.isDead || !tc.ExistsOnMap || !tc.pos.IsInBounds || lostCount >= (owner.IsPowerful ? 50 : 5) || !owner.CanSee(tc))
			{
				tc = (owner.enemy = null);
				if (owner.IsPC && EClass.game.config.autoCombat.abortOnKill)
				{
					yield return Success();
				}
				owner.FindNewEnemy();
				lostCount = 0;
				if (owner.enemy == null)
				{
					yield return Success();
				}
				tc = owner.enemy;
			}
			else
			{
				canSeeLos = owner.CanSeeLos(tc);
				lostCount = ((!canSeeLos) ? (lostCount + 1) : 0);
			}
			if (owner.IsPC && tc.HasEditorTag(EditorTag.Invulnerable))
			{
				Msg.Say("abort_idle");
				yield return Success();
			}
			if (tc.IsPCFaction && owner.id == "melilith_boss" && EClass._map.plDay.list.Count > 1 && EClass._map.plDay.list[0].data.id != 107)
			{
				EClass._zone.SetBGM(107);
			}
			if (abilities == null)
			{
				abilities = new List<ItemAbility>();
				BuildAbilityList();
			}
			if (owner.IsPCFaction && tc.IsPCFaction && EClass.rnd(5) == 0 && count > 2)
			{
				CalmDown();
				yield return Success();
			}
			if (owner.OriginalHostility >= Hostility.Neutral && tc.OriginalHostility >= Hostility.Neutral && !owner.IsPCParty && owner.c_bossType == BossType.none && (!(owner.trait is TraitGuard) || owner.IsPCFaction || !tc.IsPCParty || !EClass.player.IsCriminal))
			{
				if (owner.calmCheckTurn < 0 || (!owner.enemy.IsPCParty && EClass.rnd(10) == 0))
				{
					CalmDown();
					yield return Success();
				}
				owner.calmCheckTurn--;
			}
			if (owner.IsPC)
			{
				CursorSystem.ignoreCount = 1;
			}
			if (tc.host != null && (tc.hp == 0 || EClass.rnd(5) == 0))
			{
				tc = owner.SetEnemy(tc.host);
			}
			if (tc.parasite != null && !tc.isRestrained && tc.parasite.hp > 0 && EClass.rnd(5) == 0)
			{
				tc = owner.SetEnemy(tc.parasite);
			}
			if (tc.ride != null && !tc.isRestrained && tc.ride.hp > 0 && EClass.rnd(5) == 0)
			{
				tc = owner.SetEnemy(tc.ride);
			}
			if (tc.enemy != null)
			{
				tc.TrySetEnemy(owner);
			}
			if (!tc.IsMinion && EClass.rnd(10) == 0 && EClass.rnd(tc.DEX + 10) > owner.LV && tc.HasElement(1315) && !owner.HasElement(1315) && owner.race.IsMachine && owner.CanBeTempAlly(tc))
			{
				owner.Say("dominate_machine", tc, owner);
				owner.PlayEffect("boost");
				owner.PlaySound("boost");
				owner.ShowEmo(Emo.love);
				owner.lastEmo = Emo.angry;
				owner.MakeMinion(tc.IsPCParty ? EClass.pc : tc);
				yield return Success();
			}
			if (EClass.rnd(5) == 0 && tc.HasElement(1325) && owner.race.IsPlant && owner.CanBeTempAlly(tc))
			{
				owner.Say("dominate_plant", tc, owner);
				owner.ShowEmo(Emo.love);
				owner.lastEmo = Emo.angry;
				owner.MakeMinion((tc.IsPCParty || tc.IsPCFactionMinion) ? EClass.pc : tc, MinionType.Friend);
				yield return Success();
			}
			if (EClass.rnd(20) == 0 && owner.isRestrained)
			{
				owner.Talk("restrained");
			}
			if (this is GoalAutoCombat)
			{
				ActionMode.Adv.SetTurbo(EClass.game.config.autoCombat.turbo ? (-1) : 0);
				EClass.pc.ModExp(135, 20);
			}
			int dist = owner.Dist(tc);
			bool move = owner.host == null && (tactics.ChanceMove > EClass.rnd(100) || (owner.IsPC && tc.HasCondition<ConFear>() && dist >= EClass.pc.GetSightRadius() - 1));
			bool haltSecondMove = false;
			if (!owner.IsPC && owner.IsNeutralOrAbove() && !owner.isBlind && !owner.isSummon && !owner.IsMinion)
			{
				int num = -1;
				if (tc.HasElement(1221))
				{
					num = 1;
				}
				if (tc.source.HasTag(CTAG.suicide) && !tc.HasCondition<ConWet>())
				{
					num = 3;
				}
				if (num > 0)
				{
					if (dist <= num)
					{
						if (EClass.rnd(15) == 0)
						{
							owner.Talk("run_suicide");
						}
						if (owner.host == null && owner.TryMoveFrom(tc.pos) != 0)
						{
							yield return Status.Running;
							idleCount = 0;
							continue;
						}
						if (EClass.debug.logCombat)
						{
							Debug.Log("Failed to Run: " + owner.Name);
						}
					}
					if (dist == num + 1)
					{
						haltSecondMove = true;
						move = false;
						idleCount = 0;
					}
				}
			}
			if (dontWander)
			{
				int num2 = owner.Dist(EClass.pc);
				if (num2 > 3)
				{
					int x = tc.pos.x;
					int z = tc.pos.z;
					if (EClass.pc.pos.Distance(owner.pos.x + ((x > owner.pos.x) ? 1 : ((x < owner.pos.x) ? (-1) : 0)), owner.pos.z + ((z > owner.pos.z) ? 1 : ((z < owner.pos.z) ? (-1) : 0))) >= num2)
					{
						move = false;
						haltSecondMove = true;
					}
				}
			}
			if ((owner.IsPC && EClass.game.config.autoCombat.bDontChase) || (!canSeeLos && tc.isHidden))
			{
				move = false;
				haltSecondMove = true;
			}
			if (EClass.debug.logCombat)
			{
				Debug.Log(owner.Name + "/" + move + "/" + haltSecondMove + "/" + dist);
			}
			if (move)
			{
				if (owner.IsPC && dist <= owner.GetSightRadius() && TryUseAbility(dist, beforeMove: true))
				{
					yield return Status.Running;
					idleCount = 0;
					continue;
				}
				if (TryMove(dist))
				{
					if (EClass.debug.logCombat)
					{
						Debug.Log("moved:" + owner.Name);
					}
					yield return Status.Running;
					idleCount = 0;
					continue;
				}
			}
			if (owner == null)
			{
				yield return Cancel();
			}
			if (dist <= owner.GetSightRadius() && TryUseAbility(dist))
			{
				yield return Status.Running;
				idleCount = 0;
				continue;
			}
			if (EClass.debug.logCombat)
			{
				Debug.Log(owner.Name + "/" + move + "/" + haltSecondMove + "/" + tactics.ChanceSecondMove);
			}
			if (!move && !haltSecondMove && tactics.ChanceSecondMove > EClass.rnd(100) && TryMove(dist))
			{
				yield return Status.Running;
				idleCount = 0;
				continue;
			}
			if (owner == null)
			{
				yield return Cancel();
			}
			idleCount++;
			if (TryAbortCombat())
			{
				yield return Success();
			}
			if (idleCount > 2)
			{
				if (dontWander)
				{
					yield return Success();
				}
				idleCount = 0;
				string aiIdle = owner.source.aiIdle;
				if (aiIdle == "stand" || aiIdle == "root")
				{
					yield return Success();
				}
				yield return DoGoto(tc.pos);
			}
			else if (owner.FindNearestNewEnemy())
			{
				yield return Status.Running;
				continue;
			}
			yield return Status.Running;
		}
		void CalmDown()
		{
			owner.enemy = null;
			if (owner.ride != null)
			{
				owner.ride.enemy = null;
			}
			if (owner.parasite != null)
			{
				owner.parasite.enemy = null;
			}
			owner.hostility = owner.OriginalHostility;
			if (tc.enemy == owner)
			{
				tc.enemy = null;
				if (tc.ride != null)
				{
					tc.ride.enemy = null;
				}
				if (tc.parasite != null)
				{
					tc.parasite.enemy = null;
				}
				tc.hostility = tc.OriginalHostility;
			}
			owner.Say("calmDown", owner);
		}
	}

	public bool TryMove(int dist)
	{
		if (EClass.debug.logCombat)
		{
			Debug.Log("TryMove: " + owner.Name + "/" + dist);
		}
		if (owner.host != null)
		{
			return false;
		}
		if (owner.isBlind)
		{
			return owner.MoveRandom();
		}
		int num = (tc.HasCondition<ConFear>() ? 1 : tactics.DestDist);
		if (!owner.IsPC && (tactics.source.id == "archer" || tactics.source.id == "gunner") && !owner.TryEquipRanged())
		{
			num = 1;
		}
		if (!owner.IsPC && num > 1)
		{
			if (tactics.DestDist == 2)
			{
				if (EClass.rnd(5) == 0)
				{
					num = 1;
				}
			}
			else if (owner.turn / 3 % 5 > 2)
			{
				num--;
			}
		}
		bool flag = false;
		if (dist > num)
		{
			flag = owner.TryMoveTowards(tc.pos) != Card.MoveResult.Fail;
			if (!flag)
			{
				moveFail++;
			}
		}
		else if (dist < num)
		{
			flag = owner.TryMoveFrom(tc.pos) != Card.MoveResult.Fail;
		}
		if (flag)
		{
			moveFail = 0;
		}
		if (EClass.debug.logCombat)
		{
			Debug.Log("TryMove:" + owner.Name + "/" + flag + "/" + dist + "/" + num);
		}
		return flag;
	}

	public void AddAbility(Act a, int mod = 0, int chance = 100, bool aiPt = false)
	{
		abilities.Add(new ItemAbility
		{
			act = a,
			priorityMod = mod,
			chance = chance,
			aiPt = aiPt
		});
	}

	public void TryAddAbility(int ele)
	{
		if (abilities == null)
		{
			BuildAbilityList();
		}
		foreach (ItemAbility ability in abilities)
		{
			Act obj = ability.act;
			if (obj != null && obj.id == ele)
			{
				return;
			}
		}
		AddAbility(Element.Create(ele) as Act);
	}

	public void TryRemoveAbility(int ele)
	{
		if (abilities == null)
		{
			return;
		}
		foreach (ItemAbility ability in abilities)
		{
			Act obj = ability.act;
			if (obj != null && obj.id == ele)
			{
				abilities.Remove(ability);
				break;
			}
		}
	}

	public virtual bool TryUseRanged(int dist)
	{
		if (owner.TryEquipRanged())
		{
			return ACT.Ranged.Perform(owner, tc);
		}
		return false;
	}

	public virtual bool TryThrow(int dist)
	{
		if (dist > owner.GetSightRadius())
		{
			return false;
		}
		Thing thing = owner.TryGetThrowable();
		if (thing == null)
		{
			return false;
		}
		if (!ACT.Throw.CanPerform(owner, tc, tc.pos))
		{
			return false;
		}
		ActThrow.Throw(owner, tc.pos, tc, thing.HasElement(410) ? thing : thing.Split(1));
		return true;
	}

	public virtual bool TryUseAbility(int dist, bool beforeMove = false)
	{
		if (abilities.Count == 0)
		{
			Debug.Log("no ability:" + owner);
			return false;
		}
		int numEnemy = -1;
		int numFriend = -1;
		int numNeutral = -1;
		bool charaBuilt = false;
		bool flag = owner.CanSeeLos(tc, dist);
		bool isPCFaction = owner.IsPCFaction;
		bool flag2 = owner.HasCondition<ConSilence>();
		bool isBlind = owner.isBlind;
		bool flag3 = owner.HasCondition<ConFear>();
		bool isConfused = owner.isConfused;
		bool flag4 = owner.HasCondition<ConDim>();
		bool flag5 = owner.HasCondition<ConSupress>();
		foreach (ItemAbility ability in abilities)
		{
			Act act = ability.act;
			ability.priority = 0;
			ability.tg = null;
			ability.pt = false;
			if (EClass.rnd(100) >= ability.chance || (isBlind && ability.act.HasTag("reqSight")))
			{
				continue;
			}
			int num = 0;
			SourceElement.Row s = act.source;
			if (s.abilityType.Length == 0 || (owner.IsPC && flag2 && act is Spell) || (beforeMove && !act.HasTag("before_move")))
			{
				continue;
			}
			string text = s.abilityType[0];
			if (flag5 && !(text == "melee") && !(text == "range") && EClass.rnd(2) == 0)
			{
				continue;
			}
			switch (act.id)
			{
			case 6603:
				if (!tc.IsPCParty || tc.Evalue(418) < 0)
				{
					break;
				}
				foreach (Chara member in EClass.pc.party.members)
				{
					if (member.Evalue(418) < 0)
					{
						owner.enemy = (tc = member);
						return false;
					}
				}
				break;
			case 6602:
				if (dist <= 1 || tc.HasCondition<ConEntangle>())
				{
					continue;
				}
				break;
			case 6450:
				if (isPCFaction && (tc.HasElement(1221) || tc.HasElement(1223) || tc.id == "hedgehog_ether"))
				{
					continue;
				}
				break;
			case 8200:
			case 8201:
				if (owner.HasElement(400))
				{
					continue;
				}
				break;
			case 6400:
				if (isPCFaction)
				{
					continue;
				}
				break;
			case 8790:
			case 8791:
				if (tc.host != null)
				{
					continue;
				}
				break;
			}
			bool isHOT;
			switch (text)
			{
			case "item":
				num = (ability.act as ActItem).BuildAct(owner);
				break;
			case "wait":
				if (owner.IsPCParty)
				{
					continue;
				}
				num = 50;
				break;
			case "taunt":
			{
				bool flag7 = owner.HasCondition<StanceTaunt>();
				bool flag8 = tactics.source.taunt != -1 && 100 * owner.hp / owner.MaxHP >= tactics.source.taunt;
				num = ((flag7 && !flag8) ? 100 : ((!flag7 && flag8) ? 100 : 0));
				break;
			}
			case "melee":
				if (dist > owner.body.GetMeleeDistance())
				{
					continue;
				}
				num = ((!flag3) ? tactics.P_Melee : ((!owner.IsPC) ? (tactics.P_Melee / 2) : 0));
				if (isConfused)
				{
					num -= (owner.IsPC ? 30 : 10);
				}
				if (isBlind)
				{
					num -= (owner.IsPC ? 50 : 10);
				}
				if (dist <= 1)
				{
					if (tc.HasElement(1221))
					{
						num -= 40;
					}
					if (tc.HasElement(1223))
					{
						num -= 40;
					}
					if (!owner.IsPC && owner.IsPCFaction && tc.id == "hedgehog_ether")
					{
						continue;
					}
					if (!owner.IsPCFaction && num < 10)
					{
						num = 10;
					}
				}
				break;
			case "range":
				if (!flag || EClass.rnd(100) > tactics.RangedChance)
				{
					continue;
				}
				num = ((!flag3) ? tactics.P_Range : ((!owner.IsPC) ? (tactics.P_Range / 2) : 0));
				if (isConfused)
				{
					num -= (owner.IsPC ? 30 : 10);
				}
				if (isBlind)
				{
					num -= (owner.IsPC ? 50 : 10);
				}
				if (owner.ranged != null && owner.ranged.trait is TraitToolRangeCane && owner.mana.value <= 0)
				{
					continue;
				}
				break;
			case "teleport":
				num = 40;
				break;
			case "any":
				num = 50;
				break;
			case "hot":
			case "heal":
				isHOT = text == "hot";
				num = ForeachChara(ability, (Chara c) => HealFactor(c), isFriendlyAbility: true);
				if (ability.aiPt || (owner.IsPC && tactics.CastPartyBuff))
				{
					ability.pt = true;
				}
				break;
			case "dot":
			case "attack":
			case "attackMelee":
			{
				if (!flag)
				{
					continue;
				}
				bool flag6 = text == "dot";
				if (flag6 && (owner.isRestrained || (tc != null && tc.IsRestrainedResident)))
				{
					continue;
				}
				num = ((text == "attackMelee") ? tactics.P_Melee : tactics.P_Spell) + GetAttackMod(act);
				if (num > 0 && flag6)
				{
					num += 10;
				}
				if (ability.aiPt)
				{
					ability.pt = true;
				}
				break;
			}
			case "attackArea":
			{
				if (owner.isRestrained || (tc != null && tc.IsRestrainedResident))
				{
					continue;
				}
				bool flag9 = ability.act is ActBolt;
				if (!flag || (owner.IsPCParty && (EClass._zone.IsTown || EClass._zone.IsPCFaction)) || (act.id == 9150 && EClass._zone.IsPCFaction && owner.IsNeutralOrAbove()))
				{
					continue;
				}
				GetNumEnemy(flag9 ? 6 : 5);
				if (numEnemy == 0 || (owner.IsPCFactionOrMinion && GetNumNeutral(flag9 ? 6 : 5) > 0))
				{
					continue;
				}
				num = tactics.P_Spell - 20 + numEnemy * 10 + GetAttackMod(act);
				break;
			}
			case "buff":
				num = ForeachChara(ability, (Chara c) => (!c.HasCondition(s.proc[1])) ? tactics.P_Buff : 0, isFriendlyAbility: true);
				if (ability.aiPt || (owner.IsPC && tactics.CastPartyBuff))
				{
					ability.pt = true;
				}
				break;
			case "buffStats":
				num = ForeachChara(ability, delegate(Chara c)
				{
					Element buffStats2 = c.GetBuffStats(s.proc[1]);
					return (buffStats2 == null || buffStats2.Value < 0) ? tactics.P_Buff : 0;
				}, isFriendlyAbility: true);
				if (ability.aiPt || (owner.IsPC && tactics.CastPartyBuff))
				{
					ability.pt = true;
				}
				break;
			case "debuff":
				if (!flag)
				{
					continue;
				}
				num = tactics.P_Debuff;
				if (ability.aiPt)
				{
					ability.pt = true;
				}
				break;
			case "debuffStats":
				if (!flag)
				{
					continue;
				}
				num = ForeachChara(ability, delegate(Chara c)
				{
					Element buffStats = c.GetBuffStats(s.proc[1]);
					return (buffStats == null || buffStats.Value > 0) ? tactics.P_Debuff : 0;
				}, isFriendlyAbility: false);
				if (ability.aiPt)
				{
					ability.pt = true;
				}
				break;
			case "ground":
				if (!flag || owner.isRestrained || (tc != null && tc.IsRestrainedResident))
				{
					continue;
				}
				num = 50;
				if (isPCFaction)
				{
					num -= 10;
				}
				break;
			case "summon":
			{
				if (owner.isRestrained || (tc != null && tc.IsRestrainedResident))
				{
					continue;
				}
				int num2 = EClass._zone.CountMinions(owner);
				if (num2 >= owner.MaxSummon)
				{
					continue;
				}
				num = tactics.P_Summon - 20 * num2 / owner.MaxSummon;
				break;
			}
			case "summonAlly":
				if (owner.isRestrained || (tc != null && tc.IsRestrainedResident))
				{
					continue;
				}
				if (owner.IsPC)
				{
					if (EClass.player.lastEmptyAlly <= 0)
					{
						continue;
					}
				}
				else if (EClass._zone.CountMinions(owner) > 0)
				{
					continue;
				}
				num = tactics.P_Summon;
				break;
			case "suicide":
				if (owner.IsPC || owner.HasCondition<ConWet>())
				{
					continue;
				}
				if (owner.HasTag(CTAG.kamikaze))
				{
					num = ((dist <= 1) ? 1000 : 0);
					break;
				}
				num = 100 - 125 * owner.hp / owner.MaxHP;
				if (EClass.rnd(200) <= num && (!owner.IsPowerful || owner.hp < owner.MaxHP / 2))
				{
					break;
				}
				continue;
			default:
				num = 0;
				break;
			}
			if (s.target == "Neighbor")
			{
				if (dist > 1)
				{
					continue;
				}
				num += 10;
			}
			if (s.proc.Length != 0 && s.proc[0] == "Debuff" && tc.HasCondition(s.proc[1]))
			{
				continue;
			}
			if (s.abilityType.Length > 1)
			{
				num += (owner.IsPC ? s.abilityType[2] : s.abilityType[1]).ToInt();
			}
			if (act is Spell)
			{
				if (owner.IsPC)
				{
					if (act.vPotential <= 0)
					{
						continue;
					}
					if (flag2 || isConfused || flag4)
					{
						num -= 50;
					}
				}
				else
				{
					if (flag2)
					{
						num -= 30;
					}
					if (isConfused || flag4)
					{
						num -= 10;
					}
				}
			}
			if (num > 0)
			{
				num += ability.priorityMod + EClass.rnd(tactics.RandomFacotr + ability.priorityMod);
			}
			ability.priority = num;
			int HealFactor(Chara c)
			{
				if (isHOT && c.HasCondition(s.proc[1]))
				{
					return 0;
				}
				float num3 = (float)c.hp / (float)c.MaxHP;
				if (num3 > (isHOT ? 0.85f : 0.75f))
				{
					return 0;
				}
				int num4 = tactics.P_Heal - (int)((float)tactics.P_Heal * num3) + (isHOT ? 50 : 25);
				foreach (Condition condition in c.conditions)
				{
					if (condition is ConFear)
					{
						num4 += 10;
					}
					else if (condition is ConPoison)
					{
						num4 += 2;
					}
					else if (condition is ConConfuse)
					{
						num4 += 4;
					}
					else if (condition is ConDim)
					{
						num4 += 6;
					}
					else if (condition is ConBleed)
					{
						num4 += 8;
					}
				}
				return num4;
			}
		}
		abilities.Sort((ItemAbility a, ItemAbility b) => b.priority - a.priority);
		foreach (ItemAbility ability2 in abilities)
		{
			if (ability2.priority <= 0)
			{
				continue;
			}
			if (EClass.debug.logCombat && owner.IsPC)
			{
				Debug.Log(ability2.act.Name + "/" + ability2.priority);
			}
			if (ability2.act.source.alias == "ActRanged")
			{
				if (TryThrow(dist))
				{
					return true;
				}
				if (TryUseRanged(dist))
				{
					return true;
				}
				continue;
			}
			Cost cost = ability2.act.GetCost(owner);
			if (owner.IsPCParty && ability2.pt && !ability2.act.IsTargetHostileParty() && !ability2.act.TargetType.ForceParty && cost.cost * EClass.pc.party.members.Count > owner.mana.value)
			{
				continue;
			}
			if (isPCFaction && cost.cost > 0)
			{
				switch (cost.type)
				{
				case CostType.MP:
					if (cost.cost > owner.mana.value)
					{
						continue;
					}
					break;
				case CostType.SP:
					if (cost.cost > owner.stamina.value)
					{
						continue;
					}
					break;
				}
			}
			if (cost.cost > 0 && EClass.rnd(100) > tactics.AbilityChance)
			{
				continue;
			}
			Chara chara = owner;
			if (ability2.act.CanPerform(owner, ability2.tg ?? tc) && owner.UseAbility(ability2.act, ability2.tg ?? tc, null, (ability2.act.HaveLongPressAction && ability2.pt) || ability2.aiPt))
			{
				if (EClass.debug.logCombat)
				{
					Debug.Log("Used Ability: " + chara?.ToString() + "/" + ability2.act?.ToString() + "/" + ability2.tg?.ToString() + "/" + tc);
					Debug.Log(ability2.act.CanPerform(chara, ability2.tg ?? tc));
				}
				return true;
			}
		}
		if (EClass.debug.logCombat)
		{
			Debug.Log(owner.Name + "/" + abilities.Count);
			foreach (ItemAbility ability3 in abilities)
			{
				Debug.Log(ability3.act.Name + "/" + ability3.priority);
			}
		}
		return false;
		void BuildCharaList()
		{
			if (charaBuilt)
			{
				return;
			}
			charas.Clear();
			charaBuilt = true;
			int sightRadius = owner.GetSightRadius();
			foreach (Chara chara2 in EClass._map.charas)
			{
				if (chara2 != owner)
				{
					int num7 = owner.Dist(chara2);
					if (num7 > sightRadius || !owner.CanSeeLos(chara2, num7))
					{
						continue;
					}
				}
				charas.Add(chara2);
			}
		}
		int ForeachChara(ItemAbility a, Func<Chara, int> func, bool isFriendlyAbility)
		{
			if (a.act.TargetType.Range == TargetRange.Self)
			{
				a.tg = owner;
				return func(owner);
			}
			BuildCharaList();
			int num8 = 0;
			foreach (Chara chara3 in charas)
			{
				int num9 = func(chara3);
				if (num9 > 0)
				{
					if (isFriendlyAbility)
					{
						if (owner.IsPCParty)
						{
							if (!chara3.IsPCParty)
							{
								continue;
							}
						}
						else if (!owner.IsFriendOrAbove(chara3))
						{
							continue;
						}
						if (chara3 != owner)
						{
							num9 += tactics.P_Party;
						}
					}
					else if (!owner.IsHostile(chara3))
					{
						continue;
					}
					if (num9 >= num8)
					{
						a.tg = chara3;
						num8 = num9;
					}
				}
			}
			return num8;
		}
		int GetAttackMod(Act a)
		{
			if (!owner.IsPCParty || a.source.aliasRef.IsEmpty())
			{
				return 0;
			}
			int num5 = ((a.source.aliasRef == "mold") ? owner.MainElement.id : EClass.sources.elements.alias[a.source.aliasRef].id);
			int num6 = -15 * tc.ResistLvFrom(num5);
			switch (num5)
			{
			case 910:
				if (tc.isWet)
				{
					num6 -= 30;
				}
				break;
			case 911:
				if (tc.HasCondition<ConBurning>())
				{
					num6 -= 30;
				}
				break;
			case 912:
				if (tc.isWet)
				{
					num6 += 30;
				}
				break;
			}
			return num6;
		}
		void GetNumEnemy(int radius)
		{
			if (numEnemy != -1)
			{
				return;
			}
			BuildCharaList();
			numEnemy = 0;
			foreach (Chara chara4 in charas)
			{
				if (chara4.host == null && owner.IsHostile(chara4) && owner.Dist(chara4) < radius && owner.CanSeeLos(chara4))
				{
					numEnemy++;
				}
			}
		}
		int GetNumNeutral(int radius)
		{
			if (numNeutral != -1)
			{
				return numNeutral;
			}
			BuildCharaList();
			numNeutral = 0;
			foreach (Chara chara5 in charas)
			{
				if (!chara5.IsPCFactionOrMinion && chara5.IsNeutralOrAbove() && owner.Dist(chara5) <= radius && owner.CanSeeLos(chara5))
				{
					numNeutral++;
				}
			}
			return numNeutral;
		}
	}

	public virtual void BuildAbilityList()
	{
		foreach (ActList.Item item in owner.ability.list.items)
		{
			AddAbility(item.act, 0, item.chance, item.pt);
		}
		AddAbility(ACT.Ranged);
		AddAbility(ACT.Melee);
		AddAbility(ACT.Item);
		if (owner.HasCondition<ConBrightnessOfLife>())
		{
			TryAddAbility(6410);
		}
	}

	public virtual bool TryAbortCombat()
	{
		return false;
	}
}
