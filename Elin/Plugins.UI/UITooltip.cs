using System;
using UnityEngine;
using UnityEngine.UI;

public class UITooltip : MonoBehaviour
{
	public enum FollowType
	{
		None,
		Button,
		Mouse,
		Pivot
	}

	public TooltipData data;

	public UIText textMain;

	public UIText textSub;

	public UINote note;

	public Image imageMain;

	public Image imageSub;

	public Image icon;

	public Vector3 offset;

	public FollowType followType;

	public bool follow;

	public bool delayHide;

	public bool altSkin;

	public Vector2 orgPivot;

	public CanvasGroup cg;

	public Func<bool> hideFunc;

	public LayoutElement layout;

	[NonSerialized]
	public float hideTimer;

	private void Awake()
	{
		orgPivot = this.Rect().pivot;
	}

	public void SetData(TooltipData _data)
	{
		bool flag = data != _data;
		data = _data;
		if ((bool)note)
		{
			this.Rect().sizeDelta = new Vector2(500f, 0f);
			if (altSkin && flag)
			{
				SkinManager.tempSkin = SkinManager.CurrentSkin.colors.skinTooltip;
			}
		}
		if ((bool)layout)
		{
			layout.enabled = true;
		}
		if ((bool)textMain)
		{
			textMain.text = (data.lang.IsEmpty() ? data.text : Lang.Get(data.lang));
		}
		if (data.onShowTooltip != null)
		{
			data.onShowTooltip(this);
		}
		if ((bool)icon)
		{
			icon.SetActive(data.icon);
		}
		UIImage[] componentsInChildren = GetComponentsInChildren<UIImage>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].ApplySkin();
		}
		this.RebuildLayout(recursive: true);
	}
}
