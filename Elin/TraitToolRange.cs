public class TraitToolRange : TraitTool
{
	public override bool CanAutofire => true;

	public virtual bool NeedAmmo => true;

	public virtual bool AutoRefillAmmo => false;

	public int MaxAmmo => (GetParam(1) ?? "12").ToInt() * (100 + owner.Evalue(600) * 5) / 100;

	public int ReloadTurn => (GetParam(2) ?? "4").ToInt() * 100 / (100 + owner.Evalue(601) * 10);

	public virtual Element WeaponSkill => null;

	public virtual bool NeedReload => false;

	public int BestDist => owner.Thing?.source.range ?? 3;

	public virtual bool IsAmmo(Thing t)
	{
		return false;
	}

	public override void OnCreate(int lv)
	{
		owner.c_ammo = MaxAmmo;
	}

	public override void SetMainText(UIText t, bool hotitem)
	{
		if (NeedAmmo)
		{
			string text = owner.c_ammo + "/" + MaxAmmo;
			t.SetText(text ?? "", FontColor.Charge);
			t.SetActive(enable: true);
		}
		else
		{
			base.SetMainText(t, hotitem);
		}
	}
}
