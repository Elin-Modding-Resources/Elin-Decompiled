using System;
using System.Collections.Generic;
using UnityEngine;

public class GameSetting : ScriptableObject
{
	[Serializable]
	public class TransData
	{
		public Vector3 scale;

		public Vector3 euler;

		public Vector3 pos;
	}

	[Serializable]
	public class AudioSetting
	{
		public float maxRange;
	}

	[Serializable]
	public class UISetting
	{
		public List<Widget.Meta> widgetMetas;

		public List<Vector2> iconSizes;
	}

	[Serializable]
	public class RenderSetting
	{
		[Serializable]
		public class ZSetting
		{
			public float limit1;

			public float limit2;

			public float limitHidden;

			public float thresh1;

			public float max1;

			public float max2;

			public float mod1;

			public float multiZ;
		}

		[Serializable]
		public class AnimeSetting
		{
			public int[] animeStep;

			public int[] animeStepPC;

			public float idleTime;

			public float fixedMove;

			public float animeExtraTime;

			public float animeExtraTimeParty;

			public float nextFrameInterval;

			public float slowSpeed;

			public float hopStrength;

			public float fix1;

			public float fix2;

			public float fix3;

			public float fix4;

			public float destRadius;

			public float diagonalSpeed;

			public float maxProgressMod;

			public float regionSpeed;

			public int heightLimit = 80;

			public AnimationCurve hop;

			public AnimationCurve hopHuman;

			public AnimationCurve gradientZForward;

			public AnimationCurve gradientZBack;
		}

		[Serializable]
		public class TCSetting
		{
			public Vector3 textPos;

			public Vector3 textPosDead;

			public Vector3 statePos;

			public Vector3 censorPos;

			public Vector3 simpleTextPos;
		}

		[Serializable]
		public class ActorSetting
		{
			public Vector2 pccExtent;
		}

		[Serializable]
		public class MapBGSetting
		{
			public Material mat;

			public bool skyBox;

			public bool wall;

			public bool plane;
		}

		[Serializable]
		public class UD_MapBGSetting : UDictionary<MapBG, MapBGSetting>
		{
		}

		[Serializable]
		public class UD_FogSetting : UDictionary<FogType, ScreenGradingProfile.Fog>
		{
		}

		public ZSetting zSetting;

		public TCSetting tc;

		public AnimeSetting anime;

		public ActorSetting actor;

		public Vector3[] charaPos;

		public Vector3[] heldPos;

		public Vector3[] heldPosChara;

		public Vector3[] heldPosChara2;

		public Vector3[] mainHandPos;

		public Vector3[] offHandPos;

		public Vector3[] hatPos;

		public Vector3[] ridePos;

		public Vector3 posShackle;

		public Vector3 posGallows;

		public TransData[] dead;

		public Vector3[] rampFix;

		public Vector3 vFix;

		public Vector3 pccScale;

		public Vector3 waterFix;

		public float thingZ;

		public float laydownZ;

		public float charaZ;

		public float shadowAngle;

		public float tileMarkerZ;

		public float tileMarkerZFloor;

		public float crateHeight;

		public float hangedObjFixZ;

		public Vector3 shadowScale;

		public Vector3 shadowOffset;

		public Vector3 peakFix;

		public Vector3 peakFixBlock;

		public float roomHeightMod;

		public float defCharaHeight;

		public float alphaHair;

		public float shadowStrength;

		public Vector2 freePosFix;

		public AnimationFrame[] animeWalk;

		public UD_String_PaintPosition paintPos;

		public UD_MapBGSetting bgs;

		public UD_FogSetting fogs;
	}

	[Serializable]
	public class PassSetting
	{
		public SubPassData subDfault;

		public SubPassData subDead;

		public SubPassData subDeadPCC;

		public SubPassData subCrate;
	}

	[Serializable]
	public class StartSetting
	{
		public List<Prologue> prologues;

		public List<GameDifficultySetting> difficulties;

		public List<GamePrincipal> principals;
	}

	[Serializable]
	public class BalanceSetting
	{
		public int dateRevive;

		public int dateRegenerateZone;

		public int dateExpireRandomMap;

		public int numAdv;
	}

	[Serializable]
	public class WeatherSetting
	{
		public int splashCount;

		public float thunerInterval;
	}

	[Serializable]
	public class GenSetting
	{
		public float defaultBlockHeight;
	}

	[Serializable]
	public class EffectSetting
	{
		public UD_String_EffectData guns;
	}

	[Serializable]
	public class EffectData
	{
		public int num;

		public float delay;

		public string idEffect;

		public string idSound;

		public Sprite sprite;

		public bool eject;

		public Vector2 firePos;
	}

	public AudioSetting audio;

	public EffectSetting effect;

	public RenderSetting render;

	public PassSetting pass;

	public StartSetting start;

	public UISetting ui;

	public BalanceSetting balance;

	public GenSetting gen;

	public WeatherSetting weather;

	public UD_String_ElementRef elements;

	public int minsPerRegionMove;

	public float secsPerHour;

	public float dayRatioMod;

	public float fovPower;

	public float fovPowerChara;

	public float defaultActPace;

	public float defaultTurbo;

	public int maxGenHeight;

	public bool toolConsumeHP;

	public int defaultMapSize;

	public CoreConfig config;

	[NonSerialized]
	public Dictionary<string, Vector3> dictEquipOffsets;

	public void ApplyConfig()
	{
		CoreConfig.Reset();
		Core.Instance.OnChangeResolution();
	}

	public void CopyColors()
	{
		config.colors = Core.Instance.config.colors;
	}

	public void Init()
	{
		SubPassData.Default = (SubPassData.Current = pass.subDfault);
	}
}
