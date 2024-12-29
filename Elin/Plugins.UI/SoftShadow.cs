using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/ToJ Effects/Soft Shadow", 3)]
[RequireComponent(typeof(Text))]
public class SoftShadow : Shadow
{
	[SerializeField]
	private float m_BlurSpread = 1f;

	[SerializeField]
	private bool m_OnlyInitialCharactersDropShadow = true;

	private List<UIVertex> m_Verts = new List<UIVertex>();

	public float blurSpread
	{
		get
		{
			return m_BlurSpread;
		}
		set
		{
			m_BlurSpread = value;
			if (base.graphic != null)
			{
				base.graphic.SetVerticesDirty();
			}
		}
	}

	public bool onlyInitialCharactersDropShadow
	{
		get
		{
			return m_OnlyInitialCharactersDropShadow;
		}
		set
		{
			m_OnlyInitialCharactersDropShadow = value;
			if (base.graphic != null)
			{
				base.graphic.SetVerticesDirty();
			}
		}
	}

	protected SoftShadow()
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
		Text component = GetComponent<Text>();
		int num = 0;
		int num2 = m_Verts.Count;
		if (m_OnlyInitialCharactersDropShadow)
		{
			num = m_Verts.Count - component.cachedTextGenerator.characterCountVisible * 6;
			num2 = component.cachedTextGenerator.characterCountVisible * 6;
		}
		Color color = base.effectColor;
		color.a /= 4f;
		int start = num;
		int count2 = m_Verts.Count;
		ApplyShadowZeroAlloc(m_Verts, color, start, m_Verts.Count, base.effectDistance.x, base.effectDistance.y);
		for (int i = -1; i <= 1; i++)
		{
			for (int j = -1; j <= 1; j++)
			{
				if (i != 0 || j != 0)
				{
					start = count2;
					count2 = m_Verts.Count;
					ApplyShadowZeroAlloc(m_Verts, color, start, m_Verts.Count, base.effectDistance.x + (float)i * blurSpread, base.effectDistance.y + (float)j * blurSpread);
				}
			}
		}
		if (onlyInitialCharactersDropShadow)
		{
			List<UIVertex> range = m_Verts.GetRange(0, count - num2);
			m_Verts.RemoveRange(0, count - num2);
			m_Verts.InsertRange(m_Verts.Count - num2, range);
		}
		if (component.material.shader == Shader.Find("Text Effects/Fancy Text"))
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
