﻿using System;

public class TraitDeed : TraitScroll
{
	public override bool CanStack
	{
		get
		{
			return false;
		}
	}

	public override bool CanBeDestroyed
	{
		get
		{
			return false;
		}
	}

	public override void OnRead(Chara c)
	{
		if (!EClass.debug.enable && (EClass._zone.mainFaction == EClass.pc.faction || !EClass._zone.isClaimable || EClass._zone.instance != null))
		{
			Msg.Say("invalidClaimZone");
			return;
		}
		Dialog.YesNo("dialog_claimLand", delegate
		{
			EClass._zone.ClaimZone(false);
			this.owner.ModNum(-1, true);
			WidgetMenuPanel.OnChangeMode();
			if (EClass._zone == EClass.game.StartZone)
			{
				if (EClass.game.quests.Get<QuestHome>() != null)
				{
					EClass.game.quests.Home.ChangePhase(1);
				}
				if (QuestMain.Phase < 200)
				{
					EClass.game.quests.Main.ChangePhase(200);
				}
			}
			EClass.player.EndTurn(true);
		}, null, "yes", "no");
	}
}