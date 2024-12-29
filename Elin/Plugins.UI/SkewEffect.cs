using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/ToJ Effects/Skew Effect", 5)]
[RequireComponent(typeof(Text))]
public class SkewEffect : BaseMeshEffect
{
	public enum SkewMode
	{
		TextArea,
		FullRect
	}

	[SerializeField]
	private SkewMode m_SkewMode;

	[SerializeField]
	private Vector2 m_UpperLeftOffset = Vector2.zero;

	[SerializeField]
	private Vector2 m_UpperRightOffset = Vector2.zero;

	[SerializeField]
	private Vector2 m_LowerLeftOffset = Vector2.zero;

	[SerializeField]
	private Vector2 m_LowerRightOffset = Vector2.zero;

	private List<UIVertex> m_Verts = new List<UIVertex>();

	public Vector2 upperLeftOffset
	{
		get
		{
			return m_UpperLeftOffset;
		}
		set
		{
			m_UpperLeftOffset = value;
			if (base.graphic != null)
			{
				base.graphic.SetVerticesDirty();
			}
		}
	}

	public Vector2 upperRightOffset
	{
		get
		{
			return m_UpperRightOffset;
		}
		set
		{
			m_UpperRightOffset = value;
			if (base.graphic != null)
			{
				base.graphic.SetVerticesDirty();
			}
		}
	}

	public Vector2 lowerLeftOffset
	{
		get
		{
			return m_LowerLeftOffset;
		}
		set
		{
			m_LowerLeftOffset = value;
			if (base.graphic != null)
			{
				base.graphic.SetVerticesDirty();
			}
		}
	}

	public Vector2 lowerRightOffset
	{
		get
		{
			return m_LowerRightOffset;
		}
		set
		{
			m_LowerRightOffset = value;
			if (base.graphic != null)
			{
				base.graphic.SetVerticesDirty();
			}
		}
	}

	protected SkewEffect()
	{
	}

	public override void ModifyMesh(VertexHelper vh)
	{
		if (!IsActive())
		{
			return;
		}
		vh.GetUIVertexStream(m_Verts);
		if (m_Verts.Count == 0)
		{
			return;
		}
		Vector2 zero = Vector2.zero;
		Vector2 zero2 = Vector2.zero;
		if (m_SkewMode == SkewMode.FullRect)
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
		for (int j = 0; j < m_Verts.Count; j++)
		{
			UIVertex value = m_Verts[j];
			float num3 = (value.position.y - zero2.y) / num;
			float num4 = 1f - num3;
			float num5 = (zero2.x - value.position.x) / num2;
			float num6 = 1f - num5;
			Vector3 zero3 = Vector3.zero;
			zero3.y = (upperLeftOffset.y * num3 + lowerLeftOffset.y * num4) * num5 + (upperRightOffset.y * num3 + lowerRightOffset.y * num4) * num6;
			zero3.x = (upperLeftOffset.x * num5 + upperRightOffset.x * num6) * num3 + (lowerLeftOffset.x * num5 + lowerRightOffset.x * num6) * num4;
			value.position += zero3;
			m_Verts[j] = value;
		}
		vh.Clear();
		vh.AddUIVertexTriangleStream(m_Verts);
	}
}
