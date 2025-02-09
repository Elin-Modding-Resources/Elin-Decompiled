using System.Runtime.Serialization;
using Newtonsoft.Json;

public class SocketData : EClass
{
	public enum Type
	{
		Socket,
		Rune
	}

	[JsonProperty]
	public int[] ints = new int[5];

	public BitArray32 bits;

	public Type type
	{
		get
		{
			return ints[0].ToEnum<Type>();
		}
		set
		{
			ints[0] = (int)value;
		}
	}

	public int idEle
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

	public int value
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

	[OnSerializing]
	private void _OnSerializing(StreamingContext context)
	{
		ints[4] = (int)bits.Bits;
	}

	[OnDeserialized]
	private void _OnDeserialized(StreamingContext context)
	{
		bits.Bits = (uint)ints[4];
	}
}
