using UnityEngine;

public class TCOrbitChara : TCOrbit
{
	public GameObject goIcon;

	public SpriteRenderer iconRelation;

	public SpriteRenderer iconStatus;

	public SpriteRenderer barHP;

	public SpriteRenderer bgHP;

	public SpriteRenderer barHP2;

	public SpriteRenderer bgHP2;

	public new Chara owner;

	private float timer;

	private bool showHP;

	private bool showIcon;

	private bool showHP2;

	public override void OnSetOwner()
	{
		owner = base.owner as Chara;
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
		if ((!owner.isHidden || EMono.pc.CanSee(owner)) && (showHP || showHP2 || showIcon))
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

	public void RefreshAll()
	{
		showHP = (showHP2 = (showIcon = false));
		if (owner.isDead || owner.host != null)
		{
			return;
		}
		_ = owner.ai;
		_ = EMono.core.refs.orbitIcons;
		Emo2 emo = owner.emoIcon;
		if (emo == Emo2.none)
		{
			if (owner.isRestocking && !owner.IsPCParty && owner.trait.GetRestockedIcon() != 0 && !(EMono._zone is Zone_Dungeon))
			{
				emo = owner.trait.GetRestockedIcon();
			}
			else
			{
				bool flag = false;
				if (owner.quest != null && EMono.game.quests.list.Contains(owner.quest))
				{
					flag = true;
				}
				else if (owner.IsUnique)
				{
					string id = owner.id;
					if (!(id == "ashland"))
					{
						if (id == "fiama")
						{
							if (EMono.game.quests.GetPhase<QuestMain>() >= 200 && EMono.player.dialogFlags.TryGetValue("fiama1", 0) == 0)
							{
								flag = true;
							}
							else if (EMono.pc.homeBranch != null)
							{
								foreach (Chara member in EMono.pc.homeBranch.members)
								{
									if (member.isDead && member.GetInt(100) != 0)
									{
										flag = true;
										break;
									}
								}
							}
						}
					}
					else
					{
						int phase = EMono.game.quests.GetPhase<QuestMain>();
						if ((phase == 0 && EMono.player.dialogFlags.TryGetValue("ash1", 0) == 0) || phase == 200)
						{
							flag = true;
						}
					}
				}
				if (flag)
				{
					emo = Emo2.hint;
				}
			}
		}
		if (EMono.player.currentHotItem.Thing != null && EMono.player.currentHotItem.Thing.trait.GetHeldEmo(owner) != 0)
		{
			emo = EMono.player.currentHotItem.Thing.trait.GetHeldEmo(owner);
		}
		showIcon = emo != Emo2.none;
		if (showIcon)
		{
			iconStatus.sprite = EMono.core.refs.spritesEmo[(int)emo];
		}
		iconStatus.SetActive(showIcon);
		float num = (float)owner.hp / (float)owner.MaxHP;
		showHP = num < 0.9f && (owner.IsPCParty || owner.IsHostile() || (owner.enemy != null && owner.enemy.IsPCParty));
		if (showHP)
		{
			barHP.transform.SetLocalScaleX(num);
			barHP.SetActive(enable: true);
			bgHP.SetActive(enable: true);
		}
		else
		{
			barHP.SetActive(enable: false);
			bgHP.SetActive(enable: false);
		}
		if ((bool)barHP2)
		{
			float num2 = Mathf.Min(1f, (owner.parasite != null) ? ((float)owner.parasite.hp / (float)owner.parasite.MaxHP) : 1f, (owner.ride != null) ? ((float)owner.ride.hp / (float)owner.ride.MaxHP) : 1f);
			showHP2 = num2 < 0.9f;
			if (showHP2)
			{
				barHP2.transform.SetLocalScaleX(num2);
				barHP2.SetActive(enable: true);
				bgHP2.SetActive(enable: true);
			}
			else
			{
				barHP2.SetActive(enable: false);
				bgHP2.SetActive(enable: false);
			}
		}
	}
}
