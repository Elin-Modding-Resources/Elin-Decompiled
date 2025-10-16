public class TraitVishnu : TraitUniqueChara
{
	public override bool CanInvite => EClass.game.quests.IsCompleted("curry");
}
