using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PrimitiveUI.Examples;

public class PUIExampleSineWaves : MonoBehaviour
{
	public PrimitiveCanvas primitiveCanvas;

	public Slider sliderPeriod;

	public Slider sliderAmplitude;

	public Slider sliderFrequency;

	public Color bgLinesColor;

	public Color centerLinesColor;

	public int numBgLinesHorizontal;

	public float sampleSize = 0.01f;

	private StrokeStyle sinStrokeStyle;

	private StrokeStyle bgLinesStrokeStyle;

	private StrokeStyle centerLinesStrokeStyle;

	private float lastSineProgress;

	private void Start()
	{
		sliderPeriod.onValueChanged.AddListener(delegate
		{
			Draw();
		});
		sliderAmplitude.onValueChanged.AddListener(delegate
		{
			Draw();
		});
		sliderFrequency.onValueChanged.AddListener(delegate
		{
			Draw();
		});
		sinStrokeStyle = new StrokeStyle(Color.white, 0.025f, StrokeScaleMode.Relative);
		bgLinesStrokeStyle = new StrokeStyle(bgLinesColor, 0.004f, StrokeScaleMode.Relative);
		centerLinesStrokeStyle = new StrokeStyle(centerLinesColor, 0.01f, StrokeScaleMode.Relative);
	}

	private void Update()
	{
		Draw();
	}

	private void Draw()
	{
		primitiveCanvas.Clear();
		float num = bgLinesStrokeStyle.thickness * (float)numBgLinesHorizontal;
		float num2 = (1f - num) / (float)(numBgLinesHorizontal - 1);
		float num3 = bgLinesStrokeStyle.thickness / 2f * (float)(numBgLinesHorizontal - 1);
		for (int i = 0; i < numBgLinesHorizontal; i++)
		{
			float x = num3 + (float)i * num2;
			primitiveCanvas.DrawLine(new Vector2(x, 0f), new Vector2(x, 1f), bgLinesStrokeStyle);
		}
		float num4 = bgLinesStrokeStyle.thickness * primitiveCanvas.aspectRatio;
		num2 *= primitiveCanvas.aspectRatio;
		int num5 = Mathf.CeilToInt(1f / (num4 + num2));
		num3 = num4 / 2f * (float)(num5 - 1);
		for (int j = 0; j < num5; j++)
		{
			float y = num3 + (float)j * num2;
			primitiveCanvas.DrawLine(new Vector2(0f, y), new Vector2(1f, y), bgLinesStrokeStyle);
		}
		primitiveCanvas.DrawLine(new Vector2(0.5f, 0f), new Vector2(0.5f, 1f), centerLinesStrokeStyle);
		primitiveCanvas.DrawLine(new Vector2(0f, 0.5f), new Vector2(1f, 0.5f), centerLinesStrokeStyle);
		float num6 = 0.05f;
		List<Vector2> list = new List<Vector2>();
		for (float num7 = 0f - num6; num7 < 1f + num6; num7 += sampleSize)
		{
			float y2 = Mathf.Sin(MathF.PI * 2f / sliderPeriod.value * num7 * MathF.PI + lastSineProgress) * sliderAmplitude.value / 2f + 0.5f;
			list.Add(new Vector2(num7, y2));
		}
		primitiveCanvas.DrawPath(list.ToArray(), sinStrokeStyle, closePath: false);
		lastSineProgress += Time.deltaTime * sliderFrequency.value;
	}
}
