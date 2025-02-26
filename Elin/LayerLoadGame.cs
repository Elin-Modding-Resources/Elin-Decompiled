using System;
using System.Collections.Generic;
using System.IO;
using Ionic.Zip;
using UnityEngine;

public class LayerLoadGame : ELayer
{
	public UIList list;

	public GameObject goInfo;

	public GameObject goNoInfo;

	public GameObject goInfo2;

	public GameObject goCloudWarn;

	public UINote note;

	public UIButton buttonLoad;

	public UIButton buttonDelete;

	public UIButton buttonBackup;

	public UIButton buttonOpen;

	public UIButton buttonListBackup;

	public UIButton buttonMove;

	public List<GameIndex> worlds;

	public Portrait portrait;

	public UIText textAka;

	public UIText textName;

	public UIText textInfo;

	public UIText textRace;

	public UIText textJob;

	public UIText textDeepest;

	public UIText textDays;

	private string pathRoot;

	private string idDest;

	private string pathBackup;

	private bool backup;

	private bool cloud => ELayer.core.config.cloud;

	public void Init(bool _backup, string _pathBackup = "", string _idDest = "")
	{
		backup = _backup;
		pathBackup = _pathBackup;
		idDest = _idDest;
		if (backup)
		{
			for (int i = 0; i < 2; i++)
			{
				Vector2 anchoredPosition = windows[i].Rect().anchoredPosition;
				windows[i].Rect().anchoredPosition = new Vector2(anchoredPosition.x + 40f, anchoredPosition.y - 30f);
			}
		}
		if (!backup && Application.isEditor)
		{
			FileInfo[] files = new DirectoryInfo("D:\\Download").GetFiles();
			foreach (FileInfo fileInfo in files)
			{
				if (fileInfo.Name.StartsWith("world") && fileInfo.Name.EndsWith(".zip"))
				{
					string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileInfo.Name);
					string path = CorePath.RootSave + fileNameWithoutExtension;
					if (!Directory.Exists(path))
					{
						Directory.CreateDirectory(path);
						new StreamWriter(new MemoryStream());
						ReadOptions options = new ReadOptions();
						ZipFile.Read(fileInfo.FullName, options).ExtractAll(path);
					}
				}
			}
		}
		RefreshList();
		if (!backup)
		{
			windows[0].ClearBottomButtons();
			windows[0].AddBottomButton("toggleCloud", delegate
			{
				ELayer.core.config.cloud = !ELayer.core.config.cloud;
				SE.Tab();
				RefreshList();
			});
		}
	}

	public void RefreshList()
	{
		if (!backup)
		{
			windows[0].SetCaption("saveList".lang() + (cloud ? (" " + "isCloud".lang()) : ""));
		}
		pathRoot = (backup ? pathBackup : (cloud ? CorePath.RootSaveCloud : CorePath.RootSave));
		worlds = GameIO.GetGameList(pathRoot, backup, !backup && !cloud);
		goCloudWarn.SetActive(cloud && !backup);
		goInfo.SetActive(value: false);
		goNoInfo.SetActive(value: true);
		list.Clear();
		list.callbacks = new UIList.Callback<GameIndex, UIButton>
		{
			onInstantiate = delegate(GameIndex a, UIButton b)
			{
				FontColor c = (ELayer.core.version.IsSaveCompatible(a.version) ? FontColor.Good : FontColor.Bad);
				string s = a.Title + ((ELayer.core.IsGameStarted && a.id == Game.id) ? "currentSave".lang() : "") + Environment.NewLine;
				b.mainText.SetText(s, c);
				b.subText.SetText(a.RealDate ?? "");
				b.subText2.SetText((a.permaDeath ? "★" : "") + a.pcName + " (" + a.zoneName + ")", c);
				b.GetComponent<UIItem>().text1.SetText(a.version.GetText() + (cloud ? (" " + "isCloud".lang()) : ""));
			},
			onClick = delegate(GameIndex a, UIButton b)
			{
				RefreshInfo(a);
			}
		};
		foreach (GameIndex world in worlds)
		{
			list.Add(world);
		}
		list.Refresh();
		if (list.items.Count == 0)
		{
			buttonLoad.SetActive(enable: false);
			buttonDelete.SetActive(enable: false);
			goNoInfo.SetActive(value: true);
		}
	}

	public void RefreshInfo(GameIndex i)
	{
		goInfo.SetActive(value: true);
		goNoInfo.SetActive(value: false);
		note.Clear();
		note.AddTopic("version".lang(), i.version.GetText());
		note.AddTopic("date_real".lang(), i.RealDate);
		note.AddTopic("date_game".lang(), i.GameData);
		note.AddTopic("ID", i.id);
		bool flag = ELayer.core.version.IsSaveCompatible(i.version) && !i.IsCorrupted;
		goInfo2.SetActive(flag);
		if (i.IsCorrupted)
		{
			note.Space();
			note.AddText("corrupted_folder".lang(), FontColor.Bad);
		}
		else if (!flag)
		{
			note.Space();
			note.AddText("incompatible".lang(), FontColor.Bad);
		}
		else
		{
			note.AddTopic("currentZone".lang(), i.zoneName);
		}
		textAka.SetText(i.aka);
		textName.SetText(i.pcName);
		textDays.SetText("infoHire".lang(i.days.ToString() ?? ""));
		textDeepest.SetText("deepestLv2".lang(i.deepest.ToString() ?? ""));
		textRace.SetText(ELayer.sources.races.map.TryGetValue(i.idRace)?.GetName().ToTitleCase() ?? "");
		textJob.SetText(ELayer.sources.jobs.map.TryGetValue(i.idJob)?.GetName().ToTitleCase() ?? "");
		if (!i.idPortrait.IsEmpty())
		{
			portrait.SetActive(enable: true);
			Color color = Color.white;
			ColorUtility.TryParseHtmlString("#" + i.color, out color);
			portrait.SetPortrait(i.idPortrait, color);
		}
		else
		{
			portrait.SetActive(enable: false);
		}
		note.Build();
		buttonListBackup.SetActive(!backup);
		buttonDelete.SetActive(!backup && !ELayer.core.IsGameStarted);
		buttonBackup.SetActive(!backup && (!ELayer.core.IsGameStarted || i.id == Game.id));
		buttonOpen.SetActive(backup);
		buttonMove.SetActive(!backup && !ELayer.core.IsGameStarted);
		buttonMove.mainText.SetText((cloud ? "fromCloud" : "toCloud").lang());
		buttonMove.SetOnClick(delegate
		{
			Dialog.YesNo("dialog_switchCloud", delegate
			{
				string sourceDirName = (cloud ? CorePath.RootSaveCloud : CorePath.RootSave) + i.id;
				string text = (cloud ? CorePath.RootSave : CorePath.RootSaveCloud) + i.id;
				string text2 = (cloud ? CorePath.PathBackupCloud : CorePath.PathBackup) + i.id;
				string text3 = (cloud ? CorePath.PathBackup : CorePath.PathBackupCloud) + i.id;
				bool flag2 = Directory.Exists(text2);
				if (Directory.Exists(text) || Directory.Exists(text3))
				{
					SE.Beep();
					ELayer.ui.Say("cloud_conflict");
				}
				else
				{
					SE.Play("mutation");
					try
					{
						if (flag2)
						{
							Debug.Log("Converting Backup files:");
							Directory.Move(text2, text3);
							foreach (GameIndex game in GameIO.GetGameList(((!cloud) ? CorePath.PathBackupCloud : CorePath.PathBackup) + i.id + "/"))
							{
								Debug.Log("Processing:" + game.id + ": " + game.path);
								if (!cloud)
								{
									GameIO.PrepareSteamCloud(game.id, game.path);
								}
							}
						}
						Debug.Log("Converting Current World:");
						Directory.Move(sourceDirName, text);
						i.path = text;
						i.cloud = !cloud;
						GameIO.UpdateGameIndex(i);
						if (i.cloud)
						{
							GameIO.PrepareSteamCloud(i.id);
						}
					}
					catch (Exception ex)
					{
						ELayer.ui.Say(ex.Message);
					}
					RefreshList();
				}
			});
		});
		buttonLoad.onClick.RemoveAllListeners();
		buttonDelete.onClick.RemoveAllListeners();
		buttonLoad.SetOnClick(delegate
		{
			LayerTitle.KillActor();
			if (backup)
			{
				Dialog.YesNo("dialog_restoreWarning", delegate
				{
					GameIO.DeleteGame(idDest, cloud, deleteBackup: false);
					IO.CopyDir(pathRoot + "/" + i.id, (cloud ? CorePath.RootSaveCloud : CorePath.RootSave) + "/" + idDest);
					SE.WriteJournal();
					Close();
					Game.TryLoad(idDest, cloud, delegate
					{
						Game.Load(idDest, cloud);
					});
				});
			}
			else
			{
				if (!i.madeBackup && i.version.IsBelow(ELayer.core.version))
				{
					GameIO.MakeBackup(i);
					ELayer.ui.Say("backupDone");
					i.madeBackup = true;
					GameIO.UpdateGameIndex(i);
				}
				Game.TryLoad(i.id, cloud, delegate
				{
					Game.Load(i.id, cloud);
				});
			}
		});
		buttonDelete.SetOnClick(delegate
		{
			Dialog.YesNo("dialogDeleteGame", delegate
			{
				GameIO.DeleteGame(i.id, cloud);
				worlds = null;
				RefreshList();
				SE.Trash();
			});
		});
		buttonListBackup.SetOnClick(delegate
		{
			ELayer.ui.AddLayer<LayerLoadGame>().Init(_backup: true, (cloud ? CorePath.PathBackupCloud : CorePath.PathBackup) + i.id + "/", i.id);
		});
		buttonBackup.SetOnClick(delegate
		{
			if (ELayer.core.IsGameStarted)
			{
				ELayer.game.backupTime = 0.0;
				ELayer.game.Save();
			}
			i.cloud = cloud;
			GameIO.MakeBackup(i);
			ELayer.ui.Say("backupDone");
			SE.WriteJournal();
		});
		buttonOpen.SetOnClick(delegate
		{
			Util.ShowExplorer(i.path);
		});
		buttonLoad.SetActive(flag);
	}
}
