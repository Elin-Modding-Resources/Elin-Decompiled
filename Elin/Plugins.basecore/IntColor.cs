using UnityEngine;

public static class IntColor
{
	public static Color32 FromLong(long i)
	{
		return new Color32((byte)(i / 16777216), (byte)(i % 16777216 / 65536), (byte)(i % 65536 / 256), (byte)(i % 256));
	}

	public static long ToLong(ref Color c)
	{
		return (int)c.r * 255 * 16777216 + (int)c.g * 255 * 65536 + (int)c.b * 255 * 256 + (int)(c.a * 255f);
	}

	public static Color32 FromInt(int i)
	{
		byte b = (byte)(i / 4194304 * 2);
		byte b2 = (byte)(i % 4194304 / 32768 * 2);
		byte b3 = (byte)(i % 32768 / 256 * 2);
		byte a = (byte)(i % 256);
		return new Color32((byte)Mathf.Min(b + ((b % 2 != 0) ? 1 : 2), 255), (byte)Mathf.Min(b2 + ((b2 % 2 != 0) ? 1 : 2), 255), (byte)Mathf.Min(b3 + ((b3 % 2 != 0) ? 1 : 2), 255), a);
	}

	public static int ToInt(ref Color c)
	{
		return (int)(c.r * 127f) * 4194304 + (int)(c.g * 127f) * 32768 + (int)(c.b * 127f) * 256 + (int)(c.a * 255f);
	}

	public static int ToInt(Color c)
	{
		return (int)(c.r * 127f) * 4194304 + (int)(c.g * 127f) * 32768 + (int)(c.b * 127f) * 256 + (int)(c.a * 255f);
	}
}
