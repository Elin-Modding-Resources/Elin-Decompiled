using UnityEngine;

public class TCUI : TC
{
	private RectTransform _rect;

	protected Vector3 lastPos;

	public override bool isUI => true;

	public override Vector3 FixPos => TC._setting.textPos;

	protected virtual void Awake()
	{
		_rect = this.Rect();
	}

	public override void OnDraw(ref Vector3 pos)
	{
		GameObject go = base.gameObject;
		Vector3 _pos = pos;
		EMono.core.actionsLateUpdate.Add(delegate
		{
			if (go != null)
			{
				lastPos = _pos;
				Vector3 position = Camera.main.WorldToScreenPoint(_pos);
				position.z = 0f;
				Vector3 vector = FixPos;
				if (render.hasActor && !render.actor.isPCC && (bool)render.actor && (bool)render.actor.sr.sprite)
				{
					float num = 128f / render.actor.sr.sprite.rect.height;
					int pivotY = render.owner.Pref.pivotY;
					vector = FixPos + new Vector3(0f, num * ((float)pivotY - 48f), 0f);
				}
				position += vector * EMono.screen.Zoom;
				_rect.position = position;
			}
		});
	}

	public void DrawImmediate(ref Vector3 pos)
	{
		Vector3 position = Camera.main.WorldToScreenPoint(pos);
		position.z = 0f;
		position += FixPos * EMono.screen.Zoom;
		_rect.position = position;
	}
}
