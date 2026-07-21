public class BaseSong : BaseStance
{
	public virtual int IdAbility => -1;

	public override void OnStart()
	{
		owner.ShowEmo(Emo.happy);
		owner.mana.Validate();
	}

	public override void Tick()
	{
		if (!EClass._zone.IsRegion && !owner.HasCondition<ConSilence>())
		{
			Element element = owner.elements.GetElement(IdAbility);
			if (element != null)
			{
				owner.elements.ModExp(element.id, 20f);
			}
			if (EClass.rnd(100) == 0)
			{
				owner.elements.ModExp(241, owner.IsPC ? 10 : 100);
			}
			TickSong();
		}
	}

	public virtual void TickSong()
	{
	}
}
