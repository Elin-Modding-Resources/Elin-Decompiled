using System;
using UnityEngine.UI;

public class WidgetMemo : Widget
{
	public static WidgetMemo Instance;

	public int id;

	public InputField input;

	public Window window;

	public Image bgInput;

	public Text textInput;

	public UIButton buttonClose;

	public UIButton buttonEdit;

	public override bool AlwaysBottom => true;

	public override Type SetSiblingAfter => typeof(WidgetSideScreen);

	public override void OnActivate()
	{
		input.text = ((id == 0) ? EMono.player.memo : EMono.player.memo2);
		buttonEdit.SetOnClick(delegate
		{
			ToggleInput(!input.isFocused);
		});
		Instance = this;
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
	}
}
