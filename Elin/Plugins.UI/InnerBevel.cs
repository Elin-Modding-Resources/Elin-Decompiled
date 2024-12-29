using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/ToJ Effects/Inner Bevel", 19)]
[DisallowMultipleComponent]
[RequireComponent(typeof(Text))]
public class InnerBevel : BaseMeshEffect, IMaterialModifier
{
	public enum ColorMode
	{
		Override,
		Additive,
		Multiply
	}

	[SerializeField]
	private ColorMode m_HighlightColorMode;

	[SerializeField]
	public Color m_HighlightColor = Color.white;

	[SerializeField]
	private ColorMode m_ShadowColorMode;

	[SerializeField]
	public Color m_ShadowColor = Color.black;

	[SerializeField]
	private Vector2 m_BevelDirectionAndDepth = new Vector2(1f, 1f);

	private bool m_NeedsToSetMaterialDirty;

	private Material m_ModifiedMaterial;

	private List<UIVertex> m_Verts = new List<UIVertex>();

	public ColorMode highlightColorMode
	{
		get
		{
			return m_HighlightColorMode;
		}
		set
		{
			m_HighlightColorMode = value;
			if (base.graphic != null)
			{
				base.graphic.SetVerticesDirty();
			}
		}
	}

	public Color highlightColor
	{
		get
		{
			return m_HighlightColor;
		}
		set
		{
			m_HighlightColor = value;
			if (base.graphic != null)
			{
				base.graphic.SetVerticesDirty();
			}
		}
	}

	public ColorMode shadowColorMode
	{
		get
		{
			return m_ShadowColorMode;
		}
		set
		{
			m_ShadowColorMode = value;
			if (base.graphic != null)
			{
				base.graphic.SetVerticesDirty();
			}
		}
	}

	public Color shadowColor
	{
		get
		{
			return m_ShadowColor;
		}
		set
		{
			m_ShadowColor = value;
			if (base.graphic != null)
			{
				base.graphic.SetVerticesDirty();
			}
		}
	}

	public Vector2 bevelDirectionAndDepth
	{
		get
		{
			return m_BevelDirectionAndDepth;
		}
		set
		{
			if (!(m_BevelDirectionAndDepth == value))
			{
				m_BevelDirectionAndDepth = value;
				if (base.graphic != null)
				{
					base.graphic.SetVerticesDirty();
				}
			}
		}
	}

	protected InnerBevel()
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
		for (int i = 0; i < count; i += 6)
		{
			UIVertex value = m_Verts[i];
			Vector2 vector = (m_Verts[i + 1].uv0 - m_Verts[i].uv0).normalized;
			Vector2 vector2 = (m_Verts[i + 1].uv0 - m_Verts[i + 2].uv0).normalized;
			Vector4 tangent = vector;
			tangent.z = vector2.x;
			tangent.w = vector2.y;
			value.tangent = tangent;
			if (value.uv1 == Vector4.zero)
			{
				value.uv1 = new Vector2(1f, 1f);
			}
			m_Verts[i] = value;
			for (int j = 1; j < 6; j++)
			{
				value = m_Verts[i + j];
				value.tangent = m_Verts[i].tangent;
				if (value.uv1 == Vector4.zero)
				{
					value.uv1 = new Vector2(1f, 1f);
				}
				m_Verts[i + j] = value;
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
			Debug.Log("\"" + base.gameObject.name + "\" doesn't have the \"Fancy Text\" shader applied. Please use it if you are using the \"Inner Bevel\" effect.");
			return baseMaterial;
		}
		if (m_ModifiedMaterial == null)
		{
			m_ModifiedMaterial = new Material(baseMaterial);
		}
		m_ModifiedMaterial.CopyPropertiesFromMaterial(baseMaterial);
		m_ModifiedMaterial.name = baseMaterial.name + " with IB";
		m_ModifiedMaterial.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
		m_ModifiedMaterial.shaderKeywords = baseMaterial.shaderKeywords;
		m_ModifiedMaterial.CopyPropertiesFromMaterial(baseMaterial);
		m_ModifiedMaterial.EnableKeyword("_USEBEVEL_ON");
		m_ModifiedMaterial.SetColor("_HighlightColor", highlightColor);
		m_ModifiedMaterial.SetColor("_ShadowColor", shadowColor);
		m_ModifiedMaterial.SetVector("_HighlightOffset", bevelDirectionAndDepth / 500f);
		m_ModifiedMaterial.SetInt("_HighlightColorMode", (int)highlightColorMode);
		m_ModifiedMaterial.SetInt("_ShadowColorMode", (int)shadowColorMode);
		m_NeedsToSetMaterialDirty = true;
		return m_ModifiedMaterial;
	}
}
