using UnityEngine;

public class TCExtra : TC
{
	public enum Type
	{
		None,
		Light,
		Effect,
		QuestBoard
	}

	public enum Hack
	{
		None,
		Candle
	}

	public Type type;

	public Hack hack;

	public Vector3 fixPos;

	public Vector3 heldPos;

	public Vector3 heldPosFlip;

	public float flipFixX;

	public bool flipSR;

	public bool flipSelf;

	public bool onlyInstalled = true;

	public bool useOffsetBack = true;

	public bool alwaysFlip;

	public bool flipBack;

	public bool colorParticle;

	public SpriteRenderer sr;

	public Transform[] listTrans;

	public Vector3[] dirPos;

	private SourceMaterial.Row _mat;

	private Vector3 v;

	private Vector3 originalScale;

	private Vector3 originalAngle;

	public override Vector3 FixPos => fixPos;

	private void Awake()
	{
		originalScale = base.transform.localScale;
		originalAngle = base.transform.localEulerAngles;
	}

	public override void OnDraw(ref Vector3 pos)
	{
		if (_mat != base.owner.material)
		{
			_mat = base.owner.material;
			RefreshColor();
		}
		RenderData data = base.owner.renderer.data;
		bool flag = base.owner.dir == 2 || base.owner.dir == 3;
		bool flag2 = base.owner.parent is Chara && (base.owner.parent as Chara).held == base.owner;
		bool flag3 = base.owner.dir % 2 == 1;
		bool flag4 = (alwaysFlip || !flag2) && flag3;
		if (flag && flipBack)
		{
			flag4 = !flag4;
		}
		bool flag5 = useOffsetBack && data.useOffsetBack && flag;
		if (base.owner.renderer.hasActor && base.owner.isChara)
		{
			v.x = base.owner.renderer.position.x + (flag5 ? data.offsetBack.x : data.offset.x);
			v.y = base.owner.renderer.position.y + (flag5 ? data.offsetBack.y : data.offset.y);
			v.z = base.owner.renderer.position.z + (flag5 ? data.offsetBack.z : data.offset.z) + FixPos.z + (flag2 ? (-0.5f) : 0f) + ((!flag2) ? 0f : (flag3 ? heldPosFlip.z : heldPos.z));
		}
		else if (dirPos.Length == 0)
		{
			v.x = base.owner.renderer.position.x + (flag5 ? data.offsetBack.x : data.offset.x) * (float)((!flag4) ? 1 : (-1)) + FixPos.x + flipFixX * (float)((!flag4) ? 1 : (-1)) + ((!flag2) ? 0f : (flag3 ? heldPosFlip.x : heldPos.x));
			v.y = base.owner.renderer.position.y + (flag5 ? data.offsetBack.y : data.offset.y) + FixPos.y + ((!flag2) ? 0f : (flag3 ? heldPosFlip.y : heldPos.y));
			v.z = base.owner.renderer.position.z + (flag5 ? data.offsetBack.z : data.offset.z) + FixPos.z + (flag2 ? (-0.5f) : 0f) + ((!flag2) ? 0f : (flag3 ? heldPosFlip.z : heldPos.z));
		}
		else
		{
			v.x = base.owner.renderer.position.x + (flag5 ? data.offsetBack.x : data.offset.x) + dirPos[base.owner.dir % dirPos.Length].x + ((!flag2) ? 0f : (flag3 ? heldPosFlip.x : heldPos.x));
			v.y = base.owner.renderer.position.y + (flag5 ? data.offsetBack.y : data.offset.y) + dirPos[base.owner.dir % dirPos.Length].y + ((!flag2) ? 0f : (flag3 ? heldPosFlip.y : heldPos.y));
			v.z = base.owner.renderer.position.z + (flag5 ? data.offsetBack.z : data.offset.z) + dirPos[base.owner.dir % dirPos.Length].z + (flag2 ? (-0.5f) : 0f) + ((!flag2) ? 0f : (flag3 ? heldPosFlip.z : heldPos.z));
		}
		base.transform.position = v;
		if (flipSR && sr.flipX != flag4)
		{
			sr.flipX = flag4;
		}
		if (flipSelf)
		{
			base.transform.localScale = (flag4 ? new Vector3(originalScale.x * -1f, originalScale.y, originalScale.z) : originalScale);
			base.transform.localEulerAngles = (flag4 ? new Vector3(originalAngle.x * -1f, originalAngle.y, originalAngle.z) : originalAngle);
		}
		bool flag6 = (!onlyInstalled || base.owner.IsInstalled || base.owner.isRoofItem) && base.owner.trait.UseExtra;
		switch (type)
		{
		case Type.QuestBoard:
			flag6 = false;
			foreach (Quest global in EMono.game.quests.globalList)
			{
				if (global.IsVisibleOnQuestBoard() && global.isNew)
				{
					flag6 = true;
				}
			}
			break;
		case Type.Effect:
			if (flag6)
			{
				TraitEffect traitEffect = base.owner.trait as TraitEffect;
				if (traitEffect.timer < Time.realtimeSinceStartup)
				{
					traitEffect.timer = Time.realtimeSinceStartup - Time.realtimeSinceStartup % traitEffect.Interval + traitEffect.Interval + traitEffect.data.delay;
					traitEffect.Proc(v);
				}
			}
			break;
		}
		if (hack == Hack.Candle)
		{
			Transform[] array = listTrans;
			foreach (Transform transform in array)
			{
				if (flag4)
				{
					if (transform.localPosition.z > 0f)
					{
						transform.localPosition = transform.localPosition.SetZ(0f - transform.localPosition.z);
					}
				}
				else if (transform.localPosition.z < 0f)
				{
					transform.localPosition = transform.localPosition.SetZ(0f - transform.localPosition.z);
				}
			}
		}
		if (base.owner.isHidden && base.owner.isChara && !EMono.pc.CanSee(base.owner.Chara))
		{
			flag6 = false;
		}
		if (base.gameObject.activeSelf != flag6)
		{
			base.gameObject.SetActive(flag6);
		}
	}

	public override void OnSetOwner()
	{
		base.OnSetOwner();
		RefreshColor();
	}

	public void RefreshColor()
	{
		if ((bool)sr)
		{
			Color color = base.owner.trait.ColorExtra ?? Color.white;
			color.a = sr.color.a;
			sr.color = color;
		}
		if (colorParticle)
		{
			ParticleSystem[] componentsInChildren = GetComponentsInChildren<ParticleSystem>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				ParticleSystem.MainModule main = componentsInChildren[i].main;
				main.startColor = (base.owner.isDyed ? base.owner.DyeMat : base.owner.material).GetColor().SetAlpha(main.startColor.color.a);
			}
		}
	}
}
