public class TraitSpotBiome : TraitSpot
{
	public override int radius => 6;

	public override bool HaveUpdate => true;

	public override void Update()
	{
		if (EClass.rnd(5) != 0)
		{
			return;
		}
		Point randomPoint = GetRandomPoint();
		if (!randomPoint.IsSky && !randomPoint.HasObj && !randomPoint.HasBlock && !randomPoint.HasThing && randomPoint.cell.room == null)
		{
			if (EClass.rnd(5) == 0)
			{
				EClass._zone.SpawnMob(randomPoint);
			}
			else
			{
				randomPoint.cell.biome.Populate(randomPoint, interior: false, 100f);
			}
		}
	}
}
