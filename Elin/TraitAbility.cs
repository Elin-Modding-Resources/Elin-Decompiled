using UnityEngine;

public class TraitAbility : TraitCatalyst
{
	public override bool CanBeHeldAsFurniture => false;

	public override bool CanBeDestroyed => false;

	public override bool IsRod => false;

	public override bool CanAutofire => base.act.CanAutofire;

	public override Act CreateAct()
	{
		return ACT.Create(EClass.sources.elements.alias.TryGetValue(owner.c_idAbility ?? "AI_SelfHarm"));
	}

	public override void SetName(ref string s)
	{
		if (!owner.c_idAbility.IsEmpty())
		{
			s = EClass.sources.elements.alias.TryGetValue(owner.c_idAbility)?.GetName() ?? "???";
		}
	}

	public override void SetMainText(UIText t, bool hotitem)
	{
		Element element = EClass.pc.elements.GetElement(owner.c_idAbility);
		if (element == null)
		{
			Debug.Log("[bug] " + owner.c_idAbility);
			t.SetActive(enable: false);
		}
		else
		{
			t.SetText(element.vPotential.ToShortNumber(), FontColor.Charge);
			t.SetActive(element is Spell);
		}
	}
}
