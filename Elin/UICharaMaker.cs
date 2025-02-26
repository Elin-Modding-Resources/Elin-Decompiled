using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UICharaMaker : EMono
{
	public Chara chara;

	public Portrait portrait;

	public Portrait portraitZoom;

	public InputField inputName;

	public InputField inputAlias;

	public InputField inputJob;

	public InputField inputRace;

	public InputField inputGender;

	public InputField inputAge;

	public UISlider sliderPortrait;

	public List<SourceRace.Row> races = new List<SourceRace.Row>();

	public List<SourceJob.Row> jobs = new List<SourceJob.Row>();

	public UIButton toggleExtra;

	public UIButton toggleParentLock;

	public UINote note;

	public UINote note2;

	public UINote noteJob;

	public UINote noteRace;

	public bool addShadow;

	public bool extraRace;

	public UIText textSign;

	public UIText textMode;

	public int ageIndex;

	public Vector2 posList;

	public Vector2 posList2;

	[NonSerialized]
	public string[] listMode;

	[NonSerialized]
	public string[] listDifficulties;

	public Biography bio => chara.bio;

	public string TextUnknown => "lore_unknown".lang();

	public void SetChara(Chara c)
	{
		listMode = Lang.GetList("startMode");
		listDifficulties = Lang.GetList("difficulties");
		textMode.SetText(listMode[EMono.game.idPrologue]);
		chara = c;
		BuildRaces();
		SetPortraitSlider();
		portrait.SetChara(chara, chara.pccData);
		portraitZoom.SetChara(chara);
		Refresh();
		toggleExtra.SetToggle(extraRace, delegate(bool a)
		{
			extraRace = a;
			BuildRaces();
			SE.Tab();
		});
	}

	public void BuildRaces()
	{
		races.Clear();
		jobs.Clear();
		bool flag = EMono.core.config.test.extraRace;
		foreach (SourceRace.Row row in EMono.sources.races.rows)
		{
			if (row.playable == 1 || (row.playable <= 6 && extraRace) || (flag && row.playable != 9))
			{
				races.Add(row);
			}
		}
		foreach (SourceJob.Row row2 in EMono.sources.jobs.rows)
		{
			if (row2.playable == 1 || (row2.playable <= 6 && extraRace) || (flag && row2.playable != 9))
			{
				jobs.Add(row2);
			}
		}
		races.Sort((SourceRace.Row a, SourceRace.Row b) => (a.playable - b.playable) * 10000 + a._index - b._index);
		jobs.Sort((SourceJob.Row a, SourceJob.Row b) => (a.playable - b.playable) * 10000 + a._index - b._index);
	}

	private void Update()
	{
		RefreshPortraitZoom();
	}

	public void RefreshPortraitZoom(bool force = false)
	{
		if (!portraitZoom)
		{
			return;
		}
		List<GameObject> hovered = InputModuleEX.GetPointerEventData().hovered;
		bool enable = false;
		foreach (GameObject item in hovered)
		{
			if (item.transform.IsChildOf(portrait.transform))
			{
				enable = true;
				break;
			}
		}
		portraitZoom.SetActive(enable);
	}

	public void Refresh()
	{
		chara.elements.ApplyPotential();
		Prologue prologue = EMono.game.Prologue;
		textSign.SetText("signDisembark".lang(prologue.month.ToString() ?? "", prologue.day.ToString() ?? "", prologue.year.ToString() ?? ""));
		RefreshPortraitZoom();
		inputAlias.text = chara.Aka;
		inputName.text = chara.c_altName;
		inputRace.text = chara.race.GetText().ToTitleCase(wholeText: true);
		inputJob.text = chara.job.GetText().ToTitleCase(wholeText: true);
		inputGender.text = Lang._gender(chara.bio.gender);
		inputAge.text = Lang.GetList("ages")[ageIndex];
		note.Clear();
		note.AddTopic("name".lang(), "nameBio".lang(chara.NameSimple, chara.Aka));
		note.AddTopic("birthday".lang(), bio.TextBirthDate(chara, _age: true) + ((ageIndex == 0) ? "" : (" (" + Lang.GetList("ages")[ageIndex] + ")")));
		note.AddTopic("appearance".lang(), bio.TextAppearance());
		note.AddTopic("dad".lang(), bio.nameDad.ToTitleCase());
		note.AddTopic("mom".lang(), bio.nameMom.ToTitleCase());
		note.AddTopic("birthLoc".lang(), bio.nameBirthplace.ToTitleCase());
		note.Build();
		noteRace.Clear();
		noteRace.AddHeaderTopic("race".lang() + ": " + chara.race.GetText().ToTitleCase(wholeText: true));
		noteRace.Space(8);
		noteRace.AddText("NoteText_long", chara.race.GetDetail().IsEmpty(TextUnknown)).Hyphenate();
		noteRace.Build();
		noteJob.Clear();
		noteJob.AddHeaderTopic("job".lang() + ": " + chara.job.GetText().ToTitleCase(wholeText: true));
		noteJob.Space(8);
		noteJob.AddText("NoteText_long", chara.job.GetDetail().IsEmpty(TextUnknown)).Hyphenate();
		AddDomain(noteJob, EMono.player.GetDomains(), button: true);
		noteJob.Build();
		note2.Clear();
		note2.AddHeaderTopic("attributes".lang());
		chara.elements.AddNote(note2, (Element e) => e.HasTag("primary"), null, ElementContainer.NoteMode.CharaMake);
		note2.Space();
		note2.AddHeaderTopic("skills".lang());
		chara.elements.AddNote(note2, (Element e) => e.source.category == "skill" && e.ValueWithoutLink > 1 && e.source.categorySub != "weapon", null, ElementContainer.NoteMode.CharaMake);
		note2.Space();
		note2.AddHeaderTopic("feats".lang());
		chara.elements.AddNote(note2, (Element e) => e is Feat && e.id != 1220, null, ElementContainer.NoteMode.CharaMake, addRaceFeat: true);
		note2.Build();
		if (!addShadow)
		{
			return;
		}
		Text[] componentsInChildren = note2.gameObject.GetComponentsInChildren<Text>();
		foreach (Text text in componentsInChildren)
		{
			Shadow shadow = text.GetComponent<Shadow>();
			if (!shadow)
			{
				shadow = text.gameObject.AddComponent<Shadow>();
			}
			shadow.effectColor = Color.black;
		}
	}

	public void ListModes()
	{
		EMono.ui.AddLayer<LayerList>().SetStringList(() => listMode, delegate(int a, string b)
		{
			EMono.game.idPrologue = a;
			Prologue prologue = EMono.game.Prologue;
			EMono.world.date.year = prologue.year;
			EMono.world.date.month = prologue.month;
			EMono.world.date.day = prologue.day;
			EMono.world.weather._currentCondition = prologue.weather;
			textMode.SetText(listMode[EMono.game.idPrologue]);
			Refresh();
		}).SetSize()
			.SetTitles("wStartMode");
	}

	public void RerollAlias()
	{
		chara._alias = AliasGen.GetRandomAlias();
		Refresh();
	}

	public void ListAlias()
	{
		EMono.ui.AddLayer<LayerList>().SetStringList(delegate
		{
			List<string> list = new List<string>();
			for (int i = 0; i < 10; i++)
			{
				list.Add(AliasGen.GetRandomAlias());
			}
			return list;
		}, delegate(int a, string b)
		{
			chara._alias = b;
			Refresh();
		}).SetSize()
			.EnableReroll()
			.SetTitles("wAlias");
	}

	public void RerollName()
	{
		chara.c_altName = NameGen.getRandomName();
		Refresh();
	}

	public void EditName()
	{
		Dialog.InputName("dialogChangeName", chara.c_altName.IsEmpty(chara.NameSimple), delegate(bool cancel, string text)
		{
			if (!cancel)
			{
				chara.c_altName = text;
				Refresh();
			}
		}).SetOnKill(delegate
		{
			EMono.ui.hud.hint.Show("hintEmbarkTop".lang(), icon: false);
		});
	}

	public void OnEndEditName()
	{
		chara.c_altName = inputName.text;
	}

	public void RerollRace()
	{
		chara.ChangeRace(races.RandomItem(chara.race).id);
		RerollBio(keepParent: true);
	}

	public void ListRace()
	{
		TooltipManager.Instance.HideTooltips(immediate: true);
		TooltipManager.Instance.disableHide = "note";
		bool first = true;
		EMono.ui.AddLayer<LayerList>().SetPivot(0.5f, 0.2f).SetSize(260f)
			.SetList2(races, (SourceRace.Row a) => a.GetText().ToTitleCase(wholeText: true), delegate(SourceRace.Row a, ItemGeneral b)
			{
				chara.ChangeRace(a.id);
				RerollBio(keepParent: true);
			}, delegate(SourceRace.Row a, ItemGeneral item)
			{
				UIButton b2 = item.button1;
				b2.SetTooltip(delegate(UITooltip t)
				{
					ElementContainer elementContainer = new ElementContainer();
					elementContainer.ApplyElementMap(EMono.pc.uid, SourceValueType.Chara, a.elementMap, 1);
					elementContainer.ApplyPotential(1);
					t.note.Clear();
					t.note.AddHeader(a.GetText().ToTitleCase(wholeText: true));
					elementContainer.AddNoteAll(t.note);
					t.note.AddHeader("lore");
					t.note.AddText("NoteText_long", a.GetDetail().IsEmpty(TextUnknown)).Hyphenate();
					t.note.Space(8);
					t.note.Build();
				});
				if (first)
				{
					TooltipManager.Instance.GetComponent<CanvasGroup>().alpha = 0f;
					TooltipManager.Instance.GetComponent<CanvasGroup>().DOFade(1f, 0.3f);
					EMono.core.actionsNextFrame.Add(delegate
					{
						b2.ShowTooltipForced();
					});
				}
				first = false;
			})
			.SetTitles("wRace");
		RectTransform rectTransform = EMono.ui.GetLayer<LayerList>().windows[0].Rect();
		rectTransform.pivot = new Vector2(0.5f, 0.5f);
		rectTransform.anchoredPosition = posList;
		TweenUtil.Tween(0.3f, null, delegate
		{
			UIButton.TryShowTip();
		});
	}

	public void RerollJob()
	{
		chara.ChangeJob(jobs.RandomItem(chara.job).id);
		EMono.player.RefreshDomain();
		Refresh();
	}

	public void ListJob()
	{
		TooltipManager.Instance.HideTooltips(immediate: true);
		TooltipManager.Instance.disableHide = "note";
		bool first = true;
		EMono.ui.AddLayer<LayerList>().SetPivot(0.5f, 0.1f).SetSize(260f)
			.SetList2(jobs, (SourceJob.Row a) => a.GetText().ToTitleCase(wholeText: true), delegate(SourceJob.Row a, ItemGeneral b)
			{
				chara.ChangeJob(a.id);
				EMono.player.RefreshDomain();
				RerollBio(keepParent: true);
			}, delegate(SourceJob.Row a, ItemGeneral item)
			{
				UIButton b2 = item.button1;
				b2.SetTooltip(delegate(UITooltip t)
				{
					ElementContainer elementContainer = new ElementContainer();
					elementContainer.ApplyElementMap(EMono.pc.uid, SourceValueType.Chara, a.elementMap, 1);
					elementContainer.ApplyPotential(2);
					t.note.Clear();
					t.note.AddHeader(a.GetText().ToTitleCase(wholeText: true));
					elementContainer.AddNoteAll(t.note);
					t.note.AddHeader("lore");
					t.note.AddText("NoteText_long", a.GetDetail().IsEmpty(TextUnknown)).Hyphenate();
					AddDomain(t.note, new ElementContainer().ImportElementMap(a.domain), button: false);
					t.note.Build();
				});
				if (first)
				{
					TooltipManager.Instance.GetComponent<CanvasGroup>().alpha = 0f;
					TooltipManager.Instance.GetComponent<CanvasGroup>().DOFade(1f, 0.3f);
					EMono.core.actionsNextFrame.Add(delegate
					{
						b2.ShowTooltipForced();
					});
				}
				first = false;
			})
			.SetTitles("wClass");
		RectTransform rectTransform = EMono.ui.GetLayer<LayerList>().windows[0].Rect();
		rectTransform.pivot = new Vector2(0.5f, 0.5f);
		rectTransform.anchoredPosition = posList;
		TweenUtil.Tween(0.3f, null, delegate
		{
			UIButton.TryShowTip();
		});
	}

	public void AddDomain(UINote n, ElementContainer domains, bool button)
	{
		n.Space(8);
		string text = "";
		foreach (Element value in domains.dict.Values)
		{
			text = text + ((value == domains.dict.Values.First()) ? "" : ", ") + value.Name;
		}
		UIItem uIItem = n.AddTopic("TopicDomain", "domain".lang(), text);
		uIItem.button1.SetActive(button && EMono.pc.HasElement(1402));
		uIItem.button1.SetOnClick(delegate
		{
			EMono.player.SelectDomain(Refresh);
		});
	}

	public void ListGender()
	{
		EMono.ui.AddLayer<LayerList>().SetStringList(() => Lang.GetList("genders"), delegate(int a, string b)
		{
			if (chara.bio.gender != a)
			{
				chara.bio.SetGender(a);
				chara.c_idPortrait = Portrait.GetRandomPortrait(chara.bio.gender, chara.GetIdPortraitCat());
				portrait.SetChara(chara);
				portraitZoom.SetChara(chara);
			}
			SetPortraitSlider();
			Refresh();
		}).SetTitles("wGender");
	}

	public void ListAge()
	{
		EMono.ui.AddLayer<LayerList>().SetStringList(() => Lang.GetList("ages"), delegate(int a, string b)
		{
			if (ageIndex != a)
			{
				ageIndex = a;
				RerollBio(keepParent: true);
			}
			Refresh();
		}).SetTitles("wAge");
	}

	public void SetPortraitSlider()
	{
	}

	public void RerollPCC()
	{
		chara.pccData.Randomize(null, null, randomizeHairColor: false);
		portrait.SetChara(chara);
		portraitZoom.SetChara(chara);
	}

	public void RerollHair()
	{
		chara.pccData.SetRandomColor("hair");
		portrait.SetChara(chara);
		portraitZoom.SetChara(chara);
	}

	public void RerollBio()
	{
		RerollBio(keepParent: false);
	}

	public void RerollBio(bool keepParent)
	{
		bio.RerollBio(chara, ageIndex, keepParent);
		Refresh();
	}

	public void EditPCC()
	{
		EMono.ui.AddLayer<LayerEditPCC>().Activate(chara, UIPCC.Mode.Body, null, delegate
		{
			portrait.SetChara(chara);
			SetPortraitSlider();
		});
	}

	public void OnClickChangePortrait()
	{
		EMono.ui.AddLayer<LayerEditPortrait>().Activate(chara, chara.pccData, delegate
		{
			portrait.SetChara(chara, chara.pccData);
		});
	}
}
