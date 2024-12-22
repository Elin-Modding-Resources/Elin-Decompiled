using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LayerEditPortrait : ELayer
{
	public Portrait portrait;

	public Chara chara;

	public UISlider sliderPortrait;

	public UIColorPicker uiPicker;

	public UIDynamicList list;

	public PCCData pcc;

	public UIButton buttonGender;

	public UIButton buttonUnique;

	public Color hairColor;

	public Action<Color> action;

	private bool hasColorChanged;

	private bool gender = true;

	private bool unique;

	public void Activate(Chara _chara, PCCData _pcc = null, Action<Color> _action = null)
	{
		chara = _chara;
		pcc = _pcc;
		action = _action;
		if (action == null)
		{
			action = delegate
			{
			};
		}
		portrait.SetChara(chara);
		if (chara.GetInt(105) == 0)
		{
			hairColor = portrait.GetRandomHairColor(chara);
		}
		else
		{
			hairColor = IntColor.FromInt(chara.GetInt(105));
		}
		if (pcc != null)
		{
			hairColor = pcc.GetHairColor();
		}
		uiPicker.SetColor(hairColor, hairColor, delegate(PickerState state, Color _c)
		{
			ColorUtility.TryParseHtmlString("#" + ColorUtility.ToHtmlStringRGBA(_c), out var color);
			if (pcc == null)
			{
				chara.SetInt(105, IntColor.ToInt(color));
			}
			else
			{
				pcc.SetColor("hair", color);
			}
			portrait.SetChara(chara, pcc);
			action(color);
			hairColor = color;
			hasColorChanged = true;
		});
		RefreshList();
		InvokeRepeating("RefreshHairColor", 0f, 0.1f);
		buttonGender = windows[0].AddBottomButton("portrait_gender", delegate
		{
			gender = !gender;
			RefreshButtons();
			list.List();
		});
		buttonUnique = windows[0].AddBottomButton("portrait_unique", delegate
		{
			unique = !unique;
			RefreshButtons();
			list.List();
		});
		RefreshButtons();
	}

	public void RefreshButtons()
	{
		buttonGender.mainText.SetText("portrait_gender".lang() + " (" + (gender ? "On" : "Off") + ")");
		buttonUnique.mainText.SetText("portrait_unique".lang() + " (" + (unique ? "On" : "Off") + ")");
		buttonGender.RebuildLayout();
		buttonUnique.RebuildLayout();
	}

	public void OnClickClear()
	{
		SE.Trash();
		chara.c_idPortrait = null;
		portrait.SetChara(chara, pcc);
		action(hairColor);
	}

	public void RefreshHairColor()
	{
		if (hasColorChanged)
		{
			list.Redraw();
			hasColorChanged = false;
		}
	}

	public void RefreshList()
	{
		list.Clear();
		list.callbacks = new UIList.Callback<ModItem<Sprite>, Portrait>
		{
			onClick = delegate(ModItem<Sprite> a, Portrait b)
			{
				list.Select(a);
				SE.Click();
				chara.c_idPortrait = a.id;
				portrait.SetChara(chara, pcc);
				action(hairColor);
			},
			onRedraw = delegate(ModItem<Sprite> a, Portrait b, int i)
			{
				b.mainText.SetText(a.id);
				b.SetPortrait(a.id, hairColor);
				b.tooltip.lang = a.id;
			},
			onList = delegate
			{
				int num = (gender ? chara.bio.gender : 0);
				IEnumerable<ModItem<Sprite>> enumerable = Portrait.ListPortraits(num, "c").Concat(Portrait.ListPortraits(num, "guard")).Concat(Portrait.ListPortraits(num, "special"))
					.Concat(Portrait.ListPortraits(num, "foxfolk"));
				if (unique)
				{
					enumerable = enumerable.Concat(Portrait.ListPortraits(0, "UN"));
				}
				foreach (ModItem<Sprite> item in enumerable.ToList())
				{
					list.Add(item);
				}
			}
		};
		list.List();
		list.Select((ModItem<Sprite> a) => a.id == chara.c_idPortrait);
	}
}
