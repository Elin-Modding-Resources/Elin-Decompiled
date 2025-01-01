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
		if (id == EffectId.Explosive && actref.refThing != null)
		{
			power = power * actref.refThing.material.hardness / 10;
		}
		string text = id.ToString();
		string text2 = (EClass.sources.calc.map.ContainsKey(text) ? text : (text.ToLower() + "_"));
		foreach (Point p in points)
		{
			bool flag2 = true;
			switch (id)
			{
			case EffectId.Explosive:
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
			{
				effect = Effect.Get("spell_arrow");
				effect.sr.color = elementRef.colorSprite;
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
				effect = Effect.Get("Element/ball_" + ((e.id == 0) ? "Void" : e.source.alias.Remove(0, 3)));
				if (effect == null)
				{
					effect = Effect.Get("Element/ball_Fire");
				}
				float startDelay = ((id == EffectId.Meteor) ? 0.1f : 0.04f) * (float)CC.pos.Distance(p);
				effect.SetStartDelay(startDelay);
				effect2.SetStartDelay(startDelay);
				break;
			}
			}
			if (effect2 != null)
			{
				effect2.SetParticleColor(elementRef.colorTrail, changeMaterial: true, "_TintColor").Play(from);
			}
			if (effect != null)
			{
				if (id == EffectId.Arrow)
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
					else if (e.id != 910 || !item.IsFood || !item.category.IsChildOf("foodstuff"))
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
				if ((uint)(id - 249) <= 1u && c.isChara && CC.isChara)
				{
					c.Chara.RequestProtection(CC.Chara, delegate(Chara a)
					{
						c = a;
					});
				}
				int num4 = 0;
				bool isChara = CC.isChara;
				if (id == EffectId.Suicide)
				{
					num4 = CC.MaxHP * 2;
					num4 = num4 * 100 / (50 + point.Distance(p) * 75);
					if (c.HasTag(CTAG.suicide) && !c.HasCondition<ConWet>())
					{
						list.Add(c);
					}
				}
				else
				{
					Dice dice = Dice.Create(text2, power, CC, actref.act);
					if (dice == null)
					{
						Debug.Log(text2);
					}
					num4 = dice.Roll();
					if (id == EffectId.Earthquake)
					{
						if (c.HasCondition<ConGravity>())
						{
							num4 = dice.RollMax() * 2;
						}
						else if (c.isChara && c.Chara.IsLevitating)
						{
							num4 /= 2;
						}
					}
					if (id == EffectId.Ball || id == EffectId.BallBubble || id == EffectId.Explosive)
					{
						num4 = num4 * 100 / (90 + point.Distance(p) * 10);
					}
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
						if (num4 == 0)
						{
							continue;
						}
					}
					if (CC.HasElement(1214) || (!CC.IsPC && (CC.IsPCFaction || CC.IsPCFactionMinion) && EClass.pc.HasElement(1214) && EClass.rnd(5) != 0))
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
						num4 /= 2;
						break;
					}
				}
				if (RapidCount > 0)
				{
					num4 = num4 * 100 / (100 + RapidCount * 50);
				}
				num4 = num4 * Act.powerMod / 100;
				c.DamageHP(num4, e.id, power * num / 100, AttackSource.None, chara ?? CC);
				if (id == EffectId.Explosive && CC.trait is TraitCookerMicrowave)
				{
					chara = EClass.pc;
				}
				if (chara != null && chara.IsAliveInCurrentZone)
				{
					chara.DoHostileAction(c);
				}
				num2++;
			}
			if ((id == EffectId.Explosive || id == EffectId.Suicide) && ((id != EffectId.Suicide && id != EffectId.Meteor) || !EClass._zone.IsPCFaction))
			{
				int num6 = id switch
				{
					EffectId.Suicide => CC.LV / 3 + 40, 
					EffectId.Meteor => 50 + power / 20, 
					_ => (actref.refThing != null) ? actref.refThing.material.hardness : (30 + power / 20), 
				};
				bool flag5 = EClass._zone.HasLaw && !EClass._zone.IsPCFaction && (CC.IsPC || (id == EffectId.Explosive && actref.refThing == null)) && !(EClass._zone is Zone_Vernis);
				if (p.HasObj && p.cell.matObj.hardness <= num6)
				{
					EClass._map.MineObj(p);
					if (flag5)
					{
						EClass.player.ModKarma(-1);
					}
				}
				if (!p.HasObj && p.HasBlock && p.matBlock.hardness <= num6)
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
				int num7 = 0;
				if (id == EffectId.Meteor)
				{
					num7 = 2;
				}
				if (EClass._zone.IsPCFaction && EClass._zone.branch.HasItemProtection)
				{
					num7 = 0;
				}
				if (num7 > EClass.rnd(10))
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
			List<Point> list4 = EClass._map.ListPointsInCircle(CC.pos, 12f, mustBeWalkable: false);
			if (list4.Count == 0)
			{
				list4.Add(CC.pos.Copy());
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
			DamageEle(CC, id, power, element, list4, actRef, "spell_earthquake");
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
		{
			List<Point> list5 = new List<Point>();
			list5.Add(tp.Copy());
			EClass.Wait(0.3f, CC);
			TryDelay(delegate
			{
				CC.PlaySound("spell_hand");
			});
			if (!DamageEle(CC, id, power, element, list5, actRef, (id == EffectId.DrainBlood) ? "" : "spell_hand"))
			{
				CC.Say("spell_hand_miss", CC, element.Name.ToLower());
			}
			return;
		}
		case EffectId.Arrow:
		{
			List<Point> list = new List<Point>();
			list.Add(tp.Copy());
			CC.Say("spell_arrow", CC, element.Name.ToLower());
			EClass.Wait(0.5f, CC);
			TryDelay(delegate
			{
				CC.PlaySound("spell_arrow");
			});
			DamageEle(CC, id, power, element, list, actRef, "spell_arrow");
			return;
		}
		case EffectId.Summon:
		{
			CC.Say("summon_ally", CC);
			if (EClass._zone.CountMinions(CC) > CC.MaxSummon || CC.c_uidMaster != 0)
			{
				CC.Say("summon_ally_fail", CC);
				return;
			}
			string id3 = actRef.n1;
			int num3 = 1;
			int num4 = -1;
			int radius = 3;
			bool flag3 = false;
			int num5 = -1;
			switch (actRef.n1)
			{
			case "shadow":
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
			}
			for (int j = 0; j < num3; j++)
			{
				if (EClass._zone.CountMinions(CC) > CC.MaxSummon)
				{
					break;
				}
				Point point = tp.GetRandomPoint(radius)?.GetNearestPoint(allowBlock: false, allowChara: false);
				if (point == null || !point.IsValid)
				{
					continue;
				}
				Chara chara2 = null;
				if (num5 != -1)
				{
					CardBlueprint.Set(new CardBlueprint
					{
						lv = num5
					});
				}
				chara2 = actRef.n1 switch
				{
					"yeek" => CharaGen.CreateFromFilter(SpawnListChara.Get("yeek", (SourceChara.Row r) => r.race == "yeek"), power / 10), 
					"pawn" => CharaGen.CreateFromFilter("c_pawn", power / 10), 
					"monster" => CharaGen.CreateFromFilter("c_dungeon", power / 10), 
					"animal" => CharaGen.CreateFromFilter("c_animal", power / 15), 
					"fire" => CharaGen.CreateFromElement("Fire", power / 10), 
					_ => CharaGen.Create(id3, power / 10), 
				};
				if (chara2 == null)
				{
					continue;
				}
				int num6 = -1;
				num6 = ((!(actRef.n1 == "shadow")) ? (chara2.LV * (100 + power / 10) / 100 + power / 30) : (power / 10 + 1));
				if (chara2.LV < num6)
				{
					chara2.SetLv(num6);
				}
				chara2.interest = 0;
				if (chara2.HaveFur())
				{
					chara2.c_fur = -1;
				}
				string n = actRef.n1;
				if (!(n == "shadow"))
				{
					if (n == "special_force")
					{
						chara2.homeZone = EClass._zone;
					}
				}
				else
				{
					chara2.hp = chara2.MaxHP / 2;
				}
				EClass._zone.AddCard(chara2, point);
				if (!(actRef.n1 == "monster") || actRef.refThing == null)
				{
					chara2.MakeMinion(CC);
				}
				if (num4 != -1)
				{
					chara2.SetSummon(num4);
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
		{
			if (EClass._zone.CountMinions(CC) > CC.MaxSummon || CC.c_uidMaster != 0)
			{
				CC.Say("summon_ally_fail", CC);
				return;
			}
			CC.Say("spell_funnel", CC, element.Name.ToLower());
			CC.PlaySound("spell_funnel");
			Chara chara = CharaGen.Create("bit");
			chara.SetMainElement(element.source.alias, element.Value, elemental: true);
			chara.SetSummon(20 + power / 20 + EClass.rnd(10));
			chara.SetLv(power / 15);
			EClass._zone.AddCard(chara, tp.GetNearestPoint(allowBlock: false, allowChara: false));
			chara.PlayEffect("teleport");
			chara.MakeMinion(CC);
			return;
		}
		case EffectId.Breathe:
		{
			List<Point> list7 = EClass._map.ListPointsInArc(CC.pos, tp, 7, 35f);
			if (list7.Count == 0)
			{
				list7.Add(CC.pos.Copy());
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
			DamageEle(CC, id, power, element, list7, actRef, "spell_breathe");
			return;
		}
		case EffectId.Scream:
			CC.PlaySound("scream");
			CC.PlayEffect("scream");
			{
				foreach (Point item in EClass._map.ListPointsInCircle(cc.pos, 6f, mustBeWalkable: false, los: false))
				{
					foreach (Chara chara3 in item.Charas)
					{
						if (chara3.ResistLv(957) <= 0)
						{
							chara3.AddCondition<ConParalyze>(power);
						}
					}
				}
				return;
			}
		case EffectId.Ball:
		case EffectId.Explosive:
		case EffectId.BallBubble:
		case EffectId.Suicide:
		{
			float radius2 = ((id == EffectId.Suicide) ? 3.5f : ((float)((id == EffectId.BallBubble) ? 2 : 5)));
			if (id == EffectId.Explosive && actRef.refThing != null)
			{
				radius2 = 2f;
			}
			if (id == EffectId.Suicide)
			{
				if (CC.MainElement != Element.Void)
				{
					element = CC.MainElement;
				}
				if (CC.HasTag(CTAG.kamikaze))
				{
					radius2 = 1.5f;
				}
			}
			bool flag4 = id == EffectId.Explosive || id == EffectId.Suicide;
			List<Point> list6 = EClass._map.ListPointsInCircle(cc.pos, radius2, !flag4, !flag4);
			if (list6.Count == 0)
			{
				list6.Add(cc.pos.Copy());
			}
			cc.Say((id == EffectId.Suicide) ? "abSuicide" : "spell_ball", cc, element.Name.ToLower());
			EClass.Wait(0.8f, cc);
			TryDelay(delegate
			{
				cc.PlaySound("spell_ball");
			});
			if (cc.IsInMutterDistance() && !EClass.core.config.graphic.disableShake)
			{
				Shaker.ShakeCam("ball");
			}
			DamageEle(actRef.origin ?? cc, id, power, element, list6, actRef, (id == EffectId.Suicide) ? "suicide" : "spell_ball");
			if (id == EffectId.Suicide && CC.IsAliveInCurrentZone)
			{
				CC.Die();
			}
			return;
		}
		case EffectId.Bolt:
		{
			List<Point> list2 = EClass._map.ListPointsInLine(CC.pos, tp, 10);
			if (list2.Count == 0)
			{
				list2.Add(CC.pos.Copy());
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
			DamageEle(CC, id, power, element, list2, actRef, "spell_bolt");
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
			int id2 = id switch
			{
				EffectId.MistOfDarkness => 6, 
				EffectId.Bubble => 5, 
				EffectId.Puddle => 4, 
				_ => 7, 
			};
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
		bool flag5 = true;
		foreach (Card item2 in list8)
		{
			if (tc == null || item2 == tc)
			{
				Proc(id, power, state, CC, item2, actRef);
				if (flag2 && item2.isChara && item2 != CC)
				{
					CC.DoHostileAction(item2);
				}
				if (actRef.refThing == null || !(actRef.refThing.trait is TraitRod) || (uint)(id - 200) <= 3u)
				{
					return;
				}
				flag5 = false;
			}
		}
		if (flag5)
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
			if (randomPoint == null || randomPoint.Equals(CC.pos) || !randomPoint.IsValid || EClass._zone.IsRegion || CC.HasCondition<ConPoison>() || CC.HasCondition<ConConfuse>() || CC.HasCondition<ConDim>() || CC.HasCondition<ConParalyze>() || CC.HasCondition<ConSleep>() || CC.HasCondition<ConBurning>() || CC.HasCondition<ConFreeze>() || CC.HasCondition<ConMiasma>())
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
			EClass.game.religions.Element.Talk("ability");
			Dice dice = Dice.Create("ActManaAbsorb", power, CC, actRef.act);
			TC.mana.Mod(dice.Roll());
			TC.PlaySound("heal");
			TC.PlayEffect("heal");
			if (TC.IsPC)
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
			int num = power / 50 + 1 + EClass.rnd(3);
			List<Thing> source = TC.things.List((Thing t) => t.c_IDTState == 0);
			for (int i = 0; i < num; i++)
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
			int num5 = (flag4 ? 4 : 2) + (blessed ? 1 : 0);
			if (tc.encLV >= num5)
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
			bool flag5 = id == EffectId.GreaterIdentify;
			if (flag)
			{
				Redirect(EffectId.ForgetItems, flag5 ? BlessedState.Cursed : BlessedState.Normal, default(ActRef));
				break;
			}
			if (!tc.isThing)
			{
				int count = ((!blessed) ? 1 : (flag5 ? (2 + EClass.rnd(2)) : (3 + EClass.rnd(3))));
				LayerDragGrid.CreateIdentify(CC, flag5, state, 0, count);
				return;
			}
			cc.PlaySound("identify");
			cc.PlayEffect("identify");
			tc.Thing.Identify(cc.IsPCParty, (!flag5) ? IDTSource.Identify : IDTSource.SuperiorIdentify);
			break;
		}
		case EffectId.Uncurse:
		{
			if (!tc.isThing)
			{
				LayerDragGrid.CreateUncurse(CC, state);
				return;
			}
			Thing thing3 = tc.Thing;
			if (thing3.blessedState == BlessedState.Cursed)
			{
				thing3.SetBlessedState(BlessedState.Normal);
			}
			else if (thing3.blessedState == BlessedState.Doomed)
			{
				thing3.SetBlessedState(BlessedState.Normal);
			}
			thing3.GetRootCard()?.TryStack(thing3);
			LayerInventory.SetDirty(thing3);
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
			int num4 = (tc.isWeightChanged ? tc.c_weight : tc.Thing.source.weight);
			tc.isWeightChanged = true;
			Element orCreateElement = tc.elements.GetOrCreateElement(64);
			Element orCreateElement2 = tc.elements.GetOrCreateElement(65);
			Element orCreateElement3 = tc.elements.GetOrCreateElement(67);
			Element orCreateElement4 = tc.elements.GetOrCreateElement(66);
			bool flag3 = tc.IsEquipmentOrRanged || tc.IsThrownWeapon || tc.IsAmmo;
			if (flag)
			{
				num4 = (int)(0.01f * (float)num4 * (float)power * 0.75f + 500f);
				if (num4 < 0 || num4 > 10000000)
				{
					num4 = 10000000;
					flag3 = false;
				}
				if (flag3)
				{
					if (tc.IsWeapon || tc.IsThrownWeapon)
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
				num4 = num4 * (100 - power / 10) / 100;
				if (blessed)
				{
					power /= 4;
				}
				if (flag3)
				{
					if (tc.IsWeapon || tc.IsThrownWeapon)
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
			tc.c_weight = num4;
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
			Thing thing2 = ThingGen.Create(tc.id, -1, tc.LV * power / 100);
			thing2.SetBlessedState(state);
			tc.Destroy();
			CC.Pick(thing2, msg: false);
			if (!CC.IsPC)
			{
				CC.TryEquip(thing2);
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
				bool num2 = id == EffectId.ChangeMaterialGreater;
				bool flag2 = id == EffectId.ChangeMaterialLesser;
				string text2 = tc.Thing.source.tierGroup;
				Dictionary<string, SourceMaterial.TierList> tierMap = SourceMaterial.tierMap;
				int num3 = 1;
				if (flag)
				{
					num3 -= 2;
				}
				if (blessed)
				{
					num3++;
				}
				if (num2)
				{
					num3++;
				}
				if (flag2)
				{
					num3 -= 2;
				}
				num3 = Mathf.Clamp(num3 + EClass.rnd(2), 0, 4);
				if (EClass.rnd(10) == 0)
				{
					text2 = ((text2 == "metal") ? "leather" : "metal");
				}
				SourceMaterial.TierList tierList = (text2.IsEmpty() ? tierMap.RandomItem() : tierMap[text2]);
				for (int j = 0; j < 1000; j++)
				{
					row = tierList.tiers[num3].Select();
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
				foreach (Element item2 in tc.elements.dict.Values.ToList())
				{
					if (item2.IsTrait && item2.vBase != 0)
					{
						tc.elements.ModBase(item2.id, -item2.vBase);
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
			if (!tc.HasHost && !flag)
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
		case EffectId.ShutterHex:
		{
			if (!CC.IsHostile(TC))
			{
				break;
			}
			int hex2 = 0;
			foreach (Condition condition4 in TC.conditions)
			{
				if (condition4.Type == ConditionType.Debuff)
				{
					hex2++;
				}
			}
			if (hex2 == 0)
			{
				CC.SayNothingHappans();
				break;
			}
			TC.pos.PlayEffect("holyveil");
			TC.pos.PlaySound("holyveil");
			TC.pos.PlaySound("atk_eleSound");
			TC.conditions.ForeachReverse(delegate(Condition c)
			{
				if (c.Type == ConditionType.Debuff)
				{
					c.Kill();
				}
			});
			TC.Say("abShutterHex", TC);
			TC.pos.ForeachNeighbor(delegate(Point p)
			{
				foreach (Chara item3 in p.ListCharas())
				{
					if (!item3.IsHostile(CC))
					{
						break;
					}
					int dmg2 = Dice.Create("SpShutterHex", power * hex2, CC, actRef.act).Roll();
					item3.DamageHP(dmg2, 919, power, AttackSource.None, CC);
				}
			});
			break;
		}
		case EffectId.Draw:
		{
			Point point = CC.pos.GetRandomPoint(1)?.GetNearestPoint(allowBlock: false, allowChara: false);
			if (point == null || !CC.CanSeeLos(point) || (CC.IsPCFactionOrMinion && TC.IsPCFactionOrMinion && TC.isRestrained))
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
			Thing thing6 = null;
			bool flag8 = actRef.n1 == "food";
			if (actRef.n1 == "money")
			{
				int currency = TC.GetCurrency();
				if (currency > 0)
				{
					currency = Mathf.Clamp(EClass.rnd(currency / 10), 1, 100 + EClass.rndHalf(CC.LV * 200));
					thing6 = ThingGen.Create("money").SetNum(currency);
					TC.ModCurrency(-currency);
				}
			}
			else
			{
				Func<Thing, bool> func = (Thing t) => true;
				if (flag8)
				{
					func = (Thing t) => t.IsFood;
				}
				List<Thing> list7 = TC.things.List(delegate(Thing t)
				{
					if (t.parentCard?.trait is TraitChestMerchant || t.trait is TraitTool || t.IsThrownWeapon)
					{
						return false;
					}
					return t.trait.CanBeDestroyed && t.things.Count == 0 && t.invY != 1 && t.trait.CanBeStolen && !t.trait.CanOnlyCarry && !t.IsUnique && !t.isEquipped && t.blessedState == BlessedState.Normal && func(t);
				}, onlyAccessible: true);
				if (list7.Count > 0)
				{
					thing6 = list7.RandomItem();
					if (thing6.Num > 1)
					{
						thing6 = thing6.Split(1);
					}
				}
				CC.AddCooldown(6640, 200);
			}
			if (thing6 == null)
			{
				CC.Say("abStealNothing", CC, TC);
				break;
			}
			thing6.SetInt(116, 1);
			TC.PlaySound(thing6.material.GetSoundDrop(thing6.sourceCard));
			CC.Pick(thing6, msg: false);
			CC.Say("abSteal", CC, TC, thing6.Name);
			if (actRef.n1 == "food")
			{
				if (CC.hunger.value != 0)
				{
					CC.InstantEat(thing6);
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
			List<Thing> list6 = TC.things.List(delegate(Thing t)
			{
				if (!t.isEquipped || t.blessedState == BlessedState.Doomed || t.IsToolbelt)
				{
					return false;
				}
				return (t.blessedState < BlessedState.Blessed || EClass.rnd(10) == 0) ? true : false;
			});
			if (list6.Count == 0)
			{
				CC.SayNothingHappans();
				break;
			}
			Thing thing5 = list6.RandomItem();
			TC.Say("curse_hit", TC, thing5);
			thing5.SetBlessedState((thing5.blessedState == BlessedState.Cursed) ? BlessedState.Doomed : BlessedState.Cursed);
			LayerInventory.SetDirty(thing5);
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
				int num10 = 0;
				if ((t.isEquipped || t.IsRangedWeapon || blessed) && t.blessedState < BlessedState.Normal)
				{
					if (t.blessedState == BlessedState.Cursed)
					{
						num10 = EClass.rnd(200);
					}
					if (t.blessedState == BlessedState.Doomed)
					{
						num10 = EClass.rnd(1000);
					}
					if (blessed)
					{
						num10 /= 2;
					}
					if (id == EffectId.UncurseEQGreater)
					{
						num10 /= 10;
					}
					if (power >= num10)
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
			foreach (Thing item4 in list)
			{
				item4.GetRootCard()?.TryStack(item4);
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
			List<SourceStat.Row> list4 = EClass.sources.stats.rows.Where((SourceStat.Row con) => con.tag.Contains("random") && con.group == (hex ? "Debuff" : "Buff")).ToList();
			int power2 = power;
			for (int k = 0; k < 4 + EClass.rnd(2); k++)
			{
				SourceStat.Row row2 = list4.RandomItem();
				list4.Remove(row2);
				Proc(hex ? EffectId.Debuff : EffectId.Buff, CC, TC, power2, new ActRef
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
		{
			CC.DoHostileAction(TC);
			bool isPowerful = TC.IsPowerful;
			string n = actRef.n1;
			int a2 = power;
			int num6 = TC.WIL * (isPowerful ? 20 : 5);
			ConHolyVeil condition = TC.GetCondition<ConHolyVeil>();
			if (condition != null)
			{
				num6 += condition.power * 5;
			}
			if (EClass.rnd(a2) < num6 / EClass.sources.stats.alias[n].hexPower && EClass.rnd(10) != 0)
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
			Chara chara = CharaGen.CreateFromFilter("chara", EClass.pc.LV);
			EClass._zone.AddCard(chara, cc.pos.GetNearestPoint(allowBlock: false, allowChara: false));
			chara.MakeAlly(msg: false);
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
			if (faith.IsEyth)
			{
				tc.SayNothingHappans();
				break;
			}
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
			if (blessed && tc.bio.age > 1)
			{
				tc.Say("ageDown", tc);
				tc.bio.age--;
			}
			else if (flag)
			{
				tc.Say("ageUp", tc);
				tc.bio.age++;
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
			List<KeyValuePair<int, Chara>> list3 = EClass.game.cards.globalCharas.Where((KeyValuePair<int, Chara> a) => a.Value.isDead && a.Value.faction == EClass.pc.faction && !a.Value.isSummon && a.Value.c_wasInPcParty).ToList();
			if (TC.IsPCFaction || TC.IsPCFactionMinion)
			{
				if (TC.IsPC && list3.Count == 0)
				{
					list3 = EClass.game.cards.globalCharas.Where((KeyValuePair<int, Chara> a) => a.Value.CanRevive() && a.Value.isDead && a.Value.faction == EClass.pc.faction && !a.Value.isSummon).ToList();
				}
				if (list3.Count > 0)
				{
					list3.RandomItem().Value.Chara.GetRevived();
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
			bool flag9 = id == EffectId.DamageBody || id == EffectId.DamageBodyGreat;
			bool mind2 = id == EffectId.DamageMind || id == EffectId.DamageMindGreat;
			int num9 = ((id == EffectId.DamageBody || id == EffectId.DamageMind) ? 1 : (4 + EClass.rnd(4)));
			if (id == EffectId.Weaken)
			{
				flag9 = EClass.rnd(2) == 0;
				mind2 = !flag9;
				num9 = 1;
			}
			else
			{
				TC.PlayEffect("debuff");
				TC.PlaySound("debuff");
			}
			TC.Say(flag9 ? "damageBody" : "damageMind", TC);
			for (int m = 0; m < num9; m++)
			{
				TC.DamageTempElements(power, flag9, mind2);
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
			bool flag6 = id == EffectId.EnhanceBody || id == EffectId.EnhanceBodyGreat;
			bool mind = id == EffectId.EnhanceMind || id == EffectId.EnhanceMindGreat;
			int num7 = ((id == EffectId.EnhanceBody || id == EffectId.EnhanceMind) ? 1 : (4 + EClass.rnd(4)));
			TC.Say(flag6 ? "enhanceBody" : "enhanceMind", TC);
			TC.PlayEffect("buff");
			TC.PlaySound("buff");
			for (int l = 0; l < num7; l++)
			{
				TC.EnhanceTempElements(power, flag6, mind);
			}
			break;
		}
		case EffectId.RestoreBody:
		case EffectId.RestoreMind:
		{
			bool flag7 = id == EffectId.RestoreBody;
			if (flag)
			{
				Redirect(flag7 ? EffectId.DamageBodyGreat : EffectId.DamageMindGreat, BlessedState.Normal, default(ActRef));
				break;
			}
			TC.Say(flag7 ? "restoreBody" : "restoreMind", TC);
			TC.PlaySound("heal");
			TC.PlayEffect("heal");
			TC.CureHost(flag7 ? CureType.CureBody : CureType.CureMind, power, state);
			if (blessed)
			{
				Redirect(flag7 ? EffectId.EnhanceBodyGreat : EffectId.EnhanceMindGreat, BlessedState.Normal, default(ActRef));
			}
			break;
		}
		case EffectId.HealComplete:
			Dice.Create("SpHealLight", power, CC, actRef.act);
			TC.HealHPHost(9999, HealSource.Magic);
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
			int num8 = Dice.Create((actRef.act != null && EClass.sources.calc.map.ContainsKey(actRef.act.ID)) ? actRef.act.ID : "SpHealLight", power, CC, actRef.act).Roll();
			if (flag)
			{
				TC.DamageHP(num8 / 2, 919, power);
				break;
			}
			TC.HealHPHost(num8, HealSource.Magic);
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
			foreach (Condition item5 in TC.conditions.Copy())
			{
				if (item5.Type == ConditionType.Debuff && !item5.IsKilled && EClass.rnd(power * 2) > EClass.rnd(item5.power))
				{
					CC.Say("removeHex", TC, item5.Name.ToLower());
					item5.Kill();
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
				int dmg = ((TC.hp > 10) ? (TC.hp - EClass.rnd(10)) : 10000);
				TC.DamageHP(dmg, AttackSource.None, CC);
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
			List<Thing> list5 = TC.things.List((Thing t) => (t.Num <= 1 && t.IsEquipmentOrRanged && !t.IsToolbelt && !t.IsLightsource && t.isEquipped) ? true : false);
			if (list5.Count != 0)
			{
				Thing thing4 = list5.RandomItem();
				TC.Say("acid_hit", TC);
				if (thing4.isAcidproof)
				{
					TC.Say("acid_nullify", thing4);
				}
				else if (thing4.encLV > -5)
				{
					TC.Say("acid_rust", TC, thing4);
					thing4.ModEncLv(-1);
					LayerInventory.SetDirty(thing4);
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
				LoveMiracle(TC, CC, power);
			}
			break;
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
		int num = (int)Mathf.Sqrt(power * 100);
		tc.DamageHP(num * 2 + EClass.rnd(num), 915, power);
		if (!tc.isDead && !tc.IsPC)
		{
			EClass.player.ModKarma(-5);
		}
	}

	public static void LoveMiracle(Chara tc, Chara c, int power)
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
		if (EClass.rnd(2) != 0)
		{
			if (EClass.rnd(2) == 0)
			{
				tc.MakeMilk();
			}
			else
			{
				tc.MakeEgg();
			}
		}
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
		int wishValue = power * 200;
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
					if (item.IsValidArtifact(r.id))
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
					switch (thing.id)
					{
					case "rod_wish":
						thing.c_charges = 0;
						break;
					case "money":
						num = EClass.rndHalf(wishValue);
						break;
					case "plat":
						num = EClass.rndHalf(wishValue / 2000 + 4);
						break;
					case "money2":
						num = EClass.rndHalf(wishValue / 1000 + 4);
						break;
					case "medal":
						num = EClass.rndHalf(wishValue / 3000 + 4);
						break;
					}
					if (num < 1)
					{
						num = 1;
					}
					if (num == 1 && thing.trait.CanStack)
					{
						int num2 = wishValue;
						int price = thing.GetPrice();
						for (int i = 0; i < 1000; i++)
						{
							int num3 = price + 500 + i * Mathf.Max(price, 200);
							if (num2 > num3)
							{
								num++;
								num2 -= num3;
							}
						}
					}
					if (thing.trait is TraitDeed)
					{
						num = 1;
					}
					if (thing.source._origin == "artifact_summon")
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
			num += 100;
		}
		if (t.Contains(s))
		{
			num += 100;
		}
		return num;
	}
}
