public class TraitFertilizer : Trait
{
	public override bool CanExtendBuild => true;

	public bool Defertilize => this is TraitDefertilizer;

	public override void OnSimulateHour(VirtualDate date)
	{
		if (!owner.IsInstalled)
		{
			return;
		}
		PlantData plantData = EClass._map.TryGetPlant(owner.Cell);
		bool flag = false;
		if (Defertilize)
		{
			if (plantData == null)
			{
				plantData = EClass._map.AddPlant(owner.pos, null);
			}
			plantData.fert = -1;
		}
		else
		{
			foreach (Card item in owner.pos.ListCards())
			{
				if (item.trait is TraitSeed && !item.isSale)
				{
					flag = true;
					(item.trait as TraitSeed).TrySprout(force: true, sucker: true, date);
					break;
				}
			}
			if (!owner.pos.HasObj)
			{
				if (flag)
				{
					return;
				}
				EClass._map.SetObj(owner.pos.x, owner.pos.z, 5);
			}
			else if (plantData == null)
			{
				if (owner.pos.growth == null)
				{
					return;
				}
				owner.pos.growth.TryGrow(date);
			}
			else if (plantData.fert == 0 && owner.pos.growth != null)
			{
				owner.pos.growth.TryGrow(date);
			}
			plantData = EClass._map.TryGetPlant(owner.Cell);
			if (plantData != null)
			{
				plantData.fert++;
			}
		}
		if (date.IsRealTime)
		{
			owner.PlaySound("mutation");
			owner.PlayEffect("mutation");
		}
		owner.Destroy();
	}
}
