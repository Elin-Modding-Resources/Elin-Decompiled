using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;

[JsonObject(MemberSerialization.OptIn)]
public class SkinDeco
{
	public SkinDecoActor actor;

	[JsonProperty]
	public int[] ints = new int[9];

	public BitArray32 bits;

	public int id
	{
		get
		{
			return ints[0];
		}
		set
		{
			ints[0] = value;
		}
	}

	public int x
	{
		get
		{
			return ints[1];
		}
		set
		{
			ints[1] = value;
		}
	}

	public int y
	{
		get
		{
			return ints[2];
		}
		set
		{
			ints[2] = value;
		}
	}

	public int sx
	{
		get
		{
			return ints[3];
		}
		set
		{
			ints[3] = value;
		}
	}

	public int sy
	{
		get
		{
			return ints[4];
		}
		set
		{
			ints[4] = value;
		}
	}

	public Color color
	{
		get
		{
			return IntColor.FromInt(ints[5]);
		}
		set
		{
			ints[5] = IntColor.ToInt(ref value);
		}
	}

	public int rz
	{
		get
		{
			return ints[6];
		}
		set
		{
			ints[6] = value;
		}
	}

	public int cat
	{
		get
		{
			return ints[7];
		}
		set
		{
			ints[7] = value;
		}
	}

	public bool shadow
	{
		get
		{
			return bits[0];
		}
		set
		{
			bits[0] = value;
		}
	}

	public bool reverse
	{
		get
		{
			return bits[1];
		}
		set
		{
			bits[1] = value;
		}
	}

	public bool bottom
	{
		get
		{
			return bits[2];
		}
		set
		{
			bits[2] = value;
		}
	}

	[OnSerializing]
	internal void OnSerializing(StreamingContext context)
	{
		ints[8] = (int)bits.Bits;
	}

	[OnDeserialized]
	internal void _OnDeserialized(StreamingContext context)
	{
		bits.Bits = (uint)ints[8];
	}
}
