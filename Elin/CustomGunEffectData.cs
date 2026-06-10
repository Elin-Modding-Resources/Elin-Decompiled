using Newtonsoft.Json;
using UnityEngine;

[JsonObject(MemberSerialization.OptOut)]
public class CustomGunEffectData : GameSetting.EffectData
{
	public bool forceLaser;

	public bool forceRail;

	public string caneColor;

	public bool caneColorBlend;

	public string idSprite = "ranged_gun";

	public string idSoundEject = "bullet_drop";

	public GameSetting.EffectData CreateEffectData()
	{
		return new CustomGunEffectData
		{
			sprite = (ModUtil.LoadSprite(idSprite) ?? Resources.Load<Sprite>("Media/Effect/General/" + idSprite)),
			eject = eject,
			firePos = firePos,
			num = num,
			delay = delay,
			idEffect = idEffect,
			idSound = idSound
		};
	}

	public static CustomGunEffectData CreateFromId(string id)
	{
		if (!EClass.setting.effect.guns.TryGetValue(id, out var value))
		{
			return null;
		}
		return new CustomGunEffectData
		{
			num = value.num,
			delay = value.delay,
			idEffect = value.idEffect,
			idSound = value.idSound,
			idSprite = (value.sprite ? value.sprite.name : ""),
			eject = value.eject,
			firePos = value.firePos
		};
	}
}
