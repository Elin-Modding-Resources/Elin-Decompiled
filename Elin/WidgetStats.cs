using System.Collections.Generic;

public class WidgetStats : BaseWidgetNotice
{
	public static WidgetStats Instance;

	public List<NotificationCondition> conditions = new List<NotificationCondition>();

	public ItemNotice moldBuff;

	public ItemNotice moldStance;

	public static void RefreshAll()
	{
		if ((bool)Instance)
		{
			Instance._RefreshAll();
		}
	}

	public override void _OnActivate()
	{
		Instance = this;
		Add(new NotificationStats
		{
			stats = () => EMono.pc.hunger
		});
		Add(new NotificationStats
		{
			stats = () => EMono.pc.burden
		});
		Add(new NotificationStats
		{
			stats = () => EMono.pc.depression
		});
		Add(new NotificationStats
		{
			stats = () => EMono.pc.bladder
		});
		Add(new NotificationStats
		{
			stats = () => EMono.pc.hygiene
		});
		Add(new NotificationStats
		{
			stats = () => EMono.pc.sleepiness
		});
		Add(new NotificationStats
		{
			stats = () => EMono.pc.stamina
		});
		Add(new NotificationExceedParty());
	}

	public override void OnRefresh()
	{
		conditions.ForeachReverse(delegate(NotificationCondition a)
		{
			if (a.ShouldRemove())
			{
				conditions.Remove(a);
				Remove(a);
				dirty = true;
			}
		});
		foreach (Condition condition in EMono.pc.conditions)
		{
			if (!condition.ShowInWidget)
			{
				continue;
			}
			bool flag = true;
			foreach (NotificationCondition condition2 in conditions)
			{
				if (condition2.condition == condition)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				NotificationCondition notificationCondition = condition.CreateNotification() as NotificationCondition;
				Add(notificationCondition);
				conditions.Add(notificationCondition);
			}
		}
	}
}
