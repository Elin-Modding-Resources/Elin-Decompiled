public class TraitPotionRandom : TraitPotion
{
	public static ElementSelecter selecter = new ElementSelecter
	{
		type = "P",
		lvMod = 10
	};

	public override int Power => 200;

	public override SourceElement.Row source => EClass.sources.elements.map[owner.refVal];

	public override string AliasEle => source.aliasRef;

	public override EffectId IdEffect
	{
		get
		{
			if (!source.proc.IsEmpty())
			{
				return source.proc[0].ToEnum<EffectId>();
			}
			return EffectId.DrinkWaterDirty;
		}
	}

	public override string N1 => source.proc.TryGet(1, returnNull: true);

	public override bool IsNeg => source.tag.Contains("neg");

	public override void OnCreate(int lv)
	{
		TraitPotion.Create(owner, selecter.Select(lv));
	}

	public override SourceElement.Row GetRefElement()
	{
		return source;
	}

	public override int GetValue()
	{
		return source.value * 120 / 100;
	}

	public override string GetName()
	{
		string text;
		if (owner.refVal != 0)
		{
			text = Lang.TryGet("potion_" + source.alias);
			if (text == null)
			{
				return "potion_".lang(source.GetName().ToLower());
			}
		}
		else
		{
			text = base.GetName();
		}
		return text;
	}
}
