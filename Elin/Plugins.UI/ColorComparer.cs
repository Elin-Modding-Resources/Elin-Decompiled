using UnityEngine;

public static class ColorComparer
{
	public static float Compare(Color colorA, Color colorB)
	{
		Lab lab = ColorConverter.RgbToLab(colorA);
		Lab lab2 = ColorConverter.RgbToLab(colorB);
		return Mathf.Sqrt(Distance(lab.L, lab2.L) + Distance(lab.A, lab2.A) + Distance(lab.B, lab2.B));
	}

	private static float Distance(float a, float b)
	{
		return (a - b) * (a - b);
	}
}
