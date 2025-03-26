using UnityEngine.UI;

public class NotificationBuff : NotificationCondition
{
	public override int idSprite => 0;

	public override bool Interactable => true;

	public override ItemNotice GetMold()
	{
		return WidgetStats.Instance.moldBuff;
	}

	public override LayoutGroup GetLayoutGroup()
	{
		return WidgetStats.Instance.layout2;
	}

	public override void OnRefresh()
	{
		if (item.button.icon.sprite == EClass.core.refs.spriteDefaultCondition)
		{
			OnInstantiate();
		}
		text = condition.GetText() + (EClass.debug.showExtra ? (" " + condition.value) : "");
		item.textDuration.SetText(condition.TextDuration);
		item.textDuration.SetActive(condition.HasDuration);
	}

	public override void OnInstantiate()
	{
		item.button.icon.sprite = condition.GetSprite();
		item.button.icon.color = condition.GetSpriteColor();
		item.button.icon.SetNativeSize();
	}
}
