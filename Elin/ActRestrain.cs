public class ActRestrain : Act
{
	public TraitShackle shackle;

	public override TargetType TargetType => TargetType.Chara;

	public override int MaxRadius => 2;

	public override int PerformDistance => 2;

	public override bool IsHostileAct => true;

	public override bool CanPerform()
	{
		Act.TC = Act.TP.FirstChara;
		if (Act.TC == null || Act.TC.isRestrained || !Act.TC.IsAliveInCurrentZone || Act.TC.IsMultisize || Act.TC.Chara.host != null)
		{
			return false;
		}
		if (!Act.TC.IsPCFaction || !EClass._zone.IsPCFaction)
		{
			return false;
		}
		return true;
	}

	public override bool Perform()
	{
		Act.TC = Act.TP.FirstChara;
		if (Act.TC.Chara.IsHostile() || Act.TC.IsMultisize || Act.TC.Chara.host != null)
		{
			Msg.Say("resist", Act.TC);
			return true;
		}
		SE.Change();
		if (!shackle.owner.IsInstalled)
		{
			EClass._zone.AddCard(shackle.owner, Act.TP);
			shackle.owner.SetPlaceState(PlaceState.installed);
		}
		shackle.Restrain(Act.TC, msg: true);
		if (!Act.TC.IsPCFaction)
		{
			EClass.player.ModKarma(-1);
		}
		if (Act.TC.IsPCFaction && EClass._zone.IsPCFaction && shackle.AllowTraining)
		{
			Act.TC.Chara?.SetAI(new AI_Torture
			{
				shackle = shackle
			});
		}
		return true;
	}
}
