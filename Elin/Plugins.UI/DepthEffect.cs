using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/ToJ Effects/Depth Effect", 2)]
[RequireComponent(typeof(Text))]
public class DepthEffect : BaseMeshEffect
{
	[SerializeField]
	private Color m_EffectColor = new Color(0f, 0f, 0f, 1f);

	[SerializeField]
	private Vector2 m_EffectDirectionAndDepth = new Vector2(-1f, 1f);

	[SerializeField]
	private Vector2 m_DepthPerspectiveStrength = new Vector2(0f, 0f);

	[SerializeField]
	private bool m_OnlyInitialCharactersGenerateDepth = true;

	[SerializeField]
	private bool m_UseGraphicAlpha = true;

	private Vector2 m_OverallTextSize = Vector2.zero;

	private Vector2 m_TopLeftPos = Vector2.zero;

	private Vector2 m_BottomRightPos = Vector2.zero;

	private List<UIVertex> m_Verts = new List<UIVertex>();

	public Color effectColor
	{
		get
		{
			return m_EffectColor;
		}
		set
		{
			m_EffectColor = value;
			if (base.graphic != null)
			{
				base.graphic.SetVerticesDirty();
			}
		}
	}

	public Vector2 effectDirectionAndDepth
	{
		get
		{
			return m_EffectDirectionAndDepth;
		}
		set
		{
			if (!(m_EffectDirectionAndDepth == value))
			{
				m_EffectDirectionAndDepth = value;
				if (base.graphic != null)
				{
					base.graphic.SetVerticesDirty();
				}
			}
		}
	}

	public Vector2 depthPerspectiveStrength
	{
		get
		{
			return m_DepthPerspectiveStrength;
		}
		set
		{
			if (!(m_DepthPerspectiveStrength == value))
			{
				m_DepthPerspectiveStrength = value;
				if (base.graphic != null)
				{
					base.graphic.SetVerticesDirty();
				}
			}
		}
	}

	public bool onlyInitialCharactersGenerateDepth
	{
		get
		{
			return m_OnlyInitialCharactersGenerateDepth;
		}
		set
		{
			m_OnlyInitialCharactersGenerateDepth = value;
			if (base.graphic != null)
			{
				base.graphic.SetVerticesDirty();
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

	protected DepthEffect()
	{
	}

	protected void ApplyShadowZeroAlloc(List<UIVertex> verts, Color32 color, int start, int end, float x, float y, float factor)
	{
		for (int i = start; i < end; i++)
		{
			UIVertex uIVertex = verts[i];
			verts.Add(uIVertex);
			Vector3 position = uIVertex.position;
			position.x += x * factor;
			if (depthPerspectiveStrength.x != 0f)
			{
				position.x -= depthPerspectiveStrength.x * ((position.x - m_TopLeftPos.x) / m_OverallTextSize.x - 0.5f) * factor;
			}
			position.y += y * factor;
			if (depthPerspectiveStrength.y != 0f)
			{
				position.y += depthPerspectiveStrength.y * ((m_TopLeftPos.y - position.y) / m_OverallTextSize.y - 0.5f) * factor;
			}
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
		Text component = GetComponent<Text>();
		int num = 0;
		int num2 = m_Verts.Count;
		if (m_OnlyInitialCharactersGenerateDepth)
		{
			num = m_Verts.Count - component.cachedTextGenerator.characterCountVisible * 6;
			num2 = component.cachedTextGenerator.characterCountVisible * 6;
		}
		if (num2 == 0)
		{
			return;
		}
		if (depthPerspectiveStrength.x != 0f || depthPerspectiveStrength.y != 0f)
		{
			m_TopLeftPos = m_Verts[num].position;
			m_BottomRightPos = m_Verts[num + num2 - 1].position;
			for (int i = num; i < num + num2; i++)
			{
				if (m_Verts[i].position.x < m_TopLeftPos.x)
				{
					m_TopLeftPos.x = m_Verts[i].position.x;
				}
				if (m_Verts[i].position.y > m_TopLeftPos.y)
				{
					m_TopLeftPos.y = m_Verts[i].position.y;
				}
				if (m_Verts[i].position.x > m_BottomRightPos.x)
				{
					m_BottomRightPos.x = m_Verts[i].position.x;
				}
				if (m_Verts[i].position.y < m_BottomRightPos.y)
				{
					m_BottomRightPos.y = m_Verts[i].position.y;
				}
			}
			m_OverallTextSize = new Vector2(m_BottomRightPos.x - m_TopLeftPos.x, m_TopLeftPos.y - m_BottomRightPos.y);
		}
		int num3 = num;
		num3 = num;
		int count2 = m_Verts.Count;
		ApplyShadowZeroAlloc(m_Verts, effectColor, num3, m_Verts.Count, effectDirectionAndDepth.x, effectDirectionAndDepth.y, 0.25f);
		num3 = count2;
		int count3 = m_Verts.Count;
		ApplyShadowZeroAlloc(m_Verts, effectColor, num3, m_Verts.Count, effectDirectionAndDepth.x, effectDirectionAndDepth.y, 0.5f);
		num3 = count3;
		int count4 = m_Verts.Count;
		ApplyShadowZeroAlloc(m_Verts, effectColor, num3, m_Verts.Count, effectDirectionAndDepth.x, effectDirectionAndDepth.y, 0.75f);
		num3 = count4;
		_ = m_Verts.Count;
		ApplyShadowZeroAlloc(m_Verts, effectColor, num3, m_Verts.Count, effectDirectionAndDepth.x, effectDirectionAndDepth.y, 1f);
		if (onlyInitialCharactersGenerateDepth)
		{
			List<UIVertex> range = m_Verts.GetRange(0, count - num2);
			m_Verts.RemoveRange(0, count - num2);
			m_Verts.InsertRange(m_Verts.Count - num2, range);
		}
		if (component.material.shader == Shader.Find("Text Effects/Fancy Text"))
		{
			for (int j = 0; j < m_Verts.Count - count; j++)
			{
				UIVertex value = m_Verts[j];
				value.uv1 = new Vector2(0f, 0f);
				m_Verts[j] = value;
			}
		}
		vh.Clear();
		vh.AddUIVertexTriangleStream(m_Verts);
	}
}
