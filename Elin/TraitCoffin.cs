using System.Linq;

public class TraitCoffin : TraitContainer
{
	public override void Prespawn(int lv)
	{
		if (EClass.rnd(5) == 0)
		{
			ThingGen.CreateTreasureContent(owner.Thing, lv, TreasureType.RandomChest, clearContent: true);
			return;
		}
		owner.c_idRefCard = EClass.sources.charas.rows.Where((SourceChara.Row r) => r.race == "zombie" || r.race == "vampire").RandomItem().id;
	}
}
