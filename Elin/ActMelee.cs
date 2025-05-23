using System.Collections.Generic;
using UnityEngine;

public class ActMelee : ActBaseAttack
{
	public virtual float BaseDmgMTP => 1f;

	public virtual bool UseWeaponDist => true;

	public override int PerformDistance
	{
		get
		{
			if (Act.CC != null)
			{
				return Act.CC.body.GetMeleeDistance();
			}
			return 1;
		}
	}

	public override bool ResetAxis => true;

	public override bool CanPressRepeat => true;

	public bool HideHint(Card c)
	{
		if (!EClass.pc.isBlind && c != null)
		{
			if (c.isChara)
			{
				return !EClass.pc.CanSee(c);
			}
			return false;
		}
		return true;
	}

	public override string GetHintText(string str = "")
	{
		return "";
	}

	public override string GetTextSmall(Card c)
	{
		if (!HideHint(c))
		{
			return base.GetTextSmall(c);
		}
		return null;
	}

	public override CursorInfo GetCursorIcon(Card c)
	{
		if (!HideHint(c))
		{
			return CursorSystem.IconMelee;
		}
		return null;
	}

	public override bool ShowMouseHint(Card c)
	{
		return !HideHint(c);
	}

	public override bool CanPerform()
	{
		if (Act.TC == null || !Act.TC.IsAliveInCurrentZone)
		{
			return false;
		}
		if (UseWeaponDist && Act.CC.Dist(Act.TC) > PerformDistance)
		{
			return false;
		}
		if (PerformDistance == 1)
		{
			if (!Act.CC.CanInteractTo(Act.TC))
			{
				return false;
			}
		}
		else if (!Act.CC.CanSeeLos(Act.TC))
		{
			return false;
		}
		return base.CanPerform();
	}

	public override bool Perform()
	{
		Act.CurrentAct = this;
		bool result = Attack();
		Act.CurrentAct = null;
		return result;
	}

	public bool Attack(float dmgMulti = 1f, bool maxRoll = false)
	{
		Act.CC.combatCount = 10;
		Act.CC.LookAt(Act.TC);
		Act.CC.renderer.NextFrame();
		if (Act.CC.HasCondition<ConFear>())
		{
			Act.CC.Say("fear", Act.CC, Act.TC);
			if (Act.CC.IsPC)
			{
				EInput.Consume(consumeAxis: true);
			}
			return true;
		}
		Act.CC.renderer.PlayAnime(AnimeID.Attack, Act.TC);
		Act.TC?.Chara?.RequestProtection(Act.CC, delegate(Chara c)
		{
			Act.TC = c;
		});
		Act.TP.Set(Act.TC.pos);
		CellEffect effect = Act.TP.cell.effect;
		if (effect != null && effect.id == 6 && EClass.rnd(2) == 0)
		{
			Act.CC.PlaySound("miss");
			Act.CC.Say("abMistOfDarkness_miss", Act.CC);
			return true;
		}
		bool hasHit = false;
		bool usedWeapon = false;
		bool usedTalisman = false;
		int count = 0;
		int dist = Act.CC.Dist(Act.TC);
		Point orgPos = Act.TC.pos.Copy();
		Card orgTC = Act.TC;
		bool safety = Act.CC.HasElement(486) && Act.CC.IsPCFactionOrMinion;
		foreach (BodySlot slot in Act.CC.body.slots)
		{
			_Attack(slot);
		}
		if (!usedWeapon)
		{
			_Attack(null);
		}
		if (EClass.core.config.game.waitOnMelee)
		{
			EClass.Wait(0.25f, Act.CC);
		}
		if (!hasHit)
		{
			Act.CC.PlaySound("miss");
		}
		if (EClass.rnd(2) == 0)
		{
			Act.CC.RemoveCondition<ConInvisibility>();
		}
		return true;
		void _Attack(BodySlot slot)
		{
			Act.TC = orgTC;
			Act.TP = orgPos;
			Thing w;
			int splash;
			int chaser;
			int flurry;
			int frustration;
			int feint;
			int mod_talisman;
			if (Act.TC != null && Act.TC.IsAliveInCurrentZone)
			{
				w = null;
				if (slot != null)
				{
					if (slot.thing == null || slot.elementId != 35 || slot.thing.source.offense.Length < 2)
					{
						return;
					}
					w = slot.thing;
				}
				int num = 1 + Mathf.Max(Act.CC.Evalue(666), (w != null) ? w.Evalue(666) : 0);
				if (!UseWeaponDist || dist <= 1 || dist <= num)
				{
					int num2 = GetWeaponEnc(606, addSelfEnc: false);
					int scatter = GetWeaponEnc(607, addSelfEnc: false);
					splash = GetWeaponEnc(608, addSelfEnc: true);
					chaser = GetWeaponEnc(620, addSelfEnc: true);
					flurry = GetWeaponEnc(621, addSelfEnc: true);
					frustration = GetWeaponEnc(624, addSelfEnc: true);
					int num3 = GetWeaponEnc(622, addSelfEnc: true);
					feint = GetWeaponEnc(623, addSelfEnc: true);
					mod_talisman = GetWeaponEnc(609, addSelfEnc: true);
					List<Point> list2 = EClass._map.ListPointsInLine(Act.CC.pos, Act.TC.pos, num2 / 10 + ((num2 % 10 > EClass.rnd(10)) ? 1 : 0) + 1);
					if (w != null)
					{
						if (safety)
						{
							if (!w.HasElement(486))
							{
								return;
							}
							num2 = (scatter = (splash = (num3 = 0)));
						}
						usedWeapon = true;
						if (w.IsMeleeWithAmmo && Act.CC.IsPC && w.c_ammo <= 0 && !Act.CC.HasCondition<ConReload>())
						{
							ActRanged.TryReload(w);
						}
					}
					AttackWithFlurry(Act.TC, Act.TP, 1f, subAttack: false);
					if (num2 > 0)
					{
						foreach (Point item in list2)
						{
							if (!item.Equals(orgPos))
							{
								Chara firstChara = item.FirstChara;
								if (firstChara != null && firstChara.IsHostile(Act.CC))
								{
									AttackWithFlurry(firstChara, item, 1f, subAttack: false);
								}
							}
						}
					}
					else if (scatter > 0)
					{
						Act.TP.ForeachNeighbor(delegate(Point p)
						{
							if (!p.Equals(orgPos))
							{
								Chara firstChara2 = p.FirstChara;
								if (firstChara2 != null && firstChara2.IsHostile(Act.CC))
								{
									AttackWithFlurry(firstChara2, p, Mathf.Min(0.5f + 0.05f * Mathf.Sqrt(scatter), 1f + 0.01f * Mathf.Sqrt(scatter)), subAttack: true);
								}
							}
						});
					}
					else if (num3 > 0)
					{
						List<Point> list = new List<Point>();
						Act.TP.ForeachNeighbor(delegate(Point p)
						{
							list.Add(p.Copy());
						});
						list.Shuffle();
						int num4 = 0;
						for (int i = 0; i < 9 && num3 > EClass.rnd(10 + (int)Mathf.Pow(3f, i + 2)); i++)
						{
							num4++;
						}
						foreach (Point item2 in list)
						{
							foreach (Card item3 in item2.ListCards().Copy())
							{
								if (num4 <= 0 || !Act.CC.IsAliveInCurrentZone)
								{
									break;
								}
								if (item3.trait.CanBeAttacked || (item3.isChara && item3.Chara.IsHostile(Act.CC)))
								{
									AttackWithFlurry(item3, item2, 1f, subAttack: true);
									num4--;
								}
							}
						}
					}
					count++;
				}
			}
			void Attack(Card _tc, Point _tp, float mtp, bool subAttack)
			{
				Act.TC = _tc;
				Act.TP = _tp;
				AttackProcess.Current.Prepare(Act.CC, w, Act.TC, Act.TP, count);
				int num5 = 1;
				if (chaser > 0)
				{
					for (int j = 0; j < 10; j++)
					{
						if (chaser > EClass.rnd(4 + (int)Mathf.Pow(4f, j + 2)))
						{
							num5++;
						}
					}
				}
				bool flag = false;
				for (int k = 0; k < num5; k++)
				{
					if (k > 0)
					{
						Act.CC.Say("attack_chaser");
					}
					flag = AttackProcess.Current.Perform(count, hasHit, dmgMulti * mtp * BaseDmgMTP, maxRoll, subAttack);
					if (!flag && frustration > 0 && 10f + 2f * Mathf.Sqrt(frustration) > (float)EClass.rnd(100))
					{
						AttackProcess.Current.critFury = true;
						flag = AttackProcess.Current.Perform(count, hasHit, dmgMulti * mtp, maxRoll, subAttack);
						AttackProcess.Current.critFury = false;
					}
					if (flag || !Act.CC.IsAliveInCurrentZone || !Act.TC.IsAliveInCurrentZone)
					{
						break;
					}
				}
				Act.TC = _tc;
				Act.TP = _tp;
				if (w != null && w.c_ammo > 0 && !Act.CC.HasCondition<ConReload>() && !safety)
				{
					bool flag2 = true;
					TraitAmmo traitAmmo = ((w.ammoData == null) ? null : (w.ammoData.trait as TraitAmmo));
					if (traitAmmo != null && traitAmmo is TraitAmmoTalisman traitAmmoTalisman)
					{
						flag2 = false;
						if (flag && !usedTalisman && Act.TC != null && Act.TC.IsAliveInCurrentZone)
						{
							Act act = Act.CC.elements.GetElement(traitAmmoTalisman.owner.refVal)?.act ?? ACT.Create(traitAmmoTalisman.owner.refVal);
							Act.powerMod = traitAmmo.owner.encLV;
							if (act.Perform(Act.CC, Act.TC, Act.TP))
							{
								usedTalisman = true;
								flag2 = true;
								if (mod_talisman > 0 && Mathf.Min(10f + Mathf.Sqrt(mod_talisman) * 5f, 90f) > (float)EClass.rnd(100))
								{
									flag2 = false;
								}
								int spellExp = Act.CC.elements.GetSpellExp(Act.CC, act, 200);
								Act.CC.ModExp(act.id, spellExp);
							}
							Act.powerMod = 100;
						}
					}
					if (flag2)
					{
						w.c_ammo--;
						if (w.ammoData != null)
						{
							w.ammoData.Num = w.c_ammo;
						}
						if (w.c_ammo <= 0)
						{
							w.c_ammo = 0;
							w.ammoData = null;
						}
						LayerInventory.SetDirty(w);
					}
				}
				Act.TC = _tc;
				if (Act.TC.IsAliveInCurrentZone && Act.TC.isChara)
				{
					Act.CC.DoHostileAction(Act.TC);
					if (feint > 0 && 10f + 4f * Mathf.Sqrt(feint) > (float)EClass.rnd(100))
					{
						Act.TC.Chara.AddCondition<ConSupress>(100 + 5 * (int)Mathf.Sqrt(feint));
					}
				}
				if (splash > 0)
				{
					Act.TP.ForeachNeighbor(delegate(Point p)
					{
						if (p.Equals(Act.TP) || p.Equals(Act.CC.pos))
						{
							return;
						}
						if (!p.IsBlocked)
						{
							p.PlayEffect("smoke_shockwave");
							p.Copy().Animate(AnimeID.QuakeMini, animeBlock: true);
						}
						foreach (Card item4 in p.ListCards())
						{
							if (item4.trait.CanBeAttacked || (item4.isChara && item4.Chara.IsHostile(Act.CC)))
							{
								int rawDamage = AttackProcess.Current.GetRawDamage(0.1f + 0.05f * Mathf.Sqrt(splash), crit: false, maxRoll: false);
								rawDamage = item4.ApplyProtection(rawDamage);
								item4.DamageHP(rawDamage, 0, 100, AttackSource.Shockwave, Act.CC);
							}
						}
					});
				}
			}
			void AttackWithFlurry(Card _tc, Point _tp, float mtp, bool subAttack)
			{
				int num6 = 1;
				if (flurry > 0)
				{
					for (int l = 0; l < 10 && flurry > EClass.rnd(25 + (int)Mathf.Pow(5f, l + 2)); l++)
					{
						num6++;
					}
				}
				for (int m = 0; m < num6; m++)
				{
					if (!Act.CC.IsAliveInCurrentZone)
					{
						break;
					}
					if (!_tc.IsAliveInCurrentZone)
					{
						break;
					}
					if (m > 0)
					{
						Act.CC.Say("attack_flurry");
					}
					Attack(_tc, _tp, mtp, subAttack);
				}
			}
			int GetWeaponEnc(int ele, bool addSelfEnc)
			{
				return AttackProcess.GetWeaponEnc(Act.CC, w, ele, addSelfEnc);
			}
		}
	}
}
