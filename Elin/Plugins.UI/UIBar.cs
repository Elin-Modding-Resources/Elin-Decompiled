using UnityEngine;
using UnityEngine.UI;

public class UIBar : MonoBehaviour
{
	public Image image;

	public Text textInfo;

	public bool autoHide;

	public void Refresh(float current, float max)
	{
		Refresh(current / max);
	}

	public void Refresh(float progress)
	{
		image.Rect().SetAnchor(0f, 0f, Mathf.Clamp(progress + 0.1f, 0.1f, 1f), 1f);
		if (autoHide)
		{
			if (progress >= 1f && base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(value: false);
			}
			else if (progress < 1f && !base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(value: true);
			}
		}
	}
}
