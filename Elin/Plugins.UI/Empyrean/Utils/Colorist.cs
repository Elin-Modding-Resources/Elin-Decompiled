using System.Globalization;
using UnityEngine;

namespace Empyrean.Utils;

public static class Colorist
{
	public static readonly Color steelBlue = new Color32(70, 130, 180, byte.MaxValue);

	public static readonly Color lightSteelBlue = new Color32(176, 196, 222, byte.MaxValue);

	public static readonly Color white32 = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	public static readonly Color black = new Color32(0, 0, 0, byte.MaxValue);

	public static readonly Color nearlyBlack = new Color(0.1f, 0.1f, 0.1f);

	public static readonly Color red10 = new Color(1f, 0.9f, 0.9f);

	public static readonly Color red25 = new Color(1f, 0.75f, 0.75f);

	public static readonly Color red50 = new Color(1f, 0.5f, 0.5f);

	public static readonly Color red75 = new Color(1f, 0.25f, 0.25f);

	public static readonly Color green10 = new Color(0.9f, 1f, 0.9f);

	public static readonly Color green25 = new Color(0.75f, 1f, 0.75f);

	public static readonly Color green50 = new Color(0.5f, 1f, 0.5f);

	public static readonly Color green75 = new Color(0.25f, 1f, 0.25f);

	public static readonly Color blue10 = new Color(0.9f, 0.9f, 1f);

	public static readonly Color blue25 = new Color(0.75f, 0.75f, 1f);

	public static readonly Color blue50 = new Color(0.5f, 0.5f, 1f);

	public static readonly Color blue75 = new Color(0.25f, 0.25f, 1f);

	public static readonly Color cyan25 = new Color(0.75f, 1f, 1f);

	public static readonly Color cyan50 = new Color(0.5f, 1f, 1f);

	public static readonly Color cyan75 = new Color(0.25f, 1f, 1f);

	public static readonly Color magenta25 = new Color(1f, 0.75f, 1f);

	public static readonly Color magenta50 = new Color(1f, 0.5f, 1f);

	public static readonly Color magenta75 = new Color(1f, 0.25f, 1f);

	public static readonly Color yellow25 = new Color(1f, 1f, 0.75f);

	public static readonly Color yellow50 = new Color(1f, 1f, 0.5f);

	public static readonly Color yellow75 = new Color(1f, 1f, 0.25f);

	public static readonly Color transparentBlack = new Color(0f, 0f, 0f, 0f);

	public static readonly Color transparentWhite = new Color(1f, 1f, 1f, 0f);

	public static readonly Color white = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	public static readonly Color red = new Color32(byte.MaxValue, 0, 0, byte.MaxValue);

	public static readonly Color brown = new Color32(165, 42, 42, byte.MaxValue);

	public static readonly Color gray = new Color32(190, 190, 190, byte.MaxValue);

	public static readonly Color pink = new Color32(byte.MaxValue, 192, 203, byte.MaxValue);

	public static readonly Color green = new Color32(0, byte.MaxValue, 0, byte.MaxValue);

	public static readonly Color tan = new Color32(210, 180, 140, byte.MaxValue);

	public static readonly Color yellow = new Color32(byte.MaxValue, byte.MaxValue, 0, byte.MaxValue);

	public static readonly Color maroon = new Color32(128, 0, 0, byte.MaxValue);

	public static readonly Color navyBlue = new Color32(0, 0, 128, byte.MaxValue);

	public static readonly Color magenta = new Color32(byte.MaxValue, 0, byte.MaxValue, byte.MaxValue);

	public static readonly Color blue = new Color32(0, 0, byte.MaxValue, byte.MaxValue);

	public static readonly Color aqua = new Color32(0, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	public static readonly Color orange = new Color32(byte.MaxValue, 165, 0, byte.MaxValue);

	public static Color Lighten(this Color c, float value = 0.2f)
	{
		return new Color(c.r + value, c.g + value, c.b + value);
	}

	public static Color Darken(this Color c, float value = 0.2f)
	{
		return new Color(c.r - value, c.g - value, c.b - value);
	}

	public static Color Inverse(this Color color)
	{
		return new Color(1f - color.r, 1f - color.g, 1f - color.b, color.a);
	}

	public static Color HSVtoRGB(HSVColor hsv)
	{
		return HSVtoRGB(hsv.h, hsv.s, hsv.v, hsv.a);
	}

	public static Color HSVtoRGB(float hue, float saturation, float value, float alpha = 1f)
	{
		int num = (int)(hue / 60f) % 6;
		float num2 = hue / 60f - (float)(int)(hue / 60f);
		float num3 = value * (1f - saturation);
		float num4 = value * (1f - num2 * saturation);
		float num5 = value * (1f - (1f - num2) * saturation);
		return num switch
		{
			0 => new Color(value, num5, num3, alpha), 
			1 => new Color(num4, value, num3, alpha), 
			2 => new Color(num3, value, num5, alpha), 
			3 => new Color(num3, num4, value, alpha), 
			4 => new Color(num5, num3, value, alpha), 
			_ => new Color(value, num3, num4, alpha), 
		};
	}

	public static HSVColor RGBtoHSV(Color color)
	{
		if (color.r == color.g && color.g == color.b)
		{
			color.r += 0.01f;
		}
		return RGBtoHSV(color.r, color.g, color.b, color.a);
	}

	public static HSVColor RGBtoHSV(float r, float g, float b, float a = 1f)
	{
		HSVColor result = default(HSVColor);
		float num = Mathf.Max(r, g, b);
		float num2 = Mathf.Min(r, g, b);
		float num3 = num - num2;
		result.a = a;
		if (num == 0f || num2 == 1f || num3 == 0f)
		{
			result.h = 0f;
			result.s = 0f;
			return result;
		}
		if (num == r)
		{
			result.h = (g - b) / num3;
		}
		else if (num == g)
		{
			result.h = (b - r) / num3 + 2f;
		}
		else
		{
			result.h = (r - g) / num3 + 4f;
		}
		result.h *= 60f;
		if (result.h < 0f)
		{
			result.h += 360f;
		}
		result.v = num;
		result.s = num3 / num;
		return result;
	}

	public static HSLColor RGBtoHSL(float r, float g, float b, float a = 1f)
	{
		HSLColor result = default(HSLColor);
		float num = Mathf.Max(r, g, b);
		float num2 = Mathf.Min(r, g, b);
		float num3 = num - num2;
		result.a = a;
		if (num == 0f || num2 == 1f || num3 == 0f)
		{
			result.h = 0f;
			result.s = 0f;
			return result;
		}
		if (num == r)
		{
			result.h = (g - b) / num3;
		}
		else if (num == g)
		{
			result.h = (b - r) / num3 + 2f;
		}
		else
		{
			result.h = (r - g) / num3 + 4f;
		}
		result.h *= 60f;
		if (result.h < 0f)
		{
			result.h += 360f;
		}
		result.l = (num + num2) / 2f;
		result.s = num3 / (1f - Mathf.Abs(2f * result.l - 1f));
		return result;
	}

	public static HSLColor RGBtoHSL(Color color)
	{
		return RGBtoHSL(color.r, color.g, color.b, color.a);
	}

	public static HSLColor HSVtoHSL(HSVColor hsv)
	{
		HSLColor result = default(HSLColor);
		result.h = hsv.h;
		result.l = hsv.v * (2f - hsv.s) / 2f;
		result.s = hsv.v * hsv.s / (1f - Mathf.Abs(result.l - 1f));
		return result;
	}

	public static HSVColor HSLtoHSV(HSLColor hsl)
	{
		HSVColor result = default(HSVColor);
		result.h = hsl.h;
		result.v = (2f * hsl.l + hsl.s * (1f - Mathf.Abs(2f * hsl.l - 1f))) / 2f;
		result.s = 2f * (result.v - hsl.l) / result.v;
		return result;
	}

	public static string ColorToHex(Color32 color)
	{
		return color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2") + color.a.ToString("X2");
	}

	public static Color HexToColor(string hex)
	{
		byte r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
		byte g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
		byte b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
		byte a = byte.MaxValue;
		if (hex.Length == 8)
		{
			a = byte.Parse(hex.Substring(6, 2), NumberStyles.HexNumber);
		}
		return new Color32(r, g, b, a);
	}

	public static Color SlightlyTransparent(Color color)
	{
		return color.SetAlpha(0.75f);
	}

	public static Color SetAlpha(this Color color, float alpha = 0.5f)
	{
		color.a = alpha;
		return color;
	}
}
