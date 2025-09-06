public class TraitTape : TraitItem
{
	public override void SetName(ref string s)
	{
		s = "_tape".lang(owner.refVal.ToString() ?? "", s);
	}

	public override void OnCreate(int lv)
	{
		if (EClass.core.IsGameStarted && EClass._map.plDay != null && EClass._map.plDay.list.Count > 0)
		{
			owner.refVal = EClass._map.plDay.list[0].data.id;
		}
		else
		{
			owner.refVal = EClass.core.refs.dictBGM.RandomItem().id;
		}
	}

	public override bool OnUse(Chara c)
	{
		if (owner.refVal == 0 || EClass.player.knownBGMs.Contains(owner.refVal))
		{
			Msg.Say("songAlreayKnown");
		}
		else
		{
			Msg.Say("songAdded", EClass.core.refs.dictBGM[owner.refVal]._name, owner.refVal.ToString() ?? "");
			EClass.player.knownBGMs.Add(owner.refVal);
		}
		EClass.Sound.Play("tape");
		owner.ModNum(-1);
		return false;
	}
}
