using System;
using UnityEngine;

namespace PrimitiveUI;

[Serializable]
public class StrokeStyle
{
	public Color color;

	public float thickness;

	public StrokeScaleMode scaleMode;

	private static StrokeStyle defaultStrokeStyleInstance;

	public static StrokeStyle defaultStrokeStyle
	{
		get
		{
			if (defaultStrokeStyleInstance == null)
			{
				defaultStrokeStyleInstance = new StrokeStyle(Color.white, 0.04f, StrokeScaleMode.Relative);
			}
			return defaultStrokeStyleInstance;
		}
	}

	public StrokeStyle(Color color, float thickness, StrokeScaleMode scaleMode)
	{
		this.color = color;
		this.thickness = thickness;
		this.scaleMode = scaleMode;
	}
}
