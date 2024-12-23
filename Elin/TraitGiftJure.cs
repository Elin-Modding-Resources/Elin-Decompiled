using System.Collections.Generic;

public class TraitGiftJure : TraitGiftPack
{
	public override bool OnUse(Chara c)
	{
		if (EClass._zone.IsRegion)
		{
			Msg.SayCannotUseHere();
			return false;
		}
		EClass.pc.Say("openDoor", EClass.pc, owner);
		owner.PlaySound("dropRewardXmas");
		owner.PlayEffect("revive");
		owner.PlayEffect("smoke");
		List<string> list = new List<string> { "hat_santa", "musicbox_jure", "1228", "musicbox_cat", "1229", "holyFeather" };
		Add(list[EClass.player.giftJure % list.Count], 1);
		foreach (string item in new List<string> { "xmas_wreath", "xmas_garland", "1232", "xmas_socks", "xmas_boot", "xmas_cane" })
		{
			Add(item, 2 + EClass.rnd(2));
		}
		Add("xmas_jure", 1);
		Add("snow_globe", 1);
		Add("xmas_pedestal", 1);
		Add("cake_festival", 3);
		Add("bushdenoel", 3);
		Add("mancookie", 3);
		EClass.pc.Pick(ThingGen.CreateRecipe("xmas_tree"));
		Thing thing = ThingGen.CreateLetter("letter_juremas");
		thing.ChangeMaterial("grass_forest");
		EClass.pc.Pick(thing);
		EClass.player.giftJure++;
		owner.ModNum(-1);
		return true;
		static void Add(string id, int num)
		{
			Thing thing2 = ThingGen.Create(id).SetNum(num).SetNoSell();
			if (id == "snow_globe")
			{
				thing2.idSkin = EClass.player.giftJure % 3;
			}
			EClass.pc.Pick(thing2);
		}
	}
}
