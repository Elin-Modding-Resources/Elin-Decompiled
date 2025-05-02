public class TraitWindow : Trait
{
	public override bool CanBeOnlyBuiltInHome => true;

	public override bool UseAltTiles => EClass.world.date.IsNight;

	public override bool AlwaysHideOnLowWall => true;

	public override bool IsOpenSight => true;

	public override bool ShouldRefreshTile => true;

	public override bool UseExtra
	{
		get
		{
			bool num;
			if (EClass._map.config.hour != -1)
			{
				if (EClass._map.config.hour >= 18)
				{
					goto IL_005c;
				}
				num = EClass._map.config.hour <= 6;
			}
			else
			{
				num = EClass.world.date.IsNight;
			}
			if (!num && !EClass._map.IsIndoor)
			{
				return false;
			}
			goto IL_005c;
			IL_005c:
			return !owner.Cell.isCurtainClosed;
		}
	}
}
