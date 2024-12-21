using System;
using UnityEngine;

public class ButtonState
{
	public enum ClickCriteria
	{
		ByDuration,
		ByMargin,
		ByDurationAndMargin
	}

	public static float delta;

	public string id;

	public float pressedTimer;

	public float timeSinceLastClick;

	public float timeSinceLastButtonDown;

	public float clickDuration = 0.5f;

	public float dragMargin = 12f;

	public int mouse = -1;

	public EInput.KeyMap keymap;

	public bool down;

	public bool up;

	public bool pressing;

	public bool clicked;

	public bool doubleClicked;

	public bool consumed;

	public bool dragging;

	public bool dragging2;

	public bool draggedOverMargin;

	public bool ignoreClick;

	public bool wasConsumed;

	public bool usedMouse;

	public bool usedKey;

	public bool ignoreWheel;

	public ClickCriteria clickCriteria = ClickCriteria.ByDurationAndMargin;

	public Action pressedLongAction;

	public bool pressedLong
	{
		get
		{
			if (pressing)
			{
				return pressedTimer >= clickDuration;
			}
			return false;
		}
	}

	public bool IsInDoubleClickDuration()
	{
		return timeSinceLastClick < 0.2f;
	}

	public void Update()
	{
		timeSinceLastClick += delta;
		if (mouse != -1 && !usedKey)
		{
			down = Input.GetMouseButtonDown(mouse);
			up = Input.GetMouseButtonUp(mouse);
			pressing = Input.GetMouseButton(mouse);
			if (down)
			{
				usedMouse = true;
			}
		}
		if (mouse == 1)
		{
			if (down)
			{
				EInput.dragStartPos2 = EInput.mpos;
			}
			else if (pressing)
			{
				if (Vector3.Distance(EInput.mpos, EInput.dragStartPos2) > dragMargin)
				{
					dragging2 = true;
				}
			}
			else
			{
				dragging2 = false;
			}
		}
		if (keymap != null && !usedMouse && !EInput.isInputFieldActive)
		{
			down = Input.GetKeyDown(keymap.key);
			up = Input.GetKeyUp(keymap.key);
			pressing = Input.GetKey(keymap.key);
			if (down)
			{
				usedKey = true;
			}
		}
		if (consumed)
		{
			if (down || up || pressing)
			{
				Clear();
				return;
			}
			consumed = false;
		}
		if (down)
		{
			wasConsumed = false;
			EInput.dragStartPos = EInput.mpos;
			if (timeSinceLastClick < 0.2f)
			{
				doubleClicked = true;
			}
		}
		else if (pressing)
		{
			pressedTimer += delta;
			if (EInput.mpos != EInput.dragStartPos)
			{
				dragging = true;
			}
			if (Vector3.Distance(EInput.mpos, EInput.dragStartPos) > dragMargin)
			{
				draggedOverMargin = true;
			}
		}
		else if (up)
		{
			if (ignoreWheel)
			{
				EInput.ignoreWheelDuration = 0.2f;
			}
			switch (clickCriteria)
			{
			case ClickCriteria.ByDuration:
				clicked = pressedTimer < clickDuration;
				break;
			case ClickCriteria.ByMargin:
				clicked = !draggedOverMargin;
				break;
			case ClickCriteria.ByDurationAndMargin:
				clicked = !draggedOverMargin && pressedTimer < clickDuration;
				break;
			}
			if (clicked)
			{
				timeSinceLastClick = 0f;
			}
			if (ignoreClick)
			{
				clicked = false;
			}
		}
		else
		{
			Clear();
		}
	}

	public void Consume()
	{
		Clear();
		consumed = (wasConsumed = true);
	}

	public void Clear()
	{
		pressedTimer = 0f;
		down = (up = (pressing = false));
		usedKey = (usedMouse = (clicked = (doubleClicked = (dragging = (draggedOverMargin = (ignoreClick = false))))));
	}
}
