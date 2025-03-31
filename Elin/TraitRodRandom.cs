public class TraitRodRandom : TraitRod
{
	public static ElementSelecter selecter = new ElementSelecter
	{
		type = "R",
		lvMod = 10
	};

	public override SourceElement.Row source => EClass.sources.elements.map[owner.refVal];

	public override string aliasEle => source.aliasRef;

	public override int Power => 100;

	public override EffectId IdEffect => source.proc[0].ToEnum<EffectId>();

	public override string N1 => source.proc.TryGet(1, returnNull: true);

	public override bool IsNegative
	{
		get
		{
			if (!base.IsNegative)
			{
				return source.tag.Contains("neg");
			}
			return true;
		}
	}

	public override void OnCreate(int lv)
	{
		TraitRod.Create(owner, selecter.Select(lv));
	}

	public override string GetName()
	{
		return "rod_".lang(source.GetName().ToLower());
	}
}
