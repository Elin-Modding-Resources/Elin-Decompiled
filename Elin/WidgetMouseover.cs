using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class WidgetMouseover : Widget
{
	public static WidgetMouseover Instance;

	public Chara roster;

	public UIText textName;

	public LayoutGroup layout;

	public CanvasGroup cg;

	public Tween tween;

	private float timer;

	public override void OnActivate()
	{
		Instance = this;
		Hide(immediate: true);
	}

	public void Refresh()
	{
		timer += Core.delta;
		PointTarget mouseTarget = EMono.scene.mouseTarget;
		if (!mouseTarget.hasTargetChanged && timer < 0.1f)
		{
			return;
		}
		timer = 0f;
		bool flag = EInput.isShiftDown && mouseTarget.pos.Equals(EMono.pc.pos) && (EMono.pc.ride != null || EMono.pc.parasite != null);
		if (((!flag && mouseTarget.target == null) || (ActionMode.IsAdv && Input.GetMouseButton(0))) && roster == null)
		{
			Hide();
			return;
		}
		string text = "";
		Card card = mouseTarget.card;
		if (roster != null && roster.ExistsOnMap)
		{
			flag = false;
			card = roster;
		}
		if (flag)
		{
			if (EMono.pc.ride != null)
			{
				text += EMono.pc.ride.GetHoverText();
				text += EMono.pc.ride.GetHoverText2();
			}
			if (EMono.pc.parasite != null)
			{
				if (EMono.pc.ride != null)
				{
					text += Environment.NewLine;
				}
				text += EMono.pc.parasite.GetHoverText();
				text += EMono.pc.parasite.GetHoverText2();
			}
		}
		else if (card != null)
		{
			text = card.GetHoverText();
			if (roster == null)
			{
				int count = EMono.scene.mouseTarget.cards.Count;
				if (count > 1)
				{
					text += "otherCards".lang((count - 1).ToString() ?? "");
				}
			}
			text += card.GetHoverText2();
			if (roster == null && mouseTarget.target != card)
			{
				text = text + Environment.NewLine + mouseTarget.target.InspectName;
			}
		}
		else if (mouseTarget.target != null)
		{
			text = mouseTarget.target.InspectName;
		}
		Show(text);
	}

	public void Show(string s)
	{
		layout.SetActive(enable: true);
		cg.alpha = 1f;
		TweenUtil.KillTween(ref tween);
		textName.text = s;
		switch (this.Rect().GetAnchor())
		{
		case RectPosition.TopLEFT:
		case RectPosition.BottomLEFT:
			layout.childAlignment = TextAnchor.MiddleLeft;
			break;
		case RectPosition.TopRIGHT:
		case RectPosition.BottomRIGHT:
			layout.childAlignment = TextAnchor.MiddleRight;
			break;
		default:
			layout.childAlignment = TextAnchor.MiddleCenter;
			break;
		}
		layout.RebuildLayout();
	}

	public void Hide(bool immediate = false)
	{
		imageBG.raycastTarget = false;
		if (immediate)
		{
			layout.SetActive(enable: false);
			return;
		}
		TweenUtil.KillTween(ref tween);
		tween = cg.DOFade(0f, 0.3f).OnComplete(delegate
		{
			if ((bool)layout)
			{
				layout.SetActive(enable: false);
			}
		}).SetEase(Ease.InQuad);
	}

	public override void OnManagerActivate()
	{
		base.OnManagerActivate();
		imageBG.raycastTarget = true;
		Show("[" + Lang.Get("WidgetMouseover") + "]");
	}

	public override void OnManagerDeactivate()
	{
		base.OnManagerDeactivate();
		Hide(immediate: true);
	}
}
