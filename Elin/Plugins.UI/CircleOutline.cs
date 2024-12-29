using System;
using System.Collections.Generic;
using UnityEngine;

public class CircleOutline : ModifiedShadow
{
	[SerializeField]
	private int m_circleCount = 2;

	[SerializeField]
	private int m_firstSample = 4;

	[SerializeField]
	private int m_sampleIncrement = 2;

	public int circleCount
	{
		get
		{
			return m_circleCount;
		}
		set
		{
			m_circleCount = Mathf.Max(value, 1);
			if (base.graphic != null)
			{
				base.graphic.SetVerticesDirty();
			}
		}
	}

	public int firstSample
	{
		get
		{
			return m_firstSample;
		}
		set
		{
			m_firstSample = Mathf.Max(value, 2);
			if (base.graphic != null)
			{
				base.graphic.SetVerticesDirty();
			}
		}
	}

	public int sampleIncrement
	{
		get
		{
			return m_sampleIncrement;
		}
		set
		{
			m_sampleIncrement = Mathf.Max(value, 1);
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
		int num = (m_firstSample * 2 + m_sampleIncrement * (m_circleCount - 1)) * m_circleCount / 2;
		int num2 = verts.Count * (num + 1);
		if (verts.Capacity < num2)
		{
			verts.Capacity = num2;
		}
		int count = verts.Count;
		int num3 = 0;
		int num4 = m_firstSample;
		float num5 = base.effectDistance.x / (float)circleCount;
		float num6 = base.effectDistance.y / (float)circleCount;
		for (int i = 1; i <= m_circleCount; i++)
		{
			float num7 = num5 * (float)i;
			float num8 = num6 * (float)i;
			float num9 = MathF.PI * 2f / (float)num4;
			float num10 = (float)(i % 2) * num9 * 0.5f;
			for (int j = 0; j < num4; j++)
			{
				int num11 = num3 + count;
				ApplyShadow(verts, base.effectColor, num3, num11, num7 * Mathf.Cos(num10), num8 * Mathf.Sin(num10));
				num3 = num11;
				num10 += num9;
			}
			num4 += m_sampleIncrement;
		}
	}
}
