using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LayerShippingResult : ELayer
{
	public UIList list;

	public UIButton buttonNext;

	public UIButton buttonPrev;

	public Image gaugeGold;

	public Image gaugeHearth;

	public Image gaugeDebt;

	public Image imageBG;

	public UIText textGold;

	public UIText textGold2;

	public UIText textHearth;

	public UIText textHearth2;

	public UIText textHome;

	public UIText textIncome;

	public UIText textIncomeTotal;

	public UIText textTitle;

	public UIText textDebt;

	public bool showEffect = true;

	public float duration;

	public int update;

	public int maxUpdate;

	public ShippingResult result;

	public Sprite[] spriteBG;

	private int lastBonus;

	private new void Awake()
	{
		Rand.SetSeed(ELayer.game.seed + ELayer.player.stats.days);
		int num = ELayer.rnd(spriteBG.Length);
		if (num == 1 && ELayer.game.cards.globalCharas.Find("corgon") == null)
		{
			num = 0;
		}
		if (num == 2 && ELayer.game.cards.globalCharas.Find("loytel") == null)
		{
			num = 0;
		}
		imageBG.sprite = spriteBG[num];
		Rand.SetSeed();
		base.Awake();
	}

	public void Show()
	{
		Show(ELayer.player.shippingResults.LastItem() ?? new ShippingResult());
		SE.Play("money_shipping");
		InvokeRepeating("UpdateRepeating", 0f, duration / (float)maxUpdate);
	}

	public void UpdateRepeating()
	{
		if (showEffect)
		{
			Refresh();
		}
	}

	public void Refresh()
	{
		if (update == 0)
		{
			lastBonus = 0;
		}
		if (!showEffect)
		{
			if (update != maxUpdate)
			{
				DOTweenAnimation[] componentsInChildren = GetComponentsInChildren<DOTweenAnimation>();
				foreach (DOTweenAnimation dOTweenAnimation in componentsInChildren)
				{
					if (dOTweenAnimation.tween != null)
					{
						dOTweenAnimation.tween.Kill(complete: true);
					}
				}
			}
			update = maxUpdate;
		}
		FactionBranch branch = (ELayer.game.spatials.Find(result.uidZone) ?? ELayer.pc.homeZone).branch;
		float num = (float)update / (float)maxUpdate;
		int income = result.GetIncome();
		income = (int)((float)income * num);
		long num2 = result.total + income;
		textIncome.text = Lang._currency(income, showUnit: true);
		textIncomeTotal.text = Lang._currency(num2, showUnit: true);
		int shippingBonus = ELayer.player.stats.GetShippingBonus(result.total);
		int shippingBonus2 = ELayer.player.stats.GetShippingBonus(num2);
		int lastShippingExp = ELayer.player.stats.lastShippingExp;
		int lastShippingExpMax = ELayer.player.stats.lastShippingExpMax;
		int hearthLv = result.hearthLv;
		int num3 = (int)((float)result.hearthExpGained * num);
		int nextExp = branch.GetNextExp(hearthLv);
		int num4 = result.hearthExp + num3;
		gaugeGold.rectTransform.sizeDelta = new Vector2(Mathf.Min(300, 300 * lastShippingExp / lastShippingExpMax), 50f);
		gaugeHearth.rectTransform.sizeDelta = new Vector2(Mathf.Min(300, 300 * num4 / nextExp), 50f);
		gaugeDebt.rectTransform.sizeDelta = new Vector2(Mathf.Min(300f, 300f * ((float)result.debt / 20000000f)), 50f);
		textGold.text = "shipping_nextGold".lang(lastShippingExp.ToFormat(), lastShippingExpMax.ToFormat());
		int num5 = shippingBonus2 - shippingBonus;
		textGold2.text = ((num5 == 0) ? "" : ("+" + num5));
		textHearth.text = "shipping_hearth".lang(hearthLv.ToString() ?? "", (100f * (float)num4 / (float)nextExp).ToString("F1"));
		float num6 = 100f * (float)num3 / (float)nextExp;
		textHearth2.text = ((num6 == 0f) ? "" : ("+" + num6.ToString("F1") + "%"));
		textDebt.text = (ELayer.game.quests.IsStarted<QuestDebt>() ? (Lang._currency(result.debt, showUnit: true) ?? "") : "???");
		gaugeDebt.SetActive(ELayer.game.quests.IsStarted<QuestDebt>());
		if (shippingBonus2 - shippingBonus != lastBonus)
		{
			lastBonus = shippingBonus2 - shippingBonus;
			if (showEffect)
			{
				SE.Play("ore_drop");
			}
		}
		update++;
		if (showEffect && update > maxUpdate)
		{
			SE.Play("regi");
			showEffect = false;
		}
	}

	public void Show(ShippingResult _result)
	{
		result = _result;
		Zone zone = ELayer.game.spatials.Find(result.uidZone) ?? ELayer.pc.homeZone;
		_ = zone.branch;
		list.callbacks = new UIList.Callback<ShippingResult.Item, UIItem>
		{
			onInstantiate = delegate(ShippingResult.Item a, UIItem b)
			{
				b.text1.SetText(a.text);
				b.text2.SetText(a.income.ToFormat());
			}
		};
		list.Clear();
		foreach (ShippingResult.Item item in result.items)
		{
			list.Add(item);
		}
		list.Refresh();
		Date date = Date.ToDate(result.rawDate);
		textTitle.text = date.month + "/" + date.day + " " + date.year;
		textHome.text = "shipping_home".lang(zone.Name);
		Refresh();
		int count = ELayer.player.shippingResults.Count;
		int index = ELayer.player.shippingResults.IndexOf(result);
		buttonNext.SetActive(index != -1 && count > index + 1);
		buttonPrev.SetActive(index > 0);
		buttonNext.SetOnClick(delegate
		{
			showEffect = false;
			Show(ELayer.player.shippingResults[index + 1]);
		});
		buttonPrev.SetOnClick(delegate
		{
			showEffect = false;
			Show(ELayer.player.shippingResults[index - 1]);
		});
		this.RebuildLayout(recursive: true);
	}
}
