using Newtonsoft.Json;

public class ConSleep : BadCondition
{
	public bool succubusChecked;

	[JsonProperty]
	public int pcSleep;

	[JsonProperty]
	public Thing pcBed;

	[JsonProperty]
	public Thing pcPillow;

	[JsonProperty]
	public ItemPosition posBed;

	[JsonProperty]
	public ItemPosition posPillow;

	[JsonProperty]
	public bool pickup;

	[JsonProperty]
	public bool slept;

	[JsonProperty]
	public int uidRide;

	[JsonProperty]
	public int uidParasite;

	public override Emo2 EmoIcon => Emo2.speeing;

	public override bool ConsumeTurn => true;

	public override bool CancelAI => false;

	public override int GetPhase()
	{
		return 0;
	}

	public override void SetOwner(Chara _owner, bool onDeserialize = false)
	{
		base.SetOwner(_owner);
		owner.conSleep = this;
		if (owner.renderer != null)
		{
			owner.renderer.RefreshSprite();
		}
	}

	public override void OnBeforeStart()
	{
		TraitPillow traitPillow = owner.pos.FindThing<TraitPillow>();
		if (traitPillow != null)
		{
			owner.Say("pillow", owner, traitPillow.owner);
		}
	}

	public override void Tick()
	{
		if (!succubusChecked && EClass._zone.isStarted && (base.value > 50 || pcSleep != 0))
		{
			SuccubusVisit(owner);
			succubusChecked = true;
		}
		if (owner.IsPC && pcSleep > 0)
		{
			if (!EClass._zone.IsRegion)
			{
				if (EClass.pc.ride != null)
				{
					uidRide = EClass.pc.ride.uid;
					ActRide.Unride(EClass.pc, parasite: false, talk: false);
				}
				if (EClass.pc.parasite != null)
				{
					uidParasite = EClass.pc.parasite.uid;
					ActRide.Unride(EClass.pc, parasite: true, talk: false);
				}
			}
			pcSleep--;
			if (pcSleep != 0)
			{
				return;
			}
			int num = EClass.pc.sleepiness.GetPhase();
			int num2 = ((num >= 3) ? 12 : ((num >= 2) ? 10 : ((num >= 1) ? 7 : 5)));
			num2 += EClass.pc.stamina.GetPhase() switch
			{
				2 => 2, 
				1 => 3, 
				0 => 4, 
				_ => 0, 
			};
			num2 += EClass.rnd(3) - EClass.rnd(2);
			SuccubusSleep(EClass.pc);
			if (EClass.pc.isDead)
			{
				return;
			}
			base.value = 1;
			slept = true;
			if (!EClass.pc.pos.IsInSpot<TraitPillowStrange>())
			{
				foreach (Chara chara in EClass._map.charas)
				{
					if (chara.host != null || chara.noMove || chara.conSuspend != null || chara.isRestrained || chara.IsPC)
					{
						continue;
					}
					bool flag = chara.GetBool(123);
					if (!flag && chara.IsPCFaction && chara.race.tag.Contains("sleepBeside") && EClass.rnd(5) == 0)
					{
						flag = true;
					}
					if (flag)
					{
						chara.MoveImmediate(EClass.pc.pos);
						chara.SetDir(chara.IsPCC ? EClass.pc.dir : 0);
						chara.Say("sleep_beside", chara, EClass.pc);
						if (!chara.HasCondition<ConSleep>())
						{
							chara.AddCondition<ConSleep>(20 + EClass.rnd(25), force: true);
						}
					}
				}
			}
			foreach (Chara member in EClass.pc.party.members)
			{
				if (!member.IsPC && !member.HasCondition<ConSleep>())
				{
					member.AddCondition<ConSleep>(5 + EClass.rnd(10), force: true);
				}
			}
			owner.RemoveCondition<ConBleed>();
			owner.RemoveCondition<ConPoison>();
			owner.RemoveCondition<ConMiasma>();
			EClass.ui.AddLayer<LayerSleep>().Sleep(num2, pcBed, pcPillow);
			return;
		}
		if (EClass.rnd(50) == 0 && !owner.IsPC)
		{
			if (EClass.rnd(10) == 0)
			{
				owner.Talk("sleepFav", (EClass.rnd(2) == 0) ? owner.GetFavCat().GetName().ToLower() : owner.GetFavFood().GetName());
				if (!owner.knowFav && owner.isSynced)
				{
					Msg.Say("noteFav", owner);
					owner.knowFav = true;
				}
			}
			else
			{
				owner.Talk("sleep");
			}
		}
		if (!(owner.ai is GoalSleep) || owner.CurrentSpan != TimeTable.Span.Sleep)
		{
			owner.HealHP(EClass.rnd(4) + 1);
			owner.mana.Mod(EClass.rnd(3) + 1);
			if (!owner.IsPC)
			{
				owner.stamina.Mod(EClass.rnd(2) + 1);
			}
			Mod(-1);
		}
	}

	public static void SuccubusVisit(Chara tg)
	{
		if (tg.pos.IsInSpot<TraitPillowStrange>())
		{
			return;
		}
		foreach (Chara chara in EClass._map.charas)
		{
			if (chara == tg || chara.IsPC || EClass.rnd(3) != 0 || chara.IsDisabled || !chara.IsIdle)
			{
				continue;
			}
			Thing thing = chara.things.Find<TraitDreamBug>();
			if ((!chara.HasElement(1216) && thing == null) || chara.host != null || (tg.IsPC && thing == null && EClass.rnd(200) != 0))
			{
				continue;
			}
			chara.Teleport(tg.pos);
			if (chara.Dist(tg.pos) > 2)
			{
				continue;
			}
			if (thing != null)
			{
				tg.Say("dreambug_teleport", chara, tg);
				thing.ModCharge(-1);
				if (thing.c_charges <= 0)
				{
					tg.Say("dreambug_destroy", chara, thing);
					thing.Destroy();
				}
			}
			chara.SetAI(new AI_Fuck
			{
				target = tg,
				succubus = true
			});
		}
	}

	public static void SuccubusSleep(Chara tg)
	{
		EClass._map.charas.ForeachReverse(delegate(Chara c)
		{
			if (c.ai is AI_Fuck { IsRunning: not false } aI_Fuck && aI_Fuck.target == tg)
			{
				aI_Fuck.Finish();
				aI_Fuck.Cancel();
				if (!c.HasCondition<ConSleep>())
				{
					c.AddCondition<ConSleep>(100 + EClass.rnd(100), force: true);
				}
			}
		});
	}

	public override void OnRemoved()
	{
		owner.conSleep = null;
		owner.renderer.RefreshSprite();
		owner.renderer.SetFirst(first: true, owner.pos.PositionAuto());
		if (!owner.IsPC)
		{
			owner.sleepiness.Set(0);
			return;
		}
		if (owner.IsPC)
		{
			if (uidRide != 0)
			{
				Chara chara = EClass._map.FindChara(uidRide);
				if (chara != null)
				{
					ActRide.Ride(EClass.pc, chara, parasite: false, talk: false);
				}
			}
			if (uidParasite != 0)
			{
				Chara chara2 = EClass._map.FindChara(uidParasite);
				if (chara2 != null)
				{
					ActRide.Ride(EClass.pc, chara2, parasite: true, talk: false);
				}
			}
		}
		if (owner.isDead)
		{
			return;
		}
		TraitPillow traitPillow = (pcPillow?.trait as TraitPillow) ?? EClass.pc.pos.FindThing<TraitPillow>();
		if (pickup)
		{
			TryPick(pcBed, posBed);
			TryPick(pcPillow, posPillow);
		}
		if (slept)
		{
			Thing thing = EClass.pc.things.Find<TraitGrimoire>();
			if (thing != null && thing.c_lockLv == 0 && !EClass._zone.IsRegion)
			{
				foreach (Thing item in thing.things.List((Thing _t) => _t.trait is TraitSpellbook || _t.trait is TraitAncientbook || _t.id == "234"))
				{
					TraitBaseSpellbook traitBaseSpellbook = item.trait as TraitBaseSpellbook;
					if (item.trait is TraitAncientbook && item.isOn)
					{
						continue;
					}
					int c_charges = item.c_charges;
					for (int i = 0; i < c_charges; i++)
					{
						if (EClass.pc.isDead)
						{
							return;
						}
						int num = traitBaseSpellbook.GetActDuration(EClass.pc) + 1;
						bool flag = false;
						for (int j = 0; j < num; j++)
						{
							if (!traitBaseSpellbook.TryProgress(new AIProgress
							{
								owner = EClass.pc
							}))
							{
								flag = true;
								if (!EClass.pc.isDead)
								{
									break;
								}
								return;
							}
						}
						if (!flag)
						{
							traitBaseSpellbook.OnRead(EClass.pc);
						}
					}
				}
			}
		}
		if (EClass.pc.isDead || !slept)
		{
			return;
		}
		EClass.player.recipes.OnSleep(traitPillow is TraitPillowEhekatl);
		EClass.player.DreamSpell();
		if (traitPillow is TraitPillowGod traitPillowGod)
		{
			traitPillowGod.Deity.Talk("morning");
		}
		foreach (Chara member in EClass.pc.party.members)
		{
			if (traitPillow is TraitPillowJure)
			{
				member.SAN.Mod(-15);
			}
		}
		static void TryPick(Thing t, ItemPosition pos)
		{
			if (t != null)
			{
				t = EClass._map.FindThing(t.uid);
				if (t != null && t.ExistsOnMap && !t.isNPCProperty)
				{
					if (pos != null)
					{
						Card card = ((pos.uidContainer == EClass.pc.uid) ? ((Card)EClass.pc) : ((Card)EClass.pc.things.Find(pos.uidContainer)));
						if (card != null)
						{
							EClass.pc.PlaySound("pick_thing");
							EClass.pc.Say("pick_thing", EClass.pc, t);
							t = card.AddThing(t, tryStack: false, pos.invX, pos.invY);
							t.invX = pos.invX;
							t.invY = pos.invY;
							LayerInventory.SetDirty(t);
							return;
						}
					}
					EClass.pc.Pick(t);
					LayerInventory.SetDirty(t);
				}
			}
		}
	}
}
