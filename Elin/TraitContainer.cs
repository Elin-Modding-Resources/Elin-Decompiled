public class TraitContainer : TraitBaseContainer
{
	public override bool CanStack => false;

	public CoreRef.InventoryStyle InvStyle => EClass.core.refs.invStyle[IDInvStyle];

	public override void TrySetHeldAct(ActPlan p)
	{
		if (p.IsSelf)
		{
			TrySetAct(p);
		}
	}

	public override void TrySetAct(ActPlan p)
	{
		if (owner.c_lockLv > 0)
		{
			p.TrySetAct(new AI_OpenLock
			{
				target = owner.Thing
			}, owner);
		}
		else if (CanOpenContainer && (owner.GetRootCard() != EClass.pc || owner.parentCard == null || owner.parentCard.IsPC || owner.parentCard.trait is TraitToolBelt))
		{
			p.TrySetAct("actContainer", delegate
			{
				TryOpen();
				return false;
			}, owner, CursorSystem.Container, 1, ShowOpenActAsCrime);
		}
	}

	public void TryOpen()
	{
		if (owner.c_lockLv > 0)
		{
			if (owner.GetRootCard() == EClass.pc && EClass.pc.HasNoGoal && EClass.ui.layers.Count == 0)
			{
				EClass.pc.SetAIImmediate(new AI_OpenLock
				{
					target = owner.Thing
				});
				EClass.player.EndTurn();
			}
			else
			{
				SE.Play("lock");
			}
		}
		else
		{
			Open();
		}
	}

	public virtual void Open()
	{
		if (base.HasChara && !EClass._zone.IsRegion)
		{
			Chara c = EClass._zone.SpawnMob((owner.ExistsOnMap ? owner.pos : EClass.pc.pos).GetNearestPoint(allowBlock: false, allowChara: false), new SpawnSetting
			{
				id = owner.c_idRefCard
			});
			owner.c_idRefCard = null;
			owner.things.DestroyAll();
			if (InvStyle.sound != null)
			{
				InvStyle.sound.Play();
			}
			else
			{
				SE.PopInventory();
			}
			Msg.Say("package_chara", c, owner);
		}
		else if (LayerInventory.CreateContainer(owner.Thing) != null)
		{
			if (InvStyle.sound != null)
			{
				InvStyle.sound.Play();
			}
			else
			{
				SE.PopInventory();
			}
		}
	}

	public override void OnSetCardGrid(ButtonGrid b)
	{
		if (LayerInventory.IsOpen(owner.Thing))
		{
			b.Attach("open", rightAttach: false);
		}
	}
}
