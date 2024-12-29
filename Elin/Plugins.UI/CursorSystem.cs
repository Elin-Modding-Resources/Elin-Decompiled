using System.Collections.Generic;
using UnityEngine;

public class CursorSystem : MonoBehaviour
{
	public static CursorSystem Instance;

	public static CursorInfo currentCursor;

	public static CursorInfo lastCursor;

	public static CursorInfo leftIcon;

	public static CursorInfo lastLeftIcon;

	public static CursorInfo arrowIcon;

	public static CursorInfo lastArrowIcon;

	public static float leftIconAngle;

	public static Vector3 position;

	public static Vector3 posOrigin;

	public static int ignoreCount;

	public List<CursorInfo> cursors;

	public List<CursorInfo> icons;

	public List<CursorInfo> arrows;

	public SpriteRenderer srArrow;

	public SpriteRenderer srLeftIcon;

	public bool showArrowOrigin;

	public bool iconHidesCursor;

	public bool disable;

	private static int priority;

	public static CursorInfo Arrow => Instance.cursors[0];

	public static CursorInfo ResizeNS => Instance.cursors[1];

	public static CursorInfo ResizeWE => Instance.cursors[2];

	public static CursorInfo ResizeNWSE => Instance.cursors[3];

	public static CursorInfo ResizeNESW => Instance.cursors[4];

	public static CursorInfo Move => Instance.cursors[5];

	public static CursorInfo Select => Instance.cursors[6];

	public static CursorInfo Hand => Instance.cursors[7];

	public static CursorInfo Cut => Instance.cursors[8];

	public static CursorInfo Mine => Instance.cursors[9];

	public static CursorInfo Dig => Instance.cursors[10];

	public static CursorInfo Cancel => Instance.cursors[11];

	public static CursorInfo Build => Instance.cursors[12];

	public static CursorInfo Picker => Instance.cursors[13];

	public static CursorInfo Eye => Instance.cursors[14];

	public static CursorInfo Notice => Instance.cursors[15];

	public static CursorInfo Door => Instance.cursors[16];

	public static CursorInfo Kick => Instance.cursors[17];

	public static CursorInfo Container => Instance.cursors[18];

	public static CursorInfo Lock => Instance.cursors[19];

	public static CursorInfo MoveZone => Instance.cursors[20];

	public static CursorInfo Target => Instance.cursors[21];

	public static CursorInfo Craft => Instance.cursors[22];

	public static CursorInfo Inventory => Instance.cursors[23];

	public static CursorInfo Wait => Instance.cursors[24];

	public static CursorInfo See => Instance.cursors[25];

	public static CursorInfo Question => Instance.cursors[26];

	public static CursorInfo Invalid => Instance.cursors[27];

	public static CursorInfo Action => Instance.cursors[7];

	public static CursorInfo IconArrow => Instance.icons[0];

	public static CursorInfo IconMelee => Instance.icons[1];

	public static CursorInfo IconRange => Instance.icons[2];

	public static CursorInfo IconCut => Instance.icons[3];

	public static CursorInfo IconMine => Instance.icons[4];

	public static CursorInfo IconGear => Instance.icons[5];

	public static CursorInfo IconChat => Instance.icons[6];

	public static void SetCursor(CursorInfo info = null, int _priority = 0)
	{
		if (ignoreCount <= 0 && _priority >= priority)
		{
			priority = _priority;
			if (info == null)
			{
				currentCursor = Arrow;
			}
			else
			{
				currentCursor = info;
			}
		}
	}

	public void Draw()
	{
		if (disable)
		{
			return;
		}
		ignoreCount--;
		base.transform.position = position;
		priority = 0;
		if (currentCursor != lastCursor)
		{
			Cursor.SetCursor(currentCursor.Texture, currentCursor.Hotspot, CursorMode.Auto);
			lastCursor = currentCursor;
		}
		if (leftIcon != lastLeftIcon)
		{
			if (leftIcon == null)
			{
				srLeftIcon.SetActive(enable: false);
			}
			else
			{
				srLeftIcon.SetActive(enable: true);
				srLeftIcon.sprite = leftIcon.sprite;
			}
			lastLeftIcon = leftIcon;
		}
		srLeftIcon.transform.SetEulerAnglesZ(leftIconAngle);
	}

	public void Awake()
	{
		Instance = this;
		SetCursor();
		srLeftIcon.SetActive(enable: false);
		srArrow.SetActive(enable: false);
	}
}
