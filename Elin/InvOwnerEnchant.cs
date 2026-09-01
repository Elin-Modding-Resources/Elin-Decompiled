public class InvOwnerEnchant : InvOwnerEffect
{
	public bool armor;

	public override bool CanTargetAlly => true;

	public override string langTransfer => "invEnchant";

	public override Thing CreateDefaultContainer()
	{
		return ThingGen.CreateScroll((!armor) ? (superior ? 8251 : 8250) : (superior ? 8256 : 8255));
	}

	public override bool ShouldShowGuide(Thing t)
	{
		if (t.category.IsChildOf(armor ? "armor" : "weapon"))
		{
			return t.category.id != "relic";
		}
		return false;
	}

	public override void _OnProcess(Thing t)
	{
		ActEffect.Proc((!armor) ? (superior ? EffectId.EnchantWeaponGreat : EffectId.EnchantWeapon) : (superior ? EffectId.EnchantArmorGreat : EffectId.EnchantArmor), 100, state, t.GetRootCard(), t);
	}
}
