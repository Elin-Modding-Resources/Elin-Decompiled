public class HotItemActionBinocular : HotAction
{
	public override string Id => "Binocular";

	public override bool CanName => false;

	public override void Perform()
	{
		if (!(EClass.pc.things.Find<TraitViewMap>()?.trait is TraitViewMap traitViewMap))
		{
			Msg.Say("noBinoFound");
			SE.Beep();
		}
		else
		{
			traitViewMap.OnUse(EClass.pc);
		}
	}
}
