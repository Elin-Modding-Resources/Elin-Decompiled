using UnityEngine;
using UnityEngine.UI;

public class UIMouseInfo : MonoBehaviour
{
	public UIText text;

	public UIText textCost;

	public Image imageRange;

	public string willShow;

	private void OnEnable()
	{
		textCost.SetActive(enable: false);
		RefreshPosition();
	}

	private void LateUpdate()
	{
		RefreshPosition();
	}

	public void RefreshPosition()
	{
		base.transform.position = Input.mousePosition;
		if (CursorSystem.ignoreCount <= 0)
		{
			if (willShow != null)
			{
				text.text = willShow;
				willShow = null;
			}
			if (text.text.IsEmpty())
			{
				this.SetActive(enable: false);
			}
		}
	}

	public void SetText(string t = "")
	{
		if (CursorSystem.ignoreCount <= 0)
		{
			text.text = t;
			willShow = null;
			this.SetActive(!t.IsEmpty());
		}
		else
		{
			willShow = t;
		}
	}
}
