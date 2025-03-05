public class TraitTrainingDummy : Trait
{
	public override TileMode tileMode => TileMode.SignalAnime;

	public override bool CanBeAttacked => true;

	public override bool IdleUse(Chara c, int dist)
	{
		if (dist <= 1 && ACT.Melee.Perform(c, owner))
		{
			return true;
		}
		if (c.TryEquipRanged() && dist < c.ranged.range && ACT.Ranged.Perform(c, owner))
		{
			return true;
		}
		return false;
	}

	public override void TrySetAct(ActPlan p)
	{
		if (owner.IsInstalled)
		{
			p.TrySetAct(new AI_PracticeDummy
			{
				target = owner.Thing
			}, owner);
		}
	}
}
