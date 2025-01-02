using System;
using UnityEngine;

public class WidgetMainText : Widget
{
	public class Extra
	{
		public int maxLine;

		public int bg;

		public int width;

		public bool fadeLines;
	}

	public static WidgetMainText Instance;

	public static MsgBox boxBk;

	public static bool ignoreStack;

	public MsgBox box;

	private UIItem currentItem;

	public UIItem moldLine;

	public int visibleLines;

	public Transform layout;

	public Sprite spriteElin;

	public Sprite spriteDestroy;

	public Sprite spriteEther;

	[NonSerialized]
	public bool newLine = true;

	public Extra extra => base.config.extra as Extra;

	public override bool ShowStyleMenu => false;

	public override bool AlwaysTop => true;

	public override object CreateExtra()
	{
		return new Extra();
	}

	public override void OnActivate()
	{
		bool flag = true;
		if ((bool)boxBk)
		{
			UnityEngine.Object.DestroyImmediate(box.gameObject);
			box = boxBk;
			boxBk.transform.SetParent(base.transform, worldPositionStays: false);
			boxBk.GetComponentInChildren<UIDragPanel>(includeInactive: true).SetTarget(this.Rect());
			boxBk = null;
			flag = false;
		}
		dragPanel = box.dragPanel;
		imageBG = box.imageBg;
		Instance = this;
		box.maxBlock = extra.maxLine;
		box.fadeLines = extra.fadeLines;
		if (base.config.skin.bgColor.Equals(new Color(0f, 0f, 0f, 0f)))
		{
			base.config.skin.bgColor = EMono.core.refs.bg_msg[extra.bg].color;
		}
		RefreshBG();
		OnChangeResolution();
		box.Init();
		if (flag)
		{
			Append("welcome_intro".langGame(EMono.core.version.GetText()));
			Append(spriteElin);
			NewLine();
			Append("welcome".langGame(EInput.keys.report.key.ToString() ?? ""));
			NewLine();
		}
		_Refresh();
	}

	public void RefreshBG()
	{
		box.SetBG(EMono.core.refs.bg_msg[extra.bg], base.config.skin.bgColor);
	}

	public void Append(string s, Point pos = null)
	{
		Append(s, Msg.colors.Default, pos);
	}

	public void Append(string s, Color col, Point pos = null)
	{
		if (s.IsEmpty() || s == " ")
		{
			return;
		}
		if (pos != null)
		{
			box.Load("MsgFocus").button1.onClick.AddListener(delegate
			{
				EMono.screen.Focus(pos);
			});
		}
		if (s.StartsWith("*"))
		{
			s = " " + s;
		}
		if (Lang.setting.useSpace)
		{
			s += " ";
			if (s[0] == ' ')
			{
				s = s.TrimStart(' ');
			}
		}
		else if (s.EndsWith("*"))
		{
			s += " ";
		}
		if (MsgBlock.lastBlock != null && MsgBlock.lastText == s && !ignoreStack)
		{
			MsgBlock lastBlock = MsgBlock.lastBlock;
			if (lastBlock.txt != null)
			{
				UIText txt = lastBlock.txt;
				lastBlock.repeat++;
				string text = txt.text;
				if (text.EndsWith(")  ") && text.Contains("(x"))
				{
					text = text.Substring(0, text.IndexOf("(x"));
				}
				txt.text = text + "(x" + (lastBlock.repeat + 1) + ")  ";
				if (lastBlock.repeat == 1)
				{
					txt.RebuildLayout();
					box.RebuildLayout(recursive: true);
				}
				return;
			}
		}
		box.Append(s, col);
		_Refresh();
	}

	public void Append(Sprite sprite)
	{
		box.Append(sprite);
		_Refresh();
	}

	public static void HideLog()
	{
		if ((bool)Instance && Instance.box.isShowingLog)
		{
			Instance._ToggleLog();
		}
	}

	public static void ToggleLog()
	{
		if (!Instance)
		{
			EMono.ui.widgets.Activate("MainText");
		}
		Instance._ToggleLog();
		SE.ClickGeneral();
	}

	public void _ToggleLog()
	{
		box.ToggleLog();
		_Refresh();
		if (box.isShowingLog)
		{
			if ((bool)WidgetFeed.Instance)
			{
				WidgetFeed.Instance.pop.KillAll();
			}
			box.RebuildLayout(recursive: true);
		}
	}

	public void NewLine()
	{
		box.MarkNewBlock();
	}

	public static void Refresh()
	{
		if ((bool)Instance)
		{
			Instance._Refresh();
		}
	}

	private void _Refresh()
	{
		box.SetActive(box.isShowingLog || !WidgetFeed.Intercept, delegate(bool enabled)
		{
			if (enabled)
			{
				box.RebuildLayout(recursive: true);
			}
		});
	}

	public override void OnSetContextMenu(UIContextMenu m)
	{
		SkinConfig cfg = base.config.skin;
		UIContextMenu uIContextMenu = m.AddChild("setting");
		uIContextMenu.AddSlider("msgLines", (float n) => n.ToString() ?? "", box.maxBlock, delegate(float a)
		{
			box.maxBlock = (int)a;
			extra.maxLine = (int)a;
		}, 2f, 10f, isInt: true, hideOther: false);
		uIContextMenu.AddSlider("width", (float a) => a.ToString() ?? "", extra.width, delegate(float a)
		{
			extra.width = (int)a;
			OnChangeResolution();
		}, 30f, 100f, isInt: true);
		UIContextMenu uIContextMenu2 = m.AddChild("style");
		uIContextMenu2.AddToggle("fadeLines", extra.fadeLines, delegate(bool a)
		{
			extra.fadeLines = (box.fadeLines = a);
			box.RefreshAlpha();
		});
		uIContextMenu2.AddSlider("changeBG", (float n) => n + "/" + (EMono.core.refs.bg_msg.Count - 1), extra.bg, delegate(float a)
		{
			extra.bg = (int)a;
			cfg.bgColor = EMono.core.refs.bg_msg[extra.bg].color;
			RefreshBG();
		}, 0f, EMono.core.refs.bg_msg.Count - 1, isInt: true);
		uIContextMenu2.AddButton("colorBG", delegate
		{
			EMono.ui.AddLayer<LayerColorPicker>().SetColor(cfg.bgColor, EMono.ui.skins.skinSets[cfg.id].bgs[cfg.bg].color, delegate(PickerState state, Color _c)
			{
				cfg.bgColor = _c;
				RefreshBG();
			});
		});
		SetBaseContextMenu(m);
	}

	public override void OnChangeResolution()
	{
		base.OnChangeResolution();
		RectTransform rectTransform = this.Rect();
		rectTransform.sizeDelta = new Vector2(0.01f * (float)Screen.width * (float)extra.width, rectTransform.sizeDelta.y);
	}
}
