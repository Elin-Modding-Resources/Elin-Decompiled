using System;
using UnityEngine;

public class ColorConverter
{
	private const float Epsilon = 0.008856f;

	private const float Kappa = 903.3f;

	private static readonly Xyz WhiteReference = new Xyz
	{
		X = 95.047f,
		Y = 100f,
		Z = 108.883f
	};

	public static Lab RgbToLab(Color rgb)
	{
		return XyzToLab(RgbToXyz(rgb));
	}

	public static Lab XyzToLab(Xyz xyz)
	{
		Xyz whiteReference = WhiteReference;
		float num = PivotXyz(xyz.X / whiteReference.X);
		float num2 = PivotXyz(xyz.Y / whiteReference.Y);
		float num3 = PivotXyz(xyz.Z / whiteReference.Z);
		return new Lab
		{
			L = Math.Max(0f, 116f * num2 - 16f),
			A = 500f * (num - num2),
			B = 200f * (num2 - num3)
		};
	}

	private static float PivotXyz(float n)
	{
		if (!(n > 0.008856f))
		{
			return (903.3f * n + 16f) / 116f;
		}
		return CubicRoot(n);
	}

	private static float CubicRoot(float n)
	{
		return Mathf.Pow(n, 1f / 3f);
	}

	public static Xyz RgbToXyz(Color rgb)
	{
		float num = PivotRgb(rgb.r);
		float num2 = PivotRgb(rgb.g);
		float num3 = PivotRgb(rgb.b);
		return new Xyz
		{
			X = num * 0.4124f + num2 * 0.3576f + num3 * 0.1805f,
			Y = num * 0.2126f + num2 * 0.7152f + num3 * 0.0722f,
			Z = num * 0.0193f + num2 * 0.1192f + num3 * 0.9505f
		};
	}

	private static float PivotRgb(float n)
	{
		return ((n > 0.04045f) ? Mathf.Pow((n + 0.055f) / 1.055f, 2.4f) : (n / 12.92f)) * 100f;
	}
}
