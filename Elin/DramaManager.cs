using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DramaManager : EMono
{
	public static Chara TG;

	public LayerDrama layer;

	public CanvasGroup cg;

	public CanvasGroup cgCover;

	public UIDynamicList listCredit;

	public Transform actorPos;

	public Transform endroll;

	public DramaActor moldActor;

	public DramaSequence sequence;

	public DramaOutcome outcome;

	public DialogDrama dialog;

	public DramaSetup setup;

	public GameObject goSkip;

	public Image imageBG;

	public Image imageCover;

	public Font[] fonts;

	public Person tg;

	public bool bgmChanged;

	private List<DramaEvent> customEvents = new List<DramaEvent>();

	public List<DramaChoice> _choices = new List<DramaChoice>();

	public Dictionary<string, string> customTalkTopics = new Dictionary<string, string>();

	public Dictionary<string, string> dictLocalize = new Dictionary<string, string>();

	public float creditSpeed;

	private string lastIF;

	private string lastIF2;

	public DramaEventTalk lastTalk;

	public bool enableTone;

	public bool customEventsAdded;

	public bool idDefaultPassed;

	public int countLine;

	private string idDefault;

	private string textReplace;

	public static Dictionary<string, ExcelData> dictCache = new Dictionary<string, ExcelData>();

	public DramaActor tgActor => sequence.GetActor("tg");

	private void Update()
	{
		if (sequence != null)
		{
			sequence.OnUpdate();
		}
	}

	public DramaSequence Play(DramaSetup setup)
	{
		this.setup = setup;
		cgCover.SetActive(enable: false);
		endroll.SetActive(enable: false);
		tg = setup.person;
		SetDialog();
		sequence = new DramaSequence
		{
			setup = setup,
			manager = this,
			id = setup.sheet
		};
		Load();
		sequence.lastStep = (sequence.lastlastStep = setup.step);
		if (!setup.forceJump.IsEmpty())
		{
			sequence.Play(setup.forceJump);
		}
		else
		{
			sequence.Play();
		}
		return sequence;
	}

	public DramaSequence Load()
	{
		sequence.Clear();
		customEvents.Clear();
		customTalkTopics.Clear();
		moldActor = actorPos.CreateMold<DramaActor>();
		if (tg != null)
		{
			sequence.AddActor("tg", tg);
		}
		sequence.AddActor("pc", new Person(EMono.pc));
		string text = CorePath.DramaData + setup.book + ".xlsx";
		ExcelData excelData = dictCache.TryGetValue(text);
		if (excelData != null && excelData.IsModified())
		{
			excelData = null;
		}
		if (excelData == null)
		{
			excelData = new ExcelData();
		}
		excelData.maxEmptyRows = 10;
		excelData.path = text;
		List<Dictionary<string, string>> list = excelData.BuildList(setup.sheet);
		if (!Lang.isBuiltin && dictLocalize.Count == 0)
		{
			foreach (Dictionary<string, string> item in new ExcelData
			{
				maxEmptyRows = 10,
				path = CorePath.DramaDataLocal + setup.book + ".xlsx"
			}.BuildList(setup.sheet))
			{
				string text2 = item["id"];
				if (!text2.IsEmpty() && !item["text"].IsEmpty())
				{
					dictLocalize.Add(text2, item["text"]);
				}
			}
		}
		dictCache[text] = excelData;
		idDefault = setup.step;
		lastTalk = null;
		enableTone = (customEventsAdded = (idDefaultPassed = false));
		countLine = 0;
		for (int i = 0; i < list.Count; i++)
		{
			ParseLine(list[i]);
			countLine++;
		}
		AddCustomEvents();
		sequence.steps["end"] = 99999;
		sequence.AddActor("narrator", new Person("narrator"));
		return sequence;
	}

	public void AddCustomEvents(string idCustom = "Resident")
	{
		if (customEventsAdded)
		{
			return;
		}
		DramaCustomSequence dramaCustomSequence = new DramaCustomSequence
		{
			manager = this,
			idDefault = idDefault,
			idCustom = idCustom,
			events = customEvents,
			setup = sequence.setup,
			sequence = sequence
		};
		CustomEvent(delegate
		{
			sequence.Exit();
		});
		if (tg != null && tg.hasChara)
		{
			TG = tg.chara;
			dramaCustomSequence.Build(tg.chara);
		}
		if (!sequence.setup.textData.IsEmpty())
		{
			dramaCustomSequence.BuildTextData();
		}
		foreach (DramaEvent customEvent in customEvents)
		{
			AddEvent(customEvent);
		}
		customEventsAdded = true;
	}

	public void ParseLine(Dictionary<string, string> item)
	{
		string[] array = (item.ContainsKey("action") ? item["action"].Split('/') : null);
		string action = ((array != null) ? array[0] : null);
		string text2 = (item.ContainsKey("step") ? item["step"] : null);
		if (text2 == "//")
		{
			return;
		}
		if (text2 == idDefault)
		{
			idDefaultPassed = true;
		}
		string actor = (item.ContainsKey("actor") ? item["actor"] : "#1");
		string[] p = (item.ContainsKey("param") ? item["param"].Split(',') : new string[0]);
		string p2 = ((p.Length != 0) ? p[0] : "");
		string p3 = ((p.Length > 1) ? p[1] : "");
		string p4 = ((p.Length > 2) ? p[2] : "");
		float.TryParse(p2, out var p0f);
		float.TryParse(p3, out var result);
		bool flag = !item["text_JP"].IsEmpty();
		item.TryGetValue("text_JP");
		string text = null;
		if (flag)
		{
			if (!Lang.isBuiltin)
			{
				string key = item["id"];
				if (dictLocalize.ContainsKey(key))
				{
					text = dictLocalize[key];
				}
				else
				{
					text = item.TryGetValue("text_EN");
				}
			}
			else
			{
				text = item["text_" + Lang.langCode];
			}
		}
		if (flag && text.StartsWith("$") && tg != null && tg.hasChara)
		{
			string text3 = text.Split(' ')[0];
			text = text.Replace(text3, tg.chara.GetTalkText(text3.Remove(0, 1)));
		}
		string jump = (item.ContainsKey("jump") ? item["jump"] : null);
		string text4 = (item.ContainsKey("if") ? item["if"] : null);
		string iF = (item.ContainsKey("if2") ? item["if2"] : null);
		string cHECK = (item.ContainsKey("check") ? item["check"] : null);
		bool flag2 = false;
		if (text2 != null && !sequence.steps.ContainsKey(text2) && action != "choice" && action != "cancel")
		{
			sequence.steps.Add(text2, sequence.events.Count);
		}
		if (text4 == "*")
		{
			text4 = lastIF;
			iF = lastIF2;
		}
		else
		{
			lastIF = text4;
			lastIF2 = iF;
		}
		if (!CheckIF(text4) || !CheckIF(iF))
		{
			if (action == "reload")
			{
				string id = "flag" + countLine;
				sequence.AddStep(id);
			}
			return;
		}
		if (LayerDrama.fromBook && action == "editPlaylist")
		{
			action = "BGM";
		}
		switch (action)
		{
		case "disableFullPortrait":
			AddEvent(delegate
			{
				sequence.fullPortrait = false;
			});
			break;
		case "enableTone":
			enableTone = true;
			break;
		case "canSkip":
			AddEvent(delegate
			{
				sequence.skipJump = p2;
			});
			break;
		case "screenLock":
			layer.ShowScreenLock(p2);
			break;
		case "haltBGM":
			EMono.Sound.haltUpdate = true;
			break;
		case "forceBGM":
			AddEvent(delegate
			{
				SoundManager.ForceBGM();
			});
			break;
		case "setFlag":
			AddEvent(delegate
			{
				if (p2.StartsWith("*"))
				{
					Quest quest3 = EMono.game.quests.Get(p2.TrimStart('*'));
					quest3?.ChangePhase(p3.ToInt(quest3.GetType()));
				}
				else
				{
					EMono.player.dialogFlags[p2] = (p3.IsEmpty() ? 1 : int.Parse(p3));
				}
			});
			break;
		case "reload":
		{
			string __step = "flag" + countLine;
			AddEvent(delegate
			{
				Load();
				sequence.Play(jump.IsEmpty(__step));
			}, 0.01f, halt: true);
			sequence.AddStep(__step);
			break;
		}
		case "inject":
		{
			DramaEventTalk dramaEventTalk = lastTalk;
			if (idDefaultPassed)
			{
				AddCustomEvents(p2);
			}
			lastTalk = dramaEventTalk;
			break;
		}
		case "topic":
			customTalkTopics[p2] = text;
			break;
		case "cancel":
			lastTalk.canCancel = true;
			lastTalk.idCancelJump = jump;
			break;
		case "_choices":
			foreach (DramaChoice choice in _choices)
			{
				lastTalk.AddChoice(choice);
			}
			break;
		case "choice":
			if (!CheckIF(text4) || !CheckIF(iF))
			{
				break;
			}
			if (array.Length > 1)
			{
				switch (array[1])
				{
				case "quest":
					text = "deQuest".lang();
					jump = "_quest";
					break;
				case "depart":
					text = "depart".lang();
					jump = "_depart";
					break;
				case "rumor":
					text = "letsTalk".lang();
					jump = "_rumor";
					break;
				case "buy":
					text = "daBuy".lang();
					jump = "_buy";
					break;
				case "sell":
					text = "daSell".lang();
					jump = "_sell";
					break;
				case "give":
					text = "daGive".lang();
					jump = "_give";
					break;
				case "trade":
					text = "daTrade".lang();
					jump = "_trade";
					break;
				case "bye":
					text = "bye".lang();
					jump = "_bye";
					break;
				}
			}
			flag2 = true;
			lastTalk.AddChoice(new DramaChoice(text, jump, p2, cHECK, text4));
			break;
		case "addTempActor":
		{
			Person person = new Person(actor);
			Chara chara = EMono.game.cards.globalCharas.Find(actor) ?? EMono._map.FindChara(actor);
			if (chara != null)
			{
				person.SetChara(chara);
			}
			sequence.AddActor(actor, person);
			break;
		}
		case "addActor":
		{
			if (actor == "god")
			{
				sequence.AddActor(actor, new Person(LayerDrama.currentReligion));
				break;
			}
			DramaActor dramaActor = sequence.AddActor(actor, new Person(actor));
			if (!text.IsEmpty())
			{
				dramaActor.owner.tempName = text;
			}
			break;
		}
		case "invoke":
			if (LayerDrama.fromBook)
			{
				break;
			}
			if (jump.IsEmpty())
			{
				AddEvent(delegate
				{
					typeof(DramaOutcome).GetMethod(p[0]).Invoke(outcome, null);
				});
				break;
			}
			AddEvent(delegate
			{
			}, () => (!(bool)typeof(DramaOutcome).GetMethod(p[0]).Invoke(outcome, null)) ? "" : jump);
			flag2 = true;
			break;
		case "refAction1":
			AddEvent(delegate
			{
				LayerDrama.refAction1();
			});
			break;
		case "refAction2":
			AddEvent(delegate
			{
				LayerDrama.refAction2();
			});
			break;
		case "setBG":
			AddEvent(delegate
			{
				if (p2.IsEmpty())
				{
					imageBG.enabled = false;
				}
				else
				{
					imageBG.enabled = true;
					imageBG.sprite = Resources.Load<Sprite>("Media/Graphics/Image/Drama/" + p2);
				}
			});
			break;
		case "setBG2":
			AddEvent(delegate
			{
				Util.Instantiate<Transform>("Media/Graphics/Image/Drama/" + p2, layer).SetAsFirstSibling();
			});
			break;
		case "glitch":
			AddEvent(delegate
			{
				DramaActor.useGlitch = true;
			});
			break;
		case "setDialog":
			AddEvent(delegate
			{
				SetDialog(p2);
			});
			break;
		case "Playlist":
			AddEvent(delegate
			{
				LayerDrama.haltPlaylist = false;
				EMono.Sound.StopBGM();
				EMono.Sound.currentBGM = null;
				bgmChanged = true;
			});
			break;
		case "editPlaylist":
			AddEvent(delegate
			{
				List<int> list = new List<int>();
				string[] array2 = p;
				foreach (string s in array2)
				{
					list.Add(int.Parse(s));
				}
				EMono._zone.SetBGM(list);
			});
			break;
		case "BGM":
			AddEvent(delegate
			{
				LayerDrama.haltPlaylist = true;
				LayerDrama.maxBGMVolume = true;
				EMono.Sound.PlayBGM(EMono.core.refs.dictBGM[p2.ToInt()]);
				bgmChanged = true;
			});
			break;
		case "BGMStay":
			AddEvent(delegate
			{
				EMono.Sound.PlayBGM(EMono.core.refs.dictBGM[p2.ToInt()]);
			});
			break;
		case "lastBGM":
			AddEvent(delegate
			{
				EMono.Sound.StopBGM(p0f, playLastBGM: true);
			});
			break;
		case "stopBGM":
			AddEvent(delegate
			{
				LayerDrama.haltPlaylist = true;
				EMono.Sound.StopBGM(p0f);
				EMono.Sound.currentBGM = null;
			});
			break;
		case "sound":
			AddEvent(delegate
			{
				if (p3 != "")
				{
					SoundManager.current.MuteBGMFor(float.Parse(p3, CultureInfo.InvariantCulture));
				}
				EMono.Sound.Play(p2);
			});
			break;
		case "haltPlaylist":
			LayerDrama.haltPlaylist = true;
			break;
		case "keepBGM":
			LayerDrama.keepBGM = true;
			break;
		case "alphaInOut":
			AddEvent(delegate
			{
				DOTween.Kill(cg);
				cg.alpha = 1f;
				cg.DOFade(0f, p0f).SetDelay(0.1f);
			}, p0f, halt: true);
			AddEvent(new DramaEventWait((result == 0f) ? 0.1f : result));
			AddEvent(delegate
			{
				DOTween.Kill(cg);
				cg.alpha = 0f;
				cg.DOFade(1f, p0f).SetDelay(0.1f);
			}, p0f);
			break;
		case "alphaIn":
			AddEvent(delegate
			{
				DOTween.Kill(cg);
				cg.alpha = 0f;
				cg.DOFade(1f, p0f).SetDelay(0.1f);
			}, p0f);
			break;
		case "alphaOut":
			AddEvent(delegate
			{
				DOTween.Kill(cg);
				cg.alpha = 1f;
				cg.DOFade(0f, p0f).SetDelay(0.1f);
			}, p0f, halt: true);
			break;
		case "fadeIn":
			AddEvent(delegate
			{
				imageCover.color = ((p3 != null && p3 == "white") ? Color.white : Color.black);
				cgCover.SetActive(enable: true);
				cgCover.alpha = 1f;
				cgCover.DOFade(0f, p0f).SetDelay(0.1f);
			}, p0f);
			break;
		case "fadeOut":
			AddEvent(delegate
			{
				imageCover.color = ((p3 != null && p3 == "white") ? Color.white : Color.black);
				cgCover.SetActive(enable: true);
				cgCover.alpha = 0f;
				cgCover.DOFade(1f, p0f).SetDelay(0.1f);
			}, p0f, halt: true);
			break;
		case "fadeInOut":
			AddEvent(delegate
			{
				imageCover.color = ((p4 != null && p4 == "white") ? Color.white : Color.black);
				cgCover.SetActive(enable: true);
				cgCover.alpha = 0f;
				cgCover.DOFade(1f, p0f).SetDelay(0.1f);
			}, p0f, halt: true);
			AddEvent(new DramaEventWait((result == 0f) ? 0.1f : result));
			AddEvent(delegate
			{
				imageCover.color = ((p4 != null && p4 == "white") ? Color.white : Color.black);
				cgCover.SetActive(enable: true);
				cgCover.alpha = 1f;
				cgCover.DOFade(0f, p0f).SetDelay(0.1f);
			}, p0f);
			break;
		case "hideUI":
			AddEvent(delegate
			{
				EMono.ui.Hide(p0f);
			});
			break;
		case "hideDialog":
			AddEvent(delegate
			{
				dialog.SetActive(enable: false);
			});
			break;
		case "fadeEnd":
			if (p0f == 0f)
			{
				p0f = 1f;
			}
			AddEvent(delegate
			{
				imageCover.color = Color.black;
				cgCover.SetActive(enable: true);
				cgCover.alpha = 0f;
				cgCover.DOFade(1f, p0f).SetDelay(0.1f);
			}, p0f, halt: true);
			AddEvent(delegate
			{
				dialog.SetActive(enable: false);
				imageBG.enabled = false;
			});
			AddEvent(delegate
			{
				DOTween.Kill(cg);
				cg.alpha = 1f;
				cg.DOFade(0f, p0f).SetDelay(0.1f);
			}, p0f, halt: true);
			break;
		case "endroll":
			AddEvent(new DramaEventEndRoll());
			break;
		case "showSkip":
			goSkip.SetActive(value: true);
			break;
		case "canCancel":
			AddEvent(delegate
			{
				sequence.canCancel = bool.Parse(p2);
			});
			break;
		case "wait":
			AddEvent(new DramaEventWait(p0f));
			break;
		case "end":
			AddEvent(new DramaEventExit());
			break;
		case "acceptQuest":
			if (LayerDrama.fromBook)
			{
				break;
			}
			AddEvent(delegate
			{
				Quest quest = EMono.game.quests.globalList.Where((Quest a) => a.source.id == p2).First();
				EMono.game.quests.globalList.Remove(quest);
				EMono.game.quests.Start(quest);
			});
			break;
		case "startQuest":
			AddEvent(delegate
			{
				Quest quest2 = Quest.Create(p2);
				if (!quest2.HasDLC)
				{
					Msg.Say("(Failed DLC check)");
				}
				else
				{
					EMono.game.quests.Start(quest2);
					LayerDrama.currentQuest = quest2;
					if (tg != null && tg.chara != null)
					{
						Debug.Log("Starting Quest:" + quest2?.ToString() + "/" + tg.chara.quest?.ToString() + "/" + (quest2 == tg.chara.quest));
					}
				}
			});
			break;
		case "setQuestClient":
			AddEvent(delegate
			{
				if (LayerDrama.currentQuest != null)
				{
					LayerDrama.currentQuest.SetClient(tg.chara, assignQuest: false);
				}
			});
			break;
		case "updateJournal":
			AddEvent(delegate
			{
				EMono.game.quests.UpdateJournal();
			});
			break;
		case "addKeyItem":
			if (!LayerDrama.fromBook)
			{
				AddEvent(delegate
				{
					EMono.player.ModKeyItem(p2);
				});
			}
			break;
		case "drop":
			if (!LayerDrama.fromBook)
			{
				AddEvent(delegate
				{
					Msg.Say("dropReward");
					CardBlueprint.SetNormalRarity();
					Thing t = ThingGen.Create(p2);
					EMono._zone.AddCard(t, EMono.pc.pos);
				});
			}
			break;
		case "completeQuest":
			if (!LayerDrama.fromBook)
			{
				AddEvent(delegate
				{
					EMono.game.quests.Complete(p2.IsEmpty() ? LayerDrama.currentQuest : EMono.game.quests.Get(p2));
					LayerDrama.currentQuest = null;
				});
			}
			break;
		case "nextPhase":
			if (!LayerDrama.fromBook)
			{
				AddEvent(delegate
				{
					EMono.game.quests.Get(p2).NextPhase();
				});
			}
			break;
		case "addResource":
			AddEvent(delegate
			{
				EMono.BranchOrHomeBranch.resources.Get(p2).Mod(p3.ToInt());
			});
			break;
		case "shake":
			AddEvent(delegate
			{
				Shaker.ShakeCam();
			});
			break;
		case "tutorial":
			LayerDrama.Instance.SetOnKill(delegate
			{
				Tutorial.Play(p2);
			});
			break;
		case "slap":
			AddEvent(delegate
			{
				LayerDrama.Instance.SetOnKill(delegate
				{
					tg.chara.PlaySound("whip");
					tg.chara.Say("slap", tg.chara, EMono.pc);
					EMono.pc.PlayAnime(AnimeID.Shiver);
					EMono.pc.DamageHP(5 + EClass.rndHalf(EMono.pc.MaxHP / 3), 919, 100, AttackSource.Condition);
					EMono.pc.OnInsulted();
				});
			});
			break;
		case "destroyItem":
			AddEvent(delegate
			{
				EMono.pc.things.Find(p2).Destroy();
			});
			break;
		case "focus":
			AddEvent(delegate
			{
				Point pos = sequence.setup.person.chara.pos.Copy();
				EMono.scene.screenElin.focusOption = new BaseGameScreen.FocusOption
				{
					pos = pos
				};
			});
			break;
		case "focusChara":
			AddEvent(delegate
			{
				Point pos2 = EMono._map.FindChara(p2).pos.Copy();
				EMono.scene.screenElin.focusOption = new BaseGameScreen.FocusOption
				{
					pos = pos2,
					speed = p3.IsEmpty("2").ToFloat()
				};
			});
			break;
		case "focusPC":
			AddEvent(delegate
			{
				EMono.scene.screenElin.focusOption = new BaseGameScreen.FocusOption
				{
					player = true,
					speed = p2.IsEmpty("2").ToFloat()
				};
			});
			break;
		case "focusPos":
			AddEvent(delegate
			{
				EMono.scene.screenElin.focusOption = new BaseGameScreen.FocusOption
				{
					pos = new Point(p2.ToInt(), p3.ToInt()),
					speed = p4.IsEmpty("2").ToFloat()
				};
			});
			break;
		case "unfocus":
			AddEvent(delegate
			{
				EMono.scene.screenElin.focusOption = null;
			});
			break;
		case "setAlwaysVisible":
			AddEvent(delegate
			{
				LayerDrama.alwaysVisible = EMono._map.FindChara(p2);
			});
			break;
		case "effect":
			AddEvent(delegate
			{
				Point from = new Point(p[1].ToInt(), p[2].ToInt());
				Effect.Get(p2).Play(from);
			});
			break;
		case "effectEmbarkIn":
			AddEvent(delegate
			{
				EMono._map.RevealAll();
				Util.Instantiate("UI/Layer/LayerEmbark/EmbarkActor_crystal");
			});
			break;
		case "effectEmbarkOut":
			AddEvent(delegate
			{
				UnityEngine.Object.FindObjectOfType<EmbarkActor>().Hide();
			});
			break;
		case "propEnter":
			AddEvent(delegate
			{
				DramaProp component = Util.Instantiate("Media/Drama/Prop/" + p2).GetComponent<DramaProp>();
				component.name = p2;
				if (p3.IsEmpty())
				{
					component.transform.position = EMono.scene.cam.transform.position;
				}
				else
				{
					Point point = new Point(p[1].ToInt(), p[2].ToInt());
					component.transform.position = point.PositionCenter();
				}
				component.Enter();
			});
			break;
		case "propLeave":
			AddEvent(delegate
			{
				GameObject.Find(p2).GetComponent<DramaProp>().Leave();
			});
			break;
		case "destroy":
			AddEvent(delegate
			{
				Chara chara2 = EMono._map.FindChara(p2);
				if (chara2 != null)
				{
					chara2.Destroy();
				}
				else
				{
					Debug.Log("Drama.destroy chara not found:" + p2);
				}
			});
			break;
		case "bout_win":
		case "bout_lose":
			AddEvent(delegate
			{
				LayerDrama.Instance.SetOnKill(delegate
				{
					Zone zone = EMono.game.spatials.Find(EMono._zone.instance.uidZone);
					if (action == "bout_win")
					{
						zone.events.AddPreEnter(new ZonePreEnterBoutWin
						{
							target = tg.chara
						});
					}
					EMono.pc.MoveZone(zone);
				});
			});
			break;
		case "save":
			AddEvent(delegate
			{
				EMono.game.Save(isAutoSave: false, silent: true);
			});
			break;
		case "setHour":
			AddEvent(delegate
			{
				EMono.world.date.hour = (int)p0f;
				EMono.world.date.min = 0;
				EMono.scene.OnChangeHour();
				EMono.scene.screenElin.RefreshAll();
				EMono.pc.RecalculateFOV();
			});
			break;
		case "%worship":
			AddEvent(delegate
			{
				LayerDrama.currentReligion.JoinFaith(EMono.pc);
				Tutorial.Reserve("faith");
			});
			break;
		case "replace":
			AddEvent(delegate
			{
				textReplace = text;
			});
			break;
		default:
			if (!flag)
			{
				break;
			}
			lastTalk = AddEvent(new DramaEventTalk(actor, delegate
			{
				if (!textReplace.IsEmpty())
				{
					text = textReplace;
					textReplace = null;
				}
				if (tg != null && (actor == "tg" || actor.IsEmpty()))
				{
					text = tg.ApplyTone(text);
				}
				return text;
			})) as DramaEventTalk;
			lastTalk.center = p2 == "center";
			break;
		case "new":
		case "saveBGM":
		case "addAffinity":
		case "checkAffinity":
			break;
		}
		if (!string.IsNullOrEmpty(jump) && !flag2)
		{
			AddEvent(new DramaEventGoto(jump));
		}
	}

	public DramaActor GetActor(string id)
	{
		return sequence.GetActor(id);
	}

	public DramaEvent AddEvent(DramaEvent e)
	{
		return sequence.AddEvent(e);
	}

	public DramaEventMethod AddEvent(Action func, float duration = 0f, bool halt = false)
	{
		return AddEvent(new DramaEventMethod(func, duration, halt)) as DramaEventMethod;
	}

	public DramaEventMethod AddEvent(Action func, Func<string> funcJump)
	{
		return AddEvent(new DramaEventMethod(func)
		{
			jumpFunc = funcJump
		}) as DramaEventMethod;
	}

	public DramaEvent CustomEvent(Action func, string step = null, float duration = 0f, bool halt = false)
	{
		DramaEventMethod dramaEventMethod = new DramaEventMethod(func, duration, halt)
		{
			step = step
		};
		customEvents.Add(dramaEventMethod);
		return dramaEventMethod;
	}

	public void SetDialog(string id = "Default")
	{
		DialogDrama[] componentsInChildren = GetComponentsInChildren<DialogDrama>(includeInactive: true);
		foreach (DialogDrama dialogDrama in componentsInChildren)
		{
			if ((bool)dialogDrama.portrait)
			{
				dialogDrama.portrait.imageFull.SetActive(enable: false);
			}
			dialogDrama.SetActive(enable: false);
			if (dialogDrama.name == "Dialog" + id)
			{
				dialog = dialogDrama;
			}
		}
	}

	public bool CheckIF(string IF)
	{
		if (IF == null || IF.IsEmpty())
		{
			return true;
		}
		string[] array = IF.Split(',');
		switch (array[0])
		{
		case "fromBook":
			return LayerDrama.fromBook;
		case "!fromBook":
			return !LayerDrama.fromBook;
		case "WindRest":
			return EMono._zone is Zone_WindRest;
		case "guild_promote":
			return Guild.Current.relation.exp >= Guild.Current.relation.ExpToNext;
		case "guild_maxpromote":
			return Guild.Current.relation.rank >= Guild.Current.relation.MaxRank;
		case "scratch_check":
			return EMono.game.dateScratch > EMono.game.world.date.GetRaw();
		case "sister_money":
			return EMono.pc.GetCurrency() >= 10000;
		case "fiamaPet":
			if (EMono.pc.homeBranch != null)
			{
				foreach (Chara member in EMono.pc.homeBranch.members)
				{
					if (member.isDead && member.CanRevive() && member.GetInt(100) != 0)
					{
						return true;
					}
				}
			}
			return false;
		case "hasMelilithCurse":
			return EMono.pc.HasElement(1206);
		case "merchant":
			return Guild.Current == Guild.Merchant;
		case "fighter":
			return Guild.Current == Guild.Fighter;
		case "thief":
			return Guild.Current == Guild.Thief;
		case "mage":
			return Guild.Current == Guild.Mage;
		case "hasDLC":
			return Steam.HasDLC(array[1].ToEnum<ID_DLC>());
		case "hasFlag":
			return EMono.player.dialogFlags.TryGetValue(array[1], 0) != 0;
		case "!hasFlag":
			return EMono.player.dialogFlags.TryGetValue(array[1], 0) == 0;
		case "hasItem":
			return EMono.pc.things.Find(array[1]) != null;
		case "nasuDeliver":
			if (EMono.game.quests.GetPhase<QuestNasu>() == 2)
			{
				return EMono.pc.things.Find("backpack_holding") != null;
			}
			return false;
		case "isCompleted":
			return EMono.game.quests.IsCompleted(array[1]);
		case "costRecruit":
			if (EMono.Branch != null && EMono.Branch.IsRecruit(tg.chara))
			{
				return EMono.pc.GetCurrency("money2") >= CalcGold.Hire(tg.chara);
			}
			return true;
		case "costRecruitTicket":
			if (EMono.Branch != null && EMono.Branch.IsRecruit(tg.chara))
			{
				return EMono.pc.things.Find("ticket_resident") != null;
			}
			return false;
		case "letterTrial":
			if (Guild.CurrentQuest is QuestGuildMage && Guild.CurrentQuest.phase == 0)
			{
				return EMono.pc.things.Find("letter_trial") != null;
			}
			return false;
		default:
			if (array.Length > 2)
			{
				int value;
				bool flag = EMono.player.dialogFlags.TryGetValue(array[1], out value);
				int num = 0;
				if (array[1].StartsWith("*"))
				{
					string text = array[1].TrimStart('*');
					Quest quest = EMono.game.quests.Get(text);
					if (text == "guild")
					{
						quest = Guild.CurrentQuest;
					}
					flag = quest != null;
					if (!EMono.game.quests.completedIDs.Contains(text))
					{
						value = quest?.phase ?? (-1);
					}
					else
					{
						value = 999;
						flag = true;
					}
					num = ((!int.TryParse(array[2], out var result)) ? ((quest != null) ? array[2].ToInt(quest.GetType()) : 0) : result);
				}
				else
				{
					num = int.Parse(array[2]);
				}
				switch (array[0])
				{
				case "=":
					if (!flag && num == -1)
					{
						return true;
					}
					if (flag && value == num)
					{
						return true;
					}
					break;
				case "!":
					if (!flag || value != num)
					{
						return true;
					}
					break;
				case ">":
					if (flag && value > num)
					{
						return true;
					}
					break;
				case ">=":
					if (flag && value >= num)
					{
						return true;
					}
					break;
				case "<":
					if (!flag || value < num)
					{
						return true;
					}
					break;
				case "<=":
					if (!flag || value <= num)
					{
						return true;
					}
					break;
				}
				return false;
			}
			return setup.tag == IF;
		}
	}

	public void Show()
	{
		SE.PopDrama();
		layer.SetActive(enable: true);
		layer.cg.alpha = 0f;
		layer.cg.DOFade(1f, 0.3f);
	}

	public void Hide()
	{
		layer.SetActive(enable: false);
	}
}
