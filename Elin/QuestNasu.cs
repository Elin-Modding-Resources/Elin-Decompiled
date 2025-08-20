public class QuestNasu : QuestSequence
{
	public override void OnComplete()
	{
		Thing thing = EClass.pc.things.Find("backpack_holding");
		person.chara.AddCard(thing);
		thing.isGifted = true;
		DropReward(ThingGen.Create("697").SetNum(77));
		DropReward(TraitSeed.MakeSeed("feywood").SetNum(10));
		DropReward(TraitSeed.MakeSeed("coralwood").SetNum(10));
		if (person.chara.IsPCFaction)
		{
			person.chara.homeBranch.BanishMember(person.chara);
		}
	}
}
