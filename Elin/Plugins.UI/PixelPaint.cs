using Empyrean.ColorPicker;
using UnityEngine;
using UnityEngine.UI;

public class PixelPaint : MonoBehaviour
{
	public RawImage imageRect;

	public RawImage imageGrid;

	public RawImage imagePreview;

	public RawImage imageMask;

	public Vector2Int size;

	public int scale;

	public int brushSize;

	public int paddingBrush;

	public Texture2D tex;

	public Image imageBrush;

	public ColorPicker picker;

	public Color startColor;

	public Color bgColor;

	private bool first = true;

	private void Update()
	{
		RectTransform rectTransform = (RectTransform)imageRect.transform;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(imageRect.rectTransform, Input.mousePosition, null, out var localPoint);
		Vector2 vector = localPoint - rectTransform.rect.min;
		vector.x *= imageRect.uvRect.width / rectTransform.rect.width;
		vector.y *= imageRect.uvRect.height / rectTransform.rect.height;
		vector += imageRect.uvRect.min;
		int num = (int)(vector.x * (float)size.x);
		int num2 = (int)(vector.y * (float)size.y);
		bool flag = vector.x >= 0f && vector.y >= 0f && vector.x <= 1f && vector.y <= 1f;
		if (flag)
		{
			if (Input.GetMouseButton(0) && !first)
			{
				for (int i = num2 - brushSize + 1; i < num2 + brushSize; i++)
				{
					for (int j = num - brushSize + 1; j < num + brushSize; j++)
					{
						if (j >= 0 && i >= 0 && j < size.x && i < size.y)
						{
							tex.SetPixel(j, i, picker.SelectedColor);
						}
					}
				}
				tex.Apply();
			}
			if (Input.GetMouseButton(1))
			{
				Color pixel = tex.GetPixel(num, num2);
				picker.SelectColor(pixel);
			}
		}
		else if (Input.GetMouseButtonDown(0))
		{
			first = true;
		}
		float axis = Input.GetAxis("Mouse ScrollWheel");
		if (axis > 0f)
		{
			brushSize++;
		}
		if (axis < 0f)
		{
			brushSize--;
		}
		brushSize = Mathf.Clamp(brushSize, 1, 10);
		int num3 = ((brushSize == 1) ? 1 : (1 + (brushSize - 1) * 2));
		imageBrush.rectTransform.sizeDelta = new Vector2(num3 * scale + paddingBrush, num3 * scale + paddingBrush);
		imageBrush.rectTransform.anchoredPosition = new Vector2(scale / 2 + num * scale, -size.y * scale + num2 * scale + scale / 2);
		imageBrush.gameObject.SetActive(flag);
		if (!Input.GetMouseButton(0))
		{
			first = false;
		}
	}

	public void Init()
	{
		Vector2 sizeDelta = new Vector2(size.x * scale, size.y * scale);
		imageRect.rectTransform.sizeDelta = sizeDelta;
		imageGrid.rectTransform.sizeDelta = sizeDelta;
		imageGrid.uvRect = new Rect(0f, 0f, size.x, size.y);
		tex = new Texture2D(size.x, size.y, TextureFormat.ARGB32, mipChain: false);
		tex.filterMode = FilterMode.Point;
		imagePreview.texture = tex;
		imagePreview.rectTransform.sizeDelta = new Vector2(size.x * 2, size.y * 2);
		imageRect.texture = tex;
		Fill(bgColor);
		picker.Init();
		picker.SelectColor(startColor);
		picker.SelectColor(startColor);
	}

	public void Fill(Color color)
	{
		int num = tex.GetPixels().Length;
		Color[] array = new Color[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = color;
		}
		tex.SetPixels(array);
		tex.Apply();
	}

	private void OnDestroy()
	{
		if ((bool)tex)
		{
			Object.Destroy(tex);
			tex = null;
		}
	}
}
