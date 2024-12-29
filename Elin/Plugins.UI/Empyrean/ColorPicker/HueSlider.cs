using System;
using Empyrean.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Empyrean.ColorPicker;

public class HueSlider : MonoBehaviour
{
	[SerializeField]
	public EventAwareSlider slider;

	[SerializeField]
	private RawImage hueSliderBackground;

	public float value
	{
		get
		{
			return slider.value;
		}
		set
		{
			slider.SetValue(value, sendEvent: false);
		}
	}

	public event Action<float> HueValueChanged = delegate
	{
	};

	public void Init()
	{
		slider.onValueChanged.AddListener(OnValueChanged);
		GenerateSliderTexture();
	}

	public void OnValueChanged(float value)
	{
		this.HueValueChanged(value);
	}

	private void GenerateSliderTexture()
	{
		Color[] array = GenerateHsvSpectrum();
		Texture2D texture2D = new Texture2D(1, array.Length);
		for (int i = 0; i < texture2D.height; i++)
		{
			texture2D.SetPixel(1, i, array[i]);
		}
		texture2D.Apply();
		hueSliderBackground.texture = texture2D;
	}

	public static Color[] GenerateHsvSpectrum()
	{
		Color[] array = new Color[360];
		for (int i = 0; i < 360; i++)
		{
			array[i] = Colorist.HSVtoRGB(i, 1f, 1f);
		}
		return array;
	}

	public void SetSliderValueAndSendEvent(float value)
	{
		slider.SetValue(value, sendEvent: true);
	}
}
