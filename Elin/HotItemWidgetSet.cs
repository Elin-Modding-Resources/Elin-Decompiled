using Newtonsoft.Json;

public class HotItemWidgetSet : HotAction
{
	[JsonProperty]
	public WidgetManager.SaveData data;

	public override string Id => "WidgetSet";

	public override string TextTip => "loadWidget".lang();

	public override bool CanChangeIconColor => true;

	public HotItemWidgetSet Register()
	{
		EClass.ui.widgets.UpdateConfigs();
		data = IO.DeepCopy(EClass.player.widgets);
		SE.WriteJournal();
		return this;
	}

	public override void Perform()
	{
		Load();
	}

	public void Load()
	{
		if (EClass.player.useSubWidgetTheme)
		{
			EClass.player.subWidgets = data;
		}
		else
		{
			EClass.player.mainWidgets = data;
		}
		EClass.ui.widgets.Reset(toggleTheme: false);
		SE.Click();
	}

	public override void OnShowContextMenu(UIContextMenu m)
	{
		base.OnShowContextMenu(m);
		m.AddButton("hotActionWidgetSet", delegate
		{
			Register();
		});
	}
}
