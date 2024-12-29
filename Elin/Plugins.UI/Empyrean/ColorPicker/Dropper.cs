using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Empyrean.ColorPicker;

public class Dropper : MonoBehaviour
{
	[SerializeField]
	private ColorPalette palette;

	[SerializeField]
	private Button blockerPrefab;

	public const int BlockWidth = 16;

	public const int BlockHeight = 16;

	private Button blocker;

	private RenderTexture renderTexture;

	private Texture2D texture2D;

	private Texture2D pixelBlock;

	public Coroutine coroutine;

	private Color color;

	private Color[] black16x16 = new Color[256];

	public Action onDropCanceled;

	private Action<Color> onColorPicked;

	private void Awake()
	{
		Camera.main.targetTexture = renderTexture;
		renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
		texture2D = new Texture2D(Screen.width, Screen.height, TextureFormat.ARGB32, mipChain: false);
		pixelBlock = new Texture2D(16, 16, TextureFormat.ARGB32, mipChain: false);
		for (int i = 0; i < black16x16.Length; i++)
		{
			black16x16[i] = Color.black;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		UpdateRenderTextureIfNecessary();
	}

	private void UpdateRenderTextureIfNecessary()
	{
		if (renderTexture.width != Screen.width || renderTexture.height != Screen.height)
		{
			renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
		}
	}

	private IEnumerator EnlargeAreaAroundPointer()
	{
		while (true)
		{
			yield return new WaitForEndOfFrame();
			UpdateTexture();
			UpdateColorByMousePosition();
			int num = (int)Mathf.Clamp(Input.mousePosition.x, 0f, texture2D.width);
			int num2 = (int)Mathf.Clamp(Input.mousePosition.y, 0f, texture2D.height);
			Vector2 vector = new Vector2(num - 8, num2 - 8);
			Vector2 vector2 = new Vector2(num + 8, num2 + 8);
			Vector2 vector3 = new Vector2(Mathf.Clamp(vector.x, 0f, texture2D.width - 8), Mathf.Clamp(vector.y, 0f, texture2D.height - 8));
			Vector2 vector4 = new Vector2(Mathf.Clamp(vector2.x, 8f, texture2D.width), Mathf.Clamp(vector2.y, 8f, texture2D.height));
			int blockWidth = (int)vector4.x - (int)vector3.x;
			int blockHeight = (int)vector4.y - (int)vector3.y;
			Color[] pixels = texture2D.GetPixels((int)vector3.x, (int)vector3.y, blockWidth, blockHeight);
			float num3 = vector3.x - vector.x;
			float num4 = vector3.y - vector.y;
			pixelBlock.SetPixels(0, 0, 16, 16, black16x16);
			pixelBlock.SetPixels((int)num3, (int)num4, blockWidth, blockHeight, pixels);
			palette.ShowEnlargedPixels(pixelBlock.GetPixels());
		}
	}

	private void UpdateTexture()
	{
		texture2D.ReadPixels(new Rect(0f, 0f, renderTexture.width, renderTexture.height), 0, 0);
		texture2D.Apply();
	}

	private void UpdateColorByMousePosition()
	{
		color = texture2D.GetPixel((int)EInput.uiMousePosition.x, (int)EInput.uiMousePosition.y);
	}

	public void PickColors(Action<Color> onColorPicked, Action onDropCanceled)
	{
		this.onDropCanceled = onDropCanceled;
		this.onColorPicked = onColorPicked;
		palette.ShowDropperMarker(value: true);
		blocker = UnityEngine.Object.Instantiate(blockerPrefab);
		blocker.onClick.AddListener(OnBlockerClicked);
		coroutine = blocker.StartCoroutine(EnlargeAreaAroundPointer());
	}

	private void OnBlockerClicked()
	{
		Stop();
		onColorPicked(color);
	}

	public void Stop()
	{
		palette.ShowDropperMarker(value: false);
		StopCoroutine(coroutine);
		coroutine = null;
		UnityEngine.Object.Destroy(blocker.gameObject);
		blocker = null;
	}
}
