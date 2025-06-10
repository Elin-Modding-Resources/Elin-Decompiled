public class TraitDiary : TraitScroll
{
	public override void OnRead(Chara c)
	{
		if (!c.IsPC)
		{
			c.SayNothingHappans();
			return;
		}
		Msg.Say("diary_" + GetParam(1));
		Chara chara = CharaGen.Create(GetParam(1));
		chara.c_daysWithPC = EClass.player.stats.days + 365 + EClass.rnd(365);
		EClass._zone.AddCard(chara, EClass.pc.pos.GetNearestPoint(allowBlock: false, allowChara: false));
		chara.MakeAlly(msg: false);
		chara.PlaySound("identify");
		chara.PlayEffect("teleport");
		owner.ModNum(-1);
	}
}
