using System;
using Empyrean.ColorPicker;
using UnityEngine;

public class LayerColorPicker : ELayer
{
	public UIColorPicker uiPicker;

	public ColorPicker picker => uiPicker.picker;

	public Action<PickerState, Color> onChangeColor => uiPicker.onChangeColor;

	public Color resetColor => uiPicker.resetColor;

	public Color startColor => uiPicker.startColor;

	public void SetColor(Color _startColor, Color _resetColor, Action<PickerState, Color> _onChangeColor)
	{
		uiPicker.SetColor(_startColor, _resetColor, _onChangeColor);
	}

	public void OnClickConfirm()
	{
		onChangeColor(PickerState.Confirm, picker.SelectedColor);
		Close();
	}

	public void OnClickCancel()
	{
		onChangeColor(PickerState.Cancel, startColor);
		Close();
	}

	public void OnClickReset()
	{
		picker.SelectColor(resetColor);
		onChangeColor(PickerState.Reset, resetColor);
	}

	public override bool OnBack()
	{
		if (picker.dropper.coroutine != null)
		{
			picker.dropper.Stop();
			picker.dropper.onDropCanceled();
			return false;
		}
		onChangeColor(PickerState.Cancel, startColor);
		return base.OnBack();
	}

	public override void OnKill()
	{
		if (picker.dropper.coroutine != null)
		{
			picker.dropper.Stop();
			picker.dropper.onDropCanceled();
		}
		base.OnKill();
		EInput.Consume();
	}
}
