using UnityEngine;
using UnityEngine.UI;

public static class CanvasExt
{
	public static Vector3 CorrectLossyScale(this Canvas canvas)
	{
		if (!Application.isPlaying)
		{
			return Vector3.one;
		}
		if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
		{
			CanvasScaler component = canvas.GetComponent<CanvasScaler>();
			if ((bool)component && component.enabled)
			{
				component.enabled = false;
				Vector3 lossyScale = canvas.GetComponent<RectTransform>().lossyScale;
				component.enabled = true;
				Vector3 lossyScale2 = canvas.GetComponent<RectTransform>().lossyScale;
				return new Vector3(lossyScale2.x / lossyScale.x, lossyScale2.y / lossyScale.y, lossyScale2.z / lossyScale.z);
			}
			return Vector3.one;
		}
		return canvas.GetComponent<RectTransform>().lossyScale;
	}
}
