public static class MathEx
{
	public static int IntIDOverflowTempFix(int a)
	{
		if (a >= 0)
		{
			return a;
		}
		return -a;
	}

	public static bool IsSameSign(int a, int b)
	{
		if (a < 0 || b < 0)
		{
			if (a < 0)
			{
				return b < 0;
			}
			return false;
		}
		return true;
	}

	public static int ClampToInt(long a)
	{
		if (a > int.MaxValue)
		{
			return int.MaxValue;
		}
		if (a < int.MinValue)
		{
			return int.MinValue;
		}
		return (int)a;
	}

	public static int ClampToInt(long a, int b = int.MaxValue)
	{
		if (a > b)
		{
			return b;
		}
		if (a < -b)
		{
			return -b;
		}
		return (int)a;
	}

	public static int ClampToInt(long a, int min, int max)
	{
		if (a > max)
		{
			return max;
		}
		if (a < min)
		{
			return min;
		}
		return (int)a;
	}

	public static long Max(long a, long b)
	{
		if (a >= b)
		{
			return a;
		}
		return b;
	}
}
