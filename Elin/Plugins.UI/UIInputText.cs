using System;
using UnityEngine;
using UnityEngine.UI;

public class UIInputText : MonoBehaviour
{
	public enum Type
	{
		Number,
		Name
	}

	public Type type;

	public InputField field;

	public bool number;

	public bool showMax;

	public int min;

	public int max;

	public Action<int> onValueChanged;

	public Text textMax;

	public UIButton[] buttons;

	private float timer;

	private UIButton buttonClicked;

	public int Num
	{
		get
		{
			int.TryParse(field.text, out var result);
			return result;
		}
		set
		{
			field.SetTextWithoutNotify(value.ToString() ?? "");
		}
	}

	public string Text
	{
		get
		{
			return field.text;
		}
		set
		{
			field.SetTextWithoutNotify(value);
		}
	}

	private void Awake()
	{
		field.onValueChanged.AddListener(OnValueChange);
		buttons = GetComponentsInChildren<UIButton>();
		field.textComponent.font = SkinManager.Instance.fontSet.ui.source.font;
	}

	public void OnValueChange(string s)
	{
		Validate();
		if (onValueChanged != null)
		{
			onValueChanged(Num);
		}
	}

	public void SetMinMax(int _min, int _max)
	{
		min = _min;
		max = _max;
		if ((bool)textMax)
		{
			textMax.text = "/" + max;
		}
	}

	public void SetMin()
	{
		Num = min;
		OnValueChange("");
		SE.Click();
	}

	public void SetMax()
	{
		Num = max;
		OnValueChange("");
		SE.Click();
	}

	public void ModNum(int a)
	{
		int num = Num;
		Num += a;
		OnValueChange("");
		if (num != Num)
		{
			SE.Click();
		}
		else
		{
			SE.BeepSmall();
		}
	}

	public void Validate()
	{
		if (type == Type.Number)
		{
			if (Num < min)
			{
				Num = min;
			}
			if (Num > max)
			{
				Num = max;
			}
		}
	}

	public void Focus()
	{
		field.ActivateInputField();
		field.Select();
	}

	private void Update()
	{
		timer -= Time.deltaTime;
		if (EInput.leftMouse.pressedLong)
		{
			if (!(timer < 0f))
			{
				return;
			}
			timer = 0.1f;
			UIButton componentOf = InputModuleEX.GetComponentOf<UIButton>();
			UIButton[] array = buttons;
			foreach (UIButton uIButton in array)
			{
				if (uIButton == componentOf && uIButton == buttonClicked)
				{
					uIButton.onClick.Invoke();
				}
			}
		}
		else if (EInput.leftMouse.down)
		{
			buttonClicked = InputModuleEX.GetComponentOf<UIButton>();
		}
	}
}
