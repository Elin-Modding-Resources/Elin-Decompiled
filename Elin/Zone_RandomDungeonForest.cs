public class Zone_RandomDungeonForest : Zone_RandomDungeonNature
{
	public override string IdBiome
	{
		get
		{
			if (EClass.rnd(2) != 0)
			{
				return "Dungeon_Forest";
			}
			return "Forest";
		}
	}
}
