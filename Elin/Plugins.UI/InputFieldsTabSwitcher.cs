using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputFieldsTabSwitcher : ManlySingleton<InputFieldsTabSwitcher>
{
	private EventSystem currentEventSystem;

	private List<UiInputField> activeInputs = new List<UiInputField>();

	private int currentActiveInputIndex = -1;

	private void Start()
	{
		currentEventSystem = EventSystem.current;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Tab))
		{
			TrySelectNextInput();
		}
	}

	private void TrySelectNextInput()
	{
		CheckIfAnyInputIsSelected();
		SelectNextInput();
	}

	private void CheckIfAnyInputIsSelected()
	{
		GameObject selectedObject = currentEventSystem.currentSelectedGameObject;
		if (selectedObject != null)
		{
			currentActiveInputIndex = activeInputs.FindIndex((UiInputField x) => x.gameObject == selectedObject);
		}
	}

	private void SelectNextInput()
	{
		if (currentActiveInputIndex > -1)
		{
			activeInputs[currentActiveInputIndex].DeactivateInputField();
			IterateSelectableIndex();
			currentEventSystem.SetSelectedGameObject(activeInputs[currentActiveInputIndex].gameObject);
			activeInputs[currentActiveInputIndex].ActivateInputField();
		}
	}

	private void IterateSelectableIndex()
	{
		currentActiveInputIndex++;
		currentActiveInputIndex %= activeInputs.Count;
	}

	public void RegisterInputField(UiInputField input)
	{
		if (!activeInputs.Contains(input))
		{
			activeInputs.Add(input);
		}
	}

	public void UnregisterInputField(UiInputField input)
	{
		if (activeInputs.Contains(input))
		{
			activeInputs.Remove(input);
		}
	}
}
