using Newtonsoft.Json;
using UnityEngine;

public class CustomGunEffectData : GameSetting.EffectData
{
	public static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
	{
		NullValueHandling = NullValueHandling.Ignore,
		ContractResolver = new GameIOContext.WritablePropertiesOnlyResolver()
	};

	public CustomGunEffectData()
	{
		num = 1;
		delay = 0.1f;
		eject = true;
		firePos = new Vector2(0.23f, 0.04f);
		idSprite = "ranged_gun";
		idSoundEject = "bullet_drop";
	}

	public void ResolveSprite()
	{
		if (!idSprite.IsEmpty())
		{
			string spritePath = idSprite;
			string name = idSprite;
			sprite = ModUtil.LoadSprite(spritePath, null, name) ?? Resources.Load<Sprite>("Media/Effect/General/" + idSprite) ?? Resources.Load<Sprite>(idSprite) ?? Resources.Load<Sprite>("Media/Effect/General/ranged_gun");
		}
	}
}
