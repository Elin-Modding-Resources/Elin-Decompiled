public class Zone_DungeonDragon : Zone_DungeonUnfixed
{
	public override bool LockExit => base.lv <= -6;
}
