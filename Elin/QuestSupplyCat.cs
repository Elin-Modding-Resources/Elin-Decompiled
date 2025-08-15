using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

public class QuestSupplyCat : QuestSupply
{
	[JsonProperty]
	public string idCat;

	public SourceCategory.Row Cat => EClass.sources.categories.map[idCat];

	public override string RefDrama2 => Cat.GetName();

	public override string NameDeliver => Cat.GetName();

	public override bool IsDestThing(Thing t)
	{
		if (!t.c_isImportant && !t.isEquipped && t.category.IsChildOf(idCat))
		{
			return t.things.Count == 0;
		}
		return false;
	}

	public override void SetIdThing()
	{
		List<SourceCategory.Row> list = EClass.sources.categories.rows.Where((SourceCategory.Row c) => c._parent == "meal" && c.id != "meal_lunch").ToList();
		idCat = list.RandomItem().id;
	}

	public override string GetTextProgress()
	{
		string @ref = ((GetDestThing() != null) ? "supplyInInv".lang().TagColor(FontColor.Good) : "supplyNotInInv".lang());
		return "progressSupply".lang(Cat.GetName() + Lang.space + TextExtra2.IsEmpty(""), @ref);
	}
}
