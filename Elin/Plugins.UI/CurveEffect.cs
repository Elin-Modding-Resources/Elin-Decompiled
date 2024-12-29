using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/ToJ Effects/Curve Effect", 6)]
[RequireComponent(typeof(Text))]
public class CurveEffect : BaseMeshEffect
{
	public enum CurveMode
	{
		TextArea,
		FullRect
	}

	[SerializeField]
	private CurveMode m_CurveMode;

	[SerializeField]
	private AnimationCurve m_Curve = new AnimationCurve(new Keyframe(0f, 0f, 0f, 2f), new Keyframe(1f, 0f, -2f, 0f));

	[SerializeField]
	private float m_Strength = 1f;

	private List<UIVertex> m_Verts = new List<UIVertex>();

	public AnimationCurve curve
	{
		get
		{
			return m_Curve;
		}
		set
		{
			if (m_Curve != value)
			{
				m_Curve = value;
				if (base.graphic != null)
				{
					base.graphic.SetVerticesDirty();
				}
			}
		}
	}

	public float strength
	{
		get
		{
			return m_Strength;
		}
		set
		{
			if (m_Strength != value)
			{
				m_Strength = value;
				if (base.graphic != null)
				{
					base.graphic.SetVerticesDirty();
				}
			}
		}
	}

	protected CurveEffect()
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
		if (m_CurveMode == CurveMode.FullRect)
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
		float num = zero2.x - zero.x;
		for (int j = 0; j < m_Verts.Count; j++)
		{
			UIVertex value = m_Verts[j];
			value.position.y += curve.Evaluate((value.position.x - zero.x) / num) * strength;
			m_Verts[j] = value;
		}
		vh.Clear();
		vh.AddUIVertexTriangleStream(m_Verts);
	}
}
