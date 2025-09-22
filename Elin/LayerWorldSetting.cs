using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LayerWorldSetting : ELayer
{
	public UISelectableGroup groupTemplate;

	public List<UIButton> buttonTemplates;

	public GamePrincipal pp;

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

	public UIText textDetail;

	public UISlider sliderDropRate;

	public Image imageScoreBar;

	public UIButton moldToggle;

	public UIButton moldSlider;

	public UIButton buttonEmbark;

	public UIButton buttonWorkaround;

	public UIItem moldHeader;

	public Transform transCustom;

	public Transform transMold;

	public Transform transHint;

	public List<Sprite> sprites;

	public Sprite spriteNoToken;

	private bool started;

	public int IdxCustom => 3;

	public bool IsEmbark
	{
		get
		{
			if (ELayer.core.IsGameStarted)
			{
				return ELayer.player.resetPrincipal;
			}
			return true;
		}
	}

	public override void OnInit()
	{
		buttonEmbark.SetActive(IsEmbark);
		buttonWorkaround.SetActive(enable: true);
		transHint.SetActive(IsEmbark);
		if (IsEmbark)
		{
			ELayer.game.principal.modified.Clear();
		}
		pp = IO.DeepCopy(ELayer.game.principal);
		transMold.SetActive(enable: false);
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
		buttonWorkaround.SetToggle(ELayer.player.showWorkaround, delegate(bool a)
		{
			ELayer.player.showWorkaround = a;
			Refresh();
		});
		Refresh();
	}

	public void SetTemplate(int idx)
	{
		pp.id = idx;
		if (idx == IdxCustom)
		{
			pp.id = -1;
		}
		else
		{
			pp = IO.DeepCopy(ELayer.setting.start.principals[idx]);
		}
		Refresh();
	}

	public void RefreshTemplate()
	{
		if (pp.IsCustom)
		{
			groupTemplate.Select(buttonTemplates.LastItem());
		}
		else
		{
			groupTemplate.Select(pp.id);
		}
		textDetail.SetText(("vow_" + pp.id).lang());
	}

	public void Refresh()
	{
		RefreshTemplate();
		transCustom.DestroyChildren();
		List<GamePrincipal.Item> items = pp.ListItems();
		AddCategory(GamePrincipal.Type.Oath);
		if (ELayer.player.showWorkaround)
		{
			AddCategory(GamePrincipal.Type.Workaround);
			AddCategory(GamePrincipal.Type.Legacy);
			AddCategory(GamePrincipal.Type.Flavor);
		}
		transCustom.RebuildLayout();
		void AddCategory(GamePrincipal.Type type)
		{
			Util.Instantiate(moldHeader, transCustom).text1.SetText(("pp_" + type).lang());
			foreach (GamePrincipal.Item item in items.Where((GamePrincipal.Item a) => a.type == type))
			{
				UIButton b = null;
				GamePrincipal.ItemSlider itemSlider = item as GamePrincipal.ItemSlider;
				if (itemSlider == null)
				{
					if (item != null)
					{
						_ = item;
						b = Util.Instantiate(moldToggle, transCustom);
					}
				}
				else
				{
					b = Util.Instantiate(moldSlider, transCustom);
					b.GetComponentInChildren<UISlider>().SetSlider(itemSlider.GetInt(), delegate(float a)
					{
						itemSlider.SetInt((int)a);
						return itemSlider.funcText((int)a);
					}, 0, itemSlider.max, notify: false);
				}
				bool flag = item.id == "permadeath" && !IsEmbark && !item.WasSealed();
				b.mainText.SetText(("pp_" + item.id).lang());
				b.icon.SetActive(item.grade < 0 || item.IsSealed() || item.WasSealed());
				b.icon.SetAlpha((item.grade < 0 || item.IsSealed()) ? 1f : 0.3f);
				b.icon.sprite = ((item.grade < 0) ? spriteNoToken : sprites[item.grade]);
				b.icon.SetNativeSize();
				b.GetOrCreate<CanvasGroup>().alpha = (flag ? 0.5f : 1f);
				string text = "pp_" + item.id + "_hint";
				b.SetTooltipLang(text);
				b.tooltip.enable = Lang.Has(text);
				b.interactable = !flag;
				b.SetToggle(item.Get(), delegate(bool a)
				{
					item.Set(a);
					if (!pp.IsCustom)
					{
						pp.id = -1;
						RefreshTemplate();
					}
					b.icon.SetActive(item.grade < 0 || item.IsSealed() || item.WasSealed());
					b.icon.SetAlpha((item.grade < 0 || item.IsSealed()) ? 1f : 0.3f);
				});
			}
		}
	}

	public void StartGame()
	{
		if (ELayer.player.resetPrincipal)
		{
			Close();
			return;
		}
		started = true;
		Close();
		if (!LayerDrama.Instance)
		{
			LayerDrama.ActivateMain("mono", "1-1");
		}
	}

	public override void OnKill()
	{
		Apply();
	}

	public void Apply()
	{
		if (!IsEmbark)
		{
			pp.modified = ELayer.game.principal.modified;
			foreach (GamePrincipal.Item item in pp.ListItems())
			{
				if (item.IsModified())
				{
					pp.modified.Add(item.id);
				}
			}
		}
		ELayer.game.principal = pp;
		if (ELayer.player.resetPrincipal)
		{
			ELayer.player.resetPrincipal = false;
		}
		ELayer.pc.SetFeat(1220, pp.permadeath ? 1 : 0);
	}
}
