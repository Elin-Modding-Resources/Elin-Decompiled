using Newtonsoft.Json;

public class HotbarManager : EClass
{
	[JsonProperty]
	public Hotbar[] bars = new Hotbar[8];

	public void ResetHotbar(int id)
	{
		WidgetHotbar widgetHotbar = bars[id]?.actor ?? null;
		Hotbar hotbar = null;
		switch (id)
		{
		case 0:
			hotbar = SetHobar(0, 4);
			break;
		case 1:
			hotbar = SetHobar(1, 4);
			hotbar.SetItem(new HotItemContext
			{
				id = "system"
			}, 0);
			hotbar.SetItem(new HotItemToggleLog(), 1);
			hotbar.SetItem(new HotItemContext
			{
				id = "mapTool"
			}, 2);
			hotbar.SetItem(new HotItemToggleWidget(), 3);
			break;
		case 2:
			hotbar = SetHobar(2, 7);
			hotbar.SetItem(new HotItemLayer
			{
				id = "stash"
			});
			hotbar.SetItem(new HotItemLayer
			{
				id = "LayerAbility"
			});
			hotbar.SetItem(new HotItemLayer
			{
				id = "LayerChara"
			});
			hotbar.SetItem(new HotItemLayer
			{
				id = "LayerJournal"
			});
			if (EClass.core.IsGameStarted && EClass._zone.CanEnterBuildModeAnywhere)
			{
				hotbar.SetItem(new HotItemActionMode
				{
					id = "Inspect"
				});
			}
			if (EClass.core.IsGameStarted && EClass._zone.IsRegion)
			{
				hotbar.SetItem(new HotItemLayer
				{
					id = "LayerTravel"
				});
			}
			break;
		case 3:
		{
			hotbar = SetHobar(3, 12);
			if (!EClass.core.IsGameStarted)
			{
				return;
			}
			bool flag = EClass.debug.godBuild || EClass.debug.ignoreBuildRule;
			if (flag || EClass.Branch != null)
			{
				if (flag || EClass.Branch.elements.Has(4006))
				{
					hotbar.SetItem(new HotItemActionMode
					{
						id = "Inspect"
					}).always = true;
				}
				if (flag || EClass.Branch.elements.Has(4000))
				{
					hotbar.SetItem(new HotItemActionMode
					{
						id = "Cut"
					});
				}
				if (flag || EClass.Branch.elements.Has(4001))
				{
					hotbar.SetItem(new HotItemActionMode
					{
						id = "Mine"
					});
				}
				if (flag || EClass.Branch.elements.Has(4001))
				{
					hotbar.SetItem(new HotItemActionMode
					{
						id = "Dig"
					});
				}
				if (flag || EClass.Branch.elements.Has(4002))
				{
					hotbar.SetItem(new HotItemActionMode
					{
						id = "Terrain"
					});
				}
				if (flag)
				{
					hotbar.SetItem(new HotItemActionMode
					{
						id = "Copy"
					});
				}
				if (flag)
				{
					hotbar.SetItem(new HotItemActionMode
					{
						id = "Populate"
					});
				}
				if (flag || EClass.Branch.elements.Has(4004))
				{
					hotbar.SetItem(new HotItemActionMode
					{
						id = "Deconstruct"
					});
				}
			}
			else if (EClass._zone is Zone_Tent)
			{
				if (flag || EClass.pc.homeBranch.elements.Has(4006))
				{
					hotbar.SetItem(new HotItemActionMode
					{
						id = "Inspect"
					}).always = true;
				}
				if (flag || EClass.pc.homeBranch.elements.Has(4000))
				{
					hotbar.SetItem(new HotItemActionMode
					{
						id = "Cut"
					});
				}
				if (flag || EClass.pc.homeBranch.elements.Has(4001))
				{
					hotbar.SetItem(new HotItemActionMode
					{
						id = "Mine"
					});
				}
				if (flag || EClass.pc.homeBranch.elements.Has(4001))
				{
					hotbar.SetItem(new HotItemActionMode
					{
						id = "Dig"
					});
				}
				if (flag || EClass.pc.homeBranch.elements.Has(4004))
				{
					hotbar.SetItem(new HotItemActionMode
					{
						id = "Deconstruct"
					});
				}
				hotbar.SetItem(new HotItemActionMode
				{
					id = "Terrain"
				});
			}
			hotbar.SetItem(new HotItemActionMode
			{
				id = "ExitBuild"
			});
			break;
		}
		case 4:
			hotbar = SetHobar(4, 9);
			if (!EClass.core.IsGameStarted)
			{
				return;
			}
			if (EClass.debug.godBuild || EClass.Branch != null)
			{
				hotbar.SetItem(new HotItemActionMode
				{
					id = "Cinema"
				});
				hotbar.SetItem(new HotItemActionMode
				{
					id = "FlagCell"
				});
				if (EClass.debug.godBuild || EClass.Branch.elements.Has(4006))
				{
					hotbar.SetItem(new HotItemActionMode
					{
						id = "Visibility"
					});
				}
				if (EClass.debug.godBuild)
				{
					hotbar.SetItem(new HotItemActionMode
					{
						id = "EditMarker"
					});
				}
				if (EClass.debug.godBuild || EClass.Branch.elements.Has(4005))
				{
					hotbar.SetItem(new HotItemActionMode
					{
						id = "Picker"
					});
				}
				if (EClass.debug.godBuild)
				{
					hotbar.SetItem(new HotItemContext
					{
						id = "mapTool",
						autoExpand = false
					});
				}
			}
			break;
		case 5:
			hotbar = SetHobar(5, 9);
			hotbar.SetItem(new HotItemLayer
			{
				id = "LayerHelp"
			}, 0);
			hotbar.SetItem(new HotItemWidget
			{
				id = "Codex"
			}, 1);
			hotbar.SetItem(new HotItemWidget
			{
				id = "Memo"
			}, 2);
			hotbar.SetItem(new HotItemWidget
			{
				id = "QuestTracker"
			}, 3);
			hotbar.SetItem(new HotItemActionSleep(), 4);
			break;
		case 6:
			hotbar = SetHobar(6, 9);
			break;
		case 7:
			hotbar = SetHobar(7, 7);
			hotbar.SetItem(new HotItemToggle
			{
				type = HotItemToggle.Type.SnapFreepos
			});
			hotbar.SetItem(new HotItemToggle
			{
				type = HotItemToggle.Type.ToggleRoof
			});
			hotbar.SetItem(new HotItemToggle
			{
				type = HotItemToggle.Type.ToggleSlope
			});
			hotbar.SetItem(new HotItemToggle
			{
				type = HotItemToggle.Type.ToggleBuildLight
			});
			break;
		}
		hotbar.actor = widgetHotbar;
		if ((bool)widgetHotbar)
		{
			widgetHotbar.Rebuild();
		}
	}

	public Hotbar SetHobar(int id, int slots, WidgetHotbar actor = null)
	{
		Hotbar hotbar = new Hotbar();
		hotbar.itemsPerPage = slots;
		bars[id] = hotbar;
		hotbar.AddPage();
		hotbar.AddPage();
		hotbar.id = id;
		return hotbar;
	}

	public void OnCreateGame()
	{
		ResetHotbar(0);
		ResetHotbar(1);
		ResetHotbar(2);
		ResetHotbar(3);
		ResetHotbar(4);
		ResetHotbar(5);
		ResetHotbar(6);
		ResetHotbar(7);
	}
}
