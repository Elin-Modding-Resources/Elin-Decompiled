public class QuestIntroInspector : QuestProgression
{
	public override void OnComplete()
	{
		Chara chara = AddResident("loytel");
		chara.SetInt(100, 1);
		EClass.game.quests.globalList.Add(Quest.Create("shippingChest").SetClient(chara, assignQuest: false));
		EClass.game.quests.globalList.Add(Quest.Create("exploration").SetClient(EClass.game.cards.globalCharas.Find("ashland"), assignQuest: false));
	}
}
