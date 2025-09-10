public class TraitBasketGame : TraitFloorSwitch
{
	public override bool IgnoreOnSteppedWhenMoving => true;

	public override void OnActivateTrap(Chara c)
	{
		if (c.IsPC)
		{
			MiniGame.Activate(MiniGame.Type.Basket);
		}
	}
}
