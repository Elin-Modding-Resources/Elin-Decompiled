using System.Collections.Generic;

public class AI_Read : AIAct
{
	public Card target;

	public override bool LocalAct
	{
		get
		{
			if (target != null)
			{
				if (!(target.trait is TraitStoryBook) && !(target.trait is TraitDeedRelocate))
				{
					return !(target.trait is TraitScrollMap);
				}
				return false;
			}
			return true;
		}
	}

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

	public override void OnSetOwner()
	{
		if (target != null && target.trait.GetActDuration(owner) == 0 && target != null && (target.GetRootCard() == owner || target.parent == null))
		{
			owner.Say("read", owner, target.Duplicate(1));
			Chara chara = owner;
			target.trait.OnRead(owner);
			target.Thing?.Identify(chara.IsPCParty);
			Success();
			if (chara.IsPC)
			{
				EClass.player.EndTurn();
			}
		}
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
			yield return DoGrab<TraitDrink>();
		}
		target = owner.held;
		if (target == null)
		{
			yield return Cancel();
		}
		if (target.trait.GetActDuration(owner) == 0)
		{
			owner.Say("read", owner, target.Duplicate(1));
			target.trait.OnRead(owner);
			yield return Success();
		}
		Progress_Custom seq = new Progress_Custom
		{
			maxProgress = target.trait.GetActDuration(owner),
			interval = 2,
			canProgress = () => owner.held == target,
			onProgressBegin = delegate
			{
				owner.Say("read_start", owner, target.GetName(NameStyle.Full, 1));
				owner.PlaySound("read_book");
			},
			onProgress = delegate(Progress_Custom p)
			{
				if (!target.trait.TryProgress(p) || target.GetRootCard() != owner)
				{
					p.Cancel();
				}
			},
			onProgressComplete = delegate
			{
				owner.PlaySound("read_book_end");
				owner.Say("read_end", owner, target.GetName(NameStyle.Full, 1));
				target.trait.OnRead(owner);
			}
		};
		yield return Do(seq);
	}
}
