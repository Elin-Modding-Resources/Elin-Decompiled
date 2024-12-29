using System;
using Empyrean.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Empyrean.ColorPicker;

public class ColorPalette : MonoBehaviour, IDragHandler, IEventSystemHandler, IPointerDownHandler, IInitializePotentialDragHandler
{
	private HSVColor current;

	private Color[] colorArray;

	private Texture2D texture;

	[SerializeField]
	private RawImage paletteImage;

	[SerializeField]
	private RectTransform marker;

	[SerializeField]
	private RectTransform dropperSelectionMarker;

	private bool initiated;

	public Vector2 MarkerPosition => marker.anchorMin;

	public event Action<Vector2> ColorPicked = delegate
	{
	};

	private void Awake()
	{
		Init();
	}

	private void Init()
	{
		if (!initiated)
		{
			initiated = true;
			int width = paletteImage.mainTexture.width;
			int height = paletteImage.mainTexture.height;
			texture = new Texture2D(width, height, TextureFormat.RGB24, mipChain: false);
			colorArray = new Color[width * height];
		}
	}

	public void SelectColor(HSVColor hsv)
	{
		Init();
		if (Mathf.Approximately(current.h, current.h))
		{
			GeneratePaletteTexture(hsv);
		}
		UpdateMarkerPosition(hsv);
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		OnDrag(eventData);
	}

	public void OnDrag(PointerEventData eventData)
	{
		this.ColorPicked(GetNormalisedPointerPosition(eventData.position));
	}

	private Vector2 GetNormalisedPointerPosition(Vector2 position)
	{
		Vector2 clampedLocalPosition = GetClampedLocalPosition(position);
		return new Vector2(clampedLocalPosition.x / paletteImage.rectTransform.rect.width, clampedLocalPosition.y / paletteImage.rectTransform.rect.height);
	}

	public void OnInitializePotentialDrag(PointerEventData ped)
	{
		ped.useDragThreshold = false;
	}

	private Vector2 GetClampedLocalPosition(Vector2 position)
	{
		Vector3 vector = paletteImage.gameObject.transform.InverseTransformPoint(position);
		vector.x = Mathf.Clamp(vector.x, 0f, paletteImage.rectTransform.rect.width);
		vector.y = Mathf.Clamp(vector.y, 0f, paletteImage.rectTransform.rect.height);
		return vector;
	}

	private void GeneratePaletteTexture(HSVColor hsv)
	{
		current = hsv;
		int width = texture.width;
		for (int i = 0; i < width; i++)
		{
			for (int num = width - 1; num > 0; num--)
			{
				colorArray[i * width - 1 + num] = Colorist.HSVtoRGB(current.h, (float)num / (float)width, (float)i / (float)width);
			}
		}
		texture.SetPixels(colorArray);
		texture.Apply();
		paletteImage.texture = texture;
	}

	public void UpdateMarkerPosition(HSVColor hsv)
	{
		SetMarkerPosition(new Vector2(hsv.s, hsv.v));
	}

	private void SetMarkerPosition(Vector2 position)
	{
		marker.anchorMin = position;
		marker.anchorMax = position;
	}

	public void ShowEnlargedPixels(Color[] colors)
	{
		Color[] array = new Color[texture.width * texture.height];
		for (int i = 0; i < texture.width; i++)
		{
			for (int j = 0; j < texture.width; j++)
			{
				int num = i * texture.width + j;
				int num2 = i / 32 * 16 + j / 32;
				array[num] = colors[num2];
			}
		}
		texture.SetPixels(array);
		texture.Apply();
		paletteImage.texture = texture;
	}

	public void ShowDropperMarker(bool value)
	{
		marker.gameObject.SetActive(!value);
		dropperSelectionMarker.gameObject.SetActive(value);
	}
}
