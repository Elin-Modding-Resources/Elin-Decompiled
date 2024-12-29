using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/ToJ Effects/Outer Bevel", 4)]
[RequireComponent(typeof(Text))]
public class Bevel : BaseMeshEffect
{
	[SerializeField]
	private Color m_HighlightColor = new Color(1f, 1f, 1f, 1f);

	[SerializeField]
	private Color m_ShadowColor = new Color(0f, 0f, 0f, 1f);

	[SerializeField]
	private Vector2 m_BevelDirectionAndDepth = new Vector2(1f, 1f);

	[SerializeField]
	private bool m_UseGraphicAlpha = true;

	private List<UIVertex> m_Verts = new List<UIVertex>();

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

	public bool useGraphicAlpha
	{
		get
		{
			return m_UseGraphicAlpha;
		}
		set
		{
			m_UseGraphicAlpha = value;
			if (base.graphic != null)
			{
				base.graphic.SetVerticesDirty();
			}
		}
	}

	protected Bevel()
	{
	}

	protected void ApplyShadowZeroAlloc(List<UIVertex> verts, Color32 color, int start, int end, float x, float y)
	{
		for (int i = start; i < end; i++)
		{
			UIVertex uIVertex = verts[i];
			verts.Add(uIVertex);
			Vector3 position = uIVertex.position;
			position.x += x;
			position.y += y;
			uIVertex.position = position;
			Color32 color2 = color;
			if (useGraphicAlpha)
			{
				color2.a = (byte)(color2.a * verts[i].color.a / 255);
			}
			uIVertex.color = color2;
			verts[i] = uIVertex;
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
		int num = 0;
		num = 0;
		int count2 = m_Verts.Count;
		ApplyShadowZeroAlloc(m_Verts, shadowColor, num, m_Verts.Count, bevelDirectionAndDepth.x * 0.75f, (0f - bevelDirectionAndDepth.y) * 0.75f);
		num = count2;
		int count3 = m_Verts.Count;
		ApplyShadowZeroAlloc(m_Verts, shadowColor, num, m_Verts.Count, bevelDirectionAndDepth.x, bevelDirectionAndDepth.y * 0.5f);
		num = count3;
		int count4 = m_Verts.Count;
		ApplyShadowZeroAlloc(m_Verts, shadowColor, num, m_Verts.Count, (0f - bevelDirectionAndDepth.x) * 0.5f, 0f - bevelDirectionAndDepth.y);
		num = count4;
		int count5 = m_Verts.Count;
		ApplyShadowZeroAlloc(m_Verts, highlightColor, num, m_Verts.Count, 0f - bevelDirectionAndDepth.x, bevelDirectionAndDepth.y * 0.5f);
		num = count5;
		_ = m_Verts.Count;
		ApplyShadowZeroAlloc(m_Verts, highlightColor, num, m_Verts.Count, (0f - bevelDirectionAndDepth.x) * 0.5f, bevelDirectionAndDepth.y);
		if (GetComponent<Text>().material.shader == Shader.Find("Text Effects/Fancy Text"))
		{
			for (int i = 0; i < m_Verts.Count - count; i++)
			{
				UIVertex value = m_Verts[i];
				value.uv1 = new Vector2(0f, 0f);
				m_Verts[i] = value;
			}
		}
		vh.Clear();
		vh.AddUIVertexTriangleStream(m_Verts);
	}
}
