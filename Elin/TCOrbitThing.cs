using UnityEngine;

public class TCOrbitThing : TCOrbit
{
	public GameObject goIcon;

	public SpriteRenderer iconStatus;

	public new Thing owner;

	private float timer;

	public override void OnSetOwner()
	{
		owner = base.owner as Thing;
		RenderData data = owner.renderer.data;
		goIcon.transform.SetLocalPositionY(data.offset.y + data.size.y + 0.32f);
		RefreshAll();
	}

	public override void Refresh()
	{
		timer += Time.deltaTime;
		if (timer > 0.2f)
		{
			timer = 0f;
			RefreshAll();
		}
		if (!owner.isHidden || EMono.pc.CanSee(owner))
		{
			if (!base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(value: true);
			}
			base.transform.position = owner.renderer.position;
		}
		else if (base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	public override void RefreshAll()
	{
		_ = EMono.core.refs.orbitIcons;
		Emo2 emo = Emo2.none;
		if (owner.IsInstalled)
		{
			if (owner.isRestocking && !owner.IsPCParty && owner.trait.GetRestockedIcon() != 0 && !(EMono._zone is Zone_Dungeon))
			{
				emo = owner.trait.GetRestockedIcon();
			}
			else if (EMono.player.currentHotItem.Thing?.trait is TraitToolShears && owner.CanBeSheared())
			{
				emo = Emo2.fur;
			}
		}
		bool flag = emo != Emo2.none;
		if (flag)
		{
			iconStatus.sprite = EMono.core.refs.spritesEmo[(int)emo];
		}
		iconStatus.SetActive(flag);
	}
}
