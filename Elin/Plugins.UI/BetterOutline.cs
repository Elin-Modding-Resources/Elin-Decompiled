using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/ToJ Effects/Better Outline", 0)]
[RequireComponent(typeof(Text))]
public class BetterOutline : Shadow
{
	private List<UIVertex> m_Verts = new List<UIVertex>();

	protected BetterOutline()
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
		int num = 0;
		int num2 = 0;
		for (int i = -1; i <= 1; i++)
		{
			for (int j = -1; j <= 1; j++)
			{
				if (i != 0 && j != 0)
				{
					num = num2;
					num2 = m_Verts.Count;
					ApplyShadowZeroAlloc(m_Verts, base.effectColor, num, m_Verts.Count, (float)i * base.effectDistance.x * 0.707f, (float)j * base.effectDistance.y * 0.707f);
				}
			}
		}
		num = num2;
		num2 = m_Verts.Count;
		ApplyShadowZeroAlloc(m_Verts, base.effectColor, num, m_Verts.Count, 0f - base.effectDistance.x, 0f);
		num = num2;
		num2 = m_Verts.Count;
		ApplyShadowZeroAlloc(m_Verts, base.effectColor, num, m_Verts.Count, base.effectDistance.x, 0f);
		num = num2;
		num2 = m_Verts.Count;
		ApplyShadowZeroAlloc(m_Verts, base.effectColor, num, m_Verts.Count, 0f, 0f - base.effectDistance.y);
		num = num2;
		num2 = m_Verts.Count;
		ApplyShadowZeroAlloc(m_Verts, base.effectColor, num, m_Verts.Count, 0f, base.effectDistance.y);
		if (GetComponent<Text>().material.shader == Shader.Find("Text Effects/Fancy Text"))
		{
			for (int k = 0; k < m_Verts.Count - count; k++)
			{
				UIVertex value = m_Verts[k];
				value.uv1 = new Vector2(0f, 0f);
				m_Verts[k] = value;
			}
		}
		vh.Clear();
		vh.AddUIVertexTriangleStream(m_Verts);
	}
}
