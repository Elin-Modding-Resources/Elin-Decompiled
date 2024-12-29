using System;
using UnityEngine;

namespace PrimitiveUI.Examples;

public class PUIExampleRPGStats : MonoBehaviour
{
	public PrimitiveCanvas primitiveCanvas;

	public Color chartColorBackground;

	public Color chartColorFill;

	public Color chartColorStroke;

	public float[] stats = new float[5];

	public StrokeStyle strokeOutline;

	public StrokeStyle strokeMarks;

	public StrokeStyle strokeSegment;

	private Vector2 chartCenter = new Vector2(0.7f, 0.5f);

	private float chartRadius = 0.2f;

	private float fix1 = 72.5f;

	private Color chartColorStrokeInner;

	private StrokeStyle chartOuterStroke;

	private StrokeStyle chartMarksStroke;

	private StrokeStyle chartSegmentsStroke;

	private void Awake()
	{
		chartColorStrokeInner = new Color(chartColorStroke.r, chartColorStroke.g, chartColorStroke.b, chartColorStroke.a * 0.5f);
		chartOuterStroke = strokeOutline;
		chartMarksStroke = strokeMarks;
		chartSegmentsStroke = strokeSegment;
	}

	public void Draw()
	{
		float[] array = new float[5];
		for (int i = 0; i < 5; i++)
		{
			array[i] = stats[i] / 100f * chartRadius;
		}
		primitiveCanvas.Clear();
		primitiveCanvas.DrawRegularSolid(chartCenter, chartRadius, 5, chartColorBackground, chartOuterStroke);
		primitiveCanvas.DrawRegularSolid(chartCenter, chartRadius / 3f, 5, chartMarksStroke);
		primitiveCanvas.DrawRegularSolid(chartCenter, chartRadius / 3f * 2f, 5, chartMarksStroke);
		Vector2 vector = new Vector2(chartRadius, chartRadius * primitiveCanvas.aspectRatio);
		float num = MathF.PI * 2f / 5f;
		float num2 = MathF.PI / 2f - fix1 * (MathF.PI / 180f);
		for (int j = 0; j < 5; j++)
		{
			Vector2 point = new Vector2(Mathf.Cos(num2 - num * (float)j) * vector.x + chartCenter.x, Mathf.Sin(num2 - num * (float)j) * vector.y + chartCenter.y);
			primitiveCanvas.DrawLine(chartCenter, point, chartSegmentsStroke);
		}
		primitiveCanvas.DrawIrregularSolid(chartCenter, array, chartColorFill);
	}
}
