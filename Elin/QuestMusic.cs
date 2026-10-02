using Newtonsoft.Json;

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
		return difficulty + EClass.rnd(2);
	}

	public override void OnInit()
	{
		if (EClass.rnd(100) < EClass.rnd(EClass.pc.Evalue(241)))
		{
			partyLv = 1 + EClass.rnd(EClass.pc.Evalue(241) / 10);
		}
		destScore = difficulty * 150 * (100 + partyLv * 50) / 100;
		destScore += EClass.rnd(destScore / 5);
	}
}
