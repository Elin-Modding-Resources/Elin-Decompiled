using System;
using UnityEngine;
using UnityEngine.UI;

public class WidgetMemo : Widget
{
	public class Extra
	{
		public bool autoHide;
	}

	public static WidgetMemo Instance;

	public int id;

	[NonSerialized]
	public float hideTimer;

	public InputField input;

	public Window window;

	public Image bgInput;

	public Text textInput;

	public UIButton buttonClose;

	public UIButton buttonEdit;

	public UIButton buttonAutoHide;

	public CanvasGroup cgText;

	public RectTransform inputRect;

	public Extra extra => base.config.extra as Extra;

	public override bool AlwaysBottom => true;

	public override Type SetSiblingAfter => typeof(WidgetSideScreen);

	public override object CreateExtra()
	{
		return new Extra();
	}

	public override void OnActivate()
	{
		input.text = ((id == 0) ? EMono.player.memo : EMono.player.memo2);
		buttonEdit.SetOnClick(delegate
		{
			SE.Tab();
			ToggleInput(!input.isFocused);
		});
		Instance = this;
		buttonAutoHide.icon.SetAlpha(extra.autoHide ? 0.4f : 1f);
		if (extra.autoHide)
		{
			cgText.alpha = 0f;
		}
	}

	public override void OnDeactivate()
	{
		SaveText();
	}

	public void ToggleInput(bool enable)
	{
		input.interactable = enable;
		bgInput.enabled = enable;
		textInput.raycastTarget = enable;
		buttonClose.SetActive(enable);
		if (enable)
		{
			input.Select();
		}
	}

	public override void OnUpdateConfig()
	{
		SaveText();
	}

	public void SaveText()
	{
		if (id == 0)
		{
			EMono.player.memo = input.text;
		}
		else
		{
			EMono.player.memo2 = input.text;
		}
	}

	public void ToggleAutoHide()
	{
		SE.Tab();
		extra.autoHide = !extra.autoHide;
		hideTimer = 0f;
		cgText.alpha = 1f;
		buttonAutoHide.icon.SetAlpha(extra.autoHide ? 0.4f : 1f);
	}

	private void Update()
	{
		if (!input.isFocused)
		{
			if (input.interactable && !InputModuleEX.IsPointerChildOf(this))
			{
				ToggleInput(enable: false);
			}
		}
		else if (!bgInput.enabled)
		{
			ToggleInput(enable: true);
		}
		if (extra.autoHide)
		{
			bool flag = InputModuleEX.IsPointerOver(this) || input.isFocused;
			if (!flag && cgText.alpha != 0f)
			{
				Vector3 point = inputRect.InverseTransformPoint(Input.mousePosition);
				if (inputRect.rect.Contains(point))
				{
					flag = true;
				}
			}
			if (flag)
			{
				hideTimer = 0f;
				cgText.alpha = 1f;
				return;
			}
			hideTimer += Core.delta;
			if (hideTimer > 1f)
			{
				cgText.alpha = 0f;
			}
		}
		else
		{
			cgText.alpha = 1f;
			hideTimer = 0f;
		}
	}
}
