using Newtonsoft.Json;

public class ConFear : BadCondition
{
	[JsonProperty]
	public int turnStill;

	public override Emo2 EmoIcon => Emo2.fear;

	public override bool ConsumeTurn => !owner.IsPC;

	public override int GetPhase()
	{
		return 0;
	}

	public override void Tick()
	{
		Mod(-1);
		foreach (Condition condition in owner.conditions)
		{
			if (condition.ConsumeTurn && condition != this)
			{
				return;
			}
		}
		if (owner.IsPC || EClass._zone.IsRegion)
		{
			return;
		}
		if (owner.TryMoveFrom((owner.enemy != null) ? owner.enemy.pos : EClass.pc.pos) == Card.MoveResult.Success)
		{
			turnStill = 0;
			return;
		}
		turnStill++;
		if (EClass.rnd(turnStill) > 2)
		{
			owner.Say("fear_break", owner);
			Kill();
		}
	}
}
