using System.Collections.Generic;

public class WidgetStats : BaseWidgetNotice
{
	public static WidgetStats Instance;

	public List<NotificationCondition> conditions = new List<NotificationCondition>();

	public List<NotificationCooldown> cds = new List<NotificationCooldown>();

	public ItemNotice moldBuff;

	public ItemNotice moldStance;

	public ItemNotice moldCooldown;

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
		cds.ForeachReverse(delegate(NotificationCooldown a)
		{
			if (a.ShouldRemove())
			{
				cds.Remove(a);
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
		if (EMono.pc._cooldowns == null)
		{
			return;
		}
		foreach (int cooldown in EMono.pc._cooldowns)
		{
			int num = cooldown / 1000;
			bool flag2 = true;
			foreach (NotificationCooldown cd in cds)
			{
				if (cd.idEle == num)
				{
					flag2 = false;
					break;
				}
			}
			if (flag2)
			{
				NotificationCooldown notificationCooldown = new NotificationCooldown
				{
					idEle = num
				};
				Add(notificationCooldown);
				cds.Add(notificationCooldown);
			}
		}
	}
}
