using UnityEngine;
using UnityEngine.EventSystems;

public class UIListDragItem : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler
{
	public UIList list;

	public object item;

	public void OnBeginDrag(PointerEventData eventData)
	{
		list.BeginItemDrag(this);
	}

	public void OnDrag(PointerEventData eventData)
	{
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		list.EndItemDrag(this);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		list.UpdateItemDragHover(this);
	}
}
