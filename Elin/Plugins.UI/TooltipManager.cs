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
		UITooltip t = tooltips[0];
		UITooltip[] array = tooltips;
		foreach (UITooltip uITooltip in array)
		{
			if (uITooltip.name == data.id)
			{
				t = uITooltip;
			}
		}
		t.SetActive(enable: true);
		t.SetData(data);
		if (t.cg.alpha < 0.1f)
		{
			t.cg.alpha = 1f;
		}
		t.hideTimer = 0f;
		t.hideFunc = delegate
		{
			if (target == null || !InputModuleEX.IsPointerOver(target))
			{
				t.hideTimer += Time.smoothDeltaTime;
				return t.hideTimer > 0.2f;
			}
			t.hideTimer = 0f;
			return false;
		};
		if (t.followType == UITooltip.FollowType.None)
		{
			return;
		}
		Vector3 vector = ((t.followType == UITooltip.FollowType.Mouse) ? EInput.uiMousePosition : target.position);
		if (t.followType == UITooltip.FollowType.Pivot)
		{
			GameObject gameObject = target.FindTagInParents("PivotTooltip", includeInactive: false);
			if ((bool)gameObject)
			{
				t.Rect().pivot = gameObject.transform.Rect().pivot;
				vector = gameObject.transform.position;
			}
			else
			{
				t.Rect().pivot = t.orgPivot;
			}
		}
		t.transform.position = vector + data.offset + t.offset;
		Util.ClampToScreen(t.Rect(), 20);
	}

	public void HideTooltips(bool immediate = false)
	{
		UITooltip[] array = tooltips;
		foreach (UITooltip t in array)
		{
			if (immediate)
			{
				t.cg.alpha = 0f;
				t.data = null;
				t.SetActive(enable: false);
			}
			else
			{
				t.hideFunc = delegate
				{
					t.hideTimer += Time.smoothDeltaTime;
					return t.hideTimer > 0.2f;
				};
			}
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
