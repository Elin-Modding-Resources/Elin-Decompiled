public class QuestDialog : QuestProgression
{
	public override bool RequireClientInSameZone => false;

	public override void ShowCompleteText()
	{
		if (id == "pre_debt_runaway")
		{
			SE.WriteJournal();
			Msg.Say("completeQuest", GetTitle());
		}
		else
		{
			base.ShowCompleteText();
		}
	}

	public override bool CanStartQuest()
	{
		if (source.id == "farris_tulip")
		{
			return EClass.pc.faction.HasMember("farris");
		}
		return true;
	}

	public override void OnDropReward()
	{
		switch (source.id)
		{
		case "pre_debt":
		{
			Chara chara = EClass.game.cards.globalCharas.Find("loytel");
			if (chara.currentZone == null)
			{
				EClass.pc.homeBranch.AddMemeber(chara);
			}
			chara.MoveHome("olvina", 59, 66);
			chara.noMove = true;
			chara.AddEditorTag(EditorTag.Invulnerable);
			EClass.player.flags.loytelEscaped = true;
			EClass.game.quests.Start("pre_debt_runaway", "farris");
			break;
		}
		case "pre_debt_runaway":
			EClass.game.quests.Add("exile_meet", "quru").startDate = EClass.world.date.GetRaw() + 43200;
			break;
		case "exile_meet":
			EClass.Branch.AddMemeber(EClass._zone.AddCard(CharaGen.Create("demitas"), EClass.pc.pos.GetNearestPoint(allowBlock: false, allowChara: false)) as Chara);
			EClass.game.quests.Add("exile_quru", "quru").startDate = EClass.world.date.GetRaw() + 1440;
			break;
		case "exile_quru":
			EClass.game.quests.Add("exile_kettle", "kettle").startDate = EClass.world.date.GetRaw() + 1440;
			if (!EClass.game.quests.IsCompleted("into_darkness"))
			{
				EClass.game.quests.Add("into_darkness", "kettle").startDate = EClass.world.date.GetRaw() + 7200;
			}
			break;
		case "exile_kettle":
			EClass.game.quests.Add("exile_whisper").startDate = EClass.world.date.GetRaw() + 1440;
			break;
		case "exile_whisper":
			EClass.game.quests.Add("exile_voice").startDate = EClass.world.date.GetRaw() + 1440;
			break;
		case "fiama_reward":
			DropReward(ThingGen.CreateRecipe("workbench2"));
			DropReward(ThingGen.CreateRecipe("factory_stone"));
			DropReward(ThingGen.CreateRecipe("stonecutter"));
			EClass.game.quests.globalList.Add(Quest.Create("fiama_lock").SetClient(EClass.game.cards.globalCharas.Find("fiama"), assignQuest: false));
			break;
		case "greatDebt":
			EClass.game.quests.Add("farris_tulip", "loytel").startDate = EClass.world.date.GetRaw() + 1440;
			break;
		case "farris_tulip":
			EClass.game.quests.Add("kettle_join", "loytel").startDate = EClass.world.date.GetRaw() + 1440;
			break;
		case "kettle_join":
			EClass.Branch.AddMemeber(EClass._zone.AddCard(CharaGen.Create("kettle"), EClass.pc.pos.GetNearestPoint(allowBlock: false, allowChara: false)) as Chara);
			EClass.Branch.AddMemeber(EClass._zone.AddCard(CharaGen.Create("quru"), EClass.pc.pos.GetNearestPoint(allowBlock: false, allowChara: false)) as Chara);
			EClass.game.quests.Add("quru_morning", "loytel").startDate = EClass.world.date.GetRaw() + 1440;
			break;
		case "quru_morning":
			EClass.game.quests.Add("quru_sing", "quru").startDate = EClass.world.date.GetRaw() + 2880;
			EClass.game.quests.Add("vernis_gold", "loytel").startDate = EClass.world.date.GetRaw() + 1440;
			break;
		case "quru_sing":
			EClass.game.quests.Add("quru_past1", "kettle").startDate = EClass.world.date.GetRaw() + 1440;
			break;
		case "quru_past1":
			EClass.game.quests.Add("quru_past2", "farris").startDate = EClass.world.date.GetRaw() + 1440;
			break;
		}
	}
}
