public class TraitRod : TraitTool
{
	public override bool InvertHeldSprite => true;

	public override bool HasCharges => true;

	public virtual string aliasEle => null;

	public override bool IsNoShop
	{
		get
		{
			if (source != null)
			{
				return source.tag.Contains("noShop");
			}
			return false;
		}
	}

	public virtual SourceElement.Row source => null;

	public virtual int Power
	{
		get
		{
			if (owner.sourceCard.vals.Length <= 2)
			{
				return 100;
			}
			return owner.sourceCard.vals[2].ToInt();
		}
	}

	public virtual bool IsNegative => owner.IsNegativeGift;

	public virtual EffectId IdEffect => owner.sourceCard.vals[1].ToEnum<EffectId>();

	public virtual string N1
	{
		get
		{
			if (owner.sourceCard.vals.Length <= 3)
			{
				return "";
			}
			return owner.sourceCard.vals[3];
		}
	}

	public override bool DisableAutoCombat => true;

	public override SourceElement.Row GetRefElement()
	{
		return source;
	}

	public override void OnCreate(int lv)
	{
		owner.c_charges = EClass.rnd(12);
		if (owner.id == "rod_wish")
		{
			owner.c_charges = 1;
			owner.elements.SetBase(759, 30);
		}
	}

	public override void TrySetHeldAct(ActPlan p)
	{
		p.TrySetAct(new ActZap
		{
			trait = this
		}, owner);
	}

	public static void Create(Card owner, int ele, int charge = -1)
	{
		owner.refVal = ele;
		SourceElement.Row row = EClass.sources.elements.map[ele];
		owner.c_charges = ((charge > 0) ? charge : EClass.rnd(row.charge * 150 / 100));
		if (row.tag.Contains("noCopy"))
		{
			owner.elements.SetBase(759, 10);
		}
	}
}
