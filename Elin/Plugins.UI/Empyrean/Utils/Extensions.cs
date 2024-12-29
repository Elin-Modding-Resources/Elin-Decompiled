using UnityEngine;

namespace Empyrean.Utils;

public static class Extensions
{
	public static Color ToRGB(this HSVColor hsv)
	{
		return Colorist.HSVtoRGB(hsv);
	}

	public static HSVColor ToHsv(this Color rgb)
	{
		return Colorist.RGBtoHSV(rgb);
	}

	public static float h(this Color color)
	{
		return color.ToHsv().h;
	}

	public static float s(this Color color)
	{
		return color.ToHsv().s;
	}

	public static float v(this Color color)
	{
		return color.ToHsv().v;
	}
}
