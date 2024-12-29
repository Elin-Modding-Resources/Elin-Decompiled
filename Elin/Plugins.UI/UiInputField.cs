using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UiInputField : InputField
{
	public bool dontClearOnESC;

	public Action onUpdate;

	public Action onDisable;

	[SerializeField]
	public bool registerToTabSwitcher;

	public string stringEdit = "";

	public event Action OnSubmitEvent = delegate
	{
	};

	public event Action InputSelected = delegate
	{
	};

	public event Action InputDeselected = delegate
	{
	};

	protected override void Awake()
	{
		base.textComponent.font = SkinManager.Instance.fontSet.ui.source.font;
	}

	protected override void Start()
	{
		base.Start();
		if (registerToTabSwitcher)
		{
			ManlySingleton<InputFieldsTabSwitcher>.Instance.RegisterInputField(this);
		}
		base.onEndEdit.AddListener(OnEndEdit);
		base.onValueChanged.AddListener(OnValueChanged);
		stringEdit = base.text;
	}

	private void Update()
	{
		onUpdate?.Invoke();
	}

	public void HideCaret()
	{
		m_CaretSelectPosition = 0;
	}

	protected override void OnDisable()
	{
		onDisable?.Invoke();
	}

	private void OnValueChanged(string s)
	{
		if (dontClearOnESC && !Input.GetKeyDown(KeyCode.Escape))
		{
			stringEdit = base.text;
		}
	}

	private void OnEndEdit(string s)
	{
		if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
		{
			this.OnSubmitEvent();
		}
		Deselect();
		if (dontClearOnESC && Input.GetKeyDown(KeyCode.Escape))
		{
			base.text = stringEdit;
		}
	}

	private void Deselect()
	{
		if (!EventSystem.current.alreadySelecting)
		{
			EventSystem.current.SetSelectedGameObject(null);
		}
		DeactivateInputField();
	}

	public void Clear()
	{
		base.text = string.Empty;
	}

	public override void OnSelect(BaseEventData eventData)
	{
		base.OnSelect(eventData);
		this.InputSelected();
	}

	public override void OnDeselect(BaseEventData eventData)
	{
		base.OnDeselect(eventData);
		this.InputDeselected();
	}
}
