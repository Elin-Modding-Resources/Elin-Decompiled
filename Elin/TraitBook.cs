public class TraitBook : TraitScroll
{
	public bool IsParchment => this is TraitParchment;

	public string IdItem => owner.GetStr(53);

	public string IdCat
	{
		get
		{
			if (!IsParchment)
			{
				return "Book";
			}
			return "Scroll";
		}
	}

	public BookList.Item Item => BookList.GetItem(IdItem, IdCat);

	public override bool CanStackTo(Thing to)
	{
		return to.GetStr(53) == IdItem;
	}

	public override void OnCreate(int lv)
	{
		if (GetParam(1) != null)
		{
			owner.SetStr(53, GetParam(1));
		}
		else if (IdItem.IsEmpty())
		{
			owner.SetStr(53, BookList.GetRandomItem(IdCat).id);
		}
		owner.idSkin = Item.skin;
	}

	public override void OnImportMap()
	{
		if (GetParam(1) != null)
		{
			owner.SetStr(53, GetParam(1));
		}
	}

	public override void SetName(ref string s)
	{
		s = (IsParchment ? "_parchment" : "_book").lang(s, Item.title);
	}

	public override void OnRead(Chara c)
	{
		BookList.Item item = Item;
		EClass.ui.AddLayer<LayerHelp>(IsParchment ? "LayerParchment" : "LayerBook").book.Show((IsParchment ? "Scroll/" : "Book/") + item.id, null, item.title, item);
	}
}
