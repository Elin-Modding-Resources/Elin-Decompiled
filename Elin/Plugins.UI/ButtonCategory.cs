using System;
using UnityEngine;
using UnityEngine.UI;

public class ButtonCategory : UIButton
{
	public UIButton buttonFold;

	public UIButton buttonToggle;

	public Sprite spriteFold;

	public Sprite spriteUnfold;

	public Toggle toggle;

	public bool isFolded;

	public int uid;

	public void SetFold(bool show, bool folded)
	{
		buttonFold.SetActive(show);
		buttonFold.icon.sprite = (folded ? spriteFold : spriteUnfold);
		isFolded = folded;
	}

	public void SetFold(bool show, bool folded, Action<UIList> action)
	{
		isFolded = folded;
		buttonFold.SetActive(show);
		buttonFold.icon.sprite = (folded ? spriteFold : spriteUnfold);
		if (!show)
		{
			return;
		}
		buttonFold.onClick.AddListener(delegate
		{
			SetFold(show: true, GetComponentInParent<UIList>().OnClickFolder(this, delegate(UIList l)
			{
				action(l);
			}));
		});
		if (!folded)
		{
			GetComponentInParent<UIList>().OnClickFolder(this, delegate(UIList l)
			{
				action(l);
			}, refresh: false);
		}
	}
}
