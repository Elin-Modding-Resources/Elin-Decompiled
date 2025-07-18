using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActRanged : ActThrow
{
	public bool IsReload
	{
		get
		{
			if (Act.CC.IsPC && Act.TC == Act.CC && !Act.CC.HasCondition<ConReload>() && EClass.player.currentHotItem.Thing != null)
			{
				return !(EClass.player.currentHotItem.Thing.trait is TraitToolRangeCane);
			}
			return false;
		}
	}

	public override CursorInfo CursorIcon => CursorSystem.IconRange;

	public override int PerformDistance => 99;

	public override string Name
	{
		get
		{
			if (!IsReload)
			{
				return base.Name;
			}
			return "ActReload".lang();
		}
	}

	public override string GetText(string str = "")
	{
		if (!IsReload)
		{
			return base.GetText(str);
		}
		return Name;
	}

	public override bool CanPerform()
	{
		if (Act.CC.isRestrained)
		{
			return false;
		}
		if (Act.CC.HasCondition<ConReload>())
		{
			return false;
		}
		if (Act.TC == null)
		{
			return Act.CC.CanSeeLos(Act.TP);
		}
		if (!Act.TC.IsAliveInCurrentZone)
		{
			return false;
		}
		return Act.CC.CanSeeLos(Act.TC.pos);
	}

	public override bool Perform()
	{
		if (Act.TC == null)
		{
			Act.TC = Act.TP.FindAttackTarget();
		}
		if (Act.TC != null)
		{
			Act.TP.Set(Act.TC.pos);
		}
		if (Act.TC != null && Act.CC == EClass.pc && !(Act.CC.ai is AI_PracticeDummy) && (Act.TC.trait is TraitTrainingDummy || Act.TC.IsRestrainedResident))
		{
			Act.CC.SetAI(new AI_PracticeDummy
			{
				target = Act.TC,
				range = true
			});
			return true;
		}
		Act.CC.renderer.NextFrame();
		if (!Act.CC.IsPC)
		{
			Act.CC.TryEquipRanged();
		}
		Thing weapon = Act.CC.ranged;
		if (weapon != null && Act.CC.IsPC && Act.CC.body.IsTooHeavyToEquip(weapon))
		{
			Msg.Say("tooHeavyToEquip", weapon);
			return false;
		}
		if (weapon == null || !weapon.CanAutoFire(Act.CC, Act.TC))
		{
			return false;
		}
		bool flag = weapon.trait is TraitToolRangeGun;
		bool flag2 = weapon.trait is TraitToolRangeCane;
		GameSetting.EffectData effectData = EClass.setting.effect.guns.TryGetValue(weapon.id) ?? EClass.setting.effect.guns[flag2 ? "cane" : (flag ? "gun" : "bow")];
		bool hasHit = false;
		int numFire = effectData.num;
		int numFireWithoutDamageLoss = numFire;
		int num = GetWeaponEnc(602);
		int drill = GetWeaponEnc(606);
		int scatter = GetWeaponEnc(607);
		int chaser = GetWeaponEnc(620);
		if (num > 0)
		{
			numFire += num / 10 + ((num % 10 > EClass.rnd(10)) ? 1 : 0);
		}
		numFire += Act.CC.Evalue(1652);
		int num2 = numFire;
		int num3 = 1 + weapon.material.hardness / 30 + EClass.rnd(3);
		int num4 = weapon.Evalue(604);
		if (num4 > 0)
		{
			for (int i = 0; i < numFire; i++)
			{
				if (Mathf.Sqrt(num4) * 5f + 10f > (float)EClass.rnd(100))
				{
					num2--;
				}
			}
			num3 = Mathf.Max(1, num3 * 100 / (100 + num4 * 5));
		}
		string missSound = ((weapon.trait is TraitToolRangeGun) ? "miss_bullet" : "miss_arrow");
		if (weapon.trait is TraitToolRangeCane)
		{
			foreach (Element item in weapon.elements.dict.Values.Where((Element e) => e.source.categorySub == "eleAttack"))
			{
				num3 += item.source.LV / 15;
			}
			if (Act.CC.IsPC)
			{
				if (Act.CC.mana.value < num3)
				{
					if (!Act.CC.ai.IsNoGoal)
					{
						return false;
					}
					if (!Dialog.warned && !EClass.debug.godMode)
					{
						ActPlan.warning = true;
						Dialog.TryWarnMana(delegate
						{
							if (Perform())
							{
								EClass.player.EndTurn();
							}
						});
						return false;
					}
				}
				EClass.ui.CloseLayers();
			}
		}
		else
		{
			if (IsReload)
			{
				return TryReload(weapon);
			}
			if (weapon.c_ammo <= 0)
			{
				if (!TryReload(weapon))
				{
					if (Act.CC.IsPC)
					{
						EInput.Consume();
					}
					return false;
				}
				if (Act.CC.HasCondition<ConReload>())
				{
					return true;
				}
			}
			if (Act.CC.HasCondition<ConFear>())
			{
				Act.CC.Say("fear", Act.CC, Act.TC);
				if (Act.CC.IsPC)
				{
					EInput.Consume(consumeAxis: true);
				}
				return true;
			}
		}
		Act.TC?.Chara?.RequestProtection(Act.CC, delegate(Chara c)
		{
			Act.TC = c;
		});
		Act.CC.LookAt(Act.TP);
		int index = 0;
		Point orgTP = Act.TP.Copy();
		List<Point> points = new List<Point>();
		if (drill > 0)
		{
			points = EClass._map.ListPointsInLine(Act.CC.pos, Act.TP, drill / 10 + ((drill % 10 > EClass.rnd(10)) ? 1 : 0) + 1);
		}
		else if (scatter > 0)
		{
			Act.TP.ForeachNeighbor(delegate(Point _p)
			{
				points.Add(_p.Copy());
			});
		}
		if (EClass.core.config.game.waitOnRange)
		{
			EClass.Wait(0.25f, Act.CC);
		}
		Shoot(Act.TC, Act.TP);
		if (points.Count > 0)
		{
			Point obj = Act.TP.Copy();
			foreach (Point item2 in points)
			{
				if (!item2.Equals(obj))
				{
					Chara firstChara = item2.FirstChara;
					if ((firstChara == null || firstChara.IsHostile(Act.CC)) && (firstChara != null || scatter != 0))
					{
						Shoot(item2.FirstChara, item2);
					}
				}
			}
		}
		if (!(weapon.trait is TraitToolRangeCane))
		{
			weapon.c_ammo -= num2;
			if (weapon.ammoData != null)
			{
				weapon.ammoData.Num = weapon.c_ammo;
			}
			if (weapon.c_ammo <= 0)
			{
				weapon.c_ammo = 0;
				weapon.ammoData = null;
			}
		}
		if (Act.CC.IsPC)
		{
			LayerInventory.SetDirty(weapon);
		}
		if (Act.TC != null && !hasHit)
		{
			Act.CC.PlaySound(missSound);
		}
		if (!Act.CC.isDead)
		{
			if (EClass.rnd(2) == 0)
			{
				Act.CC.RemoveCondition<ConInvisibility>();
			}
			if (weapon.trait is TraitToolRangeCane)
			{
				Act.CC.mana.Mod(-num3 * numFire);
			}
		}
		return true;
		int GetWeaponEnc(int ele)
		{
			return weapon.Evalue(ele) + (Act.CC.IsPCFactionOrMinion ? EClass.pc.faction.charaElements.Value(ele) : 0);
		}
		void Prepare()
		{
			AttackProcess.Current.Prepare(Act.CC, weapon, Act.TC, Act.TP);
			AttackProcess.Current.numFire = numFire;
			AttackProcess.Current.numFireWithoutDamageLoss = numFireWithoutDamageLoss;
			AttackProcess.Current.posRangedAnime = Act.TP.Copy();
			AttackProcess.Current.ignoreAnime = index > 1;
			AttackProcess.Current.ignoreAttackSound = false;
			if (drill > 0 && points.Count > 0)
			{
				AttackProcess.Current.posRangedAnime = points.LastItem();
			}
			else if (scatter > 0)
			{
				AttackProcess.Current.ignoreAnime = false;
				AttackProcess.Current.ignoreAttackSound = index > 1;
			}
		}
		void Shoot(Card _tc, Point _tp)
		{
			float dmgMulti = 1f;
			index++;
			Act.TC = _tc;
			Act.TP = _tp;
			CellEffect effect = Act.TP.cell.effect;
			if (effect != null && effect.id == 6 && EClass.rnd(2) == 0)
			{
				Prepare();
				AttackProcess.Current.PlayRangedAnime(numFire);
				Act.CC.PlaySound(missSound);
				Act.CC.Say("abMistOfDarkness_miss", Act.CC);
			}
			else
			{
				if (scatter > 0)
				{
					dmgMulti = Mathf.Clamp(1.2f - 0.2f * (float)Act.CC.Dist(Act.TP) - (Act.TP.Equals(orgTP) ? 0f : 0.4f), 0.2f, 1f);
				}
				for (int j = 0; j < numFire; j++)
				{
					Act.TC = _tc;
					Prepare();
					if (AttackProcess.Current.Perform(j, hasHit, dmgMulti))
					{
						hasHit = true;
					}
					else if (chaser > 0)
					{
						for (int k = 0; k < 10; k++)
						{
							if (chaser > EClass.rnd(4 + (int)Mathf.Pow(4f, k + 2 + j)))
							{
								Act.CC.Say("attack_chaser");
								if (AttackProcess.Current.Perform(j, hasHit, dmgMulti))
								{
									hasHit = true;
									break;
								}
							}
						}
					}
					if (Act.TC == null || !Act.TC.IsAliveInCurrentZone)
					{
						break;
					}
				}
				if (Act.TC != null)
				{
					Act.CC.DoHostileAction(Act.TC);
				}
			}
		}
	}

	public static bool TryReload(Thing weapon, Thing ammo = null)
	{
		TraitToolRange traitToolRange = weapon.trait as TraitToolRange;
		if (Act.CC.IsPC)
		{
			LayerInventory.SetDirty(weapon);
		}
		if (weapon.ammoData != null)
		{
			if (weapon.ammoData.Num > 0)
			{
				Act.CC.Pick(weapon.ammoData);
			}
			weapon.ammoData = null;
		}
		int num = 0;
		if (ammo == null)
		{
			ammo = Act.CC.FindAmmo(weapon);
		}
		if (ammo == null)
		{
			if (Act.CC.IsPC)
			{
				if (!weapon.IsMeleeWithAmmo)
				{
					Msg.Say("noAmmo", weapon);
				}
				return false;
			}
			num = traitToolRange.MaxAmmo;
		}
		else
		{
			num = Mathf.Min(ammo.Num, traitToolRange.MaxAmmo);
			Thing thing = ammo.Split(num);
			Act.CC.Say("takeAmmo", thing);
			if (thing.GetRootCard() == Act.CC)
			{
				thing.parent.RemoveCard(thing);
			}
			weapon.ammoData = thing;
		}
		weapon.c_ammo = num;
		int reloadTurn = traitToolRange.ReloadTurn;
		reloadTurn = reloadTurn * 100 / (100 + Act.CC.Evalue(1652) * 100);
		if (traitToolRange.NeedReload && reloadTurn > 0)
		{
			Act.CC.AddCondition<ConReload>(reloadTurn * 10);
		}
		return true;
	}
}
