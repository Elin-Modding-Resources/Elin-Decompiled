using System.Collections.Generic;

public class ActMelee : ActBaseAttack
{
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
		if (Act.CC.Dist(Act.TC) > PerformDistance)
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
		return Attack();
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
		CellEffect effect = Act.TP.cell.effect;
		if (effect != null && effect.id == 6 && EClass.rnd(2) == 0)
		{
			Act.CC.PlaySound("miss");
			Act.CC.Say("abMistOfDarkness_miss", Act.CC);
			return true;
		}
		bool hasHit = false;
		bool flag = false;
		bool usedTalisman = false;
		int count = 0;
		int num = Act.CC.Dist(Act.TC);
		Point obj = Act.TC.pos.Copy();
		foreach (BodySlot slot in Act.CC.body.slots)
		{
			if (Act.TC == null || !Act.TC.IsAliveInCurrentZone)
			{
				return true;
			}
			if (slot.thing == null || slot.elementId != 35 || slot.thing.source.offense.Length < 2)
			{
				continue;
			}
			Thing w = slot.thing;
			if (num > 1 && num > w.Evalue(666) + 1)
			{
				continue;
			}
			flag = true;
			if (w.IsMeleeWithAmmo && Act.CC.IsPC && w.c_ammo <= 0 && !Act.CC.HasCondition<ConReload>())
			{
				ActRanged.TryReload(w);
			}
			int num2 = ((w != null) ? w.Evalue(606) : 0);
			if (w != null)
			{
				w.Evalue(607);
			}
			List<Point> list = EClass._map.ListPointsInLine(Act.CC.pos, Act.TC.pos, num2 / 10 + ((num2 % 10 > EClass.rnd(10)) ? 1 : 0) + 1);
			Attack(Act.TC, Act.TP);
			if (num2 > 0)
			{
				foreach (Point item in list)
				{
					if (!item.Equals(obj))
					{
						item.FirstChara?.IsHostile(Act.CC);
					}
				}
			}
			count++;
			void Attack(Card _tc, Point _tp)
			{
				Act.TC = _tc;
				Act.TP = _tp;
				AttackProcess.Current.Prepare(Act.CC, w, Act.TC, Act.TP, count);
				bool flag2 = AttackProcess.Current.Perform(count, hasHit, dmgMulti, maxRoll);
				if (flag2)
				{
					hasHit = true;
				}
				if (w.c_ammo > 0 && !Act.CC.HasCondition<ConReload>())
				{
					bool flag3 = true;
					TraitAmmo traitAmmo = ((w.ammoData == null) ? null : (w.ammoData.trait as TraitAmmo));
					if (traitAmmo != null && traitAmmo is TraitAmmoTalisman traitAmmoTalisman)
					{
						flag3 = false;
						if (flag2 && !usedTalisman && Act.TC != null && Act.TC.IsAliveInCurrentZone)
						{
							Act act = Act.CC.elements.GetElement(traitAmmoTalisman.owner.refVal)?.act ?? ACT.Create(traitAmmoTalisman.owner.refVal);
							Act.powerMod = traitAmmo.owner.encLV;
							if (act.Perform(Act.CC, Act.TC, Act.TP))
							{
								usedTalisman = true;
								flag3 = true;
								int spellExp = Act.CC.elements.GetSpellExp(Act.CC, act, 200);
								Act.CC.ModExp(act.id, spellExp);
							}
							Act.powerMod = 100;
						}
					}
					if (flag3)
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
				Act.CC.DoHostileAction(Act.TC);
			}
		}
		if (!flag)
		{
			AttackProcess.Current.Prepare(Act.CC, null, Act.TC, Act.TP);
			if (AttackProcess.Current.Perform(count, hasHit, dmgMulti, maxRoll))
			{
				hasHit = true;
			}
			Act.CC.DoHostileAction(Act.TC);
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
	}
}
