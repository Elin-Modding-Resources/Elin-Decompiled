using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonElement : UIButton
{
	public enum Mode
	{
		Skill,
		Attribute,
		Tech,
		Feat,
		Enchant,
		Policy,
		OnlyValue,
		FeatPurchase,
		LandFeat,
		FeatMini
	}

	public Image imagePotential;

	public GameObject goExp;

	public Element e;

	public ElementContainer owner;

	public UIButton buttonUpgrade;

	public UIItem moldItemResist;

	public LayoutGroup layout;

	public Mode mode;

	public void SetElement(Element _e, ElementContainer _owner, Mode _mode = Mode.Skill)
	{
		e = _e;
		owner = _owner;
		mode = _mode;
		Refresh();
	}

	public void Refresh()
	{
		string text = "";
		if ((bool)WindowChara.Instance && WindowChara.Instance.chara == EClass.pc && EClass.player.trackedElements.Contains(e.id))
		{
			text = "*";
		}
		switch (mode)
		{
		case Mode.Tech:
		{
			mainText.SetText(text + e.Name + " " + e.DisplayValue, FontColor.Default);
			int techUpgradeCost = EClass.Branch.GetTechUpgradeCost(e);
			int currency = EClass.pc.GetCurrency("money2");
			if (EClass.Branch.elements.ValueWithoutLink(e.id) == 0 || e.source.cost[0] == 0 || techUpgradeCost == 0)
			{
				subText.SetText("-");
			}
			else
			{
				subText.SetText(techUpgradeCost.ToString() ?? "", (currency >= techUpgradeCost) ? FontColor.Good : FontColor.Bad);
			}
			break;
		}
		case Mode.Policy:
			mainText.SetText(text + e.Name, FontColor.Default);
			e.SetTextValue(subText);
			Mathf.Clamp((float)e.Potential / 300f, 0.1f, 1f);
			if ((bool)imagePotential)
			{
				imagePotential.SetActive(enable: false);
			}
			break;
		case Mode.LandFeat:
		{
			string text3 = text + e.Name;
			if (e.HasTag("network") && EClass.Branch != null && EClass.Branch.HasNetwork)
			{
				text3 = "feat_network".lang(text3);
			}
			mainText.SetText(text3 + " " + e.Value, FontColor.Default);
			subText.horizontalOverflow = HorizontalWrapMode.Overflow;
			subText.text = e.GetDetail().SplitNewline()[0].StripLastPun();
			if ((bool)imagePotential)
			{
				imagePotential.SetActive(enable: false);
			}
			break;
		}
		case Mode.Feat:
		case Mode.FeatPurchase:
		case Mode.FeatMini:
			if (mode == Mode.FeatPurchase)
			{
				e.IsPurchaseFeatReqMet(EClass.pc.elements);
				mainText.SetText(e.FullName, e.IsPurchaseFeatReqMet(EClass.pc.elements) ? FontColor.Default : FontColor.Warning);
				subText.text = e.GetDetail().SplitNewline()[0].StripLastPun();
				subText2.text = "".TagColor((EClass.pc.feat >= e.CostLearn) ? EClass.Colors.Skin.textGood : EClass.Colors.Skin.textBad, e.CostLearn.ToString() ?? "");
			}
			else
			{
				string text2 = ((mode == Mode.FeatMini) ? e.FullName : e.source.GetText("textPhase").SplitNewline().TryGet(e.Value - 1)
					.StripLastPun());
				FontColor c = (e.HasTag("neg") ? FontColor.Bad : FontColor.ButtonSelectable);
				if (e.source.category == "ether")
				{
					c = FontColor.Ether;
				}
				mainText.SetText(text + text2, c);
				subText.text = ((mode == Mode.FeatMini) ? "" : (e as Feat).GetHint(owner));
			}
			if ((bool)imagePotential)
			{
				imagePotential.SetActive(enable: false);
			}
			break;
		case Mode.Attribute:
			mainText.SetText(text + e.Name, FontColor.Default);
			e.SetTextValue(subText);
			break;
		case Mode.OnlyValue:
			e.SetTextValue(subText);
			imagePotential.SetActive(enable: false);
			break;
		default:
		{
			mainText.SetText(text + e.Name, (e.owner == null || !e.IsFactionElement(e.owner.Card as Chara)) ? FontColor.Default : FontColor.Myth);
			e.SetTextValue(subText);
			bool flag = e.source.category == "skill";
			if ((bool)imagePotential)
			{
				imagePotential.SetActive(flag && imagePotential.enabled);
			}
			break;
		}
		}
		if ((bool)imagePotential)
		{
			int num = (e.Potential - 80) / 20;
			imagePotential.enabled = e.Potential != 80;
			imagePotential.sprite = EClass.core.refs.spritesPotential[Mathf.Clamp(Mathf.Abs(num), 0, EClass.core.refs.spritesPotential.Count - 1)];
			imagePotential.color = ((num >= 0) ? Color.white : new Color(1f, 0.7f, 0.7f));
		}
		e.SetImage(icon);
		if (mode == Mode.FeatPurchase)
		{
			Element refEle = Element.Create(e.id, e.Value - 1);
			SetTooltip("note", delegate(UITooltip tt)
			{
				e.WriteNoteWithRef(tt.note, owner, null, refEle);
			});
		}
		else
		{
			SetTooltip("note", delegate(UITooltip tt)
			{
				e.WriteNote(tt.note, owner);
			});
		}
	}

	public void SetGrid(Element e, Chara c)
	{
		string text = "";
		bool flag = false;
		IList<BodySlot> list = c.body.slots.Copy();
		list.Sort((BodySlot a, BodySlot b) => c.body.GetSortVal(b) - c.body.GetSortVal(a));
		foreach (BodySlot item in list)
		{
			if (item.elementId == 44)
			{
				continue;
			}
			UIItem uIItem = Util.Instantiate(moldItemResist, layout);
			Thing thing = item.thing;
			if (thing == null)
			{
				uIItem.text1.SetActive(enable: false);
				uIItem.image1.color = Color.white.SetAlpha(0.5f);
				continue;
			}
			if (!thing.IsIdentified)
			{
				flag = true;
				uIItem.text1.SetText("?", FontColor.Passive);
				uIItem.image1.color = Color.white.SetAlpha(0.5f);
				continue;
			}
			Element element = item.thing.elements.GetElement(e.id);
			int num = 0;
			if (element != null)
			{
				num = element.Value;
				if (element.IsGlobalElement && thing.c_idDeity != EClass.pc.faith.id)
				{
					uIItem.text1.SetActive(enable: false);
					uIItem.image1.color = Color.white.SetAlpha(0.5f);
					continue;
				}
			}
			if (num == 0)
			{
				uIItem.text1.SetActive(enable: false);
				uIItem.image1.color = Color.white.SetAlpha(0.5f);
			}
			text = (e.IsFlag ? "✓" : (Mathf.Abs(num).ToString() ?? ""));
			FontColor c2 = ((num >= 0) ? FontColor.Good : FontColor.Bad);
			if (element != null)
			{
				if (!element.CanLink(thing.elements))
				{
					c2 = FontColor.Gray;
				}
				if (element.IsGlobalElement)
				{
					c2 = FontColor.Myth;
				}
			}
			uIItem.text1.SetText(text, c2);
		}
		text = "";
		int value = e.Value;
		int num2 = value;
		if (e is Resistance)
		{
			num2 = Element.GetResistLv(value);
			text = ((num2 <= 0) ? Lang.GetList("resistNeg")[-num2] : Lang.GetList("resist")[Mathf.Min(num2, 5)]);
		}
		text = text + " (" + (flag ? "?" : (e.IsFlag ? "✓" : (value.ToString() ?? ""))) + ")";
		Sprite sprite = e.GetIcon();
		if (sprite != null && (bool)icon)
		{
			icon.sprite = sprite;
		}
		mainText.SetText(e.ShortName, (!e.IsFactionElement(c)) ? FontColor.Default : FontColor.Myth);
		subText.SetText(text, (num2 == 0) ? FontColor.Default : ((num2 > 0) ? FontColor.Good : FontColor.Bad));
		SetTooltip("note", delegate(UITooltip tt)
		{
			e.WriteNote(tt.note, c.elements);
		});
	}
}
