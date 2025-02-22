using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayerWorldSetting : ELayer
{
	public UISelectableGroup groupTemplate;

	public List<UIButton> buttonTemplates;

	public UIButton toggleDeathPenaltyProtection;

	public UIButton toggleManualSave;

	public UIButton togglePermadeath;

	public UIButton toggleInfiniteMarketFund;

	public UIButton toggleOPMilk;

	public UIText textScore;

	public UIText textTitle;

	public UIText textValidScore;

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
		groupTemplate.Select(pp.IsCustom ? IdxCustom : pp.idTemplate);
		toggleDeathPenaltyProtection.SetToggle(pp.deathPenaltyProtection, delegate(bool a)
		{
			Toggle(ref pp.deathPenaltyProtection, a);
		});
		toggleManualSave.SetToggle(pp.manualSave, delegate(bool a)
		{
			Toggle(ref pp.manualSave, a);
		});
		togglePermadeath.SetToggle(pp.permadeath, delegate(bool a)
		{
			Toggle(ref pp.permadeath, a);
		});
		toggleInfiniteMarketFund.SetToggle(pp.infiniteMarketFund, delegate(bool a)
		{
			Toggle(ref pp.infiniteMarketFund, a);
		});
		toggleOPMilk.SetToggle(pp.opMilk, delegate(bool a)
		{
			Toggle(ref pp.opMilk, a);
		});
		RefreshScore();
		void Toggle(ref bool flag, bool on)
		{
			flag = on;
			if (!pp.IsCustom)
			{
				pp.idTemplate = -1;
				groupTemplate.Select(IdxCustom);
			}
			RefreshScore();
		}
	}

	public void RefreshScore()
	{
		textTitle.text = pp.GetTitle() ?? "";
		textScore.text = "pp_score".lang(pp.GetScore().ToString() ?? "");
		textValidScore.text = "pp_validScore".lang(pp.GetValidScore().ToString() ?? "");
		imageScoreBar.rectTransform.sizeDelta = new Vector2(Mathf.Clamp(300f * (float)pp.GetScore() / 500f, 0f, 300f), 50f);
	}

	public override void OnKill()
	{
		pp.Apply();
	}
}
