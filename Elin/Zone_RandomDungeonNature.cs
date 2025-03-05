public class Zone_RandomDungeonNature : Zone_RandomDungeon
{
	public override float RespawnRate => base.RespawnRate * 3f;

	public override string GetDungenID()
	{
		if (EClass.rnd(2) == 0)
		{
			return "RoundRooms";
		}
		if (EClass.rnd(3) == 0)
		{
			return "CavernBig";
		}
		return "Cavern";
	}
}
