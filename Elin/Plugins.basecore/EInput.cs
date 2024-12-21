using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EInput : MonoBehaviour
{
	[Serializable]
	public class KeyMap
	{
		public EAction action;

		public KeyCode key;

		public bool required;

		public int GetGroup()
		{
			if (action == EAction.Examine)
			{
				return 1;
			}
			if (action == EAction.GetAll)
			{
				return 2;
			}
			return 0;
		}
	}

	[Serializable]
	public class KeyMapManager
	{
		public KeyMap cancel = new KeyMap
		{
			action = EAction.CancelUI
		};

		public KeyMap axisUp = new KeyMap
		{
			action = EAction.AxisUp
		};

		public KeyMap axisDown = new KeyMap
		{
			action = EAction.AxisDown
		};

		public KeyMap axisLeft = new KeyMap
		{
			action = EAction.AxisLeft
		};

		public KeyMap axisRight = new KeyMap
		{
			action = EAction.AxisRight
		};

		public KeyMap axisUpLeft = new KeyMap
		{
			action = EAction.AxisUpLeft
		};

		public KeyMap axisUpRight = new KeyMap
		{
			action = EAction.AxisUpRight
		};

		public KeyMap axisDownLeft = new KeyMap
		{
			action = EAction.AxisDownLeft
		};

		public KeyMap axisDownRight = new KeyMap
		{
			action = EAction.AxisDownRight
		};

		public KeyMap journal = new KeyMap
		{
			action = EAction.MenuJournal
		};

		public KeyMap chara = new KeyMap
		{
			action = EAction.MenuChara
		};

		public KeyMap inventory = new KeyMap
		{
			action = EAction.MenuInventory
		};

		public KeyMap ability = new KeyMap
		{
			action = EAction.MenuAbility
		};

		public KeyMap log = new KeyMap
		{
			action = EAction.Log
		};

		public KeyMap fire = new KeyMap
		{
			action = EAction.Fire
		};

		public KeyMap wait = new KeyMap
		{
			action = EAction.Wait
		};

		public KeyMap autoCombat = new KeyMap
		{
			action = EAction.AutoCombat
		};

		public KeyMap emptyHand = new KeyMap
		{
			action = EAction.EmptyHand
		};

		public KeyMap mouseLeft = new KeyMap
		{
			action = EAction.MouseLeft
		};

		public KeyMap mouseRight = new KeyMap
		{
			action = EAction.MouseRight
		};

		public KeyMap mouseMiddle = new KeyMap
		{
			action = EAction.MouseMiddle
		};

		public KeyMap report = new KeyMap
		{
			action = EAction.Report
		};

		public KeyMap quickSave = new KeyMap
		{
			action = EAction.QuickSave
		};

		public KeyMap quickLoad = new KeyMap
		{
			action = EAction.QuickLoad
		};

		public KeyMap switchHotbar = new KeyMap
		{
			action = EAction.SwitchHotbar
		};

		public KeyMap examine = new KeyMap
		{
			action = EAction.Examine
		};

		public KeyMap getAll = new KeyMap
		{
			action = EAction.GetAll
		};

		public KeyMap dump = new KeyMap
		{
			action = EAction.Dump
		};

		public KeyMap mute = new KeyMap
		{
			action = EAction.Mute
		};

		public KeyMap meditate = new KeyMap
		{
			action = EAction.Meditate
		};

		public KeyMap search = new KeyMap
		{
			action = EAction.Search
		};

		public List<KeyMap> List()
		{
			return new List<KeyMap>
			{
				axisUp, axisDown, axisLeft, axisRight, axisUpLeft, axisUpRight, axisDownLeft, axisDownRight, journal, chara,
				inventory, ability, log, fire, wait, mouseLeft, mouseMiddle, mouseRight, report, quickSave,
				quickLoad, autoCombat, emptyHand, switchHotbar, examine, getAll, dump, mute, meditate, search
			};
		}
	}

	public class KeyboardPress
	{
		public float timer;

		public int count;

		public bool consumed;

		public Func<KeyMap> func;

		public EAction Action => func().action;

		public bool IsRepeating => count > 1;

		public bool Update(bool forcePress = false)
		{
			if (Input.GetKey(func().key) || forcePress)
			{
				if (consumed)
				{
					return false;
				}
				if (count == 1)
				{
					timer += delta;
					if (timer < 0.5f)
					{
						return false;
					}
				}
				else
				{
					timer = 0f;
				}
				count++;
				return true;
			}
			consumed = false;
			count = 0;
			return false;
		}

		public void Consume()
		{
			consumed = true;
		}
	}

	public static KeyboardPress keyFire = new KeyboardPress();

	public static KeyboardPress keyWait = new KeyboardPress();

	public static Func<string, string> LangGetter;

	public static EInput Instance;

	public static EventSystem eventSystem;

	public static EAction action;

	public static bool isShiftDown;

	public static bool isCtrlDown;

	public static bool isAltDown;

	public static bool requireConfirmReset;

	public static bool requireAxisReset;

	public static bool hasAxisMoved;

	public static bool hasShiftChanged;

	public static bool haltInput;

	public static bool isInputFieldActive;

	public static bool firstAxisPressed;

	public static bool axisReleased;

	public static int hotkey;

	public static int functionkey;

	public static int skipFrame;

	public static int wheel;

	public static int missClickButton;

	private static float waitInput;

	private static float durationAxis;

	private static float durationFirstAxis;

	private static float durationAxisRelease;

	private static float durationAxisDiagonal;

	private static float waitReleaseKeyTimer;

	private static Vector2 lastAxis;

	private static Vector2 firstAxis;

	private static Vector2 prevAxis;

	public static bool hasMouseMoved;

	public static bool axisXChanged;

	public static bool axisYChanged;

	public static bool waitReleaseAnyKey;

	public static List<ButtonState> buttons = new List<ButtonState>();

	public static float delta;

	public static float antiMissClick;

	public static float missClickDuration;

	public static float dragHack;

	public static float ignoreWheelDuration;

	public static Vector2 axis;

	public static Vector2 forbidAxis;

	public static Vector2 axisDiagonal;

	public static Vector3 mpos;

	public static Vector3 mposWorld;

	public static Vector3 dragStartPos;

	public static Vector3 dragStartPos2;

	public static Vector3 lastMousePos;

	public static Vector3 dragAmount;

	public static ButtonState leftMouse;

	public static ButtonState rightMouse;

	public static ButtonState middleMouse;

	public static ButtonState mouse3;

	public static ButtonState mouse4;

	public static ButtonState buttonScroll;

	public static ButtonState buttonCtrl;

	public static bool rightScroll;

	public static bool disableKeyAxis;

	public static KeyMapManager keys = new KeyMapManager();

	public static Vector3 uiMousePosition;

	public float repeatThresh = 0.3f;

	public float repeatSpeed = 0.05f;

	public static bool isConfirm => action == EAction.Confirm;

	public static bool isCancel
	{
		get
		{
			if (action != EAction.Cancel)
			{
				return action == EAction.CancelUI;
			}
			return true;
		}
	}

	public static bool isPrev => action == EAction.Prev;

	public static bool isNext => action == EAction.Next;

	public static bool IsConsumed => skipFrame > 0;

	public static GameObject selectedGO => eventSystem.currentSelectedGameObject;

	public static void WaitInput(float time)
	{
		waitInput = time;
	}

	private void Awake()
	{
		Instance = this;
		Init();
	}

	public static void Init()
	{
		leftMouse = AddButton(0);
		rightMouse = AddButton(1);
		middleMouse = AddButton(2);
		mouse3 = AddButton(3);
		mouse4 = AddButton(4);
		leftMouse.dragMargin = 32f;
		middleMouse.clickDuration = 0.3f;
		middleMouse.clickCriteria = ButtonState.ClickCriteria.ByDuration;
		middleMouse.ignoreWheel = true;
		middleMouse.id = "middle";
		mouse3.clickDuration = 0.3f;
		mouse3.clickCriteria = ButtonState.ClickCriteria.ByDuration;
		mouse4.clickDuration = 0.3f;
		mouse4.clickCriteria = ButtonState.ClickCriteria.ByDuration;
		buttonCtrl = new ButtonState
		{
			mouse = -1,
			clickCriteria = ButtonState.ClickCriteria.ByDuration
		};
		buttons.Add(buttonCtrl);
		SetKeyMap(keys);
	}

	public static void SetKeyMap(KeyMapManager _keys)
	{
		keys = _keys;
		leftMouse.keymap = keys.mouseLeft;
		rightMouse.keymap = keys.mouseRight;
		middleMouse.keymap = keys.mouseMiddle;
		buttonCtrl.keymap = keys.switchHotbar;
		keyFire.func = () => keys.fire;
		keyWait.func = () => keys.wait;
	}

	public static ButtonState AddButton(int mouse)
	{
		ButtonState buttonState = new ButtonState
		{
			mouse = mouse
		};
		buttons.Add(buttonState);
		return buttonState;
	}

	public static void DisableIME()
	{
		ToggleIME(on: false);
	}

	public static void ToggleIME(bool on)
	{
		Input.imeCompositionMode = (on ? IMECompositionMode.On : IMECompositionMode.Off);
	}

	public static void UpdateOnlyAxis()
	{
		isShiftDown = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
		isCtrlDown = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
		isAltDown = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
		axis = Vector2.zero;
		hasAxisMoved = (hasMouseMoved = false);
		UpdateAxis();
		Consume(3);
	}

	public static void Update()
	{
		missClickDuration -= delta;
		ignoreWheelDuration -= delta;
		dragHack += delta;
		mpos = Input.mousePosition;
		if ((bool)Camera.main)
		{
			mposWorld = Camera.main.ScreenToWorldPoint(mpos);
		}
		mposWorld.z = 0f;
		action = EAction.None;
		axis = Vector2.zero;
		hasAxisMoved = (hasMouseMoved = false);
		wheel = 0;
		if (!Application.isFocused)
		{
			return;
		}
		GameObject gameObject = EventSystem.current?.currentSelectedGameObject;
		if ((bool)gameObject && gameObject.activeInHierarchy)
		{
			isInputFieldActive = gameObject.GetComponent<InputField>();
		}
		else
		{
			isInputFieldActive = false;
		}
		isShiftDown = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
		isCtrlDown = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
		isAltDown = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
		if (waitReleaseAnyKey)
		{
			waitReleaseKeyTimer -= Time.deltaTime;
			if (waitReleaseKeyTimer < 0f)
			{
				waitReleaseAnyKey = false;
			}
			else
			{
				if (!Input.inputString.IsEmpty() || IsAnyKeyDown(includeAxis: true, _skipframe: false))
				{
					return;
				}
				waitReleaseAnyKey = false;
			}
		}
		if (haltInput)
		{
			Consume(consumeAxis: true);
			return;
		}
		if (requireConfirmReset)
		{
			if (Input.GetKey(keys.wait.key) || Input.GetKey(KeyCode.Keypad5))
			{
				Consume(consumeAxis: true);
				return;
			}
			requireConfirmReset = false;
		}
		dragAmount = lastMousePos - mpos;
		if (lastMousePos != mpos)
		{
			lastMousePos = mpos;
			hasMouseMoved = true;
		}
		if (skipFrame > 0)
		{
			skipFrame--;
			return;
		}
		if (waitInput > 0f)
		{
			Consume(consumeAxis: true);
			waitInput -= Time.unscaledDeltaTime;
			return;
		}
		if (Input.GetMouseButtonDown(0))
		{
			missClickButton = 0;
			missClickDuration = antiMissClick;
		}
		if (Input.GetMouseButtonDown(1))
		{
			if (missClickDuration > 0f && missClickButton == 0)
			{
				Consume();
				rightMouse.consumed = false;
				skipFrame = 5;
				return;
			}
			missClickButton = 1;
			missClickDuration = antiMissClick;
		}
		foreach (ButtonState button in buttons)
		{
			button.Update();
		}
		if (ignoreWheelDuration < 0f)
		{
			float num = Input.GetAxis("Mouse ScrollWheel");
			wheel = ((num < 0f) ? (-1) : ((num > 0f) ? 1 : 0));
			if (Input.GetKeyDown(KeyCode.PageUp))
			{
				wheel = 1;
			}
			if (Input.GetKeyDown(KeyCode.PageDown))
			{
				wheel = -1;
			}
		}
		if (isInputFieldActive)
		{
			return;
		}
		UpdateAxis();
		if (requireAxisReset)
		{
			if (axis == Vector2.zero)
			{
				requireAxisReset = false;
			}
			else
			{
				axis = Vector2.zero;
			}
		}
		if (forbidAxis != Vector2.zero && axis == forbidAxis)
		{
			axis = Vector2.zero;
		}
		else
		{
			forbidAxis = Vector2.zero;
		}
		hotkey = GetHotkeyDown();
		if (hotkey == -1 && action == EAction.None)
		{
			action = GetAction();
		}
		functionkey = GetFunctionkeyDown();
	}

	public static bool IsAnyKeyDown(bool includeAxis = true, bool _skipframe = true)
	{
		if (_skipframe && skipFrame > 0)
		{
			return false;
		}
		if (isConfirm || isCancel)
		{
			return true;
		}
		if (includeAxis && axis != Vector2.zero)
		{
			return true;
		}
		foreach (ButtonState button in buttons)
		{
			if (button.clicked)
			{
				return true;
			}
		}
		for (int i = 97; i < 122; i++)
		{
			if (Input.GetKey(i.ToEnum<KeyCode>()))
			{
				return true;
			}
		}
		return false;
	}

	public static void WaitReleaseKey()
	{
		waitReleaseAnyKey = true;
		waitReleaseKeyTimer = 2.5f;
	}

	public static void Consume(int _skipFrame)
	{
		Consume(consumeAxis: false, _skipFrame);
	}

	public static void Consume(bool consumeAxis = false, int _skipFrame = 1)
	{
		wheel = 0;
		action = EAction.None;
		functionkey = -1;
		hotkey = -1;
		if (_skipFrame > skipFrame)
		{
			skipFrame = _skipFrame;
		}
		if (consumeAxis)
		{
			axis = (lastAxis = Vector2.zero);
			requireAxisReset = true;
			durationAxis = 0f;
		}
		foreach (ButtonState button in buttons)
		{
			button.Consume();
		}
	}

	public static void ConsumeWheel()
	{
		wheel = 0;
	}

	public static void SetAxis(int x, int y)
	{
		axis.x = x;
		axis.y = y;
	}

	public static void ModAxisX(int x)
	{
		axis.x += x;
		axisXChanged = true;
	}

	public static void ModAxisY(int y)
	{
		axis.y += y;
		axisYChanged = true;
	}

	private static void UpdateAxis()
	{
		if (waitReleaseAnyKey)
		{
			return;
		}
		axisXChanged = (axisYChanged = false);
		if (Input.GetKey(keys.axisUp.key))
		{
			ModAxisY(1);
		}
		if (Input.GetKey(keys.axisDown.key))
		{
			ModAxisY(-1);
		}
		if (Input.GetKey(keys.axisLeft.key))
		{
			ModAxisX(-1);
		}
		if (Input.GetKey(keys.axisRight.key))
		{
			ModAxisX(1);
		}
		if (axis == Vector2.zero && !disableKeyAxis)
		{
			if (Input.GetKey(keys.axisUp.key) || Input.GetKey(KeyCode.UpArrow))
			{
				ModAxisY(1);
			}
			if (Input.GetKey(keys.axisDown.key) || Input.GetKey(KeyCode.DownArrow))
			{
				ModAxisY(-1);
			}
			if (Input.GetKey(keys.axisLeft.key) || Input.GetKey(KeyCode.LeftArrow))
			{
				ModAxisX(-1);
			}
			if (Input.GetKey(keys.axisRight.key) || Input.GetKey(KeyCode.RightArrow))
			{
				ModAxisX(1);
			}
		}
		if (axis == Vector2.zero)
		{
			if (Input.GetKey(KeyCode.Keypad8))
			{
				ModAxisY(1);
			}
			if (Input.GetKey(KeyCode.Keypad2))
			{
				ModAxisY(-1);
			}
			if (Input.GetKey(KeyCode.Keypad4))
			{
				ModAxisX(-1);
			}
			if (Input.GetKey(KeyCode.Keypad6))
			{
				ModAxisX(1);
			}
		}
		if (Input.GetKey(keys.axisUpLeft.key))
		{
			SetAxis(-1, 1);
		}
		if (Input.GetKey(keys.axisUpRight.key))
		{
			SetAxis(1, 1);
		}
		if (Input.GetKey(keys.axisDownLeft.key))
		{
			SetAxis(-1, -1);
		}
		if (Input.GetKey(keys.axisDownRight.key))
		{
			SetAxis(1, -1);
		}
		if (Input.GetKey(KeyCode.Keypad1))
		{
			SetAxis(-1, -1);
		}
		if (Input.GetKey(KeyCode.Keypad3))
		{
			SetAxis(1, -1);
		}
		if (Input.GetKey(KeyCode.Keypad7))
		{
			SetAxis(-1, 1);
		}
		if (Input.GetKey(KeyCode.Keypad9))
		{
			SetAxis(1, 1);
		}
		if (axis.x == 0f && axisXChanged)
		{
			axis.x = 0f - prevAxis.x;
		}
		else
		{
			prevAxis.x = axis.x;
		}
		if (axis.y == 0f && axisYChanged)
		{
			axis.y = 0f - prevAxis.y;
		}
		else
		{
			prevAxis.y = axis.y;
		}
		if (Input.GetKey(KeyCode.LeftAlt) && (axis.x == 0f || axis.y == 0f))
		{
			axis = Vector2.zero;
		}
		if (axis != Vector2.zero && !firstAxisPressed)
		{
			firstAxisPressed = true;
			firstAxis = axis;
			durationFirstAxis = 0.06f;
		}
		if (firstAxisPressed)
		{
			durationFirstAxis -= delta;
			if (durationFirstAxis > 0f)
			{
				if (!(axis == Vector2.zero))
				{
					axis = Vector2.zero;
					return;
				}
				axis = firstAxis;
			}
			else if (axis == Vector2.zero)
			{
				firstAxisPressed = false;
			}
		}
		if (axis.x != 0f && axis.y != 0f)
		{
			axisDiagonal.x = axis.x;
			axisDiagonal.y = axis.y;
			durationAxisDiagonal += delta;
			if (durationAxisDiagonal > 0f)
			{
				durationAxisDiagonal = 2f;
			}
		}
		else
		{
			durationAxisDiagonal -= delta * 10f;
			if (durationAxisDiagonal < 0f)
			{
				durationAxisDiagonal = 0f;
			}
		}
		if (axis == Vector2.zero)
		{
			durationAxisRelease += delta;
			if (durationAxisRelease < 0.5f + durationAxisDiagonal * 0.1f)
			{
				return;
			}
			lastAxis = Vector2.zero;
		}
		else
		{
			durationAxisRelease = 0f;
		}
		if (durationAxisDiagonal > 1f)
		{
			if (axis != Vector2.zero)
			{
				axis.x = axisDiagonal.x;
				axis.y = axisDiagonal.y;
			}
			lastAxis = Vector2.zero;
			return;
		}
		durationAxis -= delta;
		if (axis != Vector2.zero && axis != lastAxis)
		{
			durationAxis = 0.25f;
			lastAxis = axis;
		}
		else if (durationAxis > 0f)
		{
			axis = lastAxis;
		}
	}

	public static int GetHotkeyDown()
	{
		int result = -1;
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			result = 0;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			result = 1;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			result = 2;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			result = 3;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha5))
		{
			result = 4;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha6))
		{
			result = 5;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha7))
		{
			result = 6;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha8))
		{
			result = 7;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha9))
		{
			result = 8;
		}
		return result;
	}

	public static int GetFunctionkeyDown()
	{
		int result = -1;
		if (Input.GetKeyDown(KeyCode.F1))
		{
			result = 0;
		}
		else if (Input.GetKeyDown(KeyCode.F2))
		{
			result = 1;
		}
		else if (Input.GetKeyDown(KeyCode.F3))
		{
			result = 2;
		}
		else if (Input.GetKeyDown(KeyCode.F4))
		{
			result = 3;
		}
		else if (Input.GetKeyDown(KeyCode.F5))
		{
			result = 4;
		}
		else if (Input.GetKeyDown(KeyCode.F6))
		{
			result = 5;
		}
		else if (Input.GetKeyDown(KeyCode.F7))
		{
			result = 6;
		}
		else if (Input.GetKeyDown(KeyCode.F8))
		{
			result = 7;
		}
		return result;
	}

	public static int GetHotkey()
	{
		int result = -1;
		if (Input.GetKey(KeyCode.Alpha1))
		{
			result = 0;
		}
		else if (Input.GetKey(KeyCode.Alpha2))
		{
			result = 1;
		}
		else if (Input.GetKey(KeyCode.Alpha3))
		{
			result = 2;
		}
		else if (Input.GetKey(KeyCode.Alpha4))
		{
			result = 3;
		}
		else if (Input.GetKey(KeyCode.Alpha5))
		{
			result = 4;
		}
		else if (Input.GetKey(KeyCode.Alpha6))
		{
			result = 5;
		}
		else if (Input.GetKey(KeyCode.Alpha7))
		{
			result = 6;
		}
		else if (Input.GetKey(KeyCode.Alpha8))
		{
			result = 7;
		}
		else if (Input.GetKey(KeyCode.Alpha9))
		{
			result = 8;
		}
		return result;
	}

	private static EAction GetAction()
	{
		string inputString = Input.inputString;
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			return EAction.Cancel;
		}
		if (Input.GetKeyDown(keys.wait.key) || Input.GetKey(KeyCode.Keypad5))
		{
			return EAction.Wait;
		}
		if (Input.GetKeyDown(keys.emptyHand.key))
		{
			return EAction.EmptyHand;
		}
		if (Input.GetKeyDown(keys.autoCombat.key))
		{
			return EAction.AutoCombat;
		}
		if (Input.GetKeyDown(keys.examine.key))
		{
			return EAction.Examine;
		}
		if (Input.GetKeyDown(keys.getAll.key))
		{
			return EAction.GetAll;
		}
		if (Input.GetKeyDown(keys.dump.key))
		{
			return EAction.Dump;
		}
		if (Input.GetKeyDown(keys.mute.key))
		{
			return EAction.Mute;
		}
		if (Input.GetKeyDown(keys.meditate.key))
		{
			return EAction.Meditate;
		}
		if (Input.GetKeyDown(keys.search.key))
		{
			return EAction.Search;
		}
		if (keyFire.Update())
		{
			return keyFire.Action;
		}
		if (keyWait.Update())
		{
			return keyWait.Action;
		}
		if (!isShiftDown)
		{
			if (Input.GetKeyDown(keys.chara.key))
			{
				return EAction.MenuChara;
			}
			if (Input.GetKeyDown(keys.journal.key))
			{
				return EAction.MenuJournal;
			}
			if (Input.GetKeyDown(keys.inventory.key))
			{
				return EAction.MenuInventory;
			}
			if (Input.GetKeyDown(keys.ability.key))
			{
				return EAction.MenuAbility;
			}
			if (Input.GetKeyDown(keys.log.key))
			{
				return EAction.Log;
			}
			if (Input.GetKeyDown(keys.report.key))
			{
				return EAction.Report;
			}
			if (Input.GetKeyDown(keys.quickSave.key))
			{
				return EAction.QuickSave;
			}
			if (Input.GetKeyDown(keys.quickLoad.key))
			{
				return EAction.QuickLoad;
			}
		}
		if (!(inputString == "?"))
		{
			if (inputString == "g")
			{
				return EAction.ShowGrid;
			}
			return EAction.None;
		}
		return EAction.Help;
	}

	public static void RestoreDefaultKeys()
	{
	}

	public static void Select(GameObject go)
	{
		eventSystem.SetSelectedGameObject(null);
		eventSystem.SetSelectedGameObject(go);
	}
}
