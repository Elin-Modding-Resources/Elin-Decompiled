using UnityEngine;
using UnityEngine.UI;

namespace Applibot;

public class OutlineImage : CustomImageBase
{
	[ColorUsage(true, true)]
	[SerializeField]
	private Color _outlineColor = Color.white;

	private Image _image;

	[SerializeField]
	private bool _isStatic;

	private int _OutlineColor = Shader.PropertyToID("_OutlineColor");

	private readonly int _SrcFactor = Shader.PropertyToID("_SrcFactor");

	private readonly int _DstFactor = Shader.PropertyToID("_DstFactor");

	protected override void UpdateMaterial(Material baseMaterial)
	{
		if (material == null)
		{
			Shader shader = Shader.Find("Applibot/UI/Outline");
			material = new Material(shader);
		}
		material.SetColor(_OutlineColor, _outlineColor);
		material.SetInt(_SrcFactor, 5);
		material.SetInt(_DstFactor, 10);
		if (base.canvasScaler != null)
		{
			Vector2 referenceResolution = base.canvasScaler.referenceResolution;
			Vector2 one = Vector2.one;
			if (_image != null && _image.sprite.packed)
			{
				Rect textureRect = _image.sprite.textureRect;
				one = new Vector2(textureRect.width, textureRect.height);
			}
			else
			{
				Texture mainTexture = base.graphic.mainTexture;
				one = new Vector2(mainTexture.width, mainTexture.height);
			}
			float x = one.x / referenceResolution.x;
			float y = one.y / referenceResolution.y;
			material.SetVector("_scaleFactor", new Vector4(x, y));
		}
	}

	private void Awake()
	{
		_image = base.graphic as Image;
		if (Application.isPlaying && _isStatic)
		{
			Capture();
		}
	}

	public void Capture()
	{
		UpdateMaterial(null);
		material.SetInt(_SrcFactor, 1);
		material.SetInt(_DstFactor, 0);
		if (TryGetComponent<RawImage>(out var component))
		{
			Texture mainTexture = base.graphic.mainTexture;
			float width = (base.transform as RectTransform).rect.width;
			float height = (base.transform as RectTransform).rect.height;
			RenderTexture renderTexture = new RenderTexture((int)width, (int)height, 0, RenderTextureFormat.ARGBHalf);
			Graphics.Blit(mainTexture, renderTexture, material);
			component.texture = renderTexture;
			DestroyMaterial();
			base.enabled = false;
		}
	}
}
