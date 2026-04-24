using UnityEngine;

public class StanceSongEnd : BaseSong
{
	public override int IdAbility => 6753;

	public override void TickSong()
	{
		foreach (Chara item in owner.pos.ListCharasInRadius(owner, 4, (Chara c) => !c.IsDeadOrSleeping && c.IsHostile(owner)))
		{
			if (owner == null || !owner.ExistsOnMap)
			{
				break;
			}
			if (30 * Mathf.Min(base.power / 4, 100) / 100 > EClass.rnd(100))
			{
				Act act = Element.Create((EClass.rnd(2) == 0) ? 50402 : ((EClass.rnd(2) == 0) ? 50401 : 50400), owner.CHA) as Act;
				ActEffect.ProcAt(EffectId.Hand, act.GetPower(owner), BlessedState.Normal, owner, item, item.pos, isNeg: true, new ActRef
				{
					act = act,
					aliasEle = act.source.aliasRef
				});
			}
		}
	}
}
