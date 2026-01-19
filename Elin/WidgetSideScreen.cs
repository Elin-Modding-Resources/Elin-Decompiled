using UnityEngine;
using UnityEngine.UI;

public class WidgetSideScreen : Widget
{
	public class Extra
	{
		public int width;

		public int height;

		public int idImage;

		public int bgColor;

		public bool reverse;

		public bool alignUI;
	}

	public static WidgetSideScreen Instance;

	public Sprite[] sprites;

	public Image imagePic;

	public override bool AlwaysBottom => true;

	public Extra extra => base.config.extra as Extra;

	public Color bgColor
	{
		get
		{
			return IntColor.FromInt(extra.bgColor);
		}
		set
		{
			extra.bgColor = IntColor.ToInt(ref value);
		}
	}

	public override object CreateExtra()
	{
		return new Extra();
	}

	public override void OnActivate()
	{
		Layer.blurStopInstance = base.transform;
		Refresh();
		OnChangeResolution();
		Instance = this;
	}

	public override void OnSetContextMenu(UIContextMenu m)
	{
		_ = base.config.skin;
		UIContextMenu uIContextMenu = m.AddChild("setting");
		uIContextMenu.AddToggle("alignUI", extra.alignUI, delegate
		{
			extra.alignUI = !extra.alignUI;
			OnChangeResolution();
		});
		uIContextMenu.AddToggle("reverse", extra.reverse, delegate
		{
			extra.reverse = !extra.reverse;
			OnChangeResolution();
		});
		uIContextMenu.AddSlider("width", (float a) => a.ToString() ?? "", extra.width, delegate(float a)
		{
			extra.width = (int)a;
			OnChangeResolution();
		}, 10f, 50f, isInt: true);
		uIContextMenu.AddButton("colorBG", delegate
		{
			EMono.ui.AddLayer<LayerColorPicker>().SetColor(bgColor, Color.white, delegate(PickerState state, Color _c)
			{
				bgColor = _c;
				OnChangeResolution();
			});
		});
		SetBaseContextMenu(m);
	}

	public void Refresh()
	{
		EMono.scene.cam.rect = (extra.reverse ? new Rect(0.01f * (float)extra.width, 0f, 1f + 0.01f * (float)extra.width, 1f) : new Rect(-0.01f * (float)extra.width, 0f, 1f, 1f));
	}

	public override void OnChangeResolution()
	{
		base.OnChangeResolution();
		RectTransform rectTransform = this.Rect();
		float x = 0.01f * (float)Screen.width * (float)extra.width / EMono.core.uiScale + 50f;
		float y = (float)Screen.height / EMono.core.uiScale + 50f;
		rectTransform.sizeDelta = new Vector2(x, y);
		if (extra.reverse)
		{
			rectTransform.SetAnchor(0f, 0.5f, 0f, 0.5f);
			rectTransform.anchoredPosition = new Vector2(rectTransform.sizeDelta.x * 0.5f, 0f);
		}
		else
		{
			rectTransform.SetAnchor(1f, 0.5f, 1f, 0.5f);
			rectTransform.anchoredPosition = new Vector2(rectTransform.sizeDelta.x * -0.5f, 0f);
		}
		Refresh();
		imagePic.SetActive(enable: false);
		if (extra.alignUI)
		{
			if (extra.reverse)
			{
				EMono.ui.rectLayers.anchoredPosition = new Vector2(rectTransform.sizeDelta.x / 2f, 0f);
				EMono.ui.rectLayers.sizeDelta = new Vector2(0f - rectTransform.sizeDelta.x, 0f);
				SlideMiniGame(rectTransform.sizeDelta.x);
			}
			else
			{
				EMono.ui.rectLayers.anchoredPosition = new Vector2((0f - rectTransform.sizeDelta.x) / 2f, 0f);
				EMono.ui.rectLayers.sizeDelta = new Vector2(0f - rectTransform.sizeDelta.x, 0f);
				SlideMiniGame(0f - rectTransform.sizeDelta.x);
			}
		}
		else
		{
			EMono.ui.rectLayers.anchoredPosition = Vector2.zero;
			EMono.ui.rectLayers.sizeDelta = Vector2.zero;
			SlideMiniGame(0f);
		}
	}

	public void SlideMiniGame(float w)
	{
		if ((bool)LayerMiniGame.Instance)
		{
			LayerMiniGame.Instance.mini.SlidePosition(w * EMono.core.uiScale);
		}
	}

	private void OnEnable()
	{
		OnChangeResolution();
	}

	private void OnDisable()
	{
		EMono.scene.cam.rect = new Rect(0f, 0f, 1f, 1f);
		EMono.ui.rectLayers.anchoredPosition = Vector2.zero;
		EMono.ui.rectLayers.sizeDelta = Vector2.zero;
		SlideMiniGame(0f);
	}
}
