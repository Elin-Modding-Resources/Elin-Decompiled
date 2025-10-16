public class TraitTyche : TraitUniqueChara
{
	public override bool CanInvite => EClass.game.quests.IsCompleted("curry");
}
