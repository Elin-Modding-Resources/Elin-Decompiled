public class TraitToolBrushStrip : TraitTool
{
	public override bool IsTool => true;

	public override void TrySetHeldAct(ActPlan p)
	{
		p.pos.Things.ForEach(delegate(Thing t)
		{
			if (!t.isMasked && !t.isHidden && t.isDyed)
			{
				p.TrySetAct("actHammerFurniture".lang(t.Name), delegate
				{
					Msg.Say("upgrade", t, owner.GetName(NameStyle.Full, 1));
					SE.Play("build_area");
					t.PlayEffect("buff");
					t.Dye((SourceMaterial.Row)null);
					return false;
				});
			}
		});
		if (p.pos.HasObj && p.pos.cell.isObjDyed)
		{
			p.TrySetAct("actHammerFurniture".lang(p.pos.cell.GetObjName()), delegate
			{
				Msg.Say("upgrade", p.pos.cell.GetObjName(), owner.GetName(NameStyle.Full, 1));
				SE.Play("build_area");
				p.pos.PlayEffect("buff");
				p.pos.cell.isObjDyed = false;
				p.pos.cell.objMat = (byte)p.pos.sourceObj.DefaultMaterial.id;
				return false;
			});
		}
	}
}
