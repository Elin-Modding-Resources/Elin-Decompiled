using UnityEngine;

public class BaseSkinRoot : MonoBehaviour
{
	public string Name => base.name.Replace("SkinRoot Main ", "");

	public virtual SkinColorProfile Colors => null;

	public virtual SkinColorProfileEx ColorsEx => null;

	public virtual SkinColorProfile GetColors(SkinType type)
	{
		return null;
	}

	public virtual void ApplySkin(UIImage t)
	{
	}

	public virtual void ApplySkin(UIRawImage t)
	{
	}

	public virtual void ApplySkin(UIText t, FontSource f)
	{
	}

	public virtual SkinAsset_Button GetButton()
	{
		return null;
	}
}
