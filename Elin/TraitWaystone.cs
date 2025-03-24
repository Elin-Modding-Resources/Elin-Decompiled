using UnityEngine;

public class TraitWaystone : Trait
{
	public bool IsTemp => owner.id == "waystone_temp";

	public override bool CanUse(Chara c)
	{
		return IsTemp;
	}

	public override bool OnUse(Chara c)
	{
		owner.ModNum(-1);
		EClass.pc.MoveZone(EClass._zone.ParentZone);
		return false;
	}

	public override void TrySetAct(ActPlan p)
	{
		if (EClass._zone is Zone_Tent)
		{
			p.TrySetAct("actNameZone", delegate
			{
				Dialog.InputName("dialogChangeName", EClass._zone.Name, delegate(bool cancel, string text)
				{
					if (!cancel)
					{
						EClass._zone.name = text;
						EClass._zone.idPrefix = 0;
						WidgetDate.Refresh();
					}
				});
				return false;
			}, owner);
			if (Application.isEditor || (!EClass._zone.name.IsEmpty() && !EClass.core.version.demo))
			{
				p.TrySetAct("actUploadMap", delegate
				{
					EClass.ui.AddLayer<LayerUploader>();
					return false;
				}, owner);
			}
		}
		else
		{
			if (!EClass._zone.parent.IsRegion || (EClass._zone.IsInstance && !EClass._zone.IsUserZone) || EClass._zone is Zone_Dungeon)
			{
				return;
			}
			p.TrySetAct("actNewZone", delegate
			{
				if (IsTemp)
				{
					owner.ModNum(-1);
				}
				EClass.pc.MoveZone(EClass._zone.ParentZone);
				return false;
			}, owner, CursorSystem.MoveZone);
			if (!EClass._zone.IsPCFaction && !EClass._zone.IsTown)
			{
				return;
			}
			if (EClass.player.spawnZone != EClass._zone)
			{
				p.TrySetAct("actSetSpawn", delegate
				{
					Effect.Get("aura_heaven").Play(EClass.pc.pos);
					EClass.Sound.Play("worship");
					EClass.player.spawnZone = EClass._zone;
					Msg.Say("setSpawn", owner);
					return true;
				}, owner);
			}
			else if (EClass.player.spawnZone != EClass.pc.homeZone || EClass._zone != EClass.pc.homeZone)
			{
				p.TrySetAct("actUnsetSpawn", delegate
				{
					EClass.Sound.Play("trash");
					EClass.player.spawnZone = EClass.pc.homeZone;
					Msg.Say("unsetSpawn", owner);
					return true;
				}, owner);
			}
		}
	}
}
