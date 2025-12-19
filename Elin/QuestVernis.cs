public class QuestVernis : QuestProgression
{
	public override string TitlePrefix => "★";

	public override void OnEnterZone()
	{
		if (EClass._zone is Zone_VernisMine zone_VernisMine && phase == 7 && zone_VernisMine.IsBossLv)
		{
			UpdateOnTalk();
		}
	}

	public override void OnChangePhase(int a)
	{
		switch (phase)
		{
		case 1:
			EClass.game.cards.globalCharas.Find("loytel").MoveHome("vernis");
			break;
		case 5:
		{
			Chara chara = EClass.game.cards.globalCharas.Find("quru");
			if (chara != null)
			{
				chara.MoveHome("vernis");
				EClass.Branch.AddMemeber(chara);
				EClass.game.cards.globalCharas.Find("kettle").MoveHome("vernis");
				EClass.Branch.AddMemeber(EClass.game.cards.globalCharas.Find("kettle"));
				EClass.game.cards.globalCharas.Find("farris").MoveHome("vernis");
				EClass.Branch.AddMemeber(EClass.game.cards.globalCharas.Find("farris"));
			}
			EClass.Branch.AddMemeber(EClass.game.cards.globalCharas.Find("loytel"));
			break;
		}
		case 7:
			DropReward(ThingGen.CreatePotion(8506).SetNum(3));
			DropReward(ThingGen.Create("blanket_fire"));
			break;
		}
	}

	public override bool CanUpdateOnTalk(Chara c)
	{
		if (phase != 0 && EClass._zone.id != "vernis")
		{
			return false;
		}
		switch (phase)
		{
		case 0:
			return true;
		case 1:
			return true;
		case 2:
			return true;
		case 3:
		{
			bool valid = true;
			EClass._map.bounds.ForeachCell(delegate(Cell c)
			{
				if (c.sourceObj.id == 100)
				{
					valid = false;
				}
			});
			return valid;
		}
		case 4:
			return EClass._zone.IsPCFaction;
		case 5:
			if (EClass.game.quests.IsCompleted("quru_past2") && EClass._zone.IsPCFaction)
			{
				return EClass.Branch.lv >= 2;
			}
			return false;
		case 6:
			return true;
		case 9:
			return EClass._zone.IsPCFaction;
		case 10:
			return true;
		default:
			return false;
		}
	}

	public override void OnComplete()
	{
		AddResident("corgon").SetInt(100, 1);
		EClass.game.quests.Add("mokyu", "corgon").startDate = EClass.world.date.GetRaw() + 14400;
		EClass.game.quests.Add("pre_debt", "farris").startDate = EClass.world.date.GetRaw() + 28800;
	}

	public override string GetTextProgress()
	{
		if (phase == 3 && EClass._zone is Zone_Vernis && EClass._zone.lv == 0)
		{
			int i = 0;
			EClass._map.bounds.ForeachCell(delegate(Cell c)
			{
				if (c.sourceObj.id == 100)
				{
					i++;
				}
			});
			return "progressVernis".lang(i.ToString() ?? "");
		}
		return base.GetTextProgress();
	}
}
