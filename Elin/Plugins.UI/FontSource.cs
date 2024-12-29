using UnityEngine;

public class FontSource : ScriptableObject
{
	public string _name;

	public string id;

	public Font font;

	public int sizeFix;

	public int importSize;

	public bool alwaysBold;

	public float boldAlpha = 1f;

	public float contrast;

	public float strength;

	public float strengthFixJP;

	public float lineSpacing = 1f;
}
