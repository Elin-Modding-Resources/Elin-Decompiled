public class BaseSong : BaseStance
{
	public override void OnStart()
	{
		owner.ShowEmo(Emo.happy);
		owner.mana.Validate();
	}

	public override void Tick()
	{
		if (!EClass._zone.IsRegion)
		{
			if (owner.HasCondition<ConSilence>())
			{
				Kill();
			}
			else
			{
				TickSong();
			}
		}
	}

	public virtual void TickSong()
	{
	}
}
