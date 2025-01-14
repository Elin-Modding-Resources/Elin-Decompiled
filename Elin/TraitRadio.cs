using System.Collections.Generic;

public class TraitRadio : TraitItem
{
	public List<string> ids = new List<string>
	{
		"none", "amb_fire", "amb_bbq", "amb_crowd", "amb_seagull", "amb_horror", "amb_pub", "amb_smelter", "amb_clockwork", "amb_dead",
		"amb_magic", "amb_fountain", "amb_clock", "amb_boat", "amb_waterfall", "amb_rag1", "amb_squeak1"
	};

	public override string IDActorEx => owner.GetStr(52);

	public override bool MaskOnBuild => true;

	public override bool ShowContextOnPick => true;

	public override bool OnUse(Chara c)
	{
		EClass.ui.AddLayer<LayerList>().SetStringList(() => ids, delegate(int a, string n)
		{
			EClass.scene.RemoveActorEx(owner);
			owner.SetStr(52, (a == 0) ? null : n);
			owner.isOn = a != 0;
			if (a != 0)
			{
				EClass.scene.AddActorEx(owner);
			}
			SE.SwitchOn();
		}).SetSize();
		return false;
	}
}
