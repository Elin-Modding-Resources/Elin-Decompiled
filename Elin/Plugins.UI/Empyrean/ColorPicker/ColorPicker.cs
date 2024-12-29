using System;
using UnityEngine;
using UnityEngine.UI;

namespace Empyrean.ColorPicker;

public class ColorPicker : MonoBehaviour
{
	private enum ColorUpdateMode
	{
		MANUAL,
		RGBA,
		HSVA,
		HSV,
		MARKER
	}

	internal static bool slidersHsvMode;

	[Header("Components")]
	[SerializeField]
	public SelectedColorController selectedColor;

	[SerializeField]
	public ColorPalette palette;

	[SerializeField]
	public HueSlider hueSlider;

	[SerializeField]
	public LabeledColorSlider redHueSlider;

	[SerializeField]
	public LabeledColorSlider greenSatSlider;

	[SerializeField]
	public LabeledColorSlider blueValSlider;

	[SerializeField]
	private LabeledColorSlider alphaSlider;

	[SerializeField]
	private HexInput hexInput;

	[SerializeField]
	public Dropper dropper;

	[SerializeField]
	private Button dropperButton;

	[SerializeField]
	private Button slidersModeButton;

	[Header("Settings")]
	public Color defaultColor;

	[SerializeField]
	private bool useAlpha = true;

	[NonSerialized]
	public int[] colors = new int[10];

	private Color backupColor;

	public Color SelectedColor
	{
		get
		{
			return selectedColor.RGBA;
		}
		set
		{
			SelectColor(value);
		}
	}

	public float Alpha
	{
		get
		{
			if (!useAlpha)
			{
				return 1f;
			}
			return alphaSlider.Value;
		}
	}

	public float Hue => hueSlider.value;

	public float Saturation => greenSatSlider.Value;

	public float ValueHSV => blueValSlider.Value;

	public event Action<Color> ColorUpdated = delegate
	{
	};

	public void Init()
	{
		redHueSlider.ColorValueChanged += OnColorSliderValueChanged;
		blueValSlider.ColorValueChanged += OnColorSliderValueChanged;
		greenSatSlider.ColorValueChanged += OnColorSliderValueChanged;
		alphaSlider.ColorValueChanged += OnColorSliderValueChanged;
		hueSlider.Init();
		hueSlider.HueValueChanged += OnHueSliderValueChanged;
		palette.ColorPicked += OnColorPickedFromPalette;
		hexInput.ColorSelected += OnHexColorSelected;
		if (dropperButton != null)
		{
			dropperButton.onClick.AddListener(StartDropper);
		}
		slidersModeButton.onClick.AddListener(OnSlidersModeButton);
		alphaSlider.gameObject.SetActive(useAlpha);
	}

	private void OnColorSliderValueChanged(float value)
	{
		UpdateColorPicker((!slidersHsvMode) ? ColorUpdateMode.RGBA : ColorUpdateMode.HSVA);
	}

	private void OnHueSliderValueChanged(float value)
	{
		UpdateColorPicker(ColorUpdateMode.HSV);
	}

	public void OnColorPickedFromPalette(Vector2 positionOnPalette)
	{
		selectedColor.Select(new HSVColor(hueSlider.value, positionOnPalette.x, positionOnPalette.y, Alpha));
		UpdateColorPicker(ColorUpdateMode.MARKER);
	}

	private void OnHexColorSelected(Color color)
	{
		SelectColor(color);
	}

	private void UpdateColorPicker(ColorUpdateMode mode, bool sendEvent = true)
	{
		UpdateColor(mode);
		UpdateSlidersValues(mode);
		hexInput.SelectColor(selectedColor.RGBA);
		palette.SelectColor(selectedColor.HSV);
		if (sendEvent)
		{
			this.ColorUpdated(selectedColor.RGBA);
		}
	}

	private void UpdateColor(ColorUpdateMode mode)
	{
		switch (mode)
		{
		case ColorUpdateMode.HSV:
			selectedColor.Select(new HSVColor(hueSlider.value, palette.MarkerPosition.x, palette.MarkerPosition.y, Alpha));
			break;
		case ColorUpdateMode.HSVA:
			selectedColor.Select(new HSVColor(redHueSlider.Value * 359f, greenSatSlider.Value, blueValSlider.Value, Alpha));
			break;
		case ColorUpdateMode.RGBA:
			selectedColor.Select(new Color(redHueSlider.Value, greenSatSlider.Value, blueValSlider.Value, Alpha));
			break;
		}
	}

	private void UpdateSlidersValues(ColorUpdateMode mode)
	{
		if (mode != ColorUpdateMode.HSV)
		{
			hueSlider.value = selectedColor.HSV.h;
		}
		if (mode != ColorUpdateMode.RGBA && mode != ColorUpdateMode.HSVA)
		{
			if (slidersHsvMode)
			{
				redHueSlider.Value = selectedColor.HSV.h / 359f;
				greenSatSlider.Value = selectedColor.HSV.s;
				blueValSlider.Value = selectedColor.HSV.v;
			}
			else
			{
				redHueSlider.Value = selectedColor.RGBA.r;
				greenSatSlider.Value = selectedColor.RGBA.g;
				blueValSlider.Value = selectedColor.RGBA.b;
			}
			if (useAlpha)
			{
				alphaSlider.Value = selectedColor.RGBA.a;
			}
		}
	}

	private void StartDropper()
	{
		backupColor = selectedColor.RGBA;
		dropper.PickColors(OnColorPicked, OnDropperCanceled);
	}

	private void OnColorPicked(Color color)
	{
		SelectColor(color);
	}

	private void OnDropperCanceled()
	{
		SelectColor(backupColor);
	}

	private void OnSlidersModeButton()
	{
		slidersHsvMode = !slidersHsvMode;
		redHueSlider.ToggleSliderMode();
		greenSatSlider.ToggleSliderMode();
		blueValSlider.ToggleSliderMode();
		UpdateColorPicker(ColorUpdateMode.MANUAL, sendEvent: false);
	}

	public void SelectColor(Color newColor)
	{
		selectedColor.Select(newColor);
		UpdateColorPicker(ColorUpdateMode.MANUAL);
	}

	public bool IsAlphaBeingUsed()
	{
		return useAlpha;
	}
}
