using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/ToJ Effects/Overlay Texture", 18)]
[DisallowMultipleComponent]
[RequireComponent(typeof(Text))]
public class OverlayTexture : BaseMeshEffect, IMaterialModifier
{
	public enum TextureMode
	{
		Local,
		GlobalTextArea,
		GlobalFullRect
	}

	public enum ColorMode
	{
		Override,
		Additive,
		Multiply
	}

	[SerializeField]
	private TextureMode m_TextureMode;

	[SerializeField]
	private ColorMode m_ColorMode;

	[SerializeField]
	public Texture2D m_OverlayTexture;

	private bool m_NeedsToSetMaterialDirty;

	private Material m_ModifiedMaterial;

	private List<UIVertex> m_Verts = new List<UIVertex>();

	public TextureMode textureMode
	{
		get
		{
			return m_TextureMode;
		}
		set
		{
			m_TextureMode = value;
			if (base.graphic != null)
			{
				base.graphic.SetVerticesDirty();
			}
		}
	}

	public ColorMode colorMode
	{
		get
		{
			return m_ColorMode;
		}
		set
		{
			m_ColorMode = value;
			if (base.graphic != null)
			{
				base.graphic.SetVerticesDirty();
			}
		}
	}

	public Texture2D overlayTexture
	{
		get
		{
			return m_OverlayTexture;
		}
		set
		{
			m_OverlayTexture = value;
			if (base.graphic != null)
			{
				base.graphic.SetVerticesDirty();
			}
		}
	}

	protected OverlayTexture()
	{
	}

	protected override void Start()
	{
		if (base.graphic != null)
		{
			base.graphic.SetMaterialDirty();
		}
	}

	public override void ModifyMesh(VertexHelper vh)
	{
		if (!IsActive())
		{
			return;
		}
		vh.GetUIVertexStream(m_Verts);
		int count = m_Verts.Count;
		if (m_Verts.Count == 0)
		{
			return;
		}
		if (textureMode == TextureMode.GlobalTextArea || textureMode == TextureMode.GlobalFullRect)
		{
			Vector2 zero = Vector2.zero;
			Vector2 zero2 = Vector2.zero;
			if (textureMode == TextureMode.GlobalFullRect)
			{
				Rect rect = GetComponent<RectTransform>().rect;
				zero = new Vector2(rect.xMin, rect.yMax);
				zero2 = new Vector2(rect.xMax, rect.yMin);
			}
			else
			{
				zero = m_Verts[0].position;
				zero2 = m_Verts[m_Verts.Count - 1].position;
				for (int i = 0; i < m_Verts.Count; i++)
				{
					if (m_Verts[i].position.x < zero.x)
					{
						zero.x = m_Verts[i].position.x;
					}
					if (m_Verts[i].position.y > zero.y)
					{
						zero.y = m_Verts[i].position.y;
					}
					if (m_Verts[i].position.x > zero2.x)
					{
						zero2.x = m_Verts[i].position.x;
					}
					if (m_Verts[i].position.y < zero2.y)
					{
						zero2.y = m_Verts[i].position.y;
					}
				}
			}
			float num = zero.y - zero2.y;
			float num2 = zero2.x - zero.x;
			for (int j = 0; j < count; j++)
			{
				UIVertex value = m_Verts[j];
				value.uv1 = new Vector2(1f + (value.position.x - zero.x) / num2, 2f - (zero.y - value.position.y) / num);
				m_Verts[j] = value;
			}
		}
		else
		{
			for (int k = 0; k < count; k++)
			{
				UIVertex value = m_Verts[k];
				value.uv1 = new Vector2(1 + ((k % 6 != 0 && k % 6 != 5 && k % 6 != 4) ? 1 : 0), 1 + ((k % 6 != 2 && k % 6 != 3 && k % 6 != 4) ? 1 : 0));
				m_Verts[k] = value;
			}
		}
		vh.Clear();
		vh.AddUIVertexTriangleStream(m_Verts);
	}

	private void Update()
	{
		if (m_NeedsToSetMaterialDirty && base.graphic != null)
		{
			base.graphic.SetMaterialDirty();
		}
	}

	public virtual Material GetModifiedMaterial(Material baseMaterial)
	{
		if (!IsActive())
		{
			return baseMaterial;
		}
		if (baseMaterial.shader != Shader.Find("Text Effects/Fancy Text"))
		{
			Debug.Log("\"" + base.gameObject.name + "\" doesn't have the \"Fancy Text\" shader applied. Please use it if you are using the \"Overlay Texture\" effect.");
			return baseMaterial;
		}
		if (m_ModifiedMaterial == null)
		{
			m_ModifiedMaterial = new Material(baseMaterial);
		}
		m_ModifiedMaterial.CopyPropertiesFromMaterial(baseMaterial);
		m_ModifiedMaterial.name = baseMaterial.name + " with OT";
		m_ModifiedMaterial.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
		m_ModifiedMaterial.shaderKeywords = baseMaterial.shaderKeywords;
		m_ModifiedMaterial.CopyPropertiesFromMaterial(baseMaterial);
		m_ModifiedMaterial.EnableKeyword("_USEOVERLAYTEXTURE_ON");
		m_ModifiedMaterial.SetTexture("_OverlayTex", overlayTexture);
		m_ModifiedMaterial.SetInt("_OverlayTextureColorMode", (int)colorMode);
		m_NeedsToSetMaterialDirty = true;
		return m_ModifiedMaterial;
	}
}
