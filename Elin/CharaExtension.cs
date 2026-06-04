public static class CharaExtension
{
	public static void RefreshSpriteRenderer(this Card card)
	{
		CardActor cardActor = card.renderer?.actor;
		if (!(cardActor?.sr == null))
		{
			cardActor.sr.sprite = card.GetSprite();
			if (!(cardActor.sr.sprite == null) && cardActor.mpb != null)
			{
				cardActor.mpb.SetTexture(SpriteHelper.MainTex, cardActor.sr.sprite.texture);
				cardActor.RefreshSprite();
			}
		}
	}

	public static void DestroyImmediate(this Chara chara)
	{
		chara.homeBranch?.BanishMember(chara, skipMsg: true);
		chara.SetFaction(EClass.Wilds);
		EClass.game.cards.listAdv.Remove(chara);
		chara.Destroy();
	}

	public static void SetSpriteOverride(this Chara chara, string spriteId = null)
	{
		chara.SetStr("sprite_override", spriteId);
		chara.RefreshSpriteRenderer();
	}

	public static void SetPortraitOverride(this Chara chara, string portraitId = null)
	{
		chara.SetStr("portrait_override", portraitId);
	}

	public static void SetDramaOverride(this Chara chara, string dramaId = null)
	{
		chara.SetStr("drama_override", dramaId);
	}
}
