using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/ToJ Effects/Gradient Color", 1)]
[RequireComponent(typeof(Text))]
public class GradientColor : BaseMeshEffect
{
	public enum GradientMode
	{
		Local,
		GlobalTextArea,
		GlobalFullRect
	}

	public enum GradientDirection
	{
		Vertical,
		Horizontal
	}

	public enum ColorMode
	{
		Override,
		Additive,
		Multiply
	}

	[SerializeField]
	private GradientMode m_GradientMode;

	[SerializeField]
	private GradientDirection m_GradientDirection;

	[SerializeField]
	private ColorMode m_ColorMode;

	[SerializeField]
	public Color m_FirstColor = Color.white;

	[SerializeField]
	public Color m_SecondColor = Color.black;

	[SerializeField]
	private bool m_UseGraphicAlpha = true;

	private List<UIVertex> m_Verts = new List<UIVertex>();

	public GradientMode gradientMode
	{
		get
		{
			return m_GradientMode;
		}
		set
		{
			m_GradientMode = value;
			if (base.graphic != null)
			{
				base.graphic.SetVerticesDirty();
			}
		}
	}

	public GradientDirection gradientDirection
	{
		get
		{
			return m_GradientDirection;
		}
		set
		{
			m_GradientDirection = value;
			if (base.graphic != null)
			{
				base.graphic.SetVerticesDirty();
			}
		}
	}

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

	public Color firstColor
	{
		get
		{
			return m_FirstColor;
		}
		set
		{
			m_FirstColor = value;
			if (base.graphic != null)
			{
				base.graphic.SetVerticesDirty();
			}
		}
	}

	public Color secondColor
	{
		get
		{
			return m_SecondColor;
		}
		set
		{
			m_SecondColor = value;
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

	protected GradientColor()
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
		if (gradientMode == GradientMode.GlobalTextArea || gradientMode == GradientMode.GlobalFullRect)
		{
			Vector2 zero = Vector2.zero;
			Vector2 zero2 = Vector2.zero;
			if (gradientMode == GradientMode.GlobalFullRect)
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
				if (gradientDirection == GradientDirection.Vertical)
				{
					Color newColor = Color.Lerp(firstColor, secondColor, (zero.y - value.position.y) / num);
					value.color = CalculateColor(value.color, newColor, colorMode);
				}
				else
				{
					Color newColor2 = Color.Lerp(firstColor, secondColor, (value.position.x - zero.x) / num2);
					value.color = CalculateColor(value.color, newColor2, colorMode);
				}
				if (useGraphicAlpha)
				{
					value.color.a = (byte)(value.color.a * m_Verts[j].color.a / 255);
				}
				m_Verts[j] = value;
			}
		}
		else
		{
			for (int k = 0; k < m_Verts.Count; k++)
			{
				UIVertex value2 = m_Verts[k];
				if (gradientDirection == GradientDirection.Vertical)
				{
					if (k % 6 == 0 || k % 6 == 1 || k % 6 == 5)
					{
						Color newColor3 = firstColor;
						value2.color = CalculateColor(value2.color, newColor3, colorMode);
					}
					else
					{
						Color newColor4 = secondColor;
						value2.color = CalculateColor(value2.color, newColor4, colorMode);
					}
				}
				else if (k % 6 == 0 || k % 6 == 4 || k % 6 == 5)
				{
					Color newColor5 = firstColor;
					value2.color = CalculateColor(value2.color, newColor5, colorMode);
				}
				else
				{
					Color newColor6 = secondColor;
					value2.color = CalculateColor(value2.color, newColor6, colorMode);
				}
				if (useGraphicAlpha)
				{
					value2.color.a = (byte)(value2.color.a * m_Verts[k].color.a / 255);
				}
				m_Verts[k] = value2;
			}
		}
		vh.Clear();
		vh.AddUIVertexTriangleStream(m_Verts);
	}

	private Color CalculateColor(Color initialColor, Color newColor, ColorMode colorMode)
	{
		return colorMode switch
		{
			ColorMode.Override => newColor, 
			ColorMode.Additive => initialColor + newColor, 
			ColorMode.Multiply => initialColor * newColor, 
			_ => newColor, 
		};
	}
}
