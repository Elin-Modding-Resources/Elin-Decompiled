using Empyrean.Utils;
using UnityEngine;
using UnityEngine.UI;

public class SelectedColorController : MonoBehaviour
{
	[SerializeField]
	private Image colorImage;

	[SerializeField]
	private Color rgba;

	[SerializeField]
	private HSVColor hsv;

	[SerializeField]
	private Slider slider;

	public Color RGBA
	{
		get
		{
			return rgba;
		}
		set
		{
			Select(value);
		}
	}

	public HSVColor HSV
	{
		get
		{
			return hsv;
		}
		set
		{
			Select(value);
		}
	}

	public void Select(Color rgba)
	{
		this.rgba = rgba;
		hsv = Colorist.RGBtoHSV(rgba);
		SetupComponents(rgba);
	}

	public void Select(HSVColor hsv)
	{
		rgba = Colorist.HSVtoRGB(hsv);
		this.hsv = hsv;
		SetupComponents(rgba);
	}

	private void SetupComponents(Color rgba)
	{
		slider.value = rgba.a;
		colorImage.color = new Color(rgba.r, rgba.g, rgba.b, 1f);
	}
}
