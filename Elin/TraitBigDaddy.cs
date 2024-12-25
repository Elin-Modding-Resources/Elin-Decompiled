public class TraitBigDaddy : TraitChara
{
	public override bool IsCountAsResident => true;

	public override int MaxRandomAbility
	{
		get
		{
			if (EClass._zone.DangerLv < 50)
			{
				return 0;
			}
			return 3;
		}
	}
}
