using UnityEngine;
using UnityEngine.UI;

public class UIImage : Image, IUISkin
{
	public bool useSkinSize = true;

	public ImageType imageType;

	public FrameType frameType;

	public BaseSkinRoot skinRoot;

	public Vector2 sizeFix;

	public Image frame;

	protected override void Awake()
	{
		ApplySkin();
		base.Awake();
	}

	public void ApplySkin()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		BaseSkinRoot baseSkinRoot = skinRoot ?? SkinManager.Instance.currentSkin;
		baseSkinRoot.ApplySkin(this);
		if (imageType == ImageType.Line)
		{
			color = baseSkinRoot.Colors.colorLine;
		}
		if (frameType != 0)
		{
			if (!frame)
			{
				frame = Util.Instantiate<Image>("UI/Element/Other/BGFrame", this);
				frame.transform.SetParent(base.transform.parent, worldPositionStays: false);
				frame.transform.SetSiblingIndex(base.transform.GetSiblingIndex());
			}
			SkinRootStatic skinRootStatic = baseSkinRoot as SkinRootStatic;
			if ((bool)skinRootStatic && skinRootStatic.useFrame)
			{
				frame.sprite = skinRootStatic.assets.frame1.sprite;
				if (useSkinSize)
				{
					frame.Rect().sizeDelta = skinRootStatic.assets.frame1.size;
				}
				frame.SetActive(enable: true);
			}
			else
			{
				frame.SetActive(enable: false);
			}
		}
		else if ((bool)frame)
		{
			frame.SetActive(enable: false);
		}
	}

	protected override void OnPopulateMesh(VertexHelper vh)
	{
		if (color.a == 0f || !base.sprite)
		{
			vh.Clear();
		}
		else
		{
			base.OnPopulateMesh(vh);
		}
	}
}
