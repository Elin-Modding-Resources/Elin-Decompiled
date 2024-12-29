using UnityEngine;

public class UIContextMenuManager : MonoBehaviour
{
	public Canvas canvas;

	public UIContextMenu currentMenu;

	public static UIContextMenuManager Instance;

	public bool showMenuOnRightClick = true;

	public bool autoClose;

	public bool isActive
	{
		get
		{
			if (currentMenu != null)
			{
				return !currentMenu.isDestroyed;
			}
			return false;
		}
	}

	private void Awake()
	{
		Instance = this;
	}

	public UIContextMenu Create(string menuName = "ContextMenu", bool destroyOnHide = true)
	{
		if ((bool)currentMenu)
		{
			currentMenu.Hide();
		}
		currentMenu = Util.Instantiate<UIContextMenu>(menuName, base.transform);
		currentMenu.destroyOnHide = destroyOnHide;
		if ((bool)currentMenu.logo)
		{
			currentMenu.logo.SetActive(enable: true);
		}
		return currentMenu;
	}
}
