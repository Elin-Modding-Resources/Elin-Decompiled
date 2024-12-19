using System.Collections.Generic;

public class GoalGraze : Goal
{
	public override IEnumerable<Status> Run()
	{
		Point pos = GetPos();
		if (pos != null)
		{
			yield return DoGoto(pos);
		}
		yield return DoIdle(20);
	}

	public Point GetPos()
	{
		Thing thing = null;
		Rand.SetSeed(owner.uid);
		if (thing == null)
		{
			thing = EClass._map.FindThing(typeof(TraitSpotRanch), owner);
		}
		Rand.SetSeed();
		return thing?.trait.GetRandomPoint();
	}

	public override void OnSimulatePosition()
	{
		Point pos = GetPos();
		if (pos != null)
		{
			owner.MoveImmediate(pos);
		}
	}
}
