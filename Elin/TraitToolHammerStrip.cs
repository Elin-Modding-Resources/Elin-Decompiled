public class TraitToolHammerStrip : TraitTool
{
	public override bool IsTool => true;

	public override void TrySetHeldAct(ActPlan p)
	{
		p.pos.Things.ForEach(delegate(Thing t)
		{
			if (t.category.tag.Contains("enc") && t.encLV > 0)
			{
				p.TrySetAct("actHammerFurniture".lang(t.Name), delegate
				{
					Msg.Say("upgrade", t, owner.GetName(NameStyle.Full, 1));
					SE.Play("build_area");
					t.PlayEffect("buff");
					t.SetEncLv(0);
					return false;
				});
			}
		});
	}
}
