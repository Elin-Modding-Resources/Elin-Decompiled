public class TraitDamnPool : TraitItem
{
	public override bool CanBeHeld => false;

	public override bool CanBeDestroyed => false;

	public override bool CanOnlyCarry => true;

	public override bool CanPutAway => false;

	public override bool CanUse(Chara c)
	{
		return EClass.game.quests.GetPhase<QuestIntoDarkness>() == 4;
	}

	public override bool OnUse(Chara c)
	{
		EClass.game.quests.Get<QuestIntoDarkness>().UpdateOnTalk();
		return false;
	}
}
