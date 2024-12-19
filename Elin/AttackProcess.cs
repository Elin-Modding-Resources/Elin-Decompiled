using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttackProcess : EClass
{
	public static AttackProcess Current = new AttackProcess();

	public int dNum;

	public int dDim;

	public int dBonus;

	public int toHit;

	public int toHitBase;

	public int toHitFix;

	public int evasion;

	public int penetration;

	public int distMod;

	public int attackIndex;

	public int dNumAmmo;

	public int dDimAmmo;

	public int dBonusAmmo;

	public int numFire;

	public int numFireWithoutDamageLoss;

	public float dMulti;

	public bool crit;

	public bool hit;

	public bool evadePlus;

	public bool isThrow;

	public bool ignoreAnime;

	public bool ignoreAttackSound;

	public Chara CC;

	public Card TC;

	public Point TP;

	public Point posRangedAnime;

	public Element weaponSkill;

	public Thing weapon;

	public Thing ammo;

	public TraitToolRange toolRange;

	public AttackType attackType;

	public AttackStyle attackStyle;

	public bool IsMartial => weapon == null;

	public bool IsMartialWeapon
	{
		get
		{
			if (weapon != null)
			{
				return weapon.category.skill == 100;
			}
			return false;
		}
	}

	public bool IsRanged
	{
		get
		{
			if (toolRange != null && !isThrow)
			{
				return !toolRange.owner.Thing.isEquipped;
			}
			return false;
		}
	}

	public bool IsCane
	{
		get
		{
			if (IsRanged)
			{
				return toolRange is TraitToolRangeCane;
			}
			return false;
		}
	}

	public string GetText()
	{
		string text = dNum + "d" + dDim;
		text = text + ((dBonus >= 0) ? "+" : "") + dBonus;
		string @ref = (IsMartial ? "evalHand".lang() : "evalWeapon".lang((attackIndex + 1).ToString() ?? ""));
		return "attackEval".lang(@ref, text, dMulti.ToString("F2") ?? "", toHit.ToString() ?? "", penetration.ToString() ?? "");
	}

	public void Prepare(Chara _CC, Thing _weapon, Card _TC = null, Point _TP = null, int _attackIndex = 0, bool _isThrow = false)
	{
		CC = _CC;
		TC = _TC;
		TP = _TP;
		isThrow = _isThrow;
		weapon = _weapon;
		ammo = _weapon?.ammoData;
		hit = (crit = (evadePlus = false));
		toolRange = weapon?.trait as TraitToolRange;
		attackType = AttackType.Slash;
		attackStyle = AttackStyle.Default;
		evasion = 0;
		penetration = 0;
		distMod = 100;
		attackIndex = _attackIndex;
		posRangedAnime = TP;
		ignoreAnime = (ignoreAttackSound = false);
		if (!isThrow)
		{
			if (!IsRanged)
			{
				attackStyle = CC.body.GetAttackStyle();
			}
			else if (TP != null)
			{
				int num = CC.pos.Distance(TP);
				distMod = Mathf.Max(115 - 10 * Mathf.Abs(num - toolRange.BestDist) * 100 / (100 + weapon.Evalue(605) * 10), 80);
			}
		}
		if (isThrow)
		{
			bool flag = weapon.HasTag(CTAG.throwWeapon) || weapon.HasTag(CTAG.throwWeaponEnemy);
			int num2 = (int)Mathf.Clamp(Mathf.Sqrt(weapon.SelfWeight + weapon.ChildrenWeight) * 3f + 25f + (float)(flag ? 75 : 0), 10f, 400f + Mathf.Sqrt(CC.STR) * 50f);
			int num3 = Mathf.Clamp(weapon.material.hardness, flag ? 40 : 20, 200);
			weaponSkill = CC.elements.GetOrCreateElement(108);
			attackType = AttackType.Blunt;
			dBonus = CC.DMG + (CC.IsPCParty ? 3 : 7);
			dNum = 2;
			dDim = (((!CC.IsPCParty) ? CC.LV : 0) + CC.STR + CC.Evalue(108)) * num2 * num3 / 10000 / 2;
			dMulti = 1f;
			toHitBase = EClass.curve(CC.DEX / 4 + CC.STR / 2 + weaponSkill.Value, 50, 25) + (CC.IsPCFaction ? 75 : 250);
			toHitFix = CC.HIT + weapon.HIT;
			penetration = 25;
		}
		else if (IsMartial || IsMartialWeapon)
		{
			weaponSkill = CC.elements.GetOrCreateElement(100);
			bool flag2 = weapon != null && weapon.Evalue(482) > 0;
			if (flag2)
			{
				weaponSkill = CC.elements.GetOrCreateElement(305);
			}
			attackType = ((!CC.race.meleeStyle.IsEmpty()) ? CC.race.meleeStyle.ToEnum<AttackType>() : ((EClass.rnd(2) == 0) ? AttackType.Kick : AttackType.Punch));
			dBonus = CC.DMG + CC.encLV + (int)Mathf.Sqrt(Mathf.Max(0, weaponSkill.GetParent(CC).Value / 5 + weaponSkill.Value / 4));
			dNum = 2 + Mathf.Min(weaponSkill.Value / 10, 4);
			dDim = 5 + (int)Mathf.Sqrt(Mathf.Max(0, weaponSkill.Value / 3));
			dMulti = 0.6f + (float)(weaponSkill.GetParent(CC).Value / 2 + weaponSkill.Value / 2 + CC.Evalue(flag2 ? 304 : 132) / 2) / 50f;
			dMulti += 0.05f * (float)CC.Evalue(1400);
			toHitBase = EClass.curve(CC.DEX / 3 + weaponSkill.GetParent(CC).Value / 3 + weaponSkill.Value, 50, 25) + 50;
			toHitFix = CC.HIT;
			if (attackStyle == AttackStyle.Shield)
			{
				toHitBase = toHitBase * 75 / 100;
			}
			penetration = Mathf.Clamp(weaponSkill.Value / 10 + 5, 5, 20) + CC.Evalue(92);
			if (IsMartialWeapon)
			{
				dBonus += weapon.DMG;
				dNum += weapon.source.offense[0];
				dDim = Mathf.Max(dDim / 2 + weapon.c_diceDim, 1);
				toHitFix += weapon.HIT;
				penetration += weapon.Penetration;
				if (!weapon.source.attackType.IsEmpty())
				{
					attackType = weapon.source.attackType.ToEnum<AttackType>();
				}
			}
		}
		else
		{
			if (IsRanged)
			{
				weaponSkill = CC.elements.GetOrCreateElement(toolRange.WeaponSkill);
			}
			else
			{
				weaponSkill = CC.elements.GetOrCreateElement(weapon.category.skill);
			}
			if (!weapon.source.attackType.IsEmpty())
			{
				attackType = weapon.source.attackType.ToEnum<AttackType>();
			}
			bool flag3 = IsCane || weapon.Evalue(482) > 0;
			if (flag3)
			{
				weaponSkill = CC.elements.GetOrCreateElement(305);
			}
			dBonus = CC.DMG + CC.encLV + weapon.DMG;
			dNum = weapon.source.offense[0];
			dDim = weapon.c_diceDim;
			dMulti = 0.6f + (float)(weaponSkill.GetParent(CC).Value + weaponSkill.Value / 2 + CC.Evalue(flag3 ? 304 : (IsRanged ? 133 : 132))) / 50f;
			dMulti += 0.05f * (float)CC.Evalue(IsRanged ? 1404 : 1400);
			toHitBase = EClass.curve((IsCane ? CC.WIL : CC.DEX) / 4 + weaponSkill.GetParent(CC).Value / 3 + weaponSkill.Value, 50, 25) + 50;
			toHitFix = CC.HIT + weapon.HIT;
			penetration = weapon.Penetration + CC.Evalue(92);
			if (IsCane)
			{
				toHitBase += 50;
			}
		}
		if (ammo != null && !(ammo.trait is TraitAmmoTalisman))
		{
			dNumAmmo = ((ammo.source.offense.Length != 0) ? ammo.source.offense[0] : 0);
			dDimAmmo = ammo.c_diceDim;
			dBonusAmmo = ammo.DMG + ammo.encLV;
			if (dNumAmmo < 1)
			{
				dNumAmmo = 1;
			}
			if (dDimAmmo < 1)
			{
				dDimAmmo = 1;
			}
			toHitFix += ammo.HIT;
		}
		else
		{
			dNumAmmo = 0;
			dDimAmmo = 0;
		}
		if (dNum < 1)
		{
			dNum = 1;
		}
		if (dDim < 1)
		{
			dDim = 1;
		}
		if (penetration > 100)
		{
			penetration = 100;
		}
		if (attackStyle == AttackStyle.TwoHand)
		{
			dMulti = dMulti * 1.5f + 0.1f * Mathf.Sqrt(Mathf.Max(0, CC.Evalue(130)));
		}
		dMulti = dMulti * (float)distMod / 100f;
		toHit = toHitBase + toHitFix;
		toHit = toHit * distMod / 100;
		if (CC.HasCondition<ConBane>())
		{
			toHit = toHit * 75 / 100;
		}
		if (TC != null && CC.HasHigherGround(TC))
		{
			toHit = toHit * 120 / 100;
		}
		if (CC.ride != null)
		{
			toHit = toHit * 100 / (100 + 500 / Mathf.Max(5, 10 + CC.EvalueRiding()));
		}
		if (CC.parasite != null)
		{
			toHit = toHit * 100 / (100 + 1000 / Mathf.Max(5, 10 + CC.Evalue(227)));
		}
		if (CC.host != null)
		{
			if (CC.host.ride == CC)
			{
				toHit = toHit * 100 / (100 + 1000 / Mathf.Max(5, 10 + CC.STR));
			}
			if (CC.host.parasite == CC)
			{
				toHit = toHit * 100 / (100 + 2000 / Mathf.Max(5, 10 + CC.DEX));
			}
		}
		if (attackStyle == AttackStyle.TwoHand)
		{
			toHit += 25 + (int)Mathf.Sqrt(Mathf.Max(0, CC.Evalue(130)) * 2);
		}
		else if (attackStyle == AttackStyle.TwoWield && toHit > 0)
		{
			toHit = toHit * 100 / (115 + attackIndex * 15 + attackIndex * Mathf.Clamp(2000 / (20 + CC.Evalue(131)), 0, 100));
		}
		if (CC.isBlind)
		{
			toHit /= ((IsRanged || isThrow) ? 10 : 3);
		}
		if (CC.isConfused || CC.HasCondition<ConDim>())
		{
			toHit /= 2;
		}
		if (TC != null)
		{
			evasion = EClass.curve(TC.PER / 3 + TC.Evalue(150), 50, 10) + TC.DV + 25;
			if (TC.isChara && TC.Chara.isBlind)
			{
				evasion /= 2;
			}
			if (TC.HasCondition<ConDim>())
			{
				evasion /= 2;
			}
			if (TC.isChara && TC.Chara.HasHigherGround(CC))
			{
				evasion = evasion * 120 / 100;
			}
		}
	}

	public void PlayRangedAnime(int numFire)
	{
		bool isGun = toolRange is TraitToolRangeGun;
		bool isCane = toolRange is TraitToolRangeCane;
		GameSetting.EffectData data = EClass.setting.effect.guns.TryGetValue(weapon.id) ?? EClass.setting.effect.guns[isCane ? "cane" : (isGun ? "gun" : "bow")];
		bool isPCC = CC.IsPCC && CC.renderer.hasActor;
		Vector2 firePos = (isPCC ? new Vector2(data.firePos.x * (float)((CC.renderer.actor.currentDir != 0 && CC.renderer.actor.currentDir != 1) ? 1 : (-1)), data.firePos.y) : Vector2.zero);
		Chara _CC = CC;
		Point _TP = posRangedAnime.Copy();
		Thing _weapon = weapon;
		bool ignoreSound = ignoreAttackSound;
		Zone _zone = CC.currentZone;
		for (int i = 0; i < numFire; i++)
		{
			TweenUtil.Delay((float)i * data.delay, delegate
			{
				if (EClass.core.IsGameStarted && _CC.IsAliveInCurrentZone && _zone == _CC.currentZone)
				{
					if (_weapon.id == "gun_rail")
					{
						_CC.PlayEffect("laser").GetComponent<SpriteBasedLaser>().Play(_TP.PositionCenter());
					}
					else
					{
						Effect effect = Effect.Get("ranged_arrow")._Play(_CC.pos, _CC.isSynced ? _CC.renderer.position : _CC.pos.Position(), 0f, _TP, data.sprite);
						if (isCane)
						{
							IEnumerable<Element> enumerable = toolRange.owner.elements.dict.Values.Where((Element e) => e.source.categorySub == "eleAttack");
							if (enumerable.Count() > 0)
							{
								Element element = enumerable.RandomItem();
								effect.sr.color = EClass.Colors.elementColors[element.source.alias];
							}
						}
					}
					if (data.eject)
					{
						if (!ignoreSound)
						{
							_CC.PlaySound("bullet_drop");
						}
						_CC.PlayEffect("bullet").Emit(1);
					}
					if (isGun)
					{
						if (isPCC)
						{
							_weapon.PlayEffect(data.idEffect.IsEmpty("gunfire"), useRenderPos: true, 0f, firePos);
						}
						else
						{
							_CC.PlayEffect(data.idEffect.IsEmpty("gunfire"));
						}
					}
					if (!ignoreSound)
					{
						_CC.PlaySound(data.idSound.IsEmpty("attack_gun"));
					}
				}
			});
		}
	}

	public bool Perform(int count, bool hasHit, float dmgMulti = 1f, bool maxRoll = false)
	{
		bool flag = CC.HasCondition<ConReload>();
		hit = CalcHit();
		int num = Dice.Roll(dNum, dDim, dBonus, CC);
		if (ammo != null && !flag)
		{
			num += Dice.Roll(dNumAmmo, dDimAmmo, dBonusAmmo, CC);
		}
		if (crit || maxRoll)
		{
			num = Dice.RollMax(dNum, dDim, dBonus);
			if (ammo != null && !flag)
			{
				num += Dice.RollMax(dNumAmmo, dDimAmmo, dBonusAmmo);
			}
			if (crit && (IsMartial || IsMartialWeapon))
			{
				dMulti *= 1.25f;
			}
		}
		int num2 = CC.Evalue(1355);
		ConStrife condition = CC.GetCondition<ConStrife>();
		if (condition != null)
		{
			num += condition.GetDice().Roll();
		}
		else if (num2 > 0)
		{
			num++;
		}
		num = Mathf.Clamp(num, 0, 9999999);
		num = (int)(dMulti * (float)num * dmgMulti);
		num = Mathf.Clamp(num, 0, 9999999);
		if (IsRanged && count >= numFireWithoutDamageLoss)
		{
			num = num * 100 / (100 + (count - numFireWithoutDamageLoss + 1) * 30);
		}
		if (CC.isRestrained)
		{
			num /= 2;
		}
		List<Element> list2 = new List<Element>();
		int num3 = CC.Evalue(91);
		int num4 = 0;
		if (weapon != null)
		{
			list2 = weapon.elements.dict.Values.ToList();
			if (ammo != null && !flag)
			{
				list2 = list2.Concat(ammo.elements.dict.Values).ToList();
			}
			if (IsRanged || isThrow)
			{
				num3 += weapon.Evalue(91);
			}
			num4 += weapon.Evalue(603);
		}
		else if (CC.id == "rabbit_vopal")
		{
			list2.Add(Element.Create(6650, 100));
		}
		int bane;
		if (TC?.Chara != null)
		{
			SourceRace.Row race = TC.Chara.race;
			bane = CC.Evalue(468);
			if (IsRanged)
			{
				bane += toolRange.owner.Evalue(468);
			}
			AddBane(race.IsUndead, 461);
			AddBane(race.IsAnimal, 463);
			AddBane(race.IsHuman, 464);
			AddBane(race.IsDragon, 460);
			AddBane(race.IsGod, 466);
			AddBane(race.IsMachine, 465);
			AddBane(race.IsFish, 467);
			AddBane(race.IsFairy, 462);
			if (bane != 0)
			{
				num = num * (100 + bane * 3) / 100;
			}
		}
		if (CC.IsPCFaction)
		{
			foreach (Element value in EClass.pc.faction.charaElements.dict.Values)
			{
				list2.Add(value);
			}
		}
		if (hit && num3 > EClass.rnd(100))
		{
			CC.Say("vopal");
			penetration = 100;
		}
		if (crit && CC.IsPC)
		{
			CC.Say("critical");
			CC.PlaySound("critical");
		}
		if (CC.isSynced || (TC != null && TC.isSynced))
		{
			if (toolRange != null && (!IsRanged || count == 0) && !flag && !ignoreAnime)
			{
				PlayRangedAnime((!IsRanged) ? 1 : numFire);
			}
			if (hit && TC != null && !hasHit)
			{
				PlayHitEffect();
			}
		}
		if (TC == null)
		{
			CC.Say(IsRanged ? "attack_air_range" : "attack_air", CC);
			return true;
		}
		if (!hit)
		{
			if (TC != null)
			{
				if (CC.IsPCParty)
				{
					CC.Say(evadePlus ? "evadePlus2" : "evade2", CC, TC);
				}
				else
				{
					TC.Say(evadePlus ? "evadePlus" : "evade", TC, CC);
				}
				ModExpDef(150, 90);
				ModExpDef(151, 90);
			}
			Proc(list2);
			return false;
		}
		if (TC.IsPC)
		{
			Msg.SetColor("attack_pc");
			EClass.pc.Say("attackMeleeToPC", CC, TC, GetAttackText(attackType, 3));
		}
		else
		{
			CC.Say("attackMelee", CC, TC, GetAttackText(attackType, 0));
		}
		bool showEffect = true;
		int num5 = 0;
		int num6 = 0;
		int num7 = 0;
		ConWeapon conWeapon = null;
		if (weapon != null)
		{
			foreach (Element value2 in weapon.elements.dict.Values)
			{
				if (value2.source.categorySub == "eleConvert")
				{
					num5 = EClass.sources.elements.alias[value2.source.aliasRef].id;
					num6 = 50 + value2.Value * 2;
					num7 = Mathf.Min(value2.Value, 100);
					break;
				}
			}
		}
		if (num5 == 0)
		{
			if (CC.HasCondition<ConWeapon>())
			{
				conWeapon = CC.GetCondition<ConWeapon>();
				num5 = conWeapon.sourceElement.id;
				num6 = conWeapon.power / 2;
				num7 = 40 + (int)Mathf.Min(MathF.Sqrt(conWeapon.power), 80f);
			}
			if (conWeapon == null && weapon == null && (CC.MainElement != Element.Void || CC.HasElement(1565)))
			{
				num5 = (CC.HasElement(1565) ? 915 : CC.MainElement.id);
				num6 = CC.Power / 3 + EClass.rnd(CC.Power / 2);
				if (CC.MainElement != Element.Void)
				{
					num6 += CC.MainElement.Value;
				}
				showEffect = false;
				num7 = 50;
			}
			if (conWeapon == null && weapon != null && weapon.trait is TraitToolRangeCane)
			{
				IEnumerable<Element> enumerable = weapon.elements.dict.Values.Where((Element e) => e.source.categorySub == "eleAttack");
				if (enumerable.Count() > 0)
				{
					num5 = enumerable.RandomItem().id;
					num6 = num5 switch
					{
						920 => 30, 
						914 => 50, 
						918 => 50, 
						_ => 100, 
					};
				}
				num7 = 50;
			}
		}
		int num8 = num;
		int num9 = num * num7 / 100;
		num -= num9;
		int num10 = num * penetration / 100;
		num -= num10;
		num = TC.ApplyProtection(num) + num10 + num9;
		TC.DamageHP(num, num5, num6, (!IsRanged && !isThrow) ? AttackSource.Melee : AttackSource.Range, CC, showEffect);
		conWeapon?.Mod(-1);
		bool flag2 = IsCane || (weapon != null && weapon.Evalue(482) > 0);
		int attackStyleElement = CC.body.GetAttackStyleElement(attackStyle);
		int mod2 = 100 / (count + 1);
		if (!IsRanged || count == 0)
		{
			ModExpAtk(weaponSkill.id, mod2);
			ModExpAtk(flag2 ? 304 : (IsRanged ? 133 : 132), mod2);
		}
		if (crit)
		{
			ModExpAtk(134, 50);
		}
		if (count == 0 && attackStyleElement != 0)
		{
			ModExpAtk(attackStyleElement, 100);
		}
		if (!CC.IsAliveInCurrentZone || !TC.IsAliveInCurrentZone)
		{
			return true;
		}
		if (EClass.rnd(8) == 0 && TC.isChara && CC.HasElement(1219))
		{
			CC.Say("abCrab", CC, TC);
			TC.Chara.AddCondition<ConParalyze>(30 + EClass.rnd(30));
			TC.Chara.AddCondition<ConGravity>();
		}
		if (list2.Count > 0)
		{
			foreach (Element item in list2)
			{
				if (!TC.IsAliveInCurrentZone)
				{
					break;
				}
				if (item.source.categorySub == "eleAttack")
				{
					int num11 = 25;
					int dmg = EClass.rnd(num * (100 + item.Value * 10) / 500 + 5);
					if (conWeapon == null && weapon != null && weapon.trait is TraitToolRangeCane)
					{
						num11 = 0;
					}
					if (num11 >= EClass.rnd(100))
					{
						TC.DamageHP(dmg, item.id, isThrow ? (100 + item.Value * 5) : (30 + item.Value), AttackSource.WeaponEnchant, CC);
					}
				}
			}
			Proc(list2);
		}
		if (!CC.IsAliveInCurrentZone || !TC.IsAliveInCurrentZone)
		{
			return true;
		}
		if (!IsRanged && attackStyle == AttackStyle.Shield)
		{
			int num12 = CC.Evalue(123);
			if (CC.elements.ValueWithoutLink(123) >= 10 && Mathf.Clamp(Mathf.Sqrt(num12) - 2f, 8f, 12f) > (float)EClass.rnd(100))
			{
				num = num8 * Mathf.Min(50 + num12, 200) / 100;
				num = TC.ApplyProtection(num);
				Debug.Log("Bash:" + num + "/" + num8);
				CC.PlaySound("shield_bash");
				CC.Say("shield_bash", CC, TC);
				TC.DamageHP(num, AttackSource.None, CC);
				if (TC.IsAliveInCurrentZone && TC.isChara)
				{
					if (EClass.rnd(2) == 0)
					{
						TC.Chara.AddCondition<ConDim>(50 + (int)Mathf.Sqrt(num12) * 10);
					}
					TC.Chara.AddCondition<ConParalyze>(EClass.rnd(2), force: true);
				}
			}
		}
		if (!CC.IsAliveInCurrentZone || !TC.IsAliveInCurrentZone)
		{
			return true;
		}
		if (TC.isChara && num4 > 0 && num4 * 2 + 15 > EClass.rnd(100) && !TC.isRestrained && TC.Chara.TryMoveFrom(CC.pos) == Card.MoveResult.Success)
		{
			TC.pos.PlayEffect("vanish");
			TC.PlaySound("push", 1.5f);
		}
		return true;
		void AddBane(bool valid, int idEle)
		{
			if (valid)
			{
				if (IsRanged)
				{
					bane += toolRange.owner.Evalue(idEle);
				}
				bane += CC.Evalue(idEle);
			}
		}
		bool IgnoreExp()
		{
			if (!CC.HasEditorTag(EditorTag.Invulnerable) && !CC.HasEditorTag(EditorTag.InvulnerableToMobs) && !TC.HasEditorTag(EditorTag.Invulnerable))
			{
				return TC.HasEditorTag(EditorTag.InvulnerableToMobs);
			}
			return true;
		}
		void ModExpAtk(int ele, int mod)
		{
			if (!IgnoreExp() && (!TC.isCopy || EClass.rnd(10) == 0))
			{
				int a2 = (Mathf.Clamp((TC.LV + 10 - CC.elements.ValueWithoutLink(ele)) / 2, 1, 10) + Mathf.Min(TC.LV / 10, 10)) * mod / 100;
				a2 = Mathf.Min(a2, 200);
				if (TC == CC)
				{
					a2 /= 2;
				}
				if (a2 > 0)
				{
					CC.ModExp(ele, a2 + EClass.rnd(a2 / 2 + 1));
				}
			}
		}
		void ModExpDef(int ele, int mod)
		{
			if (!IgnoreExp() && (!CC.isCopy || EClass.rnd(10) == 0))
			{
				int a = (Mathf.Clamp((CC.LV + 10 - TC.elements.ValueWithoutLink(ele)) / 2, 1, 10) + Mathf.Min(CC.LV / 10, 10)) * mod / 100;
				a = Mathf.Min(a, TC.isRestrained ? 10 : 200);
				if (TC == CC)
				{
					a /= 2;
				}
				if (!TC.IsPC && !TC.isRestrained && !TC.HasHost)
				{
					a *= 3;
				}
				if (a > 0)
				{
					TC.ModExp(ele, a + EClass.rnd(a / 2 + 1));
				}
			}
		}
		void PlayHitEffect()
		{
			string id = "hit_default";
			string id2 = "hit_default";
			switch (attackType)
			{
			case AttackType.Slash:
				id2 = "hit_slash";
				id = "hit_slash";
				break;
			case AttackType.Spore:
				id2 = "hit_spore";
				id = "hit_spore";
				break;
			case AttackType.Claw:
			case AttackType.Bite:
				id2 = "hit_claw";
				id = "hit_claw";
				break;
			case AttackType.Blunt:
			case AttackType.Punch:
			case AttackType.Kick:
			case AttackType.Bow:
			case AttackType.Gun:
			case AttackType.Cane:
				id2 = "hit_blunt";
				id = "hit_blunt";
				break;
			}
			if (TC != null)
			{
				TC.PlayEffect(id2).SetScale(crit ? 1.25f : 0.75f);
			}
			CC.PlaySound(id);
		}
		void Proc(List<Element> list)
		{
			if (list == null)
			{
				return;
			}
			foreach (Element item2 in list)
			{
				if (item2 is Ability)
				{
					int num13 = 10 + item2.Value / 5;
					int power = EClass.curve((100 + item2.Value * 10) * (100 + weaponSkill.Value) / 100, 400, 100);
					if (num13 >= EClass.rnd(100))
					{
						Act obj = item2 as Act;
						Card card = (obj.TargetType.CanSelectSelf ? CC : TC);
						string text = ((item2.source.proc.Length >= 2) ? item2.source.proc[1] : "");
						switch (obj.source.abilityType.TryGet(0))
						{
						case "buff":
							if (CC.HasCondition(text))
							{
								continue;
							}
							card = CC;
							break;
						case "debuff":
						case "attack":
						case "dot":
							card = TC;
							break;
						}
						if (card.IsAliveInCurrentZone)
						{
							Card tC = TC;
							ActEffect.ProcAt(item2.source.proc[0].ToEnum<EffectId>(), power, BlessedState.Normal, CC, card, card.pos, isNeg: false, new ActRef
							{
								n1 = text,
								aliasEle = item2.source.aliasRef,
								noFriendlyFire = true
							});
							TC = tC;
						}
					}
				}
			}
		}
	}

	private bool Crit()
	{
		crit = true;
		return true;
	}

	private bool EvadePlus()
	{
		evadePlus = true;
		return false;
	}

	public bool CalcHit()
	{
		if (TC != null)
		{
			if (TC.HasCondition<ConDim>() && EClass.rnd(4) == 0)
			{
				return Crit();
			}
			if (TC.IsDeadOrSleeping)
			{
				return Crit();
			}
			int num = TC.Evalue(151);
			if (num != 0 && toHit < num * 10)
			{
				int num2 = evasion * 100 / Mathf.Clamp(toHit, 1, toHit);
				if (num2 > 300 && EClass.rnd(num + 250) > 100)
				{
					return EvadePlus();
				}
				if (num2 > 200 && EClass.rnd(num + 250) > 150)
				{
					return EvadePlus();
				}
				if (num2 > 150 && EClass.rnd(num + 250) > 200)
				{
					return EvadePlus();
				}
			}
			if (TC.Evalue(57) > EClass.rnd(100))
			{
				return EvadePlus();
			}
		}
		if (EClass.rnd(20) == 0)
		{
			return true;
		}
		if (EClass.rnd(20) == 0)
		{
			return false;
		}
		if (toHit < 1)
		{
			return false;
		}
		if (evasion < 1)
		{
			return true;
		}
		if (EClass.rnd(toHit) < EClass.rnd(evasion * (IsRanged ? 150 : 125) / 100))
		{
			return false;
		}
		if (EClass.rnd(5000) < CC.Evalue(73) + 50)
		{
			return Crit();
		}
		if ((float)CC.Evalue(90) + Mathf.Sqrt(CC.Evalue(134)) > (float)EClass.rnd(200))
		{
			return Crit();
		}
		if (CC.Evalue(1420) > 0)
		{
			int num3 = Mathf.Min(100, 100 - CC.hp * 100 / CC.MaxHP);
			if (num3 >= 50 && num3 * num3 * num3 * num3 / 3 > EClass.rnd(100000000))
			{
				return Crit();
			}
		}
		return true;
	}

	public string GetAttackText(AttackType type, int id)
	{
		return Lang.GetList("attack" + type)[id];
	}
}
