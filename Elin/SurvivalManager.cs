using System.Runtime.Serialization;
using Newtonsoft.Json;

public class SurvivalManager : EClass
{
	public class Flags : EClass
	{
		[JsonProperty]
		public int[] ints = new int[50];

		public BitArray32 bits;

		public int spawnedFloor
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

		public int floors
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

		public int searchWreck
		{
			get
			{
				return ints[5];
			}
			set
			{
				ints[5] = value;
			}
		}

		[OnSerializing]
		private void _OnSerializing(StreamingContext context)
		{
			ints[0] = (int)bits.Bits;
		}

		[OnDeserialized]
		private void _OnDeserialized(StreamingContext context)
		{
			bits.Bits = (uint)ints[0];
		}
	}

	[JsonProperty]
	public Flags flags = new Flags();
}
