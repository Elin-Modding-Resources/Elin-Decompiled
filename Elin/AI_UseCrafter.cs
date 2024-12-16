using System.Collections.Generic;

public class AI_UseCrafter : AIAct
{
	public LayerBaseCraft layer;

	public TraitCrafter crafter;

	public Recipe recipe;

	public int num = 1;

	public List<Thing> ings = new List<Thing>();

	public override int LeftHand => 1001;

	public override int RightHand => 1002;

	public override bool CanManualCancel()
	{
		if ((bool)layer)
		{
			return layer.CanCancelAI;
		}
		return false;
	}

	public override void OnStart()
	{
		if (crafter.Icon != 0)
		{
			owner.ShowEmo(crafter.Icon);
		}
	}

	public override void OnSuccess()
	{
		OnEnd();
	}

	public override void OnCancel()
	{
		OnEnd();
		if ((bool)layer)
		{
			layer.Close();
		}
	}

	public void OnEnd()
	{
		foreach (Thing ing in ings)
		{
			if (ing != null && ing.ExistsOnMap)
			{
				ing.isHidden = false;
				EClass.pc.Pick(ing);
			}
		}
		if (crafter.AutoTurnOff && crafter.owner.isOn)
		{
			crafter.Toggle(on: false);
		}
		if (!crafter.idSoundBG.IsEmpty())
		{
			EClass.Sound.Stop(crafter.idSoundBG);
		}
		if ((bool)layer)
		{
			layer.OnEndCraft();
		}
	}

	public override IEnumerable<Status> Run()
	{
		do
		{
			if (crafter.owner.isDestroyed || !layer)
			{
				yield return Success();
			}
			if (!crafter.idSoundBG.IsEmpty())
			{
				SE.Play(crafter.idSoundBG);
			}
			List<Thing> targets = layer.GetTargets();
			BlessedState blessed = BlessedState.Normal;
			for (int j = 0; j < targets.Count; j++)
			{
				Thing t2 = targets[j];
				if (!IsIngValid(t2, j))
				{
					if (j == 0)
					{
						layer.ClearButtons();
					}
					else
					{
						layer.RefreshCurrentGrid();
					}
					yield return Success();
				}
			}
			if (!crafter.IsFuelEnough(num, targets))
			{
				Msg.Say("notEnoughFuel");
				layer.RefreshCurrentGrid();
				yield return Success();
			}
			ings = new List<Thing>();
			for (int k = 0; k < targets.Count; k++)
			{
				Thing thing = targets[k].Split(layer.GetReqIngredient(k));
				ings.Add(thing);
				if (thing.blessedState <= BlessedState.Cursed && blessed > thing.blessedState)
				{
					blessed = thing.blessedState;
				}
				if (thing.blessedState > BlessedState.Normal && blessed == BlessedState.Normal)
				{
					blessed = thing.blessedState;
				}
				if (crafter.IsConsumeIng)
				{
					Card card = EClass._zone.AddCard(thing, crafter.owner.ExistsOnMap ? crafter.owner.pos : owner.pos);
					card.altitude = ((!crafter.owner.ExistsOnMap) ? 1 : 0);
					if (crafter.animeType == TraitCrafter.AnimeType.Microwave)
					{
						card.isHidden = true;
					}
				}
			}
			if ((bool)LayerDragGrid.Instance)
			{
				LayerDragGrid.Instance.Redraw();
			}
			bool requireOn = crafter.IsRequireFuel || crafter.ToggleType != ToggleType.None;
			if (requireOn && !crafter.owner.isOn)
			{
				crafter.Toggle(on: true);
			}
			int costSP = crafter.GetCostSp(this);
			int duration = crafter.GetDuration(this, costSP);
			Progress_Custom progress = new Progress_Custom
			{
				canProgress = delegate
				{
					if (requireOn && !crafter.owner.isOn)
					{
						return false;
					}
					foreach (Thing ing in ings)
					{
						if (ing.isDestroyed || (crafter.IsConsumeIng && !ing.ExistsOnMap))
						{
							return false;
						}
					}
					if ((bool)LayerDragGrid.Instance)
					{
						InvOwnerDraglet invOwnerDraglet = LayerDragGrid.Instance.owner;
						for (int l = 0; l < invOwnerDraglet.numDragGrid; l++)
						{
							if (invOwnerDraglet.buttons[l].Card == null)
							{
								return false;
							}
						}
						if (invOwnerDraglet.numDragGrid == 2 && ings[0] == ings[1] && ings[0].Num == 1)
						{
							return false;
						}
					}
					return !crafter.owner.isDestroyed;
				},
				onProgress = delegate
				{
					if (crafter.owner.ExistsOnMap && !owner.pos.Equals(crafter.owner.pos))
					{
						owner.LookAt(crafter.owner);
					}
					owner.PlaySound(crafter.idSoundProgress);
					if (crafter.owner.ExistsOnMap)
					{
						TraitCrafter.AnimeType animeType = crafter.animeType;
						if ((uint)(animeType - 1) <= 1u)
						{
							crafter.owner.renderer.PlayAnime(crafter.IdAnimeProgress);
						}
					}
					foreach (Thing ing2 in ings)
					{
						ing2.renderer.PlayAnime(crafter.IdAnimeProgress);
					}
				},
				onProgressComplete = delegate
				{
					if (crafter.StopSoundProgress)
					{
						EClass.Sound.Stop(crafter.idSoundProgress);
					}
					owner.PlaySound(crafter.idSoundComplete);
					Element orCreateElement = owner.elements.GetOrCreateElement(crafter.IDReqEle(recipe?.source ?? null));
					if (recipe != null)
					{
						for (int m = 0; m < this.num; m++)
						{
							recipe.Craft(blessed, m == 0, ings, crafter);
						}
						EClass.Sound.Play("craft");
						Point from = (crafter.owner.ExistsOnMap ? crafter.owner.pos : owner.pos);
						Effect.Get("smoke").Play(from);
						Effect.Get("mine").Play(from).SetParticleColor(recipe.GetColorMaterial().GetColor())
							.Emit(10 + EClass.rnd(10));
						owner.renderer.PlayAnime(AnimeID.JumpSmall);
						recipe.TryGetFirstTimeBonus();
					}
					else
					{
						Thing thing2 = crafter.Craft(this);
						if (thing2 != null)
						{
							if (thing2.category.ignoreBless == 0)
							{
								thing2.SetBlessedState(blessed);
							}
							thing2.PlaySoundDrop(spatial: false);
							EClass._zone.AddCard(thing2, EClass.pc.pos);
							thing2.Identify(show: false);
							owner.Pick(thing2);
						}
					}
					for (int n = 0; n < ings.Count; n++)
					{
						if (crafter.ShouldConsumeIng(crafter.GetSource(this), n))
						{
							ings[n].Destroy();
						}
					}
					foreach (Thing ing3 in ings)
					{
						if (ing3.ExistsOnMap)
						{
							owner.Pick(ing3);
						}
					}
					if (crafter.IsRequireFuel)
					{
						crafter.owner.ModCharge(-crafter.FuelCost * this.num);
						if (crafter.owner.c_charges <= 0)
						{
							crafter.owner.c_charges = 0;
							crafter.Toggle(on: false);
						}
					}
					for (int num = 0; num < this.num; num++)
					{
						owner.RemoveCondition<ConInvulnerable>();
						owner.elements.ModExp(orCreateElement.id, costSP * 12 * (100 + duration * 2) / 100);
						owner.stamina.Mod(-costSP);
						if (owner == null || owner.isDead)
						{
							break;
						}
					}
					Rand.SetSeed();
					if (crafter is TraitCookerMicrowave && recipe.id == "onsentamago" && EClass.rnd(3) != 0)
					{
						int power = EClass.curve((200 + ings[0].Quality * 5) * (100 + EClass.pc.Evalue(287) * 10) / 100, 400, 100);
						ActEffect.ProcAt(EffectId.Explosive, power, BlessedState.Normal, crafter.owner.ExistsOnMap ? crafter.owner : EClass.pc, EClass.pc, EClass.pc.pos, isNeg: true, new ActRef
						{
							aliasEle = "eleImpact"
						});
					}
				}
			}.SetDuration(duration, 5);
			owner.SetTempHand(-1, -1);
			if (EClass.debug.godCraft)
			{
				progress.SetDuration(1, 1);
			}
			yield return Do(progress);
			if (progress.status == Status.Fail)
			{
				yield return Cancel();
			}
			if (crafter.CloseOnComplete)
			{
				yield return Cancel();
			}
			if (!crafter.IsConsumeIng)
			{
				layer.ClearButtons();
				break;
			}
		}
		while ((bool)layer && layer.RepeatAI);
		bool IsIngValid(Thing t, int i)
		{
			if (t == null || t.isDestroyed)
			{
				return false;
			}
			Card rootCard = t.GetRootCard();
			if (rootCard != null && rootCard.isChara && !rootCard.IsPC)
			{
				return false;
			}
			if (!crafter.IsFactory && !crafter.IsCraftIngredient(t, i))
			{
				return false;
			}
			return true;
		}
	}
}
