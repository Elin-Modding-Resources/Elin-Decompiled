using System.Collections.Generic;

public class TraitGiftJure : TraitGiftPack
{
	public override bool OnUse(Chara c)
	{
		if (!EClass.debug.enable)
		{
			Msg.SayNothingHappen();
			return false;
		}
		if (EClass._zone.IsRegion)
		{
			Msg.SayCannotUseHere();
			return false;
		}
		EClass.pc.Say("openDoor", EClass.pc, owner);
		SE.Play("dropReward");
		List<string> list = new List<string> { "snow_globe", "xmas_wreath", "xmas_wreath", "xmas_garland", "xmas_garland", "hat_santa" };
		list.Shuffle();
		for (int i = 0; i < 4; i++)
		{
			Thing t = ThingGen.Create(list[i]);
			EClass.pc.Pick(t);
		}
		owner.ModNum(-1);
		return true;
	}
}
