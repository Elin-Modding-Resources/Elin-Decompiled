using Newtonsoft.Json;

public class CellEffect : EClass
{
	[JsonProperty]
	public int[] ints = new int[8];

	[JsonProperty]
	public string[] strs = new string[1];

	public SourceCellEffect.Row _source;

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

	public int amount
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

	public EffectId idEffect
	{
		get
		{
			return ints[2].ToEnum<EffectId>();
		}
		set
		{
			ints[2] = (int)value;
		}
	}

	public int power
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

	public int color
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

	public bool isHostileAct
	{
		get
		{
			return (ints[5] & 2) != 0;
		}
		set
		{
			ints[5] = (value ? (ints[5] | 2) : (ints[5] & -3));
		}
	}

	public bool isBlessed
	{
		get
		{
			return (ints[5] & 4) != 0;
		}
		set
		{
			ints[5] = (value ? (ints[5] | 4) : (ints[5] & -5));
		}
	}

	public bool isCursed
	{
		get
		{
			return (ints[5] & 8) != 0;
		}
		set
		{
			ints[5] = (value ? (ints[5] | 8) : (ints[5] & -9));
		}
	}

	public int idEle
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

	public int refVal
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

	public string n1
	{
		get
		{
			return strs[0];
		}
		set
		{
			strs[0] = value;
		}
	}

	public SourceCellEffect.Row source => _source ?? (_source = EClass.sources.cellEffects.rows[id]);

	public bool IsFire => id == 3;

	public bool IsLiquid
	{
		get
		{
			if (id != 1 && id != 2)
			{
				return id == 4;
			}
			return true;
		}
	}

	public int FireAmount
	{
		get
		{
			if (id != 3)
			{
				return 0;
			}
			return amount;
		}
	}

	public int LiquidAmount
	{
		get
		{
			if (!IsLiquid)
			{
				return 0;
			}
			return amount;
		}
	}

	public bool WillFade => id >= 5;
}
