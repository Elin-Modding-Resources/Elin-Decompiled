using System;
using Empyrean.ColorPicker;
using UnityEngine;
using UnityEngine.UI;

public class UIColorPicker : EMono
{
	public ColorPicker picker;

	public Action<PickerState, Color> onChangeColor;

	public Color resetColor;

	public Color startColor;

	public GridLayoutGroup layoutColors;

	public void SetColor(Color _startColor, Color _resetColor, Action<PickerState, Color> _onChangeColor)
	{
		UIItem t = layoutColors.CreateMold<UIItem>();
		for (int i = 0; i < 8; i++)
		{
			UIItem item = Util.Instantiate(t, layoutColors);
			int _i = i;
			item.button1.icon.color = IntColor.FromInt(EMono.core.config.colors[_i]);
			item.button1.SetOnClick(delegate
			{
				picker.SelectColor(item.button1.icon.color);
			});
			item.button2.SetOnClick(delegate
			{
				item.button1.icon.color = picker.SelectedColor;
				EMono.core.config.colors[_i] = IntColor.ToInt(picker.SelectedColor);
				SE.Tab();
			});
		}
		layoutColors.RebuildLayout();
		picker.ColorUpdated += delegate(Color c)
		{
			_onChangeColor(PickerState.Modify, c);
		};
		startColor = _startColor;
		resetColor = _resetColor;
		picker.Init();
		picker.SelectColor(_startColor);
		picker.SelectColor(_startColor);
		onChangeColor = _onChangeColor;
	}
}
