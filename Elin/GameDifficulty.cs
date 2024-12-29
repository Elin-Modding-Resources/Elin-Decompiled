using Newtonsoft.Json;

public class GameDifficulty : EClass
{
	[JsonProperty]
	public int socre;

	[JsonProperty]
	public int minScore;

	[JsonProperty]
	public int bonusLoot;

	[JsonProperty]
	public bool deathPenalty;

	[JsonProperty]
	public bool economy;

	[JsonProperty]
	public bool manualSave;

	[JsonProperty]
	public bool ironMode;

	[JsonProperty]
	public bool moreFood;

	[JsonProperty]
	public bool moreReward;

	public int GetGrade(int v)
	{
		return 0;
	}

	public int CalculateScore()
	{
		return 0;
	}
}
