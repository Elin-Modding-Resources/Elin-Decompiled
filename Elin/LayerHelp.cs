public class LayerHelp : ELayer
{
	public static string lastIdFile;

	public static string lastIdTopic;

	public UIBook book;

	public override bool blockWidgetClick => false;

	public override void OnInit()
	{
		if (book.mode == UIBook.Mode.Book)
		{
			SkinManager.LimitFontSizeMod_UI = -2;
		}
	}

	public override void OnAfterInit()
	{
		switch (book.mode)
		{
		case UIBook.Mode.Announce:
			book.Show("version" + (ELayer.core.version.demo ? "_demo" : ""));
			break;
		case UIBook.Mode.About:
			book.Show("about_EA");
			break;
		}
	}

	public static void Toggle(string idFile, string idTopic = null)
	{
		LayerHelp layerHelp = ELayer.ui.ToggleLayer<LayerHelp>();
		if (layerHelp != null)
		{
			layerHelp.book.Show(idFile, idTopic);
		}
	}

	public override void OnRightClick()
	{
		if ((bool)book.inputSearch && book.transSearchResult.gameObject.activeSelf)
		{
			book.showSearchResult = false;
		}
		else
		{
			base.OnRightClick();
		}
	}

	private void OnDestroy()
	{
		if (book.mode == UIBook.Mode.Help)
		{
			lastIdFile = book.idFile;
			lastIdTopic = book.idTopic;
		}
		SkinManager.LimitFontSizeMod_UI = 999;
	}
}
