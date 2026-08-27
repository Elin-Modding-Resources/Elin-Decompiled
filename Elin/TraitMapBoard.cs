using System.Collections.Generic;
using CreativeSpore.SuperTilemapEditor;
using UnityEngine;
using UnityEngine.UI;

public class TraitMapBoard : TraitBoard
{
	public override bool IsHomeItem => true;

	public override void TrySetAct(ActPlan p)
	{
		if (!EClass.debug.enable && !EClass._zone.IsPCFactionOrTent)
		{
			return;
		}
		p.TrySetAct("actChangeHomeIcon", delegate
		{
			UIContextMenu uIContextMenu = EClass.ui.CreateContextMenuInteraction();
			GridLayoutGroup parent = uIContextMenu.AddGridLayout();
			HashSet<int> hashSet = new HashSet<int>();
			foreach (Spatial value in EClass.game.spatials.map.Values)
			{
				if (value.icon > 0)
				{
					hashSet.Add(value.icon);
				}
			}
			foreach (int item in hashSet)
			{
				UIButton uIButton = Util.Instantiate<UIButton>("UI/Element/Button/ButtonContainerIcon", parent);
				int _i = item;
				uIButton.icon.sprite = TilemapUtils.GetOrCreateTileSprite(EClass.scene.elomap.actor.tileset, item);
				uIButton.icon.Rect().localScale = new Vector3(2f, 2f, 1f);
				uIButton.SetOnClick(delegate
				{
					SE.Click();
					EClass._zone.icon = _i;
					EClass.ui.contextMenu.currentMenu.Hide();
					EClass.scene.elomap.SetZone(EClass._zone.x, EClass._zone.y, EClass._zone, updateMesh: true);
				});
			}
			uIContextMenu.Show();
			return false;
		}, owner);
		p.TrySetAct("actChangeBlockHeight", delegate
		{
			UIContextMenu uIContextMenu = EClass.ui.CreateContextMenuInteraction();
			uIContextMenu.AddSlider("adjustment", (float a) => a.ToString() ?? "", EClass._map.config.blockHeight * 10f, delegate(float b)
			{
				EClass._map.config.blockHeight = b * 0.1f;
			}, 0f, 40f, isInt: true, hideOther: false);
			uIContextMenu.Show();
			return false;
		}, owner);
		if (EClass._zone.IsSkyLevel)
		{
			p.TrySetAct("actChangeSkyBlockHeight", delegate
			{
				UIContextMenu uIContextMenu = EClass.ui.CreateContextMenuInteraction();
				uIContextMenu.AddSlider("adjustment", (float a) => a.ToString() ?? "", EClass._map.config.skyBlockHeight, delegate(float b)
				{
					EClass._map.config.skyBlockHeight = (int)b;
				}, 1f, 20f, isInt: true, hideOther: false);
				uIContextMenu.Show();
				return false;
			}, owner);
		}
		p.TrySetAct("actChangeMapBG", delegate
		{
			LayerList layerList = EClass.ui.AddLayer<LayerList>().SetSize(400f);
			List<MapBG> list = Util.EnumToList<MapBG>();
			for (int i = 0; i < list.Count; i++)
			{
				layerList.Add(list[i].ToString(), delegate(int a)
				{
					EClass._map.config.bg = list[a];
					EClass.scene.RefreshBG();
				});
			}
			layerList.Show();
			return false;
		}, owner);
		p.TrySetAct("actChangeShadowStrength", delegate
		{
			UIContextMenu uIContextMenu = EClass.ui.CreateContextMenuInteraction();
			uIContextMenu.AddSlider("adjustment", (float a) => a + "%", EClass._map.config.shadowStrength * 100f, delegate(float b)
			{
				EClass._map.config.shadowStrength = b * 0.01f;
				EClass.screen.RefreshAll();
			}, 0f, 400f, isInt: true, hideOther: false);
			uIContextMenu.Show();
			return false;
		}, owner);
		p.TrySetAct("actChangeHeightShadowStrength", delegate
		{
			UIContextMenu uIContextMenu = EClass.ui.CreateContextMenuInteraction();
			uIContextMenu.AddSlider("adjustment", (float a) => a + "%", EClass._map.config.heightLightMod * 1000f, delegate(float b)
			{
				EClass._map.config.heightLightMod = b * 0.001f;
			}, 0f, 100f, isInt: true, hideOther: false);
			uIContextMenu.Show();
			return false;
		}, owner);
		p.TrySetAct("actChangeFogDensity", delegate
		{
			LayerList layerList = EClass.ui.AddLayer<LayerList>().SetSize(400f);
			List<FogType> list = Util.EnumToList<FogType>();
			for (int i = 0; i < list.Count; i++)
			{
				layerList.Add((list[i].ToString() == "Blizard") ? "Blizzard" : list[i].ToString(), delegate(int a)
				{
					EClass._map.config.fog = list[a];
					EClass.screen.RefreshAll();
				});
			}
			layerList.Show();
			return false;
		}, owner);
		p.TrySetAct("actChangeSkyColor", delegate
		{
			EClass.ui.AddLayer<LayerColorPicker>().SetColor(EClass._map.config.colorScreen.Get(), new Color(0f, 0f, 0f, 0f), delegate(PickerState state, Color _c)
			{
				EClass._map.config.colorScreen.Set(_c);
				EClass.screen.RefreshGrading();
			});
			return false;
		}, owner);
		p.TrySetAct("actChangeSeaColor", delegate
		{
			EClass.ui.AddLayer<LayerColorPicker>().SetColor(EClass._map.config.colorSea.Get(), new Color(0f, 0f, 0f, 0f), delegate(PickerState state, Color _c)
			{
				EClass._map.config.colorSea.Set(_c);
				EClass.screen.RefreshGrading();
			});
			return false;
		}, owner);
		bool flag = !EClass._map.IsIndoor || EClass._zone is Zone_Tent;
		if (EClass._map.config.reverseRoof)
		{
			flag = !flag;
		}
		p.TrySetAct("actToggleMapRoof".lang((flag ? "on" : "off").lang()), delegate
		{
			EClass._map.config.reverseRoof = !EClass._map.config.reverseRoof;
			SE.ClickOk();
			return false;
		}, owner);
	}
}
