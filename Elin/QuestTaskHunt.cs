using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class QuestTaskHunt : QuestTask
{
	public enum Type
	{
		Default,
		Race
	}

	public enum Setup
	{
		Random,
		FighterGuild
	}

	[JsonProperty]
	public int numHunted;

	[JsonProperty]
	public int numRequired;

	[JsonProperty]
	public string idRace;

	[JsonProperty]
	public Type type;

	public Setup setup;

	public SourceRace.Row Race => EClass.sources.races.map.TryGetValue(idRace, "yeek");

	public override string RefDrama2
	{
		get
		{
			if (type != Type.Race)
			{
				return "";
			}
			return Race.GetName();
		}
	}

	public override string RefDrama3 => numRequired.ToString() ?? "";

	public override bool IsComplete()
	{
		return numHunted >= numRequired;
	}

	public static List<SourceChara.Row> ListTargets(string idRace)
	{
		List<SourceChara.Row> list = new List<SourceChara.Row>();
		foreach (SourceChara.Row row in EClass.sources.charas.rows)
		{
			if (row.quality == 0 && row.race == idRace && row.chance > 0 && row.hostility != "Friend")
			{
				list.Add(row);
			}
		}
		return list;
	}

	public override void OnInit()
	{
		if (setup == Setup.FighterGuild)
		{
			numRequired = 20;
			idRace = "yeek";
			type = Type.Race;
		}
		else if (type == Type.Race)
		{
			for (int i = 0; i < 100; i++)
			{
				SourceRace.Row row = EClass.sources.races.rows.RandomItem();
				if (ListTargets(row.id).Count != 0)
				{
					idRace = row.id;
				}
			}
			numRequired = 3 + owner.difficulty * 2 + EClass.rnd(5);
		}
		else
		{
			numRequired = 10 + owner.difficulty * 3 + EClass.rnd(5);
		}
	}

	public override void OnKillChara(Chara c)
	{
		if (c.IsPCFaction)
		{
			return;
		}
		if (type == Type.Race)
		{
			if (c.race.id == idRace)
			{
				CountKill();
			}
		}
		else if (c.OriginalHostility <= Hostility.Enemy)
		{
			CountKill();
		}
		void CountKill()
		{
			numHunted++;
			if (numHunted > numRequired)
			{
				numHunted = numRequired;
			}
			else
			{
				owner.bonusMoney += EClass.curve(3 + c.LV, 50, 10) * ((type != Type.Race) ? 1 : 2);
			}
		}
	}

	public override string GetTextProgress()
	{
		if (type == Type.Race)
		{
			return "progressHuntRace".lang(numHunted.ToString() ?? "", numRequired.ToString() ?? "", Race.GetName());
		}
		return "progressHunt".lang(numHunted.ToString() ?? "", numRequired.ToString() ?? "");
	}

	public override void OnGetDetail(ref string detail, bool onJournal)
	{
		if (type != Type.Race || !onJournal)
		{
			return;
		}
		List<SourceChara.Row> list = ListTargets(idRace);
		int num = 0;
		detail = detail + Environment.NewLine + Environment.NewLine + "target_huntRace".lang() + Environment.NewLine;
		foreach (SourceChara.Row item in list)
		{
			detail = detail + item.GetName().ToTitleCase(wholeText: true) + " (" + EClass.sources.races.map[idRace].GetName() + ")";
			num++;
			if (num > 5)
			{
				break;
			}
			detail += Environment.NewLine;
		}
	}
}
