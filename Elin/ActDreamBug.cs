public class ActDreamBug : Ability
{
	public override bool CanPerform()
	{
		if (Act.CC.HasElement(1216))
		{
			return base.CanPerform();
		}
		return false;
	}

	public override bool Perform()
	{
		if (!Act.TC.isChara || (!Act.TC.IsPC && Act.TC.things.IsFull()))
		{
			Msg.SayNothingHappen();
			return true;
		}
		Act.TC.Chara.Pick(ThingGen.Create("dreambug"), msg: false);
		Act.CC.Say("dreambug", Act.CC, Act.TC);
		Act.CC.PlaySound("pick_thing");
		return true;
	}
}
