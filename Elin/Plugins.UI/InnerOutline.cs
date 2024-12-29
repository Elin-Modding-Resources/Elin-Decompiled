using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/ToJ Effects/Inner Outline", 20)]
[DisallowMultipleComponent]
[RequireComponent(typeof(Text))]
public class InnerOutline : BaseMeshEffect, IMaterialModifier
{
	public enum ColorMode
	{
		Override,
		Additive,
		Multiply
	}

	[SerializeField]
	private ColorMode m_ColorMode;

	[SerializeField]
	public Color m_OutlineColor = Color.black;

	[SerializeField]
	private float m_OutlineThickness = 1f;

	private bool m_NeedsToSetMaterialDirty;

	private Material m_ModifiedMaterial;

	private List<UIVertex> m_Verts = new List<UIVertex>();

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

	public Color outlineColor
	{
		get
		{
			return m_OutlineColor;
		}
		set
		{
			m_OutlineColor = value;
			if (base.graphic != null)
			{
				base.graphic.SetVerticesDirty();
			}
		}
	}

	public float outlineThickness
	{
		get
		{
			return m_OutlineThickness;
		}
		set
		{
			m_OutlineThickness = value;
			if (base.graphic != null)
			{
				base.graphic.SetVerticesDirty();
			}
		}
	}

	protected InnerOutline()
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
		for (int i = 0; i < count; i++)
		{
			UIVertex value = m_Verts[i];
			if (value.uv1 == Vector4.zero)
			{
				value.uv1 = new Vector2(1f, 1f);
			}
			m_Verts[i] = value;
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
			Debug.Log("\"" + base.gameObject.name + "\" doesn't have the \"Fancy Text\" shader applied. Please use it if you are using the \"Inner Outline\" effect.");
			return baseMaterial;
		}
		if (m_ModifiedMaterial == null)
		{
			m_ModifiedMaterial = new Material(baseMaterial);
		}
		m_ModifiedMaterial.CopyPropertiesFromMaterial(baseMaterial);
		m_ModifiedMaterial.name = baseMaterial.name + " with IO";
		m_ModifiedMaterial.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
		m_ModifiedMaterial.shaderKeywords = baseMaterial.shaderKeywords;
		m_ModifiedMaterial.CopyPropertiesFromMaterial(baseMaterial);
		m_ModifiedMaterial.EnableKeyword("_USEOUTLINE_ON");
		m_ModifiedMaterial.SetColor("_OutlineColor", outlineColor);
		m_ModifiedMaterial.SetFloat("_OutlineThickness", outlineThickness / 250f);
		m_ModifiedMaterial.SetInt("_OutlineColorMode", (int)colorMode);
		m_NeedsToSetMaterialDirty = true;
		return m_ModifiedMaterial;
	}
}
