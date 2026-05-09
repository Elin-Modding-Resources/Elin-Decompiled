public class Zone_Town : Zone_Civilized
{
	public override bool IsTown => true;

	public override bool IsExplorable => false;

	public override bool CanDigUnderground => false;

	public override bool CanSpawnAdv => base.lv == 0;

	public override bool AllowCriminal => false;

	public override void OnRegenerate()
	{
		if (EClass.rnd(5) == 0)
		{
			Add("mad_rich");
		}
		bool flag = this is Zone_Kapul && IsFestival;
		if (EClass.rnd((EClass.debug.enable || flag) ? 1 : 8) == 0)
		{
			Add((EClass.rnd((EClass.debug.enable || flag) ? 1 : 4) == 0) ? "unseenhand" : "murderer").AddCondition<ConTransmuteHuman>();
		}
		Chara Add(string id)
		{
			Chara chara = CharaGen.Create(id);
			chara.isSubsetCard = true;
			EClass._zone.AddCard(chara, GetSpawnPos(chara));
			return chara;
		}
	}
}
