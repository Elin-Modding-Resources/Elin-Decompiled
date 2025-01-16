public class TraitToolButcher : TraitTool
{
	public override bool DisableAutoCombat => true;

	public override void TrySetHeldAct(ActPlan p)
	{
		foreach (Chara chara in p.pos.Charas)
		{
			if (!chara.IsPCFaction || chara.IsPC)
			{
				continue;
			}
			Chara _c = chara;
			p.TrySetAct("AI_Slaughter", delegate
			{
				Dialog.TryWarnSlaughter(delegate
				{
					EClass.pc.SetAIImmediate(new AI_Slaughter
					{
						target = _c
					});
				});
				return false;
			}, chara);
		}
	}
}
