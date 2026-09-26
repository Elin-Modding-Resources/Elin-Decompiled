public class TraitDoorBig : TraitDoor
{
	public override int height => 2;

	public override string idSound => "door3";

	public override void ToggleDoor(bool sound = true, bool refresh = true)
	{
		base.ToggleDoor(sound, refresh);
		TraitDoorBig doorPair = GetDoorPair();
		if (doorPair != null && doorPair.IsOpen() != IsOpen())
		{
			doorPair.ToggleDoor(sound: false);
		}
	}

	public TraitDoorBig GetDoorPair()
	{
		int dir = owner.dir;
		Cell cell = null;
		cell = ((!(this is TraitDoorBigL)) ? ((dir == 0 || dir == 2) ? owner.Cell.Back : owner.Cell.Left) : ((dir == 0 || dir == 2) ? owner.Cell.Front : owner.Cell.Right));
		return cell.traitDoor as TraitDoorBig;
	}
}
