using System;

public class LayerModConfig : ELayer
{
	public UINote note;

	public UIButton buttonReset;

	public UIButton buttonFile;

	public ModPackage current;

	public static void Open(ModPackage p)
	{
		ELayer.ui.AddLayerDontCloseOthers<LayerModConfig>("LayerMod/LayerModConfig").Show(p);
	}

	public override void OnInit()
	{
		buttonReset = windows[0].AddBottomButton("resetConfigTab".lang("mod".lang()), delegate
		{
			Dialog.YesNo("dialogResetConfigTab".lang(windows[0].textCaption.text), delegate
			{
				try
				{
					current.onResetConfig?.Invoke();
				}
				catch (Exception ex)
				{
					ModUtil.LogModError(ex.ToString(), current);
				}
				Show(current);
			});
		});
		buttonFile = windows[0].AddBottomButton("mod_info", delegate
		{
			Util.ShowExplorer(current.configPath);
		});
	}

	public void Show(ModPackage p)
	{
		current = p;
		windows[0].SetCaption(p.title.IsEmpty(p.dirInfo.Name));
		buttonReset.SetActive(p.onResetConfig != null);
		buttonFile.SetActive(!p.configPath.IsEmpty());
		note.Clear();
		windows[0].RebuildLayout();
		note.RebuildLayoutTo<Layer>();
		try
		{
			p.onBuildConfig(note);
		}
		catch (Exception ex)
		{
			ModUtil.LogModError(ex.ToString(), p);
		}
		note.Build();
	}
}
