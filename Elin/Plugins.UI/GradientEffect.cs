using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Effects/GradientEffect")]
public class GradientEffect : BaseMeshEffect
{
	public GradientMode gradientMode;

	public GradientDir gradientDir;

	public bool overwriteAllColor;

	public Color vertex1 = Color.white;

	public Color vertex2 = Color.black;

	private Graphic _targetGraphic;

	private Graphic targetGraphic
	{
		get
		{
			if (_targetGraphic == null)
			{
				_targetGraphic = GetComponent<Graphic>();
			}
			return _targetGraphic;
		}
	}

	public override void ModifyMesh(VertexHelper vh)
	{
		if (IsActive())
		{
			List<UIVertex> list = new List<UIVertex>();
			vh.GetUIVertexStream(list);
			ModifyVertices(list);
			vh.Clear();
			vh.AddUIVertexTriangleStream(list);
		}
	}

	public void ModifyVertices(List<UIVertex> vertexList)
	{
		if (!IsActive() || vertexList.Count == 0)
		{
			return;
		}
		int count = vertexList.Count;
		UIVertex uIVertex = vertexList[0];
		if (gradientMode == GradientMode.Global)
		{
			if (gradientDir == GradientDir.DiagonalLeftToRight || gradientDir == GradientDir.DiagonalRightToLeft)
			{
				gradientDir = GradientDir.Vertical;
			}
			float num = ((gradientDir == GradientDir.Vertical) ? vertexList[vertexList.Count - 1].position.y : vertexList[vertexList.Count - 1].position.x);
			float num2 = ((gradientDir == GradientDir.Vertical) ? vertexList[0].position.y : vertexList[0].position.x) - num;
			for (int i = 0; i < count; i++)
			{
				uIVertex = vertexList[i];
				if (overwriteAllColor || !(uIVertex.color != targetGraphic.color))
				{
					ref Color32 color = ref uIVertex.color;
					color *= Color.Lerp(vertex2, vertex1, (((gradientDir == GradientDir.Vertical) ? uIVertex.position.y : uIVertex.position.x) - num) / num2);
					vertexList[i] = uIVertex;
				}
			}
			return;
		}
		for (int j = 0; j < count; j++)
		{
			uIVertex = vertexList[j];
			if (overwriteAllColor || CompareCarefully(uIVertex.color, targetGraphic.color))
			{
				switch (gradientDir)
				{
				case GradientDir.Vertical:
				{
					ref Color32 color4 = ref uIVertex.color;
					color4 *= ((j % 4 == 0 || (j - 1) % 4 == 0) ? vertex1 : vertex2);
					break;
				}
				case GradientDir.Horizontal:
				{
					ref Color32 color5 = ref uIVertex.color;
					color5 *= ((j % 4 == 0 || (j - 3) % 4 == 0) ? vertex1 : vertex2);
					break;
				}
				case GradientDir.DiagonalLeftToRight:
				{
					ref Color32 color3 = ref uIVertex.color;
					color3 *= ((j % 4 == 0) ? vertex1 : (((j - 2) % 4 == 0) ? vertex2 : Color.Lerp(vertex2, vertex1, 0.5f)));
					break;
				}
				case GradientDir.DiagonalRightToLeft:
				{
					ref Color32 color2 = ref uIVertex.color;
					color2 *= (((j - 1) % 4 == 0) ? vertex1 : (((j - 3) % 4 == 0) ? vertex2 : Color.Lerp(vertex2, vertex1, 0.5f)));
					break;
				}
				}
				vertexList[j] = uIVertex;
			}
		}
	}

	private bool CompareCarefully(Color col1, Color col2)
	{
		if (Mathf.Abs(col1.r - col2.r) < 0.003f && Mathf.Abs(col1.g - col2.g) < 0.003f && Mathf.Abs(col1.b - col2.b) < 0.003f && Mathf.Abs(col1.a - col2.a) < 0.003f)
		{
			return true;
		}
		return false;
	}
}
