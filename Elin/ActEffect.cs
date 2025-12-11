using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActEffect : EClass
{
	private class WishItem
	{
		public string n;

		public int score;

		public Action action;
	}

	public static int RapidCount;

	public static float RapidDelay;

	public static int angle = 20;

	public static void TryDelay(Action a)
	{
		if (RapidCount == 0)
		{
			a();
			return;
		}
		TweenUtil.Delay((float)RapidCount * RapidDelay, delegate
		{
			a();
		});
	}

	public static bool DamageEle(Card CC, EffectId id, int power, Element e, List<Point> points, ActRef actref, string lang = null)
	{
		if (points.Count == 0)
		{
			CC.SayNothingHappans();
			return false;
		}
		if (!EClass.setting.elements.ContainsKey(e.source.alias))
		{
			Debug.Log(e.source.alias);
			e = Element.Create(0, 1);
		}
		ElementRef elementRef = EClass.setting.elements[e.source.alias];
		int num = actref.act?.ElementPowerMod ?? 50;
		int num2 = 0;
		Point point = CC.pos.Copy();
		List<Card> list = new List<Card>();
		bool flag = false;
		if ((id == EffectId.Explosive || id == EffectId.Rocket || id == EffectId.GravityGun) && actref.refThing != null)
		{
			power = power * actref.refThing.material.hardness / 10;
		}
		string text = id.ToString();
		string text2 = (EClass.sources.calc.map.ContainsKey(text) ? text : (EClass.sources.calc.map.ContainsKey("Sp" + text) ? ("Sp" + text) : (text.ToLowerInvariant() + "_")));
		foreach (Point p in points)
		{
			bool flag2 = true;
			AttackSource attackSource = AttackSource.None;
			switch (id)
			{
			case EffectId.Explosive:
			case EffectId.Rocket:
			case EffectId.GravityGun:
				text2 = "ball_";
				flag = false;
				break;
			case EffectId.BallBubble:
				text2 = "ball_";
				break;
			case EffectId.Earthquake:
				text2 = "SpEarthquake";
				flag2 = false;
				flag = true;
				break;
			case EffectId.Meteor:
				text2 = "SpMeteor";
				break;
			default:
				if (CC.isChara && p.Equals(CC.pos) && points.Count >= 2)
				{
					continue;
				}
				break;
			case EffectId.Suicide:
				break;
			}
			Effect effect = null;
			Effect effect2 = (flag2 ? Effect.Get("trail1") : null);
			Point from = p;
			switch (id)
			{
			case EffectId.Arrow:
			case EffectId.MoonSpear:
			case EffectId.MoonArrow:
			{
				effect = Effect.Get((id == EffectId.MoonSpear || id == EffectId.MoonArrow) ? "spell_moonspear" : "spell_arrow");
				if (id == EffectId.Arrow)
				{
					effect.sr.color = elementRef.colorSprite;
				}
				TrailRenderer componentInChildren = effect.GetComponentInChildren<TrailRenderer>();
				Color startColor = (componentInChildren.endColor = elementRef.colorSprite);
				componentInChildren.startColor = startColor;
				from = CC.pos;
				break;
			}
			case EffectId.Earthquake:
			{
				if (EClass.rnd(4) == 0 && p.IsSync)
				{
					effect = Effect.Get("smoke_earthquake");
				}
				float num3 = 0.06f * (float)CC.pos.Distance(p);
				Point pos = p.Copy();
				TweenUtil.Tween(num3, null, delegate
				{
					pos.Animate(AnimeID.Quake, animeBlock: true);
				});
				if (effect != null)
				{
					effect.SetStartDelay(num3);
				}
				break;
			}
			default:
			{
				effect = Effect.Get("Element/ball_" + ((e.id == 0) ? "Void" : ((id == EffectId.GravityGun) ? "Gravity" : e.source.alias.Remove(0, 3))));
				if (effect == null)
				{
					effect = Effect.Get("Element/ball_Fire");
				}
				float startDelay = ((id == EffectId.Meteor) ? 0.1f : 0.04f) * (float)CC.pos.Distance(p);
				effect.SetStartDelay(startDelay);
				effect2.SetStartDelay(startDelay);
				if (id == EffectId.GravityGun)
				{
					float duration = 0.06f * (float)CC.pos.Distance(p);
					Point pos2 = p.Copy();
					TweenUtil.Tween(duration, null, delegate
					{
						pos2.Animate(AnimeID.Gravity, animeBlock: true);
					});
				}
				break;
			}
			}
			if (effect2 != null)
			{
				effect2.SetParticleColor(elementRef.colorTrail, changeMaterial: true, "_TintColor").Play(from);
			}
			if (effect != null)
			{
				if (id == EffectId.Arrow || id == EffectId.MoonSpear || id == EffectId.MoonArrow)
				{
					TryDelay(delegate
					{
						effect.Play(CC.pos, 0f, p);
					});
				}
				else
				{
					TryDelay(delegate
					{
						effect.Play(p).Flip(p.x > CC.pos.x);
					});
				}
			}
			bool flag3 = false;
			if (CC.IsPCFactionOrMinion && (CC.HasElement(1651) || EClass.pc.Evalue(1651) >= 2))
			{
				bool flag4 = false;
				foreach (Card item in p.ListCards())
				{
					if (item.isChara)
					{
						if (item.IsPCFactionOrMinion)
						{
							flag4 = true;
						}
					}
					else if ((e.id != 910 && e.id != 911) || !item.IsFood || !item.category.IsChildOf("foodstuff"))
					{
						flag4 = true;
					}
				}
				flag3 = flag4;
			}
			if (!flag3)
			{
				if (e.id == 910)
				{
					EClass._map.TryShatter(p, 910, power);
				}
				if (e.id == 911)
				{
					EClass._map.TryShatter(p, 911, power);
				}
			}
			foreach (Card item2 in p.ListCards().ToList())
			{
				Card c = item2;
				if ((!c.isChara && !c.trait.CanBeAttacked) || (c.IsMultisize && item2 == CC) || (c.isChara && (c.Chara.host == CC || c.Chara.parasite == CC || c.Chara.ride == CC)))
				{
					continue;
				}
				switch (id)
				{
				case EffectId.Arrow:
				case EffectId.Hand:
				case EffectId.Sword:
				case EffectId.MoonSpear:
				case EffectId.MoonArrow:
					if (c.isChara && CC.isChara)
					{
						c.Chara.RequestProtection(CC.Chara, delegate(Chara a)
						{
							c = a;
						});
					}
					if (id == EffectId.MoonSpear || (id == EffectId.MoonArrow && EClass.rnd(5) == 0))
					{
						attackSource = AttackSource.MoonSpear;
					}
					break;
				}
				switch (id)
				{
				case EffectId.Arrow:
					attackSource = AttackSource.MagicArrow;
					break;
				case EffectId.Hand:
					attackSource = AttackSource.MagicHand;
					break;
				case EffectId.Sword:
					attackSource = AttackSource.MagicSword;
					break;
				}
				long num4 = 0L;
				bool isChara = CC.isChara;
				if (id == EffectId.Suicide)
				{
					num4 = CC.MaxHP * 2;
					num4 = num4 * 100 / (50 + point.Distance(p) * 75);
					if ((c.HasCondition<ConBrightnessOfLife>() || c.HasTag(CTAG.suicide)) && !c.HasCondition<ConWet>() && !c.IsPowerful)
					{
						list.Add(c);
					}
				}
				else
				{
					Dice dice = Dice.Create(text2, power, CC, (actref.refThing != null) ? null : actref.act);
					if (dice == null)
					{
						Debug.Log(text2);
					}
					num4 = dice.Roll();
					switch (id)
					{
					case EffectId.Earthquake:
						if (c.HasCondition<ConGravity>())
						{
							num4 = dice.RollMax() * 2;
						}
						else if (c.isChara && c.Chara.IsLevitating)
						{
							num4 /= 2;
						}
						break;
					case EffectId.Sword:
						num4 = num4 * (int)Mathf.Min(70f + Mathf.Sqrt(CC.Evalue(101)) * 3f, 200f) / 100;
						break;
					case EffectId.GravityGun:
						num4 /= 5;
						break;
					}
					if (id == EffectId.Ball || id == EffectId.BallBubble || id == EffectId.Explosive || id == EffectId.Rocket || id == EffectId.GravityGun)
					{
						num4 = num4 * 100 / (90 + point.Distance(p) * 10);
					}
				}
				if (id == EffectId.Sword)
				{
					c.PlaySound("ab_magicsword");
					c.PlayEffect("hit_slash");
				}
				if ((actref.noFriendlyFire && !CC.Chara.IsHostile(c as Chara)) || (flag && c == CC))
				{
					continue;
				}
				if (isChara && points.Count > 1 && c != null && c.isChara && CC.isChara && CC.Chara.IsFriendOrAbove(c.Chara))
				{
					int num5 = CC.Evalue(302);
					if (!CC.IsPC && CC.IsPCFactionOrMinion)
					{
						num5 += EClass.pc.Evalue(302);
					}
					if (CC.HasElement(1214))
					{
						num5 *= 2;
					}
					if (num5 > 0)
					{
						if (num5 * 10 > EClass.rnd(num4 + 1))
						{
							if (c == c.pos.FirstChara)
							{
								CC.ModExp(302, CC.IsPC ? 10 : 50);
							}
							continue;
						}
						num4 = EClass.rnd(num4 * 100 / (100 + num5 * 10 + 1));
						if (c == c.pos.FirstChara)
						{
							CC.ModExp(302, CC.IsPC ? 20 : 100);
						}
						if (num4 == 0L)
						{
							continue;
						}
					}
					if ((CC.HasElement(1214) || (!CC.IsPC && (CC.IsPCFaction || CC.IsPCFactionMinion) && EClass.pc.HasElement(1214))) && EClass.rnd(5) != 0)
					{
						continue;
					}
				}
				if (!lang.IsEmpty())
				{
					if (lang == "spell_hand")
					{
						string[] list2 = Lang.GetList("attack" + (CC.isChara ? CC.Chara.race.meleeStyle.IsEmpty("Touch") : "Touch"));
						string @ref = "_elehand".lang(e.source.GetAltname(2), list2[4]);
						CC.Say(c.IsPCParty ? "cast_hand_ally" : "cast_hand", CC, c, @ref, c.IsPCParty ? list2[1] : list2[2]);
					}
					else
					{
						CC.Say(lang + "_hit", CC, c, e.Name.ToLower());
					}
				}
				Chara chara = (CC.isChara ? CC.Chara : ((actref.refThing != null) ? EClass._map.FindChara(actref.refThing.c_uidRefCard) : null));
				if (c.IsMultisize)
				{
					switch (id)
					{
					case EffectId.Ball:
					case EffectId.Explosive:
					case EffectId.BallBubble:
					case EffectId.Meteor:
					case EffectId.Earthquake:
					case EffectId.Suicide:
					case EffectId.Rocket:
					case EffectId.Flare:
					case EffectId.GravityGun:
						num4 /= 2;
						break;
					}
				}
				if (RapidCount > 0)
				{
					num4 = num4 * 100 / (100 + RapidCount * 50);
				}
				num4 = num4 * Act.powerMod / 100;
				if (num4 > 99999999)
				{
					num4 = 99999999L;
				}
				c.DamageHP(num4, e.id, power * num / 100, attackSource, chara ?? CC);
				if (c.IsAliveInCurrentZone)
				{
					switch (id)
					{
					case EffectId.GravityGun:
						if (c.isChara)
						{
							AddCon<ConGravity>(1, power);
							AddCon<ConBlind>(4, power);
							AddCon<ConDim>(5, power / 2);
							AddCon<ConSupress>(3, power / 2);
							if (actref.refThing != null && actref.refThing.id == "gun_gravity2")
							{
								AddCon<ConEntangle>(4, power / 3);
								AddCon<ConSilence>(4, power / 3);
								AddCon<ConWeakResEle>(4, power);
								AddCon<ConNightmare>(4, power);
							}
						}
						break;
					case EffectId.DrainMana:
						if (CC.IsAliveInCurrentZone && c.isChara && CC.isChara && c.Chara.mana.value > 0)
						{
							long num6 = num4 * num / 100;
							Debug.Log(num4 + " v:" + num6 + " evalue:" + e.Value + " power:" + power + " elepMod:" + num);
							if (num6 > c.Chara.mana.value)
							{
								num6 = c.Chara.mana.value;
							}
							c.Chara.mana.Mod((int)(-num6));
							CC.Chara.mana.Mod((int)num6);
						}
						break;
					}
				}
				if (id == EffectId.Explosive && CC.trait is TraitCookerMicrowave)
				{
					chara = EClass.pc;
				}
				if (chara != null && chara.IsAliveInCurrentZone)
				{
					chara.DoHostileAction(c);
				}
				num2++;
				void AddCon<T>(int rate, int power) where T : Condition
				{
					if (EClass.rnd(1 + actref.refVal) == 0 && !c.Chara.HasCondition<T>() && EClass.rnd(rate) == 0)
					{
						c.Chara.AddCondition<T>(power);
					}
				}
			}
			if ((id == EffectId.Explosive || id == EffectId.Suicide || id == EffectId.Rocket) && (!EClass._zone.IsPCFaction || !EClass.Branch.HasItemProtection))
			{
				int num7 = id switch
				{
					EffectId.Suicide => CC.LV / 3 + 40, 
					EffectId.Meteor => 50 + power / 20, 
					_ => (actref.refThing != null) ? actref.refThing.material.hardness : (30 + power / 20), 
				};
				bool flag5 = EClass._zone.HasLaw && !EClass._zone.IsPCFaction && (CC.IsPC || (id == EffectId.Explosive && actref.refThing == null)) && !(EClass._zone is Zone_Vernis);
				if (p.HasObj && p.cell.matObj.hardness <= num7)
				{
					EClass._map.MineObj(p);
					if (flag5)
					{
						EClass.player.ModKarma(-1);
					}
				}
				if (!p.HasObj && p.HasBlock && p.matBlock.hardness <= num7)
				{
					EClass._map.MineBlock(p);
					if (flag5)
					{
						EClass.player.ModKarma(-1);
					}
				}
			}
			if (e.id == 910)
			{
				int num8 = 0;
				if (id == EffectId.Meteor)
				{
					num8 = 2;
				}
				if (EClass._zone.IsPCFaction && EClass._zone.branch.HasItemProtection)
				{
					num8 = 0;
				}
				if (num8 > EClass.rnd(10))
				{
					p.ModFire(4 + EClass.rnd(10));
				}
			}
			if (e.id == 911)
			{
				p.ModFire(-20, extinguish: true);
			}
		}
		if (RapidCount == 0)
		{
			foreach (Card item3 in list)
			{
				if (item3.ExistsOnMap)
				{
					RapidCount += 2;
					ProcAt(id, power, BlessedState.Normal, item3, null, item3.pos, isNeg: true, actref);
				}
			}
		}
		return num2 > 0;
	}

	public static void ProcAt(EffectId id, int power, BlessedState state, Card cc, Card tc, Point tp, bool isNeg, ActRef actRef = default(ActRef))
	{
		Chara CC = cc.Chara;
		bool flag = state <= BlessedState.Cursed;
		bool flag2 = isNeg || flag;
		Element element = Element.Create(actRef.aliasEle.IsEmpty("eleFire"), power / 10);
		if (EClass.debug.enable && EInput.isShiftDown)
		{
			angle += 5;
			if (angle > 100)
			{
				angle = 30;
			}
			Debug.Log(angle);
		}
		switch (id)
		{
		case EffectId.Earthquake:
		{
			List<Point> list = EClass._map.ListPointsInCircle(CC.pos, 12f, mustBeWalkable: false);
			if (list.Count == 0)
			{
				list.Add(CC.pos.Copy());
			}
			CC.Say("spell_earthquake", CC, element.Name.ToLower());
			TryDelay(delegate
			{
				CC.PlaySound("spell_earthquake");
			});
			if (CC.IsInMutterDistance())
			{
				Shaker.ShakeCam("ball");
			}
			EClass.Wait(1f, CC);
			DamageEle(CC, id, power, element, list, actRef, "spell_earthquake");
			break;
		}
		case EffectId.Meteor:
		{
			EffectMeteor.Create(cc.pos, 6, 10, delegate
			{
			});
			List<Point> list3 = EClass._map.ListPointsInCircle(CC.pos, 10f);
			if (list3.Count == 0)
			{
				list3.Add(CC.pos.Copy());
			}
			CC.Say("spell_ball", CC, element.Name.ToLower());
			TryDelay(delegate
			{
				CC.PlaySound("spell_ball");
			});
			if (CC.IsInMutterDistance())
			{
				Shaker.ShakeCam("ball");
			}
			EClass.Wait(1f, CC);
			DamageEle(CC, id, power, element, list3, actRef, "spell_ball");
			return;
		}
		case EffectId.Hand:
		case EffectId.DrainBlood:
		case EffectId.DrainMana:
		case EffectId.Sword:
		{
			List<Point> list6 = new List<Point>();
			list6.Add(tp.Copy());
			EClass.Wait(0.3f, CC);
			TryDelay(delegate
			{
				CC.PlaySound("spell_hand");
			});
			if (!DamageEle(CC, id, power, element, list6, actRef, (id == EffectId.DrainBlood || id == EffectId.DrainMana) ? "" : ((id == EffectId.Sword) ? "spell_sword" : "spell_hand")))
			{
				CC.Say("spell_hand_miss", CC, element.Name.ToLower());
			}
			return;
		}
		case EffectId.Arrow:
		case EffectId.MoonSpear:
		case EffectId.MoonArrow:
		{
			List<Point> list7 = new List<Point>();
			list7.Add(tp.Copy());
			CC.Say((id == EffectId.MoonSpear) ? "spell_spear" : "spell_arrow", CC, element.Name.ToLower());
			EClass.Wait(0.5f, CC);
			TryDelay(delegate
			{
				CC.PlaySound((id == EffectId.MoonSpear) ? "spell_moonspear" : "spell_arrow");
			});
			DamageEle(CC, id, power, element, list7, actRef, (id == EffectId.MoonSpear) ? "spell_spear" : "spell_arrow");
			return;
		}
		case EffectId.Summon:
		{
			string n = actRef.n1;
			if (!(n == "special"))
			{
				if (n == "special2")
				{
					if (EClass._zone.HasField(10000))
					{
						foreach (Chara item in EClass._map.charas.Where((Chara _c) => _c.id == "cocoon").ToList())
						{
							item.pos.PlayEffect("darkwomb3");
							item.HatchEgg();
						}
					}
					CC.PlayEffect("darkwomb");
					SE.Play("ab_womb");
				}
				else
				{
					CC.Say("summon_ally", CC);
				}
			}
			else
			{
				SE.Play("warhorn");
				Msg.Say("warhorn");
			}
			if (EClass._zone.CountMinions(CC) >= CC.MaxSummon || CC.c_uidMaster != 0)
			{
				CC.Say("summon_ally_fail", CC);
				return;
			}
			string id3 = actRef.n1;
			int num3 = 1;
			int num4 = -1;
			int radius = 3;
			bool flag3 = false;
			bool flag4 = actRef.n1 == "special";
			int num5 = -1;
			string text = "";
			switch (actRef.n1)
			{
			case "shadow":
			case "tsunami":
				num3 = Mathf.Clamp(power / 100, 1, 5) + ((power >= 100) ? EClass.rnd(2) : 0);
				break;
			case "monster":
			case "fire":
			case "animal":
				num3 = 1 + EClass.rnd(2);
				break;
			case "special_force":
				id3 = "army_palmia";
				num3 = 4 + EClass.rnd(2);
				num5 = EClass._zone.DangerLv;
				break;
			case "tentacle":
				num4 = 20 + EClass.rnd(10);
				radius = 1;
				break;
			case "special":
				CC.SetInt(70, EClass.world.date.GetRaw() + 1440);
				num3 = Mathf.Clamp(7 + CC.LV / 100, 4, 20);
				num5 = CC.LV;
				break;
			case "special2":
				num3 = 30;
				num5 = CC.LV;
				break;
			}
			num3 += CC.Evalue(1240);
			for (int j = 0; j < num3; j++)
			{
				if (EClass._zone.CountMinions(CC) >= CC.MaxSummon)
				{
					break;
				}
				Point point = null;
				point = ((!(actRef.n1 == "special2")) ? tp.GetRandomPoint(radius)?.GetNearestPoint(allowBlock: false, allowChara: false) : EClass._map.GetRandomSurface(centered: false, walkable: true, allowWater: true)?.GetNearestPoint(allowBlock: false, allowChara: false));
				if (point == null || !point.IsValid)
				{
					continue;
				}
				Chara chara = null;
				CardBlueprint.Set(new CardBlueprint());
				if (num5 != -1)
				{
					CardBlueprint.current.lv = num5;
				}
				if (!text.IsEmpty())
				{
					CardBlueprint.current.idEle = text;
				}
				switch (actRef.n1)
				{
				case "special":
					if (j == 0 && !CC.HasMinion("imolonac") && !CC.HasMinion("ygolonac"))
					{
						chara = CharaGen.Create((EClass.rnd(20) == 0) ? "imolonac" : "ygolonac");
						break;
					}
					chara = CharaGen.Create("hound", CC.LV);
					if (text.IsEmpty())
					{
						text = chara.MainElement.source.alias;
					}
					break;
				case "special2":
					chara = CharaGen.Create("cocoon");
					break;
				case "yeek":
					chara = CharaGen.CreateFromFilter(SpawnListChara.Get("summon_yeek", (SourceChara.Row r) => r.race == "yeek"), power / 10);
					break;
				case "orc":
					chara = CharaGen.CreateFromFilter(SpawnListChara.Get("summon_orc", (SourceChara.Row r) => r.race == "orc"), power / 10);
					break;
				case "dragon":
					chara = CharaGen.CreateFromFilter(SpawnListChara.Get("summon_dragon", (SourceChara.Row r) => r.race == "dragon" || r.race == "drake" || r.race == "wyvern"), power / 5);
					break;
				case "undead":
					chara = CharaGen.CreateFromFilter(SpawnListChara.Get("summon_undead", (SourceChara.Row r) => r.HasTag(CTAG.undead) || (EClass.sources.races.map.TryGetValue(r.race)?.IsUndead ?? false)), power / 5);
					break;
				case "pawn":
					chara = CharaGen.CreateFromFilter("c_pawn", power / 10);
					break;
				case "machine":
					chara = CharaGen.CreateFromFilter("c_machine", power / 10);
					break;
				case "monster":
					chara = CharaGen.CreateFromFilter("c_dungeon", power / 10);
					break;
				case "animal":
					chara = CharaGen.CreateFromFilter("c_animal", power / 15);
					break;
				case "fire":
					chara = CharaGen.CreateFromElement("Fire", power / 10);
					break;
				case "fish":
					chara = CharaGen.CreateFromFilter(SpawnListChara.Get("summon_fish", (SourceChara.Row r) => r.ContainsTag("water") || r.model.Chara.race.tag.Contains("water")), power / 10);
					break;
				case "octopus":
					chara = CharaGen.CreateFromFilter(SpawnListChara.Get("summon_octopus", (SourceChara.Row r) => r.race == "octopus"), power / 10);
					break;
				default:
					chara = CharaGen.Create(id3, power / 10);
					break;
				}
				if (chara == null)
				{
					continue;
				}
				if (chara.rarity >= Rarity.Legendary && !flag4)
				{
					j--;
					continue;
				}
				int num6 = -1;
				n = actRef.n1;
				if (!(n == "shadow"))
				{
					if (n == "special2")
					{
						point.PlayEffect("darkwomb2");
					}
					else
					{
						int num7 = 1;
						if (!CC.IsPC)
						{
							num7 = (CC.IsPCFactionOrMinion ? (CC.LV / 2) : (CC.LV / 3 * 2));
						}
						num6 = chara.LV * (100 + power / 10) / 100 + power / 30;
						if (num6 < num7)
						{
							num6 = num7;
						}
					}
				}
				else
				{
					num6 = power / 10 + 1;
				}
				if (chara.LV < num6)
				{
					chara.SetLv(num6);
				}
				chara.interest = 0;
				if (chara.HaveFur())
				{
					chara.c_fur = -1;
				}
				n = actRef.n1;
				if (!(n == "shadow"))
				{
					if (n == "special_force")
					{
						chara.homeZone = EClass._zone;
					}
				}
				else
				{
					chara.hp = chara.MaxHP / 2;
				}
				EClass._zone.AddCard(chara, point);
				if (flag)
				{
					Chara chara2 = chara;
					Hostility hostility2 = (chara.c_originalHostility = Hostility.Enemy);
					chara2.hostility = hostility2;
				}
				else if (!(chara.id == "cocoon") && (!(actRef.n1 == "monster") || actRef.refThing == null))
				{
					chara.MakeMinion(CC);
				}
				if (num4 != -1)
				{
					chara.SetSummon(num4);
				}
				flag3 = true;
			}
			if (!flag3)
			{
				CC.Say("summon_ally_fail", CC);
			}
			return;
		}
		case EffectId.Funnel:
		case EffectId.Bit:
		{
			if (EClass._zone.CountMinions(CC) >= CC.MaxSummon || CC.c_uidMaster != 0)
			{
				CC.Say("summon_ally_fail", CC);
				return;
			}
			CC.Say("spell_funnel", CC, element.Name.ToLower());
			CC.PlaySound("spell_funnel");
			Chara chara3 = CharaGen.Create((id == EffectId.Bit) ? "bit2" : "bit");
			chara3.SetMainElement(element.source.alias, element.Value, elemental: true);
			chara3.SetSummon(20 + power / 20 + EClass.rnd(10));
			chara3.SetLv(power / 15);
			chara3.interest = 0;
			EClass._zone.AddCard(chara3, tp.GetNearestPoint(allowBlock: false, allowChara: false));
			chara3.PlayEffect("teleport");
			chara3.MakeMinion(CC);
			return;
		}
		case EffectId.Breathe:
		{
			List<Point> list2 = EClass._map.ListPointsInArc(CC.pos, tp, 7, 35f);
			if (list2.Count == 0)
			{
				list2.Add(CC.pos.Copy());
			}
			CC.Say("spell_breathe", CC, element.Name.ToLower());
			EClass.Wait(0.8f, CC);
			TryDelay(delegate
			{
				CC.PlaySound("spell_breathe");
			});
			if (CC.IsInMutterDistance() && !EClass.core.config.graphic.disableShake)
			{
				Shaker.ShakeCam("breathe");
			}
			DamageEle(CC, id, power, element, list2, actRef, "spell_breathe");
			return;
		}
		case EffectId.Scream:
			CC.PlaySound("scream");
			CC.PlayEffect("scream");
			{
				foreach (Point item2 in EClass._map.ListPointsInCircle(cc.pos, 6f, mustBeWalkable: false, los: false))
				{
					foreach (Chara chara4 in item2.Charas)
					{
						if (chara4.ResistLv(957) <= 0)
						{
							chara4.AddCondition<ConParalyze>(power);
						}
					}
				}
				return;
			}
		case EffectId.Ball:
		case EffectId.Explosive:
		case EffectId.BallBubble:
		case EffectId.Suicide:
		case EffectId.Rocket:
		case EffectId.Flare:
		case EffectId.GravityGun:
		{
			float radius2 = ((id == EffectId.GravityGun) ? 4f : ((id == EffectId.Rocket) ? 2.8f : ((id == EffectId.Suicide) ? 3.5f : ((id == EffectId.Flare) ? 2.1f : ((float)((id == EffectId.BallBubble) ? 2 : 5))))));
			if ((id == EffectId.Explosive || id == EffectId.Rocket) && actRef.refThing != null)
			{
				radius2 = 2 + actRef.refThing.Evalue(666);
			}
			if (id == EffectId.Suicide)
			{
				if (CC.MainElement != Element.Void)
				{
					element = CC.MainElement;
				}
				if (CC.HasCondition<ConBrightnessOfLife>())
				{
					element = Element.Create(919, 10);
				}
				if (CC.HasTag(CTAG.kamikaze))
				{
					radius2 = 1.5f;
				}
			}
			bool flag5 = id == EffectId.Explosive || id == EffectId.Suicide || id == EffectId.Rocket;
			List<Point> list5 = EClass._map.ListPointsInCircle((id == EffectId.GravityGun || id == EffectId.Rocket || id == EffectId.Flare) ? tp : cc.pos, radius2, !flag5, !flag5);
			if (list5.Count == 0)
			{
				list5.Add(cc.pos.Copy());
			}
			cc.Say((id == EffectId.Suicide) ? "abSuicide" : "spell_ball", cc, element.Name.ToLower());
			EClass.Wait(0.8f, cc);
			TryDelay(delegate
			{
				if (id == EffectId.Flare)
				{
					tp.PlayEffect("flare");
				}
				if (id != EffectId.GravityGun)
				{
					cc.PlaySound((id == EffectId.Flare) ? "spell_flare" : "spell_ball");
				}
			});
			if (id != EffectId.GravityGun && cc.IsInMutterDistance() && !EClass.core.config.graphic.disableShake)
			{
				Shaker.ShakeCam("ball");
			}
			DamageEle(actRef.origin ?? cc, id, power, element, list5, actRef, (id == EffectId.Suicide) ? "suicide" : "spell_ball");
			if (id == EffectId.Suicide && CC.IsAliveInCurrentZone)
			{
				CC.Die();
			}
			return;
		}
		case EffectId.Bolt:
		{
			List<Point> list4 = EClass._map.ListPointsInLine(CC.pos, tp, 10);
			if (list4.Count == 0)
			{
				list4.Add(CC.pos.Copy());
			}
			CC.Say("spell_bolt", CC, element.Name.ToLower());
			EClass.Wait(0.8f, CC);
			TryDelay(delegate
			{
				CC.PlaySound("spell_bolt");
			});
			if (CC.IsInMutterDistance() && !EClass.core.config.graphic.disableShake)
			{
				Shaker.ShakeCam("bolt");
			}
			DamageEle(CC, id, power, element, list4, actRef, "spell_bolt");
			return;
		}
		case EffectId.Bubble:
		case EffectId.Web:
		case EffectId.MistOfDarkness:
		case EffectId.Puddle:
		{
			if (LangGame.Has("ab" + id))
			{
				CC.Say("ab" + id, CC);
			}
			tp.PlaySound("vomit");
			int num = 2 + EClass.rnd(3);
			int id2 = ((id == EffectId.Puddle) ? 4 : ((id == EffectId.Bubble) ? 5 : ((id == EffectId.MistOfDarkness) ? 6 : 7)));
			EffectId idEffect = ((id == EffectId.Bubble) ? EffectId.BallBubble : EffectId.PuddleEffect);
			Color matColor = EClass.Colors.elementColors.TryGetValue(element.source.alias);
			if (id == EffectId.Bubble && CC.id == "cancer")
			{
				idEffect = EffectId.Nothing;
				num = 1 + EClass.rnd(3);
			}
			for (int i = 0; i < num; i++)
			{
				Point randomPoint = tp.GetRandomPoint(2);
				if (randomPoint != null && !randomPoint.HasBlock && (id != EffectId.Puddle || !randomPoint.cell.IsTopWaterAndNoSnow))
				{
					int num2 = 4 + EClass.rnd(5);
					if (id == EffectId.Web)
					{
						num2 *= 3;
					}
					EClass._map.SetEffect(randomPoint.x, randomPoint.z, new CellEffect
					{
						id = id2,
						amount = num2,
						idEffect = idEffect,
						idEle = element.id,
						power = power,
						isHostileAct = CC.IsPCParty,
						color = BaseTileMap.GetColorInt(ref matColor, 100)
					});
				}
			}
			return;
		}
		}
		List<Card> list8 = tp.ListCards().ToList();
		list8.Reverse();
		if (list8.Contains(CC))
		{
			list8.Remove(CC);
			list8.Insert(0, CC);
		}
		bool flag6 = true;
		foreach (Card item3 in list8)
		{
			if (tc == null || item3 == tc)
			{
				Proc(id, power, state, CC, item3, actRef);
				if (flag2 && item3.isChara && item3 != CC)
				{
					CC.DoHostileAction(item3);
				}
				if (actRef.refThing == null || !(actRef.refThing.trait is TraitRod))
				{
					return;
				}
				EffectId effectId = id;
				if ((uint)(effectId - 200) <= 4u)
				{
					return;
				}
				flag6 = false;
			}
		}
		if (flag6)
		{
			CC.SayNothingHappans();
		}
	}

	public static void Proc(EffectId id, Card cc, Card tc = null, int power = 100, ActRef actRef = default(ActRef))
	{
		Proc(id, power, BlessedState.Normal, cc, tc, actRef);
	}

	public static void Proc(EffectId id, int power, BlessedState state, Card cc, Card tc = null, ActRef actRef = default(ActRef))
	{
		if (tc == null)
		{
			tc = cc;
		}
		Chara TC = tc.Chara;
		Chara CC = cc.Chara;
		bool blessed = state >= BlessedState.Blessed;
		bool flag = state <= BlessedState.Cursed;
		int orgPower = power;
		if (blessed || flag)
		{
			power *= 2;
		}
		switch (id)
		{
		case EffectId.Duplicate:
		{
			Point randomPoint = CC.pos.GetRandomPoint(2, requireLos: false, allowChara: false, allowBlocked: false, 200);
			if (randomPoint == null || randomPoint.Equals(CC.pos) || !randomPoint.IsValid || !CC.CanDuplicate())
			{
				CC.Say("split_fail", CC);
				return;
			}
			Chara t2 = CC.Duplicate();
			EClass._zone.AddCard(t2, randomPoint);
			CC.Say("split", CC);
			break;
		}
		case EffectId.Escape:
			if (CC.IsPCFaction || (EClass._zone.Boss == CC && EClass.rnd(30) != 0))
			{
				return;
			}
			CC.Say("escape", CC);
			CC.PlaySound("escape");
			if (EClass._zone.Boss == CC)
			{
				CC.TryDropBossLoot();
			}
			CC.Destroy();
			break;
		case EffectId.BurnMana:
			CC.PlaySound("fire");
			CC.PlayEffect("Element/eleFire");
			CC.Say("burn_mana", CC);
			CC.mana.Mod(-CC.mana.max / 3 - 1);
			break;
		case EffectId.Exterminate:
		{
			CC.PlaySound("clean_floor");
			Msg.Say("exterminate");
			List<Chara> list2 = EClass._map.charas.Where((Chara c) => c.isCopy && !c.IsPCFaction).ToList();
			if (list2.Count == 0)
			{
				Msg.SayNothingHappen();
				return;
			}
			foreach (Chara item in list2)
			{
				item.Say("split_fail", item);
				item.PlayEffect("vanish");
				item.Die();
			}
			break;
		}
		case EffectId.DropMine:
		{
			if (CC.pos.Installed != null || EClass._zone.IsPCFaction)
			{
				return;
			}
			Thing thing = ThingGen.Create("mine");
			thing.c_idRefCard = "dog_mine";
			Zone.ignoreSpawnAnime = true;
			EClass._zone.AddCard(thing, CC.pos).Install();
			break;
		}
		case EffectId.LittleSisterMigration:
		{
			if (!(EClass._zone is Zone_LittleGarden) && !EClass.game.IsSurvival)
			{
				Msg.SayNothingHappen();
				return;
			}
			List<Chara> list3 = new List<Chara>();
			bool flag5 = false;
			foreach (Chara chara2 in EClass._map.charas)
			{
				if (!chara2.IsPCFactionOrMinion && chara2.id == "littleOne")
				{
					if (flag5)
					{
						flag5 = false;
						continue;
					}
					list3.Add(chara2);
					flag5 = true;
				}
			}
			if (list3.Count == 0)
			{
				Msg.SayNothingHappen();
				return;
			}
			EClass.pc.PlaySound("chime_angel");
			foreach (Chara item2 in list3)
			{
				item2.PlayEffect("revive");
				item2.Destroy();
			}
			Msg.Say("little_migration", list3.Count.ToString() ?? "");
			EClass._zone.ModInfluence(list3.Count);
			EClass.player.stats.sistersDepart += list3.Count;
			break;
		}
		case EffectId.MagicMap:
			if (!CC.IsPC)
			{
				CC.SayNothingHappans();
				break;
			}
			if (flag)
			{
				CC.Say("abMagicMap_curse", CC);
				CC.PlaySound("curse3");
				CC.PlayEffect("curse");
				CC.AddCondition<ConConfuse>(200, force: true);
				break;
			}
			CC.Say("abMagicMap", CC);
			CC.PlayEffect("identify");
			CC.PlaySound("identify");
			if (blessed)
			{
				EClass._map.RevealAll();
			}
			else
			{
				EClass._map.Reveal(CC.pos, power);
			}
			break;
		case EffectId.AbsorbMana:
		{
			if (CC == TC)
			{
				EClass.game.religions.Element.Talk("ability");
			}
			Dice dice = Dice.Create("ActManaAbsorb", power, CC, (actRef.refThing != null) ? null : actRef.act);
			TC.mana.Mod(dice.Roll());
			TC.PlaySound("heal");
			TC.PlayEffect("heal");
			if (TC == CC)
			{
				CC.Say("absorbMana", CC);
			}
			break;
		}
		case EffectId.ModPotential:
		{
			Element element = cc.elements.ListElements((Element e) => e.HasTag("primary")).RandomItem();
			cc.elements.ModTempPotential(element.id, power / 10);
			break;
		}
		case EffectId.ForgetItems:
		{
			TC.PlaySound("curse3");
			TC.PlayEffect("curse");
			TC.Say("forgetItems", TC);
			int num5 = power / 50 + 1 + EClass.rnd(3);
			List<Thing> source = TC.things.List((Thing t) => t.c_IDTState == 0);
			for (int j = 0; j < num5; j++)
			{
				source.RandomItem().c_IDTState = 5;
			}
			break;
		}
		case EffectId.EnchantWeapon:
		case EffectId.EnchantArmor:
		case EffectId.EnchantWeaponGreat:
		case EffectId.EnchantArmorGreat:
		{
			bool armor = id == EffectId.EnchantArmor || id == EffectId.EnchantArmorGreat;
			bool flag4 = id == EffectId.EnchantWeaponGreat || id == EffectId.EnchantArmorGreat;
			if (!tc.isThing)
			{
				LayerDragGrid.CreateEnchant(CC, armor, flag4, state);
				return;
			}
			cc.PlaySound("identify");
			cc.PlayEffect("identify");
			if (flag)
			{
				cc.Say("enc_curse", tc);
				tc.ModEncLv(-1);
				break;
			}
			int num4 = (flag4 ? 4 : 2) + (blessed ? 1 : 0);
			if (tc.encLV >= num4)
			{
				cc.Say("enc_resist", tc);
				break;
			}
			cc.Say("enc", tc);
			tc.ModEncLv(1);
			break;
		}
		case EffectId.Identify:
		case EffectId.GreaterIdentify:
		{
			bool flag6 = id == EffectId.GreaterIdentify;
			if (flag)
			{
				Redirect(EffectId.ForgetItems, flag6 ? BlessedState.Cursed : BlessedState.Normal, default(ActRef));
				break;
			}
			if (!tc.isThing)
			{
				int count = ((!blessed) ? 1 : (flag6 ? (2 + EClass.rnd(2)) : (3 + EClass.rnd(3))));
				LayerDragGrid.CreateIdentify(CC, flag6, state, 0, count);
				return;
			}
			cc.PlaySound("identify");
			cc.PlayEffect("identify");
			tc.Thing.Identify(cc.IsPCParty, (!flag6) ? IDTSource.Identify : IDTSource.SuperiorIdentify);
			break;
		}
		case EffectId.Uncurse:
		{
			if (!tc.isThing)
			{
				LayerDragGrid.CreateUncurse(CC, state);
				return;
			}
			Thing thing2 = tc.Thing;
			if (thing2.blessedState == BlessedState.Cursed)
			{
				thing2.SetBlessedState(BlessedState.Normal);
			}
			else if (thing2.blessedState == BlessedState.Doomed)
			{
				thing2.SetBlessedState(BlessedState.Normal);
			}
			thing2.GetRootCard()?.TryStack(thing2);
			LayerInventory.SetDirty(thing2);
			break;
		}
		case EffectId.Lighten:
		{
			if (!tc.isThing)
			{
				LayerDragGrid.CreateLighten(CC, state);
				return;
			}
			if (tc.Num > 1)
			{
				tc = tc.Split(1);
			}
			cc.PlaySound("offering");
			cc.PlayEffect("buff");
			int num3 = (tc.isWeightChanged ? tc.c_weight : tc.Thing.source.weight);
			tc.isWeightChanged = true;
			Element orCreateElement = tc.elements.GetOrCreateElement(64);
			Element orCreateElement2 = tc.elements.GetOrCreateElement(65);
			Element orCreateElement3 = tc.elements.GetOrCreateElement(67);
			Element orCreateElement4 = tc.elements.GetOrCreateElement(66);
			bool flag3 = tc.IsEquipmentOrRangedOrAmmo || tc.IsThrownWeapon;
			if (flag)
			{
				num3 = (int)(0.01f * (float)num3 * (float)power * 0.75f + 500f);
				if (num3 < 0 || num3 > 10000000)
				{
					num3 = 10000000;
					flag3 = false;
				}
				if (flag3)
				{
					if (tc.IsWeapon || tc.IsThrownWeapon || tc.IsAmmo)
					{
						tc.elements.ModBase(67, Mathf.Clamp(orCreateElement3.vBase * power / 1000, 1, 5));
						tc.elements.ModBase(66, -Mathf.Clamp(orCreateElement4.vBase * power / 1000, 1, 5));
					}
					else
					{
						tc.elements.ModBase(65, Mathf.Clamp(orCreateElement2.vBase * power / 1000, 1, 5));
						tc.elements.ModBase(64, -Mathf.Clamp(orCreateElement.vBase * power / 1000, 1, 5));
					}
				}
				cc.Say("lighten_curse", cc, tc);
			}
			else
			{
				num3 = num3 * (100 - power / 10) / 100;
				if (blessed)
				{
					power /= 4;
				}
				if (flag3)
				{
					if (tc.IsWeapon || tc.IsThrownWeapon || tc.IsAmmo)
					{
						tc.elements.ModBase(67, -Mathf.Clamp(orCreateElement3.vBase * power / 1000, 1, 5));
						tc.elements.ModBase(66, Mathf.Clamp(orCreateElement4.vBase * power / 1000, 1, 5));
					}
					else
					{
						tc.elements.ModBase(65, -Mathf.Clamp(orCreateElement2.vBase * power / 1000, 1, 5));
						tc.elements.ModBase(64, Mathf.Clamp(orCreateElement.vBase * power / 1000, 1, 5));
					}
				}
				cc.Say("lighten", cc, tc);
			}
			tc.c_weight = num3;
			tc.SetDirtyWeight();
			if (tc.parent == null)
			{
				CC.Pick(tc.Thing, msg: false);
			}
			CC.body.UnqeuipIfTooHeavy(tc.Thing);
			break;
		}
		case EffectId.Reconstruction:
		{
			if (!tc.isThing)
			{
				LayerDragGrid.CreateReconstruction(CC, state);
				return;
			}
			if (tc.Num > 1)
			{
				tc = tc.Split(1);
			}
			cc.PlaySound("mutation");
			cc.PlayEffect("identify");
			cc.Say("reconstruct", cc, tc);
			EClass.game.cards.uidNext += EClass.rnd(30);
			int num6 = Mathf.Max(tc.genLv, tc.LV, EClass.player.stats.deepest);
			CardBlueprint.Set(new CardBlueprint
			{
				blesstedState = state
			});
			Thing thing3 = ThingGen.Create(tc.id, -1, (int)((long)num6 * (long)power / 400));
			thing3.genLv = num6;
			tc.Destroy();
			CC.Pick(thing3, msg: false);
			if (!CC.IsPC)
			{
				CC.TryEquip(thing3);
			}
			break;
		}
		case EffectId.ChangeMaterialLesser:
		case EffectId.ChangeMaterial:
		case EffectId.ChangeMaterialGreater:
		{
			SourceMaterial.Row row = EClass.sources.materials.alias.TryGetValue(actRef.n1);
			if (!tc.isThing)
			{
				LayerDragGrid.CreateChangeMaterial(CC, actRef.refThing, row, id, state);
				return;
			}
			if (tc.Num > 1)
			{
				tc = tc.Split(1);
			}
			string name = tc.Name;
			if (row == null)
			{
				bool num = id == EffectId.ChangeMaterialGreater;
				bool flag2 = id == EffectId.ChangeMaterialLesser;
				string text2 = tc.Thing.source.tierGroup;
				Dictionary<string, SourceMaterial.TierList> tierMap = SourceMaterial.tierMap;
				int num2 = 1;
				if (flag)
				{
					num2 -= 2;
				}
				if (blessed)
				{
					num2++;
				}
				if (num)
				{
					num2++;
				}
				if (flag2)
				{
					num2 -= 2;
				}
				num2 = Mathf.Clamp(num2 + EClass.rnd(2), 0, 4);
				if (EClass.rnd(10) == 0)
				{
					text2 = ((text2 == "metal") ? "leather" : "metal");
				}
				SourceMaterial.TierList tierList = (text2.IsEmpty() ? tierMap.RandomItem() : tierMap[text2]);
				for (int i = 0; i < 1000; i++)
				{
					row = tierList.tiers[num2].Select();
					if (row != tc.material)
					{
						break;
					}
				}
			}
			cc.PlaySound("offering");
			cc.PlayEffect("buff");
			if ((tc.id == "log" || tc.id == "branch") && tc.material.alias == "carbone")
			{
				foreach (Element item3 in tc.elements.dict.Values.ToList())
				{
					if (item3.IsTrait && item3.vBase != 0)
					{
						tc.elements.ModBase(item3.id, -item3.vBase);
					}
				}
			}
			tc.ChangeMaterial(row);
			if (tc.trait is TraitGene && tc.c_DNA != null)
			{
				DNA.Type type = DNA.GetType(tc.material.alias);
				tc.c_DNA.Generate(type);
			}
			cc.Say("materialChanged", name, row.GetName());
			if (CC != null)
			{
				if (tc.parent == null)
				{
					CC.Pick(tc.Thing, msg: false);
				}
				CC.body.UnqeuipIfTooHeavy(tc.Thing);
			}
			break;
		}
		case EffectId.ReturnVoid:
		{
			Zone_Void root = EClass.game.spatials.Find<Zone_Void>();
			if (EClass.game.IsSurvival || root == null || root.visitCount == 0 || EClass.player.stats.deepestVoid < 1 || (!EClass.debug.enable && EClass.player.CountKeyItem("license_void") == 0))
			{
				Msg.SayNothingHappen();
				return;
			}
			int max = Mathf.Min(EClass.player.stats.deepestVoid, -root.MinLv);
			int destLv = 1;
			Dialog.InputName("dialogVoidReturn".lang(max.ToString() ?? ""), max.ToString() ?? "", delegate(bool cancel, string text)
			{
				if (!cancel)
				{
					destLv = Mathf.Abs(text.ToInt());
					destLv = Mathf.Clamp(destLv, 1, max) * -1;
					Zone zone = ((root.lv == destLv) ? root : (root.children.Find((Spatial t) => t.lv == destLv) as Zone));
					Debug.Log(destLv + "/" + zone);
					if (zone == null)
					{
						zone = SpatialGen.Create(root.GetNewZoneID(destLv), root, register: true) as Zone;
						zone.lv = destLv;
					}
					Msg.Say("returnComplete");
					EClass.player.uidLastTravelZone = 0;
					EClass.pc.MoveZone(zone, ZoneTransition.EnterState.Return);
					EClass.player.lastZonePos = null;
					EClass.player.returnInfo = null;
				}
			});
			break;
		}
		case EffectId.Return:
		case EffectId.Evac:
			if (!cc.IsPC)
			{
				Redirect(EffectId.Teleport, state, default(ActRef));
				return;
			}
			cc.PlaySound("return_cast");
			if (EClass.player.returnInfo == null)
			{
				if (id == EffectId.Evac)
				{
					EClass.player.returnInfo = new Player.ReturnInfo
					{
						turns = EClass.rnd(10) + 10,
						isEvac = true
					};
				}
				else
				{
					if (EClass.game.spatials.ListReturnLocations().Count == 0)
					{
						Msg.Say("returnNowhere");
						break;
					}
					EClass.player.returnInfo = new Player.ReturnInfo
					{
						turns = EClass.rnd(10) + 10,
						askDest = true
					};
				}
				Msg.Say("returnBegin");
			}
			else
			{
				EClass.player.returnInfo = null;
				Msg.Say("returnAbort");
			}
			break;
		case EffectId.Teleport:
		case EffectId.TeleportShort:
		case EffectId.Gate:
			if (!tc.HasHost)
			{
				if (!flag)
				{
					if (id == EffectId.TeleportShort)
					{
						tc.Teleport(GetTeleportPos(tc.pos));
					}
					else
					{
						tc.Teleport(GetTeleportPos(tc.pos, EClass._map.bounds.Width));
					}
				}
				if (id == EffectId.Gate && CC.IsPC)
				{
					foreach (Chara chara3 in EClass._map.charas)
					{
						if (!chara3.HasHost && chara3 != tc && (chara3.IsPCParty || chara3.IsPCPartyMinion))
						{
							chara3.Teleport(tc.pos.GetNearestPoint(allowBlock: false, allowChara: false) ?? tc.pos);
						}
					}
				}
			}
			if (flag)
			{
				Redirect(EffectId.Gravity, BlessedState.Normal, default(ActRef));
			}
			if (blessed)
			{
				Redirect(EffectId.Levitate, BlessedState.Normal, default(ActRef));
			}
			break;
		}
		if (TC == null)
		{
			return;
		}
		switch (id)
		{
		case EffectId.ThrowPotion:
			if (!CC.pos.Equals(TC.pos))
			{
				Thing t3 = ThingGen.Create(new string[6] { "330", "331", "334", "335", "336", "1142" }.RandomItem());
				ActThrow.Throw(CC, TC.pos, t3, ThrowMethod.Punish, 0.7f);
			}
			break;
		case EffectId.StripBlessing:
		{
			List<Condition> list10 = new List<Condition>();
			foreach (Condition condition4 in TC.conditions)
			{
				if (GetBlessingDifficulty(condition4) > 0 && EClass.rnd(GetBlessingDifficulty(condition4)) == 0)
				{
					list10.Add(condition4);
				}
			}
			if (list10.Count == 0)
			{
				CC.SayNothingHappans();
				break;
			}
			TC.pos.PlayEffect("holyveil");
			TC.pos.PlaySound("holyveil");
			TC.Say("unpolluted", TC);
			list10.Shuffle();
			{
				foreach (Condition item4 in list10)
				{
					item4.Kill();
					if (CC.IsHostile(TC))
					{
						break;
					}
				}
				break;
			}
		}
		case EffectId.ShutterHex:
		{
			if (!CC.IsHostile(TC))
			{
				break;
			}
			int num12 = 0;
			foreach (Condition condition5 in TC.conditions)
			{
				if (condition5.Type == ConditionType.Debuff)
				{
					num12++;
				}
			}
			if (num12 == 0)
			{
				CC.SayNothingHappans();
				break;
			}
			TC.pos.PlayEffect("holyveil");
			TC.pos.PlaySound("holyveil");
			TC.pos.PlaySound("atk_eleSound");
			TC.conditions.ForeachReverse(delegate(Condition c)
			{
				if (c.Type == ConditionType.Debuff && EClass.rnd(3) == 0)
				{
					c.Kill();
				}
			});
			TC.Say("abShutterHex", TC);
			Point center = CC.pos.Copy();
			List<Chara> list9 = TC.pos.ListCharasInRadius(TC, 4, (Chara c) => c == TC || c.IsHostile(CC));
			for (int m = 0; m < num12; m++)
			{
				TweenUtil.Delay((float)m * 0.1f, delegate
				{
					center.PlaySound("shutterhex");
				});
				foreach (Chara item5 in list9)
				{
					if (item5.ExistsOnMap)
					{
						Effect effect = Effect.Get("spell_moonspear");
						TrailRenderer componentInChildren = effect.GetComponentInChildren<TrailRenderer>();
						Color startColor = (componentInChildren.endColor = EClass.Colors.elementColors["eleHoly"]);
						componentInChildren.startColor = startColor;
						Point pos = item5.pos.Copy();
						TweenUtil.Delay((float)m * 0.1f, delegate
						{
							effect.Play(center, 0f, pos);
						});
						int num13 = Dice.Create("SpShutterHex", power, CC, (actRef.refThing != null) ? null : actRef.act).Roll();
						item5.DamageHP(num13, 919, power, AttackSource.None, CC, showEffect: false);
					}
				}
			}
			break;
		}
		case EffectId.Draw:
		{
			if (CC.Dist(TC) <= 1 || (CC.IsPCFactionOrMinion && TC.IsPCFactionOrMinion && TC.isRestrained))
			{
				break;
			}
			Point point = CC.GetFirstStep(TC.pos, PathManager.MoveType.Combat);
			if (!point.IsValid)
			{
				point = CC.pos.GetRandomPoint(1)?.GetNearestPoint(allowBlock: false, allowChara: false);
			}
			if (point == null || !CC.CanSeeLos(point))
			{
				break;
			}
			CC.Say("abDraw", CC, TC);
			if (TC.HasCondition<ConGravity>())
			{
				CC.SayNothingHappans();
				break;
			}
			TC.MoveImmediate(point, !EClass.core.config.camera.smoothFollow);
			if (CC.id == "tentacle")
			{
				TC.AddCondition<ConEntangle>();
			}
			break;
		}
		case EffectId.CatSniff:
		{
			Chara nearbyCatToSniff = CC.GetNearbyCatToSniff();
			if (nearbyCatToSniff != null)
			{
				CC.Sniff(nearbyCatToSniff);
			}
			break;
		}
		case EffectId.Steal:
		{
			if (EClass._zone.instance is ZoneInstanceBout)
			{
				break;
			}
			if (TC.Evalue(426) > 0)
			{
				TC.Say((actRef.n1 == "money") ? "abStealNegateMoney" : "abStealNegate", TC);
				break;
			}
			Thing thing5 = null;
			bool flag7 = actRef.n1 == "food";
			if (actRef.n1 == "money")
			{
				int currency = TC.GetCurrency();
				if (currency > 0)
				{
					currency = Mathf.Clamp(EClass.rnd(currency / 10), 1, 100 + EClass.rndHalf(CC.LV * 200));
					thing5 = ThingGen.Create("money").SetNum(currency);
					TC.ModCurrency(-currency);
				}
			}
			else
			{
				Func<Thing, bool> func = (Thing t) => true;
				if (flag7)
				{
					func = (Thing t) => t.IsFood;
				}
				List<Thing> list5 = TC.things.List(delegate(Thing t)
				{
					if (t.parentCard?.trait is TraitChestMerchant || t.trait is TraitTool || t.IsThrownWeapon)
					{
						return false;
					}
					return t.trait.CanBeDestroyed && t.things.Count == 0 && t.invY != 1 && t.trait.CanBeStolen && !t.trait.CanOnlyCarry && !t.IsUnique && !t.isEquipped && t.blessedState == BlessedState.Normal && func(t);
				}, onlyAccessible: true);
				if (list5.Count > 0)
				{
					thing5 = list5.RandomItem();
					if (thing5.Num > 1)
					{
						thing5 = thing5.Split(1);
					}
				}
				CC.AddCooldown(6640, 200);
			}
			if (thing5 == null)
			{
				CC.Say("abStealNothing", CC, TC);
				break;
			}
			thing5.SetInt(116, 1);
			TC.PlaySound(thing5.material.GetSoundDrop(thing5.sourceCard));
			CC.Pick(thing5, msg: false);
			CC.Say("abSteal", CC, TC, thing5.Name);
			if (actRef.n1 == "food")
			{
				if (CC.hunger.value != 0)
				{
					CC.InstantEat(thing5);
				}
			}
			else
			{
				CC.Say("abStealEscape", CC);
				CC.Teleport(GetTeleportPos(tc.pos, 30), silent: true);
			}
			break;
		}
		case EffectId.NeckHunt:
			CC.TryNeckHunt(TC, power);
			break;
		case EffectId.CurseEQ:
		{
			if (CC != null && CC != TC)
			{
				TC.Say("curse", CC, TC);
			}
			TC.PlaySound("curse3");
			TC.PlayEffect("curse");
			if (EClass.rnd(150 + TC.LUC * 5 + TC.Evalue(972) * 20) >= power + (flag ? 200 : 0) || TC.TryNullifyCurse())
			{
				break;
			}
			List<Thing> list4 = TC.things.List(delegate(Thing t)
			{
				if (!t.isEquipped || t.blessedState == BlessedState.Doomed || t.IsToolbelt)
				{
					return false;
				}
				return (t.blessedState < BlessedState.Blessed || EClass.rnd(10) == 0) ? true : false;
			});
			if (list4.Count == 0)
			{
				CC.SayNothingHappans();
				break;
			}
			Thing thing4 = list4.RandomItem();
			TC.Say("curse_hit", TC, thing4);
			thing4.SetBlessedState((thing4.blessedState == BlessedState.Cursed) ? BlessedState.Doomed : BlessedState.Cursed);
			LayerInventory.SetDirty(thing4);
			break;
		}
		case EffectId.UncurseEQ:
		case EffectId.UncurseEQGreater:
		{
			TC.Say("uncurseEQ" + (blessed ? "_bless" : (flag ? "_curse" : "")), TC);
			TC.PlaySound("uncurse");
			TC.PlayEffect("uncurse");
			if (flag)
			{
				Redirect(EffectId.CurseEQ, BlessedState.Normal, default(ActRef));
				break;
			}
			int success = 0;
			int fail = 0;
			List<Thing> list = new List<Thing>();
			TC.things.Foreach(delegate(Thing t)
			{
				int num14 = 0;
				if ((t.isEquipped || t.IsRangedWeapon || blessed) && t.blessedState < BlessedState.Normal)
				{
					if (t.blessedState == BlessedState.Cursed)
					{
						num14 = EClass.rnd(200);
					}
					if (t.blessedState == BlessedState.Doomed)
					{
						num14 = EClass.rnd(1000);
					}
					if (blessed)
					{
						num14 /= 2;
					}
					if (id == EffectId.UncurseEQGreater)
					{
						num14 /= 10;
					}
					if (power >= num14)
					{
						TC.Say("uncurseEQ_success", t);
						t.SetBlessedState(BlessedState.Normal);
						if (t.isEquipped && t.HasElement(656))
						{
							TC.body.Unequip(t);
						}
						LayerInventory.SetDirty(t);
						success++;
						list.Add(t);
					}
					else
					{
						fail++;
					}
				}
			});
			foreach (Thing item6 in list)
			{
				item6.GetRootCard()?.TryStack(item6);
			}
			if (success == 0 && fail == 0)
			{
				TC.SayNothingHappans();
			}
			else if (fail > 0)
			{
				TC.Say("uncurseEQ_fail");
			}
			break;
		}
		case EffectId.Buff:
		{
			string text3 = actRef.n1;
			string text4 = "";
			if (flag)
			{
				text4 = EClass.sources.stats.alias[text3].curse;
				if (!text4.IsEmpty())
				{
					text3 = text4;
				}
			}
			Condition condition2 = Condition.Create(text3, power, delegate(Condition con)
			{
				if (!actRef.aliasEle.IsEmpty())
				{
					con.SetElement(EClass.sources.elements.alias[actRef.aliasEle].id);
				}
			});
			condition2.isPerfume = TC.IsPC && actRef.isPerfume;
			Condition condition3 = TC.AddCondition(condition2);
			if (condition3 != null && condition3.isPerfume)
			{
				condition3.value = 3;
				Msg.Say("perfume", TC);
			}
			if (!text4.IsEmpty())
			{
				CC.DoHostileAction(TC);
			}
			break;
		}
		case EffectId.KizuamiTrick:
		{
			EClass.game.religions.Trickery.Talk("ability");
			bool hex = CC.IsHostile(TC);
			List<SourceStat.Row> list6 = EClass.sources.stats.rows.Where((SourceStat.Row con) => con.tag.Contains("random") && con.group == (hex ? "Debuff" : "Buff")).ToList();
			int power2 = power;
			for (int k = 0; k < 4 + EClass.rnd(2); k++)
			{
				SourceStat.Row row2 = list6.RandomItem();
				list6.Remove(row2);
				Proc(hex ? EffectId.DebuffKizuami : EffectId.Buff, CC, TC, power2, new ActRef
				{
					n1 = row2.alias
				});
			}
			if (EClass.core.config.game.waitOnDebuff && !CC.IsPC)
			{
				EClass.Wait(0.3f, TC);
			}
			break;
		}
		case EffectId.Debuff:
		case EffectId.DebuffKizuami:
		{
			CC.DoHostileAction(TC);
			bool isPowerful = TC.IsPowerful;
			string n = actRef.n1;
			if (n == "ConSuffocation")
			{
				power = power * 2 / 3;
			}
			int a2 = power;
			int num7 = TC.WIL * (isPowerful ? 20 : 5);
			ConHolyVeil condition = TC.GetCondition<ConHolyVeil>();
			if (condition != null)
			{
				num7 += condition.power * 5;
			}
			if (id != EffectId.DebuffKizuami && EClass.rnd(a2) < num7 / EClass.sources.stats.alias[n].hexPower && EClass.rnd(10) != 0)
			{
				TC.Say("debuff_resist", TC);
				CC.DoHostileAction(TC);
				break;
			}
			TC.AddCondition(Condition.Create(n, power, delegate(Condition con)
			{
				con.givenByPcParty = CC.IsPCParty;
				if (!actRef.aliasEle.IsEmpty())
				{
					con.SetElement(EClass.sources.elements.alias[actRef.aliasEle].id);
				}
			}));
			if (n == "ConBane" && CC.HasElement(1416))
			{
				TC.AddCondition<ConExcommunication>(power);
			}
			CC.DoHostileAction(TC);
			if (EClass.core.config.game.waitOnDebuff && !CC.IsPC)
			{
				EClass.Wait(0.3f, TC);
			}
			break;
		}
		case EffectId.Mutation:
			TC.MutateRandom(1, 100, ether: false, state);
			if (EClass.core.config.game.waitOnDebuff)
			{
				EClass.Wait(0.3f, TC);
			}
			break;
		case EffectId.CureMutation:
			TC.MutateRandom(-1, 100, ether: false, state);
			break;
		case EffectId.Ally:
		{
			Msg.Say("gainAlly");
			Chara chara = CharaGen.CreateFromFilter("chara", cc.LV);
			EClass._zone.AddCard(chara, cc.pos.GetNearestPoint(allowBlock: false, allowChara: false));
			if (cc.IsPCFactionOrMinion)
			{
				chara.MakeAlly(msg: false);
			}
			chara.PlaySound("identify");
			chara.PlayEffect("teleport");
			break;
		}
		case EffectId.Wish:
			if (!TC.IsPC)
			{
				break;
			}
			if (blessed || flag)
			{
				power /= 2;
			}
			Dialog.InputName("dialogWish", "q", delegate(bool cancel, string text)
			{
				if (!cancel)
				{
					Msg.Say("wish", TC, text);
					Wish(text, EClass.pc.NameTitled, power, state);
				}
			});
			break;
		case EffectId.Faith:
		{
			Religion faith = tc.Chara.faith;
			tc.PlayEffect("aura_heaven");
			tc.PlaySound("aura_heaven");
			tc.Say("faith", tc, faith.Name);
			if (flag)
			{
				tc.Say("faith_curse", tc, faith.Name);
				break;
			}
			if (blessed)
			{
				tc.Say("faith_bless", tc, faith.Name);
			}
			tc.ModExp(306, power * 10);
			tc.ModExp(85, power * 10);
			if (tc.elements.Base(85) >= tc.elements.Value(306))
			{
				tc.elements.SetBase(85, tc.elements.Value(306));
			}
			break;
		}
		case EffectId.TransGender:
		{
			tc.PlaySound("mutation");
			tc.PlayEffect("mutation");
			int gender = tc.bio.gender;
			int gender2 = gender switch
			{
				1 => 2, 
				2 => 1, 
				_ => (EClass.rnd(2) != 0) ? 1 : 2, 
			};
			if (gender != 0 && EClass.rnd(10) == 0)
			{
				gender2 = 0;
			}
			tc.bio.SetGender(gender2);
			tc.Say("transGender", tc, Gender.Name(tc.bio.gender));
			tc.Talk("tail");
			int age2 = tc.bio.GetAge(tc.Chara);
			if (blessed && age2 > 1)
			{
				tc.Say("ageDown", tc);
				tc.bio.SetAge(tc.Chara, age2 - 1);
			}
			else if (flag)
			{
				tc.Say("ageUp", tc);
				tc.bio.SetAge(tc.Chara, age2 + 1);
			}
			break;
		}
		case EffectId.TransBlood:
		{
			tc.PlaySound("mutation");
			tc.PlayEffect("mutation");
			int num8 = ((actRef.refThing != null) ? actRef.refThing.GetInt(118) : actRef.refVal);
			if (num8 == 0)
			{
				num8 = tc.GetInt(118);
				if (num8 == 0)
				{
					num8 = EClass.game.seed + tc.uid;
				}
				num8++;
			}
			tc.Say("transBlood", tc);
			tc.Talk("tail");
			tc.c_bloodData = null;
			tc.SetInt(118, num8);
			break;
		}
		case EffectId.Youth:
		{
			tc.PlaySound("mutation");
			tc.PlayEffect("mutation");
			int age = tc.bio.GetAge(tc.Chara);
			if (!flag && age <= 0)
			{
				tc.SayNothingHappans();
				break;
			}
			age = Mathf.Max(0, age * 100 / (flag ? 75 : (blessed ? 400 : 200))) + (flag ? 1 : 0);
			tc.Say(flag ? "ageUp" : "ageDown", tc);
			tc.bio.SetAge(tc.Chara, age);
			break;
		}
		case EffectId.EternalYouth:
		{
			tc.PlaySound("mutation");
			tc.PlayEffect("mutation");
			if (tc.IsUnique)
			{
				tc.SayNothingHappans();
				break;
			}
			int age3 = tc.bio.GetAge(tc.Chara);
			if (flag)
			{
				if (tc.c_lockedAge != 0)
				{
					tc.Say("eternalYouth2", tc);
					tc.c_lockedAge = 0;
					tc.elements.Remove(1243);
					tc.bio.SetAge(tc.Chara, age3);
				}
				Redirect(EffectId.Youth, BlessedState.Cursed, default(ActRef));
			}
			else if (tc.c_lockedAge != 0)
			{
				tc.SayNothingHappans();
			}
			else
			{
				tc.PlaySound("dropRewardXmas");
				tc.Say("eternalYouth1", tc);
				tc.c_lockedAge = age3 + 1;
				tc.elements.SetBase(1243, 1);
				if (blessed)
				{
					Redirect(EffectId.Youth, BlessedState.Blessed, default(ActRef));
				}
			}
			break;
		}
		case EffectId.BuffStats:
		case EffectId.DebuffStats:
		case EffectId.LulwyTrick:
			Debug.Log(power + "/" + id.ToString() + "/" + actRef.n1);
			if (id == EffectId.LulwyTrick)
			{
				EClass.game.religions.Wind.Talk("ability");
			}
			if (power < 0 || id == EffectId.DebuffStats)
			{
				power = Mathf.Abs(power);
				if (blessed)
				{
					power /= 4;
				}
				flag = true;
			}
			TC.AddCondition(Condition.Create(power, delegate(ConBuffStats con)
			{
				con.SetRefVal(Element.GetId(actRef.n1), (int)id);
			}));
			break;
		case EffectId.Revive:
		{
			List<KeyValuePair<int, Chara>> list8 = EClass.game.cards.globalCharas.Where((KeyValuePair<int, Chara> a) => a.Value.isDead && a.Value.faction == EClass.pc.faction && !a.Value.isSummon && a.Value.c_wasInPcParty).ToList();
			if (TC.IsPCFaction || TC.IsPCFactionMinion)
			{
				if (TC.IsPC && list8.Count == 0)
				{
					list8 = EClass.game.cards.globalCharas.Where((KeyValuePair<int, Chara> a) => a.Value.CanRevive() && a.Value.isDead && a.Value.faction == EClass.pc.faction && !a.Value.isSummon).ToList();
				}
				if (list8.Count > 0)
				{
					list8.RandomItem().Value.Chara.GetRevived();
					break;
				}
			}
			TC.SayNothingHappans();
			break;
		}
		case EffectId.DamageBody:
		case EffectId.DamageMind:
		case EffectId.DamageBodyGreat:
		case EffectId.DamageMindGreat:
		case EffectId.Weaken:
		{
			bool flag10 = id == EffectId.DamageBody || id == EffectId.DamageBodyGreat;
			bool mind2 = id == EffectId.DamageMind || id == EffectId.DamageMindGreat;
			int num11 = ((id == EffectId.DamageBody || id == EffectId.DamageMind) ? 1 : (4 + EClass.rnd(4)));
			if (id == EffectId.Weaken)
			{
				flag10 = EClass.rnd(2) == 0;
				mind2 = !flag10;
				num11 = 1;
			}
			else
			{
				TC.PlayEffect("debuff");
				TC.PlaySound("debuff");
			}
			TC.Say(flag10 ? "damageBody" : "damageMind", TC);
			for (int l = 0; l < num11; l++)
			{
				TC.DamageTempElements(power, flag10, mind2, id != EffectId.Weaken);
			}
			if (TC.IsPC)
			{
				Tutorial.Play("healer");
			}
			break;
		}
		case EffectId.EnhanceBody:
		case EffectId.EnhanceMind:
		case EffectId.EnhanceBodyGreat:
		case EffectId.EnhanceMindGreat:
		{
			bool flag8 = id == EffectId.EnhanceBody || id == EffectId.EnhanceBodyGreat;
			bool mind = id == EffectId.EnhanceMind || id == EffectId.EnhanceMindGreat;
			if (id != EffectId.EnhanceBody && id != EffectId.EnhanceMind)
			{
				EClass.rnd(4);
			}
			TC.Say(flag8 ? "enhanceBody" : "enhanceMind", TC);
			TC.PlayEffect("buff");
			TC.PlaySound("buff");
			TC.EnhanceTempElements(power, flag8, mind, onlyRenew: true);
			break;
		}
		case EffectId.RestoreBody:
		case EffectId.RestoreMind:
		{
			bool flag9 = id == EffectId.RestoreBody;
			if (flag)
			{
				Redirect(flag9 ? EffectId.DamageBodyGreat : EffectId.DamageMindGreat, BlessedState.Normal, default(ActRef));
				break;
			}
			TC.Say(flag9 ? "restoreBody" : "restoreMind", TC);
			TC.PlaySound("heal");
			TC.PlayEffect("heal");
			TC.CureHost(flag9 ? CureType.CureBody : CureType.CureMind, power, state);
			if (blessed)
			{
				Redirect(flag9 ? EffectId.EnhanceBodyGreat : EffectId.EnhanceMindGreat, BlessedState.Normal, default(ActRef));
			}
			break;
		}
		case EffectId.HealComplete:
			TC.HealHPHost(100000000, (actRef.refThing == null) ? HealSource.Magic : HealSource.Item);
			TC.CureHost(CureType.HealComplete, power, state);
			TC.Say("heal_heavy", TC);
			break;
		case EffectId.Heal:
		case EffectId.JureHeal:
		{
			if (id == EffectId.JureHeal)
			{
				EClass.game.religions.Healing.Talk("ability");
			}
			if (actRef.act != null)
			{
				Debug.Log(actRef.act.id);
			}
			int num10 = Dice.Create((actRef.act != null && EClass.sources.calc.map.ContainsKey(actRef.act.ID)) ? actRef.act.ID : "SpHealLight", power, CC, (actRef.refThing != null) ? null : actRef.act).Roll();
			if (actRef.refThing != null)
			{
				num10 = num10 * (100 + actRef.refThing.Evalue(750) * 10) / 100;
			}
			if (flag)
			{
				TC.DamageHP(num10 / 2, 919, power);
				break;
			}
			TC.HealHPHost(num10, (actRef.refThing == null && id != EffectId.JureHeal) ? HealSource.Magic : HealSource.Item);
			TC.CureHost(CureType.Heal, power, state);
			TC.Say((power >= 300) ? "heal_heavy" : "heal_light", TC);
			break;
		}
		case EffectId.RemedyJure:
			TC.HealHP(1000000, HealSource.Magic);
			TC.CureHost(CureType.Jure, power, state);
			TC.Say("heal_jure", TC);
			break;
		case EffectId.Headpat:
			CC.Cuddle(TC, headpat: true);
			break;
		case EffectId.RemoveHex:
		case EffectId.RemoveHexAll:
			if (flag)
			{
				Redirect(EffectId.CurseEQ, BlessedState.Normal, default(ActRef));
				break;
			}
			foreach (Condition item7 in TC.conditions.Copy())
			{
				if (item7.Type == ConditionType.Debuff && !item7.IsKilled && EClass.rnd(power * (CC.IsPowerful ? 5 : 2)) > EClass.rnd(item7.power))
				{
					CC.Say("removeHex", TC, item7.Name.ToLower());
					item7.Kill();
					if (id == EffectId.RemoveHex)
					{
						break;
					}
				}
			}
			TC.AddCondition<ConHolyVeil>(power / 2);
			break;
		case EffectId.CureCorruption:
			TC.PlaySound("heal");
			TC.PlayEffect("heal");
			if (flag)
			{
				TC.Say("cureCorruption_curse", TC);
				TC.mana.Mod(9999);
				TC.ModCorruption(power);
			}
			else
			{
				TC.Say("cureCorruption", TC);
				TC.ModCorruption(-power * (blessed ? 150 : 200) / 100);
			}
			break;
		case EffectId.Drink:
		case EffectId.DrinkRamune:
		case EffectId.DrinkMilk:
			if (id == EffectId.DrinkRamune)
			{
				TC.Say("drinkRamune", TC);
			}
			if (TC.IsPC)
			{
				TC.Say("drinkGood", TC);
			}
			if (id == EffectId.DrinkMilk)
			{
				if (TC.IsPC)
				{
					TC.Say("drinkMilk", TC);
				}
				if (blessed)
				{
					TC.ModHeight(EClass.rnd(5) + 3);
				}
				else if (flag)
				{
					TC.ModHeight((EClass.rnd(5) + 3) * -1);
				}
			}
			break;
		case EffectId.DrinkWater:
			if (flag)
			{
				if (TC.IsPC)
				{
					TC.Say("drinkWater_dirty", TC);
				}
				TraitWell.BadEffect(TC);
			}
			else if (TC.IsPC)
			{
				TC.Say("drinkWater_clear", TC);
			}
			break;
		case EffectId.DrinkWaterDirty:
			if (TC.IsPC)
			{
				TC.Say("drinkWater_dirty", TC);
			}
			if (TC.IsPCFaction)
			{
				TC.Vomit();
			}
			break;
		case EffectId.SaltWater:
			if (TC.HasElement(1211))
			{
				TC.Say("drinkSaltWater_snail", TC);
				int num9 = ((TC.hp > 10) ? (TC.hp - EClass.rnd(10)) : 10000);
				TC.DamageHP(num9, AttackSource.None, CC);
			}
			else if (TC.IsPC)
			{
				TC.Say("drinkSaltWater", TC);
			}
			break;
		case EffectId.Booze:
			TC.AddCondition<ConDrunk>(power);
			if (TC.HasElement(1215))
			{
				TC.Say("drunk_dwarf", TC);
				TC.AddCondition(Condition.Create(power + EClass.rnd(power), delegate(ConBuffStats con)
				{
					con.SetRefVal(Element.List_MainAttributes.RandomItem(), (int)id);
				}));
			}
			break;
		case EffectId.CatsEye:
			if (flag)
			{
				Redirect(EffectId.Blind, BlessedState.Normal, default(ActRef));
			}
			else
			{
				TC.AddCondition<ConNightVision>(power);
			}
			break;
		case EffectId.Hero:
			if (flag)
			{
				Redirect(EffectId.Fear, BlessedState.Normal, default(ActRef));
			}
			else
			{
				TC.AddCondition<ConHero>(power);
			}
			break;
		case EffectId.HolyVeil:
			if (flag)
			{
				Redirect(EffectId.Fear, BlessedState.Normal, default(ActRef));
			}
			else
			{
				TC.AddCondition<ConHolyVeil>(power);
			}
			break;
		case EffectId.Levitate:
			if (flag)
			{
				Redirect(EffectId.Gravity, BlessedState.Normal, default(ActRef));
			}
			else
			{
				TC.AddCondition<ConLevitate>(power);
			}
			break;
		case EffectId.Gravity:
			if (blessed)
			{
				power /= 4;
			}
			TC.AddCondition<ConGravity>(power);
			if (flag)
			{
				Redirect(EffectId.BuffStats, BlessedState.Cursed, new ActRef
				{
					aliasEle = "STR"
				});
			}
			break;
		case EffectId.Fear:
			if (blessed)
			{
				power /= 4;
			}
			TC.AddCondition<ConFear>(power);
			if (flag)
			{
				Redirect(EffectId.Confuse, BlessedState.Normal, default(ActRef));
			}
			break;
		case EffectId.Faint:
			if (blessed)
			{
				power /= 4;
			}
			TC.AddCondition<ConFaint>(power);
			if (flag)
			{
				Redirect(EffectId.Disease, BlessedState.Normal, default(ActRef));
			}
			break;
		case EffectId.Paralyze:
			if (blessed)
			{
				power /= 4;
			}
			TC.AddCondition<ConParalyze>(power);
			if (flag)
			{
				Redirect(EffectId.Blind, BlessedState.Normal, default(ActRef));
			}
			break;
		case EffectId.Poison:
			if (blessed)
			{
				power /= 4;
			}
			TC.AddCondition<ConPoison>(power);
			if (flag)
			{
				Redirect(EffectId.Paralyze, BlessedState.Normal, default(ActRef));
			}
			break;
		case EffectId.Sleep:
			if (blessed)
			{
				power /= 4;
			}
			TC.AddCondition<ConSleep>(power);
			if (flag)
			{
				Redirect(EffectId.Disease, BlessedState.Normal, default(ActRef));
			}
			break;
		case EffectId.Confuse:
			if (blessed)
			{
				power /= 4;
			}
			TC.AddCondition<ConConfuse>(power);
			if (flag)
			{
				Redirect(EffectId.Fear, BlessedState.Normal, default(ActRef));
			}
			break;
		case EffectId.Blind:
			if (blessed)
			{
				power /= 4;
			}
			TC.AddCondition<ConBlind>(power);
			if (flag)
			{
				Redirect(EffectId.Confuse, BlessedState.Normal, default(ActRef));
			}
			break;
		case EffectId.Disease:
			if (blessed)
			{
				power /= 4;
			}
			TC.AddCondition<ConDisease>(power);
			if (flag)
			{
				Redirect(EffectId.Poison, BlessedState.Normal, default(ActRef));
			}
			break;
		case EffectId.Acid:
		{
			if (blessed)
			{
				power /= 4;
			}
			List<Thing> list7 = TC.things.List((Thing t) => (t.Num <= 1 && t.IsEquipmentOrRanged && !t.IsToolbelt && !t.IsLightsource && t.isEquipped) ? true : false);
			if (list7.Count != 0)
			{
				Thing thing6 = list7.RandomItem();
				TC.Say("acid_hit", TC);
				if (thing6.isAcidproof)
				{
					TC.Say("acid_nullify", thing6);
				}
				else if (thing6.encLV > -5)
				{
					TC.Say("acid_rust", TC, thing6);
					thing6.ModEncLv(-1);
					LayerInventory.SetDirty(thing6);
				}
				if (TC.IsPCParty)
				{
					Tutorial.Reserve("rust");
				}
			}
			break;
		}
		case EffectId.PuddleEffect:
			TC.DamageHP(power / 5, actRef.idEle, power);
			break;
		case EffectId.Acidproof:
			if (blessed)
			{
				power /= 4;
			}
			if (TC.IsPC)
			{
				TC.Say("pc_pain");
			}
			TC.Say("drink_acid", TC);
			TC.DamageHP(power / 5, 923, power);
			break;
		case EffectId.LevelDown:
			Msg.Say("nothingHappens");
			break;
		case EffectId.Love:
		case EffectId.LovePlus:
			if (flag)
			{
				if (CC == TC)
				{
					TC.Say("love_curse_self", TC);
				}
				else
				{
					TC.Say("love_curse", CC, TC);
					TC.ModAffinity(CC, -power / 4, show: false);
				}
				TC.ShowEmo(Emo.angry);
			}
			else
			{
				LoveMiracle(TC, CC, power, id, state);
			}
			break;
		case EffectId.HairGrowth:
			if (flag)
			{
				if (TC.HasElement(1532))
				{
					TC.SetMutation(1532);
				}
				else
				{
					TC.SetMutation(1533, 1);
				}
				TC.c_fur = 0;
				break;
			}
			if (blessed)
			{
				if (TC.HasElement(1533))
				{
					TC.SetMutation(1533);
				}
				else
				{
					TC.SetMutation(1532, 1);
				}
			}
			TC.PlayEffect("aura_heaven");
			TC.PlaySound("godbless");
			if (!TC.HaveFur())
			{
				TC.Say("grow_hair_fail", TC);
				break;
			}
			TC.Say("grow_hair", TC);
			TC.c_fur = 100;
			break;
		case EffectId.Gene:
			GeneMiracle(TC, CC, blessed ? DNA.Type.Superior : (flag ? DNA.Type.Brain : DNA.Type.Default));
			break;
		}
		int GetBlessingDifficulty(Condition c)
		{
			if (c.Type != ConditionType.Buff)
			{
				return 0;
			}
			if (!CC.IsHostile(TC))
			{
				return 1;
			}
			if (c is ConBoost)
			{
				return 5;
			}
			if (c is ConRebirth)
			{
				return 10;
			}
			if (c is ConInvulnerable)
			{
				return 100;
			}
			return 2;
		}
		void Redirect(EffectId _id, BlessedState _state, ActRef _ref1)
		{
			Proc(_id, orgPower, _state, cc, tc, _ref1);
		}
	}

	public static void Poison(Chara tc, Chara c, int power)
	{
		tc.Say("eat_poison", tc);
		tc.Talk("scream");
		if (power > 100000000)
		{
			power = 100000000;
		}
		int num = (int)Mathf.Sqrt(power * 100);
		tc.DamageHP(num * 2 + EClass.rnd(num), 915, power);
		if (!tc.isDead && !tc.IsPC)
		{
			EClass.player.ModKarma(-5);
		}
	}

	public static void LoveMiracle(Chara tc, Chara c, int power, EffectId idEffect = EffectId.Love, BlessedState? state = null)
	{
		if (c == tc)
		{
			tc.Say("love_ground", tc);
		}
		else
		{
			tc.Say("love_chara", c, tc);
		}
		tc.ModAffinity(EClass.pc, power / 4);
		if ((idEffect != EffectId.Love || EClass.rnd(2) != 0) && (!EClass._zone.IsUserZone || tc.IsPCFaction || !EClass.game.principal.disableUsermapBenefit))
		{
			if (idEffect == EffectId.MoonSpear && EClass.rnd(EClass.debug.enable ? 2 : 20) == 0)
			{
				Thing thing = tc.MakeGene();
				tc.GiveBirth(thing, effect: true);
				tc.Say("item_drop", thing);
			}
			else if (idEffect != EffectId.LovePlus && EClass.rnd(2) == 0)
			{
				Thing c2 = tc.MakeMilk(effect: true, 1, addToZone: true, state);
				tc.Say("item_drop", c2);
			}
			else
			{
				Thing c3 = tc.MakeEgg(effect: true, 1, addToZone: true, (idEffect == EffectId.LovePlus) ? 3 : 20, state);
				tc.Say("item_drop", c3);
			}
		}
	}

	public static void GeneMiracle(Chara tc, Chara c, DNA.Type type)
	{
		if (EClass._zone.IsUserZone && !tc.IsPCFactionOrMinion)
		{
			Msg.SayNothingHappen();
			return;
		}
		if (c == tc)
		{
			tc.Say("love_ground", tc);
		}
		else
		{
			tc.Say("love_chara", c, tc);
		}
		Thing t = tc.MakeGene(type);
		tc.GiveBirth(t, effect: true);
	}

	public static Point GetTeleportPos(Point org, int radius = 6)
	{
		Point point = new Point();
		for (int i = 0; i < 10000; i++)
		{
			point.Set(org);
			point.x += EClass.rnd(radius) - EClass.rnd(radius);
			point.z += EClass.rnd(radius) - EClass.rnd(radius);
			if (point.IsValid && point.IsInBounds && !point.cell.blocked && point.Distance(org) >= radius / 3 + 1 - i / 50 && !point.cell.HasZoneStairs())
			{
				return point;
			}
		}
		return org.GetRandomNeighbor().GetNearestPoint();
	}

	public static bool Wish(string s, string name, int power, BlessedState state)
	{
		Msg.thirdPerson1.Set(EClass.pc);
		string netMsg = GameLang.Parse("wish".langGame(), thirdPerson: true, name, s);
		bool net = EClass.core.config.net.enable && EClass.core.config.net.sendEvent;
		List<WishItem> list = new List<WishItem>();
		int wishLv = 10 + power / 4;
		int wishValue = 5000 + power * 50;
		if (state >= BlessedState.Blessed)
		{
			wishLv = wishLv * 150 / 100;
		}
		else if (state <= BlessedState.Cursed)
		{
			wishLv = wishLv * 150 / 100;
			wishValue = 1;
		}
		Debug.Log(power + "/" + wishValue);
		string _s = s.ToLower();
		foreach (CardRow r in EClass.sources.cards.rows)
		{
			if (r.HasTag(CTAG.godArtifact))
			{
				bool flag = false;
				foreach (Religion item in EClass.game.religions.list)
				{
					if (item.giftRank >= 2 && item.IsValidArtifact(r.id))
					{
						flag = true;
					}
				}
				if (!flag)
				{
					continue;
				}
			}
			else if (r.quality >= 4 || r.HasTag(CTAG.noWish))
			{
				switch (r.id)
				{
				case "medal":
				case "plat":
				case "money":
				case "money2":
					break;
				default:
					continue;
				}
			}
			if (r.isChara)
			{
				continue;
			}
			string text = r.GetName().ToLower();
			int score = Compare(_s, text);
			if (score == 0)
			{
				continue;
			}
			list.Add(new WishItem
			{
				score = score,
				n = text,
				action = delegate
				{
					Debug.Log(r.id);
					SourceCategory.Row category = EClass.sources.cards.map[r.id].Category;
					if (category.IsChildOf("weapon") || category.IsChildOf("armor") || category.IsChildOf("ranged"))
					{
						CardBlueprint.SetRarity(Rarity.Legendary);
					}
					Thing thing = ThingGen.Create(r.id, -1, wishLv);
					int num = 1;
					int price = thing.GetPrice(CurrencyType.Money, sell: false, PriceType.Tourism);
					bool flag2 = thing.trait is TraitDeed || thing.rarity >= Rarity.Artifact || thing.source._origin == "artifact_summon";
					switch (thing.id)
					{
					case "rod_wish":
						thing.c_charges = 0;
						break;
					case "money":
						num = EClass.rndHalf(wishValue * 3);
						break;
					case "plat":
						num = EClass.rndHalf(wishValue / 500 + 4);
						break;
					case "money2":
						num = EClass.rndHalf(wishValue / 500 + 4);
						break;
					case "medal":
						num = EClass.rndHalf(wishValue / 2000 + 4);
						break;
					default:
						if (!flag2 && thing.trait.CanStack)
						{
							int num2 = wishValue;
							num2 -= price;
							for (int i = 1; i < 1000; i++)
							{
								int num3 = price + i * 2 * (price + 500);
								if (num3 > 0 && num2 > num3)
								{
									num++;
									num2 -= num3;
								}
							}
						}
						break;
					}
					if (price > 2500 && num > 3)
					{
						num = 3 + (int)Mathf.Sqrt(num - 3);
					}
					if (price > 5000 && num > 2)
					{
						num = 2 + (int)Mathf.Sqrt(num - 2) / 2;
					}
					if (price > 10000 && num > 1)
					{
						num = 1 + Mathf.Min((int)Mathf.Sqrt(num - 1) / 3, 2);
					}
					if (num < 1)
					{
						num = 1;
					}
					thing.SetNum(num);
					Debug.Log(_s + "/" + num + "/" + score);
					if (thing.HasTag(CTAG.godArtifact))
					{
						Religion.Reforge(thing.id);
					}
					else
					{
						EClass._zone.AddCard(thing, EClass.pc.pos);
					}
					netMsg = netMsg + Lang.space + GameLang.Parse("wishNet".langGame(), Msg.IsThirdPerson(thing), Msg.GetName(thing).ToTitleCase());
					if (net)
					{
						Net.SendChat(name, netMsg, ChatCategory.Wish, Lang.langCode);
					}
					Msg.Say("dropReward");
				}
			});
		}
		if (list.Count == 0)
		{
			netMsg = netMsg + Lang.space + "wishFail".langGame();
			if (net)
			{
				Net.SendChat(name, netMsg, ChatCategory.Wish, Lang.langCode);
			}
			Msg.Say("wishFail");
			return false;
		}
		list.Sort((WishItem a, WishItem b) => b.score - a.score);
		foreach (WishItem item2 in list)
		{
			Debug.Log(item2.score + "/" + s + "/" + item2.n);
		}
		list[0].action();
		return true;
	}

	public static int Compare(string s, string t)
	{
		if (s.IsEmpty())
		{
			return 0;
		}
		int num = 0;
		if (t == s)
		{
			num += 100 + EClass.rnd(30);
		}
		if (t.Contains(s))
		{
			num += 100 + EClass.rnd(30);
		}
		return num;
	}
}
