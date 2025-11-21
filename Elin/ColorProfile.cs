using System;
using UnityEngine;

public class ColorProfile : EScriptable
{
	[Serializable]
	public class RarityColors
	{
		public Color colorC;

		public Color colorR;

		public Color colorSR;

		public Color colorSSR;

		public Color colorLE;

		public float lightMod;
	}

	[Serializable]
	public class BlockColors
	{
		public const int DefColor = 104025;

		public const int DefColorInactive = -1214986;

		public const int DefColorActive = -1173751;

		public const int DefColorValid = -2747875;

		public const int DefColorWarning = -2828814;

		public Color _valid;

		public Color _warning;

		public Color _active;

		public Color _inactive;

		public Color _activeOpacity;

		public Color _inactiveOpacity;

		public Color _passive;

		public Color _mapHighlight;

		public int Valid;

		public int Warning;

		public int Inactive;

		public int Active;

		public int ActiveOpacity;

		public int InactiveOpacity;

		public int Passive;

		public int MapHighlight;

		public void Init()
		{
			Valid = GetColor(ref _valid);
			Warning = GetColor(ref _warning);
			Inactive = GetColor(ref _inactive);
			Active = GetColor(ref _active);
			InactiveOpacity = GetColor(ref _inactiveOpacity);
			ActiveOpacity = GetColor(ref _activeOpacity);
			Passive = GetColor(ref _passive);
			MapHighlight = GetColor(ref _mapHighlight);
		}

		public int GetColor(ref Color c)
		{
			return -((int)(c.a * 10f) * 262144 + (int)(c.r * 64f) * 4096 + (int)(c.g * 64f) * 64 + (int)(c.b * 64f));
		}
	}

	[Serializable]
	public class TextColors
	{
		public Color damage;

		public Color damagePC;

		public Color damagePCParty;

		public Color damageMana;

		public Color damageStamina;
	}

	[Serializable]
	public class PCLights
	{
		public LightData torch;

		public LightData cateye;
	}

	public UD_String_MatData matColors;

	public UD_String_LightData lightColors;

	public UD_String_Color elementColors;

	public RarityColors rarityColors;

	public BlockColors blockColors;

	public TextColors textColors;

	public Gradient gradientLVComparison;

	public PCLights pcLights;

	public Color colorIngCost;

	public Color colorIngPredict;

	public Color colorIngReq;

	public Color colorNew;

	public Color colorAct;

	public Color colorActWarnning;

	public Color colorActCriticalWarning;

	public Color colorHostileAct;

	public Color colorFriend;

	public Color colorHostile;

	public Color colorBlood;

	public Color colorBuff;

	public Color colorDebuff;

	public SkinColorProfile Skin => SkinManager.CurrentColors;

	public SkinColorProfile Dark => SkinManager.Instance.skinDark.colors._default;

	public SkinColorProfile Light => SkinManager.Instance.skinLight.colors._default;

	public void Init()
	{
		blockColors.Init();
	}

	public Color GetRarityColor(int r, bool light = false)
	{
		RarityColors rarityColors = this.rarityColors;
		Color result = rarityColors.colorC;
		switch (r)
		{
		case 2:
			result = rarityColors.colorR;
			break;
		case 3:
			result = rarityColors.colorSR;
			break;
		case 4:
			result = rarityColors.colorSSR;
			break;
		case 5:
			result = rarityColors.colorLE;
			break;
		}
		if (light)
		{
			result *= rarityColors.lightMod;
		}
		return result;
	}

	private void OnValidate()
	{
		if (Application.isPlaying && EClass.core.IsGameStarted)
		{
			EClass._map.RefreshFOVAll();
		}
	}
}
