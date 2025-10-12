public class TraitToolRangeSling : TraitToolRange
{
	public override Element WeaponSkill => owner.elements.GetOrCreateElement(108);

	public override bool AutoRefillAmmo => true;

	public override bool IsAmmo(Thing t)
	{
		return t.id == "stone";
	}
}
