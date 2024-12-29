using UnityEngine;
using UnityEngine.EventSystems;

public class UIEventUtil : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public bool isPointerOver;

	public void OnPointerEnter(PointerEventData data)
	{
		isPointerOver = true;
	}

	public void OnPointerExit(PointerEventData data)
	{
		isPointerOver = false;
	}
}
