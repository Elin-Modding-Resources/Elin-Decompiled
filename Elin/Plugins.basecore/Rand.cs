using System;

public static class Rand
{
	public static int MaxBytes = 1111;

	public static Random _random = new Random();

	public static byte[] bytes;

	public static void InitBytes(int a)
	{
		SetSeed(a);
		bytes = new byte[MaxBytes];
		for (int i = 0; i < MaxBytes; i++)
		{
			bytes[i] = (byte)_random.Next(256);
		}
		SetSeed();
	}

	public static void UseSeed(int seed, Action action)
	{
		SetSeed(seed);
		action();
		SetSeed();
	}

	public static int rndSeed(int a, int seed)
	{
		SetSeed(seed);
		int result = rnd(a);
		SetSeed();
		return result;
	}

	public static void SetSeed(int a = -1)
	{
		_random = ((a == -1) ? new Random() : new Random(a));
	}

	public static int Range(int min, int max)
	{
		return _random.Next(max - min) + min;
	}

	public static float Range(float min, float max)
	{
		return (float)(_random.NextDouble() * (double)(max - min) + (double)min);
	}

	public static int rnd(int max)
	{
		if (max > 0)
		{
			return _random.Next(max);
		}
		return 0;
	}

	public static float rndf(float max)
	{
		return Range(0f, max);
	}

	public static int rndSqrt(int max)
	{
		return _random.Next(_random.Next(max) + 1);
	}

	public static int rndNormal2(int max)
	{
		int num = max / 2;
		return num + rnd(rnd(rnd(num) + 1) + 1) - rnd(rnd(rnd(num) + 1) + 1);
	}

	public static int rndNormal(int max)
	{
		int num = max / 2;
		return num + rnd(rnd(num) + 1) - rnd(rnd(num) + 1);
	}
}
