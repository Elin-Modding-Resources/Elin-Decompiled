using UnityEngine;

public class QuestGuild : QuestSequence
{
	public const int Started = 0;

	public const int CompletedTrial = 1;

	public const int Joined = 10;

	public virtual Guild guild => null;

	public override string TitlePrefix => "☆";

	public override bool UpdateOnTalk()
	{
		return false;
	}

	public override string GetTrackerText()
	{
		if (guild.relation.type == FactionRelation.RelationType.Member)
		{
			FactionRelation relation = guild.relation;
			return "faction_contribution".lang(Mathf.Min(relation.exp, relation.ExpToNext).ToString() ?? "", relation.ExpToNext.ToString() ?? "");
		}
		return base.GetTrackerText();
	}

	public override string GetDetailText(bool onJournal = false)
	{
		string text = base.GetDetailText(onJournal);
		if (guild.relation.type == FactionRelation.RelationType.Member)
		{
			FactionRelation relation = guild.relation;
			int num = relation.rank / 2;
			text += "\n\n";
			text = text + guild.Name.TagColor(FontColor.Topic) + "\n";
			text = text + "faction_rank".lang(relation.rank.ToString() ?? "", relation.TextTitle) + "\n";
			text = text + "faction_contribution".lang(Mathf.Min(relation.exp, relation.ExpToNext).ToString() ?? "", relation.ExpToNext.ToString() ?? "") + "\n";
			text = text + "faction_salary".lang(relation.GetSalary().ToString() ?? "") + "\n";
			text += "\n";
			text = text + "faction_task".lang().TagColor(FontColor.Topic) + "\n";
			text = text + (guild.id + "_task1").lang() + "\n";
			text += "\n";
			text = text + "faction_benefit".lang().TagColor(FontColor.Topic) + "\n";
			text = text + (guild.id + "_benefit1").lang() + "\n";
			if (num >= 1)
			{
				text = text + "guild_benefit_trainer".lang() + "\n";
			}
			if (num >= 2)
			{
				text = text + (guild.id + "_benefit2").lang() + "\n";
			}
			if (num >= 3)
			{
				text = text + (guild.id + "_benefit3").lang() + "\n";
			}
			if (num >= 4)
			{
				text = text + "guild_benefit_feat".lang() + "\n";
			}
		}
		return text;
	}
}
