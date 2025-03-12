public class TraitPotionRandom : TraitPotion
{
	public static ElementSelecter selecter = new ElementSelecter
	{
		type = "P",
		lvMod = 10
	};

	public override SourceElement.Row source => EClass.sources.elements.map[owner.refVal];

	public override string AliasEle => source.aliasRef;

	public override int Power => 200;

	public override EffectId IdEffect => source.proc[0].ToEnum<EffectId>();

	public override string N1 => source.proc.TryGet(1, returnNull: true);

	public override bool IsNeg => source.tag.Contains("neg");

	public override SourceElement.Row GetRefElement()
	{
		return source;
	}

	public override int GetValue()
	{
		return source.value * 120 / 100;
	}

	public override void OnCreate(int lv)
	{
		owner.refVal = selecter.Select(lv);
	}

	public override string GetName()
	{
		return Lang.TryGet("potion_" + source.alias) ?? "potion_".lang(source.GetName().ToLower());
	}
}
