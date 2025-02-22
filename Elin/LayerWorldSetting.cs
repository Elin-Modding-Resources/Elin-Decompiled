using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayerWorldSetting : ELayer
{
	public UISelectableGroup groupTemplate;

	public List<UIButton> buttonTemplates;

	public UIButton toggleEvaluate;

	public UIButton toggleDeathPenaltyProtection;

	public UIButton toggleManualSave;

	public UIButton togglePermadeath;

	public UIButton toggleInfiniteMarketFund;

	public UIButton toggleOPMilk;

	public UIButton toggleUsermapBenefit;

	public UIButton toggleDropRate;

	public UIButton toggleTax;

	public UIText textScore;

	public UIText textTitle;

	public UIText textValidScore;

	public UISlider sliderDropRate;

	public Image imageScoreBar;

	public GamePrincipal pp => ELayer.game.principal;

	public int IdxCustom => 3;

	public override void OnInit()
	{
		for (int j = 0; j < buttonTemplates.Count; j++)
		{
			int i = j;
			UIButton uIButton = buttonTemplates[i];
			uIButton.mainText.SetText(Lang.GetList("pp_templates")[i]);
			if (i != IdxCustom)
			{
				uIButton.refObj = ELayer.setting.start.principals[i];
			}
			uIButton.SetOnClick(delegate
			{
				SetTemplate(i);
			});
		}
		Refresh();
	}

	public void SetTemplate(int idx)
	{
		pp.idTemplate = idx;
		if (idx == IdxCustom)
		{
			pp.idTemplate = -1;
		}
		else
		{
			ELayer.game.principal = IO.DeepCopy(ELayer.setting.start.principals[idx]);
		}
		Refresh();
	}

	public void Refresh()
	{
		if (pp.IsCustom)
		{
			groupTemplate.Select(buttonTemplates.LastItem());
		}
		else
		{
			groupTemplate.Select(pp.idTemplate);
		}
		toggleEvaluate.SetToggleWithScore(pp.ignoreEvaluate, delegate(bool a)
		{
			Toggle(ref pp.ignoreEvaluate, a);
			Refresh();
		}, 0);
		toggleTax.SetToggleWithScore(pp.tax, delegate(bool a)
		{
			Toggle(ref pp.tax, a);
		}, pp.GetScore("tax"));
		toggleDeathPenaltyProtection.SetToggleWithScore(pp.disableDeathPenaltyProtection, delegate(bool a)
		{
			Toggle(ref pp.disableDeathPenaltyProtection, a);
		}, pp.GetScore("disableDeathPenaltyProtection"));
		toggleManualSave.SetToggleWithScore(pp.disableManualSave, delegate(bool a)
		{
			Toggle(ref pp.disableManualSave, a);
		}, pp.GetScore("disableManualSave"));
		toggleUsermapBenefit.SetToggleWithScore(pp.disableUsermapBenefit, delegate(bool a)
		{
			Toggle(ref pp.disableUsermapBenefit, a);
		}, pp.GetScore("disableUsermapBenefit"));
		toggleDropRate.SetToggleWithScore(pp.dropRate, delegate(bool a)
		{
			Toggle(ref pp.dropRate, a);
		}, pp.GetScore("dropRate"));
		togglePermadeath.SetToggleWithScore(pp.permadeath, delegate(bool a)
		{
			Toggle(ref pp.permadeath, a);
		}, pp.GetScore("permadeath"));
		toggleInfiniteMarketFund.SetToggleWithScore(pp.infiniteMarketFund, delegate(bool a)
		{
			Toggle(ref pp.infiniteMarketFund, a);
		}, pp.GetScore("infiniteMarketFund"));
		toggleOPMilk.SetToggleWithScore(pp.opMilk, delegate(bool a)
		{
			Toggle(ref pp.opMilk, a);
		}, pp.GetScore("opMilk"));
		sliderDropRate.SetSlider(pp.dropRateMtp, (float a) => (float)(int)a * 0.5f + "x", 0, 10, notify: false);
		sliderDropRate.onValueChanged.RemoveAllListeners();
		sliderDropRate.onValueChanged.AddListener(delegate(float a)
		{
			pp.dropRateMtp = (int)a;
			Refresh();
		});
		RefreshScore();
		void Toggle(ref bool flag, bool on)
		{
			flag = on;
			if (!pp.IsCustom)
			{
				pp.idTemplate = -1;
				groupTemplate.Select(buttonTemplates.LastItem());
			}
			RefreshScore();
		}
	}

	public void RefreshScore()
	{
		textTitle.text = pp.GetTitle() ?? "";
		textScore.text = "pp_score".lang(pp.ignoreEvaluate ? " - " : (pp.GetScore().ToString() ?? ""));
		textValidScore.text = "pp_validScore".lang(pp.GetValidScore().ToString() ?? "");
		textValidScore.SetActive(!pp.ignoreEvaluate);
		imageScoreBar.rectTransform.sizeDelta = new Vector2(Mathf.Clamp(300f * (float)pp.GetScore() / 100f, 0f, 300f), 50f);
	}

	public override void OnKill()
	{
		pp.Apply();
	}
}
