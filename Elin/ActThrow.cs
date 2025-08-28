using UnityEngine;

public class ActThrow : ActBaseAttack
{
	public Thing target;

	public Chara pcTarget;

	public override bool CanPressRepeat => true;

	public override TargetType TargetType => TargetType.Ground;

	public override int PerformDistance => 99;

	public override string GetText(string str = "")
	{
		string text = "";
		if (target != null && pcTarget != null && target.trait is TraitMonsterBall && pcTarget.LV > target.LV)
		{
			text = " " + "mb_invalidLV".lang();
		}
		return base.GetText(str) + text;
	}

	public override bool CanPerform()
	{
		if (pcTarget != null && pcTarget.ExistsOnMap)
		{
			Act.TP.Set(pcTarget.pos);
		}
		if (!Act.TP.IsHidden && !Act.TP.IsSky)
		{
			return Act.CC.CanSeeLos(Act.TP);
		}
		return false;
	}

	public override bool Perform()
	{
		if (target == null)
		{
			target = Act.TC as Thing;
		}
		if (target == null || target.isDestroyed || target.GetRootCard() != Act.CC)
		{
			return false;
		}
		if (pcTarget != null)
		{
			if (!pcTarget.ExistsOnMap)
			{
				return false;
			}
			Act.TP.Set(pcTarget.pos);
		}
		Throw(Act.CC, Act.TP, target.HasElement(410) ? target : target.Split(1));
		return true;
	}

	public static bool CanThrow(Chara c, Thing t, Card target, Point p = null)
	{
		if (t == null)
		{
			return false;
		}
		if (t.c_isImportant && !t.HasElement(410))
		{
			return false;
		}
		if (p == null && target != null && target.ExistsOnMap)
		{
			p = target.pos;
		}
		if (p == null)
		{
			return false;
		}
		if ((p.HasBlock && !p.cell.hasDoor && p.sourceBlock.tileType.IsBlockPass) || p.Equals(EClass.pc.pos) || t.trait.CanExtendBuild || t.trait.CanOnlyCarry)
		{
			return false;
		}
		if (c.IsPC && EClass.core.config.game.shiftToUseNegativeAbilityOnSelf && !EInput.isShiftDown && !(t.trait is TraitDrink))
		{
			Card card = target ?? p.FindAttackTarget();
			if (card != null && card.IsPCFactionOrMinion)
			{
				return false;
			}
		}
		return true;
	}

	public static EffectIRenderer Throw(Card c, Point p, Thing t, ThrowMethod method = ThrowMethod.Default, float failChance = 0f)
	{
		if (failChance > EClass.rndf(1f))
		{
			Point randomPoint = p.GetRandomPoint(1);
			if (randomPoint != null && !randomPoint.Equals(c.pos))
			{
				p = randomPoint;
			}
		}
		return Throw(c, p, p.FindAttackTarget(), t, method);
	}

	public static EffectIRenderer Throw(Card c, Point p, Card target, Thing t, ThrowMethod method = ThrowMethod.Default)
	{
		if (t.parent != EClass._zone && !t.HasElement(410))
		{
			EClass._zone.AddCard(t, c.pos).KillAnime();
		}
		Act.TP.Set(p);
		Act.TC = target;
		if (t.trait.ThrowType == ThrowType.Snow)
		{
			t.dir = EClass.rnd(2);
			c.Talk("snow_throw");
			if (EClass.rnd(2) == 0)
			{
				Act.TC = null;
			}
		}
		c.Say("throw", c, t.GetName(NameStyle.Full, 1));
		c.LookAt(p);
		c.renderer.NextFrame();
		c.PlaySound("throw");
		EffectIRenderer result = null;
		if (c.isSynced || p.IsSync)
		{
			result = Effect.Get<EffectIRenderer>((t.trait is TraitBall) ? "throw_ball" : "throw").Play((c.isChara && c.Chara.host != null) ? c.Chara.host : c, t, c.pos, p, 0.2f);
			t.renderer.SetFirst(first: false, c.renderer.position);
		}
		if (!t.HasElement(410))
		{
			t._Move(p);
		}
		if (!t.trait.CanBeDestroyed)
		{
			c.PlaySound("miss");
			return result;
		}
		ThrowType throwType = t.trait.ThrowType;
		if ((uint)(throwType - 1) <= 1u)
		{
			Msg.Say("shatter");
		}
		bool flag = method == ThrowMethod.Reward;
		bool flag2 = method == ThrowMethod.Default;
		switch (t.trait.ThrowType)
		{
		case ThrowType.Explosive:
			flag = true;
			t.c_uidRefCard = c.uid;
			t.Die(null, c, AttackSource.Throw);
			break;
		case ThrowType.Vase:
			t.Die(null, null, AttackSource.Throw);
			break;
		case ThrowType.Potion:
			flag = true;
			if (Act.TC != null)
			{
				Act.TC.Say("throw_hit", t, Act.TC);
			}
			Act.TP.ModFire(-50, extinguish: true);
			if (Act.TC != null && Act.TC.isChara)
			{
				if (t.trait.CanDrink(Act.TC.Chara))
				{
					t.trait.OnDrink(Act.TC.Chara);
				}
				flag2 = t.IsNegativeGift;
				Act.TC.Chara.AddCondition<ConWet>();
			}
			else
			{
				t.trait.OnThrowGround(c.Chara, Act.TP);
			}
			t.Die(null, null, AttackSource.Throw);
			c.ModExp(108, 50);
			break;
		case ThrowType.Snow:
			flag = true;
			flag2 = false;
			if (Act.TC != null && Act.TC.isChara)
			{
				Act.TC.Say("throw_hit", t, Act.TC);
				if (EClass.rnd(2) == 0)
				{
					c.Talk("snow_hit");
				}
				Act.TC.Chara.AddCondition<ConWet>(50);
				t.Die(null, null, AttackSource.Throw);
				c.ModExp(108, 50);
			}
			break;
		case ThrowType.Ball:
			flag = true;
			flag2 = false;
			if (Act.TC != null && Act.TC.isChara)
			{
				Act.TC.Say("throw_hit", t, Act.TC);
				if (EClass.rnd(2) == 0)
				{
					c.Talk("snow_hit");
				}
				Act.TC.Say("ball_hit");
				Act.TC.Chara?.Pick(t);
				c.ModExp(108, 50);
			}
			break;
		case ThrowType.Flyer:
			flag = true;
			flag2 = false;
			if (Act.TC != null && Act.TC.isChara && c.isChara)
			{
				Act.TC.Say("throw_hit", t, Act.TC);
				c.Chara.GiveGift(Act.TC.Chara, t);
				c.ModExp(108, 50);
			}
			break;
		case ThrowType.MonsterBall:
		{
			flag = true;
			flag2 = false;
			TraitMonsterBall traitMonsterBall = t.trait as TraitMonsterBall;
			if (traitMonsterBall.chara != null)
			{
				if (traitMonsterBall.IsLittleBall && !(EClass._zone is Zone_LittleGarden) && !EClass.game.IsSurvival)
				{
					break;
				}
				Chara _c = EClass._zone.AddCard(traitMonsterBall.chara, p).Chara;
				_c.PlayEffect("identify");
				t.Die();
				if (traitMonsterBall.IsLittleBall && _c.id == "littleOne")
				{
					_c.orgPos = c.pos.Copy();
					Chara chara = _c;
					Hostility c_originalHostility = (_c.hostility = Hostility.Neutral);
					chara.c_originalHostility = c_originalHostility;
					EClass._zone.ModInfluence(5);
					_c.PlaySound("chime_angel");
					EClass.core.actionsNextFrame.Add(delegate
					{
						_c.Talk("little_saved");
					});
					EClass.player.flags.little_saved = true;
					EClass.player.little_saved++;
				}
				else
				{
					_c.MakeAlly();
				}
			}
			else
			{
				if (Act.TC == null || !Act.TC.isChara)
				{
					break;
				}
				Act.TC.Say("throw_hit", t, Act.TC);
				Chara chara2 = Act.TC.Chara;
				if (traitMonsterBall.IsLittleBall)
				{
					if (chara2.id != "littleOne" || chara2.IsPCFactionOrMinion || EClass._zone is Zone_LittleGarden || EClass._zone.IsUserZone)
					{
						Msg.Say("monsterball_invalid");
						break;
					}
				}
				else
				{
					if (!chara2.trait.CanBeTamed || EClass._zone.IsUserZone)
					{
						Msg.Say("monsterball_invalid");
						break;
					}
					if (chara2.LV > traitMonsterBall.owner.LV)
					{
						Msg.Say("monsterball_lv");
						break;
					}
					if (!EClass.debug.enable && chara2.hp > chara2.MaxHP / 10)
					{
						Msg.Say("monsterball_hp");
						break;
					}
				}
				Msg.Say("monsterball_capture", c, chara2);
				chara2.PlaySound("identify");
				chara2.PlayEffect("identify");
				t.ChangeMaterial("copper");
				if (chara2.IsLocalChara)
				{
					Debug.Log("Creating Replacement NPC for:" + chara2);
					EClass._map.deadCharas.Add(chara2.CreateReplacement());
				}
				traitMonsterBall.chara = chara2;
				EClass._zone.RemoveCard(chara2);
				chara2.homeZone = null;
				c.ModExp(108, 100);
			}
			break;
		}
		}
		if (t.trait is TraitDye)
		{
			t.trait.OnThrowGround(c.Chara, Act.TP);
		}
		if (!flag && Act.TC != null)
		{
			AttackProcess.Current.Prepare(c.Chara, t, Act.TC, Act.TP, 0, _isThrow: true);
			if (AttackProcess.Current.Perform(0, hasHit: false))
			{
				if (Act.TC.IsAliveInCurrentZone && t.trait is TraitErohon && Act.TC.id == t.c_idRefName)
				{
					Act.TC.Chara.OnGiveErohon(t);
				}
				if (!t.isDestroyed && t.trait.CanBeDestroyed && !t.IsFurniture && !t.category.IsChildOf("instrument") && !t.IsUnique && !t.HasElement(410))
				{
					t.Destroy();
				}
			}
			else
			{
				c.PlaySound("miss");
			}
		}
		if (EClass.rnd(2) == 0)
		{
			c.Chara.RemoveCondition<ConInvisibility>();
		}
		if (Act.TC != null)
		{
			if (flag2)
			{
				c.Chara.DoHostileAction(Act.TC);
			}
			if ((Act.TC.trait.CanBeAttacked || Act.TC.IsRestrainedResident) && EClass.rnd(2) == 0)
			{
				c.Chara.stamina.Mod(-1);
			}
		}
		if (t.HasElement(410) && Act.TC != null && Act.CC == EClass.pc && !(Act.CC.ai is AI_PracticeDummy) && (Act.TC.trait is TraitTrainingDummy || Act.TC.IsRestrainedResident) && Act.CC.stamina.value > 0)
		{
			Act.CC.SetAI(new AI_PracticeDummy
			{
				target = Act.TC,
				throwItem = t
			});
		}
		return result;
	}
}
