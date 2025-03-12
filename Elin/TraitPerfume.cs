public class TraitPerfume : TraitPotionRandom
{
	public new static ElementSelecter selecter = new ElementSelecter
	{
		type = "F",
		lvMod = 10
	};

	public override int GetValue()
	{
		return source.value * 1200 / 100;
	}

	public override void OnCreate(int lv)
	{
		owner.refVal = selecter.Select(lv);
	}

	public override string GetName()
	{
		return Lang.TryGet("perfume_" + source.alias) ?? "perfume_".lang(source.GetName().ToLower());
	}
}
