using System.Collections.Generic;
using System.Linq;

public class AI_Offer : AIAct
{
	public Card target;

	public Card altar;

	public bool IsValidTarget(Card c)
	{
		if (altar != null)
		{
			return altar.trait.CanOffer(owner, c);
		}
		return false;
	}

	public override IEnumerable<Status> Run()
	{
		if (target != null && target.ExistsOnMap)
		{
			yield return DoGoto(target);
		}
		altar = EClass._map.Installed.traits.altars.GetRandomInstalled();
		if (altar == null)
		{
			yield return Cancel();
		}
		if (!IsValidTarget(owner.held))
		{
			yield return DoGrab(EClass._map.charas.Where(IsValidTarget).RandomItem());
			if (!IsValidTarget(owner.held))
			{
				yield return Cancel();
			}
		}
		yield return DoGoto(altar);
		if (!IsValidTarget(owner.held))
		{
			yield return Cancel();
		}
		owner.DropHeld(altar.pos);
		Progress_Custom seq = new Progress_Custom
		{
			canProgress = () => altar.ExistsOnMap,
			onProgress = delegate
			{
				altar.trait.OfferProcess(owner);
			},
			onProgressComplete = delegate
			{
				altar.trait.Offer(owner);
			}
		}.SetDuration(15, 5);
		yield return Do(seq);
	}
}
