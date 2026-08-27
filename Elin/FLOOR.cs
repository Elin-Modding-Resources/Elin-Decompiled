public class FLOOR : EClass
{
	public const int TileWidth = 32;

	public const int floor_raw = 40;

	public const int field = 4;

	public const int floor_rock = 6;

	public const int floor_wood = 21;

	public const int floor_water = 43;

	public const int floor_water_shallow = 44;

	public const int floor_deck = 57;

	public const int floor_sand = 33;

	public const int floor_snow = 39;

	public const int floor_ice = 38;

	public const int floor_snow2 = 56;

	public const int sky = 90;

	public const int floor_shore = 33;

	public static SourceFloor.Row sourceSnow;

	public static SourceFloor.Row sourceSnow2;

	public static SourceFloor.Row sourceIce;

	public static SourceFloor.Row sourceWood;

	public static bool IsTatami(int id)
	{
		if (id != 93)
		{
			return id == 98;
		}
		return true;
	}
}
