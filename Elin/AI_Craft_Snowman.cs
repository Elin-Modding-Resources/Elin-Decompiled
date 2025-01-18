using System.Collections.Generic;

public class AI_Craft_Snowman : AI_Craft
{
	public override IEnumerable<Status> Run()
	{
		yield return DoGoto(pos, 1);
		Progress_Custom seq = new Progress_Custom
		{
			onProgress = delegate
			{
				owner.LookAt(pos);
				Msg.SetColor(Msg.colors.Ono);
				Msg.Say(Lang.GetList("walk_snow").RandomItem());
				pos.PlaySound(MATERIAL.sourceSnow.soundFoot);
			},
			onProgressComplete = delegate
			{
				Thing thing = ThingGen.Create("snowman");
				EClass._zone.AddCard(thing, pos).Install();
				owner.Say("crafted", thing);
				owner.PlaySound(MATERIAL.sourceSnow.GetSoundDead());
				owner.PlayAnime(AnimeID.Jump);
				pos.TalkWitnesses(EClass.pc, (EClass.rnd(2) == 0) ? "nice_statue" : "ding_other", 5);
				EClass.pc.ModExp(258, 50);
				if (EClass.rnd(3) != 0)
				{
					EClass.pc.stamina.Mod(-1);
				}
			}
		}.SetDuration(25, 5);
		yield return Do(seq);
	}
}
