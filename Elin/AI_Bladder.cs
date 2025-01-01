using System.Collections.Generic;

public class AI_Bladder : AIAct
{
	public TraitBath toilet;

	public override IEnumerable<Status> Run()
	{
		if (toilet == null)
		{
			toilet = EClass._map.Installed.traits.GetTraitSet<TraitBath>().GetRandom() as TraitBath;
		}
		if (toilet == null)
		{
			yield return Cancel();
		}
		yield return DoGoto(toilet.owner);
		Progress_Custom seq = new Progress_Custom
		{
			onProgressBegin = delegate
			{
				owner.SetTempHand(-1, -1);
				owner.SetPCCState(PCCState.Naked);
				owner.SetCensored(enable: true);
				owner.PlaySound("water");
				owner.Kick(owner.pos, ignoreSelf: true);
				owner.pos.TalkWitnesses(owner, "disgust");
			},
			onProgress = delegate
			{
				owner.renderer.PlayAnime(AnimeID.Shiver);
			},
			onProgressComplete = delegate
			{
				if (toilet == null || !toilet.ExistsOnMap || !toilet.owner.pos.Equals(owner.pos))
				{
					owner.hygiene.Mod(-30);
					EClass._map.SetLiquid(owner.pos.x, owner.pos.z, 1, 3);
				}
				else
				{
					owner.hygiene.Mod(10);
				}
				owner.ShowEmo(Emo.happy);
			}
		}.SetDuration(15, 5);
		yield return Do(seq);
	}

	public override void OnReset()
	{
		owner.SetPCCState(PCCState.Normal);
		owner.SetCensored(enable: false);
	}
}
