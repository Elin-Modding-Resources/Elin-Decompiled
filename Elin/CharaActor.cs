using UnityEngine;

public class CharaActor : CardActor
{
	public new Chara owner;

	public void SetOwnerAsChara()
	{
		owner = base.owner as Chara;
	}

	public override void OnSetOwner()
	{
		SetOwnerAsChara();
		sr.sprite = owner.GetSprite();
		mpb.SetTexture("_MainTex", sr.sprite.texture);
		IdleFrame();
	}

	public override void NextFrame()
	{
		RefreshSprite();
	}

	public override void ReloadSprite()
	{
		if (owner != null)
		{
			sr.sprite = owner.GetSprite();
			RefreshSprite();
		}
	}

	public override void RefreshSprite()
	{
		Sprite sprite = sr.sprite;
		Texture2D texture = sprite.texture;
		Rect textureRect = sprite.textureRect;
		if (!isPCC)
		{
			sr.flipX = owner.flipX;
		}
		Vector4 value = new Vector4(textureRect.x / (float)texture.width, textureRect.min.y / (float)texture.height, textureRect.max.x / (float)texture.width, textureRect.max.y / (float)texture.height);
		mpb.SetVector("_Rect", value);
		mpb.SetFloat("_PixelHeight", sprite.rect.height);
	}
}
