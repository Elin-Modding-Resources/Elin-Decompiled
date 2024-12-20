using System.Collections.Generic;
using UnityEngine.UI;

public class ContentConfigInput : ContentConfigGame
{
	public UIList listMovement;

	public UIList listGeneral;

	public UIList listMenu;

	public UIList listEtc;

	public UIList listAdvanced;

	public UIButton toggleAutorun;

	public UIButton toggleAltKeyAxis;

	public UIButton toggleKeepRunning;

	public UIButton toggleRightClickExitBuildMode;

	public UIButton toggleIgnoreNPCs;

	public UIButton toggleAltExamine;

	public UIButton toggleZoomToMouse;

	public UIButton toggleZoomMin;

	public UIButton toggleZoomMax;

	public UIButton toggleSmoothFollow;

	public UIButton toggleSmoothMove;

	public Slider sliderKeyboardScroll;

	public Slider sliderDragScroll;

	public Slider sliderEdgeScroll;

	public Slider sliderRunDistance;

	public Slider sliderMoveFrame;

	public UIButton toggleEdge;

	public UIButton toggleInvertX;

	public UIButton toggleInvertY;

	public UIButton toggleLinearZoom;

	public UIButton toggleAltChangeHeight;

	public UIDropdown ddMiddleClick;

	public UIDropdown ddMiddleClickLong;

	public UIDropdown ddMouse3Click;

	public UIDropdown ddMouse3Long;

	public UIDropdown ddMouse4Click;

	public UIDropdown ddMouse4Long;

	public UIDropdown b_ddMiddleClick;

	public UIDropdown b_ddMiddleClickLong;

	public UIDropdown b_ddMouse3Click;

	public UIDropdown b_ddMouse3Long;

	public UIDropdown b_ddMouse4Click;

	public UIDropdown b_ddMouse4Long;

	public override void OnInstantiate()
	{
		toggleAutorun.SetToggle(base.config.input.autorun, delegate(bool on)
		{
			base.config.input.autorun = on;
		});
		toggleAltKeyAxis.SetToggle(base.config.input.altKeyAxis, delegate(bool on)
		{
			base.config.input.altKeyAxis = on;
		});
		toggleIgnoreNPCs.SetToggle(base.config.input.ignoreNPCs, delegate(bool on)
		{
			base.config.input.ignoreNPCs = on;
		});
		toggleKeepRunning.SetToggle(base.config.input.keepRunning, delegate(bool on)
		{
			base.config.input.keepRunning = on;
		});
		toggleRightClickExitBuildMode.SetToggle(base.config.input.rightClickExitBuildMode, delegate(bool on)
		{
			base.config.input.rightClickExitBuildMode = on;
		});
		toggleAltExamine.SetToggle(base.config.input.altExamine, delegate(bool on)
		{
			base.config.input.altExamine = on;
		});
		toggleAltChangeHeight.SetToggle(base.config.input.altChangeHeight, delegate(bool on)
		{
			base.config.input.altChangeHeight = on;
		});
		toggleSmoothFollow.SetToggle(base.config.camera.smoothFollow, delegate(bool on)
		{
			base.config.camera.smoothFollow = on;
		});
		toggleSmoothMove.SetToggle(base.config.camera.smoothMove, delegate(bool on)
		{
			base.config.camera.smoothMove = on;
			sliderMoveFrame.SetActive(!on);
		});
		toggleZoomToMouse.SetToggle(base.config.camera.zoomToMouse, delegate(bool on)
		{
			base.config.camera.zoomToMouse = on;
		});
		toggleZoomMin.SetToggle(base.config.camera.extendZoomMin, delegate(bool on)
		{
			base.config.camera.extendZoomMin = on;
		});
		toggleZoomMax.SetToggle(base.config.camera.extendZoomMax, delegate(bool on)
		{
			base.config.camera.extendZoomMax = on;
		});
		toggleEdge.SetToggle(base.config.camera.edgeScroll, delegate(bool on)
		{
			base.config.camera.edgeScroll = on;
		});
		toggleInvertX.SetToggle(base.config.camera.invertX, delegate(bool on)
		{
			base.config.camera.invertX = on;
		});
		toggleInvertY.SetToggle(base.config.camera.invertY, delegate(bool on)
		{
			base.config.camera.invertY = on;
		});
		toggleLinearZoom.SetToggle(base.config.camera.linearZoom, delegate(bool on)
		{
			base.config.camera.linearZoom = on;
		});
		SetSlider(sliderMoveFrame, base.config.camera.moveframe, delegate(float a)
		{
			base.config.camera.moveframe = (int)a;
			return Lang.Get("moveframe") + "(" + (int)a + ")";
		});
		sliderMoveFrame.SetActive(!base.config.camera.smoothMove);
		SetSlider(sliderKeyboardScroll, base.config.camera.senseKeyboard, delegate(float a)
		{
			base.config.camera.senseKeyboard = a;
			return Lang.Get("speed") + "(" + (int)(a * 100f) + ")";
		});
		SetSlider(sliderDragScroll, base.config.camera.sensDrag, delegate(float a)
		{
			base.config.camera.sensDrag = a;
			return Lang.Get("speed") + "(" + (int)(a * 100f) + ")";
		});
		SetSlider(sliderEdgeScroll, base.config.camera.sensEdge, delegate(float a)
		{
			base.config.camera.sensEdge = a;
			return Lang.Get("speed") + "(" + (int)(a * 100f) + ")";
		});
		List<CoreConfig.GameFunc> list = Util.EnumToList<CoreConfig.GameFunc>();
		ddMiddleClick.SetList((int)base.config.input.middleClick, list, (CoreConfig.GameFunc a, int b) => a.ToString().lang(), delegate(int a, CoreConfig.GameFunc b)
		{
			base.config.input.middleClick = b;
		});
		ddMiddleClickLong.SetList((int)base.config.input.middlePressLong, list, (CoreConfig.GameFunc a, int b) => a.ToString().lang(), delegate(int a, CoreConfig.GameFunc b)
		{
			base.config.input.middlePressLong = b;
		});
		ddMouse3Click.SetList((int)base.config.input.mouse3Click, list, (CoreConfig.GameFunc a, int b) => a.ToString().lang(), delegate(int a, CoreConfig.GameFunc b)
		{
			base.config.input.mouse3Click = b;
		});
		ddMouse3Long.SetList((int)base.config.input.mouse3PressLong, list, (CoreConfig.GameFunc a, int b) => a.ToString().lang(), delegate(int a, CoreConfig.GameFunc b)
		{
			base.config.input.mouse3PressLong = b;
		});
		ddMouse4Click.SetList((int)base.config.input.mouse4Click, list, (CoreConfig.GameFunc a, int b) => a.ToString().lang(), delegate(int a, CoreConfig.GameFunc b)
		{
			base.config.input.mouse4Click = b;
		});
		ddMouse4Long.SetList((int)base.config.input.mouse4PressLong, list, (CoreConfig.GameFunc a, int b) => a.ToString().lang(), delegate(int a, CoreConfig.GameFunc b)
		{
			base.config.input.mouse4PressLong = b;
		});
		List<CoreConfig.GameFuncBuild> list2 = Util.EnumToList<CoreConfig.GameFuncBuild>();
		b_ddMiddleClick.SetList((int)base.config.input.b_middleClick, list2, (CoreConfig.GameFuncBuild a, int b) => a.ToString().lang(), delegate(int a, CoreConfig.GameFuncBuild b)
		{
			base.config.input.b_middleClick = b;
		});
		b_ddMiddleClickLong.SetList((int)base.config.input.b_middlePressLong, list2, (CoreConfig.GameFuncBuild a, int b) => a.ToString().lang(), delegate(int a, CoreConfig.GameFuncBuild b)
		{
			base.config.input.b_middlePressLong = b;
		});
		b_ddMouse3Click.SetList((int)base.config.input.b_mouse3Click, list2, (CoreConfig.GameFuncBuild a, int b) => a.ToString().lang(), delegate(int a, CoreConfig.GameFuncBuild b)
		{
			base.config.input.b_mouse3Click = b;
		});
		b_ddMouse3Long.SetList((int)base.config.input.b_mouse3PressLong, list2, (CoreConfig.GameFuncBuild a, int b) => a.ToString().lang(), delegate(int a, CoreConfig.GameFuncBuild b)
		{
			base.config.input.b_mouse3PressLong = b;
		});
		b_ddMouse4Click.SetList((int)base.config.input.b_mouse4Click, list2, (CoreConfig.GameFuncBuild a, int b) => a.ToString().lang(), delegate(int a, CoreConfig.GameFuncBuild b)
		{
			base.config.input.b_mouse4Click = b;
		});
		b_ddMouse4Long.SetList((int)base.config.input.b_mouse4PressLong, list2, (CoreConfig.GameFuncBuild a, int b) => a.ToString().lang(), delegate(int a, CoreConfig.GameFuncBuild b)
		{
			base.config.input.b_mouse4PressLong = b;
		});
		listMovement.callbacks = new UIList.Callback<EInput.KeyMap, ItemKeymap>
		{
			onInstantiate = delegate(EInput.KeyMap a, ItemKeymap b)
			{
				_onInstantiate(a, b);
			},
			onList = delegate
			{
				EInput.KeyMapManager keys5 = base.config.input.keys;
				UIList uIList5 = listMovement;
				uIList5.Add(keys5.axisUp);
				uIList5.Add(keys5.axisDown);
				uIList5.Add(keys5.axisLeft);
				uIList5.Add(keys5.axisRight);
				uIList5.Add(keys5.axisUpLeft);
				uIList5.Add(keys5.axisUpRight);
				uIList5.Add(keys5.axisDownLeft);
				uIList5.Add(keys5.axisDownRight);
				uIList5.Add(keys5.wait);
			}
		};
		listGeneral.callbacks = new UIList.Callback<EInput.KeyMap, ItemKeymap>
		{
			onInstantiate = delegate(EInput.KeyMap a, ItemKeymap b)
			{
				_onInstantiate(a, b);
			},
			onList = delegate
			{
				EInput.KeyMapManager keys4 = base.config.input.keys;
				UIList uIList4 = listGeneral;
				uIList4.Add(keys4.mouseLeft);
				uIList4.Add(keys4.mouseMiddle);
				uIList4.Add(keys4.mouseRight);
				uIList4.Add(keys4.fire);
				uIList4.Add(keys4.autoCombat);
				uIList4.Add(keys4.emptyHand);
			}
		};
		listMenu.callbacks = new UIList.Callback<EInput.KeyMap, ItemKeymap>
		{
			onInstantiate = delegate(EInput.KeyMap a, ItemKeymap b)
			{
				_onInstantiate(a, b);
			},
			onList = delegate
			{
				EInput.KeyMapManager keys3 = base.config.input.keys;
				UIList uIList3 = listMenu;
				uIList3.Add(keys3.chara);
				uIList3.Add(keys3.inventory);
				uIList3.Add(keys3.ability);
				uIList3.Add(keys3.journal);
				uIList3.Add(keys3.log);
				uIList3.Add(keys3.report);
				uIList3.Add(keys3.search);
			}
		};
		listEtc.callbacks = new UIList.Callback<EInput.KeyMap, ItemKeymap>
		{
			onInstantiate = delegate(EInput.KeyMap a, ItemKeymap b)
			{
				_onInstantiate(a, b);
			},
			onList = delegate
			{
				EInput.KeyMapManager keys2 = base.config.input.keys;
				UIList uIList2 = listEtc;
				uIList2.Add(keys2.switchHotbar);
				uIList2.Add(keys2.quickSave);
				uIList2.Add(keys2.quickLoad);
			}
		};
		listAdvanced.callbacks = new UIList.Callback<EInput.KeyMap, ItemKeymap>
		{
			onInstantiate = delegate(EInput.KeyMap a, ItemKeymap b)
			{
				_onInstantiate(a, b);
			},
			onList = delegate
			{
				EInput.KeyMapManager keys = base.config.input.keys;
				UIList uIList = listAdvanced;
				uIList.Add(keys.examine);
				uIList.Add(keys.getAll);
				uIList.Add(keys.dump);
				uIList.Add(keys.mute);
				uIList.Add(keys.meditate);
			}
		};
		_refreshList();
		void _onInstantiate(EInput.KeyMap a, ItemKeymap b)
		{
			b.text.text = ("key_" + a.action).lang();
			string text = a.key.ToString() ?? "";
			b.buttonKey.mainText.text = text;
			b.buttonKey.SetOnClick(delegate
			{
				Dialog.Keymap(a).SetOnKill(_refreshList);
			});
		}
		void _refreshList()
		{
			listMovement.List();
			listGeneral.List();
			listMenu.List();
			listEtc.List();
			listAdvanced.List();
		}
	}
}
