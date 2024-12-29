using System;
using UnityEngine;

[Serializable]
public class CursorInfo
{
	public string Name;

	public Sprite sprite;

	public Vector2 Hotspot;

	public Texture2D Texture => sprite.texture;
}
