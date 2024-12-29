using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/ToJ Effects/ToJ Outline", 15)]
public class ToJOutline : Shadow
{
	private List<UIVertex> m_Verts = new List<UIVertex>();

	protected ToJOutline()
	{
	}

	public override void ModifyMesh(VertexHelper vh)
	{
		if (!IsActive())
		{
			return;
		}
		vh.GetUIVertexStream(m_Verts);
		int count = m_Verts.Count;
		int num = m_Verts.Count * 5;
		if (m_Verts.Capacity < num)
		{
			m_Verts.Capacity = num;
		}
		int start = 0;
		int count2 = m_Verts.Count;
		ApplyShadowZeroAlloc(m_Verts, base.effectColor, start, m_Verts.Count, base.effectDistance.x, base.effectDistance.y);
		start = count2;
		int count3 = m_Verts.Count;
		ApplyShadowZeroAlloc(m_Verts, base.effectColor, start, m_Verts.Count, base.effectDistance.x, 0f - base.effectDistance.y);
		start = count3;
		int count4 = m_Verts.Count;
		ApplyShadowZeroAlloc(m_Verts, base.effectColor, start, m_Verts.Count, 0f - base.effectDistance.x, base.effectDistance.y);
		start = count4;
		_ = m_Verts.Count;
		ApplyShadowZeroAlloc(m_Verts, base.effectColor, start, m_Verts.Count, 0f - base.effectDistance.x, 0f - base.effectDistance.y);
		Text component = GetComponent<Text>();
		if (component != null && component.material.shader == Shader.Find("Text Effects/Fancy Text"))
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
