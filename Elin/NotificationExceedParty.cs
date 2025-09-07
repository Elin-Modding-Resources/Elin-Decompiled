using System;

public class NotificationExceedParty : BaseNotification
{
	public override bool Visible => EClass.pc.party.members.Count - 1 > EClass.player.MaxAlly;

	public override Action<UITooltip> onShowTooltip => delegate(UITooltip a)
	{
		a.note.Clear();
		a.note.AddHeader("exceedParty");
		a.note.AddText("exceedParty_tip".lang((EClass.pc.party.members.Count - 1).ToString() ?? "", EClass.player.MaxAlly.ToString() ?? ""));
		a.note.Build();
	};

	public override int GetSortVal()
	{
		return 2000000;
	}

	public override void OnRefresh()
	{
		text = "exceedParty".lang();
	}
}
