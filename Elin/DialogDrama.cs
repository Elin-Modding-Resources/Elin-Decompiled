using System;
using Applibot;
using UnityEngine;
using UnityEngine.UI;

public class DialogDrama : EMono
{
	[Header("Dialog")]
	public Text textName;

	public Text textBio;

	public Text textAffinity;

	public Text textNoInterest;

	public Text textFav;

	public Text textRank;

	public GameObject transInterest;

	public GameObject transAffinity;

	public GameObject transFav;

	public GameObject transRank;

	public UIText textMain;

	public HyphenationJpn hypen;

	public Transform transChoices;

	private UIButton moldButton;

	public GameObject iconNext;

	public GameObject goAffinity;

	public Portrait portrait;

	public LayoutGroup layoutInterest;

	public Transform moldInterest;

	public Font fontRune;

	public Glitch glitch;

	public Image imageBgAdv;

	private bool warned;

	private void Awake()
	{
		moldButton = transChoices.CreateMold<UIButton>();
	}

	public virtual UIButton AddChoice(DramaChoice choice, string text, Action func = null, bool deactivateOnChoice = true)
	{
		if (!transChoices.gameObject.activeSelf)
		{
			transChoices.gameObject.SetActive(value: true);
		}
		UIButton uIButton = Util.Instantiate(moldButton, transChoices);
		uIButton.mainText.text = text;
		if (deactivateOnChoice)
		{
			uIButton.onClick.AddListener(Deactivate);
		}
		if (func != null)
		{
			uIButton.onClick.AddListener(delegate
			{
				func();
			});
		}
		uIButton.RebuildLayout();
		choice.button = uIButton;
		return uIButton;
	}

	public void ClearChoice()
	{
		transChoices.DestroyChildren();
		transChoices.gameObject.SetActive(value: false);
	}

	public virtual void SetText(string detail = "", bool center = false)
	{
		if ((bool)fontRune)
		{
			if (detail.StartsWith("#rune"))
			{
				detail = detail.Replace("#rune", "");
			}
			else
			{
				textMain.ApplySkin();
			}
		}
		textMain.SetText(detail);
		textMain.RebuildLayoutTo<LayerDrama>();
	}

	public void Deactivate()
	{
	}
}
