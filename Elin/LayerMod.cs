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

	public UIItem infoTitle;

	public UIItem infoPreview;

	public UIItem infoPreviewNote;

	public UIItem infoAuthor;

	public UIItem infoVersion;

	public UIItem infoId;

	public UIItem infoWorkshop;

	public UIItem infoStatus;

	public UIItem infoProblems;

	public UIItem infoTags;

	public UIItem infoRequires;

	public UIItem infoIncompatible;

	public UIItem infoLoadAfter;

	public UIItem infoLoadBefore;

	public UIItem infoDescription;

	public GameObject infoSpace;

	public Sprite spriteNoPreview;

	private UIButton buttonFilter;

	private UIButton buttonCollapse;

	private Action<ModPackage, ItemMod> onInstantiate;

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

	private void Move(object row, int a)
	{
		List<object> items = list.items;
		int num = items.IndexOf(row);
		if (a > 0 && row is ModGroup { collapsed: false } modGroup)
		{
			a += Members(modGroup).Count;
		}
		if (num < 0 || num + a < 0 || num + a >= items.Count)
		{
			SE.BeepSmall();
			return;
		}
		object obj = items[num + a];
		ModGroup modGroup2;
		bool num2;
		if (!HasFilter && row is BaseModPackage baseModPackage)
		{
			modGroup2 = (obj as BaseModPackage)?.group;
			if (a <= 0)
			{
				if (baseModPackage.group == null)
				{
					num2 = modGroup2 != null;
					goto IL_00b8;
				}
			}
			else if (baseModPackage.group != null)
			{
				num2 = modGroup2 != baseModPackage.group;
				goto IL_00b8;
			}
		}
		goto IL_00d2;
		IL_00b8:
		if (num2)
		{
			baseModPackage.group = ((a > 0) ? null : modGroup2);
			OnReorder();
			return;
		}
		goto IL_00d2;
		IL_00d2:
		MoveTo(row, obj);
	}

	private void MoveTo(object row, object target)
	{
		List<BaseModPackage> packages = manager.packages;
		List<object> items = this.list.items;
		ModGroup modGroup = row as ModGroup;
		BaseModPackage baseModPackage = target as BaseModPackage;
		ModGroup modGroup2 = (target as ModGroup) ?? ((modGroup == null) ? null : baseModPackage?.group);
		if (target == null || target == row || (baseModPackage != null && baseModPackage.builtin) || (modGroup != null && modGroup == modGroup2))
		{
			SE.BeepSmall();
			RefreshLists();
			return;
		}
		bool flag = items.IndexOf(row) < items.IndexOf(target);
		List<BaseModPackage> list = ((modGroup != null) ? Members(modGroup) : new List<BaseModPackage> { (BaseModPackage)row });
		if (list.Count == 0)
		{
			SE.BeepSmall();
			RefreshLists();
			return;
		}
		int num = packages.IndexOf(list[0]);
		foreach (BaseModPackage item in list)
		{
			packages.Remove(item);
		}
		bool flag2 = manager.emptyGroups.Contains(modGroup2);
		bool flag3 = modGroup == null && flag && (flag2 || (modGroup2 != null && !modGroup2.collapsed));
		List<BaseModPackage> list2 = ((modGroup2 != null) ? Members(modGroup2) : null);
		bool flag4 = flag && !flag3;
		BaseModPackage baseModPackage2 = ((list2 == null) ? baseModPackage : ((list2.Count == 0) ? null : (flag4 ? list2[^1] : list2[0])));
		int num2 = ((baseModPackage2 != null) ? (packages.IndexOf(baseModPackage2) + (flag4 ? 1 : 0)) : (flag2 ? packages.Count : num));
		packages.InsertRange(num2, list);
		ModGroup modGroup3 = ((modGroup != null) ? list[0].group : ((modGroup2 == null) ? baseModPackage.group : (flag3 ? modGroup2 : null)));
		if (num2 == num && modGroup3 == list[0].group)
		{
			SE.BeepSmall();
			RefreshLists();
		}
		else
		{
			list[0].group = modGroup3;
			OnReorder();
		}
	}

	private void OnReorder(bool restart = true)
	{
		SE.Tab();
		if (restart)
		{
			textRestart.SetActive(enable: true);
		}
		manager.emptyGroups.RemoveAll((ModGroup g) => manager.packages.Any((BaseModPackage p) => p.group == g));
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
		if ((bool)buttonCollapse)
		{
			buttonCollapse.mainText.text = (Groups().Any((ModGroup g) => !g.collapsed) ? "mod_group_collapse_all" : "mod_group_expand_all").lang();
			windows[0].rectBottom.RebuildLayout(recursive: true);
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
		list.dragScrollView = list.GetComponentInParent<UIScrollView>();
		list.dragViewport = windows[0].Rect();
		list.onDragBegin = delegate(object row)
		{
			if (row is ModGroup g)
			{
				SetMembersActive(g, active: false);
			}
		};
		onInstantiate = SetModRow;
		list.callbacks = CreateCallbacks(list, builtin: false);
		list2.callbacks = CreateCallbacks(list2, builtin: true);
		InitSearchUI();
		windows[0].AddBottomButton("mod_preset", ShowPresetMenu);
		windows[0].AddBottomButton("mod_group_new", NewGroup);
		buttonCollapse = windows[0].AddBottomButton("mod_group_collapse_all", CollapseAll);
		infoPreview.button1.SetOnClick(delegate
		{
			ShowPreview(panelTarget);
		});
		RefreshLists();
		ShowInfo(FirstInfoTarget());
		list.dragEdgeSize = list.callbacks.GetMold()?.Rect().sizeDelta.y ?? 34f;
	}

	private UIList.Callback<object, ItemMod> CreateCallbacks(UIList target, bool builtin)
	{
		return new UIList.Callback<object, ItemMod>
		{
			onClick = delegate
			{
			},
			onInstantiate = delegate(object row, ItemMod b)
			{
				if (row is ModGroup g)
				{
					SetGroupRow(g, b);
				}
				else
				{
					onInstantiate((ModPackage)row, b);
				}
			},
			onList = delegate
			{
				ModGroup modGroup = null;
				foreach (BaseModPackage package in manager.packages)
				{
					if (package.builtin == builtin && (builtin || Match(package)))
					{
						if (!HasFilter && package.group != modGroup)
						{
							modGroup = package.group;
							if (modGroup != null)
							{
								target.Add(modGroup);
							}
						}
						if (HasFilter || modGroup == null || !modGroup.collapsed)
						{
							target.Add(package);
						}
					}
				}
				if (!builtin && !HasFilter)
				{
					target.AddCollection(manager.emptyGroups);
				}
			},
			onRefresh = Refresh,
			onDragReorder = delegate(object row, int a)
			{
				if (row is ModGroup g)
				{
					SetMembersActive(g, active: true);
				}
				if (a != 0)
				{
					List<object> items = target.items;
					int num = items.IndexOf(row);
					MoveTo(row, items[Mathf.Clamp(num + a, 0, items.Count - 1)]);
				}
			},
			canDragReorder = (object row) => !(row is BaseModPackage baseModPackage) || !baseModPackage.builtin
		};
	}

	private void SetModRow(ModPackage a, ItemMod b)
	{
		b.package = a;
		string title = ((a.group != null && !HasFilter) ? "   " : "") + (ELayer.core.mods.packages.IndexOf(a) + 1) + ". " + (a.isInPackages ? "[Local] " : "") + a.title.IsEmpty(a.dirInfo.Name);
		b.buttonActivate.mainText.SetText(FitTitle(b.buttonActivate.mainText, title), (!a.IsValidVersion() || a.blockedBy != null || !a.langDepError.IsEmpty()) ? FontColor.Bad : (a.activated ? FontColor.ButtonGeneral : FontColor.Passive));
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
		b.buttonActivate.GetOrCreate<HoverRelay>().onEnter = delegate
		{
			ShowInfo(a);
		};
		UIButton bt = b.buttonToggle;
		bt.SetOnClick(delegate
		{
			a.willActivate = !a.willActivate;
			bt.SetCheck(a.willActivate);
			ELayer.core.mods.SaveLoadOrder();
			textRestart.SetActive(enable: true);
			if (a.group != null && !HasFilter)
			{
				SetGroupRow(a.group, list.GetPair<ItemMod>(a.group));
			}
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
						modPackage.group = a.group;
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
				AddGroupButtons(uIContextMenu, a);
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
			if (!a.id.IsEmpty())
			{
				uIContextMenu.AddButton("mod_id_copy", delegate
				{
					SE.Click();
					GUIUtility.systemCopyBuffer = a.id;
					ELayer.ui.Say("mod_id_copied".lang(a.id));
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
	}

	private List<BaseModPackage> Members(ModGroup g)
	{
		return manager.packages.Where((BaseModPackage p) => p.group == g).ToList();
	}

	private IEnumerable<ModGroup> Groups()
	{
		return (from p in manager.packages
			select p.@group into g
			where g != null
			select g).Distinct().Concat(manager.emptyGroups);
	}

	private void SetMembersActive(ModGroup g, bool active)
	{
		if (g.collapsed)
		{
			return;
		}
		foreach (BaseModPackage item in Members(g))
		{
			list.GetPair(item).component.SetActive(active);
		}
	}

	private void NewGroup()
	{
		if (ModManager.disableMod)
		{
			SE.BeepSmall();
			return;
		}
		SE.Click();
		InputGroupName("mod_group_default_name".lang(), delegate(string name)
		{
			if (HasFilter)
			{
				ClearSearch();
			}
			ModGroup item = new ModGroup
			{
				name = name
			};
			manager.emptyGroups.Add(item);
			OnReorder(restart: false);
			list.dragScrollView.content.RebuildLayout(recursive: true);
			list.dragScrollView.verticalNormalizedPosition = 0f;
		});
	}

	private void SetGroupRow(ModGroup g, ItemMod b)
	{
		List<BaseModPackage> members = Members(g);
		int on = members.Count((BaseModPackage p) => p.willActivate);
		string title = (g.collapsed ? "[+] " : "[-] ") + g.name;
		UIText mainText = b.buttonActivate.mainText;
		mainText.fontStyle = FontStyle.Bold;
		mainText.SetText(FitTitle(mainText, title, " (" + on + "/" + members.Count + ")"), FontColor.Topic);
		Color topic = mainText.color;
		Color? color = g.color;
		if (color.HasValue)
		{
			Color valueOrDefault = color.GetValueOrDefault();
			mainText.SetText(mainText.text, valueOrDefault.Multiply(0.45f, 0f));
		}
		b.imageGroup.SetActive(enable: true);
		b.imageGroup.color = (g.color ?? topic).SetAlpha(0.4f);
		b.buttonActivate.subText.text = "";
		b.buttonLock.mainText.text = "";
		b.buttonUp.SetOnClick(delegate
		{
			Move(g, -1);
		});
		b.buttonDown.SetOnClick(delegate
		{
			Move(g, 1);
		});
		b.buttonToggle.SetToggle(on > 0 && on == members.Count);
		b.buttonToggle.interactable = members.Count > 0;
		b.buttonToggle.SetOnClick(delegate
		{
			foreach (BaseModPackage item in members)
			{
				item.willActivate = on != members.Count;
			}
			OnReorder();
		});
		b.buttonActivate.SetOnClick(delegate
		{
			g.collapsed = !g.collapsed;
			OnReorder(restart: false);
		});
		b.buttonRename.SetActive(enable: true);
		b.buttonRename.SetOnClick(delegate
		{
			InputGroupName(g.name, delegate(string name)
			{
				g.name = name;
				OnReorder(restart: false);
			});
		});
		b.buttonColor.SetActive(enable: true);
		b.buttonColor.SetOnClick(delegate
		{
			ELayer.ui.AddLayer<LayerColorPicker>().SetColor(g.color ?? topic, topic, delegate(PickerState state, Color c)
			{
				c.a = topic.a;
				g.color = ((c == topic) ? ((Color?)null) : new Color?(c));
				if (state == PickerState.Confirm)
				{
					OnReorder(restart: false);
				}
				else
				{
					SetGroupRow(g, b);
				}
			});
		});
		b.buttonDisband.SetActive(enable: true);
		b.buttonDisband.SetOnClick(delegate
		{
			foreach (BaseModPackage item2 in members)
			{
				item2.group = null;
			}
			manager.emptyGroups.Remove(g);
			OnReorder(restart: false);
		});
	}

	private void AddGroupButtons(UIContextMenu m, BaseModPackage a)
	{
		m.AddButton("mod_group_new", delegate
		{
			InputGroupName("mod_group_default_name".lang(), delegate(string name)
			{
				SetGroup(a, new ModGroup
				{
					name = name
				});
			});
		});
		List<ModGroup> groups = (from g in Groups()
			where g != a.@group
			select g).ToList();
		if (groups.Count > 0)
		{
			m.AddButton("mod_group_move", delegate
			{
				SE.Click();
				Dialog.List("mod_group_select", groups, (ModGroup g) => g.name, delegate(int i, string _)
				{
					SetGroup(a, groups[i]);
					return true;
				}, canCancel: true);
			});
		}
		if (a.group != null)
		{
			m.AddButton("mod_group_remove", delegate
			{
				SetGroup(a, null);
			});
		}
	}

	private void SetGroup(BaseModPackage p, ModGroup g)
	{
		if (HasFilter)
		{
			ClearSearch();
		}
		List<BaseModPackage> packages = manager.packages;
		ModGroup host = ((g != null && packages.Any((BaseModPackage m) => m.group == g)) ? g : p.group);
		BaseModPackage baseModPackage = ((host == null) ? null : packages.LastOrDefault((BaseModPackage m) => m != p && m.group == host));
		if (baseModPackage != null)
		{
			packages.Remove(p);
			packages.Insert(packages.IndexOf(baseModPackage) + 1, p);
		}
		p.group = g;
		if (g != null)
		{
			g.collapsed = false;
		}
		OnReorder();
	}

	private void CollapseAll()
	{
		List<ModGroup> list = Groups().ToList();
		if (list.Count == 0)
		{
			SE.BeepSmall();
			return;
		}
		bool collapsed = list.Any((ModGroup g) => !g.collapsed);
		foreach (ModGroup item in list)
		{
			item.collapsed = collapsed;
		}
		OnReorder(restart: false);
	}

	private void InputGroupName(string text, Action<string> onInput)
	{
		Dialog.InputName("mod_group_name", text, delegate(bool cancel, string s)
		{
			if (!cancel)
			{
				string text2 = (s ?? "").Trim();
				if (text2.IsEmpty())
				{
					SE.BeepSmall();
				}
				else
				{
					onInput(text2);
				}
			}
		});
	}

	private static string FitTitle(UIText text, string title, string suffix = "")
	{
		TextGenerationSettings generationSettings = text.GetGenerationSettings(Vector2.zero);
		TextGenerator cachedTextGeneratorForLayout = text.cachedTextGeneratorForLayout;
		float num = text.rectTransform.rect.width * text.pixelsPerUnit;
		if (cachedTextGeneratorForLayout.GetPreferredWidth(title + suffix, generationSettings) <= num)
		{
			return title + suffix;
		}
		int num2 = title.Length;
		while (num2 > 1 && cachedTextGeneratorForLayout.GetPreferredWidth(title[..num2].TrimEnd() + ".." + suffix, generationSettings) > num)
		{
			num2--;
		}
		return title[..num2].TrimEnd() + ".." + suffix;
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
		}
		buttonFilter = windows[0].AddBottomButton("mod_filter", CycleFilter);
		RefreshFilterButton();
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
		return list.items.OfType<BaseModPackage>().FirstOrDefault() ?? manager.packages.FirstOrDefault();
	}

	private void ShowInfo(BaseModPackage p)
	{
		if (p == null || p == panelTarget)
		{
			return;
		}
		panelTarget = p;
		UIText text = infoTitle.text1;
		text.SetText(p.title.IsEmpty(p.dirInfo.Name));
		TextGenerationSettings generationSettings = text.GetGenerationSettings(new Vector2(text.rectTransform.rect.width, 0f));
		generationSettings.horizontalOverflow = HorizontalWrapMode.Wrap;
		TextGenerator cachedTextGeneratorForLayout = text.cachedTextGeneratorForLayout;
		cachedTextGeneratorForLayout.Populate(text.text, generationSettings);
		if (cachedTextGeneratorForLayout.lineCount < 2)
		{
			text.text += "\n";
		}
		if (cachedTextGeneratorForLayout.lineCount > 2)
		{
			string text2 = text.text;
			int num = cachedTextGeneratorForLayout.lines[2].startCharIdx;
			do
			{
				int length = --num - 0;
				string str = (text.text = text2.Substring(0, length).TrimEnd() + "..");
				cachedTextGeneratorForLayout.Populate(str, generationSettings);
			}
			while (num > 1 && cachedTextGeneratorForLayout.lineCount > 2);
		}
		PreviewEntry thumb = GetThumb(p);
		infoPreview.SetActive(enable: true);
		infoPreview.image1.sprite = thumb.sprite ?? spriteNoPreview;
		SetTopic(infoAuthor, p.author.IsEmpty("-"), null);
		SetTopic(infoVersion, p.version.IsEmpty("-"), null);
		SetTopic(infoId, p.id, null);
		string text4 = ModLoadOrderPreset.WorkshopId(p);
		SetTopic(infoWorkshop, text4.IsEmpty() ? p.dirInfo.Name : text4, text4.IsEmpty() ? "mod_filter_local" : "mod_filter_workshop");
		SetTopic(infoStatus, (!p.builtin && !p.willActivate) ? "mod_filter_disabled".lang() : null, null);
		List<string> list = new List<string>();
		if (p.blockedBy != null)
		{
			list.Add("mod_info_blocked".lang(p.blockedBy.title.IsEmpty(p.blockedBy.id)));
		}
		if (!p.langDepError.IsEmpty())
		{
			string[] array = p.langDepError.Split('\t');
			list.Add(array[0].lang(string.Join("mod_info_or".lang(), array, 1, array.Length - 1)));
		}
		if (p.duplicateOf != null)
		{
			list.Add("mod_info_duplicate".lang(p.duplicateOf.title.IsEmpty(p.duplicateOf.dirInfo?.Name).IsEmpty(p.duplicateOf.id)));
		}
		if (!p.IsValidVersion())
		{
			list.Add("mod_info_old_version".lang(p.version, ELayer.core.versionMod.GetText()));
		}
		if (!p.parseError.IsEmpty())
		{
			list.Add("mod_info_parse_error".lang(p.parseError));
		}
		SetInfo(infoProblems, string.Join("\n", list));
		UIItem item = infoTags;
		string[] tags = p.tags;
		SetTopic(item, (tags != null && tags.Length > 0) ? string.Join(", ", p.tags) : null, null);
		SetTopic(infoRequires, IdList(p.dependency, alternatives: true), null);
		SetTopic(infoIncompatible, IdList(p.incompatible, alternatives: false), null);
		SetTopic(infoLoadAfter, IdList(p.loadAfter, alternatives: false), null);
		SetTopic(infoLoadBefore, IdList(p.loadBefore, alternatives: false), null);
		infoSpace.SetActive(!p.description.IsEmpty());
		SetInfo(infoDescription, p.description);
		panelNote.Build();
		panelScroll.content.anchoredPosition = Vector2.zero;
		static string IdList(string[][] rows, bool alternatives)
		{
			if (rows == null)
			{
				return null;
			}
			string sep = (alternatives ? "mod_info_or".lang() : "\n");
			return string.Join("\n", from row in rows
				where row != null && row.Length > 0
				select string.Join(sep, row.Where((string id) => !id.IsEmpty())) into line
				where !line.IsEmpty()
				select line);
		}
		static void SetInfo(UIItem uIItem, string value)
		{
			uIItem.SetActive(!value.IsEmpty());
			if (!value.IsEmpty())
			{
				uIItem.text1.SetText(value);
			}
		}
		static void SetTopic(UIItem uIItem, string value, string lang)
		{
			uIItem.SetActive(!value.IsEmpty());
			if (!value.IsEmpty())
			{
				if (lang != null)
				{
					uIItem.text1.SetText(lang.lang());
				}
				uIItem.text2.SetText(value);
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
