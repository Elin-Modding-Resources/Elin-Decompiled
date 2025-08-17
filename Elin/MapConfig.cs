using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class MapConfig : EClass
{
	[JsonProperty]
	public string idSceneProfile;

	[JsonProperty]
	public string idFowProfile;

	[JsonProperty]
	public string idLiquid = "Default Dark";

	[JsonProperty]
	public string idLut;

	[JsonProperty]
	public string idRefraction = "Reflection";

	[JsonProperty]
	public string idSceneTemplate;

	[JsonProperty]
	public string idBiome;

	[JsonProperty]
	public bool indoor;

	[JsonProperty]
	public bool fullWallHeight;

	[JsonProperty]
	public bool forceHideOutbounds;

	[JsonProperty]
	public bool forceGodRay;

	[JsonProperty]
	public bool blossom;

	[JsonProperty]
	public bool retainDecal;

	[JsonProperty]
	public bool reverseRoof;

	[JsonProperty]
	public Weather.Condition fixedCondition = Weather.Condition.None;

	[JsonProperty]
	public SerializableColor colorLiquid;

	[JsonProperty]
	public float heightLightMod;

	[JsonProperty]
	public float lutBlend = 1f;

	[JsonProperty]
	public float lutBrightness = 1f;

	[JsonProperty]
	public float lutContrast = 1f;

	[JsonProperty]
	public float lutSaturation = 1f;

	[JsonProperty]
	public float blockHeight;

	[JsonProperty]
	public float shadowStrength = 1f;

	[JsonProperty]
	public MapBG bg;

	[JsonProperty]
	public FogType fog;

	[JsonProperty]
	public int embarkX;

	[JsonProperty]
	public int embarkY;

	[JsonProperty]
	public int seaDir;

	[JsonProperty]
	public int skyBlockHeight = 20;

	[JsonProperty]
	public int hour = -1;

	[JsonProperty]
	public SerializableColor colorScreen = new SerializableColor(0, 0, 0, 0);

	[JsonProperty]
	public SerializableColor colorSea = new SerializableColor(0, 0, 0, 0);

	private IEnumerable<string> SceneProfileIDs()
	{
		SceneProfile[] array = Resources.LoadAll<SceneProfile>("Scene/Profile/");
		List<string> list = new List<string>();
		SceneProfile[] array2 = array;
		foreach (SceneProfile sceneProfile in array2)
		{
			list.Add(sceneProfile.name.Replace("SceneProfile_", ""));
		}
		return list;
	}

	private IEnumerable<string> FowProfileIDs()
	{
		FowProfile[] array = Resources.LoadAll<FowProfile>("Scene/Profile/Fow/");
		List<string> list = new List<string>();
		FowProfile[] array2 = array;
		foreach (FowProfile fowProfile in array2)
		{
			list.Add(fowProfile.name.Replace("FowProfile_", ""));
		}
		return list;
	}
}
