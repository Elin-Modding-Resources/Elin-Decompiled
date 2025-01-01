using System;
using UnityEngine;

public class NotificationStats : BaseNotification
{
	public Func<BaseStats> stats;

	public override bool Visible => !text.IsEmpty();

	public override bool Interactable => !stats().source.GetDetail().IsEmpty();

	public override Action<UITooltip> onShowTooltip => delegate(UITooltip t)
	{
		stats().WriteNote(t.note);
	};

	public override Sprite Sprite => stats().GetSprite();

	public override void OnClick()
	{
		EClass.ui.AddLayer<LayerChara>().SetChara(EClass.pc);
	}

	public override void OnRefresh()
	{
		BaseStats baseStats = stats();
		text = baseStats.GetText() + ((EClass.debug.showExtra && !baseStats.GetText().IsEmpty()) ? ("(" + baseStats.GetValue() + ")") : "");
		item.button.mainText.color = baseStats.GetColor(item.button.skinRoot.GetButton().colorProf);
	}
}
