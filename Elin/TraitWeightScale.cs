public class TraitWeightScale : Trait
{
	public override void OnStepped(Chara c)
	{
		c.PlaySound("trap");
		c.Say("trap", c, owner);
		if (this is TraitHeightMeasure)
		{
			owner.TalkRaw("weightScale".langGame(c.Name, c.bio.weight.ToFormat() ?? ""));
		}
		else
		{
			owner.TalkRaw("heightMeasure".langGame(c.Name, c.bio.height.ToFormat() ?? ""));
		}
	}
}
