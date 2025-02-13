public class ContentHomeLog : EContent
{
	public UIList listLog;

	public ItemStatistics stat;

	public ItemStatistics statLast;

	public override void OnSwitchContent(int idTab)
	{
		stat.Refresh(EClass.Branch.statistics);
		RefreshLog();
	}

	public void RefreshLog()
	{
		UIList uIList = listLog;
		uIList.callbacks = new UIList.Callback<MsgLog.Data, UIItem>
		{
			onInstantiate = delegate(MsgLog.Data a, UIItem b)
			{
				b.text1.text = a.text.ToTitleCase();
				if (!a.col.IsEmpty())
				{
					b.text1.SetColor(a.col.ToEnum<FontColor>());
				}
				b.text2.text = a.date.month + "/" + a.date.day + " " + ((a.date.hour < 10) ? "0" : "") + a.date.hour + ":" + ((a.date.min < 10) ? "0" : "") + a.date.min;
			}
		};
		uIList.Clear();
		foreach (MsgLog.Data item in EClass.Branch.log.GetList(reverse: true))
		{
			uIList.Add(item);
		}
		uIList.Refresh();
		this.RebuildLayout(recursive: true);
	}
}
