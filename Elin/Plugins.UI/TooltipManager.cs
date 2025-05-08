using UnityEngine;

public class TooltipManager : MonoBehaviour
{
	public static TooltipManager Instance;

	public UITooltip[] tooltips;

	public string disableHide;

	public float disableTimer;

	private void Awake()
	{
		Instance = this;
	}

	public void ResetTooltips()
	{
		UITooltip[] array = tooltips;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].data = null;
		}
	}

	public void ShowTooltip(TooltipData data, Transform target)
	{
		if (disableTimer > 0f)
		{
			return;
		}
		if ((bool)UIContextMenu.Current)
		{
			UIButton componentOf = InputModuleEX.GetComponentOf<UIButton>();
			if (!componentOf || !componentOf.transform.GetComponentInParent<UIContextMenu>())
			{
				return;
			}
		}
		UITooltip uITooltip = tooltips[0];
		UITooltip[] array = tooltips;
		foreach (UITooltip uITooltip2 in array)
		{
			if (uITooltip2.name == data.id)
			{
				uITooltip = uITooltip2;
			}
		}
		uITooltip.SetActive(enable: true);
		uITooltip.SetData(data);
		if (uITooltip.cg.alpha < 0.1f)
		{
			uITooltip.cg.alpha = 1f;
		}
		float timer = 0f;
		uITooltip.hideFunc = delegate
		{
			if (target == null || !InputModuleEX.IsPointerOver(target))
			{
				timer += Time.smoothDeltaTime;
				return timer > 0.2f;
			}
			timer = 0f;
			return false;
		};
		if (uITooltip.followType == UITooltip.FollowType.None)
		{
			return;
		}
		Vector3 vector = ((uITooltip.followType == UITooltip.FollowType.Mouse) ? EInput.uiMousePosition : target.position);
		if (uITooltip.followType == UITooltip.FollowType.Pivot)
		{
			GameObject gameObject = target.FindTagInParents("PivotTooltip", includeInactive: false);
			if ((bool)gameObject)
			{
				uITooltip.Rect().pivot = gameObject.transform.Rect().pivot;
				vector = gameObject.transform.position;
			}
			else
			{
				uITooltip.Rect().pivot = uITooltip.orgPivot;
			}
		}
		uITooltip.transform.position = vector + data.offset + uITooltip.offset;
		Util.ClampToScreen(uITooltip.Rect(), 20);
	}

	public void HideTooltips(bool immediate = false)
	{
		UITooltip[] array = tooltips;
		foreach (UITooltip uITooltip in array)
		{
			if (immediate)
			{
				uITooltip.cg.alpha = 0f;
				uITooltip.data = null;
				uITooltip.SetActive(enable: false);
				continue;
			}
			float timer = 0f;
			uITooltip.hideFunc = delegate
			{
				timer += Time.smoothDeltaTime;
				return timer > 0.2f;
			};
		}
	}

	private void Update()
	{
		if (disableTimer > 0f)
		{
			disableTimer -= Time.deltaTime;
		}
		UITooltip[] array = tooltips;
		foreach (UITooltip uITooltip in array)
		{
			if (!(uITooltip.name == disableHide) && uITooltip.gameObject.activeSelf)
			{
				if (uITooltip.hideFunc())
				{
					uITooltip.cg.alpha -= Time.smoothDeltaTime * 5f;
				}
				else
				{
					uITooltip.cg.alpha += Time.smoothDeltaTime * 5f;
				}
				if (uITooltip.cg.alpha <= 0f)
				{
					uITooltip.data = null;
					uITooltip.SetActive(enable: false);
				}
			}
		}
	}
}
