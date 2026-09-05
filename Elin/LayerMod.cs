using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HeathenEngineering.SteamworksIntegration;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LayerMod : ELayer
{
	private sealed class PreviewEntry
	{
		public Sprite sprite;

		public bool ownsTexture;

		public ModPreview.PreviewType kind;
	}

	private sealed class HoverRelay : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler
	{
		public Action onEnter;

		public void OnPointerEnter(PointerEventData eventData)
		{
			onEnter?.Invoke();
		}
	}

	private const float previewBoxHeight = 240f;

	private const string urlWorkshopItem = "https://steamcommunity.com/sharedfiles/filedetails/?id=";

	public static LayerMod Instance;

	public UIList list;

	public UIList list2;

	public UIText textRestart;

	public UIText textNoResult;

	public UIButton toggleDisableMods;

	public UIButton buttonClearSearch;

	public InputField inputSearch;

	public UIScrollView panelScroll;

	public UIHeader panelHeader;

	public UINote panelNote;

	private UIButton buttonFilter;

	private BaseModPackage panelTarget;

	private float timerSearch;

	private bool wasSearchFocused;

	private string lastSearch = "";

	private string[] searchTerms = Array.Empty<string>();

	private ModSearch.Filter filterMode;

	private readonly Dictionary<BaseModPackage, string> searchCache = new Dictionary<BaseModPackage, string>();

	private readonly Dictionary<BaseModPackage, PreviewEntry> previews = new Dictionary<BaseModPackage, PreviewEntry>();

	public ModManager manager => ELayer.core.mods;

	public bool HasFilter
	{
		get
		{
			if (searchTerms.Length == 0)
			{
				return filterMode != ModSearch.Filter.All;
			}
			return true;
		}
	}

	private void Move(BaseModPackage p, int a)
	{
		List<object> items = list.items;
		int num = items.IndexOf(p);
		if (num < 0 || num + a < 0 || num + a >= items.Count)
		{
			SE.BeepSmall();
		}
		else
		{
			MoveTo(p, items[num + a] as BaseModPackage);
		}
	}

	private void MoveTo(BaseModPackage p, BaseModPackage target)
	{
		List<BaseModPackage> packages = manager.packages;
		int num = packages.IndexOf(p);
		int num2 = ((target == null) ? (-1) : packages.IndexOf(target));
		if (num < 0 || num2 < 0 || num == num2 || target.builtin)
		{
			SE.BeepSmall();
			return;
		}
		packages.Move(p, num2 - num);
		SE.Tab();
		textRestart.SetActive(enable: true);
		ELayer.core.mods.SaveLoadOrder();
		RefreshLists();
	}

	public void RefreshLists()
	{
		list.List();
		list2.List();
		if ((bool)textNoResult)
		{
			textNoResult.SetActive(HasFilter && list.items.Count == 0);
		}
	}

	public override void OnInit()
	{
		textRestart.SetActive(enable: false);
		toggleDisableMods.SetToggle(ELayer.config.other.disableMods, delegate(bool on)
		{
			ELayer.config.other.disableMods = on;
			ELayer.config.Save();
			textRestart.SetActive(enable: true);
		});
		Instance = this;
		foreach (BaseModPackage package in manager.packages)
		{
			package.UpdateMeta(updateOnly: true);
			searchCache[package] = ModSearch.BuildText(package);
		}
		list.dragScrollView = GetComponentInChildren<UIScrollView>();
		list.dragViewport = windows[0].Rect();
		list.callbacks = CreateCallbacks(list, builtin: false);
		list2.callbacks = CreateCallbacks(list2, builtin: true);
		InitSearchUI();
		CreatePresetUI();
		panelHeader.SetText("info".lang());
		RefreshLists();
		ShowInfo(FirstInfoTarget());
		list.dragEdgeSize = list.callbacks.GetMold()?.Rect().sizeDelta.y ?? 34f;
	}

	private UIList.Callback<ModPackage, ItemMod> CreateCallbacks(UIList target, bool builtin)
	{
		return new UIList.Callback<ModPackage, ItemMod>
		{
			onClick = delegate
			{
			},
			onInstantiate = delegate(ModPackage a, ItemMod b)
			{
				b.package = a;
				string s = ELayer.core.mods.packages.IndexOf(a) + 1 + ". " + (a.isInPackages ? "[Local] " : "") + a.title.IsEmpty(a.dirInfo.Name);
				b.buttonActivate.mainText.SetText(s, (!a.IsValidVersion() || a.blockedBy != null || !a.langDepError.IsEmpty()) ? FontColor.Bad : (a.activated ? FontColor.ButtonGeneral : FontColor.Passive));
				b.buttonActivate.subText.text = a.version;
				b.buttonLock.mainText.text = a.author;
				b.buttonUp.SetActive(!a.builtin);
				b.buttonDown.SetActive(!a.builtin);
				b.buttonToggle.SetToggle(a.willActivate);
				b.buttonUp.SetOnClick(delegate
				{
					Move(a, -1);
				});
				b.buttonDown.SetOnClick(delegate
				{
					Move(a, 1);
				});
				(b.buttonActivate.GetComponent<HoverRelay>() ?? b.buttonActivate.gameObject.AddComponent<HoverRelay>()).onEnter = delegate
				{
					ShowInfo(a);
				};
				UIButton bt = b.buttonToggle;
				bt.SetOnClick(delegate
				{
					a.willActivate = !a.willActivate;
					bt.SetToggle(a.willActivate);
					ELayer.core.mods.SaveLoadOrder();
					textRestart.SetActive(enable: true);
				});
				bt.interactable = !a.builtin;
				b.buttonActivate.onClick.AddListener(delegate
				{
					Refresh();
					UIContextMenu uIContextMenu = ELayer.ui.CreateContextMenuInteraction();
					if (!a.builtin)
					{
						if (ELayer.debug.enable || (!BaseCore.IsOffline && a.isInPackages && !ELayer.core.version.demo))
						{
							uIContextMenu.AddButton("mod_publish", delegate
							{
								Core.TryWarnUpload(delegate
								{
									Dialog.YesNo("mod_publish_warn".lang(a.title, a.id, a.author), delegate
									{
										ELayer.core.steam.CreateUserContent(a);
									});
								});
							});
						}
						uIContextMenu.AddButton(a.willActivate ? "mod_deactivate" : "mod_activate", delegate
						{
							SE.Click();
							a.willActivate = !a.willActivate;
							ELayer.core.mods.SaveLoadOrder();
							RefreshLists();
							textRestart.SetActive(enable: true);
						});
						if (!a.isInPackages && !a.workshopId.IsEmpty())
						{
							uIContextMenu.AddButton("mod_convert_local", delegate
							{
								SE.Click();
								string path = ("Mod_" + a.workshopId + "_" + a.id).SanitizeDirectoryName();
								string text = Path.Combine(BaseModManager.rootMod, path);
								a.CopyContentTo(text);
								ModPackage modPackage = manager.AddPackage(new DirectoryInfo(text), isInPackages: true);
								modPackage.UpdateMeta(updateOnly: true);
								searchCache[modPackage] = ModSearch.BuildText(modPackage);
								manager.packages.Move(modPackage, manager.packages.IndexOf(a) - manager.packages.Count + 2);
								modPackage.willActivate = false;
								modPackage.activated = false;
								ELayer.core.mods.SaveLoadOrder();
								RefreshLists();
								textRestart.SetActive(enable: true);
							});
						}
						if (a.isInPackages && a.IsSourceLocalizable)
						{
							uIContextMenu.AddButton("mod_export_text", delegate
							{
								SE.Click();
								string text = a.UpdateSourceLocalizationFile(Lang.langCode, force: true);
								ELayer.ui.Say(text);
							});
						}
					}
					if (ModPreview.FindPreviewFile(a) != null || (bool)SteamPreview(a))
					{
						uIContextMenu.AddButton("mod_preview", delegate
						{
							ShowPreview(a);
						});
					}
					if (!a.workshopId.IsEmpty())
					{
						uIContextMenu.AddButton("mod_info_workshop", delegate
						{
							SE.Click();
							Application.OpenURL("https://steamcommunity.com/sharedfiles/filedetails/?id=" + a.workshopId.Trim());
						});
					}
					uIContextMenu.AddButton("mod_info", delegate
					{
						SE.Click();
						if (!a.dirInfo.Exists)
						{
							SE.BeepSmall();
							ELayer.ui.Say(a.dirInfo.FullName);
						}
						else
						{
							string text = a.dirInfo.FullName + "/package.xml";
							Util.ShowExplorer(File.Exists(text) ? text : a.dirInfo.FullName);
						}
					});
					uIContextMenu.Show();
				});
				b.buttonLock.onClick.AddListener(Refresh);
			},
			onList = delegate
			{
				foreach (BaseModPackage package in manager.packages)
				{
					if (package.builtin == builtin && (builtin || Match(package)))
					{
						target.Add(package);
					}
				}
			},
			onRefresh = Refresh,
			onDragReorder = delegate(ModPackage p, int a)
			{
				BaseCore.Instance.FreezeScreen(0.1f);
				List<object> items = target.items;
				int num = items.IndexOf(p);
				MoveTo(p, items[Mathf.Clamp(num + a, 0, items.Count - 1)] as BaseModPackage);
			},
			canDragReorder = (ModPackage p) => !p.builtin
		};
	}

	private bool Match(BaseModPackage p)
	{
		if (!searchCache.TryGetValue(p, out var value))
		{
			value = (searchCache[p] = ModSearch.BuildText(p));
		}
		return ModSearch.Match(p, value, searchTerms, filterMode);
	}

	private void InitSearchUI()
	{
		if ((bool)inputSearch)
		{
			inputSearch.SetTextWithoutNotify("");
			if (inputSearch.placeholder is Text text)
			{
				text.text = "mod_search".lang();
			}
			inputSearch.onValueChanged.AddListener(Search);
			inputSearch.onSubmit.AddListener(Search);
		}
		if ((bool)buttonClearSearch)
		{
			buttonClearSearch.SetOnClick(ClearSearchText);
			buttonClearSearch.SetActive(enable: false);
		}
		buttonFilter = windows[0].AddBottomButton("mod_filter", CycleFilter);
		RefreshFilterButton();
		if ((bool)textNoResult)
		{
			textNoResult.SetText("noResult".lang(), FontColor.Passive);
			textNoResult.SetActive(enable: false);
		}
	}

	public void Search(string s)
	{
		lastSearch = s ?? "";
		timerSearch = 0.15f;
		if ((bool)buttonClearSearch)
		{
			buttonClearSearch.SetActive(!lastSearch.IsEmpty());
		}
	}

	private void ClearSearchText()
	{
		SE.Click();
		if ((bool)inputSearch)
		{
			inputSearch.SetTextWithoutNotify("");
		}
		Search("");
	}

	public void ClearSearch()
	{
		if ((bool)inputSearch)
		{
			inputSearch.SetTextWithoutNotify("");
		}
		if ((bool)buttonClearSearch)
		{
			buttonClearSearch.SetActive(enable: false);
		}
		lastSearch = "";
		timerSearch = 0f;
		searchTerms = Array.Empty<string>();
		filterMode = ModSearch.Filter.All;
		RefreshFilterButton();
		RefreshLists();
	}

	private void CycleFilter()
	{
		SE.Click();
		filterMode = ModSearch.Filters[(int)(filterMode + 1) % ModSearch.Filters.Length];
		RefreshFilterButton();
		RefreshLists();
	}

	private void RefreshFilterButton()
	{
		if ((bool)buttonFilter)
		{
			buttonFilter.mainText.text = "mod_filter".lang() + ": " + FilterLabel(filterMode).lang();
			windows[0].rectBottom.RebuildLayout(recursive: true);
		}
	}

	private static string FilterLabel(ModSearch.Filter f)
	{
		return f switch
		{
			ModSearch.Filter.Enabled => "mod_filter_enabled", 
			ModSearch.Filter.Disabled => "mod_filter_disabled", 
			ModSearch.Filter.Problems => "mod_filter_problem", 
			ModSearch.Filter.Local => "mod_filter_local", 
			ModSearch.Filter.Workshop => "mod_filter_workshop", 
			_ => "all", 
		};
	}

	private void LateUpdate()
	{
		if (timerSearch > 0f)
		{
			timerSearch -= Core.delta;
			if (timerSearch <= 0f)
			{
				timerSearch = 0f;
				searchTerms = ModSearch.Terms(lastSearch);
				RefreshLists();
			}
		}
		if ((bool)inputSearch)
		{
			bool isFocused = inputSearch.isFocused;
			EventSystem current = EventSystem.current;
			if (wasSearchFocused && !isFocused && (bool)current && current.currentSelectedGameObject == inputSearch.gameObject)
			{
				current.SetSelectedGameObject(null);
			}
			wasSearchFocused = isFocused;
		}
	}

	private PreviewEntry GetThumb(BaseModPackage p)
	{
		if (previews.TryGetValue(p, out var value))
		{
			return value;
		}
		value = new PreviewEntry();
		bool owns;
		Texture2D texture2D = LoadPreviewTexture(p, mipmap: true, out value.kind, out owns);
		if ((bool)texture2D)
		{
			Texture2D texture2D2 = ModPreview.Downscale(texture2D, 280, owns);
			value.sprite = texture2D2.ToSprite();
			value.ownsTexture = owns || texture2D2 != texture2D;
		}
		previews[p] = value;
		return value;
	}

	private static Texture2D LoadPreviewTexture(BaseModPackage p, bool mipmap, out ModPreview.PreviewType kind, out bool owns)
	{
		kind = ModPreview.PreviewType.None;
		owns = true;
		FileInfo fileInfo = ModPreview.FindPreviewFile(p);
		if (fileInfo != null)
		{
			try
			{
				Texture2D texture2D = ModPreview.LoadTexture(File.ReadAllBytes(fileInfo.FullName), out kind, mipmap);
				if ((bool)texture2D)
				{
					return texture2D;
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarning("#mod preview " + fileInfo.FullName + ": " + ex.Message);
			}
		}
		Texture2D texture2D2 = SteamPreview(p);
		if ((bool)texture2D2)
		{
			owns = false;
			if (kind == ModPreview.PreviewType.None)
			{
				kind = ModPreview.PreviewType.Jpeg;
			}
		}
		return texture2D2;
	}

	private static Texture2D SteamPreview(BaseModPackage p)
	{
		if (!(p.item is WorkshopItem workshopItem) || !workshopItem.previewImage || workshopItem.previewImage.width <= 2)
		{
			return null;
		}
		return workshopItem.previewImage;
	}

	private BaseModPackage FirstInfoTarget()
	{
		using (List<object>.Enumerator enumerator = list.items.GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				return enumerator.Current as BaseModPackage;
			}
		}
		if (manager.packages.Count <= 0)
		{
			return null;
		}
		return manager.packages[0];
	}

	private void ShowInfo(BaseModPackage p)
	{
		if (!panelNote || p == null || p == panelTarget)
		{
			return;
		}
		panelTarget = p;
		UINote n = panelNote;
		n.Clear();
		n.AddHeader(p.title.IsEmpty(p.dirInfo.Name));
		PreviewEntry thumb = GetThumb(p);
		if ((bool)thumb.sprite)
		{
			AddPreviewBox(n, thumb.sprite, delegate
			{
				ShowPreview(p);
			});
			if (thumb.kind == ModPreview.PreviewType.Gif)
			{
				n.AddText("mod_preview_gif".lang(), FontColor.Passive);
			}
		}
		else
		{
			n.AddText(((thumb.kind == ModPreview.PreviewType.None) ? "mod_preview_none" : "mod_preview_unsupported").lang(), FontColor.Passive);
		}
		if (!p.author.IsEmpty())
		{
			AddTopic("author", p.author);
		}
		if (!p.version.IsEmpty())
		{
			AddTopic("version", p.version);
		}
		if (!p.id.IsEmpty())
		{
			FitValue(AddTopic("mod_info_id", p.id));
		}
		string text = ModLoadOrderPreset.WorkshopId(p);
		FitValue(AddTopic(text.IsEmpty() ? "mod_filter_local" : "mod_filter_workshop", text.IsEmpty() ? p.dirInfo.Name : text));
		if (!p.builtin && !p.willActivate)
		{
			AddTopic("status", "mod_filter_disabled".lang());
		}
		if (p.blockedBy != null)
		{
			n.AddText("mod_info_blocked".lang(p.blockedBy.title.IsEmpty(p.blockedBy.id)), FontColor.Bad);
		}
		if (!p.langDepError.IsEmpty())
		{
			n.AddText(p.langDepError, FontColor.Bad);
		}
		if (p.duplicateOf != null)
		{
			n.AddText("mod_info_duplicate".lang(p.duplicateOf.dirInfo?.Name ?? p.duplicateOf.id), FontColor.Bad);
		}
		if (!p.IsValidVersion())
		{
			n.AddText("mod_info_old_version".lang(p.version, ELayer.core.versionMod.GetText()), FontColor.Bad);
		}
		if (!p.parseError.IsEmpty())
		{
			n.AddText(p.parseError, FontColor.Bad);
		}
		string[] tags = p.tags;
		if (tags != null && tags.Length > 0)
		{
			AddTopic("mod_info_tags", string.Join(", ", p.tags));
		}
		AddIdList(n, "mod_info_requires", p.dependency);
		AddIdList(n, "incompatible", p.incompatible);
		AddIdList(n, "mod_info_load_after", p.loadAfter);
		AddIdList(n, "mod_info_load_before", p.loadBefore);
		if (!p.description.IsEmpty())
		{
			n.Space(8);
			n.AddText(p.description);
		}
		n.Build();
		panelScroll.content.anchoredPosition = Vector2.zero;
		UIItem AddTopic(string lang, string value)
		{
			UIItem uIItem = n.AddTopic(lang, value);
			uIItem.text1.alignment = TextAnchor.LowerLeft;
			return uIItem;
		}
	}

	private static void AddPreviewBox(UINote n, Sprite sprite, Action onClick)
	{
		n.AddImage(sprite);
		RectTransform rectTransform = n.transform.GetChild(n.transform.childCount - 1).Rect();
		rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 240f);
		Image componentInChildren = rectTransform.GetComponentInChildren<Image>();
		RectTransform rectTransform2 = componentInChildren.rectTransform;
		rectTransform2.anchorMin = Vector2.zero;
		rectTransform2.anchorMax = Vector2.one;
		rectTransform2.pivot = new Vector2(0.5f, 0.5f);
		rectTransform2.anchoredPosition = Vector2.zero;
		rectTransform2.sizeDelta = Vector2.zero;
		componentInChildren.preserveAspect = true;
		Button button = componentInChildren.gameObject.AddComponent<Button>();
		button.transition = Selectable.Transition.None;
		button.onClick.AddListener(delegate
		{
			onClick();
		});
	}

	private static void FitValue(UIItem item)
	{
		if ((bool)item && (bool)item.text2)
		{
			item.text2.resizeTextForBestFit = true;
			item.text2.horizontalOverflow = HorizontalWrapMode.Wrap;
			item.text2.resizeTextMinSize = 9;
			item.text2.resizeTextMaxSize = item.text2.fontSize;
		}
	}

	private static void AddIdList(UINote n, string lang, string[][] rows)
	{
		if (rows != null && rows.Length != 0)
		{
			List<string> list = (from row in rows
				where row != null && row.Length > 0 && !row[0].IsEmpty()
				select row[0]).ToList();
			if (list.Count > 0)
			{
				n.AddTopic(lang, string.Join(", ", list)).text1.alignment = TextAnchor.LowerLeft;
			}
		}
	}

	private void ShowPreview(BaseModPackage a)
	{
		SE.Click();
		ModPreview.PreviewType kind;
		bool owns;
		Texture2D tex = LoadPreviewTexture(a, mipmap: false, out kind, out owns);
		if (!tex)
		{
			SE.BeepSmall();
			ELayer.ui.Say("mod_preview_unsupported");
			return;
		}
		Sprite image = tex.ToSprite();
		LayerImage layerImage = ELayer.ui.AddLayer<LayerImage>();
		layerImage.SetImage(image);
		layerImage.SetOnKill(delegate
		{
			UnityEngine.Object.Destroy(tex);
			if (owns)
			{
				UnityEngine.Object.Destroy(tex);
			}
		});
	}

	private void CreatePresetUI()
	{
		windows[0].AddBottomButton("mod_preset", ShowPresetMenu);
	}

	private void ShowPresetMenu()
	{
		SE.Click();
		if (ModManager.disableMod)
		{
			Dialog.Ok("mod_preset_disabled");
			return;
		}
		UIContextMenu uIContextMenu = ELayer.ui.CreateContextMenu();
		uIContextMenu.AddButton("mod_preset_save", SavePreset);
		uIContextMenu.AddButton("mod_preset_load", delegate
		{
			SelectPreset(ApplyPresetFile);
		});
		uIContextMenu.AddButton("mod_preset_delete", delegate
		{
			SelectPreset(DeletePreset);
		});
		uIContextMenu.AddButton("mod_preset_copy", CopyPreset);
		uIContextMenu.AddButton("mod_preset_paste", PastePreset);
		uIContextMenu.AddButton("mod_preset_folder", delegate
		{
			Directory.CreateDirectory(CorePath.PathLoadOrderPreset);
			Util.ShowExplorer(CorePath.PathLoadOrderPreset, selectFirstFile: true);
		});
		uIContextMenu.Show();
	}

	private void SavePreset()
	{
		Dialog.InputName("mod_preset_name", "mod_preset_default_name".lang(), delegate(bool cancel, string text)
		{
			if (!cancel)
			{
				string name = (text ?? "").Trim().SanitizeFileName().Trim();
				if (name.IsEmpty())
				{
					SE.BeepSmall();
				}
				else if (File.Exists(ModManager.GetPresetPath(name)))
				{
					Dialog.YesNo("mod_preset_overwrite".lang(name), delegate
					{
						WritePreset(name);
					});
				}
				else
				{
					WritePreset(name);
				}
			}
		});
	}

	private void WritePreset(string name)
	{
		if (!manager.SavePreset(name, out var _))
		{
			SE.BeepSmall();
			return;
		}
		SE.Click();
		ELayer.ui.Say("mod_preset_saved".lang(name));
	}

	private void SelectPreset(Action<FileInfo> onSelect)
	{
		List<FileInfo> files = manager.ListPresets();
		if (files.Count == 0)
		{
			Dialog.Ok("mod_preset_none");
			return;
		}
		Dialog.List("mod_preset_select", files, (FileInfo f) => Path.GetFileNameWithoutExtension(f.Name), delegate(int i, string _)
		{
			onSelect(files[i]);
			return true;
		}, canCancel: true);
	}

	private void ApplyPresetFile(FileInfo file)
	{
		if (!manager.TryLoadPresetFile(file, out var preset, out var _))
		{
			Dialog.Ok("mod_preset_invalid");
		}
		else
		{
			ConfirmPreset(preset);
		}
	}

	private void DeletePreset(FileInfo file)
	{
		string name = Path.GetFileNameWithoutExtension(file.Name);
		Dialog.YesNo("mod_preset_delete_confirm".lang(name), delegate
		{
			if (file.Exists)
			{
				file.Delete();
			}
			SE.Trash();
			ELayer.ui.Say("mod_preset_deleted".lang(name));
		});
	}

	private void CopyPreset()
	{
		ModLoadOrderPreset.Preset preset = ModLoadOrderPreset.Save(manager.packages, "", ELayer.core.version.GetText());
		if (preset.entries.Count == 0)
		{
			SE.BeepSmall();
			return;
		}
		GUIUtility.systemCopyBuffer = ModLoadOrderPreset.Serialize(preset);
		SE.Click();
		ELayer.ui.Say("mod_preset_copied");
	}

	private void PastePreset()
	{
		if (!ModLoadOrderPreset.TryParse(GUIUtility.systemCopyBuffer, out var preset, out var _))
		{
			Dialog.Ok("mod_preset_invalid");
			return;
		}
		if (preset.name.IsEmpty())
		{
			preset.name = "mod_preset_default_name".lang();
		}
		ConfirmPreset(preset);
	}

	private void ConfirmPreset(ModLoadOrderPreset.Preset preset)
	{
		List<ModLoadOrderPreset.Entry> list = ModLoadOrderPreset.FindMissing(manager.packages, preset);
		if (list.Count == 0)
		{
			ApplyPreset(preset);
			return;
		}
		Dialog.YesNo("mod_preset_missing".lang(DescribeEntries(list, 15)), delegate
		{
			ApplyPreset(preset);
		}, null, "mod_preset_apply", "cancel");
	}

	private static string DescribeEntries(List<ModLoadOrderPreset.Entry> entries, int max)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < entries.Count && i < max; i++)
		{
			ModLoadOrderPreset.Entry entry = entries[i];
			stringBuilder.Append(entry.title.IsEmpty() ? entry.id : (entry.title + " (" + entry.id + ")")).Append('\n');
		}
		if (entries.Count > max)
		{
			stringBuilder.Append("mod_preset_more".lang((entries.Count - max).ToString() ?? "")).Append('\n');
		}
		return stringBuilder.ToString().TrimEnd();
	}

	private void ApplyPreset(ModLoadOrderPreset.Preset preset)
	{
		if (HasFilter)
		{
			ClearSearch();
		}
		ModLoadOrderPreset.ApplyResult applyResult = manager.ApplyPreset(preset);
		SE.Tab();
		textRestart.SetActive(enable: true);
		RefreshLists();
		string text = ELayer.core.version.GetText();
		if (!preset.gameVersion.IsEmpty() && preset.gameVersion != text)
		{
			ELayer.ui.Say("mod_preset_version_warn".lang(preset.gameVersion, text));
		}
		ELayer.ui.Say("mod_preset_applied".lang(preset.name, applyResult.enabled.ToString() ?? "", applyResult.missing.Count.ToString() ?? ""));
	}

	public void Refresh()
	{
	}

	public override void OnKill()
	{
		if ((bool)TooltipManager.Instance)
		{
			TooltipManager.Instance.HideTooltips(immediate: true);
		}
		foreach (PreviewEntry value in previews.Values)
		{
			if ((bool)value.sprite)
			{
				if (value.ownsTexture)
				{
					UnityEngine.Object.Destroy(value.sprite.texture);
				}
				UnityEngine.Object.Destroy(value.sprite);
			}
		}
		previews.Clear();
		ELayer.core.mods.SaveLoadOrder();
		if (Instance == this)
		{
			Instance = null;
		}
	}
}
