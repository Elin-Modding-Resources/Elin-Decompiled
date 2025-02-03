public class TraitStairsDown : TraitStairs
{
	public override ZoneTransition.EnterState enterState => ZoneTransition.EnterState.Down;

	public override string langOnUse => "stairsDown";

	public override bool IsDownstairs => true;

	public override int InstallBottomPriority => 20000;
}
