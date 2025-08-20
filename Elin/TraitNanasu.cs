public class TraitNanasu : TraitUniqueChara
{
	public override bool CanInvite => EClass.game.quests.IsCompleted("nasu");

	public override bool CanBeBanished => EClass.game.quests.IsCompleted("nasu");
}
