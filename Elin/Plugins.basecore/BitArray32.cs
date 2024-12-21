using System;
using Unity.Burst;

public struct BitArray32 : IEquatable<BitArray32>
{
	public uint Bits;

	public bool this[int index]
	{
		get
		{
			uint num = (uint)(1 << index);
			return (Bits & num) == num;
		}
		set
		{
			uint num = (uint)(1 << index);
			if (value)
			{
				Bits |= num;
			}
			else
			{
				Bits &= ~num;
			}
		}
	}

	public int Length => 32;

	public BitArray32(uint bits)
	{
		Bits = bits;
	}

	public void SetBit(int index)
	{
		RequireIndexInBounds(index);
		uint num = (uint)(1 << index);
		Bits |= num;
	}

	public void UnsetBit(int index)
	{
		RequireIndexInBounds(index);
		uint num = (uint)(1 << index);
		Bits &= ~num;
	}

	public uint GetBits(uint mask)
	{
		return Bits & mask;
	}

	public void SetBits(uint mask)
	{
		Bits |= mask;
	}

	public void UnsetBits(uint mask)
	{
		Bits &= ~mask;
	}

	public override bool Equals(object obj)
	{
		if (obj is BitArray32)
		{
			return Bits == ((BitArray32)obj).Bits;
		}
		return false;
	}

	public bool Equals(BitArray32 arr)
	{
		return Bits == arr.Bits;
	}

	public override int GetHashCode()
	{
		return Bits.GetHashCode();
	}

	public int ToInt()
	{
		return (int)Bits;
	}

	public void SetInt(int i)
	{
		Bits = (uint)i;
	}

	public override string ToString()
	{
		char[] array = new char[44];
		int i;
		for (i = 0; i < 11; i++)
		{
			array[i] = "BitArray32{"[i];
		}
		uint num = 2147483648u;
		while (num != 0)
		{
			array[i] = (((Bits & num) != 0) ? '1' : '0');
			num >>= 1;
			i++;
		}
		array[i] = '}';
		return new string(array);
	}

	[BurstDiscard]
	public void RequireIndexInBounds(int index)
	{
	}
}
