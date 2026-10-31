using Newtonsoft.Json;

public class QuestEscort : QuestDestZone
{
	[JsonProperty]
	public int uidChara;

	public Chara target => EClass._map.FindChara(uidChara);

	public override DifficultyType difficultyType => DifficultyType.Escort;

	public override int KarmaOnFail => -4;

	public override bool ForbidTeleport => true;

	public override void OnStart()
	{
		Chara chara = CharaGen.CreateFromFilter("c_neutral", 10);
		EClass._zone.AddCard(chara, EClass.pc.pos.GetNearestPoint(allowBlock: false, allowChara: false));
		chara.MakeMinion(EClass.pc);
		uidChara = chara.uid;
		chara.Talk("parasite", null, null, forceSync: true);
	}

	public override int GetExtraMoney()
	{
		return base.DestZone.Dist(base.ClientZone) * 6;
	}

	public override string GetTextProgress()
	{
		return "progressEscort".lang((target == null) ? "???" : target.NameSimple, base.DestZone.Name);
	}

	public override void OnEnterZone()
	{
		if (target == null || target.isDead)
		{
			Fail();
		}
		else if (EClass._zone == base.DestZone)
		{
			Complete();
			target.Talk("thanks", null, null, forceSync: true);
			ReleaseEscort();
		}
	}

	public override void OnFail()
	{
		ReleaseEscort();
	}

	public void ReleaseEscort()
	{
		if (target != null)
		{
			target.UnmakeMinion();
			if (EClass._zone.IsRegion)
			{
				target.Destroy();
			}
			else
			{
				target.SetSummon(60);
			}
		}
	}
}
