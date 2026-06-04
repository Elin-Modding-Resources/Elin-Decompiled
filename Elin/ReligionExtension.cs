public static class ReligionExtension
{
	public static void SpawnAltar(this Religion religion)
	{
		Thing thing = ThingGen.Create("altar");
		(thing.trait as TraitAltar)?.SetDeity(religion.id);
		Point nearestPoint = EClass.pc.pos.GetNearestPoint(allowBlock: true, allowChara: false, allowInstalled: false);
		if (nearestPoint != null)
		{
			EClass._zone.AddCard(thing, nearestPoint).Install();
		}
	}
}
