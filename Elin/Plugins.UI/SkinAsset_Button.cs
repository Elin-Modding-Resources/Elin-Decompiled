using UnityEngine;
using UnityEngine.UI;

public class SkinAsset_Button : SkinAsset
{
	public Selectable.Transition transition = Selectable.Transition.SpriteSwap;

	public Sprite spriteNormal;

	public Sprite spriteHighlight;

	public SkinColorProfile colorProf;

	public Color color;

	public Color textColor;

	public Color textShadowColor;

	public Color colorHighlight;

	public bool textShadow;

	public Vector2Int size;

	public Vector2Int spacing;
}
