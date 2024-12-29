using UnityEngine;
using UnityEngine.UI;

namespace PrimitiveUI.Examples;

public class PUIExampleCharts : MonoBehaviour
{
	public PrimitiveCanvas primitiveCanvas;

	public Slider sliderAka;

	public Slider sliderBlauw;

	public Slider sliderGelb;

	public Slider sliderVerde;

	[Range(0.1f, 0.25f)]
	public float barChartBarWidth = 0.2f;

	private Rect barChartBounds = new Rect(0.45f, 0.05f, 0.5f, 0.5f);

	private Vector2 pieChartCenter = new Vector2(0.7f, 0.8f);

	private float pieChartRadius = 0.1f;

	private Color red = new Color(1f, 0.196f, 0f);

	private Color blue = new Color(0f, 0.47f, 1f);

	private Color yellow = new Color(1f, 0.92f, 0f);

	private Color green = new Color(0f, 1f, 0.176f);

	private StrokeStyle barChartAxisStroke;

	private StrokeStyle barChartBgStroke;

	private void Start()
	{
		sliderAka.onValueChanged.AddListener(delegate
		{
			Draw();
		});
		sliderBlauw.onValueChanged.AddListener(delegate
		{
			Draw();
		});
		sliderGelb.onValueChanged.AddListener(delegate
		{
			Draw();
		});
		sliderVerde.onValueChanged.AddListener(delegate
		{
			Draw();
		});
		barChartAxisStroke = new StrokeStyle(Color.black, 0.006f, StrokeScaleMode.Relative);
		barChartBgStroke = new StrokeStyle(new Color(0f, 0f, 0f, 0.2f), 0.0025f, StrokeScaleMode.Relative);
		Draw();
	}

	private void Draw()
	{
		primitiveCanvas.Clear();
		float num = barChartBounds.height / 9f;
		for (int i = 0; i < 8; i++)
		{
			float y = barChartBounds.yMin + (float)(i + 1) * num;
			primitiveCanvas.DrawLine(new Vector2(barChartBounds.xMin, y), new Vector2(barChartBounds.xMax, y), barChartBgStroke);
		}
		float num2 = barChartBounds.width * barChartBarWidth;
		float num3 = (barChartBounds.width - num2 * 4f) / 5f;
		Rect rect = new Rect(barChartBounds.xMin + num3 * 1f, barChartBounds.yMin, num2, sliderAka.value * barChartBounds.height * 0.01f);
		primitiveCanvas.DrawRectangle(rect, red);
		rect = new Rect(barChartBounds.xMin + num3 * 2f + num2 * 1f, barChartBounds.yMin, num2, sliderBlauw.value * barChartBounds.height * 0.01f);
		primitiveCanvas.DrawRectangle(rect, blue);
		rect = new Rect(barChartBounds.xMin + num3 * 3f + num2 * 2f, barChartBounds.yMin, num2, sliderGelb.value * barChartBounds.height * 0.01f);
		primitiveCanvas.DrawRectangle(rect, yellow);
		rect = new Rect(barChartBounds.xMin + num3 * 4f + num2 * 3f, barChartBounds.yMin, num2, sliderVerde.value * barChartBounds.height * 0.01f);
		primitiveCanvas.DrawRectangle(rect, green);
		Vector2[] points = new Vector2[3]
		{
			new Vector2(barChartBounds.xMin, barChartBounds.yMax),
			barChartBounds.min,
			new Vector2(barChartBounds.xMax, barChartBounds.yMin)
		};
		primitiveCanvas.DrawPath(points, barChartAxisStroke, closePath: false);
		float num4 = sliderAka.value + sliderBlauw.value + sliderGelb.value + sliderVerde.value;
		float num5 = 0f;
		float num6 = sliderAka.value / num4 * 360f;
		primitiveCanvas.DrawCircle(pieChartCenter, pieChartRadius, 1f, num5, num5 + num6, red);
		num5 += num6;
		num6 = sliderBlauw.value / num4 * 360f;
		primitiveCanvas.DrawCircle(pieChartCenter, pieChartRadius, 1f, num5, num5 + num6, blue);
		num5 += num6;
		num6 = sliderGelb.value / num4 * 360f;
		primitiveCanvas.DrawCircle(pieChartCenter, pieChartRadius, 1f, num5, num5 + num6, yellow);
		num5 += num6;
		num6 = sliderVerde.value / num4 * 360f;
		primitiveCanvas.DrawCircle(pieChartCenter, pieChartRadius, 1f, num5, num5 + num6, green);
		primitiveCanvas.DrawCircle(pieChartCenter, pieChartRadius, 1f, 0f, 1f, red);
	}
}
