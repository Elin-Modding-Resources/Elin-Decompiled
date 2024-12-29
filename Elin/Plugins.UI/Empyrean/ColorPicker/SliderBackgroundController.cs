using System.Collections;
using Empyrean.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Empyrean.ColorPicker;

public class SliderBackgroundController : MonoBehaviour
{
	[SerializeField]
	private ColorPicker colorPicker;

	[SerializeField]
	private RawImage sliderBackground;

	private LabeledColorSlider parent;

	private Color[] colorArray;

	private void Awake()
	{
		colorPicker.ColorUpdated += UpdateBackround;
	}

	private void UpdateBackround(Color color)
	{
		GenerateSliderTexture();
	}

	private void GenerateSliderTexture()
	{
		Color[] array = ((parent.Type == SliderType.Hue) ? HueSlider.GenerateHsvSpectrum() : GenerateColorSpectrum());
		Texture2D texture2D = new Texture2D(array.Length, 1);
		for (int i = 0; i < texture2D.width; i++)
		{
			texture2D.SetPixel(i, 1, array[i]);
		}
		texture2D.Apply();
		sliderBackground.texture = texture2D;
	}

	private Color[] GenerateColorSpectrum()
	{
		Color[] array = new Color[256];
		for (int i = 0; i < 256; i++)
		{
			array[i] = GetColorByTypeAndIndex(i);
		}
		return array;
	}

	private Color GetColorByTypeAndIndex(int index)
	{
		Color selectedColor = colorPicker.SelectedColor;
		switch (parent.Type)
		{
		case SliderType.Red:
			return new Color((float)index / 255f, selectedColor.g, selectedColor.b, 1f);
		case SliderType.Green:
			return new Color(selectedColor.r, (float)index / 255f, selectedColor.b, 1f);
		case SliderType.Blue:
			return new Color(selectedColor.r, selectedColor.g, (float)index / 255f, 1f);
		case SliderType.Sat:
		{
			float v = ((colorPicker.ValueHSV >= 0.3f) ? colorPicker.ValueHSV : 0.3f);
			return new HSVColor(colorPicker.Hue, (float)index / 255f, v).ToRGB();
		}
		case SliderType.Val:
			return new HSVColor(colorPicker.Hue, colorPicker.Saturation, (float)index / 255f).ToRGB();
		case SliderType.Alpha:
			return new Color(1f, 1f, 1f, (float)index / 255f);
		default:
			return Color.red;
		}
	}

	public void Init(LabeledColorSlider parent)
	{
		this.parent = parent;
	}

	public void ToggleSliderMode()
	{
		StartCoroutine(DelayedUpdateBg());
	}

	private IEnumerator DelayedUpdateBg()
	{
		yield return null;
		UpdateBackround(colorPicker.SelectedColor);
	}
}
