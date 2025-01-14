public class ConInvisibility : BaseBuff
{
	public override bool SyncRide => true;

	public override bool ShouldRefresh => true;

	public override void OnStart()
	{
		owner.isHidden = true;
		foreach (Chara chara in EClass._map.charas)
		{
			if (chara.enemy == owner && !chara.CanSeeLos(owner))
			{
				chara.SetEnemy();
			}
		}
	}

	public override void OnRefresh()
	{
		owner.isHidden = true;
	}
}
