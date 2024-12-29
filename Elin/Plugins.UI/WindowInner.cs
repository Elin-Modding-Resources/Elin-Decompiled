using UnityEngine;
using UnityEngine.UI;

public class WindowInner : MonoBehaviour, IUISkin
{
	public LayoutGroup layout;

	public UIImage bg;

	public RectTransform rectCorner;

	public UIScrollView scrollView;

	protected void Awake()
	{
		ApplySkin();
	}

	public void ApplySkin()
	{
		SkinRootStatic currentSkin = SkinManager.Instance.currentSkin;
		if ((bool)rectCorner)
		{
			rectCorner.anchoredPosition = currentSkin.positions.innerCorner;
			rectCorner.SetAsLastSibling();
		}
	}
}
