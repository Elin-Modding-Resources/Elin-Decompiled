public class LayerSleep : ELayer
{
	public float repeatRate;

	public float coverAlpha;

	public float coverShow;

	public float coverHide;

	public float hideDelay;

	private int hours;

	private int min;

	private int maxMin;

	private Thing bed;

	private Thing pillow;

	public override void OnAfterInit()
	{
		ELayer.ui.ShowCover(coverShow, coverAlpha);
	}

	public void Wait(float duration = 5f)
	{
		ELayer.Sound.Play("jingle_sleep");
		TweenUtil.Tween(duration, null, delegate
		{
			ELayer.ui.HideCover(coverHide, delegate
			{
				Close();
			});
		});
	}

	public void Sleep(int _hours, Thing _bed, Thing _pillow = null)
	{
		ELayer.Sound.Play("jingle_sleep");
		hours = _hours;
		bed = _bed;
		pillow = _pillow;
		maxMin = hours * 60;
		InvokeRepeating("Advance", repeatRate, repeatRate);
		ELayer.pc.faith.Revelation("sleep");
		ELayer.player.stats.slept++;
	}

	public void Advance()
	{
		if (min > maxMin)
		{
			CancelInvoke();
			if (!ELayer._zone.IsInstance && (ELayer._zone.IsPCFaction || ELayer._zone is Zone_Field || ELayer._zone.IsRegion))
			{
				ELayer.player.SimulateFaction();
			}
			foreach (Chara chara in ELayer._map.charas)
			{
				if (chara.affinity.GetLunchChance() > ELayer.rnd(100) && chara.GetInt(71) == -1)
				{
					chara.SetInt(71, -2);
					if (!chara.elements.Has(287))
					{
						chara.elements.Learn(287);
					}
					for (int i = 0; i < 5; i++)
					{
						chara.elements.ModExp(287, 2000f);
					}
				}
			}
			foreach (Chara member in ELayer.pc.party.members)
			{
				member.OnSleep(bed);
			}
			Msg.Say("slept", hours.ToString() ?? "");
			if (pillow != null && pillow.trait is TraitPillowOpatos)
			{
				ELayer.world.region.RenewRandomSites();
			}
			if (!ELayer.debug.ignoreAutoSave)
			{
				ELayer.game.Save();
			}
			if (ELayer._zone.IsRegion)
			{
				ELayer._zone.Region.UpdateRandomSites();
			}
			ELayer.ui.ShowCover();
			TweenUtil.Delay(hideDelay, delegate
			{
				ELayer.ui.HideCover(coverHide, delegate
				{
					Close();
				});
			});
		}
		else
		{
			min += 10;
			ELayer.world.date.AdvanceMin(10);
		}
	}
}
