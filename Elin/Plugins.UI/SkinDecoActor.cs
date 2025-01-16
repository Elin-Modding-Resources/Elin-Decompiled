using UnityEngine;
using UnityEngine.UI;

public class SkinDecoActor : MonoBehaviour
{
	public SkinDeco owner;

	public Image image;

	public Shadow shadow;

	public void Refresh()
	{
		image.SetNativeSize();
		Vector2 sizeDelta = this.Rect().sizeDelta;
		this.Rect().sizeDelta = new Vector2(sizeDelta.x * (float)owner.sx * 0.01f, sizeDelta.y * (float)owner.sy * 0.01f);
		image.color = owner.color;
		base.transform.localEulerAngles = new Vector3(0f, 0f, owner.rz * 45);
		base.transform.localScale = new Vector3((!owner.reverse) ? 1 : (-1), 1f, 1f);
		shadow.enabled = owner.shadow;
		shadow.effectDistance = new Vector2(owner.reverse ? (-3) : 3, -3f);
		if (owner.bottom)
		{
			base.transform.SetSiblingIndex(1);
		}
	}
}
