using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace Empyrean.ColorPicker;

public class LabeledColorSlider : MonoBehaviour
{
	[SerializeField]
	private EventAwareSlider slider;

	[SerializeField]
	private UiInputField input;

	[SerializeField]
	private Text label;

	[SerializeField]
	private SliderBackgroundController bgController;

	private bool useSecondaryMode;

	private float sliderValueBackup;

	[SerializeField]
	private SliderType sliderType;

	[SerializeField]
	private SliderType secondarySliderType;

	internal SliderType Type
	{
		get
		{
			if (!useSecondaryMode)
			{
				return sliderType;
			}
			return secondarySliderType;
		}
	}

	public float Value
	{
		get
		{
			return slider.value;
		}
		set
		{
			slider.SetValue(value, sendEvent: false);
			UpdateInputLabel(value);
		}
	}

	public event Action<float> ColorValueChanged = delegate
	{
	};

	private void Awake()
	{
		slider.onValueChanged.AddListener(UpdateInputLabel);
		slider.onValueChanged.AddListener(OnValueChanged);
		bgController.Init(this);
		input.InputSelected += OnInputSelected;
		UiInputField uiInputField = input;
		uiInputField.onValidateInput = (InputField.OnValidateInput)Delegate.Combine(uiInputField.onValidateInput, new InputField.OnValidateInput(OnValidateInput));
		input.onValueChanged.AddListener(OnInputValueChanged);
		input.onEndEdit.AddListener(OnInputEndEdit);
	}

	private char OnValidateInput(string text, int charIndex, char addedChar)
	{
		if (!Regex.IsMatch(addedChar.ToString(), "[0-9]"))
		{
			return '\0';
		}
		return addedChar;
	}

	public void SetSliderValueAndSendEvent(float value)
	{
		slider.SetValue(value, sendEvent: true);
	}

	private void OnValueChanged(float value)
	{
		this.ColorValueChanged(value);
	}

	private void UpdateInputLabel(float value)
	{
		input.text = ((int)(value * (float)GetMaxValue())).ToString();
	}

	private void OnInputSelected()
	{
		sliderValueBackup = slider.value;
	}

	private void OnInputValueChanged(string stringValue)
	{
		if (input.isFocused)
		{
			int value = 0;
			if (stringValue != "")
			{
				value = int.Parse(stringValue);
			}
			value = Mathf.Clamp(value, 0, GetMaxValue());
			slider.value = (float)value / (float)GetMaxValue();
			input.MoveTextEnd(shift: false);
			this.ColorValueChanged(slider.value);
		}
	}

	private void OnInputEndEdit(string stringValue)
	{
		if (string.IsNullOrEmpty(stringValue))
		{
			input.text = "0";
		}
		if (Input.GetKey(KeyCode.Escape))
		{
			RestoreOldValue();
		}
	}

	private void RestoreOldValue()
	{
		if (!Mathf.Approximately(slider.value, sliderValueBackup))
		{
			Value = sliderValueBackup;
		}
		input.text = ((int)(Value * 255f)).ToString();
		this.ColorValueChanged(slider.value);
	}

	public void ToggleSliderMode()
	{
		useSecondaryMode = !useSecondaryMode;
		bgController.ToggleSliderMode();
		label.text = Type.ToString().Substring(0, 1);
	}

	private int GetMaxValue()
	{
		if (Type != SliderType.Hue)
		{
			return 255;
		}
		return 360;
	}
}
