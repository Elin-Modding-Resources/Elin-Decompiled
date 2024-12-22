using System.Collections.Generic;

public class LayerMod : ELayer
{
	public static LayerMod Instance;

	public UIList list;

	public UIList list2;

	public UIText textRestart;

	public UIButton toggleDisableMods;

	public ModManager manager => ELayer.core.mods;

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
		UIList uIList = list;
		UIList uIList2 = list2;
		UIList.Callback<BaseModPackage, ItemMod> obj = new UIList.Callback<BaseModPackage, ItemMod>
		{
			onClick = delegate
			{
			},
			onInstantiate = delegate(BaseModPackage a, ItemMod b)
			{
				a.UpdateMeta(updateOnly: true);
				b.package = a;
				string s = ELayer.core.mods.packages.IndexOf(a) + 1 + ". " + (a.isInPackages ? "[Local] " : "") + a.title;
				b.buttonActivate.mainText.SetText(s, (!a.IsValidVersion()) ? FontColor.Bad : (a.activated ? FontColor.ButtonGeneral : FontColor.Passive));
				b.buttonActivate.subText.text = a.version;
				b.buttonLock.mainText.text = a.author;
				b.buttonUp.SetActive(!a.builtin);
				b.buttonDown.SetActive(!a.builtin);
				b.buttonToggle.SetToggle(a.willActivate);
				b.buttonUp.SetOnClick(delegate
				{
					Move(a, b, -1);
				});
				b.buttonDown.SetOnClick(delegate
				{
					Move(a, b, 1);
				});
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
					if (ELayer.debug.enable || (!BaseCore.IsOffline && a.isInPackages && !a.builtin && !ELayer.core.version.demo))
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
					if (!a.builtin)
					{
						uIContextMenu.AddButton(a.willActivate ? "mod_deactivate" : "mod_activate", delegate
						{
							SE.Click();
							a.willActivate = !a.willActivate;
							ELayer.core.mods.SaveLoadOrder();
							list.List();
							textRestart.SetActive(enable: true);
						});
					}
					uIContextMenu.AddButton("mod_info", delegate
					{
						SE.Click();
						Util.ShowExplorer(a.dirInfo.FullName + "/package.xml");
					});
					uIContextMenu.Show();
				});
				b.buttonLock.onClick.AddListener(delegate
				{
					Refresh();
				});
			},
			onList = delegate
			{
				foreach (BaseModPackage package in manager.packages)
				{
					if (package.builtin)
					{
						list2.Add(package);
					}
					else
					{
						list.Add(package);
					}
				}
			},
			onRefresh = Refresh
		};
		UIList.ICallback callbacks = obj;
		uIList2.callbacks = obj;
		uIList.callbacks = callbacks;
		list.List();
		list2.List();
		void Move(BaseModPackage p, ItemMod b, int a)
		{
			List<BaseModPackage> packages = ELayer.core.mods.packages;
			int num = packages.IndexOf(p);
			if (num + a < 0 || num + a >= packages.Count || packages[num + a].builtin)
			{
				SE.BeepSmall();
			}
			else
			{
				packages.Move(p, a);
				SE.Tab();
				textRestart.SetActive(enable: true);
				ELayer.core.mods.SaveLoadOrder();
				list.List();
			}
		}
	}

	public void Refresh()
	{
	}

	public override void OnKill()
	{
		ELayer.core.mods.SaveLoadOrder();
	}
}
