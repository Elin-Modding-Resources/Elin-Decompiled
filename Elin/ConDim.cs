public class ConDim : BadCondition
{
	public override bool ConsumeTurn => GetPhase() >= 2;
}
