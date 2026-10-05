using Newtonsoft.Json;
using UnityEngine;

public class QuestMusic : QuestInstance
{
	[JsonProperty]
	public int score;

	[JsonProperty]
	public int destScore = 10;

	[JsonProperty]
	public int sumMoney;

	[JsonProperty]
	public int partyLv;

	public override DifficultyType difficultyType => DifficultyType.Music;

	public override string IdZone => "instance_music";

	public override string RewardSuffix => "Music";

	public override string RefDrama2 => destScore.ToString() ?? "";

	public override int KarmaOnFail => 0;

	public override bool FameContent => partyLv > 0;

	public override int DangerLv
	{
		get
		{
			if (partyLv <= 0)
			{
				return 1;
			}
			return partyLv * 50;
		}
	}

	public override ZoneEventQuest CreateEvent()
	{
		return new ZoneEventMusic();
	}

	public override ZoneInstanceRandomQuest CreateInstance()
	{
		return new ZoneInstanceMusic();
	}

	public override string GetTextProgress()
	{
		return "progressMusic".lang(score.ToString() ?? "", destScore.ToString() ?? "");
	}

	public override int GetRewardPlat(int money)
	{
		return (difficulty + EClass.rnd(2)) * (100 + Mathf.Min(partyLv, 20) * 50) / 100;
	}

	public override void OnInit()
	{
		if (EClass.rnd(100) < EClass.rnd(EClass.pc.elements.ValueWithoutLink(241)))
		{
			partyLv = Mathf.Min(1 + EClass.rnd(EClass.pc.elements.ValueWithoutLink(241) / 10), 1000000);
		}
		destScore = difficulty * 150 * (100 + Mathf.Min(partyLv, 10) * 10) / 100;
		destScore += EClass.rnd(destScore / 5);
	}
}
