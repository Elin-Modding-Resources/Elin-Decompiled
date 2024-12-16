using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class HotItemContext : HotItem
{
	[JsonProperty]
	public string id;

	[JsonProperty]
	public bool autoExpand;

	public override string Name => ("m_" + id).lang();

	public override string TextTip => id.lang();

	public override string pathSprite
	{
		get
		{
			if (!(id == "system"))
			{
				return "icon_" + id;
			}
			return "icon_m_system";
		}
	}

	public override Selectable.Transition Transition
	{
		get
		{
			if (!AutoExpand)
			{
				return base.Transition;
			}
			return Selectable.Transition.None;
		}
	}

	public bool AutoExpand => autoExpand;

	public override void OnHover(UIButton b)
	{
		if (AutoExpand && !EClass.ui.BlockInput)
		{
			OnClick(null, null);
		}
	}

	public override void OnClick(UIButton b)
	{
		if (!EClass.ui.contextMenu.isActive)
		{
			Show(id, UIButton.buttonPos);
		}
	}

	public override void OnShowContextMenu(UIContextMenu m)
	{
		m.AddToggle("autoExpand", autoExpand, delegate(bool on)
		{
			autoExpand = on;
		});
	}

	public static void Show(string id, Vector3 pos)
	{
		string menuName = ((id == "system") ? "ContextSystem" : "ContextMenu");
		UIContextMenu m = EClass.ui.contextMenu.Create(menuName);
		GameDate d = EClass.core.game.world.date;
		Game.Config conf = EClass.game.config;
		bool isRegion = EClass._zone.IsRegion;
		if (!(id == "mapTool"))
		{
			if (id == "system")
			{
				UIContextMenu uIContextMenu = m.AddChild("etc");
				uIContextMenu.AddButton("LayerFeedback".lang() + "(" + EInput.keys.report.key.ToString() + ")", delegate
				{
					EClass.ui.ToggleFeedback();
				});
				uIContextMenu.AddButton("LayerConsole", delegate
				{
					EClass.ui.AddLayer<LayerConsole>();
				});
				uIContextMenu.AddButton("LayerCredit", delegate
				{
					EClass.ui.AddLayer<LayerCredit>();
				});
				uIContextMenu.AddButton("announce", delegate
				{
					EClass.ui.AddLayer("LayerAnnounce");
				});
				uIContextMenu.AddButton("about", delegate
				{
					EClass.ui.AddLayer("LayerAbout");
				});
				uIContextMenu.AddButton("hideUI", delegate
				{
					SE.ClickGeneral();
					EClass.ui.ToggleCanvas();
				});
				UIContextMenu uIContextMenu2 = m.AddChild("tool");
				uIContextMenu2.AddButton("LayerMod", delegate
				{
					EClass.ui.AddLayer<LayerMod>();
				});
				uIContextMenu2.AddButton("LayerTextureViewer", delegate
				{
					EClass.ui.AddLayer<LayerTextureViewer>();
				});
				uIContextMenu2.AddButton("OpenCustomFolder", delegate
				{
					Util.ShowExplorer(CorePath.custom + "Portrait");
				});
				m.AddSeparator();
				m.AddButton("help", delegate
				{
					LayerHelp.Toggle("general", "1");
				});
				m.AddButton("widget", delegate
				{
					EClass.ui.AddLayer<LayerWidget>();
				});
				m.AddButton("config", delegate
				{
					EClass.ui.AddLayer<LayerConfig>();
				});
				m.AddSeparator();
				m.AddButton("LayerHoard", delegate
				{
					EClass.ui.AddLayer<LayerHoard>();
				});
				m.AddSeparator();
				if (EClass.game.Difficulty.allowManualSave || EClass.debug.enable)
				{
					m.AddButton("save", delegate
					{
						EClass.game.Save();
					});
					m.AddButton("load", delegate
					{
						EClass.ui.AddLayer<LayerLoadGame>().Init(_backup: false);
					});
				}
				m.AddSeparator();
				m.AddButton("title", delegate
				{
					EClass.game.GotoTitle();
				});
				m.AddButton("quit", EClass.game.Quit);
				m.GetComponent<Image>().SetAlpha(1f);
			}
		}
		else if (EClass.scene.actionMode.IsBuildMode)
		{
			if (EClass.debug.enable)
			{
				m.AddButton("Reset Map", delegate
				{
					Zone.forceRegenerate = true;
					EClass._zone.Activate();
				});
				m.AddChild("Map Subset");
				m.AddSeparator();
				AddSliderMonth();
				AddSliderHour();
				AddSliderWeather();
			}
		}
		else
		{
			if (!isRegion && EClass.scene.flock.gameObject.activeSelf)
			{
				m.AddButton("birdView", delegate
				{
					EClass.scene.ToggleBirdView();
				});
			}
			AddTilt();
			m.AddToggle("highlightArea", conf.highlightArea, delegate
			{
				EClass.scene.ToggleHighlightArea();
			});
			m.AddToggle("noRoof", conf.noRoof, delegate
			{
				EClass.scene.ToggleRoof();
			});
			if (EClass._zone.IsRegion)
			{
				m.AddSlider("zoomRegion", (float a) => a * (float)CoreConfig.ZoomStep + "%", conf.regionZoom / CoreConfig.ZoomStep, delegate(float b)
				{
					conf.regionZoom = (int)b * CoreConfig.ZoomStep;
				}, 100 / CoreConfig.ZoomStep, 200 / CoreConfig.ZoomStep, isInt: true, hideOther: false);
			}
			else if (ActionMode.Adv.zoomOut2)
			{
				m.AddSlider("zoomAlt", (float a) => a * (float)CoreConfig.ZoomStep + "%", conf.zoomedZoom / CoreConfig.ZoomStep, delegate(float b)
				{
					conf.zoomedZoom = (int)b * CoreConfig.ZoomStep;
				}, 50 / CoreConfig.ZoomStep, 200 / CoreConfig.ZoomStep, isInt: true, hideOther: false);
			}
			else
			{
				m.AddSlider("zoom", (float a) => a * (float)CoreConfig.ZoomStep + "%", conf.defaultZoom / CoreConfig.ZoomStep, delegate(float b)
				{
					conf.defaultZoom = (int)b * CoreConfig.ZoomStep;
				}, 50 / CoreConfig.ZoomStep, 200 / CoreConfig.ZoomStep, isInt: true, hideOther: false);
			}
			m.AddSlider("backDrawAlpha", (float a) => a + "%", EClass.game.config.backDrawAlpha, delegate(float b)
			{
				EClass.game.config.backDrawAlpha = (int)b;
			}, 0f, 50f, isInt: true, hideOther: false);
			if (EClass.debug.enable)
			{
				m.AddSeparator();
				AddSliderMonth();
				m.AddSlider("sliderDay", (float a) => a.ToString() ?? "", EClass.world.date.day, delegate(float b)
				{
					if ((int)b != EClass.world.date.day)
					{
						EClass.world.date.day = (int)b - 1;
						EClass.world.date.AdvanceDay();
					}
					EClass._map.RefreshAllTiles();
					EClass.screen.RefreshAll();
				}, 1f, 30f, isInt: true, hideOther: false);
				AddSliderHour();
				AddSliderWeather();
				m.AddSlider("sliderAnimeSpeed", (float a) => EClass.game.config.animeSpeed + "%", EClass.game.config.animeSpeed, delegate(float b)
				{
					EClass.game.config.animeSpeed = (int)b;
				}, 0f, 100f, isInt: true, hideOther: false);
				UIContextMenu uIContextMenu3 = m.AddChild("debug");
				uIContextMenu3.AddToggle("reveal_map", EClass.debug.revealMap, delegate
				{
					EClass.debug.ToggleRevealMap();
				});
				uIContextMenu3.AddToggle("test_los", EClass.debug.testLOS, delegate
				{
					Toggle(ref EClass.debug.testLOS);
				});
				uIContextMenu3.AddToggle("test_los2", EClass.debug.testLOS2, delegate
				{
					Toggle(ref EClass.debug.testLOS2);
				});
				uIContextMenu3.AddToggle("godBuild", EClass.debug.godBuild, delegate
				{
					Toggle(ref EClass.debug._godBuild);
				});
				uIContextMenu3.AddToggle("godMode", EClass.debug.godMode, delegate
				{
					Toggle(ref EClass.debug.godMode);
				});
				uIContextMenu3.AddToggle("autoAdvanceQuest", EClass.debug.autoAdvanceQuest, delegate
				{
					Toggle(ref EClass.debug.autoAdvanceQuest);
				});
				uIContextMenu3.AddSlider("slopeMod", (float a) => a.ToString() ?? "", conf.slopeMod, delegate(float b)
				{
					EClass.game.config.slopeMod = (int)b;
					(EClass.pc.renderer as CharaRenderer).first = true;
				}, 0f, 500f, isInt: true, hideOther: false);
			}
		}
		m.Show(pos);
		if (id == "system")
		{
			m.hideOnMouseLeave = false;
		}
		void AddSliderHour()
		{
			m.AddSlider("sliderTime", (float a) => a.ToString() ?? "", d.hour, delegate(float b)
			{
				Weather.Condition currentCondition = EClass.world.weather._currentCondition;
				if (d.hour != (int)b)
				{
					d.hour = (int)b - 1;
					d.AdvanceHour();
					EClass.world.weather.SetCondition(currentCondition);
				}
				EClass.pc.faith.OnChangeHour();
				EClass._map.RefreshFOV(EClass.pc.pos.x, EClass.pc.pos.z, 20, recalculate: true);
				EClass.screen.RefreshAll();
			}, 0f, 23f, isInt: true, hideOther: false);
		}
		void AddSliderMonth()
		{
			m.AddSlider("sliderMonth", (float a) => a.ToString() ?? "", EClass.world.date.month, delegate(float b)
			{
				if (d.month != (int)b)
				{
					d.month = (int)b - 1;
					d.AdvanceMonth();
				}
				EClass.player.holyWell++;
				EClass._map.RefreshAllTiles();
				EClass.screen.RefreshAll();
				EClass.pc.c_daysWithGod += 30;
				EClass.pc.RefreshFaithElement();
			}, 1f, 12f, isInt: true, hideOther: false);
		}
		void AddSliderWeather()
		{
			m.AddSlider("sliderWeather", (float a) => EClass.world.weather.GetName(((int)a).ToEnum<Weather.Condition>()) ?? "", (float)EClass.world.weather._currentCondition, delegate(float b)
			{
				EClass.world.weather.SetCondition(((int)b).ToEnum<Weather.Condition>());
			}, 0f, 7f, isInt: true, hideOther: false);
		}
		void AddTilt()
		{
			m.AddToggle("alwaysTilt".lang() + (isRegion ? "(Region)" : ""), isRegion ? conf.tiltRegion : conf.tilt, delegate
			{
				EClass.scene.ToggleTilt();
			});
			m.AddSlider("tiltPower", (float a) => a.ToString() ?? "", isRegion ? conf.tiltPowerRegion : conf.tiltPower, delegate(float b)
			{
				if (isRegion)
				{
					conf.tiltPowerRegion = (int)b;
				}
				else
				{
					conf.tiltPower = (int)b;
				}
				EClass.scene.camSupport.tiltShift.blurArea = 0.1f * b;
			}, 0f, 150f, isInt: true, hideOther: false);
		}
		static void Toggle(ref bool flag)
		{
			flag = !flag;
			WidgetMenuPanel.OnChangeMode();
			EClass.player.hotbars.ResetHotbar(2);
			SE.ClickGeneral();
		}
	}
}
