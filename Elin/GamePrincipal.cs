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
	public int dropRateMtp;

	[JsonProperty]
	public bool ignoreEvaluate;

	[JsonProperty]
	public bool disableDeathPenaltyProtection;

	[JsonProperty]
	public bool tax;

	[JsonProperty]
	public bool opMilk;

	[JsonProperty]
	public bool disableManualSave;

	[JsonProperty]
	public bool permadeath;

	[JsonProperty]
	public bool infiniteMarketFund;

	[JsonProperty]
	public bool disableUsermapBenefit;

	[JsonProperty]
	public bool dropRate;

	public bool IsCustom => idTemplate == -1;

	public int GetGrade(int score)
	{
		return Mathf.Clamp(score / 20, 0, 5);
	}

	public string GetTitle()
	{
		int grade = GetGrade(GetScore());
		return Lang.GetList("pp_titles")[grade];
	}

	public int GetScore()
	{
		if (ignoreEvaluate)
		{
			return 0;
		}
		int num = 0;
		if (tax)
		{
			num += GetScore("tax");
		}
		if (disableManualSave)
		{
			num += GetScore("disableManualSave");
		}
		if (disableDeathPenaltyProtection)
		{
			num += GetScore("disableDeathPenaltyProtection");
		}
		if (disableUsermapBenefit)
		{
			num += GetScore("disableUsermapBenefit");
		}
		if (permadeath)
		{
			num += GetScore("permadeath");
		}
		if (infiniteMarketFund)
		{
			num += GetScore("infiniteMarketFund");
		}
		if (opMilk)
		{
			num += GetScore("opMilk");
		}
		if (dropRate)
		{
			num += GetScore("dropRate");
		}
		if (num >= 0)
		{
			return num;
		}
		return 0;
	}

	public int GetScore(string s)
	{
		if (ignoreEvaluate)
		{
			return 0;
		}
		return s switch
		{
			"tax" => 20, 
			"disableManualSave" => 20, 
			"disableDeathPenaltyProtection" => 10, 
			"disableUsermapBenefit" => 20, 
			"permadeath" => 50, 
			"infiniteMarketFund" => -40, 
			"opMilk" => -40, 
			"dropRate" => 20 + dropRateMtp * -10, 
			_ => 0, 
		};
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
