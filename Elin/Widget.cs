using System;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class Widget : EMono, IChangeResolution, ISkinRoot
{
	[Serializable]
	public class Meta
	{
		public string id;

		public bool enabled;

		public bool locked;

		public bool debugOnly;

		public bool system;

		public WidgetType type;

		public GameMode mode;

		public override string ToString()
		{
			return id;
		}
	}

	public enum WidgetType
	{
		Default,
		ZoomMenu
	}

	public class Config
	{
		public State state;

		public RectPosition anchor;

		public RectPosition userAnchor;

		public RectPosition pivot = RectPosition.Center;

		public string id;

		public string titleLang;

		public float x;

		public float y;

		public bool locked;

		public bool RC;

		public object extra;

		public SkinConfig skin = new SkinConfig();

		[JsonIgnore]
		public bool valid;

		[JsonIgnore]
		public bool annoyPlayer;

		[JsonIgnore]
		public Meta meta;

		[JsonIgnore]
		public GameMode mode => meta.mode;

		[JsonIgnore]
		public bool IsSealed
		{
			get
			{
				if (IsSystem)
				{
					return !EMono.core.config.test.unsealWidgets;
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool IsSystem
		{
			get
			{
				if (!meta.system)
				{
					return annoyPlayer;
				}
				return true;
			}
		}

		public bool IsInRightMode()
		{
			ActionMode actionMode = EMono.scene.actionMode;
			if (actionMode.ShowBuildWidgets)
			{
				return mode.HasFlag(GameMode.Build);
			}
			if (actionMode == ActionMode.NoMap || actionMode == ActionMode.NewZone || actionMode == ActionMode.ViewMap)
			{
				return mode.HasFlag(GameMode.NoMap);
			}
			if (actionMode == ActionMode.Bird)
			{
				return mode.HasFlag(GameMode.Bird);
			}
			if (ActionMode.DefaultMode == ActionMode.Sim)
			{
				return mode.HasFlag(GameMode.Sim);
			}
			if (ActionMode.DefaultMode == ActionMode.EloMap)
			{
				return mode.HasFlag(GameMode.EloMap);
			}
			if (ActionMode.IsAdv || ActionMode.DefaultMode == ActionMode.View)
			{
				if (actionMode == ActionMode.Region && !mode.HasFlag(GameMode.EloMap))
				{
					return false;
				}
				if (!ActionMode.Adv.zoomOut && meta.type == WidgetType.ZoomMenu)
				{
					return false;
				}
				return mode.HasFlag(GameMode.Adv);
			}
			return false;
		}
	}

	public enum State
	{
		Active,
		Inactive
	}

	private string _ID;

	private Config _config;

	public Image imageBG;

	public SoundData soundActivate;

	public UIDragPanel dragPanel;

	private RectTransform _rect;

	public RectTransform tipPivotLeft;

	public RectTransform tipPivotRight;

	private SkinRoot _skinRoot;

	protected bool flip;

	private GameObject goCover;

	public string ID
	{
		get
		{
			if (_ID == null)
			{
				return _ID = base.name.Replace("Widget", "").Replace("(Clone)", "");
			}
			return _ID;
		}
	}

	public Config config => _config ?? (_config = EMono.player.widgets.dict[ID]);

	public SkinRoot skinRoot => _skinRoot ?? (_skinRoot = GetComponent<SkinRoot>());

	public bool IsSealed
	{
		get
		{
			if (config.meta.system)
			{
				return !EMono.core.config.test.unsealWidgets;
			}
			return false;
		}
	}

	public virtual bool ShowStyleMenu => true;

	public virtual bool AlwaysTop => false;

	public virtual bool IsStaticPosition => false;

	public virtual bool AlwaysBottom => false;

	public virtual Type SetSiblingAfter => null;

	public virtual bool ShowInBuildMode => config.mode.HasFlag(GameMode.Build);

	public virtual bool RightClickToClose => config.RC;

	public virtual bool AllowRightClickToClose => false;

	public void Test()
	{
	}

	public bool IsInRightMode()
	{
		if (EMono.scene.actionMode.ShowBuildWidgets && ShowInBuildMode)
		{
			return true;
		}
		return config.IsInRightMode();
	}

	public void Activate()
	{
		_rect = this.Rect();
		config.state = State.Active;
		if (config.extra == null)
		{
			config.extra = CreateExtra();
		}
		if (!dragPanel)
		{
			dragPanel = GetComponentInChildren<UIDragPanel>();
		}
		if (IsStaticPosition)
		{
			dragPanel.enable = false;
		}
		else
		{
			dragPanel.autoAnchor = config.userAnchor == RectPosition.Auto;
			if (config.userAnchor != 0)
			{
				_rect.SetAnchor(config.userAnchor);
			}
			else
			{
				_rect.SetAnchor(config.anchor);
			}
			_rect.anchoredPosition = new Vector2(config.x, config.y);
			dragPanel.onDrag = OnChangePosition;
		}
		SetPivot(config.pivot);
		OnActivate();
		if (this == null)
		{
			return;
		}
		flip = _rect.position.x > (float)Screen.width * 0.5f;
		OnFlip();
		OnChangePosition();
		ClampToScreen();
		foreach (SkinDeco deco in config.skin.decos)
		{
			InstantiateDeco(deco);
		}
		RefreshOrder();
	}

	public void RefreshOrder()
	{
		if (!AlwaysBottom)
		{
			return;
		}
		Type setSiblingAfter = SetSiblingAfter;
		bool flag = false;
		if (setSiblingAfter != null)
		{
			foreach (Widget item in EMono.ui.widgets.list)
			{
				if (item.GetType() == setSiblingAfter)
				{
					base.transform.SetSiblingIndex(item.transform.GetSiblingIndex() + 1);
					flag = true;
					break;
				}
			}
		}
		if (!flag)
		{
			base.transform.SetAsFirstSibling();
		}
	}

	public virtual void OnActivate()
	{
	}

	public virtual object CreateExtra()
	{
		return null;
	}

	public void Deactivate()
	{
		OnDeactivate();
		config.state = State.Inactive;
		UpdateConfig();
		UnityEngine.Object.DestroyImmediate(base.gameObject);
	}

	public virtual void OnDeactivate()
	{
	}

	public void Close()
	{
		EMono.ui.widgets.DeactivateWidget(this);
	}

	public void Reactivate()
	{
		EMono.ui.widgets.DeactivateWidget(this);
		EMono.ui.widgets.ActivateWidget(config);
	}

	public virtual void OnManagerActivate()
	{
		goCover = Util.Instantiate("UI/Widget/CoverWidget", base.transform).gameObject;
		RectTransform rectTransform = goCover.transform.Rect();
		RectTransform rectTransform2 = dragPanel.Rect();
		if ((bool)rectTransform && (bool)rectTransform2)
		{
			rectTransform.pivot = rectTransform2.pivot;
			rectTransform.anchorMin = rectTransform2.anchorMin;
			rectTransform.anchorMax = rectTransform2.anchorMax;
			rectTransform.anchoredPosition = rectTransform2.anchoredPosition;
			rectTransform.sizeDelta = rectTransform2.sizeDelta;
		}
		UIDragPanel componentInChildren = goCover.GetComponentInChildren<UIDragPanel>();
		componentInChildren.autoAnchor = config.userAnchor == RectPosition.Auto;
		componentInChildren.onDrag = OnChangePosition;
		componentInChildren.container = dragPanel.container;
		componentInChildren.target = dragPanel.target;
		componentInChildren.bound = dragPanel.bound;
		if (this is WidgetSideScreen)
		{
			componentInChildren.enable = false;
		}
		goCover.GetComponentInChildren<UIText>().SetText(("Widget" + ID).lang());
	}

	public virtual void OnManagerDeactivate()
	{
		UnityEngine.Object.DestroyImmediate(goCover);
	}

	public void SetAnchor(RectPosition p)
	{
		config.userAnchor = p;
		dragPanel.autoAnchor = p == RectPosition.Auto;
		_rect.SetAnchor(p);
	}

	public void SetPivot(RectPosition p)
	{
		config.pivot = p;
		RectTransform rectTransform = this.Rect();
		Vector3 position = base.transform.position;
		switch (p)
		{
		case RectPosition.TopCenter:
			rectTransform.pivot = new Vector2(0.5f, 1f);
			break;
		case RectPosition.BottomCenter:
			rectTransform.pivot = new Vector2(0.5f, 0f);
			break;
		case RectPosition.TopLEFT:
			rectTransform.pivot = new Vector2(0f, 1f);
			break;
		case RectPosition.BottomLEFT:
			rectTransform.pivot = new Vector2(0f, 0f);
			break;
		case RectPosition.TopRIGHT:
			rectTransform.pivot = new Vector2(1f, 1f);
			break;
		case RectPosition.BottomRIGHT:
			rectTransform.pivot = new Vector2(1f, 0f);
			break;
		case RectPosition.Left:
			rectTransform.pivot = new Vector2(0f, 0.5f);
			break;
		case RectPosition.Right:
			rectTransform.pivot = new Vector2(1f, 0.5f);
			break;
		default:
			rectTransform.pivot = new Vector2(0.5f, 0.5f);
			break;
		}
		base.transform.position = position;
		OnChangePivot();
		ClampToScreen();
	}

	public virtual void OnChangePivot()
	{
	}

	public void UpdateConfig()
	{
		config.anchor = _rect.GetAnchor();
		config.x = _rect.anchoredPosition.x;
		config.y = _rect.anchoredPosition.y;
		OnUpdateConfig();
	}

	public virtual void OnUpdateConfig()
	{
	}

	protected void ClampToScreenEnsured(Component c, Vector2 anchoredPos)
	{
		RectTransform rectTransform = c.Rect();
		rectTransform.anchoredPosition = anchoredPos;
		rectTransform.RebuildLayout();
		rectTransform.ForceUpdateRectTransforms();
		ClampToScreen(rectTransform);
		rectTransform.anchoredPosition = anchoredPos;
		rectTransform.RebuildLayout();
		rectTransform.ForceUpdateRectTransforms();
		ClampToScreen(rectTransform);
	}

	public void ClampToScreen()
	{
		ClampToScreen(this.Rect());
	}

	protected void ClampToScreen(RectTransform rect, float margin = 10f)
	{
		if (!IsStaticPosition && config.userAnchor == RectPosition.Auto)
		{
			Vector3 position = rect.position;
			Vector2 vector = new Vector2(margin, margin);
			Vector2 vector2 = new Vector2((float)Screen.width - margin, (float)Screen.height - margin);
			Vector3 vector3 = vector - rect.rect.min;
			Vector3 vector4 = vector2 - rect.rect.max;
			position.x = Mathf.Clamp(position.x, vector3.x - 20f, vector4.x + 20f);
			position.y = Mathf.Clamp(position.y, vector3.y - 20f, vector4.y + 20f);
			rect.position = position;
		}
	}

	public virtual void OnChangeResolution()
	{
		ClampToScreen(this.Rect());
	}

	public void OnChangePosition()
	{
		bool flag = this.Rect().position.x > (float)Screen.width * 0.5f;
		if (flip != flag)
		{
			flip = flag;
			OnFlip();
		}
		RefreshTipPivotPosition();
	}

	public virtual void OnChangeActionMode()
	{
		this.SetActive(IsInRightMode());
	}

	public virtual void OnFlip()
	{
	}

	public bool IsAlignTop()
	{
		RectPosition anchor = this.Rect().GetAnchor();
		if (anchor != RectPosition.TopCenter && anchor != RectPosition.TopLEFT)
		{
			return anchor == RectPosition.TopRIGHT;
		}
		return true;
	}

	public Layer AddLayer(Layer l, Transform trans)
	{
		l.OnBeforeAddLayer();
		l.gameObject.SetActive(value: true);
		l.transform.SetParent(trans, worldPositionStays: false);
		l.Init();
		l.OnAfterAddLayer();
		Window window = l.windows[0];
		RectTransform rectTransform = l.Rect();
		RectTransform rectTransform2 = window.Rect();
		rectTransform.sizeDelta = window.Rect().sizeDelta;
		rectTransform.SetPivot(0.5f, 0.5f);
		rectTransform.SetAnchor(0.5f, 0.5f, 0.5f, 0.5f);
		rectTransform2.SetPivot(0.5f, 0.5f);
		rectTransform2.SetAnchor(0.5f, 0.5f, 0.5f, 0.5f);
		rectTransform2.anchoredPosition = Vector2.zero;
		window.setting.allowMove = (window.setting.allowResize = false);
		window.setting.saveWindow = false;
		window.bgCollider.SetActive(enable: false);
		return l;
	}

	public void RefreshTipPivotPosition()
	{
		bool flag = (float)(Screen.width / 2 - 40) > base.transform.position.x;
		if ((bool)tipPivotLeft)
		{
			tipPivotLeft.SetActive(!flag || !tipPivotRight);
		}
		if ((bool)tipPivotRight)
		{
			tipPivotRight.SetActive(flag || !tipPivotLeft);
		}
	}

	public SkinSet GetSkin()
	{
		return SkinManager.Instance.skinSets[config.skin.id];
	}

	public SkinConfig GetSkinConfig()
	{
		return config.skin;
	}

	public void SetSkin(int id, int v = 0)
	{
		SkinConfig skin = config.skin;
		if (skin.id != id)
		{
			skin.SetID(id);
		}
		skin.id = id;
		skin.bg = v;
		skin.bgColor = skin.BG.color;
		ApplySkin();
	}

	public void TestSkin()
	{
		config.skin.SetID();
		ApplySkin();
	}

	public virtual void ApplySkin()
	{
		SkinRoot component = GetComponent<SkinRoot>();
		if ((bool)component)
		{
			component.Reset();
			IUISkin[] componentsInChildren = GetComponentsInChildren<IUISkin>(includeInactive: true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].ApplySkin();
			}
			OnApplySkin();
		}
	}

	public virtual void OnApplySkin()
	{
	}

	public void InstantiateDeco(SkinDeco deco)
	{
		SkinDecoActor skinDecoActor = (deco.actor = Util.Instantiate<SkinDecoActor>("UI/Widget/SkinDecoActor", base.transform));
		skinDecoActor.owner = deco;
		skinDecoActor.image.sprite = ResourceCache.Load<Sprite>("Media/Graphics/Deco/deco" + deco.id);
		skinDecoActor.Rect().anchoredPosition = new Vector2(deco.x, deco.y);
		skinDecoActor.Refresh();
	}

	public void AddDeco(SkinDeco deco)
	{
		config.skin.decos.Add(deco);
		InstantiateDeco(deco);
	}

	public void RemoveDeco(SkinDeco deco)
	{
		config.skin.decos.Remove(deco);
		UnityEngine.Object.DestroyImmediate(deco.actor.gameObject);
	}

	public virtual bool CanShowContextMenu()
	{
		return true;
	}

	public void ShowContextMenu()
	{
		UIContextMenu uIContextMenu = EMono.ui.CreateContextMenu();
		OnSetContextMenu(uIContextMenu);
		uIContextMenu.Show();
	}

	public void SetBaseContextMenu(UIContextMenu m)
	{
		UIContextMenu uIContextMenu = m.AddChild("anchor");
		SkinConfig cfg = config.skin;
		foreach (RectPosition p in Util.EnumToList<RectPosition>())
		{
			uIContextMenu.AddButton(((config.userAnchor == p) ? "★ " : "") + p.ToString().lang(), delegate
			{
				SetAnchor(p);
				SE.ClickGeneral();
				m.Hide();
			});
		}
		uIContextMenu = m.AddChild("pivot");
		foreach (RectPosition p2 in Util.EnumToList<RectPosition>())
		{
			uIContextMenu.AddButton(((config.pivot == p2) ? "★ " : "") + p2.ToString().lang(), delegate
			{
				SetPivot(p2);
				SE.ClickGeneral();
				m.Hide();
			});
		}
		UIContextMenuItem sliderB = null;
		Action Refresh = delegate
		{
			int num = EMono.ui.skins.skinSets[cfg.id].bgs.Count - 1;
			sliderB.slider.maxValue = num;
			sliderB.slider.value = cfg.bg;
			sliderB.textSlider.text = cfg.bg + "/" + (EMono.ui.skins.skinSets[cfg.id].bgs.Count - 1);
		};
		if (ShowStyleMenu)
		{
			UIContextMenu uIContextMenu2 = m.AddOrGetChild("style");
			sliderB = uIContextMenu2.AddSlider("changeBG", (float n) => n + "/" + (EMono.ui.skins.skinSets[cfg.id].bgs.Count - 1), cfg.bg, delegate(float a)
			{
				SetSkin(cfg.id, (int)a);
				Refresh();
			}, 0f, EMono.ui.skins.skinSets[cfg.id].bgs.Count - 1, isInt: true);
			uIContextMenu2.AddSlider("bgSize", (float n) => n.ToString() ?? "", cfg.bgSize, delegate(float a)
			{
				cfg.bgSize = (int)a;
				ApplySkin();
			}, -50f, 50f, isInt: true);
			uIContextMenu2.AddButton("colorBG", delegate
			{
				if (EMono.ui.skins.skinSets[cfg.id].bgs[cfg.bg].redirect != 0)
				{
					SE.Beep();
				}
				else
				{
					EMono.ui.AddLayer<LayerColorPicker>().SetColor(cfg.bgColor, EMono.ui.skins.skinSets[cfg.id].bgs[cfg.bg].color, delegate(PickerState state, Color _c)
					{
						cfg.bgColor = _c;
						ApplySkin();
					});
				}
			});
			uIContextMenu2.AddButton("editDeco", delegate
			{
				EMono.ui.AddLayer<LayerSkinDeco>().SetWidget(this);
			});
		}
		if (AllowRightClickToClose)
		{
			m.AddToggle("RC_widget", config.RC, delegate(bool a)
			{
				config.RC = a;
			});
		}
		m.AddButton(() => (config.locked ? "unlockWidget" : "lockWidget").lang(), delegate
		{
			EMono.ui.widgets.ToggleLock(config);
			SE.ClickGeneral();
			m.Hide();
			EMono.ui.GetLayer<LayerWidget>()?.Refresh();
		});
		if (IsSealed)
		{
			return;
		}
		m.AddButton("closeWidget", delegate
		{
			if (this is WidgetQuestTracker)
			{
				EMono.player.questTracker = false;
			}
			EMono.ui.widgets.DeactivateWidget(this);
			EMono.ui.GetLayer<LayerWidget>()?.Refresh();
		});
	}

	public virtual void OnSetContextMenu(UIContextMenu m)
	{
		SetBaseContextMenu(m);
	}

	public void SetGridContextMenu(UIContextMenu m)
	{
		SkinConfig cfg = config.skin;
		m.AddSlider("skinGrid", (float n) => n.ToString() ?? "", cfg.grid, delegate(float a)
		{
			cfg.grid = (int)a;
			cfg.gridColor = cfg.Grid.color;
			ApplySkin();
		}, 0f, cfg.Skin.bgGrid.Count - 1, isInt: true);
		m.AddButton("colorGrid", delegate
		{
			EMono.ui.AddLayer<LayerColorPicker>().SetColor(cfg.gridColor, cfg.Grid.color, delegate(PickerState state, Color _c)
			{
				cfg.gridColor = _c;
				ApplySkin();
			});
		});
	}

	public void SoundActivate()
	{
		EMono.Sound.Play(soundActivate);
	}
}
