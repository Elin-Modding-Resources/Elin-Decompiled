public class Zone_EternalGarden : Zone_Civilized
{
	public override void OnActivate()
	{
		if (EClass._map.version.IsBelow(0, 23, 226))
		{
			SetBGM(121, refresh: false);
		}
		base.OnActivate();
	}
}
