using UnityEngine;

public class TraitSkyHelm : TraitItem
{
	public override bool OnUse(Chara c)
	{
		if (!EClass.game.IsSurvival)
		{
			Msg.SayNothingHappen();
			return false;
		}
		EClass.game.survival.RefreshGateZones();
		int ticket = EClass.pc.GetCurrency("ticket_sky");
		EClass.ui.AddLayer<LayerList>().ManualList(delegate(UIList list, LayerList l)
		{
			list.moldItem = Resources.Load<ButtonElement>("UI/Element/Button/ButtonSkyTravel").transform;
			list.callbacks = new UIList.Callback<Zone, ButtonElement>
			{
				onClick = delegate(Zone a, ButtonElement b)
				{
					int costSkyTravel = a.GetCostSkyTravel();
					if (costSkyTravel > 0 && costSkyTravel > ticket)
					{
						if (EClass.debug.enable)
						{
							EClass.pc.ModCurrency(costSkyTravel * 2, "ticket_sky");
						}
						SE.Beep();
					}
					else
					{
						EClass.ui.FlashCover(1f, 0.5f, 1f, null, null, Color.black);
						EClass.pc.ModCurrency(-costSkyTravel, "ticket_sky");
						EClass.game.survival.gateZone = a;
						SE.Play("ship_bell");
						EClass.game.survival.listGateZone.Clear();
						EClass.ui.CloseLayers();
						EClass.pc.pos.TalkWitnesses(EClass.pc, "ahoy", 6, WitnessType.everyone, (Chara chara) => true);
					}
				},
				onRedraw = delegate(Zone a, ButtonElement b, int i)
				{
					int cost = a.GetCostSkyTravel();
					b.mainText.text = a.NameWithDangerLevel;
					b.subText2.text = ((cost == 0) ? "-" : (cost.ToString() ?? "")).TagColor(() => ticket >= cost || cost == 0);
					b.RebuildLayout();
				},
				onInstantiate = delegate
				{
				},
				onList = delegate
				{
					foreach (Zone item in EClass.game.survival.listGateZone)
					{
						list.Add(item);
					}
				}
			};
		}).SetSize()
			.SetTitles("wSkyTravel")
			.windows[0].AttachCurrency().Build(new UICurrency.Options
		{
			ticketSky = true
		});
		return false;
	}
}
