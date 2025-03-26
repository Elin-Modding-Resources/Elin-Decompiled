public class TraitChestOyster : TraitChest
{
	public override int ChanceLock => 100;

	public override void Prespawn(int lv)
	{
		if (EClass.rnd(3) != 0)
		{
			if (EClass.rnd(2) == 0)
			{
				owner.c_lockLv = 0;
			}
		}
		else
		{
			ThingGen.CreateTreasureContent(owner.Thing, lv, TreasureType.RandomChest, clearContent: true);
		}
	}
}
