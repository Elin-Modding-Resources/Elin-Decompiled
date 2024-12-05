public class TraitAdventurer : TraitChara
{
	public override bool UseGlobalGoal => base.owner.IsGlobal;

	public override bool UseRandomAbility => true;

	public override bool UseRandomAlias => true;

	public override bool ShowAdvRank => true;

	public override bool HaveNews => true;

	public override bool CanBout => true;

	public override Adv_Type AdvType
	{
		get
		{
			if (!(base.owner.id == "adv"))
			{
				return Adv_Type.Adv_Fairy;
			}
			return Adv_Type.Adv;
		}
	}
}