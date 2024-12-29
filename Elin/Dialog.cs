using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dialog : ELayer
{
	public enum InputType
	{
		None,
		Default,
		Password,
		Item,
		DistributionFilter
	}

	public Text textConfetti;

	public Image image;

	public UIText textDetail;

	public UINote note;

	public UIButtonList list;

	public LayoutGroup layout;

	public ScreenEffect effect;

	public UIList listGrid;

	public Transform spacer;

	public List<GridItem> gridItems = new List<GridItem>();

	public UIInputText input;

	public InputType inputType;

	[NonSerialized]
	public EInput.KeyMap keymap;

	[NonSerialized]
	public bool isInputEnter;

	public Action<bool, string> onEnterInput;

	public static bool warned;

	public override void OnAfterInit()
	{
		ELayer.ui.hud.textMouseHintRight.transform.parent.SetActive(enable: false);
		textDetail.SetActive(!textDetail.text.IsEmpty());
		listGrid.SetActive(gridItems.Count > 0);
		if (gridItems.Count >= 8)
		{
			GridLayoutGroup obj = listGrid.layoutItems as GridLayoutGroup;
			obj.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
			obj.constraintCount = 8;
		}
		if (gridItems.Count > 0)
		{
			listGrid.Clear();
			listGrid.callbacks = new UIList.Callback<GridItem, ButtonGrid>
			{
				onInstantiate = delegate(GridItem a, ButtonGrid b)
				{
					b.SetItem(a);
				},
				onClick = delegate(GridItem a, ButtonGrid b)
				{
					a.OnClick(b);
				}
			};
			foreach (GridItem gridItem in gridItems)
			{
				listGrid.Add(gridItem);
			}
			listGrid.Refresh();
		}
		list.Refresh();
		this.RebuildLayout(recursive: true);
		GraphicRaycaster g = windows[0].GetComponent<GraphicRaycaster>();
		g.enabled = false;
		TweenUtil.Delay(0.3f, delegate
		{
			if ((bool)g)
			{
				g.enabled = true;
			}
		});
		if ((bool)input)
		{
			input.Focus();
		}
		EInput.WaitReleaseKey();
	}

	public void AddButton(string text, Action onClick = null, bool close = true)
	{
		list.AddButton(null, text, delegate
		{
			if (onClick != null)
			{
				onClick();
			}
			if (close)
			{
				Close();
			}
		});
	}

	private void Update()
	{
		if ((bool)input && option.canClose && Input.GetKeyDown(KeyCode.Escape))
		{
			Close();
		}
	}

	public override void OnUpdateInput()
	{
		if ((bool)input && option.canClose && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
		{
			isInputEnter = true;
			Close();
			return;
		}
		if (!input.gameObject.activeInHierarchy && this.list != null)
		{
			foreach (UIList.ButtonPair button in this.list.buttons)
			{
				UIButton uIButton = button.component as UIButton;
				if ((bool)uIButton && uIButton.interactable && !EInput.waitReleaseAnyKey && (bool)uIButton.keyText && !uIButton.keyText.text.IsEmpty() && uIButton.keyText.text == Input.inputString)
				{
					uIButton.onClick.Invoke();
					return;
				}
			}
		}
		if (keymap != null)
		{
			List<KeyCode> list = new List<KeyCode>
			{
				KeyCode.Mouse0,
				KeyCode.Mouse1,
				KeyCode.Mouse2,
				KeyCode.Mouse3,
				KeyCode.Mouse4,
				KeyCode.Escape,
				KeyCode.Return,
				KeyCode.LeftShift,
				KeyCode.Delete,
				KeyCode.Backspace
			};
			if (Input.GetKeyDown(KeyCode.Delete))
			{
				if (keymap.required)
				{
					SE.Beep();
					return;
				}
				SE.Tab();
				keymap.key = KeyCode.None;
				Close();
				return;
			}
			foreach (KeyCode value in Enum.GetValues(typeof(KeyCode)))
			{
				if (list.Contains(value) || !Input.GetKey(value))
				{
					continue;
				}
				foreach (EInput.KeyMap item in ELayer.core.config.input.keys.List())
				{
					if (item.key == value && item.GetGroup() == keymap.GetGroup())
					{
						item.key = keymap.key;
					}
				}
				keymap.key = value;
				SE.Tab();
				Close();
				return;
			}
		}
		base.OnUpdateInput();
	}

	public void OnEnterInput()
	{
		isInputEnter = true;
		Close();
	}

	public override void OnKill()
	{
		ELayer.ui.hud.textMouseHintRight.transform.parent.SetActive(enable: true);
		if ((bool)input && onEnterInput != null)
		{
			onEnterInput(!isInputEnter, input.Text);
		}
		if ((bool)input)
		{
			input.field.DeactivateInputField();
		}
		EInput.WaitReleaseKey();
	}

	public static Dialog CreateNarration(string idImage, string idText)
	{
		Dialog dialog = Layer.Create<Dialog>("DialogNarration");
		dialog.image.SetActive(enable: true);
		dialog.image.sprite = Resources.Load<Sprite>("Media/Graphics/Image/Dialog/" + idImage);
		dialog.image.SetNativeSize();
		dialog.textDetail.SetText(IO.LoadText(CorePath.CorePackage.TextNarration + idText + ".txt"));
		return dialog;
	}

	public static Dialog Ok(string langDetail, Action action = null)
	{
		Dialog dialog = Layer.Create<Dialog>();
		dialog.textDetail.SetText(langDetail.lang() + " ");
		dialog.list.AddButton(null, Lang.Get("ok"), dialog.Close);
		if (action != null)
		{
			dialog.SetOnKill(action);
		}
		ELayer.ui.AddLayer(dialog);
		return dialog;
	}

	public static Dialog Choice(string langDetail, Action<Dialog> action)
	{
		Dialog dialog = Layer.Create<Dialog>();
		dialog.textDetail.SetText(langDetail.lang() + " ");
		action(dialog);
		ELayer.ui.AddLayer(dialog);
		return dialog;
	}

	public static Dialog YesNo(string langDetail, Action actionYes, Action actionNo = null, string langYes = "yes", string langNo = "no")
	{
		Dialog d = Layer.Create<Dialog>();
		d.textDetail.SetText(langDetail.lang() + " ");
		d.list.AddButton(null, Lang.Get(langYes), delegate
		{
			if (actionYes != null)
			{
				actionYes();
			}
			d.Close();
		});
		d.list.AddButton(null, Lang.Get(langNo), delegate
		{
			if (actionNo != null)
			{
				actionNo();
			}
			d.Close();
		});
		ELayer.ui.AddLayer(d);
		return d;
	}

	public static Dialog List<TValue>(string langDetail, ICollection<TValue> items, Func<TValue, string> getString, Func<int, string, bool> onSelect, bool canCancel = false)
	{
		Dialog d = Layer.Create<Dialog>();
		d.textDetail.SetText(langDetail.lang() + " ");
		int num = 0;
		foreach (TValue item in items)
		{
			int _i = num;
			d.list.AddButton(null, getString(item).lang(), delegate
			{
				if (onSelect(_i, getString(item)))
				{
					d.Close();
				}
			});
			num++;
		}
		d.option.canClose = canCancel;
		ELayer.ui.AddLayer(d);
		return d;
	}

	public static void TryWarn(string lang, Action action, bool yes = true)
	{
		Dialog d = Layer.Create<Dialog>();
		d.textDetail.SetText(lang.lang() + " ");
		d.SetOnKill(delegate
		{
			ELayer.core.actionsNextFrame.Add(delegate
			{
				ActPlan.warning = false;
			});
		});
		d.list.AddButton(null, Lang.Get("yes"), delegate
		{
			d.Close();
			Commit();
		});
		if (yes)
		{
			d.list.AddButton(null, Lang.Get("yes_dontask"), delegate
			{
				switch (lang)
				{
				case "warn_crime":
					ELayer.core.config.game.ignoreWarnCrime = true;
					break;
				case "warn_mana":
					ELayer.core.config.game.ignoreWarnMana = true;
					break;
				case "warn_disassemble":
					ELayer.core.config.game.ignoreWarnDisassemble = true;
					break;
				case "warn_slaughter":
					ELayer.core.config.game.ignoreWarnSlaughter = true;
					break;
				}
				d.Close();
				Commit();
			});
		}
		d.list.AddButton(null, Lang.Get("no"), delegate
		{
			d.Close();
		});
		if (!yes)
		{
			d.list.AddButton(null, Lang.Get("no_dontask"), delegate
			{
				string text = lang;
				if (!(text == "warn_parallels"))
				{
					if (text == "warn_linuxMod")
					{
						ELayer.core.config.ignoreLinuxModWarning = true;
					}
				}
				else
				{
					ELayer.core.config.ignoreParallelsWarning = true;
				}
				d.Close();
			});
		}
		ELayer.ui.AddLayer(d);
		void Commit()
		{
			warned = true;
			action();
			warned = false;
		}
	}

	public static void TryWarnSlaughter(Action action)
	{
		if (ELayer.core.config.game.ignoreWarnSlaughter)
		{
			warned = true;
			action();
			warned = false;
			ActPlan.warning = false;
		}
		else
		{
			TryWarn("warn_slaughter", action);
		}
	}

	public static void TryWarnCrime(Action action)
	{
		if (!ELayer.core.config.game.warnCrime || ELayer.core.config.game.ignoreWarnCrime)
		{
			warned = true;
			action();
			warned = false;
			ActPlan.warning = false;
		}
		else
		{
			TryWarn("warn_crime", action);
		}
	}

	public static void TryWarnMana(Action action)
	{
		if (!ELayer.core.config.game.warnMana || ELayer.core.config.game.ignoreWarnMana)
		{
			warned = true;
			action();
			warned = false;
			ActPlan.warning = false;
		}
		else
		{
			TryWarn("warn_mana", action);
		}
	}

	public static void TryWarnDisassemble(Action action)
	{
		if (!ELayer.core.config.game.warnDisassemble || ELayer.core.config.game.ignoreWarnDisassemble)
		{
			warned = true;
			action();
			warned = false;
		}
		else
		{
			TryWarn("warn_disassemble", action);
		}
	}

	public static Dialog Gift(string langHeader, bool autoAdd, params Card[] cards)
	{
		return Gift(langHeader, autoAdd, new List<Card>(cards));
	}

	public static Dialog Gift(string langHeader, bool autoAdd, List<Card> list)
	{
		List<GridItem> list2 = new List<GridItem>();
		foreach (Card item in list)
		{
			list2.Add(new GridItemCard
			{
				c = item
			});
		}
		return Gift(langHeader, autoAdd, list2);
	}

	public static Dialog Gift(string langHeader, bool autoAdd, List<GridItem> list)
	{
		Dialog d = Layer.Create<Dialog>();
		d.spacer.SetActive(enable: false);
		d.note.AddHeader(langHeader.IsEmpty("headerGift").lang());
		d.list.AddButton(null, Lang.Get("ok"), delegate
		{
			if (autoAdd)
			{
				foreach (GridItem item in list)
				{
					item.AutoAdd();
				}
			}
			d.Close();
		});
		d.option.soundActivate = null;
		d.option.canClose = false;
		d.gridItems = list;
		ELayer.Sound.Play("good");
		ELayer.ui.AddLayer(d);
		return d;
	}

	public static Dialog Recipe(List<RecipeSource> list)
	{
		Dialog d = Layer.Create<Dialog>();
		d.spacer.SetActive(enable: false);
		d.note.AddHeader("giftRecipe".lang());
		d.list.AddButton(null, Lang.Get("ok"), delegate
		{
			d.Close();
		});
		d.option.soundActivate = null;
		for (int i = 0; i < list.Count; i++)
		{
			RecipeSource recipeSource = list[i];
			List<Recipe.Ingredient> ingredients = recipeSource.GetIngredients();
			if (ingredients.Count > 0 && ELayer.sources.cards.map.ContainsKey(ingredients[0].id))
			{
				ELayer.sources.cards.map[ingredients[0].id].GetName();
			}
			d.note.AddText("・" + recipeSource.Name.ToTitleCase());
			if (i >= 9 && list.Count > 10)
			{
				d.note.Space(6);
				d.note.AddText("moreRecipes".lang((list.Count - 10).ToString() ?? ""));
				break;
			}
		}
		d.SetOnKill(SE.Click);
		ELayer.Sound.Play("idea");
		ELayer.ui.AddLayer(d);
		return d;
	}

	public static Dialog Confetti(string langTitle, string langDetail, string langConfetti = "Grats!")
	{
		ELayer.Sound.Play("confetti");
		return _Confetti("DialogConfetti", langTitle, langDetail, langConfetti);
	}

	public static Dialog ConfettiSimple(string langTitle, string langDetail, string langConfetti = "Grats!")
	{
		ELayer.Sound.Play("confettiSimple");
		return _Confetti("DialogConfettiSimple", langTitle, langDetail, langConfetti);
	}

	public static Dialog _Confetti(string idPrefab, string langTitle, string langDetail, string langConfetti = "Grats!")
	{
		Dialog d = Layer.Create(idPrefab) as Dialog;
		d.textConfetti.text = langConfetti.lang();
		d.textDetail.SetText(langDetail.lang());
		d.list.AddButton(null, Lang.Get("ok"), delegate
		{
			d.Close();
		});
		ELayer.ui.AddLayer(d);
		return d;
	}

	public static Dialog Keymap(EInput.KeyMap keymap)
	{
		Dialog dialog = Layer.Create<Dialog>("DialogKeymap");
		dialog.textDetail.SetText("dialog_keymap".lang(("key_" + keymap.action).lang()));
		dialog.keymap = keymap;
		ELayer.ui.AddLayer(dialog);
		return dialog;
	}

	public static Dialog InputName(string langDetail, string text, Action<bool, string> onClose, InputType inputType = InputType.Default)
	{
		Dialog d = Layer.Create<Dialog>("DialogInput");
		d.inputType = inputType;
		d.langHint = langDetail;
		d.note.AddText(langDetail.lang()).text1.alignment = TextAnchor.MiddleCenter;
		switch (inputType)
		{
		case InputType.DistributionFilter:
			d.input.field.characterLimit = 100;
			break;
		case InputType.Item:
			d.input.field.characterLimit = 30;
			break;
		case InputType.Password:
			d.input.field.characterLimit = 8;
			d.input.field.contentType = InputField.ContentType.Alphanumeric;
			break;
		}
		d.input.Text = text;
		d.onEnterInput = onClose;
		d.list.AddButton(null, Lang.Get("ok"), delegate
		{
			onClose(arg1: false, d.input.Text);
			d.Close();
		});
		d.list.AddButton(null, Lang.Get("cancel"), delegate
		{
			onClose(arg1: true, "");
			d.Close();
		});
		ELayer.ui.AddLayer(d);
		return d;
	}
}
