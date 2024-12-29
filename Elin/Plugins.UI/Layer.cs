using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Layer : MonoBehaviour, IUISkin
{
	[Serializable]
	public class Option
	{
		public enum ScreenlockType
		{
			Default,
			None,
			Dark,
			DarkLight
		}

		public Playlist playlist;

		public SoundData bgm;

		public SoundData soundActivate;

		public SoundData soundDeactivate;

		public bool canClose = true;

		public bool screenClickClose;

		public bool screenClickCloseRight;

		public bool passive;

		public bool important;

		public bool hideHud;

		public bool allowGeneralInput = true;

		public bool allowInventoryInteraction;

		public bool rebuildLayout;

		public bool persist;

		public bool blur;

		public bool hideOnUnfocus;

		public bool hideOthers = true;

		public bool hideInspector = true;

		public bool pauseGame = true;

		public bool consumeInput = true;

		public bool dontRefreshHint;

		public bool dontShowHint;

		public bool hideFloatUI;

		public bool hideWidgets;

		public ScreenlockType screenlockType = ScreenlockType.None;
	}

	public static int skipInput;

	public static bool closeOnRightClick;

	public static bool rightClicked;

	public static bool cancelKeyDown;

	public static bool ignoreSounds;

	public static Transform blurStopInstance;

	public Option option;

	public UnityEvent onKill;

	public Anime animeIn;

	public Anime animeOut;

	public bool closeOthers;

	public bool defaultActionMode = true;

	protected RectTransform _rect;

	protected bool isDestroyed;

	[NonSerialized]
	public Button screenLock;

	[NonSerialized]
	public Layer parent;

	[NonSerialized]
	public string idLayer;

	[NonSerialized]
	public List<Layer> layers = new List<Layer>();

	[NonSerialized]
	public Vector2 lastParentPos;

	[NonSerialized]
	public List<Window> windows = new List<Window>();

	[NonSerialized]
	public string langHint;

	public static string[] searchPath = new string[3] { "Layer", "Layer/Dialog", "Layer/LayerHome" };

	public Layer TopLayer => layers.LastItem();

	public string uid => base.name;

	public virtual RectTransform rectLayers => _rect;

	public virtual bool blockWidgetClick => true;

	public virtual string GetTextHeader(Window window)
	{
		string text = (window.CurrentTab?.idLang ?? window.setting.textCaption).lang();
		if (!HeaderIsListOf(window.idTab))
		{
			return text;
		}
		return "_listOf".lang(text);
	}

	public virtual bool HeaderIsListOf(int id)
	{
		return false;
	}

	protected virtual void Awake()
	{
		_rect = this.Rect();
		foreach (Window componentsInDirectChild in this.GetComponentsInDirectChildren<Window>())
		{
			windows.Add(componentsInDirectChild);
		}
		for (int i = 0; i < windows.Count; i++)
		{
			windows[i].windowIndex = i;
		}
		if (windows.Count > 1 && windows[0].transform.position.x < windows[1].transform.position.x)
		{
			windows[0].transform.SetAsLastSibling();
		}
	}

	public virtual void Init()
	{
		if (option.screenlockType != Option.ScreenlockType.None && !screenLock)
		{
			ShowScreenLock(option.screenlockType.ToString());
		}
		OnInit();
		for (int i = 0; i < windows.Count; i++)
		{
			windows[i].Init(this);
		}
		ApplySkin();
		OnAfterInit();
		SoundManager.current.Play(option.soundActivate);
		if ((bool)animeIn)
		{
			animeIn.Play(rectLayers);
		}
		if ((bool)option.bgm)
		{
			SoundManager.current.PlayBGM(option.bgm);
		}
		if ((bool)option.playlist)
		{
			SoundManager.current.SwitchPlaylist(option.playlist);
		}
		if (option.rebuildLayout)
		{
			this.RebuildLayout(recursive: true);
		}
	}

	public virtual void OnInit()
	{
	}

	public virtual void OnAfterInit()
	{
	}

	public virtual void ApplySkin()
	{
	}

	public void ShowScreenLock(string id)
	{
		if ((bool)screenLock)
		{
			UnityEngine.Object.DestroyImmediate(screenLock.gameObject);
		}
		screenLock = Util.Instantiate<Button>("UI/Screenlock" + id, rectLayers);
		screenLock.transform.SetAsFirstSibling();
	}

	public void UpdateInput()
	{
		if (skipInput > 0 || UIContextMenuManager.Instance.isActive)
		{
			skipInput--;
		}
		else if (layers.Count > 0)
		{
			layers.LastItem().UpdateInput();
		}
		else if ((EInput.leftMouse.down || (EInput.rightMouse.down && option.screenClickCloseRight)) && (((bool)screenLock && InputModuleEX.IsPointerOver(screenLock)) || (!InputModuleEX.IsPointerOverRoot(this) && !InputModuleEX.GetComponentOf<RectTransform>())))
		{
			if ((EInput.leftMouse.down && option.screenClickClose) || (EInput.rightMouse.down && option.screenClickCloseRight))
			{
				OnBack();
			}
		}
		else if (rightClicked || cancelKeyDown)
		{
			if ((bool)UIDropdown.activeInstance)
			{
				UIDropdown.activeInstance.Hide();
			}
			else if (rightClicked)
			{
				OnRightClick();
			}
			else
			{
				OnBack();
			}
		}
		else
		{
			OnUpdateInput();
		}
	}

	public virtual void OnUpdateInput()
	{
	}

	public virtual void OnRightClick()
	{
		if (closeOnRightClick)
		{
			OnBack();
		}
	}

	public virtual bool OnBack()
	{
		if (layers.Count > 0)
		{
			return layers.LastItem().OnBack();
		}
		if (!parent)
		{
			return false;
		}
		if (option.canClose)
		{
			Close();
			return true;
		}
		return false;
	}

	public virtual void OnChangeLayer()
	{
	}

	public static T Create<T>() where T : Layer
	{
		return Create(typeof(T).Name) as T;
	}

	public static T Create<T>(string path) where T : Layer
	{
		return Create(path) as T;
	}

	public static Layer Create(string path)
	{
		Layer layer = null;
		string text = path.Split('/').LastItem();
		string[] array = searchPath;
		foreach (string text2 in array)
		{
			layer = Resources.Load<Layer>("UI/" + text2 + "/" + path);
			if (!layer)
			{
				layer = Resources.Load<Layer>("UI/" + text2 + "/" + path + "/" + text);
			}
			if ((bool)layer)
			{
				break;
			}
		}
		if (!layer)
		{
			Debug.Log(path + " --- " + text);
		}
		layer = UnityEngine.Object.Instantiate(layer);
		Layer layer2 = layer;
		string text4 = (layer.name = text);
		layer2.idLayer = text4;
		layer.OnCreate();
		return layer;
	}

	public virtual void OnCreate()
	{
	}

	public void _AddLayer(string id)
	{
		ToggleLayer(id);
	}

	public Layer AddLayer(string id)
	{
		return AddLayer(Create(id));
	}

	public T AddLayer<T>() where T : Layer
	{
		return AddLayer(typeof(T).Name) as T;
	}

	public T AddLayer<T>(string id) where T : Layer
	{
		return AddLayer(id.IsEmpty(typeof(T).Name)) as T;
	}

	public T AddLayerDontCloseOthers<T>() where T : Layer
	{
		return AddLayerDontCloseOthers(Create<T>()) as T;
	}

	public Layer AddLayerDontCloseOthers(Layer l)
	{
		l.option.screenlockType = Option.ScreenlockType.Default;
		l.closeOthers = false;
		AddLayer(l);
		return l;
	}

	public T GetOrAddLayer<T>() where T : Layer
	{
		T val = GetLayer<T>();
		if (!val)
		{
			val = AddLayer<T>();
		}
		return val;
	}

	public virtual void OnBeforeAddLayer()
	{
	}

	public virtual void OnAfterAddLayer()
	{
	}

	public Layer AddLayer(Layer l)
	{
		l.OnBeforeAddLayer();
		if (l.option.persist && l.isDestroyed)
		{
			l.isDestroyed = false;
			l.gameObject.SetActive(value: true);
			layers.Add(l);
			if (!ignoreSounds)
			{
				SoundManager.current.Play(l.option.soundActivate);
			}
			return l;
		}
		l.gameObject.SetActive(value: true);
		l.parent = this;
		l.transform.SetParent(rectLayers, worldPositionStays: false);
		l.Init();
		if (l.option.hideOthers)
		{
			foreach (Layer layer in layers)
			{
				if (layer.option.hideOnUnfocus)
				{
					CanvasGroup canvasGroup = layer.gameObject.GetComponent<CanvasGroup>();
					if (!canvasGroup)
					{
						canvasGroup = layer.gameObject.AddComponent<CanvasGroup>();
					}
					canvasGroup.alpha = 0f;
				}
			}
		}
		layers.Add(l);
		OnChangeLayer();
		l._rect.anchorMin = Vector2.zero;
		l._rect.anchorMax = Vector2.one;
		l._rect.anchoredPosition = Vector2.zero;
		l._rect.sizeDelta = Vector2.zero;
		l.OnAfterAddLayer();
		return l;
	}

	public void ToggleLayer(string id)
	{
		Layer layer = GetLayer(id);
		if ((bool)layer)
		{
			RemoveLayer(layer);
		}
		else
		{
			AddLayer(id);
		}
	}

	public T ToggleLayer<T>(string id = null) where T : Layer
	{
		T layer = GetLayer<T>();
		if ((bool)layer)
		{
			RemoveLayer(layer);
			return null;
		}
		return AddLayer<T>(id);
	}

	public void WaitAndClose()
	{
		BaseCore.Instance.WaitForEndOfFrame(Close);
	}

	public void OnClickClose()
	{
		if (!BaseCore.BlockInput())
		{
			Close();
		}
	}

	public virtual void Close()
	{
		if ((bool)animeOut)
		{
			animeOut.Play(rectLayers).OnComplete(_Close);
		}
		else
		{
			_Close();
		}
	}

	protected virtual void _Close()
	{
		if ((bool)parent)
		{
			parent.RemoveLayer(this);
		}
		else
		{
			Kill();
		}
		skipInput = 2;
	}

	public void CloseLayers()
	{
		for (int num = layers.Count - 1; num >= 0; num--)
		{
			layers[num].Close();
		}
	}

	public void RemoveLayers(bool removeImportant = false)
	{
		for (int num = layers.Count - 1; num >= 0; num--)
		{
			if (removeImportant || (!layers[num].option.important && !layers[num].option.persist))
			{
				RemoveLayer(layers[num]);
			}
		}
	}

	public bool RemoveLayer<T>()
	{
		for (int num = layers.Count - 1; num >= 0; num--)
		{
			if (layers[num] is T)
			{
				RemoveLayer(layers[num]);
				return true;
			}
		}
		return false;
	}

	public void RemoveLayer(Layer l)
	{
		_RemoveLayer(l);
	}

	private void _RemoveLayer(Layer l)
	{
		l.Kill();
		layers.Remove(l);
		if (layers.Count > 0)
		{
			Layer layer = layers.LastItem();
			if (!layer.isDestroyed)
			{
				CanvasGroup component = layer.GetComponent<CanvasGroup>();
				if (layer.option.hideOnUnfocus && (bool)component)
				{
					component.alpha = 1f;
				}
			}
		}
		OnChangeLayer();
	}

	protected virtual void Kill()
	{
		if (isDestroyed || base.gameObject == null)
		{
			return;
		}
		isDestroyed = true;
		foreach (Layer layer in layers)
		{
			layer.Kill();
		}
		if (windows.Count > 0)
		{
			CursorSystem.SetCursor();
			foreach (Window window in windows)
			{
				window.OnKill();
			}
		}
		OnKill();
		onKill.Invoke();
		if (option.persist)
		{
			base.gameObject.SetActive(value: false);
		}
		else
		{
			UnityEngine.Object.DestroyImmediate(base.gameObject);
		}
	}

	public virtual void OnKill()
	{
	}

	public Layer SetOnKill(Action action)
	{
		onKill.AddListener(delegate
		{
			action();
		});
		return this;
	}

	public void DisableClose()
	{
		option.canClose = false;
		foreach (Window window in windows)
		{
			window.buttonClose.SetActive(enable: false);
		}
	}

	public void Delay(float duration = 0.05f)
	{
		foreach (Window window in windows)
		{
			window.setting.anime = null;
		}
		_rect.anchoredPosition = new Vector2(10000f, 10000f);
		TweenUtil.Tween(0.1f, delegate
		{
			BaseCore.Instance.WaitForEndOfFrame(delegate
			{
				if ((bool)this && (bool)base.gameObject)
				{
					_rect.anchoredPosition = new Vector2(0f, 0f);
					foreach (Window window2 in windows)
					{
						window2.ClampToScreen();
					}
					UIButton.TryShowTip();
				}
			});
		});
	}

	public Layer SetDelay(float d)
	{
		CanvasGroup cg = GetComponent<CanvasGroup>() ?? base.gameObject.AddComponent<CanvasGroup>();
		cg.alpha = 0f;
		TweenUtil.Tween(d, null, delegate
		{
			cg.alpha = 1f;
		});
		return this;
	}

	public Layer GetLayer(string id)
	{
		foreach (Layer layer in layers)
		{
			if (layer.idLayer == id)
			{
				return layer;
			}
		}
		return null;
	}

	public T GetLayer<T>(bool fromTop = false) where T : Layer
	{
		if (fromTop)
		{
			for (int num = layers.Count - 1; num >= 0; num--)
			{
				Layer layer = layers[num];
				if (layer is T)
				{
					return layer as T;
				}
			}
		}
		else
		{
			foreach (Layer layer2 in layers)
			{
				if (layer2 is T)
				{
					return layer2 as T;
				}
			}
		}
		return null;
	}

	public Layer GetTopLayer()
	{
		if (layers.Count > 0)
		{
			return layers.LastItem().GetTopLayer();
		}
		return this;
	}

	public void SwitchContent(int idWindow, int i)
	{
		windows[idWindow].SwitchContent(i);
	}

	public virtual void OnSwitchContent(Window window)
	{
	}

	public Layer SetTitles(string langList, string idHeaderRow = null)
	{
		windows[0].SetTitles(langList, idHeaderRow);
		return this;
	}

	public bool IsBlockWidgetClick()
	{
		foreach (Layer layer in layers)
		{
			if (layer.blockWidgetClick)
			{
				return true;
			}
		}
		return blockWidgetClick;
	}

	public bool IsHideHud()
	{
		foreach (Layer layer in layers)
		{
			if (layer.IsHideHud())
			{
				return true;
			}
		}
		return option.hideHud;
	}

	public bool IsAllowGeneralInput()
	{
		foreach (Layer layer in layers)
		{
			if (!layer.IsAllowGeneralInput())
			{
				return false;
			}
		}
		return option.allowGeneralInput;
	}

	public bool IsUseBlur()
	{
		if ((bool)blurStopInstance)
		{
			return false;
		}
		foreach (Layer layer in layers)
		{
			if (layer.IsUseBlur())
			{
				return true;
			}
		}
		return option.blur;
	}

	public bool IsPointerOnLayer()
	{
		foreach (GameObject item in InputModuleEX.GetPointerEventData().hovered)
		{
			if (item.GetComponentInParent<Layer>() == this)
			{
				return true;
			}
		}
		return false;
	}
}
