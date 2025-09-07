using UnityEngine;

public class NotificationCooldown : BaseNotification
{
	public int idEle;

	public SourceElement.Row row => EClass.sources.elements.map.TryGetValue(idEle, 0);

	public override bool Visible => true;

	public override bool Interactable => false;

	public override Sprite Sprite => null;

	public override ItemNotice GetMold()
	{
		return WidgetStats.Instance.moldCooldown;
	}

	public override int GetSortVal()
	{
		return 1000000 + idEle;
	}

	public override void OnRefresh()
	{
		if (EClass.pc._cooldowns != null)
		{
			int cooldown = EClass.pc.GetCooldown(idEle);
			if (cooldown != 0)
			{
				text = cooldown.ToString() ?? "";
				item.button.subText.text = row.GetName();
			}
		}
	}

	public override bool ShouldRemove()
	{
		if (EClass.pc._cooldowns != null)
		{
			return !EClass.pc.HasCooldown(idEle);
		}
		return true;
	}
}
