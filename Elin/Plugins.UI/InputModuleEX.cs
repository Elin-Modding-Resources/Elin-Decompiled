using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputModuleEX : StandaloneInputModule
{
	public static PointerEventData eventData;

	public static GameObject topGameObject;

	public static List<GameObject> list;

	public static InputModuleEX Instance;

	public static void UpdateEventData()
	{
		Instance.GetPointerData(-1, out eventData, create: true);
		list = eventData.hovered;
		topGameObject = ((list.Count == 0) ? null : list[list.Count - 1]);
	}

	public static PointerEventData GetPointerEventData(int pointerId = -1)
	{
		return eventData;
	}

	public static List<GameObject> GetList()
	{
		return eventData.hovered;
	}

	public static bool IsPointerOverRoot(Component c)
	{
		foreach (GameObject item in list)
		{
			if ((bool)item && item.IsChildOf(c.gameObject))
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsPointerOver(Component c)
	{
		Transform transform = c.transform;
		foreach (GameObject item in list)
		{
			if ((bool)item && item.transform == transform)
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsPointerChildOf(Component c)
	{
		foreach (GameObject item in list)
		{
			if ((bool)item && item.IsChildOf(c.gameObject))
			{
				return true;
			}
		}
		return false;
	}

	public static T GetComponentOf<T>() where T : Component
	{
		foreach (GameObject item in list)
		{
			if ((bool)item)
			{
				T component = item.GetComponent<T>();
				if ((bool)component)
				{
					return component;
				}
			}
		}
		return null;
	}

	public static T GetTopComponentOf<T>() where T : Component
	{
		foreach (GameObject item in list)
		{
			if ((bool)item)
			{
				return item.GetComponent<T>();
			}
		}
		return null;
	}

	protected override void Awake()
	{
		base.Awake();
		Instance = this;
		UpdateEventData();
	}
}
