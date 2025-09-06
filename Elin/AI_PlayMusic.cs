using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AI_PlayMusic : AIAct
{
	public static bool keepPlaying;

	public static Thing playingTool;

	public Thing tool;

	public static bool ignoreDamage;

	public PlayingSong playing;

	public int score;

	public int gold;

	public int toolLv;

	public override bool ShowProgress => false;

	public override bool CancelWhenDamaged => !ignoreDamage;

	public override TargetType TargetType
	{
		get
		{
			if (tool != null && tool != EClass.pc.Tool)
			{
				return TargetType.Any;
			}
			return TargetType.Self;
		}
	}

	public static void CancelKeepPlaying()
	{
		if (keepPlaying)
		{
			SE.CancelAction();
			keepPlaying = false;
			playingTool = null;
		}
	}

	public override IEnumerable<Status> Run()
	{
		if (tool == null)
		{
			tool = (owner.IsPC ? owner.Tool : owner.things.Find<TraitToolMusic>());
		}
		if (tool == null && !owner.IsPCFaction && EClass.rnd(20) == 0)
		{
			tool = owner.AddThing(ThingGen.Create("lute"));
		}
		if (tool == null)
		{
			yield return Cancel();
		}
		if (tool.parent is Zone)
		{
			yield return DoGoto(tool.pos, tool.pos.IsBlocked ? 1 : 0);
		}
		if (owner.IsPC && EClass.game.config.preference.keepPlayingMusic)
		{
			keepPlaying = true;
			playingTool = tool;
			EInput.Consume(consumeAxis: true, 10);
		}
		toolLv = 1;
		string idSong = null;
		KnownSong song = null;
		playing = null;
		score = 40;
		gold = 0;
		if (owner.IsPC && EClass.player.playingSong != null && EClass.player.playingSong.idTool == tool.id)
		{
			playing = EClass.player.playingSong;
			idSong = EClass.player.playingSong.id;
			song = EClass.player.knownSongs[EClass.player.playingSong.id];
		}
		if (playing == null)
		{
			switch (tool.id)
			{
			case "trumpet":
				idSong = "trumpet_practice";
				break;
			case "piano_gould":
				idSong = "piano_gould";
				break;
			case "piano_killkill":
			case "piano2":
				idSong = "piano_neko";
				break;
			case "piano":
				idSong = "piano_kanon";
				break;
			case "harpsichord":
				idSong = "harpsichord_goldberg";
				break;
			case "guitar_ash":
				idSong = "guitar_caccini";
				break;
			case "guitar_efrond":
				idSong = "guitar_dusk";
				break;
			case "guitar":
				idSong = "guitar_air";
				break;
			case "harp":
				idSong = "harp_komori";
				break;
			case "shield_lute":
			case "lute":
				idSong = "guitar_sad";
				break;
			case "recorder":
				idSong = "recorder";
				break;
			case "flute":
				idSong = "flute";
				break;
			case "taiko":
				idSong = "taiko";
				break;
			case "mokugyo":
				idSong = "mokugyo";
				break;
			case "tambourine":
				idSong = "tambourine";
				break;
			case "mic":
				idSong = "mic_rachmaninoff";
				break;
			case "cello":
				idSong = "cello_prelude";
				break;
			case "instrument_violin":
			case "panty":
				idSong = "violin_chaconne";
				break;
			case "stradivarius":
				idSong = "violin_furusato";
				break;
			default:
				idSong = "violin_chaconne";
				break;
			}
			if (owner.IsPC && EClass.player.knownSongs.ContainsKey(idSong))
			{
				song = EClass.player.knownSongs[idSong];
			}
			if (song == null)
			{
				song = new KnownSong();
				if (owner.IsPC)
				{
					EClass.player.knownSongs[idSong] = song;
				}
			}
			playing = new PlayingSong
			{
				id = idSong,
				idTool = tool.id
			};
			if (owner.IsPC)
			{
				EClass.player.playingSong = playing;
			}
		}
		if (owner.IsPC)
		{
			(EClass.Sound.GetData("Instrument/" + idSong) as BGMData).song.index = playing.index;
		}
		if (Application.isEditor && owner.IsPC)
		{
			song.lv += 10;
			Debug.Log(song.lv);
		}
		List<Chara> reacted = new List<Chara>();
		Progress_Custom seq = new Progress_Custom
		{
			maxProgress = 15,
			cancelWhenMoved = false,
			showProgress = false,
			canProgress = () => tool != null && !tool.isDestroyed,
			onProgressBegin = delegate
			{
				owner.Say("music_start", owner, tool);
				owner.ShowEmo(Emo.happy, 3f, skipSame: false);
				if (tool != null && tool.ExistsOnMap)
				{
					owner.LookAt(tool);
				}
				owner.PlayAnime(AnimeID.PlayMusic);
				if (owner.IsPC)
				{
					bool flag = false;
					if (playing.mistakes == 0 && (bool)playing.source && playing.source.isPlaying && playing.source.data.name == idSong)
					{
						SoundSource soundSource = playing.source;
						if (Mathf.Abs(soundSource.source.time - (soundSource.data as BGMData).song.parts[playing.index].start) < 2f && playing.source.source.volume >= 0.1f)
						{
							soundSource.KeepPlay();
							flag = true;
							Debug.Log("keep:" + soundSource.source.time);
						}
						else
						{
							EClass.Sound.Stop(soundSource.data, 0.1f);
						}
						Debug.Log(playing.source);
					}
					if (!flag)
					{
						playing.source = owner.PlaySound("Instrument/" + idSong);
						Debug.Log(playing.source);
					}
					playing.bgm = playing.source.data as BGMData;
					BGMData.Part part = playing.bgm.song.parts[playing.index];
					if (!UISong.Instance)
					{
						Util.Instantiate<UISong>("UI/Util/UISong", EClass.ui.rectDynamicEssential);
					}
					UISong.Instance.SetSong(playing.source, playing.bgm, part);
					if (EClass.Sound.currentBGM != null)
					{
						float num3 = 0.5f - 0.1f * (float)playing.index;
						if (num3 < 0f)
						{
							num3 = 0f;
						}
						if (EClass.Sound.sourceBGM.volume > EClass.Sound.currentBGM.data.volume * num3)
						{
							EClass.Sound.sourceBGM.DOFade(EClass.Sound.currentBGM.data.volume * num3, 3f);
						}
						SoundManager.jingleTimer = part.duration / playing.bgm.pitch + playing.bgm.song.fadeIn + 2f;
					}
				}
			},
			onProgress = delegate(Progress_Custom p)
			{
				Msg.SetColor(Msg.colors.Ono);
				owner.Say(Lang.GetList("music").RandomItem());
				Msg.SetColor();
				if (tool.id == "tambourine")
				{
					Msg.Say("tambourine");
				}
				if (EClass.debug.enable && EClass.pc.job.id == "pianist")
				{
					song.lv = 10000;
				}
				if (p.progress > 2 && (EClass.rnd(100 + 50 * owner.Evalue(1405)) == 0 || EClass.rnd(4 + (int)MathF.Max(0f, song.lv - playing.index * 25 - playing.index * playing.index / 2)) == 0))
				{
					playing.mistakes++;
					if (EClass.rnd(2) == 0)
					{
						LevelSong(2 + 2 * EClass.rnd(owner.Evalue(1405) + 1));
					}
					if (playing.mistakes >= 10)
					{
						playing.index = 0;
					}
					Cancel();
				}
				else
				{
					foreach (Chara item in EClass._map.ListCharasInCircle(owner.pos, 4f))
					{
						if (item.conSleep != null && item.ResistLv(957) <= 0)
						{
							item.conSleep.Kill();
							item.ShowEmo(Emo.angry);
						}
					}
					List<Chara> list = owner.pos.ListWitnesses(owner, 4, WitnessType.music);
					int num = owner.Evalue(241) * (100 + toolLv) / 100;
					int num2 = 0;
					foreach (Chara item2 in list)
					{
						if (owner == null)
						{
							break;
						}
						if (!reacted.Contains(item2) && EClass.rnd(5) == 0)
						{
							if (owner.IsPCParty)
							{
								if (item2.interest <= 0 || (EClass._zone is Zone_Music && (item2.IsPCFaction || item2.IsPCFactionMinion)))
								{
									continue;
								}
								item2.interest -= EClass.rnd(10);
								if (item2.interest < 0)
								{
									item2.Talk("musicBored");
									continue;
								}
							}
							if (EClass.rnd(num2 * num2) <= 30 && item2.pos.FirstChara == item2)
							{
								bool isMinion = item2.IsMinion;
								if (num < item2.LV && EClass.rnd(2) == 0)
								{
									reacted.Add(item2);
									if (!isMinion)
									{
										score -= item2.LV / 2 - 10;
									}
									if (EClass.rnd(2) == 0)
									{
										item2.Talk("musicBad");
									}
									else
									{
										item2.Say("musicBad", item2, owner);
									}
									item2.ShowEmo(Emo.sad);
									owner.elements.ModExp(241, 10f);
									if (EClass.rnd(5) == 0)
									{
										ThrowReward(item2, punish: true);
									}
									num2++;
								}
								else if (EClass.rnd(num + 5) > EClass.rnd(item2.LV * 5 + 1))
								{
									reacted.Add(item2);
									if (!isMinion)
									{
										score += EClass.rnd(item2.LV / 2 + 5) + 5;
									}
									if (EClass.rnd(2) == 0)
									{
										item2.Talk("musicGood");
									}
									else
									{
										item2.Say("musicGood", item2, owner);
									}
									item2.ShowEmo(Emo.happy);
									item2.renderer.PlayAnime((EClass.rnd(2) == 0) ? AnimeID.Jump : AnimeID.Fishing);
									owner.elements.ModExp(241, EClass._zone.IsUserZone ? 10 : 50);
									if (!isMinion)
									{
										ThrowReward(item2, punish: false);
									}
									num2++;
								}
							}
						}
					}
					if (owner != null && owner.IsPC && EClass.rnd(80) < num2)
					{
						owner.stamina.Mod(-1);
					}
				}
			},
			onProgressComplete = delegate
			{
				if (EClass.rnd(2) == 0)
				{
					LevelSong(2);
				}
				if ((bool)playing.bgm)
				{
					playing.index++;
					playing.mistakes = 0;
					if (playing.index >= playing.bgm.song.parts.Count)
					{
						playing.index = 0;
					}
					playing.bgm.song.index = playing.index;
				}
				Evaluate(success: true);
			}
		}.SetDuration(26);
		yield return Do(seq);
		void LevelSong(int a)
		{
			if (a > 0)
			{
				song.lv += a + EClass.rnd(2);
				if (owner == EClass.pc)
				{
					Msg.Say("level_song");
				}
			}
		}
	}

	public void Evaluate(bool success)
	{
		if (owner == null || !owner.IsPC)
		{
			return;
		}
		if (success)
		{
			score = score * 110 / 100;
		}
		else
		{
			score = score / 2 - 20;
		}
		int num = Mathf.Clamp(score / 20 + 1, 0, 9);
		owner.Say(Lang.GetList("music_result")[num]);
		if (gold > 0)
		{
			owner.Say("music_reward", owner, gold.ToString() ?? "");
		}
		if (EClass.rnd(3) != 0)
		{
			owner.stamina.Mod(-1);
		}
		QuestMusic questMusic = EClass.game.quests.Get<QuestMusic>();
		if (questMusic != null)
		{
			questMusic.score += score;
			questMusic.sumMoney += gold;
			int num2 = num / 2 - 1;
			if (num > 0)
			{
				SE.Play("clap" + num2);
			}
		}
	}

	public void ThrowReward(Chara c, bool punish)
	{
		Thing thing = null;
		string text = "";
		int num = 1;
		if (punish)
		{
			text = ((EClass.rnd(5) == 0) ? "rock" : "pebble");
			if (EClass.rnd(8) == 0)
			{
				text = ((EClass.rnd(3) == 0) ? "water_dirty" : "water");
			}
			if (!c.IsPCFactionOrMinion)
			{
				thing = c.TryGetThrowable()?.Split(1);
			}
		}
		else if (EClass.rnd(100) == 0 && !EClass._zone.IsUserZone)
		{
			text = "ecopo";
			if (EClass.rnd(4) == 0)
			{
				text = "gacha_coin";
			}
			if (EClass.rnd(4) == 0)
			{
				text = "plat";
			}
			if (EClass.rnd(3) == 0)
			{
				text = "tomato";
			}
			if (EClass.rnd(3) == 0)
			{
				text = "casino_coin";
			}
			if (owner.IsPC && !EClass._zone.IsInstance)
			{
				if (c.LV >= 20 && EClass.rnd(10 * (int)Mathf.Pow(2f, EClass.player.flags.reward_killkill + 1)) == 0)
				{
					text = "piano_killkill";
					punish = true;
					EClass.player.flags.reward_killkill++;
				}
				if (c.LV >= 40 && EClass.rnd(10 * (int)Mathf.Pow(2f, EClass.player.flags.reward_gould + 1)) == 0)
				{
					text = "piano_gould";
					punish = true;
					EClass.player.flags.reward_gould++;
				}
			}
		}
		else
		{
			num = (EClass.rnd(c.LV * 2 + 1) + 1) * (100 + toolLv * 2 + owner.Evalue(1405) * 10) / 100;
			if (c.IsUnique)
			{
				num *= 2;
			}
			if (!(EClass._zone is Zone_Music))
			{
				if (num > 25)
				{
					num /= 2;
				}
				if (num > 50)
				{
					num /= 2;
				}
				if (num > 100)
				{
					num /= 2;
				}
				if (EClass._zone.IsUserZone)
				{
					num /= 5;
				}
			}
			if (num < 1)
			{
				num = 1;
			}
			gold += num;
			text = "money";
		}
		if (!owner.IsPCParty && !punish && text != "money")
		{
			return;
		}
		if (thing == null)
		{
			thing = ThingGen.Create(text).SetNum(num);
		}
		ignoreDamage = true;
		ActThrow.Throw(c, owner.pos, thing, (!punish) ? ThrowMethod.Reward : ThrowMethod.Punish);
		ignoreDamage = false;
		if (owner == null || !thing.ExistsOnMap)
		{
			return;
		}
		if (!owner.IsPC && owner.things.IsFull())
		{
			thing.Destroy();
		}
		else
		{
			if (!owner.IsAliveInCurrentZone)
			{
				return;
			}
			owner.Pick(thing);
			if (thing.id == "money" && !owner.IsPC)
			{
				int num2 = (owner.Evalue(241) * 10 + 100) / ((owner.IsPCFaction && owner.memberType == FactionMemberType.Default) ? 1 : 10);
				if (owner.GetCurrency() - num2 > 0)
				{
					owner.c_allowance += num;
					owner.ModCurrency(-num);
				}
			}
		}
	}

	public override void OnCancel()
	{
		if (playing != null && (bool)playing.bgm)
		{
			playing.bgm.song.Fail(playing.source, playing.bgm);
			if ((bool)UISong.Instance)
			{
				UISong.Instance.Kill();
			}
			playing.source = null;
			SoundManager.jingleTimer = 1f;
		}
		Evaluate(success: false);
	}
}
