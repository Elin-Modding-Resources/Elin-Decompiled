using UnityEngine;

public class ZonePreEnterOnCompleteQuestInstance : ZonePreEnterEvent
{
	public int uidClient;

	public int uidQuest;

	public bool fail;

	public override void Execute()
	{
		EClass.player.returnInfo = null;
		if (uidQuest == 0)
		{
			return;
		}
		Quest quest = EClass.game.quests.Get(uidQuest);
		Chara chara = EClass._map.FindChara(uidClient);
		if (chara == null)
		{
			chara = EClass._map.deadCharas.Find((Chara c) => c.uid == uidClient);
			if (chara == null)
			{
				Debug.Log(uidClient + "/" + uidQuest + "/" + fail + "/" + quest);
				Debug.LogError("exception: quest not found:" + chara?.ToString() + "/" + chara?.quest);
				quest.Fail();
				return;
			}
			chara.Revive();
		}
		if (chara.quest == null)
		{
			if (quest != null)
			{
				chara.quest = quest;
			}
			Debug.Log(uidClient + "/" + uidQuest + "/" + fail + "/" + quest);
			Debug.LogWarning("exception: assigned quest to:" + chara);
		}
		if (quest == null && chara.quest.uid == uidQuest)
		{
			quest = chara.quest;
		}
		if (quest != null)
		{
			if (fail)
			{
				quest.Fail();
			}
			else
			{
				quest.Complete();
			}
			if (EClass.pc.IsAliveInCurrentZone)
			{
				chara.ShowDialog("_chara", fail ? "quest_fail" : "quest_success");
			}
		}
	}
}
