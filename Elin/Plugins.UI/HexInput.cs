using System;
using System.Text.RegularExpressions;
using Empyrean.ColorPicker;
using Empyrean.Utils;
using UnityEngine;
using UnityEngine.UI;

public class HexInput : MonoBehaviour
{
	[SerializeField]
	private ColorPicker colorPicker;

	[SerializeField]
	private UiInputField input;

	private string hexBackup;

	private string currentHex;

	public event Action<Color> ColorSelected = delegate
	{
	};

	private void Awake()
	{
		input.InputSelected += BackupColor;
		input.onEndEdit.AddListener(OnInputEndEdit);
		input.onValueChanged.AddListener(OnInputValueChanged);
		UiInputField uiInputField = input;
		uiInputField.onValidateInput = (InputField.OnValidateInput)Delegate.Combine(uiInputField.onValidateInput, new InputField.OnValidateInput(OnValidateInput));
		input.characterLimit = (colorPicker.IsAlphaBeingUsed() ? 8 : 6);
	}

	private char OnValidateInput(string text, int charIndex, char addedChar)
	{
		if (!Regex.IsMatch(addedChar.ToString(), "[A-Fa-f0-9]"))
		{
			return '\0';
		}
		return addedChar;
	}

	private void BackupColor()
	{
		hexBackup = input.text;
	}

	private void OnInputValueChanged(string value)
	{
		if (input.isFocused)
		{
			switch (value.Length)
			{
			default:
				return;
			case 0:
			case 1:
			case 2:
			case 5:
			case 7:
				currentHex = hexBackup;
				return;
			case 3:
				currentHex = string.Concat(value[0], value[0], value[1], value[1], value[2], value[2], GetAlpha("FF"));
				break;
			case 4:
				currentHex = string.Concat(value[0], value[0], value[1], value[1], value[2], value[2], GetAlpha(string.Concat(value[3], value[3])));
				break;
			case 6:
				currentHex = value + GetAlpha("FF");
				break;
			case 8:
				currentHex = value;
				break;
			}
			this.ColorSelected(GetColorByHex(currentHex));
		}
	}

	private string GetAlpha(string alpha)
	{
		if (!colorPicker.IsAlphaBeingUsed())
		{
			return "";
		}
		return alpha;
	}

	private void OnInputEndEdit(string value)
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			currentHex = hexBackup;
		}
		input.text = currentHex;
		this.ColorSelected(GetColorByHex(currentHex));
	}

	private Color GetColorByHex(string hex)
	{
		return Colorist.HexToColor(hex);
	}

	public void SelectColor(Color color)
	{
		if (!input.isFocused)
		{
			string text = (currentHex = Colorist.ColorToHex(color));
			input.text = text;
		}
	}
}
