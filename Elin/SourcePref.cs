using System;
using System.IO;
using System.Text;
using IniParser;
using IniParser.Model;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class SourcePref : EClass, ISerializationCallbackReceiver
{
	[JsonProperty]
	public int[] ints = new int[25];

	public BitArray32 _bits1;

	public int test
	{
		get
		{
			Validate();
			return ints[0];
		}
	}

	public PrefFlag flags
	{
		get
		{
			return ints[1].ToEnum<PrefFlag>();
		}
		set
		{
			ints[1] = (int)value;
		}
	}

	public int shadow
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

	public float height
	{
		get
		{
			return (float)ints[3] * 0.01f;
		}
		set
		{
			ints[3] = (int)(value * 100f);
		}
	}

	public int liquidMod
	{
		get
		{
			return ints[16];
		}
		set
		{
			ints[16] = value;
		}
	}

	public int liquidModMax
	{
		get
		{
			return ints[21];
		}
		set
		{
			ints[21] = value;
		}
	}

	public float hatY
	{
		get
		{
			return (float)ints[23] * 0.01f;
		}
		set
		{
			ints[23] = (int)(value * 100f);
		}
	}

	public float stackX
	{
		get
		{
			return (float)ints[17] * 0.01f;
		}
		set
		{
			ints[17] = (int)(value * 100f);
		}
	}

	public float x
	{
		get
		{
			return (float)ints[4] * 0.01f;
		}
		set
		{
			ints[4] = (int)(value * 100f);
		}
	}

	public float y
	{
		get
		{
			return (float)ints[5] * 0.01f;
		}
		set
		{
			ints[5] = (int)(value * 100f);
		}
	}

	public float z
	{
		get
		{
			return (float)ints[2] * 0.01f;
		}
		set
		{
			ints[2] = (int)(value * 100f);
		}
	}

	public int pivotX
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

	public int pivotY
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

	public int shadowX
	{
		get
		{
			return ints[8];
		}
		set
		{
			ints[8] = value;
		}
	}

	public int shadowY
	{
		get
		{
			return ints[9];
		}
		set
		{
			ints[9] = value;
		}
	}

	public int shadowRX
	{
		get
		{
			return ints[10];
		}
		set
		{
			ints[10] = value;
		}
	}

	public int shadowRY
	{
		get
		{
			return ints[11];
		}
		set
		{
			ints[11] = value;
		}
	}

	public int shadowBX
	{
		get
		{
			return ints[12];
		}
		set
		{
			ints[12] = value;
		}
	}

	public int shadowBY
	{
		get
		{
			return ints[13];
		}
		set
		{
			ints[13] = value;
		}
	}

	public int shadowBRX
	{
		get
		{
			return ints[14];
		}
		set
		{
			ints[14] = value;
		}
	}

	public int shadowBRY
	{
		get
		{
			return ints[15];
		}
		set
		{
			ints[15] = value;
		}
	}

	public int equipX
	{
		get
		{
			return ints[18];
		}
		set
		{
			ints[18] = value;
		}
	}

	public int equipY
	{
		get
		{
			return ints[19];
		}
		set
		{
			ints[19] = value;
		}
	}

	public int scaleIcon
	{
		get
		{
			return ints[20];
		}
		set
		{
			ints[20] = value;
		}
	}

	public bool bypassShadow
	{
		get
		{
			return _bits1[0];
		}
		set
		{
			_bits1[0] = value;
		}
	}

	public bool UsePref => (ints[1] & 1) != 0;

	public bool Float => (ints[1] & 4) != 0;

	public bool Surface => (ints[1] & 8) != 0;

	public bool FloatUnderwater => (ints[1] & 0x10) != 0;

	public void OnAfterDeserialize()
	{
		if (ints.Length >= 25)
		{
			_bits1.SetInt(ints[22]);
		}
		else
		{
			Validate();
		}
	}

	public void OnBeforeSerialize()
	{
		ints[22] = _bits1.ToInt();
	}

	public void Validate()
	{
		if (ints.Length < 25)
		{
			Array.Resize(ref ints, 25);
		}
	}

	public void WriteIni(string path)
	{
		FileIniDataParser fileIniDataParser = new FileIniDataParser();
		if (File.Exists(path))
		{
			EClass.ui.Say("Ini file already exists.");
			return;
		}
		File.CreateText(path).Close();
		IniData iniData = fileIniDataParser.ReadFile(path, Encoding.UTF8);
		iniData.Global["x"] = ints[4].ToString() ?? "";
		iniData.Global["y"] = ints[5].ToString() ?? "";
		iniData.Global["z"] = ints[2].ToString() ?? "";
		iniData.Global["pivotX"] = pivotX.ToString() ?? "";
		iniData.Global["pivotY"] = pivotY.ToString() ?? "";
		iniData.Global["shadow"] = shadow.ToString() ?? "";
		iniData.Global["shadowX"] = shadowX.ToString() ?? "";
		iniData.Global["shadowY"] = shadowY.ToString() ?? "";
		iniData.Global["height"] = ints[3].ToString() ?? "";
		iniData.Global["scaleIcon"] = scaleIcon.ToString() ?? "";
		iniData.Global["liquidMod"] = liquidMod.ToString() ?? "";
		iniData.Global["hatY"] = ints[23].ToString() ?? "";
		iniData.Global["flags"] = ints[1].ToString() ?? "";
		fileIniDataParser.WriteFile(path, iniData);
	}

	public static SourcePref ReadFromIni(string path)
	{
		SourcePref sourcePref = new SourcePref();
		IniData iniData = new FileIniDataParser().ReadFile(path, Encoding.UTF8);
		sourcePref.ints[4] = iniData.Global["x"].ToInt();
		sourcePref.ints[5] = iniData.Global["y"].ToInt();
		sourcePref.ints[2] = iniData.Global["z"].ToInt();
		sourcePref.pivotX = iniData.Global["pivotX"].ToInt();
		sourcePref.pivotY = iniData.Global["pivotY"].ToInt();
		sourcePref.shadow = iniData.Global["shadow"].ToInt();
		sourcePref.shadowX = iniData.Global["shadowX"].ToInt();
		sourcePref.shadowY = iniData.Global["shadowY"].ToInt();
		sourcePref.ints[3] = iniData.Global["height"].ToInt();
		sourcePref.scaleIcon = iniData.Global["scaleIcon"].ToInt();
		sourcePref.liquidMod = iniData.Global["liquidMod"].ToInt();
		sourcePref.ints[23] = iniData.Global["hatY"].ToInt();
		sourcePref.ints[1] = iniData.Global["flags"].ToInt();
		return sourcePref;
	}
}
