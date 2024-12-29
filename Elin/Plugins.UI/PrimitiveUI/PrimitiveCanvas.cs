using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace PrimitiveUI;

[AddComponentMenu("UI/Primitive Canvas")]
public class PrimitiveCanvas : MaskableGraphic
{
	private abstract class PUIElement
	{
		protected Color32 color;

		protected Vector2[] points;

		protected UIVertex[] uiVerts;

		protected List<UIVertex> uiVertexTriangleStream;

		public abstract List<UIVertex> GetUIVertexTriangleStream(Vector2 offset, Vector2 scale, Color32 color);
	}

	private class PUIFillElement : PUIElement
	{
		private int[] triangles;

		public PUIFillElement(Vector2[] points, int[] triangles, Color32 color)
		{
			uiVerts = new UIVertex[points.Length];
			uiVertexTriangleStream = new List<UIVertex>(triangles.Length);
			base.points = points;
			this.triangles = triangles;
			base.color = color;
			for (int i = 0; i < uiVerts.Length; i++)
			{
				UIVertex simpleVert = UIVertex.simpleVert;
				simpleVert.color = color;
				uiVerts[i] = simpleVert;
			}
		}

		public override List<UIVertex> GetUIVertexTriangleStream(Vector2 offset, Vector2 scale, Color32 color)
		{
			uiVertexTriangleStream.Clear();
			color = (Color)color * (Color)base.color;
			if (color.Equals(uiVerts[0].color))
			{
				for (int i = 0; i < uiVerts.Length; i++)
				{
					uiVerts[i].position = new Vector3((points[i].x + offset.x) * scale.x, (points[i].y + offset.y) * scale.y, 0f);
				}
			}
			else
			{
				for (int j = 0; j < uiVerts.Length; j++)
				{
					uiVerts[j].color = color;
					uiVerts[j].position = new Vector3((points[j].x + offset.x) * scale.x, (points[j].y + offset.y) * scale.y, 0f);
				}
			}
			for (int k = 0; k < triangles.Length; k++)
			{
				uiVertexTriangleStream.Add(uiVerts[triangles[k]]);
			}
			return uiVertexTriangleStream;
		}
	}

	private class PUIStrokeElement : PUIElement
	{
		public Vector2[] rawPoints { get; private set; }

		public StrokeStyle strokeStyle { get; private set; }

		public bool isClosedPath { get; private set; }

		public PUIStrokeElement(Vector2[] rawPoints, Vector2[] points, StrokeStyle strokeStyle, bool isClosedPath)
		{
			uiVerts = new UIVertex[points.Length];
			uiVertexTriangleStream = new List<UIVertex>((points.Length - 1) * 6);
			this.rawPoints = rawPoints;
			this.strokeStyle = strokeStyle;
			this.isClosedPath = isClosedPath;
			color = strokeStyle.color;
			for (int i = 0; i < uiVerts.Length; i++)
			{
				UIVertex simpleVert = UIVertex.simpleVert;
				simpleVert.color = color;
				uiVerts[i] = simpleVert;
			}
			UpdatePoints(points);
		}

		public void UpdatePoints(Vector2[] newPoints)
		{
			points = newPoints;
		}

		public override List<UIVertex> GetUIVertexTriangleStream(Vector2 offset, Vector2 scale, Color32 color)
		{
			uiVertexTriangleStream.Clear();
			color = (Color)color * (Color)base.color;
			if (color.Equals(uiVerts[0].color))
			{
				for (int i = 0; i < uiVerts.Length; i++)
				{
					uiVerts[i].position = new Vector3((points[i].x + offset.x) * scale.x, (points[i].y + offset.y) * scale.y, 0f);
				}
			}
			else
			{
				for (int j = 0; j < uiVerts.Length; j++)
				{
					uiVerts[j].color = color;
					uiVerts[j].position = new Vector3((points[j].x + offset.x) * scale.x, (points[j].y + offset.y) * scale.y, 0f);
				}
			}
			for (int k = 0; k < uiVerts.Length; k += 4)
			{
				uiVertexTriangleStream.Add(uiVerts[k]);
				uiVertexTriangleStream.Add(uiVerts[k + 1]);
				uiVertexTriangleStream.Add(uiVerts[k + 2]);
				uiVertexTriangleStream.Add(uiVerts[k + 2]);
				uiVertexTriangleStream.Add(uiVerts[k + 3]);
				uiVertexTriangleStream.Add(uiVerts[k]);
			}
			return uiVertexTriangleStream;
		}
	}

	private class PUIUtils
	{
		public static float Cross2D(Vector2 lhs, Vector2 rhs)
		{
			return lhs.x * rhs.y - lhs.y * rhs.x;
		}

		public static float GetTriangleArea(Vector2 tri0, Vector2 tri1, Vector2 tri2)
		{
			return Mathf.Abs((0f - tri1.y) * tri2.x + tri0.y * (0f - tri1.x + tri2.x) + tri0.x * (tri1.y - tri2.y) + tri1.x * tri2.y);
		}

		public static bool PointInTriangle(Vector2 point, Vector2 tri0, Vector2 tri1, Vector2 tri2, float triAarea)
		{
			float num = tri0.y * tri2.x - tri0.x * tri2.y + (tri2.y - tri0.y) * point.x + (tri0.x - tri2.x) * point.y;
			float num2 = tri0.x * tri1.y - tri0.y * tri1.x + (tri0.y - tri1.y) * point.x + (tri1.x - tri0.x) * point.y;
			if (num <= 0f || num2 <= 0f)
			{
				return false;
			}
			return num + num2 < triAarea;
		}

		public static Vector2? GetLineIntersection(Vector2 line1P1, Vector2 line1P2, Vector2 line2P1, Vector2 line2P2)
		{
			float num = (line2P2.y - line2P1.y) * (line1P2.x - line1P1.x) - (line2P2.x - line2P1.x) * (line1P2.y - line1P1.y);
			float num2 = (line2P2.x - line2P1.x) * (line1P1.y - line2P1.y) - (line2P2.y - line2P1.y) * (line1P1.x - line2P1.x);
			float f = (line1P2.x - line1P1.x) * (line1P1.y - line2P1.y) - (line1P2.y - line1P1.y) * (line1P1.x - line2P1.x);
			if (Mathf.Abs(num2) < Mathf.Epsilon && Mathf.Abs(f) < Mathf.Epsilon && Mathf.Abs(num) < Mathf.Epsilon)
			{
				return new Vector2((line1P1.x + line1P2.x) * 0.5f, (line1P1.y + line1P2.y) * 0.5f);
			}
			if (Mathf.Abs(num) < Mathf.Epsilon)
			{
				return null;
			}
			float num3 = num2 / num;
			return new Vector2(line1P1.x + num3 * (line1P2.x - line1P1.x), line1P1.y + num3 * (line1P2.y - line1P1.y));
		}

		public static Vector2[] GetLinePoints(Vector2 point1, Vector2 point2, float strokeThickness, float aspectRatio)
		{
			Vector2 vector = new Vector2(strokeThickness, strokeThickness * aspectRatio);
			Vector2 normalized = (point2 - point1).normalized;
			Vector2 vector2 = new Vector2((0f - normalized.y) * vector.x, normalized.x * vector.y);
			return new Vector2[4]
			{
				point1 - vector2,
				point1 + vector2,
				point2 + vector2,
				point2 - vector2
			};
		}

		public static Vector2[] GetPathPoints(Vector2[] points, bool closePath, float strokeThickness, float aspectRatio)
		{
			Vector2 vector = new Vector2(strokeThickness, strokeThickness * aspectRatio);
			Vector2 normalized = (points[1] - points[0]).normalized;
			Vector2 vector2 = new Vector2((0f - normalized.y) * vector.x, normalized.x * vector.y);
			List<Vector2> list = new List<Vector2>();
			list.Add(points[0] - vector2);
			list.Add(points[0] + vector2);
			list.Add(points[1] + vector2);
			list.Add(points[1] - vector2);
			for (int i = 1; i < points.Length - 1; i++)
			{
				normalized = (points[i + 1] - points[i]).normalized;
				vector2 = new Vector2((0f - normalized.y) * vector.x, normalized.x * vector.y);
				list.Add(points[i] - vector2);
				list.Add(points[i] + vector2);
				list.Add(points[i + 1] + vector2);
				list.Add(points[i + 1] - vector2);
				Vector2? lineIntersection = GetLineIntersection(list[list.Count - 8], list[list.Count - 5], list[list.Count - 4], list[list.Count - 1]);
				Vector2? lineIntersection2 = GetLineIntersection(list[list.Count - 7], list[list.Count - 6], list[list.Count - 3], list[list.Count - 2]);
				if (lineIntersection.HasValue)
				{
					Vector2 value = lineIntersection.Value;
					int index = list.Count - 5;
					Vector2 value2 = (list[list.Count - 4] = value);
					list[index] = value2;
				}
				if (lineIntersection2.HasValue)
				{
					Vector2 value = lineIntersection2.Value;
					int index2 = list.Count - 6;
					Vector2 value2 = (list[list.Count - 3] = value);
					list[index2] = value2;
				}
			}
			if (closePath)
			{
				normalized = (points[^1] - points[0]).normalized;
				vector2 = new Vector2((0f - normalized.y) * vector.x, normalized.x * vector.y);
				list.Add(points[^1] - vector2);
				list.Add(points[^1] + vector2);
				list.Add(points[0] + vector2);
				list.Add(points[0] - vector2);
				Vector2? lineIntersection = GetLineIntersection(list[list.Count - 8], list[list.Count - 5], list[list.Count - 3], list[list.Count - 2]);
				Vector2? lineIntersection2 = GetLineIntersection(list[list.Count - 7], list[list.Count - 6], list[list.Count - 4], list[list.Count - 1]);
				if (lineIntersection.HasValue)
				{
					Vector2 value = lineIntersection.Value;
					int index3 = list.Count - 5;
					Vector2 value2 = (list[list.Count - 3] = value);
					list[index3] = value2;
				}
				if (lineIntersection2.HasValue)
				{
					Vector2 value = lineIntersection2.Value;
					int index4 = list.Count - 6;
					Vector2 value2 = (list[list.Count - 4] = value);
					list[index4] = value2;
				}
				lineIntersection = GetLineIntersection(list[3], list[0], list[list.Count - 3], list[list.Count - 2]);
				lineIntersection2 = GetLineIntersection(list[2], list[1], list[list.Count - 4], list[list.Count - 1]);
				if (lineIntersection.HasValue)
				{
					Vector2 value = lineIntersection.Value;
					Vector2 value2 = (list[list.Count - 2] = value);
					list[0] = value2;
				}
				if (lineIntersection2.HasValue)
				{
					Vector2 value = lineIntersection2.Value;
					Vector2 value2 = (list[list.Count - 1] = value);
					list[1] = value2;
				}
			}
			return list.ToArray();
		}
	}

	public bool setDirtyOnDraw = true;

	private List<PUIElement> elements = new List<PUIElement>();

	public float aspectRatio => base.rectTransform.rect.width / base.rectTransform.rect.height;

	protected override void OnPopulateMesh(VertexHelper vh)
	{
		vh.Clear();
		Vector2 size = base.rectTransform.rect.size;
		Vector3 vector = new Vector3(0f - base.rectTransform.pivot.x, 0f - base.rectTransform.pivot.y, 0f);
		List<List<UIVertex>> list = new List<List<UIVertex>>(elements.Count);
		for (int i = 0; i < elements.Count; i++)
		{
			list.Add(elements[i].GetUIVertexTriangleStream(vector, size, color));
		}
		vh.AddUIVertexTriangleStream(list.SelectMany((List<UIVertex> l) => l).ToList());
	}

	public void Clear()
	{
		elements.Clear();
		SetAllDirty();
	}

	protected override void OnRectTransformDimensionsChange()
	{
		base.OnRectTransformDimensionsChange();
		foreach (PUIElement element in elements)
		{
			if (!(element.GetType() == typeof(PUIStrokeElement)))
			{
				continue;
			}
			PUIStrokeElement pUIStrokeElement = (PUIStrokeElement)element;
			if (pUIStrokeElement.strokeStyle.scaleMode == StrokeScaleMode.Absolute)
			{
				float strokeThickness = pUIStrokeElement.strokeStyle.thickness / base.rectTransform.rect.width;
				if (pUIStrokeElement.rawPoints.Length == 2)
				{
					pUIStrokeElement.UpdatePoints(PUIUtils.GetLinePoints(pUIStrokeElement.rawPoints[0], pUIStrokeElement.rawPoints[1], strokeThickness, aspectRatio));
				}
				else
				{
					pUIStrokeElement.UpdatePoints(PUIUtils.GetPathPoints(pUIStrokeElement.rawPoints, pUIStrokeElement.isClosedPath, strokeThickness, aspectRatio));
				}
			}
		}
	}

	public void DrawSquare(Vector2 center, float size, Color fillColor)
	{
		DrawSquare(center, size, 0f, fillColor);
	}

	public void DrawSquare(Vector2 center, float size, StrokeStyle strokeStyle)
	{
		DrawSquare(center, size, 0f, null, strokeStyle);
	}

	public void DrawSquare(Vector2 center, float size, Color fillColor, StrokeStyle strokeStyle)
	{
		DrawSquare(center, size, 0f, fillColor, strokeStyle);
	}

	public void DrawSquare(Vector2 center, float size, float rotation = 0f, Color? fillColor = null, StrokeStyle strokeStyle = null)
	{
		DrawRectangle(new Rect(center.x - size * 0.5f, center.y - size * 0.5f * aspectRatio, size, size * aspectRatio), rotation, fillColor, strokeStyle);
	}

	public void DrawRectangle(float x, float y, float width, float height, Color fillColor)
	{
		DrawRectangle(new Rect(x, y, width, height), 0f, fillColor);
	}

	public void DrawRectangle(float x, float y, float width, float height, StrokeStyle strokeStyle)
	{
		DrawRectangle(new Rect(x, y, width, height), strokeStyle);
	}

	public void DrawRectangle(float x, float y, float width, float height, Color fillColor, StrokeStyle strokeStyle)
	{
		DrawRectangle(new Rect(x, y, width, height), fillColor, strokeStyle);
	}

	public void DrawRectangle(float x, float y, float width, float height, float rotation = 0f, Color? fillColor = null, StrokeStyle strokeStyle = null)
	{
		DrawRectangle(new Rect(x, y, width, height), rotation, fillColor, strokeStyle);
	}

	public void DrawRectangle(Rect rect, Color fillColor)
	{
		DrawRectangle(rect, 0f, fillColor);
	}

	public void DrawRectangle(Rect rect, StrokeStyle strokeStyle)
	{
		DrawRectangle(rect, 0f, null, strokeStyle);
	}

	public void DrawRectangle(Rect rect, Color fillColor, StrokeStyle strokeStyle)
	{
		DrawRectangle(rect, 0f, fillColor, strokeStyle);
	}

	public void DrawRectangle(Rect rect, float rotation = 0f, Color? fillColor = null, StrokeStyle strokeStyle = null)
	{
		Vector2 center = rect.center;
		Vector2[] array = new Vector2[4];
		if (rotation != 0f)
		{
			rect.width *= aspectRatio;
		}
		array[0] = new Vector2(rect.min.x, rect.min.y);
		array[1] = new Vector2(rect.min.x, rect.max.y);
		array[2] = new Vector2(rect.max.x, rect.max.y);
		array[3] = new Vector2(rect.max.x, rect.min.y);
		if (rotation != 0f)
		{
			rotation *= MathF.PI / 180f;
			float num = Mathf.Cos(rotation);
			float num2 = Mathf.Sin(rotation);
			for (int i = 0; i < array.Length; i++)
			{
				float num3 = array[i].x - rect.center.x;
				float num4 = array[i].y - rect.center.y;
				array[i] = new Vector2((num3 * num + num4 * num2) / aspectRatio + center.x, num4 * num - num3 * num2 + center.y);
			}
		}
		if (fillColor.HasValue)
		{
			elements.Add(new PUIFillElement(array, new int[6] { 0, 1, 2, 2, 3, 0 }, fillColor.Value));
		}
		if (strokeStyle != null)
		{
			DrawPath(array, strokeStyle, closePath: true);
		}
		if (setDirtyOnDraw)
		{
			SetVerticesDirty();
		}
	}

	public void DrawCircle(Vector2 center, float radius, Color fillColor)
	{
		DrawCircle(center, radius, 1f, 0f, 360f, fillColor);
	}

	public void DrawCircle(Vector2 center, float radius, StrokeStyle strokeStyle)
	{
		DrawCircle(center, radius, 1f, 0f, 360f, Color.white, strokeStyle);
	}

	public void DrawCircle(Vector2 center, float radius, Color fillColor, StrokeStyle strokeStyle)
	{
		DrawCircle(center, radius, 1f, 0f, 360f, fillColor, strokeStyle);
	}

	public void DrawCircle(Vector2 center, float radius, float stepSize = 1f, float startAngle = 0f, float endAngle = 360f, Color? fillColor = null, StrokeStyle strokeStyle = null)
	{
		if (endAngle - startAngle < 0f)
		{
			Debug.LogWarning("DrawCircle() only works in the clockwise-direction; please ensure endAngle > startAngle.");
		}
		else
		{
			DrawEllipse(center, new Vector2(radius, radius * aspectRatio), stepSize, 0f, startAngle, endAngle, fillColor, strokeStyle);
		}
	}

	public void DrawEllipse(Vector2 center, Vector2 radii, Color fillColor)
	{
		DrawEllipse(center, radii, 1f, 0f, 0f, 360f, fillColor);
	}

	public void DrawEllipse(Vector2 center, Vector2 radii, StrokeStyle strokeStyle)
	{
		DrawEllipse(center, radii, 1f, 0f, 0f, 360f, null, strokeStyle);
	}

	public void DrawEllipse(Vector2 center, Vector2 radii, Color fillColor, StrokeStyle strokeStyle)
	{
		DrawEllipse(center, radii, 1f, 0f, 0f, 360f, fillColor, strokeStyle);
	}

	public void DrawEllipse(Vector2 center, Vector2 radii, float stepSize = 1f, float rotation = 0f, float startAngle = 0f, float endAngle = 360f, Color? fillColor = null, StrokeStyle strokeStyle = null)
	{
		if (endAngle - startAngle == 0f)
		{
			return;
		}
		if (endAngle - startAngle < 0f)
		{
			Debug.LogWarning("DrawEllipse() only works in the clockwise-direction; please ensure endAngle > startAngle.");
			return;
		}
		Vector2 vector = center;
		if (rotation != 0f)
		{
			radii.x *= aspectRatio;
		}
		float num = MathF.PI / 2f - startAngle * (MathF.PI / 180f);
		float num2 = MathF.PI * 2f / (360f / stepSize);
		int num3 = Mathf.CeilToInt((endAngle - startAngle) / stepSize) + 1;
		List<Vector2> list = new List<Vector2>();
		list.Add(center);
		for (int i = 0; i < num3; i++)
		{
			list.Add(new Vector2(Mathf.Cos(num - num2 * (float)i) * radii.x + center.x, Mathf.Sin(num - num2 * (float)i) * radii.y + center.y));
		}
		if (rotation != 0f)
		{
			rotation *= MathF.PI / 180f;
			float num4 = aspectRatio;
			float num5 = Mathf.Cos(rotation);
			float num6 = Mathf.Sin(rotation);
			for (int j = 0; j < list.Count; j++)
			{
				float num7 = list[j].x - center.x;
				float num8 = list[j].y - center.y;
				list[j] = new Vector2((num7 * num5 + num8 * num6) / num4 + vector.x, num8 * num5 - num7 * num6 + vector.y);
			}
		}
		if (fillColor.HasValue)
		{
			int[] array;
			if (endAngle - startAngle == 0f)
			{
				array = new int[(list.Count - 1) * 3];
				int num9 = 0;
				int num10 = 1;
				while (num9 < array.Length)
				{
					array[num9] = 0;
					array[num9 + 1] = num10;
					array[num9 + 2] = num10 + 1;
					num9 += 3;
					num10++;
				}
				array[^1] = 1;
			}
			else
			{
				array = new int[(list.Count - 2) * 3];
				int num11 = 0;
				int num12 = 1;
				while (num11 < array.Length)
				{
					array[num11] = 0;
					array[num11 + 1] = num12;
					array[num11 + 2] = num12 + 1;
					num11 += 3;
					num12++;
				}
			}
			elements.Add(new PUIFillElement(list.ToArray(), array, fillColor.Value));
		}
		if (strokeStyle != null)
		{
			DrawPath(list.GetRange(1, list.Count - 2).ToArray(), strokeStyle, closePath: true);
		}
		if (setDirtyOnDraw)
		{
			SetVerticesDirty();
		}
	}

	public void DrawRegularSolid(Vector2 center, float radius, int sides, Color fillColor)
	{
		DrawRegularSolid(center, radius, sides, 0f, fillColor);
	}

	public void DrawRegularSolid(Vector2 center, float radius, int sides, StrokeStyle strokeStyle)
	{
		DrawRegularSolid(center, radius, sides, 0f, null, strokeStyle);
	}

	public void DrawRegularSolid(Vector2 center, float radius, int sides, Color fillColor, StrokeStyle strokeStyle)
	{
		DrawRegularSolid(center, radius, sides, 0f, fillColor, strokeStyle);
	}

	public void DrawRegularSolid(Vector2 center, float radius, int sides, float rotation = 0f, Color? fillColor = null, StrokeStyle strokeStyle = null)
	{
		if (sides < 3)
		{
			Debug.LogError("DrawRegularSolid() requires at least 3 sides.");
		}
		else
		{
			DrawCircle(center, radius, 360f / (float)sides, rotation, 360f + rotation, fillColor, strokeStyle);
		}
	}

	public void DrawIrregularSolid(Vector2 center, float[] radii, Color fillColor)
	{
		DrawIrregularSolid(center, radii, 0f, fillColor, null);
	}

	public void DrawIrregularSolid(Vector2 center, float[] radii, StrokeStyle strokeStyle)
	{
		DrawIrregularSolid(center, radii, 0f, null, strokeStyle);
	}

	public void DrawIrregularSolid(Vector2 center, float[] radii, Color fillColor, StrokeStyle strokeStyle)
	{
		DrawIrregularSolid(center, radii, 0f, fillColor, strokeStyle);
	}

	public void DrawIrregularSolid(Vector2 center, float[] radii, float rotation, Color? fillColor, StrokeStyle strokeStyle)
	{
		int num = radii.Length;
		if (num < 3)
		{
			Debug.LogError("DrawIrregularSolid() requires at least 3 radii.");
			return;
		}
		float num2 = MathF.PI / 2f - rotation * (MathF.PI / 180f);
		float num3 = MathF.PI * 2f / (float)num;
		float num4 = aspectRatio;
		List<Vector2> list = new List<Vector2>();
		list.Add(center);
		for (int i = 0; i < num; i++)
		{
			Vector2 vector = new Vector2(radii[i], radii[i] * num4);
			list.Add(new Vector2(Mathf.Cos(num2 - num3 * (float)i) * vector.x + center.x, Mathf.Sin(num2 - num3 * (float)i) * vector.y + center.y));
		}
		if (fillColor.HasValue)
		{
			int[] array = new int[(list.Count - 1) * 3];
			int num5 = 0;
			int num6 = 1;
			while (num5 < array.Length)
			{
				array[num5] = 0;
				array[num5 + 1] = num6;
				array[num5 + 2] = num6 + 1;
				num5 += 3;
				num6++;
			}
			array[^1] = 1;
			elements.Add(new PUIFillElement(list.ToArray(), array, fillColor.Value));
		}
		if (strokeStyle != null)
		{
			DrawPath(list.GetRange(1, list.Count - 2).ToArray(), strokeStyle, closePath: true);
		}
		if (setDirtyOnDraw)
		{
			SetVerticesDirty();
		}
	}

	public void DrawPolygon(Vector2[] points, Color fillColor)
	{
		DrawPolygon(points, fillColor, null);
	}

	public void DrawPolygon(Vector2[] points, StrokeStyle strokeStyle)
	{
		DrawPath(points, strokeStyle, closePath: true);
	}

	public void DrawPolygon(Vector2[] points, Color fillColor, StrokeStyle strokeStyle)
	{
		if (points.Length < 3)
		{
			Debug.LogError("DrawPolygon() requires at least 3 vertices");
			return;
		}
		if (points.Length == 3)
		{
			points = new Vector2[3]
			{
				points[0],
				points[1],
				points[2]
			};
			elements.Add(new PUIFillElement(points, new int[3] { 0, 1, 2 }, fillColor));
		}
		else if (points.Length == 4)
		{
			elements.Add(new PUIFillElement(points, new int[6] { 0, 1, 2, 2, 3, 0 }, fillColor));
		}
		else
		{
			int[] array = new int[(points.Length - 2) * 3];
			int num = 0;
			LinkedList<Vector2> linkedList = new LinkedList<Vector2>(points);
			List<LinkedListNode<Vector2>> list = new List<LinkedListNode<Vector2>>();
			List<LinkedListNode<Vector2>> list2 = new List<LinkedListNode<Vector2>>();
			List<LinkedListNode<Vector2>> list3 = new List<LinkedListNode<Vector2>>();
			LinkedListNode<Vector2> linkedListNode = linkedList.First;
			LinkedListNode<Vector2> linkedListNode2 = linkedList.First;
			while (linkedListNode.Next != null)
			{
				linkedListNode = linkedListNode.Next;
				if (linkedListNode.Value.x < linkedListNode2.Value.x)
				{
					linkedListNode2 = linkedListNode;
				}
			}
			LinkedListNode<Vector2> linkedListNode3 = linkedListNode2.Previous ?? linkedListNode2.List.Last;
			LinkedListNode<Vector2> linkedListNode4 = linkedListNode2.Next ?? linkedListNode2.List.First;
			Func<Vector2, Vector2, Vector2, bool> func = ((!(linkedListNode4.Value.y > linkedListNode3.Value.y)) ? ((Func<Vector2, Vector2, Vector2, bool>)delegate(Vector2 prev, Vector2 cur, Vector2 next)
			{
				Vector2 vector2 = cur - prev;
				Vector2 rhs2 = next - cur;
				return Vector2.Dot(new Vector2(0f - vector2.y, vector2.x), rhs2) > 0f;
			}) : ((Func<Vector2, Vector2, Vector2, bool>)delegate(Vector2 prev, Vector2 cur, Vector2 next)
			{
				Vector2 vector = cur - prev;
				Vector2 rhs = next - cur;
				return Vector2.Dot(new Vector2(0f - vector.y, vector.x), rhs) < 0f;
			}));
			linkedListNode = linkedList.First;
			for (int i = 0; i < linkedList.Count; i++)
			{
				linkedListNode3 = linkedListNode.Previous ?? linkedListNode.List.Last;
				linkedListNode4 = linkedListNode.Next ?? linkedListNode.List.First;
				if (func(linkedListNode3.Value, linkedListNode.Value, linkedListNode4.Value))
				{
					list2.Add(linkedListNode);
					float triangleArea = PUIUtils.GetTriangleArea(linkedListNode3.Value, linkedListNode.Value, linkedListNode4.Value);
					bool flag = true;
					foreach (LinkedListNode<Vector2> item in list3)
					{
						if (PUIUtils.PointInTriangle(item.Value, linkedListNode3.Value, linkedListNode.Value, linkedListNode4.Value, triangleArea))
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						list.Add(linkedListNode);
					}
				}
				else
				{
					list3.Add(linkedListNode);
				}
				linkedListNode = linkedListNode.Next;
			}
			while (linkedList.Count > 3)
			{
				linkedListNode = list[0];
				linkedListNode3 = linkedListNode.Previous ?? linkedListNode.List.Last;
				linkedListNode4 = linkedListNode.Next ?? linkedListNode.List.First;
				array[num] = Array.IndexOf(points, linkedListNode3.Value);
				array[num + 1] = Array.IndexOf(points, linkedListNode.Value);
				array[num + 2] = Array.IndexOf(points, linkedListNode4.Value);
				num += 3;
				list.Remove(linkedListNode);
				linkedList.Remove(linkedListNode);
				LinkedListNode<Vector2>[] array2 = new LinkedListNode<Vector2>[2] { linkedListNode3, linkedListNode4 };
				foreach (LinkedListNode<Vector2> linkedListNode5 in array2)
				{
					LinkedListNode<Vector2> linkedListNode6 = linkedListNode5.Previous ?? linkedListNode5.List.Last;
					LinkedListNode<Vector2> linkedListNode7 = linkedListNode5.Next ?? linkedListNode5.List.First;
					if (func(linkedListNode6.Value, linkedListNode5.Value, linkedListNode7.Value))
					{
						if (list3.Contains(linkedListNode5))
						{
							list3.Remove(linkedListNode5);
							list2.Add(linkedListNode5);
						}
						float triangleArea2 = PUIUtils.GetTriangleArea(linkedListNode6.Value, linkedListNode5.Value, linkedListNode7.Value);
						bool flag2 = true;
						foreach (LinkedListNode<Vector2> item2 in list3)
						{
							if (PUIUtils.PointInTriangle(item2.Value, linkedListNode6.Value, linkedListNode5.Value, linkedListNode7.Value, triangleArea2))
							{
								flag2 = false;
								break;
							}
						}
						if (flag2 && !list.Contains(linkedListNode5))
						{
							list.Add(linkedListNode5);
						}
						else if (!flag2 && list.Contains(linkedListNode5))
						{
							list.Remove(linkedListNode5);
						}
					}
					else
					{
						list2.Remove(linkedListNode5);
						list3.Add(linkedListNode5);
					}
				}
			}
			array[num] = Array.IndexOf(points, linkedList.First.Value);
			array[num + 1] = Array.IndexOf(points, linkedList.First.Next.Value);
			array[num + 2] = Array.IndexOf(points, linkedList.First.Next.Next.Value);
			elements.Add(new PUIFillElement(points, array, fillColor));
		}
		if (strokeStyle != null)
		{
			DrawPath(points, strokeStyle, closePath: true);
		}
		if (setDirtyOnDraw)
		{
			SetVerticesDirty();
		}
	}

	public void DrawRawMesh(Vector2[] points, int[] triangles, Color fillColor)
	{
		elements.Add(new PUIFillElement(points, triangles, fillColor));
		if (setDirtyOnDraw)
		{
			SetVerticesDirty();
		}
	}

	public void DrawLine(Vector2 point1, Vector2 point2)
	{
		DrawLine(point1, point2, StrokeStyle.defaultStrokeStyle);
	}

	public void DrawLine(Vector2 point1, Vector2 point2, StrokeStyle strokeStyle)
	{
		float strokeThickness = ((strokeStyle.scaleMode == StrokeScaleMode.Absolute) ? (strokeStyle.thickness / base.rectTransform.rect.width) : strokeStyle.thickness);
		elements.Add(new PUIStrokeElement(new Vector2[2] { point1, point2 }, PUIUtils.GetLinePoints(point1, point2, strokeThickness, aspectRatio), strokeStyle, isClosedPath: false));
		if (setDirtyOnDraw)
		{
			SetVerticesDirty();
		}
	}

	public void DrawPath(Vector2[] points)
	{
		DrawPath(points, StrokeStyle.defaultStrokeStyle, closePath: false);
	}

	public void DrawPath(Vector2[] points, StrokeStyle strokeStyle)
	{
		DrawPath(points, strokeStyle, closePath: false);
	}

	public void DrawPath(Vector2[] points, StrokeStyle strokeStyle, bool closePath)
	{
		if (points.Length < 2)
		{
			Debug.LogError("DrawPath() needs at least two points to draw");
			return;
		}
		if (points.Length == 2)
		{
			DrawLine(points[0], points[1], strokeStyle);
			if (closePath)
			{
				Debug.LogWarning("DrawPath() can't close a path with only two points. 'closePath' parameter ignored.");
			}
			return;
		}
		float strokeThickness = ((strokeStyle.scaleMode == StrokeScaleMode.Absolute) ? (strokeStyle.thickness / base.rectTransform.rect.width) : strokeStyle.thickness);
		elements.Add(new PUIStrokeElement(points, PUIUtils.GetPathPoints(points, closePath, strokeThickness, aspectRatio), strokeStyle, closePath));
		if (setDirtyOnDraw)
		{
			SetVerticesDirty();
		}
	}
}
