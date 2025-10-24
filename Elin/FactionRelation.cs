using Newtonsoft.Json;
using UnityEngine;

public class FactionRelation : EClass
{
	public enum RelationType
	{
		Default,
		Owner,
		Member
	}

	[JsonProperty]
	public int affinity;

	[JsonProperty]
	public int exp;

	[JsonProperty]
	public int rank;

	[JsonProperty]
	public RelationType type;

	public Faction faction;

	public int ExpToNext => 100 + rank * rank * 100;

	public int MaxRank => 6;

	public string TextTitle => Lang.GetList("guild_title")[Mathf.Min(rank / 2, 5)];

	public int GetSalary()
	{
		if (type != RelationType.Member)
		{
			return 0;
		}
		return rank * 100;
	}

	public string GetTextRelation()
	{
		return ("faction_" + type).lang();
	}

	public void SetTextHostility(UIText text)
	{
		if (IsMember() || affinity > 100)
		{
			text.SetText("reFriend".lang(), FontColor.Good);
		}
		else if (affinity < -100)
		{
			text.SetText("reEnemy".lang(), FontColor.Bad);
		}
		else
		{
			text.SetText("reNeutral".lang(), FontColor.Passive);
		}
	}

	public bool IsMember()
	{
		if (type != RelationType.Member)
		{
			return type == RelationType.Owner;
		}
		return true;
	}

	public void Promote()
	{
		exp -= ExpToNext;
		rank++;
		Msg.Say("faction_promote");
		SE.Play("questComplete");
	}
}
