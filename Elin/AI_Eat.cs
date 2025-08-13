using System.Collections.Generic;
using UnityEngine;

public class AI_Eat : AIAct
{
	public Card target;

	public bool cook = true;

	public override bool LocalAct => false;

	public override bool IsHostileAct
	{
		get
		{
			if (target != null)
			{
				return target.isNPCProperty;
			}
			return false;
		}
	}

	public bool IsValidTarget(Card c)
	{
		return c?.trait.CanEat(owner) ?? false;
	}

	public override bool CanManualCancel()
	{
		return true;
	}

	public override void OnStart()
	{
	}

	public override IEnumerable<Status> Run()
	{
		if (target != null && (target.GetRootCard() == owner || target.parent == null))
		{
			owner.HoldCard(target, 1);
		}
		else if (target != null)
		{
			yield return DoGrab(target, 1);
		}
		else
		{
			if (!IsValidTarget(owner.held))
			{
				yield return DoGrab<TraitFood>();
				if (!IsValidTarget(owner.held))
				{
					yield return Cancel();
				}
			}
			if (cook)
			{
				yield return Do(new AI_Cook(), base.KeepRunning);
				if (!IsValidTarget(owner.held))
				{
					yield return Cancel();
				}
				yield return DoGotoSpot<TraitHearth>(base.KeepRunning);
			}
		}
		target = owner.held;
		if (target == null)
		{
			yield return Cancel();
		}
		if (EClass._zone.IsPCFaction && !owner.IsPCParty && owner.memberType != FactionMemberType.Livestock && !owner.noMove)
		{
			yield return DoGotoSpot<TraitSpotDining>(base.KeepRunning);
		}
		int max = ((target.SelfWeight < 100) ? 1 : (2 + (int)Mathf.Sqrt(target.SelfWeight * 2 / 3)));
		int turn = 0;
		Progress_Custom seq = new Progress_Custom
		{
			cancelWhenMoved = false,
			canProgress = () => IsValidTarget(target) && owner.held == target,
			onProgressBegin = delegate
			{
				owner.Say("eat_start", owner, target.GetName(NameStyle.Full, 1));
				owner.PlaySound("eat");
			},
			onProgress = delegate(Progress_Custom p)
			{
				target.PlayAnime(AnimeID.Eat);
				if (turn == 1 && owner.IsPC && owner.hunger.GetPhase() == 0 && !EClass.debug.godFood)
				{
					owner.Say("eat_full");
					p.Cancel();
				}
				if (turn == 1)
				{
					foreach (Element value in target.elements.dict.Values)
					{
						if (!value.source.foodEffect.IsEmpty())
						{
							string[] foodEffect = value.source.foodEffect;
							if (foodEffect[0] == "poison" || foodEffect[0] == "love")
							{
								owner.Talk("eatWeird");
								break;
							}
						}
					}
					CardRow refCard = target.refCard;
					if (refCard != null && refCard.id == "mammoth")
					{
						EClass.player.forceTalk = true;
						owner.Talk("eatammoth");
					}
				}
				turn++;
			},
			onProgressComplete = delegate
			{
				if (owner.IsPC && owner.hunger.GetPhase() == 0 && !EClass.debug.godFood)
				{
					owner.Say("eat_full");
				}
				else
				{
					owner.Say("eat_end", owner, target.GetName(NameStyle.Full, 1));
					owner.ShowEmo(Emo.happy);
					FoodEffect.Proc(owner, target.Thing);
				}
			}
		}.SetDuration(max, 5);
		yield return Do(seq);
	}
}
