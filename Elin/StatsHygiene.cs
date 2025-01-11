public class StatsHygiene : Stats
{
	public const int VeryClean = 5;

	public const int Clean = 4;

	public const int Normal = 3;

	public const int Dirty = 2;

	public const int Filthy = 1;

	public const int Garbage = 0;

	public static int[] listMod = new int[6] { 10, 50, 80, 100, 110, 115 };

	public override int max => 100;

	public static int GetAffinityMod(int phase)
	{
		return listMod.TryGet(phase);
	}
}
