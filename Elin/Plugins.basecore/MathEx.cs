public static class MathEx
{
	public static int Min(long a, int b = int.MaxValue)
	{
		if (a > b)
		{
			return b;
		}
		return (int)a;
	}
}
