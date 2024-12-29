using UnityEngine;
using UnityEngine.UI;

public class RenderCamUpscaler : MonoBehaviour, IChangeResolution
{
	public static RenderCamUpscaler Instance;

	public RenderTexture renderTex;

	public RawImage image;

	public bool active;

	private void Awake()
	{
		Instance = this;
	}

	public void SetEnable(bool enable)
	{
		bool flag2 = (image.enabled = enable);
		active = flag2;
		if (active)
		{
			OnChangeResolution();
			return;
		}
		if ((bool)renderTex)
		{
			renderTex.Release();
		}
		Camera.main.targetTexture = null;
	}

	public void OnChangeResolution()
	{
		if (active)
		{
			int num = Screen.width / 2;
			int num2 = Screen.height / 2;
			if (!renderTex || num != renderTex.width || num2 != renderTex.height)
			{
				CreateTex(num, num2);
			}
			image.Rect().sizeDelta = new Vector2(num * 2, num2 * 2);
		}
	}

	public void CreateTex(int w, int h)
	{
		if ((bool)renderTex)
		{
			renderTex.Release();
		}
		renderTex = new RenderTexture(w, h, 32);
		image.texture = renderTex;
		renderTex.filterMode = FilterMode.Point;
		Camera.main.targetTexture = renderTex;
	}
}
