using System;
using UnityEngine;
using UnityEngine.UI;

namespace Applibot;

[ExecuteAlways]
[RequireComponent(typeof(Graphic))]
public class CustomImageBase : MonoBehaviour, IMaterialModifier
{
	[NonSerialized]
	private Graphic _graphic;

	[NonSerialized]
	private Image _image;

	protected Material material;

	private CanvasScaler _canvasScaler;

	private int _textureRectId = Shader.PropertyToID("_textureRect");

	protected CanvasScaler canvasScaler
	{
		get
		{
			if (_canvasScaler == null)
			{
				_canvasScaler = graphic.canvas.GetComponent<CanvasScaler>();
			}
			return _canvasScaler;
		}
	}

	public Graphic graphic
	{
		get
		{
			if (_graphic == null)
			{
				_graphic = GetComponent<Graphic>();
			}
			return _graphic;
		}
	}

	public Material GetModifiedMaterial(Material baseMaterial)
	{
		if (!base.isActiveAndEnabled || graphic == null)
		{
			return baseMaterial;
		}
		UpdateMaterial(baseMaterial);
		SetAtlasInfo();
		return material;
	}

	private void OnDidApplyAnimationProperties()
	{
		if (base.isActiveAndEnabled && !(graphic == null))
		{
			graphic.SetMaterialDirty();
		}
	}

	protected virtual void UpdateMaterial(Material baseMaterial)
	{
	}

	private void SetAtlasInfo()
	{
		if (!(_image == null))
		{
			if (!_image.sprite.packed)
			{
				material.DisableKeyword("USE_ATLAS");
				return;
			}
			Rect textureRect = _image.sprite.textureRect;
			Vector4 value = new Vector4(textureRect.x, textureRect.y, textureRect.width, textureRect.height);
			material.SetVector(_textureRectId, value);
			material.EnableKeyword("USE_ATLAS");
		}
	}

	protected void OnEnable()
	{
		if (!(graphic == null))
		{
			_image = graphic as Image;
			graphic.SetMaterialDirty();
		}
	}

	protected void OnDisable()
	{
		if (material != null)
		{
			DestroyMaterial();
		}
		if (graphic != null)
		{
			graphic.SetMaterialDirty();
		}
	}

	protected void OnDestroy()
	{
		if (material != null)
		{
			DestroyMaterial();
		}
	}

	public void DestroyMaterial()
	{
		UnityEngine.Object.Destroy(material);
		material = null;
	}
}
