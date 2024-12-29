using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[JsonObject(MemberSerialization.OptIn)]
public class SkinConfig
{
	[JsonProperty]
	public List<SkinDeco> decos = new List<SkinDeco>();

	[JsonProperty]
	public int[] ints = new int[10];

	public int id;

	public SkinSet _Skin;

	public int bg
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

	public int bgSize
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

	public int grid
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

	public int button
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

	public Color bgColor
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

	public Color gridColor
	{
		get
		{
			return IntColor.FromInt(ints[6]);
		}
		set
		{
			ints[6] = IntColor.ToInt(ref value);
		}
	}

	public SkinSet Skin => _Skin ?? (_Skin = SkinManager.Instance.skinSets[0]);

	public SkinAsset_BG BG
	{
		get
		{
			SkinAsset_BG skinAsset_BG = Skin.bgs[bg];
			return skinAsset_BG.redirect switch
			{
				SkinAssetRedirect.BG1 => SkinManager.CurrentSkin.assets.BG1, 
				SkinAssetRedirect.BG2 => SkinManager.CurrentSkin.assets.BG2, 
				_ => skinAsset_BG, 
			};
		}
	}

	public SpriteAsset Grid => Skin.bgGrid[grid];

	public SkinAsset_Button Button
	{
		get
		{
			SkinAsset_Button skinAsset_Button = Skin.buttons[button];
			if (skinAsset_Button.redirect == SkinAssetRedirect.Button1)
			{
				return SkinManager.CurrentSkin.assets.Button1;
			}
			return skinAsset_Button;
		}
	}

	public SkinConfig()
	{
	}

	public SkinConfig(SkinSet __skin)
	{
		_Skin = __skin;
		bg = (grid = (button = 0));
		bgColor = BG.color;
		gridColor = Grid.color;
	}

	public void SetID(int _id)
	{
		id = _id;
		int num2 = (button = 0);
		int num4 = (grid = num2);
		bg = num4;
		_Skin = null;
		bgColor = BG.color;
		gridColor = Grid.color;
	}

	public void SetID()
	{
		bgColor = BG.color;
		gridColor = Grid.color;
	}
}
