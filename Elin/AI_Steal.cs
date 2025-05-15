using System.Collections.Generic;
using UnityEngine;

public class AI_Steal : AI_TargetCard
{
	public override TargetType TargetType => TargetType.SelfAndNeighbor;

	public override int MaxRadius => 2;

	public override bool IsHostileAct => true;

	public override bool IsValidTC(Card c)
	{
		if (EClass._zone.IsUserZone)
		{
			return false;
		}
		if (c.isThing & (EClass._zone is Zone_LittleGarden))
		{
			return false;
		}
		if (!c.isNPCProperty && c.isThing)
		{
			return false;
		}
		if (!c.trait.CanBeStolen)
		{
			return false;
		}
		if (c.c_lockLv > 0)
		{
			return false;
		}
		if (!c.isThing)
		{
			return !c.IsPCFaction;
		}
		return true;
	}

	public override bool CanPerform()
	{
		if (Act.TC != null)
		{
			return Act.TC.trait.CanBeStolen;
		}
		return false;
	}

	public override bool Perform()
	{
		target = Act.TC;
		return base.Perform();
	}

	public override IEnumerable<Status> Run()
	{
		Chara chara = target.Chara;
		if (chara != null)
		{
			target = chara.things.FindStealable();
			if (target == null && chara.GetInt(30) < EClass.world.date.GetRaw())
			{
				if (EClass.rnd(2) == 0)
				{
					target = chara.AddThing(ThingGen.Create("money").SetNum(1 + EClass.rnd(chara.LV * 10)));
				}
				else
				{
					target = chara.AddThing(ThingGen.CreateFromFilter("steal", chara.LV));
					chara.SetInt(30, EClass.world.date.GetRaw() + 1440);
				}
			}
			if (target == null)
			{
				owner.Say("steal_chara_nothing", owner, chara);
				yield return Cancel();
			}
			owner.Say("steal_chara", owner, chara);
		}
		else
		{
			if (target.things.Count > 0)
			{
				Thing thing = target.things.FindStealable();
				if (thing != null)
				{
					target = thing;
				}
			}
			owner.Say("steal_thing", owner, target);
		}
		ICardParent targetParent = target.parent;
		Card card = chara;
		if (card == null)
		{
			card = target;
		}
		Card root = card.GetRootCard();
		Progress_Custom seq = new Progress_Custom
		{
			canProgress = () => target.parent == targetParent && (chara == null || chara.ExistsOnMap),
			onProgressBegin = delegate
			{
			},
			onProgress = delegate(Progress_Custom p)
			{
				owner.LookAt(root);
				owner.PlaySound("steal");
				root.renderer.PlayAnime(AnimeID.Shiver);
				if (!EClass.debug.godMode)
				{
					if (chara != null && owner.Dist(chara) > 1)
					{
						EClass.pc.TryMoveTowards(chara.pos);
						if (owner == null)
						{
							p.Cancel();
							return;
						}
						if (chara != null && owner.Dist(chara) > 1)
						{
							EClass.pc.Say("targetTooFar");
							p.Cancel();
							return;
						}
					}
					if (target.ChildrenAndSelfWeight > owner.Evalue(281) * 200 + owner.STR * 100 + 1000)
					{
						EClass.pc.Say("tooHeavy", target);
						p.Cancel();
					}
					else
					{
						int count = owner.pos.ListWitnesses(owner, 4, WitnessType.crime, chara).Count;
						if (owner.pos.TryWitnessCrime(owner, chara, 4, delegate(Chara c)
						{
							int num = ((!c.CanSee(owner)) ? 30 : 0);
							int num2 = c.PER * 250 / 100;
							if (target.isThing && (target.Thing.isEquipped || target.IsRangedWeapon || target.IsThrownWeapon))
							{
								num2 *= 2;
								if (target.rarity >= Rarity.Legendary)
								{
									num2 *= 2;
								}
								if (target.rarity >= Rarity.Artifact)
								{
									num2 *= 2;
								}
							}
							if (c.IsUnique)
							{
								num2 *= 2;
							}
							return EClass.rnd(num2) > owner.Evalue(281) + owner.DEX + num;
						}))
						{
							p.Cancel();
						}
						else
						{
							owner.elements.ModExp(281, Mathf.Min(count * 5 + 5, 25));
						}
					}
				}
			},
			onProgressComplete = delegate
			{
				if (target.isThing && target.IsInstalled)
				{
					target.SetPlaceState(PlaceState.roaming);
				}
				owner.Say("steal_end", owner, target);
				if (chara != null && (chara.IsPCFaction || chara.OriginalHostility >= Hostility.Friend))
				{
					EClass.player.ModKarma(-1);
				}
				else if (chara == null || chara.hostility > Hostility.Enemy)
				{
					EClass.player.ModKarma(-1);
				}
				target.isNPCProperty = false;
				if (!target.category.IsChildOf("currency"))
				{
					target.isStolen = true;
				}
				owner.Pick(target.Thing);
				owner.elements.ModExp(281, 50);
				if (EClass.rnd(2) == 0)
				{
					EClass.pc.stamina.Mod(-1);
				}
			}
		}.SetDuration(20, 4);
		yield return Do(seq);
	}
}
