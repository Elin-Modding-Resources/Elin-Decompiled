using System.Collections.Generic;
using UnityEngine;

public class BoxOutline : ModifiedShadow
{
	private const int maxHalfSampleCount = 20;

	[SerializeField]
	[Range(1f, 20f)]
	private int m_halfSampleCountX = 1;

	[SerializeField]
	[Range(1f, 20f)]
	private int m_halfSampleCountY = 1;

	public int halfSampleCountX
	{
		get
		{
			return m_halfSampleCountX;
		}
		set
		{
			m_halfSampleCountX = Mathf.Clamp(value, 1, 20);
			if (base.graphic != null)
			{
				base.graphic.SetVerticesDirty();
			}
		}
	}

	public int halfSampleCountY
	{
		get
		{
			return m_halfSampleCountY;
		}
		set
		{
			m_halfSampleCountY = Mathf.Clamp(value, 1, 20);
			if (base.graphic != null)
			{
				base.graphic.SetVerticesDirty();
			}
		}
	}

	public override void ModifyVertices(List<UIVertex> verts)
	{
		if (!IsActive())
		{
			return;
		}
		int num = verts.Count * (m_halfSampleCountX * 2 + 1) * (m_halfSampleCountY * 2 + 1);
		if (verts.Capacity < num)
		{
			verts.Capacity = num;
		}
		int count = verts.Count;
		int num2 = 0;
		float num3 = base.effectDistance.x / (float)m_halfSampleCountX;
		float num4 = base.effectDistance.y / (float)m_halfSampleCountY;
		for (int i = -m_halfSampleCountX; i <= m_halfSampleCountX; i++)
		{
			for (int j = -m_halfSampleCountY; j <= m_halfSampleCountY; j++)
			{
				if (i != 0 || j != 0)
				{
					int num5 = num2 + count;
					ApplyShadow(verts, base.effectColor, num2, num5, num3 * (float)i, num4 * (float)j);
					num2 = num5;
				}
			}
		}
	}
}
