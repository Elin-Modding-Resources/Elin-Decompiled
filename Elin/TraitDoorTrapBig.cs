public class TraitDoorTrapBig : TraitDoorBig
{
	public override TileMode tileMode => base.tileMode;

	public override bool UseAltTiles => IsOpen();

	public override TraitDoorBig GetDoorPair()
	{
		Cell cell = owner.Cell;
		if (cell.Front.traitDoor is TraitDoorTrapBig)
		{
			return cell.Front.traitDoor as TraitDoorTrapBig;
		}
		if (cell.Back.traitDoor is TraitDoorTrapBig)
		{
			return cell.Back.traitDoor as TraitDoorTrapBig;
		}
		if (cell.Left.traitDoor is TraitDoorTrapBig)
		{
			return cell.Left.traitDoor as TraitDoorTrapBig;
		}
		if (cell.Right.traitDoor is TraitDoorTrapBig)
		{
			return cell.Right.traitDoor as TraitDoorTrapBig;
		}
		return null;
	}
}
