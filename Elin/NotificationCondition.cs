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
		switch (condition.Type)
		{
		case ConditionType.Stance:
			num = 0;
			break;
		case ConditionType.Buff:
			num = 10;
			break;
		case ConditionType.Debuff:
			num = (sort ? 11 : 10);
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
		return 8000000 + num * 100000 + (sort ? condition.GetSortVal() : instance.list.IndexOf(this));
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
