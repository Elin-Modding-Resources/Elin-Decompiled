public class HotItemActionExitMap : HotAction
{
	public override string Id => "ExitMap";

	public override string Name => "exitNoMap".lang();

	public override void Perform()
	{
		EClass.pc.MoveZone(EClass._zone.ParentZone);
	}
}
