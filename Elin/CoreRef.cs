using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CoreRef : ScriptableObject
{
	[Serializable]
	public class UDIconWeather : UDictionary<Weather.Condition, Sprite>
	{
	}

	[Serializable]
	public class UDInvTab : UDictionary<UIInventory.Mode, Sprite>
	{
	}

	[Serializable]
	public class UDInvStyle : UDictionary<string, InventoryStyle>
	{
	}

	[Serializable]
	public class InventoryStyle
	{
		public Sprite bg;

		public Vector2 sizeDelta;

		public Vector2 gridSize;

		public Vector2 sizeContainer;

		public Vector2 posFix;

		public Color gridColor;

		public SoundData sound;
	}

	[Serializable]
	public class Icons
	{
		[Serializable]
		public class Affinity
		{
			public Sprite hate;

			public Sprite dontLike;

			public Sprite like;

			public Sprite love;
		}

		[Serializable]
		public class Enc
		{
			public Sprite mat;

			public Sprite card;

			public Sprite cat;

			public Sprite enc;

			public Sprite rune;

			public Sprite trait;

			public Sprite traitFood;

			public Sprite mod;

			public Sprite weaponEnc;
		}

		public Sprite suspend;

		public Sprite resume;

		public Sprite delete;

		public Sprite go;

		public Sprite talk;

		public Sprite caste;

		public Sprite home;

		public Sprite bed;

		public Sprite work;

		public Sprite uniformM;

		public Sprite uniformF;

		public Sprite inspect;

		public Sprite up;

		public Sprite down;

		public Sprite trash;

		public Sprite trans;

		public Sprite fav;

		public Sprite noHotItem;

		public Sprite defaultHotItem;

		public Sprite defaultAbility;

		public Sprite targetSelf;

		public Sprite targetAny;

		public Sprite mana;

		public Sprite stamina;

		public Sprite shared;

		public Sprite personal;

		public Affinity affnity;

		public Enc enc;

		public UDInvTab invTab;

		public UDIconWeather weather;

		public List<Sprite> quality;
	}

	[Serializable]
	public class Biomes
	{
		public UD_Biome dict;

		public BiomeProfile Plain;

		public BiomeProfile Sand;

		public BiomeProfile Water;
	}

	[Serializable]
	public class Rects
	{
		public RectData bottomRight;

		public RectData inv;

		public RectData invFloat;

		public RectData invCenter;

		public RectData center;

		public RectData centerFloat;

		public RectData abilityDebug;
	}

	[Serializable]
	public class StateIcons
	{
		public MultiSprite combat;

		public MultiSprite sleep;

		public MultiSprite gather;

		public MultiSprite meditation;

		public MultiSprite selfharm;
	}

	[Serializable]
	public class PopperSprites
	{
		public Sprite[] damage;
	}

	[Serializable]
	public class OrbitIcons
	{
		public Sprite Default;

		public Sprite Search;

		public Sprite Hostile;

		public Sprite Guest;

		public Sprite healthCritical;

		public Sprite healthDanger;

		public Sprite healthLow;
	}

	[Serializable]
	public class Crawlers
	{
		public Crawler start;
	}

	[Serializable]
	public class ButtonAssets
	{
		public Sprite bgDefault;

		public Sprite bgNew;

		public Sprite bgSuperior;

		public Sprite bgLegendary;

		public Sprite bgMythical;

		public Sprite bgArtifact;
	}

	[Serializable]
	public class TCs
	{
		public TCText text;

		public TCCensored censored;

		public TCState state;

		public TCSimpleText simpleText;

		public TCSimpleText simpleTextIcon;

		public Sprite spriteVisited;

		public Sprite spriteConquer;

		public Sprite spriteDeath;
	}

	[Serializable]
	public class TextureDatas
	{
		public TextureData block;

		public TextureData block_snow;

		public TextureData floor;

		public TextureData floor_snow;

		public TextureData objs;

		public TextureData objs_snow;

		public TextureData objs_S;

		public TextureData objs_S_snow;

		public TextureData objs_L;

		public TextureData objs_L_snow;

		public TextureData objs_C;

		public TextureData objs_CLL;

		public TextureData shadows;

		public TextureData fov;

		public TextureData world;

		public TextureData roofs;

		public TextureData objs_SS;

		public TextureData bird;
	}

	[Serializable]
	public class Renderers
	{
		public RenderData pcc;

		public RenderData pcc_L;

		public RenderData obj_wheat;

		public RenderData objS_flat;

		public RenderData objL_harvest;

		public RenderData obj_paint;

		public RenderData objs_shrine;
	}

	[Serializable]
	public class DefaultRoof
	{
		public int idBlock;

		public int idRamp;
	}

	[Serializable]
	public class PrefData
	{
		public SourcePref sonwputit1;

		public SourcePref snowputit2;

		public SourcePref pcc;
	}

	public Biomes biomes;

	public Crawlers crawlers;

	public StateIcons stateIcons;

	public OrbitIcons orbitIcons;

	public ButtonAssets buttonAssets;

	public PopperSprites popperSprites;

	public TCs tcs;

	public Icons icons;

	public Renderers renderers;

	public TextureDatas textureData;

	public Rects rects;

	public List<Effect> fireworks;

	public List<Sprite> spritesCorner;

	public List<Sprite> icon_HotItem;

	public List<SpriteAsset> bg_msg;

	public List<Sprite> spritesHighlight;

	public List<Sprite> spritesHighlightSpeed;

	public List<Sprite> spritesPotential;

	public List<Sprite> spritesEmo;

	public List<Sprite> spritesContainerIcon;

	public List<BGMData> bgms;

	public List<DefaultRoof> defaultRoofs;

	public Dictionary<int, BGMData> dictBGM;

	public UDInvStyle invStyle;

	public Sprite spriteRecipe;

	public Sprite spriteNull;

	public Sprite spriteArea;

	public Sprite spriteNoIng;

	public Sprite spriteButtonGrid;

	public Sprite spriteButtonGridBad;

	public Sprite spriteThingActor;

	public Sprite spriteDefaultCondition;

	public Material matUIObj;

	public Material matUIPortraitChara;

	public MsgColors msgColors;

	public GameObject debugText;

	public UD_Int_String dictSketches;

	public PrefData prefs;

	public float testColor;

	public float testColor2;

	public static string[] ArtDirs = new string[10] { "000-099", "100-199 Elin", "200-299 Elin Chara", "300-399 Ylva Illust", "400-499 Ylva Other", "500-599 Ylva Wall", "600-699 Kickstarter", "700-799 Goods", "800-899 Etc", "900-999 MT" };

	public void Init()
	{
		if (bgms.Count == 0)
		{
			RebuildBGMList();
		}
		RefreshBGM();
	}

	public void RefreshBGM()
	{
		dictBGM = new Dictionary<int, BGMData>();
		int num = 0;
		foreach (BGMData bgm in bgms)
		{
			dictBGM.Add(bgm.id, bgm);
			num++;
		}
	}

	public void RebuildBGMList()
	{
		bgms.Clear();
		BGMData[] array = Resources.LoadAll<BGMData>("Media/Sound/BGM/");
		List<AudioClip> list = Resources.LoadAll<AudioClip>("Media/Sound/BGM/").ToList();
		BGMData[] array2 = array;
		foreach (BGMData bGMData in array2)
		{
			if (bGMData.id == 0)
			{
				continue;
			}
			bgms.Add(bGMData);
			foreach (AudioClip item in list)
			{
				if (bGMData.clip == item)
				{
					list.Remove(item);
					break;
				}
			}
		}
		foreach (AudioClip item2 in list)
		{
			Debug.Log("Unused:" + item2.name);
		}
		RefreshBGM();
		Debug.Log("Rebuild BGM Done.");
	}

	public void RebuildSketchList()
	{
		dictSketches.Clear();
		for (int i = 0; i < 10; i++)
		{
			Sprite[] array = Resources.LoadAll<Sprite>("Media/Gallery/" + GetArtDir(i * 100));
			foreach (Sprite sprite in array)
			{
				int key = sprite.name.Split('_')[0].ToInt();
				dictSketches[key] = sprite.name;
				Debug.Log(key + " " + sprite.name);
			}
		}
		Debug.Log("Sketches rebuilt:" + dictSketches.Count());
	}

	public static string GetArtDir(int a)
	{
		return ArtDirs[a / 100];
	}

	public void RebuildBiomeList()
	{
		biomes.dict.Clear();
		BiomeProfile[] array = Resources.LoadAll<BiomeProfile>("World/Map/Biome/");
		BiomeProfile[] array2 = array;
		foreach (BiomeProfile biomeProfile in array2)
		{
			biomes.dict.Add(biomeProfile.name, biomeProfile);
			Debug.Log(biomeProfile.name);
		}
		Debug.Log("Biomes rebuilt:" + array.Length);
	}

	public void RebuildFireworks()
	{
		fireworks.Clear();
		Effect[] array = Resources.LoadAll<Effect>("Media/Effect/General/Firework/");
		Effect[] array2 = array;
		foreach (Effect effect in array2)
		{
			fireworks.Add(effect);
			Debug.Log(effect.name);
		}
		Debug.Log("Fireworks rebuilt:" + array.Length);
	}
}
