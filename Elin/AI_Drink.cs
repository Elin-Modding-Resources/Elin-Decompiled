using System.Collections.Generic;

public class AI_Drink : AIAct
{
	public Card target;

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
		if (c != null)
		{
			return c.trait is TraitDrink;
		}
		return false;
	}

	public override void OnSetOwner()
	{
		if (target != null && (target.GetRootCard() == owner || target.parent == null))
		{
			owner.Drink(target);
			if (owner == EClass.pc)
			{
				EClass.player.EndTurn();
			}
			Success();
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
		yield return Success(delegate
		{
			owner.Drink(target);
		});
	}
}
