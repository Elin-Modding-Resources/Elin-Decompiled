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
		if (EClass._zone.IsRegion)
		{
			return;
		}
		if (owner.HasCondition<ConSilence>())
		{
			Kill();
			return;
		}
		Element element = owner.elements.GetElement(IdAbility);
		if (element != null)
		{
			owner.elements.ModExp(element.id, 20f);
		}
		TickSong();
	}

	public virtual void TickSong()
	{
	}
}
