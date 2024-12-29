using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DG.Tweening;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform), typeof(EventTrigger))]
public class Window : MonoBehaviour, IChangeResolution, IUISkin
{
	public enum Type
	{
		Default,
		AlignToMouse,
		Side
	}

	[Serializable]
	public class Setting
	{
		[Serializable]
		public class Tab
		{
			public string idLang;

			public string langTooltip;

			public UIContent content;

			public UIButton button;

			public Action action;

			public bool disable;

			public Sprite sprite;

			public UIButton customTab;

			public object refObj;
		}

		public class BottomAction
		{
			public string idLang;

			public Button.ButtonClickedEvent action;
		}

		public string textCaption;

		public Sprite spriteCaption;

		public Rect bound = new Rect(400f, 300f, 1000f, 1000f);

		public bool asWidget;

		public bool allowMove;

		public bool allowResize;

		public bool saveWindow;

		public bool bottomBack;

		public bool dontBringToTop;

		public bool transparent = true;

		public bool tabAfterCaption;

		public bool openLastTab = true;

		public bool alwaysShowTab;

		public Type type;

		public List<Tab> tabs;

		public Anime anime;

		public Vector2 posFix;

		public bool usePosFix = true;

		public Vector2 clampMargin = new Vector2(-1f, -1f);

		public Vector2 resizeGrid;

		public string rightMenuAlias;
	}

	[Serializable]
	[JsonObject(MemberSerialization.OptIn)]
	public class SaveData
	{
		public enum CategoryType
		{
			Main,
			Sub,
			Exact,
			None
		}

		public enum FilterResult
		{
			Pass,
			Block,
			PassWithoutFurtherTest
		}

		[JsonProperty]
		public int[] ints = new int[20];

		[JsonProperty]
		public HashSet<int> cats = new HashSet<int>();

		[JsonProperty]
		public string filter;

		[JsonIgnore]
		public string[] _filterStrs;

		[JsonIgnore]
		public int[] filterOptions;

		public BitArray32 b1;

		public bool userFilter => !filter.IsEmpty();

		public string[] filterStrs
		{
			get
			{
				if (_filterStrs == null)
				{
					return BuildFilter();
				}
				return _filterStrs;
			}
		}

		public float x
		{
			get
			{
				return 0.01f * (float)ints[1];
			}
			set
			{
				ints[1] = (int)(value * 100f);
			}
		}

		public float y
		{
			get
			{
				return 0.01f * (float)ints[2];
			}
			set
			{
				ints[2] = (int)(value * 100f);
			}
		}

		public float w
		{
			get
			{
				return 0.01f * (float)ints[3];
			}
			set
			{
				ints[3] = (int)(value * 100f);
			}
		}

		public float h
		{
			get
			{
				return 0.01f * (float)ints[4];
			}
			set
			{
				ints[4] = (int)(value * 100f);
			}
		}

		public RectPosition anchor
		{
			get
			{
				return ints[5].ToEnum<RectPosition>();
			}
			set
			{
				ints[5] = (int)value;
			}
		}

		public AutodumpFlag autodump
		{
			get
			{
				return ints[6].ToEnum<AutodumpFlag>();
			}
			set
			{
				ints[6] = (int)value;
			}
		}

		public int size
		{
			get
			{
				return ints[7];
			}
			set
			{
				ints[7] = value;
			}
		}

		public Color32 color
		{
			get
			{
				return IntColor.FromInt(ints[8]);
			}
			set
			{
				ints[8] = IntColor.ToInt(value);
			}
		}

		public ContainerSharedType sharedType
		{
			get
			{
				return ints[9].ToEnum<ContainerSharedType>();
			}
			set
			{
				ints[9] = (int)value;
			}
		}

		public int priority
		{
			get
			{
				return ints[10];
			}
			set
			{
				ints[10] = value;
			}
		}

		public ContainerFlag flag
		{
			get
			{
				return ints[11].ToEnum<ContainerFlag>();
			}
			set
			{
				ints[11] = (int)value;
			}
		}

		public int columns
		{
			get
			{
				return ints[12];
			}
			set
			{
				ints[12] = value;
			}
		}

		public RectPosition customAnchor
		{
			get
			{
				return ints[13].ToEnum<RectPosition>();
			}
			set
			{
				ints[13] = (int)value;
			}
		}

		public CategoryType category
		{
			get
			{
				return ints[14].ToEnum<CategoryType>();
			}
			set
			{
				ints[14] = (int)value;
			}
		}

		public UIList.SortMode sortMode
		{
			get
			{
				if (ints[15] != 0)
				{
					return ints[15].ToEnum<UIList.SortMode>();
				}
				return UIList.SortMode.ByCategory;
			}
			set
			{
				ints[15] = (int)value;
			}
		}

		public bool open
		{
			get
			{
				return b1[0];
			}
			set
			{
				b1[0] = value;
			}
		}

		public bool useBG
		{
			get
			{
				return b1[1];
			}
			set
			{
				b1[1] = value;
			}
		}

		public bool firstSorted
		{
			get
			{
				return b1[2];
			}
			set
			{
				b1[2] = value;
			}
		}

		public bool excludeDump
		{
			get
			{
				return b1[3];
			}
			set
			{
				b1[3] = value;
			}
		}

		public bool excludeCraft
		{
			get
			{
				return b1[4];
			}
			set
			{
				b1[4] = value;
			}
		}

		public bool noRightClickClose
		{
			get
			{
				return b1[5];
			}
			set
			{
				b1[5] = value;
			}
		}

		public bool fixedPos
		{
			get
			{
				return b1[6];
			}
			set
			{
				b1[6] = value;
			}
		}

		public bool compress
		{
			get
			{
				return b1[7];
			}
			set
			{
				b1[7] = value;
			}
		}

		public bool advDistribution
		{
			get
			{
				return b1[8];
			}
			set
			{
				b1[8] = value;
			}
		}

		public bool noRotten
		{
			get
			{
				return b1[9];
			}
			set
			{
				b1[9] = value;
			}
		}

		public bool onlyRottable
		{
			get
			{
				return b1[10];
			}
			set
			{
				b1[10] = value;
			}
		}

		public bool alwaysSort
		{
			get
			{
				return b1[11];
			}
			set
			{
				b1[11] = value;
			}
		}

		public bool sort_ascending
		{
			get
			{
				return b1[12];
			}
			set
			{
				b1[12] = value;
			}
		}

		public FilterResult IsFilterPass(string text)
		{
			bool flag = false;
			bool flag2 = false;
			for (int i = 0; i < filterStrs.Length; i++)
			{
				switch (filterOptions[i])
				{
				case 0:
					if (text.Contains(filterStrs[i]))
					{
						flag = true;
					}
					flag2 = true;
					break;
				case 1:
					if (text.Contains(filterStrs[i]))
					{
						return FilterResult.Block;
					}
					break;
				case 2:
					if (text.Contains(filterStrs[i]))
					{
						return FilterResult.PassWithoutFurtherTest;
					}
					break;
				}
			}
			if (!flag && flag2)
			{
				return FilterResult.Block;
			}
			return FilterResult.Pass;
		}

		public string[] BuildFilter()
		{
			_filterStrs = filter.Replace('、', ',').Split(',');
			filterOptions = new int[filterStrs.Length];
			for (int i = 0; i < _filterStrs.Length; i++)
			{
				if (_filterStrs[i].Length != 0)
				{
					switch (_filterStrs[i][0])
					{
					case '-':
						filterOptions[i] = 1;
						_filterStrs[i] = _filterStrs[i].Replace("-", "");
						break;
					case '+':
						filterOptions[i] = 2;
						_filterStrs[i] = _filterStrs[i].Replace("+", "");
						break;
					case '"':
					case '\'':
						_filterStrs[i] = _filterStrs[i].Replace("\"", "");
						break;
					}
				}
			}
			return _filterStrs;
		}

		[OnSerializing]
		private void OnSerializing(StreamingContext context)
		{
			ints[0] = (int)b1.Bits;
		}

		[OnDeserialized]
		private void _OnDeserialized(StreamingContext context)
		{
			if (ints.Length < 20)
			{
				Array.Resize(ref ints, 20);
			}
			b1.Bits = (uint)ints[0];
		}

		public void CopyFrom(SaveData data)
		{
			for (int i = 0; i < ints.Length; i++)
			{
				if (i != 1 && i != 2)
				{
					ints[i] = data.ints[i];
				}
			}
			cats = IO.DeepCopy(data.cats);
			filter = data.filter;
			b1.Bits = (uint)ints[0];
			_filterStrs = null;
		}
	}

	public static Dictionary<string, SaveData> dictData = new Dictionary<string, SaveData>();

	public static Dictionary<string, int> dictTab = new Dictionary<string, int>();

	public static bool openLastTab;

	public static bool animateWindow;

	public bool leftSide = true;

	public bool isFloat;

	public Setting setting;

	public UIContentView view;

	public UIText textCaption;

	public UIText textStats;

	public Image iconCaption;

	public Image deco1;

	public Image deco2;

	public UIImage imageBG;

	public UIImage bgCaption;

	public Button buttonClose;

	public UIButton buttonHelp;

	public UIButton buttonSort;

	public UIButton buttonQuickSort;

	public UIButton buttonExtra;

	public UIButton buttonShared;

	public UIButton moldTab;

	public UIButton moldBottom;

	public RectTransform rectCorner;

	public RectTransform rectCaption;

	public RectTransform rectTab;

	public RectTransform rectBottom;

	public RectTransform rectStats;

	public RectTransform tipPiovotLeft;

	public RectTransform tipPivotRight;

	public RectTransform rectDeco;

	public RectTransform rectHeader;

	public UISelectableGroup groupTab;

	public WindowController controller;

	public SkinRootStatic skinRoot;

	public Tween tweenAnime;

	public CanvasGroup cgBG;

	public CanvasGroup cgFloatMenu;

	public UICollider bgCollider;

	public List<CanvasGroup> listCgFloat;

	private RectTransform _rect;

	[NonSerialized]
	public Layer layer;

	private LayoutGroup layoutWindow;

	public float FrameOffsetL = -15f;

	public float FrameOffsetR = -15f;

	public float FrameOffsetTop = -30f;

	public float FrameThickness = 14f;

	public float CaptionHeight = 50f;

	public float frameOffsetY = 5f;

	[NonSerialized]
	public int idTab;

	[NonSerialized]
	public int windowIndex;

	[NonSerialized]
	public WindowMenu menuLeft;

	[NonSerialized]
	public WindowMenu menuLeftBottom;

	[NonSerialized]
	public WindowMenu menuBottom;

	[NonSerialized]
	public WindowMenu menuRight;

	[NonSerialized]
	public CanvasGroup cg;

	[NonSerialized]
	public Vector2 posBottom;

	[NonSerialized]
	public Vector2 posTab;

	[NonSerialized]
	public Vector2 posCaption;

	[NonSerialized]
	public SaveData saveData;

	private bool? shadowEnabled;

	private Shadow bgShadow;

	public Window attachedWindow;

	public Vector2 attachFix;

	private Canvas _canvas;

	private EventTrigger _eventTrigger;

	private RectTransform _rectTransform;

	private Vector3 _correctedLossyScale;

	private Vector3[] _frameOuterCorners;

	private Vector3[] _frameInnerCorners;

	private Vector3[] _captionCorners;

	private bool IsProcessingCursorChanges;

	private bool IsProcessingMovement;

	private bool IsProcessingResize;

	private bool IsResizingOverTop;

	private bool IsResizingOverBottom;

	private bool IsResizingOverLeft;

	private bool IsResizingOverRight;

	private Vector3 LastScreenCursorPosition;

	private Vector3 dragStartPos;

	private Vector3 dragOffset;

	private bool scrolling;

	private bool isTop;

	private float floatAlpha;

	public string idWindow
	{
		get
		{
			if (!(layer == null))
			{
				return layer.uid + windowIndex;
			}
			return "nolayer";
		}
	}

	public Setting.Tab CurrentTab
	{
		get
		{
			if (setting.tabs.Count <= 0)
			{
				return null;
			}
			return setting.tabs[idTab];
		}
	}

	public UIContent CurrentContent
	{
		get
		{
			if (CurrentTab == null || !CurrentTab.content)
			{
				return null;
			}
			return CurrentTab.content;
		}
	}

	public bool AutoAnchor
	{
		get
		{
			if (!isFloat || (bool)attachedWindow)
			{
				if (saveData != null)
				{
					return saveData.customAnchor != RectPosition.Auto;
				}
				return false;
			}
			return true;
		}
	}

	public SkinRootStatic Skin
	{
		get
		{
			if (!(skinRoot != null))
			{
				return SkinManager.CurrentSkin;
			}
			return skinRoot;
		}
	}

	public Canvas Canvas
	{
		get
		{
			if (!_canvas)
			{
				_canvas = GetComponentInParent<Canvas>();
			}
			return _canvas;
		}
	}

	public EventTrigger EventTrigger
	{
		get
		{
			if (!_eventTrigger)
			{
				_eventTrigger = GetComponent<EventTrigger>();
			}
			return _eventTrigger;
		}
	}

	public RectTransform RectTransform
	{
		get
		{
			if (!_rectTransform)
			{
				_rectTransform = base.transform as RectTransform;
			}
			return _rectTransform;
		}
	}

	public Vector3 CorrectedLossyScale
	{
		get
		{
			if (_correctedLossyScale == Vector3.zero)
			{
				_correctedLossyScale = Canvas.CorrectLossyScale();
			}
			return _correctedLossyScale;
		}
	}

	public Vector3[] FrameOuterCorners
	{
		get
		{
			if (_frameOuterCorners == null || _frameOuterCorners.Length != 4)
			{
				_frameOuterCorners = new Vector3[4];
				RectTransform.GetScreenCorners(_frameOuterCorners, Canvas, FrameOffsetL, FrameOffsetR, frameOffsetY);
			}
			return _frameOuterCorners;
		}
	}

	public Vector3[] FrameInnerCorners
	{
		get
		{
			if (_frameInnerCorners == null || _frameInnerCorners.Length != 4)
			{
				_frameInnerCorners = new Vector3[4];
				RectTransform.GetScreenCorners(_frameInnerCorners, Canvas, FrameOffsetL + FrameThickness, FrameOffsetR + FrameThickness, frameOffsetY + FrameThickness);
			}
			return _frameInnerCorners;
		}
	}

	public Vector3[] CaptionCorners
	{
		get
		{
			if (_captionCorners == null || _captionCorners.Length != 4)
			{
				_captionCorners = new Vector3[4];
				for (int i = 0; i < 4; i++)
				{
					_captionCorners[i] = FrameInnerCorners[i];
				}
				_captionCorners[1].y -= FrameOffsetTop * CorrectedLossyScale.y;
				_captionCorners[2].y -= FrameOffsetTop * CorrectedLossyScale.y;
				_captionCorners[0].y = _captionCorners[1].y - CaptionHeight * CorrectedLossyScale.y;
				_captionCorners[3].y = _captionCorners[2].y - CaptionHeight * CorrectedLossyScale.y;
			}
			return _captionCorners;
		}
	}

	public bool isStatic => false;

	private void Awake()
	{
		controller = base.gameObject.GetComponent<WindowController>();
		if ((bool)controller)
		{
			controller.window = this;
		}
		menuLeft = new WindowMenu("Left", this);
		menuRight = new WindowMenu(setting.rightMenuAlias.IsEmpty("Right"), this);
		menuBottom = new WindowMenu("Bottom", this);
		menuLeftBottom = new WindowMenu("LeftBottom", this);
		if (!base.gameObject.GetComponent<Canvas>())
		{
			base.gameObject.AddComponent<Canvas>();
			base.gameObject.AddComponent<GraphicRaycaster>();
		}
		cg = base.gameObject.GetComponent<CanvasGroup>();
		if (cg == null)
		{
			cg = base.gameObject.AddComponent<CanvasGroup>();
		}
		posTab = rectTab.anchoredPosition;
		posBottom = rectBottom.anchoredPosition;
		layoutWindow = GetComponent<LayoutGroup>();
		posCaption = new Vector2(layoutWindow.padding.left, layoutWindow.padding.top);
		if (setting.tabAfterCaption)
		{
			rectTab.SetSiblingIndex(rectCaption.GetSiblingIndex());
		}
		if ((bool)imageBG)
		{
			bgShadow = imageBG.GetComponent<Shadow>();
		}
		_rect = this.Rect();
	}

	public void Init(Layer _layer = null)
	{
		_rect = this.Rect();
		layer = _layer;
		if (!setting.asWidget)
		{
			setting.allowResize = false;
			setting.saveWindow = false;
		}
		if (setting.saveWindow)
		{
			saveData = dictData.TryGetValue(idWindow);
		}
		if (saveData != null)
		{
			if (saveData.customAnchor != 0)
			{
				saveData.anchor = saveData.customAnchor;
			}
			if (AutoAnchor)
			{
				_rect.SetAnchor(saveData.anchor);
			}
			_rect.anchoredPosition = new Vector2(saveData.x, saveData.y);
			_rect.sizeDelta = new Vector2(saveData.w, saveData.h);
		}
		else if (setting.usePosFix)
		{
			_rect.anchoredPosition += setting.posFix;
		}
		if (openLastTab && setting.openLastTab && setting.tabs.Count > idTab && !setting.tabs[dictTab.TryGetValue(idWindow, 0)].disable)
		{
			idTab = dictTab.TryGetValue(idWindow, 0);
		}
		if (setting.textCaption.IsEmpty())
		{
			textCaption.SetActive(enable: false);
			iconCaption.SetActive(enable: false);
		}
		else
		{
			if (Lang.GetList(setting.textCaption) != null)
			{
				SetTitles(setting.textCaption);
			}
			else
			{
				SetCaption(setting.textCaption.lang());
			}
			if ((bool)setting.spriteCaption)
			{
				iconCaption.sprite = setting.spriteCaption;
			}
		}
		BuildTabs();
		bool flag = !(layer == null) && layer.option.canClose;
		if (flag && setting.bottomBack)
		{
			AddBottomButton("back", Close, setFirst: true);
		}
		if ((bool)buttonClose && buttonClose.gameObject.activeSelf)
		{
			buttonClose.onClick.RemoveAllListeners();
			buttonClose.onClick.AddListener(Close);
			buttonClose.SetActive(flag);
		}
		rectBottom.RebuildLayout(recursive: true);
		switch (setting.type)
		{
		case Type.AlignToMouse:
			leftSide = Input.mousePosition.x > (float)Screen.width / 1.5f;
			_rect.pivot = new Vector2(leftSide ? 1 : 0, _rect.pivot.y);
			_rect.position = Input.mousePosition + new Vector3(60f * (leftSide ? (-1f) : 1f), 0f, 0f);
			break;
		case Type.Side:
		{
			leftSide = Input.mousePosition.x > (float)Screen.width / 1.7f;
			float num = BaseCore.Instance.uiScale * (_rect.sizeDelta.x * 0.5f + 30f);
			_rect.position = new Vector3(leftSide ? num : ((float)Screen.width - num), _rect.position.y, 0f);
			setting.anime = Resources.Load<Anime>("Media/Anime/" + (leftSide ? "In Window from left" : "In Window from right"));
			break;
		}
		}
		RefreshTipPivotPosition();
		SwitchContent(idTab);
		InitPanel();
		ApplySkin();
		GameObject go = base.gameObject;
		if (!setting.anime || !animateWindow)
		{
			return;
		}
		BaseCore.Instance.actionsLateUpdate.Add(delegate
		{
			if ((bool)setting.anime && go != null)
			{
				tweenAnime = setting.anime.Play(this.Rect());
			}
		});
	}

	public void RefreshTipPivotPosition()
	{
		bool flag = (float)(Screen.width / 2 - 40) > base.transform.position.x;
		if ((bool)tipPiovotLeft)
		{
			tipPiovotLeft.SetActive(!flag || !tipPivotRight);
		}
		if ((bool)tipPivotRight)
		{
			tipPivotRight.SetActive(flag || !tipPiovotLeft);
		}
	}

	public void SetRect(RectData data, bool mousePos = false)
	{
		data.Apply(_rect);
		if (mousePos)
		{
			_rect.position = EInput.uiMousePosition;
		}
	}

	public void UpdateSaveData()
	{
		if (setting.saveWindow)
		{
			if (saveData == null)
			{
				saveData = new SaveData();
			}
			dictData[idWindow] = saveData;
		}
		if (saveData != null)
		{
			saveData.anchor = ((saveData.customAnchor == RectPosition.Auto) ? _rect.GetAnchor() : saveData.customAnchor);
			_rect.SetAnchor(saveData.anchor);
			Vector2 anchoredPosition = _rect.anchoredPosition;
			Vector2 sizeDelta = _rect.sizeDelta;
			saveData.x = anchoredPosition.x;
			saveData.y = anchoredPosition.y;
			saveData.w = sizeDelta.x;
			saveData.h = sizeDelta.y;
		}
	}

	public void OnKill()
	{
		UpdateSaveData();
		if (saveData != null)
		{
			saveData.open = false;
		}
		if (setting.openLastTab)
		{
			dictTab[idWindow] = idTab;
		}
	}

	public void ApplySkin()
	{
		SkinRootStatic skin = Skin;
		if ((bool)cg && setting.transparent)
		{
			cg.alpha = skin.transparency;
		}
		if ((bool)deco1)
		{
			Image image = deco1;
			Color color2 = (deco2.color = skin.colorsEx.deco1);
			image.color = color2;
		}
		layoutWindow.padding = skin.positions.paddingWindow;
		rectBottom.anchoredPosition = posBottom + skin.positions.bottom;
		if (!setting.tabAfterCaption)
		{
			rectTab.anchoredPosition = posTab + skin.positions.tab;
		}
		Image image2 = imageBG?.frame;
		if (!shadowEnabled.HasValue)
		{
			shadowEnabled = bgShadow?.enabled ?? false;
		}
		if ((bool)image2 && skin.useFrame)
		{
			image2.GetComponent<Shadow>().enabled = shadowEnabled.GetValueOrDefault();
			if ((bool)bgShadow)
			{
				bgShadow.enabled = false;
			}
		}
		else if ((bool)bgShadow)
		{
			bgShadow.enabled = shadowEnabled.GetValueOrDefault();
		}
		Outline component = textCaption.GetComponent<Outline>();
		if ((bool)component)
		{
			component.enabled = skin.captionOutline;
		}
		if ((bool)rectDeco)
		{
			rectDeco.SetActive(skin.useDeco);
		}
		if ((bool)rectCorner && imageBG.imageType != ImageType.BG_Window_Board)
		{
			rectCorner.anchoredPosition = skin.positions.windowCorner;
		}
		if ((bool)menuLeft.layout)
		{
			menuLeft.layout.Rect().anchoredPosition = skin.positions.leftMenu;
		}
		if ((bool)bgCaption)
		{
			bgCaption.SetActive(skin.captionBG && textCaption.gameObject.activeInHierarchy);
		}
		if (skin.alwaysShadow || skin.noShadow)
		{
			EnableShadow();
		}
	}

	public T SwitchContent<T>() where T : UIContent
	{
		SwitchContent(GetTab(typeof(T).Name.Replace("Content", "")));
		return CurrentContent as T;
	}

	public void SwitchContent(UIContent content)
	{
		SwitchContent(GetTab(content));
	}

	public void SwitchContent(string name)
	{
		SwitchContent(GetTab(name));
	}

	public void SwitchContent(Setting.Tab tab)
	{
		SwitchContent(setting.tabs.IndexOf(tab));
	}

	public UIContent SwitchContent(int index)
	{
		idTab = index;
		if (setting.tabs.Count > 0)
		{
			Setting.Tab tab = setting.tabs[index];
			foreach (Setting.Tab tab2 in setting.tabs)
			{
				if ((bool)tab2.content && !tab2.content.IsPrefab())
				{
					tab2.content.SetActive(tab2 == tab);
				}
			}
			if ((bool)groupTab)
			{
				groupTab.Select(setting.tabs[index].button);
			}
			if ((bool)tab.content)
			{
				if (tab.content.IsPrefab())
				{
					tab.content = Util.Instantiate(tab.content, view);
					tab.content.SetActive(enable: true);
					tab.content.OnInstantiate();
				}
				tab.content.SetActive(enable: true);
				tab.content.OnSwitchContent(idTab);
			}
			if (tab.action != null)
			{
				tab.action();
			}
			string text = ((layer == null) ? "" : layer.GetTextHeader(this));
			if (!text.IsEmpty())
			{
				SetCaption(text);
			}
		}
		if ((bool)controller)
		{
			controller.OnSwitchContent(this);
		}
		else if ((bool)layer)
		{
			layer.OnSwitchContent(this);
		}
		return CurrentContent;
	}

	public void SetTitles(string langList, string idHeaderRow = null)
	{
		string[] list = Lang.GetList(langList);
		if (!list[0].IsEmpty())
		{
			SetCaption(list[0]);
		}
		if (list.Length >= 3)
		{
			if (!rectHeader || idHeaderRow != null)
			{
				LoadHeader(idHeaderRow);
			}
			UIHeader[] componentsInChildren = rectHeader.GetComponentsInChildren<UIHeader>(includeInactive: true);
			UIHeader[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(enable: false);
			}
			for (int j = 0; j < list.Length - 3 && componentsInChildren.Length >= j + 1; j++)
			{
				componentsInChildren[j].SetActive(!list[j + 3].IsEmpty());
				componentsInChildren[j].SetText(list[j + 3]);
			}
		}
	}

	public void SetCaption(string text)
	{
		textCaption.SetText(text);
		textCaption.RebuildLayout();
	}

	public void AddHeader(string langLeft = null, string langRight = null)
	{
		if (!rectHeader)
		{
			LoadHeader();
		}
		UIHeader[] componentsInChildren = rectHeader.GetComponentsInChildren<UIHeader>(includeInactive: true);
		if (!langLeft.IsEmpty())
		{
			componentsInChildren[0].SetText(langLeft);
		}
		if (componentsInChildren.Length > 1)
		{
			if (langRight.IsEmpty())
			{
				componentsInChildren[1].SetActive(enable: false);
			}
			else
			{
				componentsInChildren[1].SetText(langRight);
			}
		}
	}

	public void LoadHeader(string id = "HeaderRow")
	{
		ScrollRect componentInChildren = GetComponentInChildren<ScrollRect>();
		if ((bool)componentInChildren)
		{
			if ((bool)rectHeader)
			{
				UnityEngine.Object.DestroyImmediate(rectHeader.gameObject);
			}
			else
			{
				RectTransform viewport = componentInChildren.viewport;
				Vector2 sizeDelta = viewport.sizeDelta;
				sizeDelta.y -= 30f;
				viewport.anchoredPosition = sizeDelta;
				viewport.sizeDelta = sizeDelta;
			}
			Transform c = Util.Instantiate("UI/Element/Header/" + id, componentInChildren.transform);
			rectHeader = c.Rect();
		}
	}

	public void SetPosition()
	{
		RectTransform rectTransform = this.Rect();
		rectTransform.pivot = new Vector2(0.5f, 0.5f);
		rectTransform.anchoredPosition = Vector2.zero;
	}

	public T SetContent<T>(int tabIndex = -1, string id = null) where T : UIContent
	{
		T val = Util.Instantiate<T>("UI/Content/" + typeof(T).Name, view);
		SetContent(tabIndex, val);
		return val;
	}

	public void SetContent(int tabIndex, UIContent content)
	{
		if (tabIndex != -1)
		{
			Setting.Tab tab = setting.tabs[tabIndex];
			if ((bool)tab.content && !tab.content.IsPrefab())
			{
				tab.content.SetActive(enable: false);
			}
			tab.content = content;
			tab.button = rectTab.GetComponentsInChildren<UIButton>()[tabIndex];
		}
	}

	public T GetContent<T>() where T : UIContent
	{
		if ((bool)CurrentContent)
		{
			return CurrentContent as T;
		}
		return null;
	}

	public bool NextTab(int a)
	{
		List<Setting.Tab> list = new List<Setting.Tab>();
		foreach (Setting.Tab tab in setting.tabs)
		{
			if (!tab.disable && (bool)tab.button)
			{
				list.Add(tab);
			}
		}
		if (list.Count > 1)
		{
			((a < 0) ? list.NextItem(CurrentTab) : list.PrevItem(CurrentTab)).button._OnPress();
			return true;
		}
		return false;
	}

	public void BuildTabs(int index = -1)
	{
		if (setting.tabs.Count > 0)
		{
			bool flag = false;
			foreach (Setting.Tab tab in setting.tabs)
			{
				if (!tab.button)
				{
					if (!flag && (bool)tab.customTab)
					{
						GameObject obj = new GameObject();
						obj.AddComponent<RectTransform>().sizeDelta = new Vector2(22f, 1f);
						obj.transform.SetParent(rectTab);
						flag = true;
					}
					UIButton uIButton = (tab.customTab ? Util.Instantiate(tab.customTab, rectTab) : (moldTab ? Util.Instantiate(moldTab, rectTab) : Util.Instantiate<UIButton>("UI/Window/Base/Element/ButtonTab", rectTab)));
					uIButton.mainText.SetLang(tab.idLang);
					tab.button = uIButton;
					if (!tab.langTooltip.IsEmpty())
					{
						uIButton.tooltip.enable = true;
						uIButton.tooltip.lang = tab.langTooltip;
					}
					if ((bool)tab.sprite)
					{
						uIButton.icon.sprite = tab.sprite;
					}
					if (tab.disable)
					{
						uIButton.SetActive(enable: false);
					}
				}
				tab.button.onClick.AddListener(delegate
				{
					SwitchContent(tab);
				});
			}
		}
		rectTab.SetActive(setting.tabs.Count > 1 || setting.alwaysShowTab);
		rectTab.RebuildLayout(recursive: true);
		if ((bool)groupTab)
		{
			groupTab.Init(index);
		}
	}

	public Setting.Tab AddTab(string idLang, UIContent content = null, Action action = null, Sprite sprite = null, string langTooltip = null)
	{
		Setting.Tab tab = new Setting.Tab
		{
			idLang = idLang,
			content = content,
			action = action,
			sprite = sprite,
			langTooltip = langTooltip
		};
		setting.tabs.Add(tab);
		return tab;
	}

	public int GetTab(UIContent content)
	{
		for (int i = 0; i < setting.tabs.Count; i++)
		{
			if (content == setting.tabs[i].content)
			{
				return i;
			}
		}
		return 0;
	}

	public int GetTab(string name)
	{
		for (int i = 0; i < setting.tabs.Count; i++)
		{
			if (setting.tabs[i].content.name == name)
			{
				return i;
			}
		}
		return 0;
	}

	public void ClearBottomButtons()
	{
		rectBottom.DestroyChildren();
	}

	public UIButton AddBottomButton(string idLang, UnityAction onClick, bool setFirst = false)
	{
		UIButton uIButton = (moldBottom ? Util.Instantiate(moldBottom, rectBottom) : Util.Instantiate<UIButton>("UI/Window/Base/Element/ButtonBottom", rectBottom));
		uIButton.mainText.SetLang(idLang);
		uIButton.onClick.AddListener(onClick);
		if (setFirst)
		{
			uIButton.transform.SetAsFirstSibling();
		}
		uIButton.RebuildLayout(recursive: true);
		return uIButton;
	}

	public void AddBottomSpace(int size = 20)
	{
		RectTransform rectTransform = new GameObject().AddComponent<RectTransform>();
		rectTransform.sizeDelta = new Vector2(size, 20f);
		rectTransform.SetParent(rectBottom, worldPositionStays: false);
	}

	public void EnableShadow()
	{
		if (!imageBG)
		{
			return;
		}
		Shadow component = imageBG.GetComponent<Shadow>();
		if ((bool)component)
		{
			if (Skin.noShadow)
			{
				component.enabled = false;
				return;
			}
			component.effectColor = Skin.shadowColor;
			component.enabled = true;
		}
	}

	public void SetInteractable(bool enable, float alpha = 0.5f)
	{
		cg.alpha = (enable ? 1f : alpha);
		GetComponent<GraphicRaycaster>().enabled = enable;
	}

	public void Close()
	{
		if ((bool)layer)
		{
			layer.OnClickClose();
		}
	}

	public void InitPanel()
	{
		if (!isStatic)
		{
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerEnter;
			entry.callback.AddListener(delegate(BaseEventData d)
			{
				OnPointerEnter(d as PointerEventData);
			});
			EventTrigger.triggers.Add(entry);
			entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerExit;
			entry.callback.AddListener(delegate(BaseEventData d)
			{
				OnPointerExit(d as PointerEventData);
			});
			EventTrigger.triggers.Add(entry);
			entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerDown;
			entry.callback.AddListener(delegate(BaseEventData d)
			{
				OnPointerDown(d as PointerEventData);
			});
			EventTrigger.triggers.Add(entry);
			entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerUp;
			entry.callback.AddListener(delegate(BaseEventData d)
			{
				OnPointerUp(d as PointerEventData);
			});
			EventTrigger.triggers.Add(entry);
			RecalculatePositionCaches();
			if (AutoAnchor)
			{
				SetAnchor();
			}
			ClampToScreen();
		}
	}

	public void OnChangeResolution()
	{
		if (!isStatic)
		{
			RecalculatePositionCaches();
			if (AutoAnchor)
			{
				SetAnchor();
			}
			IsProcessingCursorChanges = true;
			ClampToScreen();
		}
	}

	public void SetAnchor()
	{
		_rect.SetAnchor((saveData != null) ? saveData.customAnchor : RectPosition.Auto);
	}

	private void RecalculatePositionCaches()
	{
		_frameInnerCorners = null;
		_frameOuterCorners = null;
		_captionCorners = null;
	}

	public void OnApplicationFocus(bool focus)
	{
		IsProcessingCursorChanges = false;
		IsProcessingMovement = false;
		IsResizingOverBottom = false;
		IsResizingOverLeft = false;
		IsResizingOverRight = false;
	}

	private void Update()
	{
		RefreshTipPivotPosition();
		if (isStatic)
		{
			return;
		}
		isTop = InputModuleEX.GetComponentOf<Window>() == this;
		if (isFloat && (bool)cgFloatMenu && cgFloatMenu.enabled)
		{
			bool flag = InputModuleEX.IsPointerOver(this);
			floatAlpha = Mathf.Clamp(floatAlpha + (flag ? 2f : (-2f)) * Time.deltaTime * 1.5f, 0.5f, 1f);
			if ((bool)cgBG)
			{
				cgBG.alpha = floatAlpha;
			}
			float alpha = (floatAlpha - 0.5f) * 2f;
			if ((bool)cgFloatMenu)
			{
				cgFloatMenu.alpha = alpha;
			}
			cgFloatMenu.SetActive(cgFloatMenu.alpha > 0f);
			foreach (CanvasGroup item in listCgFloat)
			{
				item.alpha = alpha;
			}
		}
		if ((bool)attachedWindow)
		{
			Vector2 anchoredPosition = attachedWindow._rect.anchoredPosition;
			Vector2 anchoredPosition2 = new Vector2(anchoredPosition.x, anchoredPosition.y - attachedWindow._rect.sizeDelta.y) + attachFix;
			if (!_rect.anchorMin.Equals(attachedWindow._rect.anchorMin) || !_rect.anchorMax.Equals(attachedWindow._rect.anchorMax))
			{
				_rect.anchorMin = attachedWindow._rect.anchorMin;
				_rect.anchorMax = attachedWindow._rect.anchorMax;
			}
			if (_rect.anchoredPosition.x != anchoredPosition2.x || _rect.anchoredPosition.y != anchoredPosition2.y)
			{
				_rect.anchoredPosition = anchoredPosition2;
			}
			if (_rect.sizeDelta.x != attachedWindow._rect.sizeDelta.x)
			{
				_rect.sizeDelta = new Vector2(attachedWindow._rect.sizeDelta.x, _rect.sizeDelta.y);
			}
			rectCaption.SetActive(enable: false);
		}
		if (setting.dontBringToTop && base.transform.GetSiblingIndex() != 0)
		{
			base.transform.SetAsFirstSibling();
		}
		if (scrolling)
		{
			IsProcessingCursorChanges = false;
			IsProcessingMovement = false;
			IsResizingOverBottom = false;
			IsResizingOverLeft = false;
			IsResizingOverRight = false;
			CursorSystem.SetCursor();
		}
		else
		{
			if (IsProcessingCursorChanges)
			{
				ProcessCursorChanges();
			}
			if (IsProcessingMovement)
			{
				ProcessMovement();
			}
			if (IsProcessingResize)
			{
				ProcessResize();
			}
			if (isTop)
			{
				if ((IsResizingOverTop || IsResizingOverBottom) && (IsResizingOverRight || IsResizingOverLeft))
				{
					CursorSystem.SetCursor(CursorSystem.ResizeNWSE);
				}
				else if (IsResizingOverTop || IsResizingOverBottom)
				{
					CursorSystem.SetCursor(CursorSystem.ResizeNS);
				}
				else if (IsResizingOverRight || IsResizingOverLeft)
				{
					CursorSystem.SetCursor(CursorSystem.ResizeWE);
				}
			}
		}
		if (Input.GetMouseButtonDown(0))
		{
			if ((bool)InputModuleEX.GetComponentOf<Scrollbar>())
			{
				scrolling = true;
			}
			if (isTop && !setting.dontBringToTop)
			{
				BringToTop();
			}
		}
		if (Input.GetMouseButtonUp(0))
		{
			if (IsProcessingMovement)
			{
				IsProcessingMovement = false;
				if (AutoAnchor)
				{
					SetAnchor();
				}
			}
			scrolling = false;
			RecalculatePositionCaches();
			if (IsProcessingResize)
			{
				UIDynamicList[] componentsInChildren = GetComponentsInChildren<UIDynamicList>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].OnResizeWindow();
				}
				IsProcessingResize = false;
			}
			IsProcessingCursorChanges = true;
			IsResizingOverTop = false;
			IsResizingOverBottom = false;
			IsResizingOverLeft = false;
			IsResizingOverRight = false;
		}
		LastScreenCursorPosition = Input.mousePosition;
		LastScreenCursorPosition.x = Mathf.Clamp(LastScreenCursorPosition.x, 0f, Canvas.pixelRect.width);
		LastScreenCursorPosition.y = Mathf.Clamp(LastScreenCursorPosition.y, 0f, Canvas.pixelRect.height);
	}

	public void BringToTop()
	{
		if (!layer)
		{
			return;
		}
		int num = 0;
		foreach (Window window in layer.windows)
		{
			int siblingIndex = window.transform.GetSiblingIndex();
			if (siblingIndex > num)
			{
				num = siblingIndex;
			}
		}
		base.transform.SetSiblingIndex(num);
	}

	public void ClampToScreen()
	{
		RectTransform rectTransform = base.transform.parent.Rect();
		RectTransform rectTransform2 = base.transform.Rect();
		Transform transform = base.transform;
		Vector3 localPosition = base.transform.localPosition;
		Vector3 vector = rectTransform.rect.min - rectTransform2.rect.min;
		Vector3 vector2 = rectTransform.rect.max - rectTransform2.rect.max;
		float num = ((setting.clampMargin.x == -1f) ? 20f : setting.clampMargin.x);
		float num2 = ((setting.clampMargin.y == -1f) ? 40f : setting.clampMargin.y);
		localPosition.x = Mathf.Clamp(transform.localPosition.x, vector.x + num - 50f, vector2.x - num);
		localPosition.y = Mathf.Clamp(transform.localPosition.y, vector.y + num2 - 50f, vector2.y - num2 + 50f);
		transform.localPosition = localPosition;
	}

	private void OnPointerEnter(PointerEventData data)
	{
		IsProcessingCursorChanges = !Input.GetMouseButton(0);
	}

	private void OnPointerExit(PointerEventData data)
	{
		IsProcessingCursorChanges = false;
		if (!IsProcessingResize && !Input.GetMouseButton(0))
		{
			CursorSystem.SetCursor();
		}
	}

	private void OnPointerDown(PointerEventData data)
	{
		if (data.button == PointerEventData.InputButton.Left && !(EInput.dragHack < 0.1f))
		{
			base.transform.SetAsLastSibling();
			IsProcessingCursorChanges = false;
			IsResizingOverBottom = setting.allowResize && CursorOverBottomBorder();
			IsResizingOverLeft = setting.allowResize && CursorOverLeftBorder();
			IsResizingOverRight = setting.allowResize && CursorOverRightBorder();
			IsProcessingMovement = !IsResizingOverBottom && !IsResizingOverLeft && !IsResizingOverRight && setting.allowMove && CursorOverCaption();
			dragStartPos = Input.mousePosition;
			dragOffset = base.transform.position - dragStartPos;
			IsProcessingResize = IsResizingOverTop || IsResizingOverBottom || IsResizingOverLeft || IsResizingOverRight;
		}
	}

	private void OnPointerUp(PointerEventData data)
	{
	}

	private void ProcessCursorChanges()
	{
		if (setting.allowResize && (isTop || IsProcessingResize))
		{
			if ((CursorOverTopBorder() && CursorOverLeftBorder()) || (CursorOverBottomBorder() && CursorOverRightBorder()))
			{
				SetCursor(CursorSystem.ResizeNWSE);
			}
			else if ((CursorOverTopBorder() && CursorOverRightBorder()) || (CursorOverBottomBorder() && CursorOverLeftBorder()))
			{
				SetCursor(CursorSystem.ResizeNESW);
			}
			else if (CursorOverTopBorder() || CursorOverBottomBorder())
			{
				SetCursor(CursorSystem.ResizeNS);
			}
			else if (CursorOverLeftBorder() || CursorOverRightBorder())
			{
				SetCursor(CursorSystem.ResizeWE);
			}
		}
		void SetCursor(CursorInfo info)
		{
			Window topComponentOf = InputModuleEX.GetTopComponentOf<Window>();
			if (!topComponentOf || !(topComponentOf != this))
			{
				CursorSystem.SetCursor(info);
			}
		}
	}

	private void ProcessMovement()
	{
		Vector3 mousePosition = Input.mousePosition;
		mousePosition.x = Mathf.Clamp(mousePosition.x, 0f, Canvas.pixelRect.width);
		mousePosition.y = Mathf.Clamp(mousePosition.y, 0f, Canvas.pixelRect.height);
		base.transform.position = Input.mousePosition + dragOffset;
		ClampToScreen();
	}

	private void ProcessResize()
	{
		Vector3 vector = Input.mousePosition;
		vector.x = Mathf.Clamp(vector.x, 0f, Canvas.pixelRect.width);
		vector.y = Mathf.Clamp(vector.y, 0f, Canvas.pixelRect.height);
		Vector3 vector2 = LastScreenCursorPosition;
		Vector3 vector3 = vector - vector2;
		if (vector3.x == 0f && vector3.y == 0f)
		{
			return;
		}
		if (Canvas.renderMode == RenderMode.ScreenSpaceCamera)
		{
			vector = Canvas.worldCamera.ScreenToWorldPoint(new Vector3(vector.x, vector.y, Canvas.planeDistance));
			vector2 = Canvas.worldCamera.ScreenToWorldPoint(new Vector3(vector2.x, vector2.y, Canvas.planeDistance));
		}
		Vector3 vector4 = vector - vector2;
		Vector3 position = RectTransform.position;
		Vector2 sizeDelta = RectTransform.sizeDelta;
		Vector2 size = RectTransform.rect.size;
		Vector3 zero = Vector3.zero;
		Vector2 zero2 = Vector2.zero;
		Rect bound = setting.bound;
		float x = bound.x;
		float y = bound.y;
		float width = bound.width;
		float height = bound.height;
		if (IsResizingOverTop || IsResizingOverBottom)
		{
			if (RectTransform.anchorMin.y == RectTransform.anchorMax.y)
			{
				if (IsResizingOverTop)
				{
					float num = sizeDelta.y + vector3.y;
					if (num > y && num < height)
					{
						zero.y = vector4.y * RectTransform.pivot.y;
						zero2.y = vector3.y;
					}
				}
				else if (IsResizingOverBottom)
				{
					float num2 = sizeDelta.y - vector3.y;
					if (num2 > y && num2 < height)
					{
						zero.y = vector4.y * (1f - RectTransform.pivot.y);
						zero2.y = 0f - vector3.y;
					}
				}
				position.y += zero.y;
				sizeDelta.y += zero2.y / CorrectedLossyScale.y;
			}
			else
			{
				if (IsResizingOverTop)
				{
					float num3 = size.y + vector3.y;
					if (num3 > y && num3 < height)
					{
						zero.y = vector4.y * RectTransform.pivot.y;
						zero2.y = vector3.y;
					}
				}
				else if (IsResizingOverBottom)
				{
					float num4 = size.y - vector3.y;
					if (num4 > y && num4 < height)
					{
						zero.y = vector4.y * (1f - RectTransform.pivot.y);
						zero2.y = 0f - vector3.y;
					}
				}
				position.y += zero.y;
				sizeDelta.y += zero2.y / CorrectedLossyScale.y;
			}
		}
		if (IsResizingOverRight || IsResizingOverLeft)
		{
			if (RectTransform.anchorMin.x == RectTransform.anchorMax.x)
			{
				if (IsResizingOverRight)
				{
					float num5 = sizeDelta.x + vector3.x;
					if (num5 > x && num5 < width)
					{
						zero.x = vector4.x * RectTransform.pivot.x;
						zero2.x = vector3.x;
					}
				}
				else if (IsResizingOverLeft)
				{
					float num6 = sizeDelta.x - vector3.x;
					if (num6 > x && num6 < width)
					{
						zero.x = vector4.x * (1f - RectTransform.pivot.x);
						zero2.x = 0f - vector3.x;
					}
				}
				position.x += zero.x;
				sizeDelta.x += zero2.x / CorrectedLossyScale.x;
			}
			else
			{
				if (IsResizingOverRight)
				{
					float num7 = size.x + vector3.x;
					if (num7 > x && num7 < width)
					{
						zero.x = vector4.x * RectTransform.pivot.x;
						zero2.x = vector3.x;
					}
				}
				else if (IsResizingOverLeft)
				{
					float num8 = size.x - vector3.x;
					if (num8 > x && num8 < width)
					{
						zero.x = vector4.x * (1f - RectTransform.pivot.x);
						zero2.x = 0f - vector3.x;
					}
				}
				position.x += zero.x;
				sizeDelta.x += zero2.x / CorrectedLossyScale.x;
			}
		}
		if (setting.resizeGrid != Vector2.zero)
		{
			sizeDelta.x = setting.resizeGrid.x * (float)(int)(sizeDelta.x / setting.resizeGrid.x);
			sizeDelta.y = setting.resizeGrid.y * (float)(int)(sizeDelta.y / setting.resizeGrid.y);
		}
		RectTransform.sizeDelta = sizeDelta;
		RectTransform.position = position;
		RecalculatePositionCaches();
		this.RebuildLayout(recursive: true);
	}

	private bool CursorOverTopBorder(bool canExceed = false)
	{
		return false;
	}

	private bool CursorOverBottomBorder(bool canExceed = false)
	{
		UIButton componentOf = InputModuleEX.GetComponentOf<UIButton>();
		if ((bool)componentOf && componentOf.GetComponentInParent<Window>() == this)
		{
			return false;
		}
		if (canExceed)
		{
			return LastScreenCursorPosition.y <= FrameInnerCorners[0].y;
		}
		if (LastScreenCursorPosition.y <= FrameInnerCorners[0].y && LastScreenCursorPosition.y >= FrameOuterCorners[0].y && LastScreenCursorPosition.x >= FrameOuterCorners[0].x)
		{
			return LastScreenCursorPosition.x <= FrameOuterCorners[2].x;
		}
		return false;
	}

	private bool CursorOverLeftBorder(bool canExceed = false)
	{
		if (canExceed)
		{
			return LastScreenCursorPosition.x <= FrameInnerCorners[0].x;
		}
		if (LastScreenCursorPosition.x <= FrameInnerCorners[0].x && LastScreenCursorPosition.x >= FrameOuterCorners[0].x && LastScreenCursorPosition.y >= FrameOuterCorners[0].y)
		{
			return LastScreenCursorPosition.y <= FrameOuterCorners[1].y;
		}
		return false;
	}

	private bool CursorOverRightBorder(bool canExceed = false)
	{
		Scrollbar componentOf = InputModuleEX.GetComponentOf<Scrollbar>();
		if ((bool)componentOf && componentOf.GetComponentInParent<Window>() == this)
		{
			return false;
		}
		UIButton componentOf2 = InputModuleEX.GetComponentOf<UIButton>();
		if ((bool)componentOf2 && componentOf2.GetComponentInParent<Window>() == this)
		{
			return false;
		}
		if (canExceed)
		{
			return LastScreenCursorPosition.x >= FrameInnerCorners[3].x;
		}
		if (LastScreenCursorPosition.x >= FrameInnerCorners[3].x && LastScreenCursorPosition.x <= FrameOuterCorners[3].x && LastScreenCursorPosition.y >= FrameOuterCorners[0].y)
		{
			return LastScreenCursorPosition.y <= FrameOuterCorners[1].y;
		}
		return false;
	}

	private bool CursorOverCaption()
	{
		foreach (GameObject item in InputModuleEX.GetList())
		{
			if (item != null && (bool)item.GetComponent<ScrollRect>())
			{
				return false;
			}
		}
		return true;
	}

	private bool CursorOverWindow()
	{
		if (LastScreenCursorPosition.x >= FrameOuterCorners[0].x && LastScreenCursorPosition.x <= FrameOuterCorners[3].x && LastScreenCursorPosition.y >= FrameOuterCorners[0].y)
		{
			return LastScreenCursorPosition.y <= FrameOuterCorners[1].y;
		}
		return false;
	}
}
