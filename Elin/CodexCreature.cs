using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class CodexCreature : EClass
{
	public string id;

	[JsonProperty]
	public int[] _ints = new int[5];

	public string Name => source.GetName(null, full: true);

	public SourceChara.Row source => EClass.sources.charas.map[id];

	public int numCard
	{
		get
		{
			return _ints[0];
		}
		set
		{
			_ints[0] = value;
		}
	}

	public int weakspot
	{
		get
		{
			return _ints[2];
		}
		set
		{
			_ints[2] = value;
		}
	}

	public int kills
	{
		get
		{
			return _ints[3];
		}
		set
		{
			_ints[3] = value;
		}
	}

	public int spawns
	{
		get
		{
			return _ints[4];
		}
		set
		{
			_ints[4] = value;
		}
	}

	public int BonusDropLv
	{
		get
		{
			if (numCard > 1)
			{
				return (int)Mathf.Sqrt((float)numCard * 1.5f);
			}
			return 0;
		}
	}

	public bool droppedCard
	{
		get
		{
			return (_ints[1] & 2) != 0;
		}
		set
		{
			_ints[1] = (value ? (_ints[1] | 2) : (_ints[1] & -3));
		}
	}

	public void SetImage(Image image, int idSkin = 0, bool nativeSize = true)
	{
		source.SetImage(image, null, 0, nativeSize, 0, idSkin);
		image.SetActive(enable: true);
	}

	public string GetTextBonus()
	{
		string result = "noItem".lang();
		if (BonusDropLv > 0)
		{
			result = "codexBonus1".lang(BonusDropLv.ToString() ?? "");
		}
		return result;
	}
}
