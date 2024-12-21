using System;
using UnityEngine;

[Serializable]
public class LightData
{
	public Color color = new Color(1f, 1f, 1f, 25f / 64f);

	[Range(1f, 12f)]
	public int radius = 4;
}
