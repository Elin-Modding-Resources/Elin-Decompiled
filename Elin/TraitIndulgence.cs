public class TraitIndulgence : TraitScroll
{
	public override void OnRead(Chara c)
	{
		c.PlaySound("holyveil");
		c.PlayEffect("holyveil");
		Msg.Say("indulgence", c);
		if (c.IsPC)
		{
			EClass.player.ModKarma(20);
		}
		owner.ModNum(-1);
	}
}
