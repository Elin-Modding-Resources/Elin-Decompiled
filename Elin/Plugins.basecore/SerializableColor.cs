using System;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
[JsonObject(MemberSerialization.OptIn)]
public class SerializableColor
{
	[JsonProperty]
	public byte[] _color;

	public Color Get()
	{
		return (_color != null) ? new Color32(_color[0], _color[1], _color[2], _color[3]) : default(Color32);
	}

	public SerializableColor()
	{
	}

	public SerializableColor(Color color)
	{
		Set(color);
	}

	public SerializableColor(byte[] bytes)
	{
		_color = bytes;
	}

	public SerializableColor(byte r, byte g, byte b, byte a = byte.MaxValue)
	{
		_color = new byte[4] { r, g, b, a };
	}

	public SerializableColor Set(Color color)
	{
		Color32 color2 = color;
		_color = new byte[4] { color2.r, color2.g, color2.b, color2.a };
		return this;
	}

	public static void ToBytes(Color color, ref byte[] bytes, int index)
	{
		Color32 color2 = color;
		new byte[4] { color2.r, color2.g, color2.b, color2.a }.CopyTo(bytes, index);
	}

	public static Color FromBytes(byte[] _color, int index)
	{
		return new Color32(_color[index], _color[index + 1], _color[index + 2], _color[index + 3]);
	}
}
