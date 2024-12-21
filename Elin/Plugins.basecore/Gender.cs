public class Gender
{
	public const int unknown = 0;

	public const int female = 1;

	public const int male = 2;

	public static string Name(int g)
	{
		return (g switch
		{
			1 => "female", 
			2 => "male", 
			_ => "gay", 
		}).lang();
	}

	public static int GetRandom()
	{
		if (Rand.rnd(30) == 0)
		{
			return 0;
		}
		return Rand.Range(1, 3);
	}
}
