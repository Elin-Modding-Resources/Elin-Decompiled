public class TraitBedCoffin : TraitBed
{
	public override bool UseAltTiles
	{
		get
		{
			if (owner.ExistsOnMap && owner.IsInstalled)
			{
				return !owner.pos.HasChara;
			}
			return true;
		}
	}
}
