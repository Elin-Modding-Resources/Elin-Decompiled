using System;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class GamePrincipal : EClass
{
	[JsonProperty]
	public int idTemplate;

	[JsonProperty]
	public int socre;

	[JsonProperty]
	public int bonusLoot;

	[JsonProperty]
	public bool deathPenaltyProtection;

	[JsonProperty]
	public bool opMilk;

	[JsonProperty]
	public bool manualSave;

	[JsonProperty]
	public bool permadeath;

	[JsonProperty]
	public bool infiniteMarketFund;

	[JsonProperty]
	public bool moreFood;

	[JsonProperty]
	public bool moreReward;

	public bool IsCustom => idTemplate == -1;

	public int GetGrade(int score)
	{
		return Mathf.Clamp(score / 100, 0, 5);
	}

	public string GetTitle()
	{
		int grade = GetGrade(GetScore());
		return Lang.GetList("pp_titles")[grade];
	}

	public int GetScore()
	{
		int num = 300;
		if (permadeath)
		{
			num += 200;
		}
		if (infiniteMarketFund)
		{
			num -= 200;
		}
		if (opMilk)
		{
			num -= 200;
		}
		if (manualSave)
		{
			num -= 100;
		}
		if (deathPenaltyProtection)
		{
			num -= 50;
		}
		return num;
	}

	public int GetValidScore()
	{
		int score = GetScore();
		if (EClass.player.validScore != -1)
		{
			if (score >= EClass.player.validScore)
			{
				return EClass.player.validScore;
			}
			return score;
		}
		return score;
	}

	public void Apply()
	{
		EClass.player.validScore = GetScore();
	}
}
