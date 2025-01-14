using UnityEngine;

public class CardActor : Actor
{
	public enum Type
	{
		Default,
		Boat,
		Canvas,
		MultiSprite
	}

	public Card owner;

	public SpriteRenderer sr;

	public SpriteRenderer sr2;

	public SpriteReplacerAnimation srAnime;

	public MaterialPropertyBlock mpb;

	public MaterialPropertyBlock mpb2;

	public Type type;

	private float destLiquid;

	private float spriteTimer;

	private int spriteIndex;

	private Vector3 originalScale;

	protected static Vector3 tempV;

	protected static Vector3 tempV2;

	public virtual bool isPCC => false;

	public virtual int currentDir => 0;

	private void Awake()
	{
		originalScale = base.transform.localScale;
	}

	public void SetOwner(Card t)
	{
		base.gameObject.SetActive(value: true);
		owner = t;
		if (mpb == null)
		{
			mpb = new MaterialPropertyBlock();
		}
		mpb.SetColor("_Mat", EMono.Colors.matColors.TryGetValue(t.material.alias).main);
		if ((bool)sr)
		{
			sr.SetPropertyBlock(mpb);
		}
		if ((bool)sr2)
		{
			if (mpb2 == null)
			{
				mpb2 = new MaterialPropertyBlock();
			}
			mpb2.SetColor("_Mat", Color.white);
			sr2.SetPropertyBlock(mpb2);
		}
		OnSetOwner();
		if (Application.isEditor)
		{
			base.gameObject.name = owner.Name;
		}
		if (type == Type.Boat)
		{
			srAnime.SetData(owner.id + "_anime");
		}
		RefreshSprite();
	}

	public virtual void OnSetOwner()
	{
	}

	public virtual void IdleFrame()
	{
	}

	public virtual void NextFrame()
	{
	}

	public virtual void NextDir()
	{
	}

	public virtual int GetFrame()
	{
		return 0;
	}

	public virtual void OnRender(RenderParam p)
	{
		this.SetActive(enable: true);
		SubPassData current = SubPassData.Current;
		RenderData data = owner.renderer.data;
		if (data.useOffsetBack && owner != EMono.pc.held)
		{
			bool num;
			if (!data.symmetry)
			{
				if (p.dir == 1)
				{
					goto IL_006a;
				}
				num = p.dir == 2;
			}
			else
			{
				num = p.dir % 2 == 1;
			}
			if (num)
			{
				goto IL_006a;
			}
		}
		tempV.x = p.x + data.offset.x + current.offset.x;
		tempV.y = p.y + data.offset.y + current.offset.y;
		tempV.z = p.z + data.offset.z + current.offset.z;
		goto IL_015c;
		IL_006a:
		tempV.x = p.x + data.offsetBack.x + current.offset.x;
		tempV.y = p.y + data.offsetBack.y + current.offset.y;
		tempV.z = p.z + data.offsetBack.z + current.offset.z;
		goto IL_015c;
		IL_015c:
		if (isPCC)
		{
			RenderDataPcc renderDataPcc = data as RenderDataPcc;
			CharaActorPCC charaActorPCC = this as CharaActorPCC;
			tempV.x = (float)(int)(tempV.x * 100f) * 0.01f;
			tempV.y = (float)(int)(tempV.y * 100f) * 0.01f;
			tempV2.x = charaActorPCC.provider.vCurrent.source.size.x * current.scale.x;
			tempV2.y = charaActorPCC.provider.vCurrent.source.size.y * current.scale.y;
			tempV2.z = renderDataPcc._scale.z * current.scale.z;
			base.transform.localRotation = current.rotation;
		}
		else
		{
			tempV2.x = originalScale.x * current.scale.x;
			tempV2.y = originalScale.y * current.scale.y;
			tempV2.z = originalScale.z * current.scale.z;
		}
		if (type == Type.Boat)
		{
			float floatY = EMono.scene.screenElin.tileMap.floatY;
			int floatV = EMono.scene.screenElin.tileMap.floatV;
			sr2.transform.localPosition = new Vector3(0f, -0.005f * floatY, sr2.transform.localPosition.z);
			int num2 = 0;
			if (floatV == 1)
			{
				if (floatY < 2f)
				{
					num2 = 0;
				}
				else
				{
					num2 = 1;
				}
			}
			else if (floatY < 3f)
			{
				num2 = 3;
			}
			else
			{
				num2 = 2;
			}
			num2 = ((floatV == 1) ? ((!(floatY < 0f)) ? ((floatY < 3f) ? 1 : 2) : 0) : ((!(floatY < 2f)) ? ((!(floatY < 5f)) ? 2 : 3) : 0));
			srAnime.sr.sprite = srAnime.data.GetSprites().TryGet(num2);
		}
		base.transform.position = tempV;
		base.transform.localScale = tempV2;
		destLiquid = Mathf.Lerp(destLiquid, p.liquidLv, Core.gameDelta * (float)((destLiquid > (float)p.liquidLv) ? 90 : 20));
		mpb.SetFloat("_MatColor", p.matColor);
		mpb.SetFloat("_Color", p.color);
		mpb.SetFloat("_Liquid", destLiquid);
		sr.SetPropertyBlock(mpb);
		if ((bool)sr2)
		{
			if ((bool)sr2.sprite)
			{
				mpb2.SetTexture("_MainTex", sr2.sprite.texture);
			}
			mpb2.SetTexture("_MaskTex", sr.sprite.texture);
			mpb2.SetFloat("_MatColor", p.matColor);
			mpb2.SetFloat("_Color", p.color);
			mpb2.SetFloat("_Liquid", destLiquid);
			sr2.SetPropertyBlock(mpb2);
		}
		if (owner.sourceCard.replacer.data == null)
		{
			return;
		}
		SpriteData data2 = owner.sourceCard.replacer.data;
		if (data2.frame <= 1)
		{
			return;
		}
		spriteTimer += Core.delta;
		if (spriteTimer >= data2.time)
		{
			spriteTimer -= data2.time;
			spriteIndex++;
			if (spriteIndex >= data2.frame)
			{
				spriteIndex = 0;
			}
			sr.sprite = ((data2.spritesSnow != null && p.snow) ? data2.spritesSnow[spriteIndex] : data2.sprites[spriteIndex]);
		}
	}

	public virtual void RefreshSprite()
	{
		switch (type)
		{
		case Type.Canvas:
			if (owner == null || !sr2)
			{
				break;
			}
			if (owner.c_textureData != null)
			{
				sr2.sprite = owner.GetPaintSprite();
				if (owner.trait is TraitCanvas traitCanvas)
				{
					sr2.transform.SetLocalScale(traitCanvas.Scale, traitCanvas.Scale, 1f);
				}
			}
			else
			{
				sr2.sprite = null;
			}
			break;
		case Type.Boat:
			sr2.SetActive(owner.Cell.IsFloorWater);
			break;
		case Type.MultiSprite:
			sr2.sprite = owner.GetSprite();
			break;
		}
		int dir = owner.dir;
		SpriteRenderer spriteRenderer = sr;
		Sprite sprite2 = (sr.sprite = owner.GetSprite(dir));
		Sprite sprite4 = (spriteRenderer.sprite = sprite2);
		Sprite sprite5 = sprite4;
		sr.flipX = dir == 1 || dir == 3;
		mpb.SetTexture("_MainTex", sprite5.texture);
		Vector4 value = new Vector4(sprite5.textureRect.min.x / (float)sprite5.texture.width, sprite5.textureRect.min.y / (float)sprite5.texture.height, sprite5.textureRect.max.x / (float)sprite5.texture.width, sprite5.textureRect.max.y / (float)sprite5.texture.height);
		mpb.SetVector("_Rect", value);
		mpb.SetFloat("_PixelHeight", sprite5.rect.height);
		if (!sr2)
		{
			return;
		}
		if ((bool)sr2.sprite && (bool)sr2.sprite.texture)
		{
			sr2.flipX = dir == 1 || dir == 3;
			sprite5 = sr2.sprite;
			if ((bool)sprite5)
			{
				_ = sprite5.textureRect;
				value = new Vector4(sprite5.textureRect.min.x / (float)sprite5.texture.width, sprite5.textureRect.min.y / (float)sprite5.texture.height, sprite5.textureRect.max.x / (float)sprite5.texture.width, sprite5.textureRect.max.y / (float)sprite5.texture.height);
				mpb2.SetTexture("_MainTex", sprite5.texture);
				mpb2.SetVector("_Rect", value);
				mpb2.SetFloat("_PixelHeight", sprite5.rect.height);
			}
		}
		else
		{
			sr2.sprite = null;
		}
	}

	public virtual void Kill()
	{
		PoolManager.Despawn(base.transform);
	}
}
