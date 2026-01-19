using UnityEngine;

public class HotItemNoItem : HotItemGameAction
{
	public override Sprite GetSprite()
	{
		return EClass.core.refs.icons.noHotItem;
	}

	public static bool _TrySetAct(ActPlan p)
	{
		if (p.IsNeighborBlocked || !p.IsSelfOrNeighbor)
		{
			return false;
		}
		Card tg = null;
		p.pos.ListVisibleCards().ForeachReverse(delegate(Card a)
		{
			if (a.isChara && a.Chara.mimicry != null && !a.IsPCParty)
			{
				tg = a;
				return true;
			}
			if (a.TileType.CanBeHeld && a.trait.CanBeHeld && a.isThing && !a.isNPCProperty && a.isThing && a.TileType.CanBeHeld)
			{
				tg = a;
				return true;
			}
			return false;
		});
		if (tg != null)
		{
			return p.TrySetAct("actHold", delegate
			{
				if (tg.ExistsOnMap)
				{
					if (tg.isChara && tg.Chara.mimicry != null && !tg.IsPCParty)
					{
						tg.Chara.mimicry.RevealMimicry(EClass.pc, surprise: true);
						return true;
					}
					if (!EClass.pc.CanLift(tg))
					{
						EClass.pc.Say("tooHeavy", tg);
						return false;
					}
					if (tg.HasEditorTag(EditorTag.TreasureMelilith))
					{
						if (EClass.player.flags.pickedMelilithTreasure)
						{
							EClass.pc.PlaySound("curse3");
							EClass.pc.PlayEffect("curse");
							EClass.pc.SetFeat(1206, 1, msg: true);
							EClass.player.flags.gotMelilithCurse = true;
						}
						else
						{
							Msg.Say("pickedMelilithTreasure");
							EClass.player.flags.pickedMelilithTreasure = true;
							EClass.game.quests.Get<QuestCursedManor>()?.NextPhase();
						}
						tg.c_editorTags = null;
					}
					EClass.pc.HoldCard(tg);
					if (EClass.pc.held != null)
					{
						tg.PlaySoundHold(spatial: false);
						EClass.player.RefreshCurrentHotItem();
						ActionMode.Adv.planRight.Update(ActionMode.Adv.mouseTarget);
						EClass.pc.renderer.Refresh();
						if (EClass.pc.held.trait is TraitToolMusic)
						{
							HotItemHeld.disableTool = true;
						}
					}
				}
				return true;
			}, tg, null, 1, isHostileAct: false, localAct: false);
		}
		TaskHarvest taskHarvest = TaskHarvest.TryGetAct(EClass.pc, p.pos);
		if (taskHarvest != null)
		{
			p.TrySetAct(taskHarvest);
		}
		return false;
	}

	public override bool TrySetAct(ActPlan p)
	{
		return _TrySetAct(p);
	}
}
