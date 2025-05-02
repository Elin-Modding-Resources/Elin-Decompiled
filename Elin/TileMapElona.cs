using CreativeSpore.SuperTilemapEditor;
using UnityEngine;

public class TileMapElona : BaseTileMap
{
	public EloMap elomap => EMono.scene.elomapActor.elomap;

	public override void Draw()
	{
		Zone zone = EMono._zone;
		_ = EMono.scene.camSupport.Zoom;
		SceneProfile profile = EMono.scene.profile;
		lightSetting = EMono.scene.profile.light;
		map = zone.map;
		Size = map.Size;
		SizeXZ = map.SizeXZ;
		isMining = EMono.scene.actionMode == ActionMode.Mine;
		iconMode = EMono.scene.actionMode.cardIconMode;
		count = 0;
		totalFire = 0;
		pcX = EMono.pc.pos.x;
		pcZ = EMono.pc.pos.z;
		float num = lightSetting.nightRatioCurve.Evaluate(EMono.scene.timeRatio);
		modSublight1 = profile.global.modSublight1 * num;
		modSublight2 = profile.global.modSublight2 * num;
		pcMaxLight = EMono.player.lightPower * profile.global.fovModNonGradient * 0.8f;
		buildMode = EMono.scene.actionMode.IsBuildMode;
		subtleHighlightArea = EMono.scene.actionMode.AreaHihlight == AreaHighlightMode.Build || EMono.scene.actionMode.AreaHihlight != AreaHighlightMode.Edit;
		highlightArea = EMono.scene.actionMode.AreaHihlight == AreaHighlightMode.Edit || subtleHighlightArea;
		lightLimit = lightSetting.lightLimit.Evaluate(EMono.scene.timeRatio);
		_lightMod = lightSetting.lightMod * lightSetting.lightModCurve.Evaluate(EMono.scene.timeRatio);
		_baseBrightness = lightSetting.baseBrightness * lightSetting.baseBrightnessCurve.Evaluate(EMono.scene.timeRatio);
		Fov.nonGradientMod = profile.global.fovModNonGradient;
		shadowStrength = lightSetting.shadowCurve.Evaluate(EMono.scene.timeRatio);
		RefreshHeight();
		innerMode = ((buildMode || ActionMode.Select.ForceInnerBlockMode()) ? defaultInnerMode : (ActionMode.Bird.IsActive ? InnerMode.Height : defaultInnerMode));
		currentHeight = EMono.pc.pos.cell.TopHeight;
		currentRoom = EMono.pc.pos.cell.room;
		map.rooms.Refresh();
		lowObj = false;
		if (usingHouseBoard || ActionMode.Bird.IsActive)
		{
			lowBlock = (hideRoomFog = (showFullWall = (hideHang = false)));
			showRoof = true;
		}
		else if (buildMode)
		{
			showRoof = EMono.game.config.showRoof;
			lowBlock = !showRoof && !EMono.game.config.showWall;
			showFullWall = !showRoof && EMono.game.config.showWall;
			hideRoomFog = true;
			hideHang = !showRoof && !EMono.game.config.showWall;
		}
		else if (ActionMode.IsAdv)
		{
			if (EMono.pc.pos.cell.Front.UseLowBlock || EMono.pc.pos.cell.Right.UseLowBlock || EMono.pc.pos.cell.Front.Right.UseLowBlock || EMono.pc.pos.cell.UseLowBlock || EMono.pc.pos.cell.UseLowBlock)
			{
				if (!EMono.pc.IsMoving)
				{
					lowblockTimer = 0.1f;
				}
			}
			else
			{
				lowblockTimer = 0f;
			}
			lowBlock = lowblockTimer > 0f;
			hideRoomFog = currentRoom != null && currentRoom.lot.idRoofStyle != 0;
			x = EMono.pc.pos.x;
			z = EMono.pc.pos.z;
			showFullWall = hideRoomFog;
			showRoof = !lowBlock && !hideRoomFog && EMono.game.config.noRoof;
			hideHang = showFullWall || lowBlock;
			EMono.game.config.showRoof = !hideRoomFog;
		}
		else
		{
			lowBlock = (hideRoomFog = (showFullWall = (hideHang = false)));
			showRoof = true;
		}
		if (map.config.indoor)
		{
			showRoof = false;
		}
		int num2 = TilemapUtils.GetMouseGridX(elomap.fogmap, EMono.scene.cam) - elomap.minX;
		int num3 = TilemapUtils.GetMouseGridY(elomap.fogmap, EMono.scene.cam) - elomap.minY;
		WidgetDebug.output = num2 + "/" + num3 + "\n";
		base.HitPoint.Set(num2, num3);
		base.HitPoint.Clamp();
		for (z = 0; z < screen.height; z++)
		{
			for (x = 0; x < screen.width; x++)
			{
				cx = screen.scrollX + x;
				cz = screen.scrollY + z;
				if (cx >= 0 && cz >= 0 && cx < Size && cz < Size)
				{
					DrawTile();
				}
			}
		}
		EMono.scene.sfxFire.SetVolume(Mathf.Clamp(0.1f * (float)totalFire + ((totalFire != 0) ? 0.2f : 0f), 0f, 1f));
		int valueOrDefault = (currentRoom?.lot?.idBGM).GetValueOrDefault();
		if ((valueOrDefault != 0 && (EMono.Sound.currentPlaylist != EMono.Sound.plLot || EMono.Sound.plLot.list[0].data.id != valueOrDefault)) || (valueOrDefault == 0 && EMono.Sound.currentPlaylist == EMono.Sound.plLot))
		{
			EMono._zone.RefreshBGM();
		}
		if (currentRoom != lastRoom)
		{
			screen.RefreshWeather();
			lastRoom = currentRoom;
		}
	}

	public override void DrawTile()
	{
		count++;
		index = cx + cz * Size;
		cell = (param.cell = map.cells[cx, cz]);
		detail = cell.detail;
		isSeen = cell.isSeen;
		roof = cell.HasRoof;
		matBlock = cell.matBlock;
		matFloor = cell.matFloor;
		sourceBlock = cell.sourceBlock;
		sourceFloor = cell.sourceFloor;
		light = (int)cell.light;
		float num = Fov.DistanceFloat(cx, cz, pcX, pcZ);
		if (num < 4f)
		{
			num = 4f;
		}
		if (light < pcMaxLight && num < (float)(EMono.player.lightRadius - 1))
		{
			float num2 = (light + pcMaxLight) / 2f * modSublight1 / num;
			if (num2 > light)
			{
				light = num2;
			}
		}
		_lowblock = lowBlock;
		liquidLv = (param.liquidLv = ((cell.liquidLv + cell._bridge != 0) ? cell.sourceBridge.tileType.LiquidLV : sourceFloor.tileType.LiquidLV) * 10);
		if (liquidLv > 99)
		{
			liquidLv = (param.liquidLv = 99);
		}
		height = cell.height;
		hasBridge = cell._bridge != 0;
		if (cell.room != null)
		{
			cell.room.lot.sync = true;
		}
		blockLight = _lightMod * lightLookUp[(int)light] + _baseBrightness + ((roof || cell.isShadowed) ? shadowStrength : 0f);
		CellEffect effect = cell.effect;
		if (effect != null && effect.FireAmount > 0)
		{
			blockLight += 0.2f;
			totalFire++;
		}
		if (blockLight > lightLimit)
		{
			blockLight = lightLimit;
		}
		blockLight -= 0.025f * (float)(int)cell.shadowMod * _heightMod.x;
		param.color = (blockLight = (int)(blockLight * 50f) * 262144 + ((cell.lightR >= 64) ? 63 : cell.lightR) * 4096 + ((cell.lightG >= 64) ? 63 : cell.lightG) * 64 + ((cell.lightB >= 64) ? 63 : cell.lightB));
		param.x = (float)cx * screen.tileAlign.x;
		param.y = (float)cz * screen.tileAlign.y + (float)height * _heightMod.y;
		param.z = 1000f + param.x * screen.tileWeight.x + param.y * screen.tileWeight.z + (float)height * _heightMod.z;
		if (detail != null)
		{
			TransAnime anime = detail.anime;
			if (anime != null && anime.animeBlock)
			{
				TransAnime anime2 = detail.anime;
				param.x += anime2.v.x;
				param.y += anime2.v.y;
				param.z += anime2.v.z;
			}
			if (detail.designation != null)
			{
				detail.designation.Draw(cx, cz, param);
			}
		}
		if (cell.isSlopeEdge)
		{
			float num3 = (float)height * _heightMod.y;
			orgY = param.y;
			orgZ = param.z;
			param.dir = cell.blockDir;
			if (cell.isSurrounded)
			{
				param.matColor = 0f;
				param.color = 262144 * (int)((_baseBrightness + fogBrightness) * 50f);
				param.tile = 0f;
				for (int i = 0; (float)i < num3 / heightBlockSize; i++)
				{
					param.y += ugFix.y;
					param.z += ugFix.z + slopeFixZ * (float)i;
					rendererInnerBlock.Draw(param);
				}
			}
			else
			{
				SourceBlock.Row defBlock;
				if (sourceBlock.tileType.IsFullBlock)
				{
					defBlock = sourceBlock;
					param.mat = matBlock;
					param.tile = sourceBlock._tiles[cell.blockDir % sourceBlock._tiles.Length];
					param.matColor = ((sourceBlock.colorMod == 0) ? 104025 : BaseTileMap.GetColorInt(ref matBlock.matColor, sourceBlock.colorMod));
				}
				else
				{
					defBlock = sourceFloor._defBlock;
					param.mat = matFloor;
					param.tile = defBlock._tiles[cell.blockDir % defBlock._tiles.Length];
					if (defBlock.id != 1)
					{
						param.matColor = ((sourceFloor.colorMod == 0) ? 104025 : BaseTileMap.GetColorInt(ref matFloor.matColor, sourceFloor.colorMod));
					}
					else
					{
						param.matColor = 104025f;
					}
				}
				for (int j = 0; (float)j < num3 / heightBlockSize; j++)
				{
					param.y += ugFix.y;
					param.z += ugFix.z + slopeFixZ * (float)j;
					defBlock.renderData.Draw(param);
				}
			}
			param.y = orgY;
			param.z = orgZ;
		}
		if (sourceBlock.id != 0)
		{
			base.tileType = sourceBlock.tileType;
			room = cell.room;
			_lowblock = (showFullWall ? (room != null) : lowBlock);
			if (base.tileType.RepeatBlock)
			{
				room = room ?? cell.Front.room ?? cell.Right.room ?? cell.FrontRight.room;
				if (room != null)
				{
					roomHeight = ((_lowblock && !base.tileType.ForceRpeatBlock) ? 0f : room.lot.realHeight);
					maxHeight = (float)(cz - cx) * screen.tileAlign.y + (float)room.lot.mh * _heightMod.y;
				}
				else
				{
					roomHeight = 0f;
				}
			}
			else
			{
				roomHeight = 0f;
			}
			param.mat = matBlock;
			param.dir = cell.blockDir;
			switch (base.tileType.blockRenderMode)
			{
			case BlockRenderMode.FullBlock:
				if (cell.isSurrounded)
				{
					switch (innerMode)
					{
					case InnerMode.InnerBlock:
					case InnerMode.BuildMode:
						param.matColor = 104025f;
						param.color = 262144 * (int)((_baseBrightness + fogBrightness) * 50f);
						param.tile = (_lowblock ? 3000000 : 0);
						rendererInnerBlock.Draw(param);
						return;
					case InnerMode.None:
					case InnerMode.Height:
						param.color = blockLight;
						break;
					}
				}
				param.tile = sourceBlock._tiles[cell.blockDir % sourceBlock._tiles.Length] + (_lowblock ? 3000000 : 0);
				param.matColor = ((sourceBlock.colorMod == 0) ? 104025 : BaseTileMap.GetColorInt(ref matBlock.matColor, sourceBlock.colorMod));
				if (roomHeight == 0f)
				{
					if (!cell.hasDoor)
					{
						sourceBlock.renderData.Draw(param);
					}
				}
				else
				{
					sourceBlock.renderData.DrawRepeatTo(param, maxHeight, roomHeight, ref renderSetting.peakFixBlock, cell.hasDoor, cell.effect?.FireAmount ?? 0);
				}
				break;
			case BlockRenderMode.WallOrFence:
			{
				orgY = param.y;
				orgZ = param.z;
				int blockDir = cell.blockDir;
				if (blockDir == 0 || blockDir == 2)
				{
					param.dir = 0;
					if (blockDir == 2 && cell.Left.sourceBlock.tileType.IsWallOrFence)
					{
						Cell left = cell.Left;
						_sourceBlock = left.sourceBlock;
						if (_sourceBlock.useAltColor)
						{
							param.matColor = ((_sourceBlock.colorMod == 0) ? 104025 : BaseTileMap.GetColorInt(ref param.mat.altColor, _sourceBlock.colorMod));
						}
						else
						{
							param.matColor = ((_sourceBlock.colorMod == 0) ? 104025 : BaseTileMap.GetColorInt(ref param.mat.matColor, _sourceBlock.colorMod));
						}
					}
					else
					{
						_sourceBlock = sourceBlock;
						if (_sourceBlock.useAltColor)
						{
							param.matColor = ((_sourceBlock.colorMod == 0) ? 104025 : BaseTileMap.GetColorInt(ref param.mat.altColor, _sourceBlock.colorMod));
						}
						else
						{
							param.matColor = ((_sourceBlock.colorMod == 0) ? 104025 : BaseTileMap.GetColorInt(ref param.mat.matColor, _sourceBlock.colorMod));
						}
					}
					param.tile = (tile = _sourceBlock._tiles[0] + (_lowblock ? 1000000 : 0));
					if (roomHeight == 0f || !base.tileType.RepeatBlock)
					{
						if (!cell.hasDoor)
						{
							_sourceBlock.renderData.Draw(param);
						}
					}
					else
					{
						_sourceBlock.renderData.DrawRepeatTo(param, maxHeight, roomHeight, ref renderSetting.peakFix, cell.hasDoor, cell.effect?.FireAmount ?? 0);
					}
					param.z -= 0.01f;
					if (blockDir == 2 || (cell.Front.HasWallOrFence && cell.Front.blockDir != 0))
					{
						param.tile = tile + 16;
						if (roomHeight == 0f || !base.tileType.RepeatBlock)
						{
							_sourceBlock.renderData.Draw(param);
						}
						else
						{
							_sourceBlock.renderData.DrawRepeatTo(param, maxHeight, roomHeight, ref renderSetting.peakFix);
						}
					}
					if (cell.Left.Front.HasWallOrFence && cell.Left.Front.blockDir == 1)
					{
						orgX = param.x;
						param.tile = tile + 16;
						param.x += cornerWallFix.x;
						param.y += cornerWallFix.y;
						param.z += cornerWallFix.z;
						if (roomHeight == 0f || !base.tileType.RepeatBlock)
						{
							_sourceBlock.renderData.Draw(param);
						}
						else
						{
							_sourceBlock.renderData.DrawRepeatTo(param, maxHeight, roomHeight + cornerWallFix.y, ref renderSetting.peakFix);
						}
						param.x = orgX;
					}
				}
				if (blockDir == 1 || blockDir == 2)
				{
					param.y = orgY;
					param.z = orgZ;
					param.dir = 1;
					if (blockDir == 2 && cell.Back.sourceBlock.tileType.IsWallOrFence)
					{
						Cell back = cell.Back;
						_sourceBlock = back.sourceBlock;
						if (_sourceBlock.useAltColor)
						{
							param.matColor = ((_sourceBlock.colorMod == 0) ? 104025 : BaseTileMap.GetColorInt(ref param.mat.altColor, _sourceBlock.colorMod));
						}
						else
						{
							param.matColor = ((_sourceBlock.colorMod == 0) ? 104025 : BaseTileMap.GetColorInt(ref param.mat.matColor, _sourceBlock.colorMod));
						}
					}
					else
					{
						_sourceBlock = sourceBlock;
						if (_sourceBlock.useAltColor)
						{
							param.matColor = ((_sourceBlock.colorMod == 0) ? 104025 : BaseTileMap.GetColorInt(ref param.mat.altColor, _sourceBlock.colorMod));
						}
						else
						{
							param.matColor = ((_sourceBlock.colorMod == 0) ? 104025 : BaseTileMap.GetColorInt(ref param.mat.matColor, _sourceBlock.colorMod));
						}
					}
					param.tile = (tile = -_sourceBlock._tiles[0] + (_lowblock ? (-1000000) : 0));
					if (roomHeight == 0f || !base.tileType.RepeatBlock)
					{
						if (!cell.hasDoor)
						{
							_sourceBlock.renderData.Draw(param);
						}
					}
					else
					{
						_sourceBlock.renderData.DrawRepeatTo(param, maxHeight, roomHeight, ref renderSetting.peakFix, cell.hasDoor, cell.effect?.FireAmount ?? 0);
					}
					if (cell.Right.HasWallOrFence && cell.Right.blockDir != 1)
					{
						orgX = param.x;
						param.tile = -tile + 16;
						if (roomHeight == 0f || !base.tileType.RepeatBlock)
						{
							_sourceBlock.renderData.Draw(param);
						}
						else
						{
							_sourceBlock.renderData.DrawRepeatTo(param, maxHeight, roomHeight, ref renderSetting.peakFix);
						}
						param.x = orgX;
					}
				}
				if (cell.room != null && !hideRoomFog && !cell.hasDoor)
				{
					return;
				}
				param.y = orgY;
				param.z = orgZ;
				break;
			}
			case BlockRenderMode.HalfBlock:
				_sourceBlock = ((sourceBlock.id == 5) ? EMono.sources.blocks.rows[matBlock.defBlock] : sourceBlock);
				param.tile = _sourceBlock._tiles[0];
				param.matColor = ((_sourceBlock.colorMod == 0) ? 104025 : BaseTileMap.GetColorInt(ref matBlock.matColor, _sourceBlock.colorMod));
				param.tile2 = _sourceBlock.sourceAutoFloor._tiles[0];
				param.halfBlockColor = ((_sourceBlock.sourceAutoFloor.colorMod == 0) ? 104025 : BaseTileMap.GetColorInt(ref matBlock.matColor, _sourceBlock.sourceAutoFloor.colorMod));
				sourceBlock.renderData.Draw(param);
				break;
			case BlockRenderMode.Pillar:
			{
				RenderData renderData = sourceBlock.renderData;
				param.tile = sourceBlock._tiles[cell.blockDir % sourceBlock._tiles.Length];
				param.matColor = ((sourceBlock.colorMod == 0) ? 104025 : BaseTileMap.GetColorInt(ref matBlock.matColor, sourceBlock.colorMod));
				int num4 = cell.objDir * 2 + 1;
				if (num4 == 0)
				{
					renderData.Draw(param);
				}
				else
				{
					renderData.DrawRepeat(param, num4, sourceBlock.tileType.RepeatSize);
				}
				param.tile = renderData.idShadow;
				SourcePref shadowPref = renderData.shadowPref;
				int shadow = shadowPref.shadow;
				passShadow.AddShadow(param.x + renderData.offsetShadow.x, param.y + renderData.offsetShadow.y, param.z + renderData.offsetShadow.z, ShadowData.Instance.items[shadow], shadowPref);
				break;
			}
			default:
				param.tile = sourceBlock._tiles[cell.blockDir % sourceBlock._tiles.Length] + ((_lowblock && base.tileType.UseLowBlock) ? 3000000 : 0);
				param.matColor = ((sourceBlock.colorMod == 0) ? 104025 : BaseTileMap.GetColorInt(ref matBlock.matColor, sourceBlock.colorMod));
				if (roomHeight == 0f)
				{
					sourceBlock.renderData.Draw(param);
				}
				else
				{
					sourceBlock.renderData.DrawRepeatTo(param, maxHeight, roomHeight, ref renderSetting.peakFixBlock);
				}
				break;
			}
		}
		if (cell.effect != null)
		{
			if (cell.effect.IsLiquid)
			{
				SourceCellEffect.Row sourceEffect = cell.sourceEffect;
				SourceMaterial.Row defaultMaterial = sourceEffect.DefaultMaterial;
				tile = 4 + Rand.bytes[index % Rand.MaxBytes] % 4;
				param.tile = tile + cell.sourceEffect._tiles[0];
				param.mat = defaultMaterial;
				param.matColor = BaseTileMap.GetColorInt(ref defaultMaterial.matColor, sourceEffect.colorMod);
				sourceEffect.renderData.Draw(param);
			}
			else
			{
				param.tile = cell.effect.source._tiles[0];
				if (cell.effect.IsFire)
				{
					rendererEffect.Draw(param);
				}
				else
				{
					cell.effect.source.renderData.Draw(param);
				}
			}
		}
		if (detail != null)
		{
			TransAnime anime3 = detail.anime;
			if (anime3 != null && !anime3.animeBlock)
			{
				TransAnime anime4 = detail.anime;
				param.x += anime4.v.x;
				param.y += anime4.v.y;
				param.z += anime4.v.z;
			}
		}
		if (cell.obj != 0)
		{
			SourceObj.Row sourceObj = cell.sourceObj;
			param.mat = cell.matObj;
			if (sourceObj.useAltColor)
			{
				param.matColor = ((sourceObj.colorMod == 0) ? 104025 : BaseTileMap.GetColorInt(ref param.mat.altColor, sourceObj.colorMod));
			}
			else
			{
				param.matColor = ((sourceObj.colorMod == 0) ? 104025 : BaseTileMap.GetColorInt(ref param.mat.matColor, sourceObj.colorMod));
			}
			if (sourceObj.HasGrowth)
			{
				cell.growth.OnRenderTileMap(param);
			}
			else
			{
				if (sourceObj.tileType.IsUseBlockDir)
				{
					param.tile = sourceObj._tiles[cell.blockDir % sourceObj._tiles.Length];
				}
				else
				{
					param.tile = sourceObj._tiles[cell.objDir % sourceObj._tiles.Length];
				}
				if (_lowblock && sourceObj.tileType.IsSkipLowBlock)
				{
					param.tile += ((param.tile > 0f) ? 1 : (-1)) * 3000000;
				}
				sourceObj.renderData.Draw(param);
				int shadow2 = sourceObj.pref.shadow;
				if (shadow2 > 1 && !cell.ignoreObjShadow)
				{
					passShadow.AddShadow(param.x + sourceObj.renderData.offsetShadow.x, param.y + sourceObj.renderData.offsetShadow.y, param.z + sourceObj.renderData.offsetShadow.z, ShadowData.Instance.items[shadow2], sourceObj.pref);
				}
			}
		}
		if (cell.decal != 0)
		{
			passDecal.Add(param, (int)cell.decal, blockLight);
		}
		if (detail == null)
		{
			return;
		}
		if (highlightArea && detail.area != null)
		{
			passArea.Add(param, (int)detail.area.GetTile(index) - ((!subtleHighlightArea) ? 1 : 0), 0f);
		}
		if (detail.footmark != null && sourceFloor.id != 0)
		{
			param.tile = detail.footmark.tile;
			param.mat = matFloor;
			param.matColor = floorMatColor;
			renderFootmark.Draw(param);
		}
		if (detail.things.Count == 0 && detail.charas.Count == 0)
		{
			return;
		}
		int num5 = 0;
		thingPos.x = 0f;
		thingPos.y = 0f;
		param.y += ((cell._bridge != 0) ? cell.sourceBridge.tileType.FloorHeight : sourceFloor.tileType.FloorHeight);
		orgX = param.x;
		orgY = param.y;
		orgZ = param.z;
		Thing thing = null;
		bool flag = liquidLv == 0;
		if (detail.things.Count > 0)
		{
			_ = zSetting.max1;
			for (int k = 0; k < detail.things.Count; k++)
			{
				Thing thing2 = detail.things[k];
				_actorPos.x = orgX;
				_actorPos.y = orgY;
				_actorPos.z = orgZ;
				SourcePref pref = thing2.sourceCard.pref;
				TileType tileType = thing2.trait.tileType;
				float num6 = (tileType.UseMountHeight ? 0f : ((pref.height == 0f) ? 0.1f : pref.height));
				if (tileType.CanStack || !thing2.IsInstalled)
				{
					if (thing?.id != thing2.id)
					{
						_actorPos.x += thingPos.x;
					}
					_actorPos.y += thingPos.y;
					_actorPos.z += renderSetting.thingZ + (float)k * -0.01f + zSetting.mod1 * thingPos.y;
				}
				if (thing2.IsInstalled && tileType.UseMountHeight)
				{
					if (hideHang && cell.room == null && thing2.altitude > 3)
					{
						continue;
					}
					thing2.TileType.GetMountHeight(ref _actorPos, Point.shared.Set(index), thing2.dir, thing2);
					flag = false;
				}
				else
				{
					flag = thingPos.y == 0f && liquidLv == 0;
				}
				param.liquidLv = ((k == 0) ? liquidLv : 0);
				thingPos.y += num6;
				if (!thing2.sourceCard.multisize || (thing2.pos.x == cx && thing2.pos.z == cz))
				{
					if (iconMode != 0)
					{
						int num7 = 0;
						switch (iconMode)
						{
						case CardIconMode.State:
							if (thing2.placeState == PlaceState.installed)
							{
								num7 = 18;
							}
							break;
						case CardIconMode.Deconstruct:
							if (thing2.isDeconstructing)
							{
								num7 = 14;
							}
							break;
						}
						if (thing2.isNPCProperty)
						{
							num7 = 13;
						}
						if (num7 != 0)
						{
							passGuideBlock.Add(_actorPos.x, _actorPos.y, _actorPos.z - 10f, num7);
						}
					}
					thing2.SetRenderParam(param);
					thing2.renderer.Draw(param, ref _actorPos, flag);
				}
				param.x = orgX;
				param.y = orgY;
				param.z = orgZ;
				thing = thing2;
			}
		}
		if (detail.charas.Count <= 0)
		{
			return;
		}
		param.shadowFix = 0f;
		param.color += 1310720f;
		float max = zSetting.max2;
		bool flag2 = EMono.pc.fov != null && EMono.pc.fov.lastPoints.ContainsKey(index);
		for (int l = 0; l < detail.charas.Count; l++)
		{
			Chara chara = detail.charas[l];
			if (chara.host != null || (!flag2 && chara.party?.leader != EMono.pc && !chara.isDead))
			{
				continue;
			}
			_actorPos.x = orgX;
			_actorPos.y = orgY;
			_actorPos.z = orgZ;
			chara.SetRenderParam(param);
			_ = chara.IsAliveInCurrentZone;
			if (chara.IsDeadOrSleeping && chara.IsPCC)
			{
				float num8 = chara.renderer.data.size.y * 0.3f;
				if (thingPos.y > max)
				{
					thingPos.y = max;
				}
				float num9 = thingPos.y + num8;
				float num10 = (float)l * -0.01f;
				if (num9 > zSetting.thresh1)
				{
					num10 = zSetting.mod1;
				}
				_actorPos.x += thingPos.x;
				_actorPos.y += thingPos.y;
				_actorPos.z += renderSetting.laydownZ + num10;
				param.liquidLv = ((thingPos.y == 0f && liquidLv > 0) ? 90 : 0);
				thingPos.y += num8 * 0.8f;
				chara.renderer.Draw(param, ref _actorPos, liquidLv == 0);
			}
			else
			{
				param.liquidLv = liquidLv;
				_actorPos.z += 0.01f * (float)l + renderSetting.charaZ;
				num5++;
				chara.renderer.Draw(param, ref _actorPos, liquidLv == 0);
			}
			param.x = orgX;
			param.y = orgY;
			param.z = orgZ;
		}
	}
}
