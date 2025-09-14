using System;
using UnityEngine;

public class NotificationCondition : BaseNotification
{
	public Condition condition;

	public override bool Visible => !text.IsEmpty();

	public override bool Interactable => !condition.source.GetDetail().IsEmpty();

	public override Action<UITooltip> onShowTooltip => delegate(UITooltip t)
	{
		condition.WriteNote(t.note);
	};

	public override Sprite Sprite => condition.GetSprite();

	public override bool ShouldRemove()
	{
		if (condition.IsKilled)
		{
			return true;
		}
		if (EClass.core.IsGameStarted && !EClass.pc.conditions.Contains(condition))
		{
			return true;
		}
		return false;
	}

	public override int GetSortVal()
	{
		int num = 5;
		WidgetStats instance = WidgetStats.Instance;
		bool sort = instance.extra.sort;
		bool sort_ascend = instance.extra.sort_ascend;
		switch (condition.Type)
		{
		case ConditionType.Stance:
			num = 0;
			break;
		case ConditionType.Buff:
			num = 10;
			break;
		case ConditionType.Debuff:
			num = ((!sort) ? 10 : (sort_ascend ? 9 : 11));
			break;
		}
		if (condition is ConBuffStats)
		{
			num = 1;
		}
		if (condition is ConAwakening)
		{
			num = 2;
		}
		int num2 = (sort ? condition.GetSortVal() : instance.list.IndexOf(this));
		if ((!sort && !sort_ascend) || (sort && sort_ascend))
		{
			num2 = 100000 - num2;
		}
		return 8000000 + num * 100000 + num2;
	}

	public override void OnClick()
	{
		if (condition.CanManualRemove)
		{
			SE.Trash();
			condition.Kill();
			WidgetStats.RefreshAll();
			if ((bool)WidgetStats.Instance)
			{
				WidgetStats.Instance.layout.RebuildLayout();
			}
		}
		else
		{
			SE.Beep();
		}
	}

	public override void OnRefresh()
	{
		text = condition.GetText() + (EClass.debug.showExtra ? (" " + condition.value) : "");
		item.button.mainText.color = condition.GetColor(item.button.skinRoot.GetButton().colorProf);
	}
}
