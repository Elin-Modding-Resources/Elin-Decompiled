public class TraitGiftNewYear : TraitGiftPack
{
	public override bool OnUse(Chara c)
	{
		if (EClass._zone.IsRegion)
		{
			Msg.SayCannotUseHere();
			return false;
		}
		EClass.pc.Say("openDoor", EClass.pc, owner);
		SE.Play("dropReward");
		EClass.pc.Pick(ThingGen.Create("kagamimochi"));
		EClass.pc.Pick(ThingGen.Create("1134"));
		EClass.pc.Pick(ThingGen.CreateRecipe("1310"));
		Chara chara = CharaGen.Create("putty_snow");
		EClass._zone.AddCard(chara, (owner.ExistsOnMap ? owner.pos : EClass.pc.pos).GetNearestPoint(allowBlock: false, allowChara: false));
		Msg.Say("package_chara", chara, owner);
		chara.MakeAlly();
		for (int i = 0; i < 3; i++)
		{
			chara = CharaGen.Create("bell_silver");
			Chara chara2 = chara;
			Hostility c_originalHostility = (chara.hostility = Hostility.Neutral);
			chara2.c_originalHostility = c_originalHostility;
			EClass._zone.AddCard(chara, EClass._map.GetRandomSurface());
		}
		owner.ModNum(-1);
		return true;
	}
}
