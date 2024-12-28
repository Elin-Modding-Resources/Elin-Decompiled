using System.Linq;
using UnityEngine;

public class ContentPolicy : EContent
{
	public UIList listPolicyLaw;

	public UIList listPolicyEconomy;

	public UIList listPolicyLife;

	public UIList listUtility;

	public Material matGrayscale;

	public override void OnSwitchContent(int idTab)
	{
		EClass.Branch.policies.Validate();
		RefreshPolicy();
	}

	public void RefreshPolicy()
	{
		RefreshPolicyList(listPolicyLaw, "law", "department_law");
		RefreshPolicyList(listPolicyEconomy, "economy", "department_economy");
		RefreshPolicyList(listPolicyLife, "life", "department_life");
		RefreshPolicyList(listUtility, "utility", "department_utility");
		listUtility.SetActive(EClass.Branch.lv >= 2 || EClass.debug.enable);
		RefreshPolicyIcons();
		GetComponentInParent<Layer>().RebuildLayout(recursive: true);
		this.RebuildLayout(recursive: true);
	}

	public void RefreshPolicyList(UIList list, string cat, string plan)
	{
		list.callbacks = new UIList.Callback<Policy, UIButton>
		{
			onClick = delegate
			{
			},
			onInstantiate = delegate(Policy a, UIButton b)
			{
				b.mainText.SetText(a.Name);
				b.onClick.AddListener(delegate
				{
					if (!a.active)
					{
						if ((a.Cost > 0 && EClass.Branch.policies.CurrentAP() + a.Cost > EClass.Branch.MaxAP) || !CanActivate(a))
						{
							SE.Beep();
							return;
						}
						if (a.id == 2514 && EClass.pc.faction.CountTaxFreeLand() >= 3)
						{
							SE.Beep();
							return;
						}
					}
					SE.ClickOk();
					a.active = !a.active;
					if (a.id == 2705)
					{
						EClass.pc.faction.SetGlobalPolicyActive(2705, a.active);
					}
					if (a.id == 2708)
					{
						EClass.pc.faction.SetGlobalPolicyActive(2708, a.active);
					}
					if (a.id == 2710)
					{
						EClass.pc.faction.SetGlobalPolicyActive(2710, a.active);
					}
					if (a.id == 2711)
					{
						EClass.pc.faction.SetGlobalPolicyActive(2711, a.active);
					}
					if (a.id == 2712)
					{
						EClass.pc.faction.SetGlobalPolicyActive(2712, a.active);
					}
					EClass.Branch.policies.RefreshEffects();
					RefreshPolicyIcons();
				});
				b.refObj = a;
				b.tooltip.onShowTooltip = delegate(UITooltip tp)
				{
					a.WriteNote(tp.note);
				};
			},
			onList = delegate
			{
				foreach (Policy item in EClass.Branch.policies.list.Where((Policy p) => p.source.categorySub == cat))
				{
					list.Add(item);
				}
			},
			onSort = (Policy a, UIList.SortMode m) => a.GetSortVal(m),
			onRefresh = null
		};
		list.List();
		list.GetComponentInChildren<CanvasGroup>().alpha = ((list.items.Count > 0) ? 0.9f : 0.4f);
		static bool CanActivate(Policy p)
		{
			if (p.id == 2515 && EClass.Branch.policies.IsActive(2516))
			{
				return false;
			}
			if (p.id == 2516 && EClass.Branch.policies.IsActive(2515))
			{
				return false;
			}
			if (p.id == 2814 && EClass.Branch.policies.IsActive(2823))
			{
				return false;
			}
			if (p.id == 2823 && EClass.Branch.policies.IsActive(2814))
			{
				return false;
			}
			return true;
		}
	}

	public void RefreshPolicyIcons()
	{
		int num = EClass.Branch.policies.CurrentAP();
		UIList[] array = new UIList[4] { listPolicyLaw, listPolicyLife, listPolicyEconomy, listUtility };
		for (int i = 0; i < array.Length; i++)
		{
			UIButton[] componentsInChildren = array[i].GetComponentsInChildren<UIButton>();
			foreach (UIButton obj in componentsInChildren)
			{
				Policy policy = obj.refObj as Policy;
				obj.icon.material = (policy.active ? null : matGrayscale);
				obj.imageCheck.SetActive(policy.active);
				obj.subText.SetText(policy.Cost.ToString() ?? "", (num + policy.Cost <= EClass.Branch.MaxAP || policy.active) ? FontColor.Good : FontColor.Bad);
			}
		}
	}
}
