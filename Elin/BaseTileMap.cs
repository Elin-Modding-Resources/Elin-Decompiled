using System;
using NoiseSystem;
using UnityEngine;

public class BaseTileMap : EMono
{
	public enum WallClipMode
	{
		ByRoom,
		ByLot
	}

	public enum InnerMode
	{
		None,
		InnerBlock,
		Height,
		BuildMode
	}

	public enum CardIconMode
	{
		None,
		Inspect,
		Deconstruct,
		State,
		Visibility
	}

	public enum ScreenHighlight
	{
		None,
		SunMap
	}

	public static bool forceShowHang;

	public int count;

	[Header("Mouse Hit")]
	public Collider2D mouseCollider;

	public Vector3 colliderFix;

	public int maxColliderCheck;

	[Header("Height")]
	public float heightBlockSize;

	public float slopeFixZ;

	public float heightModDeco;

	public float heightLimitDeco;

	public Vector3 heightMod;

	public Vector3 ugFix;

	public Vector3 ugFixBridge;

	public Vector3 ugFixBridgeBottom;

	public Vector3 ugFixBridgeTop;

	public Vector3 heightModRoofBlock;

	[Header("Room")]
	public float rightWallShade;

	public Vector3 roofRampFix;

	public Vector3 roofFix;

	public Vector3 roofFix2;

	public Vector3 roofFix3;

	public float roofLightMod;

	public float lotLight;

	public float lotLight2;

	public float roofLightSnow;

	[Header("Other")]
	public WallClipMode wallClipMode;

	public InnerMode defaultInnerMode;

	public Vector3 edgeBlockFix;

	public Vector3 bridgeFix;

	public Vector3 cornerWallFix;

	public Vector3 cornerWallFix2;

	public Vector3 cornerWallFix3;

	public Vector3 altitudeFix;

	public Vector3 waterEdgeFix;

	public Vector3 waterEdgeBridgeFix;

	public Vector3 waterEdgeFixShore;

	public Vector3 waterEdgeFixShoreSand;

	public Vector3[] ambientShadowFix;

	public Vector3[] transitionFix;

	public Vector3[] wallHangFix;

	public Vector3[] waterEdgeBlockFix;

	public int fogTile;

	public float floatSpeed;

	public float shadowModStrength;

	public int maxFloat;

	public int[] seaAnimeIndexes;

	[Header("References")]
	public NoiseLayer layerGroundLights;

	public MeshPass passShadow;

	public MeshPass passLiquid;

	public MeshPass passGuideBlock;

	public MeshPass passGuideFloor;

	public MeshPass passArea;

	public MeshPass passRamp;

	public MeshPass passFloor;

	public MeshPass passBlock;

	public MeshPass passObjS;

	public MeshPass passObjSS;

	public MeshPass passObj;

	public MeshPass passObjL;

	public MeshPass passDecal;

	public MeshPass passRoof;

	public MeshPass passBlockEx;

	public MeshPass passFloorEx;

	public MeshPass passFloorWater;

	public MeshPass passInner;

	public MeshPass passFog;

	public MeshPass passFov;

	public MeshPass passEdge;

	public MeshPass passAutoTile;

	public MeshPass passAutoTileWater;

	public MeshPass passBlockMarker;

	public MeshPass passFloorMarker;

	public MeshPass passWaterBlock;

	public MeshPass passIcon;

	public MeshPass passChara;

	public MeshPass passCharaL;

	public MeshPass passCharaLL;

	public MeshPass passShore;

	public RenderData renderFootmark;

	public RenderData rendererBlockMarker;

	public RenderData rendererFloorMarker;

	public RenderData rendererInnerBlock;

	public RenderData rendererFov;

	public RenderData rendererFov2;

	public RenderData rendererShore;

	public RenderData renderBorder;

	public RenderData rendererFogBlockSolid;

	public RenderData rendererFogFloorSolid;

	public RenderData rendererFogRoomSolid;

	public RenderData rendererFogRoomBlockSolid;

	public RenderData rendererFogRoomWallSolid;

	public RenderData rendererWallDeco;

	public RenderData rendererWaterBlock;

	public RenderDataObjDummy rendererObjDummy;

	public RenderDataEffect rendererEffect;

	public Point TestPoint = new Point();

	protected InnerMode innerMode;

	[NonSerialized]
	public int Size;

	[NonSerialized]
	public int SizeXZ;

	[NonSerialized]
	public int mx;

	[NonSerialized]
	public int mz;

	[NonSerialized]
	public int x;

	[NonSerialized]
	public int z;

	[NonSerialized]
	public int cx;

	[NonSerialized]
	public int cz;

	[NonSerialized]
	public int activeCount;

	[NonSerialized]
	public int floatV = 1;

	[NonSerialized]
	public byte[] groundLights;

	[NonSerialized]
	public bool lowBlock;

	[NonSerialized]
	public bool lowObj;

	[NonSerialized]
	public bool highlightArea;

	[NonSerialized]
	public bool subtleHighlightArea;

	[NonSerialized]
	public bool hideRoomFog;

	[NonSerialized]
	public bool showRoof;

	[NonSerialized]
	public bool showFullWall;

	[NonSerialized]
	public bool hideHang;

	[NonSerialized]
	public bool usingHouseBoard;

	[NonSerialized]
	public bool noRoofMode;

	[NonSerialized]
	public bool fogged;

	[NonSerialized]
	public float[] lightLookUp;

	[NonSerialized]
	public float _lightMod;

	[NonSerialized]
	public float _baseBrightness;

	[NonSerialized]
	public float lowblockTimer;

	[NonSerialized]
	public float heightLightMod;

	[NonSerialized]
	public float _rightWallShade;

	[NonSerialized]
	public float roofLightLimitMod;

	[NonSerialized]
	public float floatY;

	[NonSerialized]
	public float floorShadowStrength;

	[NonSerialized]
	public Vector3 _heightMod;

	[NonSerialized]
	public ScreenHighlight screenHighlight;

	protected RaycastHit2D[] rays = new RaycastHit2D[1];

	protected CardIconMode iconMode;

	protected bool isMining;

	protected bool buildMode;

	protected bool hasBridge;

	protected bool _lowblock;

	protected bool isIndoor;

	public new BaseGameScreen screen;

	protected Map map;

	protected MeshPass pass;

	protected BaseTileSelector selector;

	protected GameSetting.RenderSetting renderSetting;

	protected GameSetting.RenderSetting.ZSetting zSetting;

	protected int liquidLv;

	protected int index;

	protected int totalFire;

	protected int snowColorToken;

	protected int waterAnimeIndex;

	protected int lowWallObjAltitude;

	protected SourceMaterial.Row matBlock;

	protected SourceMaterial.Row matFloor;

	protected SourceMaterial.Row matBridge;

	protected float blockLight;

	protected float floorLight;

	protected float floorLight2;

	protected float light;

	protected float pcMaxLight;

	protected float orgX;

	protected float orgY;

	protected float orgZ;

	protected float roomHeight;

	protected float maxHeight;

	protected float snowLight;

	protected float waterAnimeTimer;

	protected float floatTimer;

	protected float destBrightness;

	protected float lightLimit;

	protected float modSublight1;

	protected float modSublight2;

	protected float shadowStrength;

	protected float _shadowStrength;

	protected float fogBrightness;

	protected float defaultBlockHeight;

	protected float snowLimit;

	protected float snowColor;

	protected float snowColor2;

	protected float nightRatio;

	protected RenderParam param = new RenderParam();

	protected MeshBatch batch;

	protected Vector3 _actorPos;

	protected Vector3 freePos;

	protected int tile;

	protected int floorMatColor;

	protected int height;

	protected int currentHeight;

	protected int pcX;

	protected int pcZ;

	protected int floorDir;

	protected bool roof;

	protected bool isSeen;

	protected bool showAllCards;

	protected bool fogBounds;

	protected bool snowed;

	protected bool isSnowCovered;

	protected bool highlightCells;

	protected bool cinemaMode;

	protected bool alwaysLowblock;

	protected bool showBorder;

	protected Vector3 thingPos;

	protected Vector3 orgPos;

	protected Cell cell;

	protected CellDetail detail;

	protected SourceBlock.Row sourceBlock;

	protected SourceFloor.Row sourceFloor;

	protected SourceFloor.Row sourceBridge;

	protected Room currentRoom;

	protected Room lastRoom;

	protected Room room;

	protected Lot currentLot;

	protected SourceBlock.Row _sourceBlock;

	protected TileType tileType;

	protected SceneLightProfile lightSetting;

	private bool noSlopMode;

	public RoofStyle[] roofStyles;

	public const int DefColor = 104025;

	public const int BlocklightToken = 262144;

	public const int BlocklightMTP = 50;

	public Point HitPoint => Scene.HitPoint;

	public void OnActivate(BaseGameScreen _screen)
	{
		screen = _screen;
		if (lightLookUp == null)
		{
			lightLookUp = new float[256];
			for (int i = 0; i < 256; i++)
			{
				lightLookUp[i] = EMono.scene.profile.global.lightLookupCurve.Evaluate((float)i / 255f);
			}
		}
		selector = screen.tileSelector;
		renderSetting = EMono.setting.render;
		zSetting = renderSetting.zSetting;
		RenderObject.syncList = EMono.scene.syncList;
		RenderObject.altitudeFix = altitudeFix.y;
		if (Rand.bytes == null)
		{
			Rand.InitBytes(1);
		}
		RefreshHeight();
	}

	public virtual void Draw()
	{
		Zone zone = EMono._zone;
		map = zone.map;
		Size = map.Size;
		SizeXZ = map.SizeXZ;
		isIndoor = EMono._map.IsIndoor;
		count = 0;
		totalFire = 0;
		pcX = EMono.pc.pos.x;
		pcZ = EMono.pc.pos.z;
		_ = EMono.scene.camSupport.Zoom;
		SceneProfile profile = EMono.scene.profile;
		lightSetting = profile.light;
		buildMode = EMono.scene.actionMode.IsBuildMode;
		if (ActionMode.ViewMap.IsActive)
		{
			buildMode = false;
		}
		cinemaMode = ActionMode.Cinema.IsActive;
		isMining = EMono.scene.actionMode == ActionMode.Mine;
		iconMode = EMono.scene.actionMode.cardIconMode;
		showAllCards = buildMode;
		highlightCells = ActionMode.FlagCell.IsActive;
		subtleHighlightArea = EMono.scene.actionMode.AreaHihlight != 0 && (EMono.scene.actionMode.AreaHihlight == AreaHighlightMode.Build || EMono.scene.actionMode.AreaHihlight != AreaHighlightMode.Edit);
		highlightArea = EMono.scene.actionMode.AreaHihlight == AreaHighlightMode.Edit || subtleHighlightArea;
		nightRatio = profile.light.nightRatioCurve.Evaluate(EMono.scene.timeRatio);
		_rightWallShade = (int)(50f * rightWallShade) * 262144;
		alwaysLowblock = EMono._zone.AlwaysLowblock;
		lowWallObjAltitude = EMono.game.config.lowWallObjAltitude;
		showBorder = (EMono.core.config.game.showBorder == 1 && EMono._zone is Zone_Field && !EMono._zone.IsPlayerFaction) || (EMono.core.config.game.showBorder == 2 && !EMono._zone.map.IsIndoor && !EMono._zone.IsSkyLevel && !EMono._zone.IsRegion);
		snowLimit = profile.global.snowLimit.Evaluate(EMono.scene.timeRatio);
		snowLight = profile.global.snowLight;
		snowColor = profile.global.snowColor;
		snowColor2 = profile.global.snowColor2;
		snowColorToken = (int)((float)profile.global.snowRGB.x * nightRatio) * 4096 + (int)((float)profile.global.snowRGB.y * nightRatio) * 64 + (int)((float)profile.global.snowRGB.z * nightRatio);
		isSnowCovered = zone.IsSnowCovered;
		if (EMono.game.config.reverseSnow && buildMode)
		{
			isSnowCovered = !isSnowCovered;
		}
		roofLightLimitMod = profile.global.roofLightLimitMod.Evaluate(EMono.scene.timeRatio);
		fogBounds = (EMono._map.config.forceHideOutbounds && (!EMono.debug.godBuild || !buildMode)) || (EMono.core.config.game.dontRenderOutsideMap && !buildMode);
		heightLightMod = ((EMono._map.config.heightLightMod == 0f) ? 0.005f : EMono._map.config.heightLightMod);
		modSublight1 = profile.global.modSublight1;
		modSublight2 = profile.global.modSublight2 * nightRatio;
		pcMaxLight = EMono.player.lightPower * 2f * (float)(EMono.player.lightRadius - 2);
		fogBrightness = lightSetting.fogBrightness.Evaluate(EMono.scene.timeRatio);
		lightLimit = lightSetting.lightLimit.Evaluate(EMono.scene.timeRatio);
		_lightMod = lightSetting.lightMod * lightSetting.lightModCurve.Evaluate(EMono.scene.timeRatio);
		destBrightness = lightSetting.baseBrightness * lightSetting.baseBrightnessCurve.Evaluate(EMono.scene.timeRatio);
		float num = destBrightness;
		destBrightness = destBrightness * lightSetting.dynamicBrightnessCurve2.Evaluate(EMono.scene.timeRatio) + lightSetting.dynamicBrightnessCurve.Evaluate(EMono.scene.timeRatio) * (float)(int)EMono.pc.pos.cell.light + ((EMono.pc.pos.cell.light == 0) ? 0f : lightSetting.dynamicBrightnessLightBonus);
		if (destBrightness > num)
		{
			destBrightness = num;
		}
		if (!Mathf.Approximately(_baseBrightness, destBrightness))
		{
			_baseBrightness = Mathf.Lerp(_baseBrightness, destBrightness, Core.delta * lightSetting.dynamicBrightnessSpeed);
		}
		if (activeCount == 0)
		{
			_baseBrightness = destBrightness;
		}
		activeCount++;
		if (buildMode && EMono.game.config.buildLight)
		{
			_baseBrightness = 0.7f;
		}
		Fov.nonGradientMod = profile.global.fovModNonGradient;
		shadowStrength = lightSetting.shadowCurve.Evaluate(EMono.scene.timeRatio);
		floorShadowStrength = lightSetting.shadowCurveFloor.Evaluate(EMono.scene.timeRatio);
		float num2 = Core.delta * (float)EMono.game.config.animeSpeed * 0.01f;
		floatTimer += num2;
		if (floatTimer > floatSpeed)
		{
			floatTimer -= floatSpeed;
			floatY += floatV;
			if (floatY >= (float)maxFloat)
			{
				floatV *= -1;
			}
			if (floatY < 0f)
			{
				floatV *= -1;
			}
		}
		waterAnimeTimer += num2;
		if (waterAnimeTimer > 0.5f)
		{
			waterAnimeTimer = 0f;
			waterAnimeIndex++;
		}
		if (cinemaMode)
		{
			CinemaConfig cinemaConfig = EMono.player.cinemaConfig;
			profile = ActionMode.Cinema.profile;
			lightSetting = profile.light;
			lightLimit = lightSetting.lightLimit.Evaluate(EMono.scene.timeRatio);
			_lightMod += lightSetting.lightMod * lightSetting.lightModCurve.Evaluate(EMono.scene.timeRatio) * (0.01f * (float)cinemaConfig.light);
			destBrightness += 0.01f * (float)cinemaConfig.brightness;
			EMono.scene.camSupport.grading.cinemaBrightness = 0.01f * (float)cinemaConfig.brightness;
			EMono.scene.camSupport.grading.SetGrading();
			heightLightMod = 0f;
		}
		map.rooms.Refresh();
		noSlopMode = EInput.isAltDown && EInput.isShiftDown;
		RefreshHeight();
		innerMode = ((!buildMode && isIndoor) ? defaultInnerMode : InnerMode.None);
		if (EMono.pc.IsInActiveZone)
		{
			currentHeight = EMono.pc.pos.cell.TopHeight;
			currentRoom = EMono.pc.pos.cell.room;
		}
		else
		{
			currentHeight = 0;
			currentRoom = null;
		}
		lowObj = false;
		defaultBlockHeight = map.config.blockHeight;
		noRoofMode = false;
		bool flag = !isIndoor || EMono._zone is Zone_Tent;
		if (usingHouseBoard || ActionMode.Bird.IsActive)
		{
			lowBlock = (hideRoomFog = (hideHang = false));
			showRoof = (showFullWall = flag);
			if (ActionMode.Bird.IsActive)
			{
				fogBounds = false;
			}
		}
		else if (buildMode)
		{
			defaultBlockHeight = 0f;
			if (HitPoint.IsValid)
			{
				currentRoom = HitPoint.cell.room;
			}
			hideRoomFog = true;
			showRoof = flag && EMono.game.config.showRoof;
			showFullWall = showRoof || EMono.game.config.showWall;
			lowBlock = !showFullWall;
			hideHang = !showFullWall;
			if (cinemaMode)
			{
				hideRoomFog = !showRoof;
			}
		}
		else if (ActionMode.IsAdv)
		{
			noRoofMode = EMono.game.config.noRoof || screen.focusOption != null;
			if (EMono.pc.pos.cell.Front.UseLowBlock || EMono.pc.pos.cell.Right.UseLowBlock || EMono.pc.pos.cell.Front.Right.UseLowBlock || EMono.pc.pos.cell.UseLowBlock || (EMono.pc.pos.cell.Front.Right.isWallEdge && EMono.pc.pos.cell.Front.Right.Right.UseLowBlock))
			{
				if (!EMono.pc.IsMoving)
				{
					lowblockTimer = 0.1f;
				}
			}
			else if (!EInput.rightMouse.pressing)
			{
				lowblockTimer = 0f;
			}
			x = EMono.pc.pos.x;
			z = EMono.pc.pos.z;
			Room room = null;
			if (room != null)
			{
				currentRoom = room;
			}
			if (currentRoom != null)
			{
				currentRoom.data.visited = true;
			}
			if (room != null)
			{
				room.data.visited = true;
			}
			lowBlock = lowblockTimer > 0f;
			hideRoomFog = currentRoom != null && (currentRoom.HasRoof || isIndoor);
			if (hideRoomFog)
			{
				lowBlock = true;
			}
			if (noRoofMode && (currentRoom == null || currentRoom.lot.idRoofStyle == 0))
			{
				hideRoomFog = true;
				showRoof = (showFullWall = false);
			}
			else
			{
				showRoof = (showFullWall = flag && !lowBlock && !hideRoomFog);
			}
			hideHang = lowBlock;
			EMono.game.config.showRoof = !hideRoomFog;
			if (forceShowHang)
			{
				hideHang = false;
				forceShowHang = false;
			}
		}
		else
		{
			lowBlock = (hideRoomFog = (hideHang = false));
			showRoof = (showFullWall = true);
		}
		currentLot = currentRoom?.lot ?? null;
		Vector3 mposWorld = EInput.mposWorld;
		mposWorld.z = 0f;
		Vector3Int vector3Int = screen.grid.WorldToCell(mposWorld);
		mx = -vector3Int.y;
		mz = vector3Int.x - 1;
		HitPoint.Set(mx, mz);
		bool isAltDown = EInput.isAltDown;
		for (int num3 = maxColliderCheck; num3 >= 0; num3--)
		{
			TestPoint.x = mx + num3;
			TestPoint.z = mz - num3;
			if (TestPoint.x >= 0 && TestPoint.x < Size && TestPoint.z >= 0 && TestPoint.z < Size)
			{
				mouseCollider.transform.position = (isAltDown ? TestPoint.Position(TestPoint.cell.height) : TestPoint.Position()) + colliderFix;
				Physics2D.SyncTransforms();
				if (Physics2D.RaycastNonAlloc(mposWorld, Vector2.zero, rays) > 0)
				{
					HitPoint.Set(TestPoint);
					break;
				}
			}
			TestPoint.x = mx + num3 - 1;
			TestPoint.z = mz - num3;
			if (TestPoint.x >= 0 && TestPoint.x < Size && TestPoint.z >= 0 && TestPoint.z < Size)
			{
				mouseCollider.transform.position = (isAltDown ? TestPoint.Position(TestPoint.cell.height) : TestPoint.Position()) + colliderFix;
				Physics2D.SyncTransforms();
				if (Physics2D.RaycastNonAlloc(mposWorld, Vector2.zero, rays) > 0)
				{
					HitPoint.Set(TestPoint);
					break;
				}
			}
			TestPoint.x = mx + num3;
			TestPoint.z = mz - num3 + 1;
			if (TestPoint.x >= 0 && TestPoint.x < Size && TestPoint.z >= 0 && TestPoint.z < Size)
			{
				mouseCollider.transform.position = (isAltDown ? TestPoint.Position(TestPoint.cell.height) : TestPoint.Position()) + colliderFix;
				Physics2D.SyncTransforms();
				if (Physics2D.RaycastNonAlloc(mposWorld, Vector2.zero, rays) > 0)
				{
					HitPoint.Set(TestPoint);
					break;
				}
			}
		}
		HitPoint.Clamp();
		bool flag2 = EMono._zone.UseFog && !EMono.scene.bg.wall;
		for (z = 0; z < screen.height; z++)
		{
			for (x = 0; x < screen.width; x++)
			{
				cx = screen.scrollX - screen.scrollY + x - (z + 1) / 2;
				cz = screen.scrollY + screen.scrollX + x + z / 2;
				if (((cz < 0 && cx >= -cz && cx > 0 && cx < Size - cz) || (cx >= Size && cx < Size * 2 - cz - 1 && cz >= -cx && cz < Size - 1)) && EMono.scene.bg.wall)
				{
					param.x = (float)(cx + cz) * screen.tileAlign.x + edgeBlockFix.x;
					param.y = (float)(cz - cx) * screen.tileAlign.y + edgeBlockFix.y;
					param.z = 1000f + param.x * screen.tileWeight.x + param.y * screen.tileWeight.z + (float)z * edgeBlockFix.z;
					blockLight = 10485760f;
					pass = passInner;
					batch = pass.batches[pass.batchIdx];
					batch.matrices[pass.idx].m03 = param.x;
					batch.matrices[pass.idx].m13 = param.y;
					batch.matrices[pass.idx].m23 = param.z;
					batch.tiles[pass.idx] = 4 + ((cx >= Size) ? 1 : 0);
					batch.colors[pass.idx] = blockLight;
					batch.matColors[pass.idx] = 104025f;
					pass.idx++;
					if (pass.idx == pass.batchSize)
					{
						pass.NextBatch();
					}
				}
				else if (cx < 0 || cz < 0 || cx >= Size || cz >= Size)
				{
					if (flag2)
					{
						param.x = (float)(cx + cz) * screen.tileAlign.x;
						param.y = (float)(cz - cx) * screen.tileAlign.y + (float)height * _heightMod.y;
						param.z = 1000f + param.x * screen.tileWeight.x + param.y * screen.tileWeight.z + (float)height * _heightMod.z;
						param.tile = 0f;
						rendererFov2.Draw(param);
					}
				}
				else
				{
					DrawTile();
				}
			}
		}
		if (showRoof)
		{
			foreach (Lot item in map.rooms.listLot)
			{
				if (item.sync)
				{
					DrawRoof(item);
					item.sync = false;
					item.light = 0f;
				}
			}
		}
		else
		{
			foreach (Lot item2 in map.rooms.listLot)
			{
				item2.sync = false;
				item2.light = 0f;
			}
		}
		EMono.scene.sfxFire.SetVolume(Mathf.Clamp(0.1f * (float)totalFire + ((totalFire != 0) ? 0.2f : 0f), 0f, 1f));
		this.room = EMono.pc.pos.cell.room;
		int valueOrDefault = (this.room?.lot?.idBGM).GetValueOrDefault();
		if (valueOrDefault == 0)
		{
			goto IL_1671;
		}
		if (!(EMono.Sound.currentPlaylist != EMono.Sound.plLot))
		{
			BGMData data = EMono.Sound.plLot.list[0].data;
			if ((object)data != null && data.id == valueOrDefault)
			{
				goto IL_1671;
			}
		}
		goto IL_1690;
		IL_169a:
		if (this.room != lastRoom)
		{
			screen.RefreshWeather();
			lastRoom = this.room;
		}
		SoundManager.bgmVolumeMod = ((!LayerDrama.maxBGMVolume && !EMono._map.IsIndoor && this.room != null && !this.room.data.atrium && this.room.HasRoof) ? (-0.6f) : 0f);
		screenHighlight = ScreenHighlight.None;
		return;
		IL_1690:
		EMono._zone.RefreshBGM();
		goto IL_169a;
		IL_1671:
		if (valueOrDefault == 0 && EMono.Sound.currentPlaylist == EMono.Sound.plLot)
		{
			goto IL_1690;
		}
		goto IL_169a;
	}

	public void RefreshHeight()
	{
		if (EMono.game != null)
		{
			float num = (((buildMode && !EMono.game.config.slope) || noSlopMode) ? 0f : ((float)EMono.game.config.slopeMod * 0.01f));
			_heightMod = heightMod;
			_heightMod.x = num;
			_heightMod.y *= num;
			_heightMod.z *= num;
		}
	}

	public virtual void DrawTile()
	{
		count++;
		index = cx + cz * Size;
		this.cell = (param.cell = map.cells[cx, cz]);
		detail = this.cell.detail;
		isSeen = this.cell.isSeen;
		this.room = this.cell.room;
		roof = this.cell.HasRoof;
		matBlock = this.cell.matBlock;
		sourceBlock = this.cell.sourceBlock;
		bool flag = this.cell.isFloating;
		snowed = isSnowCovered && !roof && !this.cell.isClearSnow && !this.cell.isDeck && !flag;
		_shadowStrength = (snowed ? (shadowStrength * 0.4f) : shadowStrength);
		floorDir = this.cell.floorDir;
		if (snowed && !this.cell.sourceFloor.ignoreSnow)
		{
			if (this.cell.IsFloorWater)
			{
				sourceFloor = FLOOR.sourceIce;
				matFloor = this.cell.matFloor;
			}
			else
			{
				if (this.cell.sourceObj.snowTile > 0)
				{
					sourceFloor = FLOOR.sourceSnow2;
					floorDir = this.cell.sourceObj.snowTile - 1;
				}
				else if (index % 3 == 0 && Rand.bytes[index % Rand.MaxBytes] < 8 && !this.cell.HasObj && this.cell.FirstThing == null)
				{
					sourceFloor = FLOOR.sourceSnow2;
					floorDir = Rand.bytes[index % Rand.MaxBytes];
				}
				else
				{
					sourceFloor = FLOOR.sourceSnow;
				}
				matFloor = MATERIAL.sourceSnow;
			}
		}
		else
		{
			sourceFloor = this.cell.sourceFloor;
			matFloor = this.cell.matFloor;
		}
		bool isWater = sourceFloor.tileType.IsWater;
		light = (this.cell.pcSync ? this.cell.light : (this.cell.light / 3 * 2));
		_lowblock = lowBlock;
		height = this.cell.height;
		hasBridge = this.cell._bridge != 0;
		blockLight = _lightMod * lightLookUp[(int)light] + _baseBrightness + ((roof || this.cell.isShadowed) ? _shadowStrength : 0f);
		if (hasBridge)
		{
			if (this.cell.bridgeHeight < currentHeight)
			{
				blockLight -= heightLightMod * (float)(currentHeight - this.cell.bridgeHeight);
			}
			if (snowed && !this.cell.sourceBridge.ignoreSnow)
			{
				if (this.cell.IsBridgeWater)
				{
					sourceBridge = FLOOR.sourceIce;
					matBridge = this.cell.matBridge;
				}
				else
				{
					sourceBridge = FLOOR.sourceSnow;
					matBridge = MATERIAL.sourceSnow;
				}
			}
			else
			{
				sourceBridge = this.cell.sourceBridge;
				matBridge = this.cell.matBridge;
			}
		}
		else if (height < currentHeight)
		{
			blockLight -= heightLightMod * (float)(currentHeight - height);
		}
		liquidLv = (param.liquidLv = ((!flag) ? (((this.cell.liquidLv + this.cell._bridge != 0) ? this.cell.sourceBridge.tileType.LiquidLV : sourceFloor.tileType.LiquidLV) * 10) : 0));
		if (this.cell.shore != 0 && liquidLv > 20)
		{
			liquidLv = (param.liquidLv = 20);
		}
		if (liquidLv > 99)
		{
			liquidLv = (param.liquidLv = 99);
		}
		CellEffect effect = this.cell.effect;
		if (effect != null && effect.IsFire)
		{
			blockLight += 0.2f;
			totalFire++;
		}
		if (blockLight > lightLimit)
		{
			blockLight = lightLimit;
		}
		blockLight += shadowModStrength * (float)(int)this.cell.shadowMod * _heightMod.x * _shadowStrength;
		if (this.room != null)
		{
			this.room.lot.sync = true;
			if (this.room.lot.light < blockLight)
			{
				this.room.lot.light = blockLight;
			}
		}
		if (currentLot != null && currentLot.idRoofStyle != 0)
		{
			bool flag2 = this.cell.IsRoomEdge && (this.cell.Front.room?.lot == currentLot || this.cell.Right.room?.lot == currentLot || this.cell.FrontRight.room?.lot == currentLot);
			if (!buildMode)
			{
				if ((this.room != null && this.room.lot == currentLot) || flag2)
				{
					blockLight += lotLight;
				}
				else
				{
					blockLight += (isSnowCovered ? (-0.02f) : lotLight2);
				}
			}
		}
		if (this.cell.outOfBounds && !cinemaMode)
		{
			blockLight -= 0.1f;
			if (fogBounds)
			{
				isSeen = false;
			}
		}
		floorLight = blockLight;
		param.color = (blockLight = (int)(blockLight * 50f) * 262144 + ((this.cell.lightR >= 64) ? 63 : this.cell.lightR) * 4096 + ((this.cell.lightG >= 64) ? 63 : this.cell.lightG) * 64 + ((this.cell.lightB >= 64) ? 63 : this.cell.lightB));
		if (snowed)
		{
			floorLight += snowLight;
			if (floorLight > lightLimit * snowLimit)
			{
				floorLight = lightLimit * snowLimit;
			}
			floorLight = (int)(floorLight * 50f) * 262144 + (int)((float)((this.cell.lightR >= 50) ? 50 : this.cell.lightR) * snowColor) * 4096 + (int)((float)((this.cell.lightG >= 50) ? 50 : this.cell.lightG) * snowColor) * 64 + (int)((float)((this.cell.lightB >= 50) ? 50 : this.cell.lightB) * snowColor) + snowColorToken;
		}
		else if (isSnowCovered && !roof)
		{
			floorLight = (int)(floorLight * 50f) * 262144 + (int)((float)((this.cell.lightR >= 50) ? 50 : this.cell.lightR) * snowColor2) * 4096 + (int)((float)((this.cell.lightG >= 50) ? 50 : this.cell.lightG) * snowColor2) * 64 + (int)((float)((this.cell.lightB >= 50) ? 50 : this.cell.lightB) * snowColor2) + snowColorToken;
		}
		else
		{
			floorLight = blockLight;
		}
		bool num = this.room != null && sourceBlock.tileType.CastShadowSelf && !this.cell.hasDoor;
		bool flag3 = this.room != null && showRoof && this.room.lot.idRoofStyle != 0 && !this.room.data.atrium && !sourceBlock.tileType.Invisible;
		if (flag3 && this.cell.hasDoor && this.cell.IsLotEdge)
		{
			flag3 = false;
		}
		if (num || !isSeen || flag3)
		{
			floorLight += -3145728f;
		}
		if (this.cell.isWatered && !snowed)
		{
			floorLight += -2359296f;
		}
		param.snow = false;
		param.x = (float)(cx + cz) * screen.tileAlign.x;
		param.y = (float)(cz - cx) * screen.tileAlign.y + (float)height * _heightMod.y;
		param.z = 1000f + param.x * screen.tileWeight.x + param.y * screen.tileWeight.z + (float)height * _heightMod.z;
		if (flag)
		{
			param.y += 0.01f * floatY;
		}
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
		if (screenHighlight != 0 && screenHighlight == ScreenHighlight.SunMap && Map.sunMap.Contains(index))
		{
			screen.guide.passGuideFloor.Add(param.x, param.y, param.z - 10f, 11f);
		}
		if (this.cell._roofBlock != 0 && (isSeen || !EMono._zone.UseFog) && showRoof && !lowBlock)
		{
			SourceBlock.Row row = Cell.blockList[this.cell._roofBlock];
			SourceMaterial.Row row2 = Cell.matList[this.cell._roofBlockMat];
			this.tileType = row.tileType;
			param.mat = row2;
			param.dir = this.cell._roofBlockDir % 4;
			param.snow = isSnowCovered;
			orgX = param.x;
			orgY = param.y;
			orgZ = param.z;
			SetRoofHeight(param, this.cell, cx, cz, 0, this.cell._roofBlockDir / 4, this.tileType.IsWallOrFence ? param.dir : (-1));
			switch (this.tileType.blockRenderMode)
			{
			case BlockRenderMode.FullBlock:
				param.color -= (int)(_shadowStrength * 50f) * 262144;
				param.tile = row._tiles[param.dir % row._tiles.Length];
				param.matColor = ((row.colorMod == 0) ? 104025 : GetColorInt(ref row2.matColor, row.colorMod));
				row.renderData.Draw(param);
				break;
			case BlockRenderMode.WallOrFence:
			{
				_lowblock = true;
				int dir = param.dir;
				if (dir == 0 || dir == 2)
				{
					param.dir = 0;
					_sourceBlock = row;
					this.tileType = _sourceBlock.tileType;
					if (_sourceBlock.useAltColor)
					{
						param.matColor = ((_sourceBlock.colorMod == 0) ? 104025 : GetColorInt(ref param.mat.altColor, _sourceBlock.colorMod));
					}
					else
					{
						param.matColor = ((_sourceBlock.colorMod == 0) ? 104025 : GetColorInt(ref param.mat.matColor, _sourceBlock.colorMod));
					}
					param.tile = (tile = _sourceBlock._tiles[0] + ((_lowblock && !this.tileType.IsFence) ? 32 : 0));
					_sourceBlock.renderData.Draw(param);
					param.z -= 0.01f;
				}
				if (dir == 1 || dir == 2)
				{
					param.dir = 1;
					_sourceBlock = row;
					this.tileType = _sourceBlock.tileType;
					if (_sourceBlock.useAltColor)
					{
						param.matColor = ((_sourceBlock.colorMod == 0) ? 104025 : GetColorInt(ref param.mat.altColor, _sourceBlock.colorMod));
					}
					else
					{
						param.matColor = ((_sourceBlock.colorMod == 0) ? 104025 : GetColorInt(ref param.mat.matColor, _sourceBlock.colorMod));
					}
					param.tile = (tile = -_sourceBlock._tiles[0] + ((_lowblock && !this.tileType.IsFence) ? (-32) : 0));
					_sourceBlock.renderData.Draw(param);
				}
				break;
			}
			case BlockRenderMode.HalfBlock:
				_sourceBlock = ((row.id == 5) ? EMono.sources.blocks.rows[row2.defBlock] : row);
				param.tile = _sourceBlock._tiles[0];
				param.matColor = ((_sourceBlock.colorMod == 0) ? 104025 : GetColorInt(ref row2.matColor, _sourceBlock.colorMod));
				param.tile2 = _sourceBlock.sourceAutoFloor._tiles[0];
				param.halfBlockColor = ((_sourceBlock.sourceAutoFloor.colorMod == 0) ? 104025 : GetColorInt(ref row2.matColor, _sourceBlock.sourceAutoFloor.colorMod));
				row.renderData.Draw(param);
				break;
			case BlockRenderMode.Pillar:
			{
				RenderData renderData = row.renderData;
				param.tile = row._tiles[param.dir % row._tiles.Length];
				param.matColor = ((row.colorMod == 0) ? 104025 : GetColorInt(ref row2.matColor, row.colorMod));
				renderData.Draw(param);
				param.tile = renderData.idShadow;
				SourcePref shadowPref = renderData.shadowPref;
				int shadow2 = shadowPref.shadow;
				passShadow.AddShadow(param.x + renderData.offsetShadow.x, param.y + renderData.offsetShadow.y, param.z + renderData.offsetShadow.z, ShadowData.Instance.items[shadow2], shadowPref, 0, param.snow);
				break;
			}
			default:
				param.tile = row._tiles[param.dir % row._tiles.Length];
				param.matColor = ((row.colorMod == 0) ? 104025 : GetColorInt(ref row2.matColor, row.colorMod));
				row.renderData.Draw(param);
				break;
			}
			param.x = orgX;
			param.y = orgY;
			param.z = orgZ;
			param.color = blockLight;
		}
		fogged = false;
		bool flag4 = this.cell.isSurrounded && innerMode != 0 && sourceBlock.tileType.IsFullBlock;
		if (!(!isSeen || flag4))
		{
			goto IL_1668;
		}
		bool isRoomEdge = this.cell.IsRoomEdge;
		orgY = param.y;
		orgZ = param.z;
		param.color = (int)(50f * (_baseBrightness + fogBrightness)) * 262144;
		param.matColor = 104025f;
		if (hasBridge)
		{
			param.y = (float)(cz - cx) * screen.tileAlign.y + (float)(int)this.cell.bridgeHeight * _heightMod.y + ugFixBridgeBottom.x;
			param.z = 1000f + param.x * screen.tileWeight.x + param.y * screen.tileWeight.z + (float)(int)this.cell.bridgeHeight * _heightMod.z;
		}
		bool flag5 = (!isSeen && EMono._zone.UseFog) || flag4;
		if (flag5)
		{
			param.tile = 7f;
			rendererFogFloorSolid.Draw(param);
			param.tile = 0f;
			rendererFov2.Draw(param);
		}
		else if (this.cell.HasFloodBlock && isRoomEdge)
		{
			param.tile = 9f;
			rendererFogRoomWallSolid.Draw(param);
		}
		else
		{
			param.tile = 8f;
			rendererFogRoomSolid.Draw(param);
		}
		if ((this.cell.isSlopeEdge || hasBridge) && (flag5 || !isRoomEdge))
		{
			float num2 = (float)(int)this.cell.TopHeight * _heightMod.y;
			param.tile = 0f;
			for (int i = 0; (float)i < num2 / heightBlockSize; i++)
			{
				param.y += ugFix.y;
				param.z += ugFix.z + slopeFixZ * (float)i;
				if (flag5)
				{
					rendererFogBlockSolid.Draw(param);
				}
				else
				{
					rendererFogRoomBlockSolid.Draw(param);
				}
			}
		}
		param.y = orgY;
		param.z = orgZ;
		param.color = blockLight;
		if (flag5)
		{
			if (detail == null || !EMono.pc.hasTelepathy)
			{
				return;
			}
		}
		else
		{
			if (isRoomEdge)
			{
				goto IL_1668;
			}
			if (detail == null || !EMono.pc.hasTelepathy)
			{
				if (noRoofMode || detail == null)
				{
					return;
				}
				fogged = true;
			}
		}
		goto IL_7b3a;
		IL_7b3a:
		if (detail.things.Count == 0 && detail.charas.Count == 0)
		{
			return;
		}
		int num3 = 0;
		thingPos.x = 0f;
		thingPos.y = 0f;
		thingPos.z = 0f;
		freePos.x = (freePos.y = (freePos.z = 0f));
		if (this.cell.HasRamp)
		{
			Vector3 rampFix = sourceBlock.tileType.GetRampFix(this.cell.blockDir);
			param.x += rampFix.x;
			param.y += rampFix.y;
			param.z += rampFix.z;
			freePos.x += rampFix.x;
			freePos.y += rampFix.y;
			freePos.z += rampFix.z;
		}
		param.y += (flag ? 0f : ((this.cell._bridge != 0) ? this.cell.sourceBridge.tileType.FloorHeight : sourceFloor.tileType.FloorHeight));
		orgPos.x = (orgX = param.x);
		orgPos.y = (orgY = param.y);
		orgPos.z = (orgZ = param.z);
		if (flag && liquidLv > 0)
		{
			if (liquidLv > 10)
			{
				liquidLv = TileType.FloorWaterShallow.LiquidLV * 10;
			}
			liquidLv -= (int)(floatY * 0.5f);
			param.liquidLv = liquidLv;
			param.y -= TileType.FloorWaterShallow.FloorHeight;
		}
		Thing thing = null;
		bool shadow = liquidLv == 0;
		float num4 = 0f;
		float num5 = 0f;
		bool flag6 = false;
		float num6 = 0f;
		bool flag7 = false;
		float num7 = 0f;
		if (detail.things.Count > 0 && isSeen)
		{
			_ = zSetting.max1;
			float num8 = 0f;
			for (int j = 0; j < detail.things.Count; j++)
			{
				Thing t = detail.things[j];
				if ((fogged && !t.isRoofItem) || ((t.isHidden || t.trait.HideInAdv || t.isMasked) && !EMono.scene.actionMode.ShowMaskedThings) || (t.isRoofItem && ((this.room == null && !sourceBlock.tileType.IsFullBlock && !EMono._zone.IsPCFaction) || (lowBlock && !showFullWall && this.room != null) || (noRoofMode && currentRoom == null))) || (flag3 && !t.isRoofItem))
				{
					continue;
				}
				TileType tileType = t.trait.tileType;
				bool isInstalled = t.IsInstalled;
				SourcePref pref = t.Pref;
				if (!isInstalled && t.category.tileDummy != 0)
				{
					pref = rendererObjDummy.shadowPref;
				}
				float num9 = ((tileType.UseMountHeight && isInstalled) ? 0f : ((pref.height < 0f) ? 0f : ((pref.height == 0f) ? 0.1f : pref.height)));
				if (t.ignoreStackHeight)
				{
					thingPos.y -= num4;
				}
				shadow = thingPos.y < 0.16f && num7 < 0.16f;
				_ = pref.bypassShadow;
				param.shadowFix = 0f - thingPos.y;
				param.liquidLv = ((thingPos.y + (float)t.altitude < 0.1f) ? liquidLv : 0);
				if (t.isRoofItem)
				{
					param.snow = isSnowCovered && !this.cell.isClearSnow;
					SetRoofHeight(param, this.cell, cx, cz);
					_actorPos.x = param.x;
					_actorPos.y = param.y;
					_actorPos.z = param.z + num8;
					if (this.room != null)
					{
						param.color = GetRoofLight(this.room.lot);
					}
					shadow = false;
					param.liquidLv = 0;
				}
				else
				{
					param.snow = snowed;
					_actorPos.x = orgX + num5;
					_actorPos.y = orgY;
					_actorPos.z = orgZ + num8 + thingPos.z;
					if (tileType.CanStack || !isInstalled)
					{
						if (thing?.id != t.id)
						{
							_actorPos.x += thingPos.x;
						}
						_actorPos.y += thingPos.y;
						if (t.trait.IgnoreLastStackHeight && (thing == null || !thing.trait.IgnoreLastStackHeight))
						{
							thingPos.y -= num4;
							if (thing != null)
							{
								_actorPos.z -= 0.2f;
								thingPos.z -= 0.2f;
							}
							_actorPos.y -= num4;
						}
						_actorPos.z += renderSetting.thingZ + (float)j * -0.01f + zSetting.mod1 * thingPos.y;
					}
					if (isInstalled)
					{
						if (t.TileType.IsRamp)
						{
							Vector3 rampFix2 = t.TileType.GetRampFix(t.dir);
							orgX += rampFix2.x;
							orgY += rampFix2.y;
							orgZ += rampFix2.z;
							freePos.x += rampFix2.x;
							freePos.y += rampFix2.y;
							freePos.z += rampFix2.z;
							if (!this.cell.IsTopWater || t.altitude > 0)
							{
								num7 += rampFix2.y;
							}
							liquidLv -= (int)(rampFix2.y * 150f);
							if (liquidLv < 0)
							{
								liquidLv = 0;
							}
						}
						else if (!flag7 && t.trait.IsChangeFloorHeight && !t.ignoreStackHeight)
						{
							orgY += num9 + (float)t.altitude * altitudeFix.y;
							orgZ += (float)t.altitude * altitudeFix.z;
							freePos.y += num9 + (float)t.altitude * altitudeFix.y;
							if (!this.cell.IsTopWater || t.altitude > 0)
							{
								num7 += num9 + (float)t.altitude * altitudeFix.y;
							}
							_actorPos.x += pref.x * (float)((!t.flipX) ? 1 : (-1));
							_actorPos.z += pref.z;
							thingPos.z += pref.z;
							liquidLv -= (int)(num9 * 150f);
							if (liquidLv < 0)
							{
								liquidLv = 0;
							}
						}
						else
						{
							thingPos.y += num9;
							_actorPos.x += pref.x * (float)((!t.flipX) ? 1 : (-1));
							_actorPos.z += pref.z;
							if (pref.height >= 0f)
							{
								thingPos.z += pref.z;
							}
						}
						if (!tileType.UseMountHeight && j > 10)
						{
							flag7 = true;
						}
					}
					else
					{
						thingPos.y += num9;
						_actorPos.x += pref.x * (float)((!t.flipX) ? 1 : (-1));
						_actorPos.z += pref.z;
						thingPos.z += pref.z;
					}
					if (t.isFloating && isWater && !hasBridge && !flag)
					{
						flag = true;
						float num10 = ((this.cell._bridge != 0) ? sourceBridge.tileType.FloorHeight : sourceFloor.tileType.FloorHeight);
						orgY += 0.01f * floatY - num10;
						num6 = num9;
						_actorPos.y += 0.01f * floatY - num10;
						if (liquidLv > 10)
						{
							liquidLv = TileType.FloorWaterShallow.LiquidLV * 10;
						}
						liquidLv -= (int)(floatY * 0.5f);
						if (liquidLv < 0)
						{
							liquidLv = 0;
						}
						param.liquidLv = liquidLv;
					}
					num4 = num9;
					if (t.sourceCard.multisize && !t.trait.IsGround)
					{
						num8 += zSetting.multiZ;
					}
					orgZ += t.renderer.data.stackZ;
					if (param.liquidLv > 0)
					{
						param.liquidLv += pref.liquidMod;
						if (param.liquidLv < 1)
						{
							param.liquidLv = 1;
						}
						else if (param.liquidLv > 99 + pref.liquidModMax)
						{
							param.liquidLv = 99 + pref.liquidModMax;
						}
					}
				}
				if (isInstalled && tileType.UseMountHeight)
				{
					if (tileType != TileType.Illumination || !this.cell.HasObj)
					{
						if (noRoofMode && currentRoom == null && t.altitude >= lowWallObjAltitude)
						{
							continue;
						}
						if (hideHang && (this.cell.room?.lot != currentLot || (!this.cell.lotWall && this.cell.room != currentRoom)))
						{
							Room room = ((t.dir == 0) ? this.cell.Back.room : this.cell.Left.room);
							if (t.trait.AlwaysHideOnLowWall)
							{
								if (room == null || !room.data.showWallItem)
								{
									continue;
								}
							}
							else if (t.altitude >= lowWallObjAltitude)
							{
								continue;
							}
						}
					}
					if (tileType.UseHangZFix)
					{
						flag6 = true;
					}
					tileType.GetMountHeight(ref _actorPos, Point.shared.Set(index), t.dir, t);
					shadow = false;
					param.liquidLv = 0;
					if (t.freePos)
					{
						_actorPos.x += t.fx;
						_actorPos.y += t.fy;
					}
				}
				else
				{
					if (t.altitude != 0)
					{
						_actorPos += altitudeFix * t.altitude;
						if (t.altitude > 2 && ((this.cell.Back.room != null && this.cell.Back.IsRoomEdge) || (this.cell.Left.room != null && this.cell.Left.IsRoomEdge)) && hideHang && (this.cell.room?.lot != currentLot || (!this.cell.lotWall && this.cell.room != currentRoom)))
						{
							continue;
						}
					}
					if (t.freePos)
					{
						_actorPos.x = orgX + t.fx - freePos.x;
						_actorPos.y = orgY + t.fy - freePos.y;
					}
					if (t.trait is TraitDoor && (t.trait as TraitDoor).IsOpen())
					{
						_actorPos.z += -0.5f;
					}
				}
				if (!t.sourceCard.multisize || (t.pos.x == cx && t.pos.z == cz))
				{
					if (iconMode != 0)
					{
						int num11 = 0;
						switch (iconMode)
						{
						case CardIconMode.Visibility:
							if (t.isMasked)
							{
								num11 = 17;
							}
							break;
						case CardIconMode.State:
							if (t.placeState == PlaceState.installed)
							{
								num11 = 18;
							}
							break;
						case CardIconMode.Deconstruct:
							if (t.isDeconstructing)
							{
								num11 = 14;
							}
							break;
						}
						if (t.isNPCProperty && !EMono.debug.godBuild)
						{
							num11 = 13;
						}
						if (num11 != 0)
						{
							passGuideBlock.Add(_actorPos.x, _actorPos.y, _actorPos.z - 10f, num11);
						}
					}
					t.SetRenderParam(param);
					if (_lowblock && t.trait.UseLowblock && !this.cell.HasFullBlock)
					{
						param.tile += ((param.tile < 0f) ? (-64) : 64);
					}
					if (t.trait is TraitTrolley && EMono.pc.ai is AI_Trolley aI_Trolley && aI_Trolley.trolley.owner == t)
					{
						RenderParam _param = new RenderParam(param);
						EMono.core.actionsLateUpdate.Add(delegate
						{
							t.SetRenderParam(_param);
							_actorPos.x = EMono.pc.renderer.position.x;
							_actorPos.y = EMono.pc.renderer.position.y - pref.height;
							_actorPos.z = EMono.pc.renderer.position.z + 0.02f;
							t.renderer.Draw(_param, ref _actorPos, !t.noShadow && (shadow || tileType.AlwaysShowShadow));
						});
					}
					else
					{
						t.renderer.Draw(param, ref _actorPos, !t.noShadow && (shadow || tileType.AlwaysShowShadow));
					}
				}
				if (isInstalled)
				{
					num5 += pref.stackX * (float)((!t.flipX) ? 1 : (-1));
				}
				param.x = orgX;
				param.y = orgY;
				param.z = orgZ;
				param.color = floorLight;
				thing = t;
				if (pref.Float)
				{
					liquidLv = 0;
				}
			}
		}
		orgY += num6;
		if (detail.charas.Count <= 0)
		{
			return;
		}
		param.shadowFix = 0f - num7;
		param.color += 1310720f;
		float max = zSetting.max2;
		for (int k = 0; k < detail.charas.Count; k++)
		{
			Chara chara = detail.charas[k];
			if (chara.host != null || (chara != EMono.pc && chara != LayerDrama.alwaysVisible && (flag3 || fogged || (!showAllCards && !EMono.player.CanSee(chara)))))
			{
				continue;
			}
			_actorPos.x = orgX;
			_actorPos.y = orgY;
			_actorPos.z = orgZ;
			chara.SetRenderParam(param);
			_ = chara.IsAliveInCurrentZone;
			if (chara.isRestrained)
			{
				TraitShackle restrainer = chara.GetRestrainer();
				if (restrainer != null)
				{
					Vector3 getRestrainPos = restrainer.GetRestrainPos;
					if (getRestrainPos != default(Vector3))
					{
						Vector3 position = restrainer.owner.renderer.position;
						float defCharaHeight = EMono.setting.render.defCharaHeight;
						float num12 = getRestrainPos.y + defCharaHeight - ((chara.Pref.height == 0f) ? defCharaHeight : chara.source.pref.height);
						_actorPos.x = position.x + getRestrainPos.x * (float)((restrainer.owner.dir % 2 == 0) ? 1 : (-1));
						_actorPos.y = position.y + num12;
						_actorPos.z = position.z + getRestrainPos.z;
						param.liquidLv = 0;
						param.shadowFix = orgY - _actorPos.y;
						chara.renderer.SetFirst(first: true);
						chara.renderer.Draw(param, ref _actorPos, drawShadow: true);
						param.shadowFix = 0f;
						continue;
					}
				}
			}
			if (!chara.sourceCard.multisize || (chara.pos.x == cx && chara.pos.z == cz))
			{
				if (chara.IsDeadOrSleeping && chara.IsPCC)
				{
					float num13 = chara.renderer.data.size.y * 0.3f;
					if (thingPos.y > max)
					{
						thingPos.y = max;
					}
					float num14 = thingPos.y + num13;
					float num15 = (float)k * -0.01f;
					if (num14 > zSetting.thresh1)
					{
						num15 = zSetting.mod1;
					}
					_actorPos.x += thingPos.x;
					_actorPos.y += thingPos.y;
					_actorPos.z += renderSetting.laydownZ + num15;
					param.liquidLv = ((thingPos.y == 0f && liquidLv > 0) ? 90 : 0);
					thingPos.y += num13 * 0.8f;
					chara.renderer.Draw(param, ref _actorPos, liquidLv == 0);
				}
				else
				{
					param.liquidLv = liquidLv;
					if (param.liquidLv > 0)
					{
						param.liquidLv += chara.Pref.liquidMod;
						if (param.liquidLv < 1)
						{
							param.liquidLv = 1;
						}
						else if (param.liquidLv > 99 + chara.Pref.liquidModMax)
						{
							param.liquidLv = 99 + chara.Pref.liquidModMax;
						}
					}
					if (!chara.IsPC && !chara.renderer.IsMoving && detail.charas.Count > 1 && (detail.charas.Count != 2 || !detail.charas[0].IsDeadOrSleeping || !detail.charas[0].IsPCC))
					{
						_actorPos += renderSetting.charaPos[1 + ((num3 < 4) ? num3 : 3)];
					}
					_actorPos.z += 0.01f * (float)k + renderSetting.charaZ;
					num3++;
					if (flag6)
					{
						_actorPos.z += chara.renderer.data.hangedFixZ;
					}
					chara.renderer.Draw(param, ref _actorPos, liquidLv == 0);
				}
			}
			param.x = orgX;
			param.y = orgY;
			param.z = orgZ;
		}
		return;
		IL_6f1f:
		int num16;
		if (!showRoof || !roof || this.cell.room == null || this.cell.Front.room == null || this.cell.Right.room == null)
		{
			param.tile = num16;
			rendererFov.Draw(param);
		}
		goto IL_6f7f;
		IL_6f7f:
		if (isSnowCovered && (sourceBlock.id != 0 || this.cell.hasDoor) && !snowed && !this.cell.isClearSnow && ((!this.cell.Front.HasRoof && !this.cell.Front.HasBlock) || (!this.cell.Right.HasRoof && !this.cell.Right.HasBlock)))
		{
			snowed = true;
		}
		if (this.cell.effect != null)
		{
			if (this.cell.effect.IsLiquid)
			{
				SourceCellEffect.Row sourceEffect = this.cell.sourceEffect;
				SourceMaterial.Row defaultMaterial = sourceEffect.DefaultMaterial;
				tile = 4 + Rand.bytes[index % Rand.MaxBytes] % 4;
				param.tile = tile + this.cell.sourceEffect._tiles[0];
				param.mat = defaultMaterial;
				param.matColor = ((this.cell.effect.color == 0) ? GetColorInt(ref defaultMaterial.matColor, sourceEffect.colorMod) : this.cell.effect.color);
				sourceEffect.renderData.Draw(param);
			}
			else
			{
				param.tile = this.cell.effect.source._tiles[0];
				SourceCellEffect.Row sourceEffect2 = this.cell.sourceEffect;
				if (sourceEffect2.anime.Length != 0)
				{
					if (sourceEffect2.anime.Length > 2)
					{
						float num17 = Time.realtimeSinceStartup * 1000f / (float)sourceEffect2.anime[1] % (float)sourceEffect2.anime[2];
						if (!(num17 >= (float)sourceEffect2.anime[0]))
						{
							param.tile += num17;
						}
					}
					else
					{
						float num18 = Time.realtimeSinceStartup * 1000f / (float)sourceEffect2.anime[1] % (float)sourceEffect2.anime[0];
						param.tile += num18;
					}
				}
				if (this.cell.effect.IsFire)
				{
					rendererEffect.Draw(param);
				}
				else
				{
					this.cell.effect.source.renderData.Draw(param);
				}
			}
		}
		param.color = floorLight;
		if (this.cell.critter != null)
		{
			Critter critter = this.cell.critter;
			int snowTile = critter.tile;
			if (snowed && critter.SnowTile != 0)
			{
				critter.x = 0.06f;
				critter.y = -0.06f;
				snowTile = critter.SnowTile;
			}
			else
			{
				critter.Update();
			}
			pass = passObjSS;
			batch = pass.batches[pass.batchIdx];
			batch.matrices[pass.idx].m03 = param.x + (float)(int)(critter.x * 100f) * 0.01f;
			batch.matrices[pass.idx].m13 = param.y + (float)(int)(critter.y * 100f) * 0.01f;
			batch.matrices[pass.idx].m23 = param.z;
			batch.tiles[pass.idx] = snowTile * ((!critter.reverse) ? 1 : (-1));
			batch.colors[pass.idx] = floorLight;
			pass.idx++;
			if (pass.idx == pass.batchSize)
			{
				pass.NextBatch();
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
		if (this.cell.obj != 0 && !this.cell.sourceObj.renderData.SkipOnMap)
		{
			SourceObj.Row sourceObj = this.cell.sourceObj;
			if (!snowed || sourceObj.snowTile <= 0)
			{
				param.snow = snowed;
				param.mat = this.cell.matObj;
				orgY = param.y;
				if (param.liquidLv > 0)
				{
					if (sourceObj.pref.Float)
					{
						param.y += 0.01f * floatY;
						if (liquidLv > 10)
						{
							liquidLv = TileType.FloorWaterShallow.LiquidLV * 10;
						}
						liquidLv -= (int)(floatY * 0.5f);
						param.liquidLv = liquidLv;
					}
					if (sourceObj.tileType.IsWaterTop)
					{
						param.liquidLv = 0;
					}
					else
					{
						param.liquidLv += sourceObj.pref.liquidMod;
						if (param.liquidLv < 1)
						{
							param.liquid = 1f;
						}
						else if (param.liquidLv > 99 + sourceObj.pref.liquidModMax)
						{
							param.liquidLv = 99 + sourceObj.pref.liquidModMax;
						}
					}
				}
				if (sourceObj.useAltColor)
				{
					param.matColor = ((sourceObj.colorMod == 0) ? 104025 : GetColorInt(ref param.mat.altColor, sourceObj.colorMod));
				}
				else
				{
					param.matColor = ((sourceObj.colorMod == 0) ? 104025 : GetColorInt(ref param.mat.matColor, sourceObj.colorMod));
				}
				if (sourceObj.HasGrowth)
				{
					this.cell.growth.OnRenderTileMap(param);
					if (this.cell.obj == 118 && Core.fixedFrame % 10f == 0f && sourceObj.growth.IsMature)
					{
						EMono.scene.psFey.transform.position = new Vector3(param.x, param.y, param.z - 2f);
						EMono.scene.psFey.Emit(1);
					}
				}
				else
				{
					if (this.cell.autotileObj != 0)
					{
						param.tile = sourceObj._tiles[0] + this.cell.autotileObj;
					}
					else if (sourceObj.tileType.IsUseBlockDir)
					{
						param.tile = sourceObj._tiles[this.cell.blockDir % sourceObj._tiles.Length];
					}
					else
					{
						param.tile = sourceObj._tiles[this.cell.objDir % sourceObj._tiles.Length];
					}
					if (_lowblock && sourceObj.tileType.IsSkipLowBlock)
					{
						param.tile += ((param.tile > 0f) ? 1 : (-1)) * 3000000;
					}
					orgY = param.y;
					orgZ = param.z;
					param.y += sourceObj.pref.y;
					param.z += sourceObj.pref.z;
					sourceObj.renderData.Draw(param);
					param.y = orgY;
					param.z = orgZ;
					int shadow3 = sourceObj.pref.shadow;
					if (shadow3 > 1 && !this.cell.ignoreObjShadow)
					{
						passShadow.AddShadow(param.x + sourceObj.renderData.offsetShadow.x, param.y + sourceObj.renderData.offsetShadow.y, param.z + sourceObj.renderData.offsetShadow.z, ShadowData.Instance.items[shadow3], sourceObj.pref, 0, param.snow);
					}
					param.y = orgY;
				}
			}
		}
		if (this.cell.decal != 0 && sourceFloor.tileType.AllowBlood)
		{
			passDecal.Add(param, (int)this.cell.decal, floorLight);
		}
		if (highlightCells)
		{
			switch (ActionMode.FlagCell.mode)
			{
			case AM_FlagCell.Mode.flagWallPillar:
				if (this.cell.isToggleWallPillar)
				{
					passArea.Add(param, 34f, 0f);
				}
				break;
			case AM_FlagCell.Mode.flagSnow:
				if (this.cell.isClearSnow)
				{
					passArea.Add(param, 34f, 0f);
				}
				break;
			case AM_FlagCell.Mode.flagFloat:
				if (this.cell.isForceFloat)
				{
					passArea.Add(param, 34f, 0f);
				}
				break;
			case AM_FlagCell.Mode.flagClear:
				if (this.cell.isClearArea)
				{
					passArea.Add(param, 34f, 0f);
				}
				break;
			}
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
			param.matColor = 104025f;
			renderFootmark.Draw(param);
		}
		goto IL_7b3a;
		IL_1668:
		if (this.cell.isSlopeEdge)
		{
			float num19 = (float)height * _heightMod.y;
			orgY = param.y;
			orgZ = param.z;
			param.dir = this.cell.blockDir;
			if (snowed)
			{
				param.color = floorLight;
			}
			SourceBlock.Row defBlock;
			if (sourceBlock.tileType.IsFullBlock)
			{
				defBlock = sourceBlock;
				param.mat = matBlock;
				param.tile = sourceBlock._tiles[this.cell.blockDir % sourceBlock._tiles.Length];
				param.matColor = ((sourceBlock.colorMod == 0) ? 104025 : GetColorInt(ref matBlock.matColor, sourceBlock.colorMod));
			}
			else
			{
				defBlock = sourceFloor._defBlock;
				param.mat = matFloor;
				param.tile = defBlock._tiles[this.cell.blockDir % defBlock._tiles.Length];
				if (defBlock.id != 1)
				{
					param.matColor = ((sourceFloor.colorMod == 0) ? 104025 : GetColorInt(ref matFloor.matColor, sourceFloor.colorMod));
				}
				else
				{
					param.matColor = 104025f;
				}
			}
			for (int l = 0; (float)l < num19 / heightBlockSize; l++)
			{
				param.y += ugFix.y;
				param.z += ugFix.z + slopeFixZ * (float)l;
				defBlock.renderData.Draw(param);
				if (this.cell.pcSync && EMono.player.lightPower > 0f)
				{
					float num20 = param.tile;
					param.tile = 0f;
					rendererFov.Draw(param);
					param.tile = num20;
				}
			}
			param.y = orgY;
			param.z = orgZ;
		}
		param.color = floorLight;
		if (!isWater && (this.cell.Front.sourceFloor.tileType.IsWater || this.cell.Right.sourceFloor.tileType.IsWater) && this.cell.sourceBlock.tileType.RenderWaterBlock && !flag)
		{
			orgY = param.y;
			orgZ = param.z;
			int num21 = 0;
			if (sourceBlock.tileType.IsFullBlock)
			{
				SourceBlock.Row row3 = sourceBlock;
				num21 = sourceBlock._tiles[this.cell.blockDir % sourceBlock._tiles.Length];
			}
			else
			{
				SourceBlock.Row row3 = sourceFloor._defBlock;
				num21 = row3._tiles[this.cell.blockDir % row3._tiles.Length];
			}
			if (((this.cell.Front.shore / 12) & 1) == 0 && this.cell.Front.sourceFloor.tileType.IsWater && this.cell.Front.height <= height && this.cell.Front.sourceBlock.tileType.RenderWaterBlock)
			{
				param.y = (float)(cz - cx) * screen.tileAlign.y - (this.cell.Front.sourceFloor.tileType.IsDeepWater ? 0.6f : 0.4f) + (float)(int)this.cell.Front.height * _heightMod.y;
				param.z = 1000f + param.x * screen.tileWeight.x + param.y * screen.tileWeight.z;
				param.tile = num21 + ((!this.cell.Front.sourceFloor.tileType.IsDeepWater) ? 3000000 : 0);
				rendererWaterBlock.Draw(param);
			}
			if (((this.cell.Right.shore / 12) & 8) == 0 && this.cell.Right.sourceFloor.tileType.IsWater && this.cell.Right.height <= height && this.cell.Right.sourceBlock.tileType.RenderWaterBlock)
			{
				param.y = (float)(cz - cx) * screen.tileAlign.y - (this.cell.Right.sourceFloor.tileType.IsDeepWater ? 0.6f : 0.4f) + (float)(int)this.cell.Right.height * _heightMod.y;
				param.z = 1000f + param.x * screen.tileWeight.x + param.y * screen.tileWeight.z;
				param.tile = num21 + ((!this.cell.Right.sourceFloor.tileType.IsDeepWater) ? 3000000 : 0);
				rendererWaterBlock.Draw(param);
			}
			param.y = orgY;
			param.z = orgZ;
		}
		if (showBorder && !this.cell.outOfBounds)
		{
			param.matColor = 104025f;
			if (cx == EMono._map.bounds.x)
			{
				renderBorder.Draw(param, 12 + (EMono.world.date.IsNight ? 4 : 0));
			}
			else if (cx == EMono._map.bounds.maxX)
			{
				renderBorder.Draw(param, 13 + (EMono.world.date.IsNight ? 4 : 0));
			}
			if (cz == EMono._map.bounds.z)
			{
				renderBorder.Draw(param, 14 + (EMono.world.date.IsNight ? 4 : 0));
			}
			else if (cz == EMono._map.bounds.maxZ)
			{
				renderBorder.Draw(param, 15 + (EMono.world.date.IsNight ? 4 : 0));
			}
		}
		if (this.cell.isSkyFloor || (detail != null && detail.anime != null && detail.anime.drawBlock))
		{
			orgY = param.y;
			orgZ = param.z;
			SourceBlock.Row defBlock2 = sourceFloor._defBlock;
			param.mat = matFloor;
			param.tile = defBlock2._tiles[this.cell.blockDir % defBlock2._tiles.Length];
			if (defBlock2.id != 1)
			{
				param.matColor = ((sourceFloor.colorMod == 0) ? 104025 : GetColorInt(ref matFloor.matColor, sourceFloor.colorMod));
			}
			else
			{
				param.matColor = 104025f;
			}
			for (int m = 0; m < ((!this.cell.isSkyFloor) ? 1 : EMono._map.config.skyBlockHeight); m++)
			{
				param.y += ugFix.y;
				param.z += ugFix.z + slopeFixZ * (float)m;
				defBlock2.renderData.Draw(param);
			}
			param.y = orgY;
			param.z = orgZ;
		}
		if (!sourceFloor.tileType.IsSkipFloor)
		{
			if ((hasBridge && sourceBridge.tileType.CastShadowSelf) || this.cell.castFloorShadow)
			{
				floorLight2 = _lightMod * light * 0.2f + _baseBrightness + _shadowStrength * floorShadowStrength * (isWater ? 0.7f : (hasBridge ? 1f : (0.6f * (1f - nightRatio))));
				if (snowed)
				{
					floorLight2 = (int)((double)floorLight2 * 0.85 * 50.0) * 262144 + snowColorToken;
				}
				else
				{
					floorLight2 = (int)(floorLight2 * 50f) * 262144 + ((this.cell.lightR >= 64) ? 63 : this.cell.lightR) * 4096 + ((this.cell.lightG >= 64) ? 63 : this.cell.lightG) * 64 + ((this.cell.lightB >= 64) ? 63 : this.cell.lightB);
				}
				param.color = floorLight2;
				if (this.cell.lotShade)
				{
					floorLight = floorLight2;
				}
			}
			floorMatColor = ((sourceFloor.colorMod == 0) ? 104025 : GetColorInt(ref matFloor.matColor, sourceFloor.colorMod));
			if (isWater && flag)
			{
				param.y -= 0.01f * floatY;
			}
			if (!sourceBlock.tileType.IsSkipFloor || sourceBlock.transparent || hasBridge || this.cell.hasDoor || this.cell.skipRender)
			{
				param.mat = matFloor;
				param.tile = sourceFloor._tiles[floorDir % sourceFloor._tiles.Length];
				param.matColor = floorMatColor;
				param.snow = snowed;
				if (this.cell.isDeck)
				{
					param.z += 1f;
					if ((bool)sourceFloor.renderData.subData)
					{
						sourceFloor.renderData.subData.Draw(param);
					}
					sourceFloor.renderData.Draw(param);
					param.z -= 1f;
				}
				else
				{
					if ((bool)sourceFloor.renderData.subData)
					{
						sourceFloor.renderData.subData.Draw(param);
					}
					sourceFloor.renderData.Draw(param);
				}
				int num22 = 0;
				if (isSnowCovered && sourceFloor == FLOOR.sourceSnow && !this.cell.hasDoor)
				{
					if (!this.cell.Right.IsSnowTile && this.cell.Right.topHeight == this.cell.topHeight)
					{
						num22++;
					}
					if (!this.cell.Front.IsSnowTile && this.cell.Front.topHeight == this.cell.topHeight)
					{
						num22 += 2;
					}
					if (num22 != 0)
					{
						param.tile = 448 + num22 + 12;
						param.z -= 0.1f;
						sourceFloor.renderData.Draw(param);
						param.z += 0.1f;
					}
				}
				if (this.cell.shadow != 0 && !hasBridge && !this.cell.skipRender)
				{
					if (snowed)
					{
						if (sourceFloor == FLOOR.sourceSnow)
						{
							param.tile = 448 + this.cell.shadow + 8 + (this.cell.HasFence ? 4 : 0);
							param.z -= 0.01f;
							sourceFloor.renderData.Draw(param);
						}
					}
					else
					{
						pass = passEdge;
						batch = pass.batches[pass.batchIdx];
						batch.matrices[pass.idx].m03 = param.x + ambientShadowFix[this.cell.shadow].x;
						batch.matrices[pass.idx].m13 = param.y + ambientShadowFix[this.cell.shadow].y;
						batch.matrices[pass.idx].m23 = param.z + ambientShadowFix[this.cell.shadow].z;
						batch.tiles[pass.idx] = 448 + this.cell.shadow;
						batch.colors[pass.idx] = param.color;
						batch.matColors[pass.idx] = 104025f;
						pass.idx++;
						if (pass.idx == pass.batchSize)
						{
							pass.NextBatch();
						}
					}
					if (!sourceFloor.ignoreTransition && !snowed)
					{
						Cell back = this.cell.Back;
						if (back.sourceBlock.transition[0] != -1 && back.isSeen && !back.hasDoor)
						{
							pass = passFloor;
							batch = pass.batches[pass.batchIdx];
							batch.matrices[pass.idx].m03 = param.x + transitionFix[0].x;
							batch.matrices[pass.idx].m13 = param.y + transitionFix[0].y;
							batch.matrices[pass.idx].m23 = param.z + transitionFix[0].z;
							batch.tiles[pass.idx] = 480 + back.sourceBlock.transition[0] + Rand.bytes[index % Rand.MaxBytes] % back.sourceBlock.transition[1];
							batch.colors[pass.idx] = param.color;
							batch.matColors[pass.idx] = GetColorInt(ref back.matBlock.matColor, back.sourceBlock.colorMod);
							pass.idx++;
							if (pass.idx == pass.batchSize)
							{
								pass.NextBatch();
							}
						}
						back = this.cell.Left;
						if (back.sourceBlock.transition[0] != -1 && back.isSeen && !back.hasDoor)
						{
							pass = passFloor;
							batch = pass.batches[pass.batchIdx];
							batch.matrices[pass.idx].m03 = param.x + transitionFix[1].x;
							batch.matrices[pass.idx].m13 = param.y + transitionFix[1].y;
							batch.matrices[pass.idx].m23 = param.z + transitionFix[1].z;
							batch.tiles[pass.idx] = 512 + back.sourceBlock.transition[0] + Rand.bytes[index % Rand.MaxBytes] % back.sourceBlock.transition[1];
							batch.colors[pass.idx] = param.color;
							batch.matColors[pass.idx] = GetColorInt(ref back.matBlock.matColor, back.sourceBlock.colorMod);
							pass.idx++;
							if (pass.idx == pass.batchSize)
							{
								pass.NextBatch();
							}
						}
					}
				}
				if (this.cell.autotile != 0 && sourceFloor.autotile != 0 && (!hasBridge || this.cell.bridgeHeight - this.cell.height > 3) && !this.cell.skipRender && num22 == 0)
				{
					pass = (isWater ? passAutoTileWater : passAutoTile);
					batch = pass.batches[pass.batchIdx];
					batch.matrices[pass.idx].m03 = param.x;
					batch.matrices[pass.idx].m13 = param.y;
					batch.matrices[pass.idx].m23 = param.z + ((hasBridge || this.cell._block != 0) ? 0.8f : 0f);
					batch.tiles[pass.idx] = (26 + sourceFloor.autotile / 2) * 32 + sourceFloor.autotile % 2 * 16 + this.cell.autotile;
					batch.colors[pass.idx] = param.color + (float)((int)(sourceFloor.autotileBrightness * 100f) * 262144);
					batch.matColors[pass.idx] = param.matColor;
					pass.idx++;
					if (pass.idx == pass.batchSize)
					{
						pass.NextBatch();
					}
				}
			}
			if (isWater)
			{
				int num23 = 12;
				int num24 = this.cell.shore / num23;
				int num25 = this.cell.shore % num23;
				bool isShoreSand = this.cell.isShoreSand;
				if (this.cell.shore != 0)
				{
					Cell cell = ((((uint)num24 & (true ? 1u : 0u)) != 0) ? this.cell.Back : ((((uint)num24 & 2u) != 0) ? this.cell.Right : ((((uint)num24 & 4u) != 0) ? this.cell.Front : this.cell.Left)));
					if (isShoreSand && !cell.sourceFloor.isBeach)
					{
						cell = ((((uint)num24 & 8u) != 0) ? this.cell.Left : ((((uint)num24 & 4u) != 0) ? this.cell.Front : ((((uint)num24 & 2u) != 0) ? this.cell.Right : this.cell.Back)));
					}
					if (!cell.IsSnowTile)
					{
						param.matColor = GetColorInt(ref cell.matFloor.matColor, cell.sourceFloor.colorMod);
						if (isShoreSand)
						{
							pass = passShore;
							batch = pass.batches[pass.batchIdx];
							batch.matrices[pass.idx].m03 = param.x;
							batch.matrices[pass.idx].m13 = param.y;
							batch.matrices[pass.idx].m23 = param.z;
							batch.tiles[pass.idx] = 768 + this.cell.shore / num23;
							batch.colors[pass.idx] = param.color;
							batch.matColors[pass.idx] = param.matColor;
							pass.idx++;
							if (pass.idx == pass.batchSize)
							{
								pass.NextBatch();
							}
							num25 = 2;
						}
						else
						{
							num25 = cell.sourceFloor.edge;
						}
						param.tile = (24 + num25 / 2) * 32 + num25 % 2 * 16 + num24;
						rendererShore.Draw(param);
					}
				}
				if (this.cell.Back.isShoreSand && ((uint)(this.cell.Back.shore / num23) & 8u) != 0 && this.cell.Left.isShoreSand && ((uint)(this.cell.Left.shore / num23) & (true ? 1u : 0u)) != 0)
				{
					param.tile = 785f;
					param.matColor = GetColorInt(ref this.cell.BackLeft.matFloor.matColor, this.cell.BackLeft.sourceFloor.colorMod);
					passShore.Add(param);
					Draw(60);
				}
				if (this.cell.Back.isShoreSand && ((uint)(this.cell.Back.shore / num23) & 2u) != 0 && this.cell.Right.isShoreSand && ((uint)(this.cell.Right.shore / num23) & (true ? 1u : 0u)) != 0)
				{
					param.tile = 786f;
					param.matColor = GetColorInt(ref this.cell.BackRight.matFloor.matColor, this.cell.BackRight.sourceFloor.colorMod);
					passShore.Add(param);
					Draw(56);
				}
				if (this.cell.Front.isShoreSand && ((uint)(this.cell.Front.shore / num23) & 2u) != 0 && this.cell.Right.isShoreSand && ((uint)(this.cell.Right.shore / num23) & 4u) != 0)
				{
					param.tile = 787f;
					param.matColor = GetColorInt(ref this.cell.FrontRight.matFloor.matColor, this.cell.FrontRight.sourceFloor.colorMod);
					passShore.Add(param);
					Draw(48);
				}
				if (this.cell.Front.isShoreSand && ((uint)(this.cell.Front.shore / num23) & 8u) != 0 && this.cell.Left.isShoreSand && ((uint)(this.cell.Left.shore / num23) & 4u) != 0)
				{
					param.tile = 788f;
					param.matColor = GetColorInt(ref this.cell.FrontLeft.matFloor.matColor, this.cell.FrontLeft.sourceFloor.colorMod);
					passShore.Add(param);
					Draw(52);
				}
				if (this.cell._bridge != 0 && this.cell.isBridgeEdge && this.cell.bridgePillar != byte.MaxValue)
				{
					pass = passEdge;
					batch = pass.batches[pass.batchIdx];
					batch.matrices[pass.idx].m03 = param.x + waterEdgeBridgeFix.x;
					batch.matrices[pass.idx].m13 = param.y + waterEdgeBridgeFix.y;
					batch.matrices[pass.idx].m23 = param.z + waterEdgeBridgeFix.z;
					batch.tiles[pass.idx] = 616 + waterAnimeIndex % 4;
					batch.colors[pass.idx] = param.color;
					batch.matColors[pass.idx] = 104025f;
					pass.idx++;
					if (pass.idx == pass.batchSize)
					{
						pass.NextBatch();
					}
				}
				bool flag8 = false;
				if (isShoreSand)
				{
					if (((uint)num24 & (true ? 1u : 0u)) != 0)
					{
						if (((uint)num24 & 8u) != 0)
						{
							Draw(16);
							flag8 = true;
						}
						if (((uint)num24 & 2u) != 0)
						{
							Draw(20);
							flag8 = true;
						}
					}
					if (((uint)num24 & 4u) != 0)
					{
						if (((uint)num24 & 8u) != 0)
						{
							Draw(24);
							flag8 = true;
						}
						if (((uint)num24 & 2u) != 0)
						{
							Draw(28);
							flag8 = true;
						}
					}
					if (!flag8)
					{
						if (!this.cell.Front.sourceFloor.tileType.IsWater && !this.cell.Front.isDeck)
						{
							Draw(8);
						}
						if (!this.cell.Right.sourceFloor.tileType.IsWater && !this.cell.Right.isDeck)
						{
							Draw(12);
						}
					}
				}
				if (!flag8)
				{
					if (!this.cell.Back.sourceFloor.tileType.IsWater && !this.cell.Back.isDeck)
					{
						pass = passEdge;
						batch = pass.batches[pass.batchIdx];
						batch.tiles[pass.idx] = 608 + waterAnimeIndex % 4;
						batch.matColors[pass.idx] = 104025f;
						if (((uint)(this.cell.shore / num23) & (true ? 1u : 0u)) != 0)
						{
							if (isShoreSand)
							{
								param.matColor = GetColorInt(ref this.cell.Back.matFloor.matColor, this.cell.Back.sourceFloor.colorMod);
								batch.matrices[pass.idx].m03 = param.x + waterEdgeFixShoreSand.x;
								batch.matrices[pass.idx].m13 = param.y + waterEdgeFixShoreSand.y;
								batch.matrices[pass.idx].m23 = param.z + waterEdgeFixShoreSand.z;
								batch.tiles[pass.idx] = 640 + seaAnimeIndexes[waterAnimeIndex % seaAnimeIndexes.Length];
								batch.matColors[pass.idx] = param.matColor;
							}
							else
							{
								batch.matrices[pass.idx].m03 = param.x;
								batch.matrices[pass.idx].m13 = param.y;
								batch.matrices[pass.idx].m23 = param.z + waterEdgeFixShore.z;
							}
						}
						else
						{
							batch.matrices[pass.idx].m03 = param.x;
							batch.matrices[pass.idx].m13 = param.y;
							batch.matrices[pass.idx].m23 = param.z + waterEdgeFix.z;
							batch.tiles[pass.idx] += 12f;
						}
						batch.colors[pass.idx] = param.color;
						pass.idx++;
						if (pass.idx == pass.batchSize)
						{
							pass.NextBatch();
						}
					}
					if (!this.cell.Left.sourceFloor.tileType.IsWater && !this.cell.Left.isDeck)
					{
						pass = passEdge;
						batch = pass.batches[pass.batchIdx];
						batch.tiles[pass.idx] = 612 + waterAnimeIndex % 4;
						batch.matColors[pass.idx] = 104025f;
						if (((uint)(this.cell.shore / num23) & 8u) != 0)
						{
							if (isShoreSand)
							{
								param.matColor = GetColorInt(ref this.cell.Left.matFloor.matColor, this.cell.Left.sourceFloor.colorMod);
								batch.matrices[pass.idx].m03 = param.x + waterEdgeFixShoreSand.x;
								batch.matrices[pass.idx].m13 = param.y + waterEdgeFixShoreSand.y;
								batch.matrices[pass.idx].m23 = param.z + waterEdgeFixShoreSand.z;
								batch.tiles[pass.idx] = 644 + seaAnimeIndexes[waterAnimeIndex % seaAnimeIndexes.Length];
								batch.matColors[pass.idx] = param.matColor;
							}
							else
							{
								batch.matrices[pass.idx].m03 = param.x;
								batch.matrices[pass.idx].m13 = param.y;
								batch.matrices[pass.idx].m23 = param.z + waterEdgeFixShore.z;
							}
						}
						else
						{
							batch.matrices[pass.idx].m03 = param.x;
							batch.matrices[pass.idx].m13 = param.y;
							batch.matrices[pass.idx].m23 = param.z + waterEdgeFix.z;
							batch.tiles[pass.idx] += 12f;
						}
						batch.colors[pass.idx] = param.color;
						pass.idx++;
						if (pass.idx == pass.batchSize)
						{
							pass.NextBatch();
						}
					}
				}
				if (flag)
				{
					param.y += 0.01f * floatY;
				}
			}
			if (flag)
			{
				param.z -= 1f;
			}
		}
		if (this.cell.skipRender)
		{
			if (this.cell.pcSync)
			{
				param.tile = 0f;
				rendererFov.Draw(param);
			}
			return;
		}
		if (hasBridge)
		{
			param.y = (float)(cz - cx) * screen.tileAlign.y + (float)(int)this.cell.bridgeHeight * _heightMod.y;
			param.z = 1000f + param.x * screen.tileWeight.x + param.y * screen.tileWeight.z + (float)(int)this.cell.bridgeHeight * _heightMod.z;
			if (flag)
			{
				param.y += 0.01f * floatY;
			}
			param.color = floorLight;
			param.mat = matBridge;
			floorMatColor = ((sourceBridge.colorMod == 0) ? 104025 : GetColorInt(ref matBridge.matColor, sourceBridge.colorMod));
			param.dir = this.cell.floorDir;
			param.tile = sourceBridge._tiles[this.cell.floorDir % sourceBridge._tiles.Length];
			param.matColor = floorMatColor;
			sourceBridge.renderData.Draw(param);
			if (this.cell.autotileBridge != 0 && sourceBridge.autotile != 0)
			{
				pass = passAutoTile;
				batch = pass.batches[pass.batchIdx];
				batch.matrices[pass.idx].m03 = param.x;
				batch.matrices[pass.idx].m13 = param.y;
				batch.matrices[pass.idx].m23 = param.z + ((this.cell._block != 0) ? 0.8f : 0f);
				batch.tiles[pass.idx] = (26 + sourceBridge.autotile / 2) * 32 + sourceBridge.autotile % 2 * 16 + this.cell.autotileBridge;
				batch.colors[pass.idx] = param.color + (float)((int)(sourceBridge.autotileBrightness * 100f) * 262144);
				batch.matColors[pass.idx] = param.matColor;
				pass.idx++;
				if (pass.idx == pass.batchSize)
				{
					pass.NextBatch();
				}
			}
			if (this.cell.shadow != 0)
			{
				if (sourceBridge == FLOOR.sourceSnow)
				{
					param.tile = 448 + this.cell.shadow + 8 + (this.cell.HasFence ? 4 : 0);
					param.z -= 0.01f;
					sourceBridge.renderData.Draw(param);
				}
				else
				{
					pass = passEdge;
					batch = pass.batches[pass.batchIdx];
					batch.matrices[pass.idx].m03 = param.x + ambientShadowFix[this.cell.shadow].x;
					batch.matrices[pass.idx].m13 = param.y + ambientShadowFix[this.cell.shadow].y;
					batch.matrices[pass.idx].m23 = param.z + ambientShadowFix[this.cell.shadow].z;
					batch.tiles[pass.idx] = 448 + this.cell.shadow;
					batch.colors[pass.idx] = blockLight;
					batch.matColors[pass.idx] = 104025f;
					pass.idx++;
					if (pass.idx == pass.batchSize)
					{
						pass.NextBatch();
					}
				}
			}
			if (this.cell.isBridgeEdge && this.cell.bridgeHeight - this.cell.height >= 3 && this.cell.bridgePillar != byte.MaxValue && !noSlopMode)
			{
				orgY = param.y;
				orgZ = param.z;
				param.y += bridgeFix.y;
				param.z += bridgeFix.z;
				param.dir = 0;
				SourceBlock.Row row4 = sourceBridge._bridgeBlock;
				float num26 = (float)(this.cell.bridgeHeight - this.cell.height) * _heightMod.y;
				if (this.cell.sourceFloor.tileType == TileType.Sky)
				{
					num26 += (float)EMono._map.config.skyBlockHeight;
				}
				int num27 = (int)(num26 / heightBlockSize) + 2;
				if (this.cell.bridgePillar != 0)
				{
					row4 = EMono.sources.blocks.rows[this.cell.bridgePillar];
					param.tile = row4._tiles[0] + ((num27 == 2) ? 32 : 0);
					param.mat = ((sourceBridge.DefaultMaterial == row4.DefaultMaterial) ? sourceBridge.DefaultMaterial : row4.DefaultMaterial);
					param.matColor = ((row4.colorMod == 0) ? 104025 : GetColorInt(ref param.mat.matColor, row4.colorMod));
				}
				else
				{
					param.mat = matBlock;
					param.tile = row4._tiles[0] + 32;
					param.matColor = ((row4.colorMod == 0) ? 104025 : GetColorInt(ref matBridge.matColor, row4.colorMod));
				}
				param.y += ugFixBridgeTop.y;
				param.z += ugFixBridgeTop.z;
				for (int n = 0; n < num27; n++)
				{
					if (n == num27 - 1)
					{
						param.y = (float)(cz - cx) * screen.tileAlign.y + (float)height * _heightMod.y + ugFixBridgeBottom.y;
						param.z = 1000f + param.x * screen.tileWeight.x + param.y * screen.tileWeight.z + (float)height * _heightMod.z + ugFixBridgeBottom.z;
					}
					else
					{
						param.y += ugFixBridge.y;
						param.z += ugFixBridge.z;
					}
					row4.renderData.Draw(param);
				}
				param.y = orgY;
				param.z = orgZ;
			}
		}
		if (!buildMode && this.cell.highlight != 0)
		{
			if (this.cell._block != 0 && !this.cell.hasDoor)
			{
				screen.guide.DrawWall(this.cell.GetPoint(), EMono.Colors.blockColors.MapHighlight, useMarkerPass: true);
			}
			else
			{
				passGuideFloor.Add(this.cell.GetPoint(), (int)this.cell.highlight);
			}
		}
		param.color = blockLight;
		if (isSnowCovered && (sourceBlock.id != 0 || this.cell.hasDoor) && (snowed || this.cell.isClearSnow) && (this.cell.Front.HasRoof || this.cell.Right.HasRoof))
		{
			snowed = false;
		}
		num16 = 0;
		if (sourceBlock.id != 0)
		{
			this.tileType = sourceBlock.tileType;
			roomHeight = 0f;
			int blockDir = this.cell.blockDir;
			bool flag9 = false;
			switch (wallClipMode)
			{
			case WallClipMode.ByRoom:
				if (!this.tileType.RepeatBlock)
				{
					break;
				}
				if (currentRoom == null || showFullWall)
				{
					this.room = this.room ?? this.cell.Front.room ?? this.cell.Right.room ?? this.cell.FrontRight.room;
					_lowblock = lowBlock;
				}
				else if (this.room != this.cell.Front.room && (this.cell.Front.room == currentRoom || (this.room?.lot != currentLot && this.cell.Front.room?.lot == currentLot)))
				{
					this.room = this.cell.Front.room;
					_lowblock = !this.cell.Front.lotWall && !this.cell.Front.fullWall;
				}
				else if (this.room != this.cell.Right.room && (this.cell.Right.room == currentRoom || (this.room?.lot != currentLot && this.cell.Right.room?.lot == currentLot)))
				{
					this.room = this.cell.Right.room;
					_lowblock = !this.cell.Right.lotWall && !this.cell.Right.fullWall;
				}
				else if (this.tileType.IsFullBlock && this.room != this.cell.FrontRight.room && (this.cell.FrontRight.room == currentRoom || (this.room?.lot != currentLot && this.cell.FrontRight.room?.lot == currentLot)))
				{
					this.room = this.cell.FrontRight.room;
					_lowblock = !this.cell.FrontRight.lotWall && !this.cell.FrontRight.fullWall;
				}
				else
				{
					this.room = this.room ?? this.cell.Front.room ?? this.cell.Right.room ?? this.cell.FrontRight.room;
					_lowblock = true;
					if (!this.tileType.IsFullBlock)
					{
						if (this.cell.lotWall)
						{
							_lowblock = false;
						}
						else if (this.room == currentRoom)
						{
							_lowblock = !this.cell.fullWall;
						}
					}
				}
				flag9 = (this.room != null && this.room.data.atrium) || (this.cell.room != null && this.cell.room.data.atrium);
				if (flag9)
				{
					_lowblock = false;
				}
				if (this.room == null && alwaysLowblock)
				{
					_lowblock = true;
					roomHeight = 0f;
				}
				if (this.room != null)
				{
					maxHeight = (float)(cz - cx) * screen.tileAlign.y + (float)this.room.lot.mh * _heightMod.y;
					if (showRoof)
					{
						roomHeight = this.room.lot.realHeight;
						break;
					}
					if ((noRoofMode && currentRoom == null) || (_lowblock && !this.tileType.ForceRpeatBlock))
					{
						roomHeight = 0f;
						break;
					}
					int num28 = ((this.room.data.maxHeight == 0) ? 2 : this.room.data.maxHeight);
					roomHeight = EMono.setting.render.roomHeightMod * (float)((this.room.lot.height < num28) ? this.room.lot.height : num28) + 0.01f * (float)this.room.lot.heightFix;
				}
				break;
			case WallClipMode.ByLot:
				if (defaultBlockHeight > 0f || isIndoor)
				{
					_lowblock = cx != 0 && cz != Size - 1 && ((!this.cell.Back.HasBlock && !this.cell.Back.isWallEdge) || (!this.cell.Left.HasBlock && !this.cell.Left.isWallEdge) || !this.cell.Back.Left.HasBlock);
					if (!_lowblock)
					{
						roomHeight = defaultBlockHeight * EMono.setting.render.roomHeightMod;
						maxHeight = (float)(cz - cx) * screen.tileAlign.y + (float)(int)this.cell.TopHeight * _heightMod.y;
					}
					break;
				}
				if (showFullWall)
				{
					_lowblock = this.room != null;
					if (_lowblock)
					{
						if (this.cell.Back.IsRoomEdge && this.cell.Right.IsRoomEdge && this.cell.Back.room == null && this.cell.Right.room == null && this.cell.Right.Front.room?.lot == this.room?.lot)
						{
							_lowblock = false;
						}
					}
					else if (this.cell.Back.room != null && this.cell.Back.room.lot == (this.cell.Front.room ?? this.cell.Right.room)?.lot)
					{
						_lowblock = true;
					}
				}
				else
				{
					_lowblock = lowBlock;
				}
				if (this.tileType.RepeatBlock)
				{
					this.room = this.room ?? this.cell.Front.room ?? this.cell.Right.room ?? this.cell.FrontRight.room;
					if (this.room != null && (!noRoofMode || currentRoom != null) && (!showFullWall || currentRoom == null || this.room.lot == currentRoom.lot))
					{
						roomHeight = ((_lowblock && !this.tileType.ForceRpeatBlock) ? 0f : this.room.lot.realHeight);
						maxHeight = (float)(cz - cx) * screen.tileAlign.y + (float)this.room.lot.mh * _heightMod.y;
					}
				}
				break;
			}
			if (!_lowblock && (double)roomHeight > 1.2 && this.tileType.RepeatBlock)
			{
				num16 = 1;
			}
			else if (lowBlock)
			{
				num16 = 2;
			}
			param.mat = matBlock;
			param.dir = this.cell.blockDir;
			param.snow = snowed;
			switch (this.tileType.blockRenderMode)
			{
			case BlockRenderMode.FullBlock:
			{
				bool invisible = sourceBlock.tileType.Invisible;
				if (invisible && !buildMode && !ActionMode.Cinema.IsActive)
				{
					break;
				}
				if (this.cell.isSurrounded)
				{
					switch (innerMode)
					{
					case InnerMode.InnerBlock:
					case InnerMode.BuildMode:
						blockLight = _baseBrightness + fogBrightness;
						param.color = (int)(50f * blockLight) * 262144;
						param.matColor = 104025f;
						param.tile = (buildMode ? 1 : 2) + ((_lowblock || defaultBlockHeight > 0f) ? 3000000 : 0);
						rendererInnerBlock.Draw(param);
						return;
					case InnerMode.None:
					case InnerMode.Height:
						param.color = blockLight;
						break;
					}
				}
				if (snowed)
				{
					param.color = floorLight;
				}
				param.color -= (int)(_shadowStrength * 0.8f * 50f) * 262144;
				if (currentRoom != null && !showFullWall)
				{
					_lowblock = true;
					roomHeight = 0f;
					if (this.cell.room != currentRoom && (this.cell.Front.room == currentRoom || this.cell.Right.room == currentRoom || this.cell.FrontRight.room == currentRoom) && (this.cell.Back.room != currentRoom || this.cell.Right.room != currentRoom) && (this.cell.Front.room != currentRoom || this.cell.Left.room != currentRoom))
					{
						_lowblock = false;
					}
					if (!_lowblock)
					{
						int num29 = ((currentRoom.data.maxHeight == 0) ? 2 : currentRoom.data.maxHeight);
						roomHeight = EMono.setting.render.roomHeightMod * (float)((currentRoom.lot.height < num29) ? currentRoom.lot.height : num29) + 0.01f * (float)currentRoom.lot.heightFix;
					}
				}
				if (flag9)
				{
					_lowblock = (!this.cell.Front.HasFullBlock || !this.cell.Right.HasFullBlock) && (!this.cell.Front.HasFullBlock || !this.cell.Left.HasFullBlock) && (!this.cell.Back.HasFullBlock || !this.cell.Right.HasFullBlock) && (!this.cell.Back.HasFullBlock || !this.cell.Left.HasFullBlock);
					if (_lowblock)
					{
						roomHeight = 0f;
					}
				}
				if (invisible)
				{
					roomHeight = 0f;
					_lowblock = false;
				}
				if (this.cell.Things.Count > 0)
				{
					_lowblock = false;
				}
				param.tile = sourceBlock._tiles[this.cell.blockDir % sourceBlock._tiles.Length] + (_lowblock ? 3000000 : 0);
				param.matColor = ((sourceBlock.colorMod == 0) ? 104025 : GetColorInt(ref matBlock.matColor, sourceBlock.colorMod));
				if (roomHeight == 0f)
				{
					if (!this.cell.hasDoor)
					{
						sourceBlock.renderData.Draw(param);
					}
				}
				else
				{
					sourceBlock.renderData.DrawRepeatTo(param, maxHeight, roomHeight, ref renderSetting.peakFixBlock, this.cell.hasDoor, this.cell.effect?.FireAmount ?? 0, isBlock: true);
				}
				Room room2 = this.cell.Front.room ?? this.cell.room;
				if (room2 == null && this.cell.Right.room != null)
				{
					room2 = this.cell.Right.room;
				}
				if (!invisible && room2 != null)
				{
					if (room2.lot.idDeco != 0 && !this.cell.hasDoor)
					{
						param.tile = EMono.sources.blocks.rows[0].ConvertTile(1000 + room2.lot.idDeco);
						param.matColor = room2.lot.colDeco;
						float y = param.y;
						param.y += (float)room2.lot.decoFix * 0.01f;
						rendererWallDeco.Draw(param);
						param.y = y;
					}
					if (room2.lot.idDeco2 != 0 && roomHeight != 0f && (float)room2.lot.decoFix2 * 0.01f + heightLimitDeco < roomHeight + maxHeight - param.y)
					{
						param.tile = EMono.sources.blocks.rows[0].ConvertTile(1000 + room2.lot.idDeco2);
						param.matColor = room2.lot.colDeco2;
						float y2 = param.y;
						float num30 = param.z;
						param.y += (float)room2.lot.decoFix2 * 0.01f;
						param.z += (float)room2.lot.decoFix2 * 0.01f * heightModDeco;
						rendererWallDeco.Draw(param);
						param.y = y2;
						param.z = num30;
					}
				}
				room2 = this.cell.Right.room ?? this.cell.room;
				if (room2 == null && this.cell.Front.room != null)
				{
					room2 = this.cell.Front.room;
				}
				if (!invisible && room2 != null)
				{
					if (room2.lot.idDeco != 0 && !this.cell.hasDoor)
					{
						param.tile = EMono.sources.blocks.rows[0].ConvertTile(1000 + room2.lot.idDeco) * -1;
						param.matColor = room2.lot.colDeco;
						float y3 = param.y;
						param.y += (float)room2.lot.decoFix * 0.01f;
						rendererWallDeco.Draw(param);
						param.y = y3;
					}
					if (room2.lot.idDeco2 != 0 && roomHeight != 0f && (float)room2.lot.decoFix2 * 0.01f + heightLimitDeco < roomHeight + maxHeight - param.y)
					{
						param.tile = EMono.sources.blocks.rows[0].ConvertTile(1000 + room2.lot.idDeco2) * -1;
						param.matColor = room2.lot.colDeco2;
						float y4 = param.y;
						float num31 = param.z;
						param.y += (float)room2.lot.decoFix2 * 0.01f;
						param.z += (float)room2.lot.decoFix2 * 0.01f * heightModDeco;
						rendererWallDeco.Draw(param);
						param.y = y4;
						param.z = num31;
					}
				}
				break;
			}
			case BlockRenderMode.WallOrFence:
			{
				if (map.config.fullWallHeight)
				{
					showFullWall = true;
					_lowblock = false;
				}
				orgY = param.y;
				orgZ = param.z;
				param.color = (this.tileType.IsFence ? (floorLight - (float)((int)(_shadowStrength * 0.8f * 50f) * 262144)) : blockLight);
				bool flag10 = blockDir == 1 || _lowblock || flag9;
				bool flag11 = blockDir == 0 || _lowblock || flag9;
				if (!showFullWall && currentRoom != null)
				{
					if (!flag10)
					{
						if (currentRoom == this.cell.room || (this.cell.lotWall && this.cell.room?.lot == currentLot && this.cell.Front.room != currentRoom))
						{
							if (!this.cell.IsRoomEdge || (this.cell.Front.room != this.cell.room && this.cell.FrontRight.room != this.cell.room))
							{
								flag10 = true;
							}
						}
						else if ((!this.cell.Front.lotWall || this.cell.Front.room?.lot != currentLot) && this.cell.Front.room != currentRoom)
						{
							flag10 = true;
						}
					}
					if (!flag11)
					{
						if (currentRoom == this.cell.room || (this.cell.lotWall && this.cell.room?.lot == currentLot && this.cell.Right.room != currentRoom))
						{
							if (!this.cell.IsRoomEdge || (this.cell.Right.room != this.cell.room && this.cell.FrontRight.room != this.cell.room))
							{
								flag11 = true;
							}
						}
						else if ((!this.cell.Right.lotWall || this.cell.Right.room?.lot != currentLot) && this.cell.Right.room != currentRoom)
						{
							flag11 = true;
						}
					}
				}
				if (blockDir == 0 || blockDir == 2)
				{
					param.dir = 0;
					Room room3 = this.cell.Front.room ?? this.cell.room;
					if (room3 != null && this.tileType.IsWall)
					{
						if (room3.lot.idDeco != 0 && !this.cell.hasDoor)
						{
							param.tile = EMono.sources.blocks.rows[0].ConvertTile(1000 + room3.lot.idDeco);
							param.matColor = room3.lot.colDeco;
							param.y += (float)room3.lot.decoFix * 0.01f;
							rendererWallDeco.Draw(param);
							param.y = orgY;
						}
						if (room3.lot.idDeco2 != 0 && roomHeight != 0f && !flag10 && (float)room3.lot.decoFix2 * 0.01f + heightLimitDeco < roomHeight + maxHeight - param.y)
						{
							param.tile = EMono.sources.blocks.rows[0].ConvertTile(1000 + room3.lot.idDeco2);
							param.matColor = room3.lot.colDeco2;
							param.y += (float)room3.lot.decoFix2 * 0.01f;
							param.z += (float)room3.lot.decoFix2 * 0.01f * heightModDeco;
							rendererWallDeco.Draw(param);
							param.y = orgY;
							param.z = orgZ;
						}
					}
					Cell left = this.cell.Left;
					if (blockDir == 2 && left.sourceBlock.tileType.IsWallOrFence && !this.cell.crossWall)
					{
						_sourceBlock = left.sourceBlock;
						param.mat = left.matBlock;
					}
					else
					{
						_sourceBlock = sourceBlock;
						param.mat = matBlock;
					}
					this.tileType = _sourceBlock.tileType;
					param.tile = (tile = _sourceBlock._tiles[0] + ((flag10 && !this.tileType.IsFence) ? 32 : 0));
					if (_sourceBlock.useAltColor)
					{
						param.matColor = ((_sourceBlock.colorMod == 0) ? 104025 : GetColorInt(ref param.mat.altColor, _sourceBlock.colorMod));
					}
					else
					{
						param.matColor = ((_sourceBlock.colorMod == 0) ? 104025 : GetColorInt(ref param.mat.matColor, _sourceBlock.colorMod));
					}
					if (roomHeight == 0f || flag10 || !this.tileType.RepeatBlock)
					{
						if (!this.cell.hasDoor)
						{
							_sourceBlock.renderData.Draw(param);
						}
					}
					else
					{
						_sourceBlock.renderData.DrawRepeatTo(param, maxHeight, roomHeight, ref renderSetting.peakFix, this.cell.hasDoor, this.cell.effect?.FireAmount ?? 0);
					}
					param.z += cornerWallFix2.z;
					if ((blockDir == 2 || (this.cell.Front.HasWallOrFence && this.cell.Front.blockDir != 0)) != this.cell.isToggleWallPillar)
					{
						if (this.cell.Back.IsSnowTile && this.cell.Right.IsSnowTile)
						{
							param.snow = true;
						}
						param.tile = _sourceBlock._tiles[0] + ((flag10 && flag11 && !this.tileType.IsFence && !flag9) ? 32 : 0) + (this.tileType.IsFence ? 32 : 64);
						if (roomHeight == 0f || !this.tileType.RepeatBlock || (flag10 && flag11 && !flag9))
						{
							_sourceBlock.renderData.Draw(param);
						}
						else
						{
							_sourceBlock.renderData.DrawRepeatTo(param, maxHeight, roomHeight, ref renderSetting.peakFix);
						}
					}
					if (!flag10 && !showRoof && this.cell.Left.HasWallOrFence && this.cell.Left.blockDir != 0 && !this.cell.isToggleWallPillar)
					{
						orgX = param.x;
						param.tile = _sourceBlock._tiles[0] + ((flag10 && !this.tileType.IsFence && !flag9) ? 32 : 0) + (this.tileType.IsFence ? 32 : 64);
						param.x += cornerWallFix3.x;
						param.y += cornerWallFix3.y;
						param.z += cornerWallFix3.z;
						if (!flag9 && (roomHeight == 0f || flag10))
						{
							_sourceBlock.renderData.Draw(param);
						}
						else
						{
							_sourceBlock.renderData.DrawRepeatTo(param, maxHeight, roomHeight + cornerWallFix.y, ref renderSetting.peakFix);
						}
						param.x = orgX;
					}
					else if (this.cell.FrontLeft.HasWallOrFence && this.cell.FrontLeft.blockDir != 0 && (!flag10 || !this.cell.Left.HasWall) && !this.cell.isToggleWallPillar)
					{
						orgX = param.x;
						param.tile = _sourceBlock._tiles[0] + ((flag10 && !this.tileType.IsFence && !flag9) ? 32 : 0) + (this.tileType.IsFence ? 32 : 64);
						param.x += cornerWallFix.x;
						param.y += cornerWallFix.y;
						param.z += cornerWallFix.z;
						if (!flag9 && (roomHeight == 0f || flag10))
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
					Room room4 = this.cell.Right.room ?? this.cell.room;
					if (room4 != null && this.tileType.IsWall)
					{
						if (room4.lot.idDeco != 0 && !this.cell.hasDoor)
						{
							param.tile = -EMono.sources.blocks.rows[0].ConvertTile(1000 + room4.lot.idDeco);
							param.matColor = room4.lot.colDeco;
							param.y += (float)room4.lot.decoFix * 0.01f;
							rendererWallDeco.Draw(param);
							param.y = orgY;
						}
						if (room4.lot.idDeco2 != 0 && roomHeight != 0f && !flag11 && (float)room4.lot.decoFix2 * 0.01f + heightLimitDeco < roomHeight + maxHeight - param.y)
						{
							param.tile = -EMono.sources.blocks.rows[0].ConvertTile(1000 + room4.lot.idDeco2);
							param.matColor = room4.lot.colDeco2;
							param.y += (float)room4.lot.decoFix2 * 0.01f;
							param.z += (float)room4.lot.decoFix2 * 0.01f * heightModDeco;
							rendererWallDeco.Draw(param);
							param.y = orgY;
							param.z = orgZ;
						}
					}
					if (blockDir == 2 && this.cell.room == null && this.cell.Right.room != null)
					{
						Room room5 = this.cell.Right.room;
						maxHeight = (float)(cz - cx) * screen.tileAlign.y + (float)room5.lot.mh * _heightMod.y;
						if (showRoof)
						{
							roomHeight = room5.lot.realHeight;
						}
						else if ((noRoofMode && currentRoom == null) || (_lowblock && !this.tileType.ForceRpeatBlock))
						{
							roomHeight = 0f;
						}
						else
						{
							int num32 = ((room5.data.maxHeight == 0) ? 2 : room5.data.maxHeight);
							roomHeight = EMono.setting.render.roomHeightMod * (float)((room5.lot.height < num32) ? room5.lot.height : num32) + 0.01f * (float)room5.lot.heightFix;
						}
					}
					Cell back2 = this.cell.Back;
					if (blockDir == 2 && back2.sourceBlock.tileType.IsWallOrFence && !this.cell.crossWall)
					{
						_sourceBlock = back2.sourceBlock;
						param.mat = back2.matBlock;
					}
					else
					{
						_sourceBlock = sourceBlock;
						param.mat = matBlock;
					}
					this.tileType = _sourceBlock.tileType;
					param.tile = (tile = -_sourceBlock._tiles[0] + ((flag11 && !this.tileType.IsFence) ? (-32) : 0));
					if (_sourceBlock.useAltColor)
					{
						param.matColor = ((_sourceBlock.colorMod == 0) ? 104025 : GetColorInt(ref param.mat.altColor, _sourceBlock.colorMod));
					}
					else
					{
						param.matColor = ((_sourceBlock.colorMod == 0) ? 104025 : GetColorInt(ref param.mat.matColor, _sourceBlock.colorMod));
					}
					param.color += _rightWallShade;
					if (roomHeight == 0f || flag11 || !this.tileType.RepeatBlock)
					{
						if (!this.cell.hasDoor)
						{
							_sourceBlock.renderData.Draw(param);
						}
					}
					else
					{
						_sourceBlock.renderData.DrawRepeatTo(param, maxHeight, roomHeight, ref renderSetting.peakFix, this.cell.hasDoor, this.cell.effect?.FireAmount ?? 0);
					}
					if ((this.cell.Right.HasWallOrFence && this.cell.Right.blockDir != 1) != this.cell.isToggleWallPillar && (blockDir != 2 || !this.cell.isToggleWallPillar))
					{
						if (this.cell.Left.IsSnowTile && this.cell.Front.IsSnowTile)
						{
							param.snow = true;
						}
						orgX = param.x;
						param.tile = _sourceBlock._tiles[0] + ((flag11 && !this.tileType.IsFence && !flag9) ? 32 : 0) + (this.tileType.IsFence ? 32 : 64);
						if (!flag9 && (roomHeight == 0f || !this.tileType.RepeatBlock || flag11))
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
				param.y = orgY;
				param.z = orgZ;
				break;
			}
			case BlockRenderMode.HalfBlock:
				param.color = floorLight;
				_sourceBlock = ((sourceBlock.id == 5) ? EMono.sources.blocks.rows[matBlock.defBlock] : sourceBlock);
				param.tile = _sourceBlock._tiles[0];
				param.matColor = ((_sourceBlock.colorMod == 0) ? 104025 : GetColorInt(ref matBlock.matColor, _sourceBlock.colorMod));
				param.tile2 = _sourceBlock.sourceAutoFloor._tiles[0];
				param.halfBlockColor = ((_sourceBlock.sourceAutoFloor.colorMod == 0) ? 104025 : GetColorInt(ref matBlock.matColor, _sourceBlock.sourceAutoFloor.colorMod));
				sourceBlock.renderData.Draw(param);
				break;
			case BlockRenderMode.Pillar:
			{
				RenderData renderData2 = sourceBlock.renderData;
				param.tile = sourceBlock._tiles[this.cell.blockDir % sourceBlock._tiles.Length];
				param.matColor = ((sourceBlock.colorMod == 0) ? 104025 : GetColorInt(ref matBlock.matColor, sourceBlock.colorMod));
				int num33 = this.cell.objDir + ((this.cell.objDir >= 7) ? this.cell.objDir : 0) + 1;
				if (num33 == 0)
				{
					renderData2.Draw(param);
				}
				else
				{
					renderData2.DrawRepeat(param, num33, sourceBlock.tileType.RepeatSize);
				}
				param.tile = renderData2.idShadow;
				SourcePref shadowPref2 = renderData2.shadowPref;
				int shadow4 = shadowPref2.shadow;
				passShadow.AddShadow(param.x + renderData2.offsetShadow.x, param.y + renderData2.offsetShadow.y, param.z + renderData2.offsetShadow.z, ShadowData.Instance.items[shadow4], shadowPref2, 0, param.snow);
				break;
			}
			default:
				param.color = floorLight;
				param.tile = sourceBlock._tiles[this.cell.blockDir % sourceBlock._tiles.Length] + ((_lowblock && this.tileType.UseLowWallTiles) ? 3000000 : 0);
				param.matColor = ((sourceBlock.colorMod == 0) ? 104025 : GetColorInt(ref matBlock.matColor, sourceBlock.colorMod));
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
		if (this.cell.pcSync && EMono.player.lightPower > 0f && !cinemaMode)
		{
			if (this.cell.room != null || !this.cell.IsRoomEdge || !showRoof)
			{
				goto IL_6f1f;
			}
			if (this.cell._block == 0 || !this.cell.sourceBlock.tileType.RepeatBlock)
			{
				Room obj = this.cell.FrontRight.room;
				if (obj == null || !obj.HasRoof)
				{
					goto IL_6f1f;
				}
			}
		}
		goto IL_6f7f;
		void Draw(int tile)
		{
			pass = passEdge;
			batch = pass.batches[pass.batchIdx];
			batch.tiles[pass.idx] = 640 + seaAnimeIndexes[waterAnimeIndex % seaAnimeIndexes.Length] + tile;
			batch.matrices[pass.idx].m03 = param.x + waterEdgeFixShoreSand.x;
			batch.matrices[pass.idx].m13 = param.y + waterEdgeFixShoreSand.y;
			batch.matrices[pass.idx].m23 = param.z + waterEdgeFixShoreSand.z;
			batch.colors[pass.idx] = param.color;
			batch.matColors[pass.idx] = param.matColor;
			pass.idx++;
			if (pass.idx == pass.batchSize)
			{
				pass.NextBatch();
			}
		}
	}

	public Vector3 GetThingPosition(Card tg, Point p)
	{
		Vector3 zero = Vector3.zero;
		float num = 0f;
		cell = p.cell;
		sourceFloor = cell.sourceFloor;
		if (!tg.TileType.UseMountHeight)
		{
			if (cell.isFloating && !cell.IsSnowTile)
			{
				zero.z -= 1f;
			}
			else if (!tg.sourceCard.multisize)
			{
				float num2 = ((cell._bridge != 0) ? cell.sourceBridge.tileType.FloorHeight : sourceFloor.tileType.FloorHeight);
				zero.y += num2;
				zero.z -= num2 * heightMod.z;
			}
			if (cell.HasRamp)
			{
				Vector3 rampFix = cell.sourceBlock.tileType.GetRampFix(cell.blockDir);
				zero.x += rampFix.x;
				zero.y += rampFix.y;
				zero.z += rampFix.z;
			}
		}
		if (tg.sourceCard.multisize)
		{
			zero.z -= 1f;
		}
		zero.x += tg.Pref.x * (float)((!tg.flipX) ? 1 : (-1));
		zero.z += tg.Pref.z;
		detail = cell.detail;
		if (tg.isChara)
		{
			return zero;
		}
		bool flag = false;
		if (tg.TileType.UseMountHeight && !EMono.scene.actionMode.IsRoofEditMode(tg))
		{
			flag = true;
		}
		else if (!tg.TileType.CanStack || EMono.scene.actionMode.IsRoofEditMode(tg))
		{
			if (tg.altitude != 0)
			{
				zero += altitudeFix * tg.altitude;
			}
			flag = true;
		}
		if (EMono.scene.actionMode.IsRoofEditMode(tg))
		{
			return zero;
		}
		float num3 = 0f;
		if (detail != null && detail.things.Count > 0)
		{
			Card card = null;
			for (int i = 0; i < detail.things.Count; i++)
			{
				Thing thing = detail.things[i];
				SourcePref pref = thing.Pref;
				TileType tileType = thing.trait.tileType;
				float num4 = (tileType.UseMountHeight ? 0f : ((pref.height == 0f) ? 0.1f : pref.height));
				if (!thing.IsInstalled || thing == ActionMode.Inspect.target)
				{
					continue;
				}
				if (thing.TileType.IsRamp)
				{
					Vector3 rampFix2 = thing.TileType.GetRampFix(thing.dir);
					zero.x += rampFix2.x;
					zero.y += rampFix2.y;
					zero.z += rampFix2.z;
				}
				if (!flag && tileType.CanStack)
				{
					if (thing.ignoreStackHeight)
					{
						zero.y -= num3;
					}
					zero.y += num4;
					zero.x += pref.stackX * (float)((!thing.flipX) ? 1 : (-1));
					zero.z += pref.z + thing.renderer.data.stackZ;
					if (!tileType.UseMountHeight && thing.altitude != 0)
					{
						zero += altitudeFix * thing.altitude;
						num4 += altitudeFix.y * (float)thing.altitude;
					}
					if (thing.trait.IgnoreLastStackHeight && (card == null || !card.trait.IgnoreLastStackHeight))
					{
						zero.y -= num3;
					}
					num3 = num4;
					zero.z += renderSetting.thingZ + num + (float)i * -0.01f + zSetting.mod1 * zero.y;
					if (thing.sourceCard.multisize)
					{
						num += zSetting.multiZ;
					}
					card = thing;
				}
			}
		}
		if (flag)
		{
			return zero;
		}
		if (tg.ignoreStackHeight)
		{
			zero.y -= num3;
		}
		if (tg.altitude != 0)
		{
			zero += altitudeFix * tg.altitude;
		}
		return zero;
	}

	public int GetApproximateBlocklight(Cell cell)
	{
		float num = _baseBrightness + 0.05f;
		num = ((!cell.IsSnowTile) ? ((float)((int)(num * 50f) * 262144 + ((cell.lightR >= 64) ? 63 : cell.lightR) * 4096 + ((cell.lightG >= 64) ? 63 : cell.lightG) * 64 + ((cell.lightB >= 64) ? 63 : cell.lightB))) : ((float)((int)(num * 50f) * 262144 + (int)((float)((cell.lightR >= 50) ? 50 : cell.lightR) * snowColor) * 4096 + (int)((float)((cell.lightG >= 50) ? 50 : cell.lightG) * snowColor) * 64 + (int)((float)((cell.lightB >= 50) ? 50 : cell.lightB) * snowColor) + snowColorToken)));
		return (int)num;
	}

	public int GetRoofLight(Lot lot)
	{
		float num = Mathf.Sqrt(lot.light) * roofLightMod;
		if (num > lightLimit * roofLightLimitMod)
		{
			num = lightLimit * roofLightLimitMod;
		}
		if (isSnowCovered)
		{
			num += roofLightSnow * (1f - nightRatio);
		}
		int num2 = (int)(num * 50f) * 262144;
		if (isSnowCovered)
		{
			num2 += snowColorToken;
		}
		return num2;
	}

	public void DrawRoof(Lot lot)
	{
		RoofStyle roofStyle = roofStyles[lot.idRoofStyle];
		if (roofStyle.type == RoofStyle.Type.None)
		{
			return;
		}
		bool reverse = lot.reverse;
		int num;
		int num2;
		int num3;
		int num4;
		if (reverse)
		{
			num = lot.z - roofStyle.h;
			num2 = lot.x - roofStyle.w;
			num3 = lot.mz + 1 + roofStyle.h;
			num4 = lot.mx + 1 + roofStyle.w;
			if (num2 > 1 && num > 0 && map.cells[num2 - 1, num].HasFullBlock)
			{
				num2--;
			}
			if (num3 < Size && num4 < Size && map.cells[num4 - 1, num3].HasFullBlock)
			{
				num3++;
			}
		}
		else
		{
			num = lot.x - roofStyle.w;
			num2 = lot.z - roofStyle.h;
			num3 = lot.mx + 1 + roofStyle.w;
			num4 = lot.mz + 1 + roofStyle.h;
			if (num2 > 0 && num > 1 && map.cells[num - 1, num2].HasFullBlock)
			{
				num--;
			}
			if (num3 < Size && num4 < Size && map.cells[num3 - 1, num4].HasFullBlock)
			{
				num4++;
			}
		}
		int num5;
		if (roofStyle.wing)
		{
			num5 = ((lot.height > 1) ? 1 : 0);
			if (num5 != 0)
			{
				num2--;
				num4++;
			}
		}
		else
		{
			num5 = 0;
		}
		int num6 = num4 + (reverse ? roofStyle.w : roofStyle.h) * 2 - num2;
		int idRoofTile = lot.idRoofTile;
		int num7 = lot.idBlock;
		int num8 = num7;
		if (num7 >= EMono.sources.blocks.rows.Count)
		{
			num7 = EMono.sources.blocks.rows.Count - 1;
		}
		if (num8 >= EMono.sources.floors.rows.Count)
		{
			num8 = EMono.sources.floors.rows.Count - 1;
		}
		int num9 = lot.idRamp;
		if (num9 >= EMono.sources.blocks.rows.Count)
		{
			num9 = EMono.sources.blocks.rows.Count - 1;
		}
		bool flag = false;
		int num10 = num6 / 2 - roofStyle.flatW;
		int num11 = num6 / 2 + roofStyle.flatW + ((num6 % 2 != 0) ? 1 : 0);
		SourceBlock.Row row = (roofStyle.useDefBlock ? cell.sourceFloor._defBlock : EMono.sources.blocks.rows[num7]);
		int num12 = 0;
		int num13 = ((num5 != 0) ? (-1) : 0);
		int num14 = 0;
		Vector3 vector = (lot.fullblock ? roofStyle.posFixBlock : roofStyle.posFix);
		switch (roofStyle.type)
		{
		case RoofStyle.Type.Default:
			flag = num6 % 2 == 1 && roofStyle.flatW == 0;
			break;
		case RoofStyle.Type.Flat:
		case RoofStyle.Type.FlatFloor:
			num10 = roofStyle.flatW;
			num11 = num6 - roofStyle.flatW;
			if (num10 == 0)
			{
				num14 = 1;
			}
			if (roofStyle.type != RoofStyle.Type.FlatFloor)
			{
				num--;
				num3++;
				num2--;
				num4++;
			}
			break;
		case RoofStyle.Type.Triangle:
			num10 = 999;
			num11 = 999;
			break;
		}
		for (cz = num2; cz < num4; cz++)
		{
			for (cx = num; cx < num3; cx++)
			{
				if (cx < 0 || cz < 0 || cx >= Size || cz >= Size)
				{
					continue;
				}
				int num15;
				int num16;
				if (reverse)
				{
					num15 = cz;
					num16 = cx;
					cell = map.cells[num15, num16];
					if (roofStyle.wing && cz == num4 - 1 && cell.Right.Right.room != null && cell.Right.Right.room.lot != lot)
					{
						continue;
					}
				}
				else
				{
					num15 = cx;
					num16 = cz;
					cell = map.cells[num15, num16];
					if (roofStyle.wing && cz == 0 && cell.Front.Front.room != null && cell.Front.Front.room.lot != lot)
					{
						continue;
					}
				}
				int num17 = num16 - num15;
				room = cell.room;
				if (room != null && room.lot != lot)
				{
					continue;
				}
				bool flag2 = false;
				if (roofStyle.type == RoofStyle.Type.Flat)
				{
					if (reverse)
					{
						if (!cell.HasFullBlock || cell.room != null)
						{
							num14 = ((cell.Left.room != null && cell.Left.room.lot == lot) ? ((cell.Right.room != null && cell.Right.room.lot == lot) ? 1 : ((cell.HasFullBlock && cell.Right.HasFullBlock && cell.Right.room != null) ? 1 : 2)) : (cell.Left.HasFullBlock ? 1 : 0));
						}
						else if (cell.Left.room?.lot == lot && cell.Right.room != null)
						{
							num14 = 1;
							flag2 = true;
						}
						else if (cell.Front.room?.lot == lot)
						{
							num14 = ((cell.FrontRight.room?.lot == lot) ? 1 : 2);
							flag2 = true;
						}
						else if (cell.Right.room?.lot == lot || cell.FrontRight.room?.lot == lot)
						{
							num14 = 0;
							flag2 = true;
						}
					}
					else if (!cell.HasFullBlock || cell.room != null)
					{
						num14 = ((cell.Front.room != null && cell.Front.room.lot == lot) ? ((cell.Back.room != null && cell.Back.room.lot == lot) ? 1 : (cell.Back.HasFullBlock ? 1 : 2)) : ((cell.HasFullBlock && cell.Front.HasFullBlock && cell.Front.room != null) ? 1 : 0));
					}
					else if (cell.Right.room?.lot == lot)
					{
						num14 = ((cell.FrontRight.room?.lot == lot) ? 1 : 0);
						flag2 = true;
					}
					else if (cell.Front.room?.lot == lot || cell.FrontRight.room?.lot == lot)
					{
						num14 = 2;
						flag2 = true;
					}
					num13 = 0;
				}
				bool flag3 = isSnowCovered && !cell.isClearSnow;
				if (room == null && !roofStyle.coverLot && !flag2 && !(cell.HasFullBlock && cell.IsRoomEdge && flag3))
				{
					continue;
				}
				index = cx + cz * Size;
				height = cell.TopHeight;
				float num18 = (float)num17 * screen.tileAlign.y + (float)lot.mh * _heightMod.y + lot.realHeight + roofFix.y + vector.y;
				float num19 = 1000f + param.x * screen.tileWeight.x + (float)lot.mh * _heightMod.z + lot.realHeight * roofFix3.z + roofFix.z + vector.z;
				if (lot.height == 1 && lot.heightFix < 20)
				{
					num18 += roofStyle.lowRoofFix.y;
					num19 += roofStyle.lowRoofFix.z;
				}
				param.x = (float)(cx + cz) * screen.tileAlign.x + roofFix.x + (float)num13 * roofFix2.x + vector.x * (float)(reverse ? 1 : (-1));
				param.y = num18 + (float)num13 * roofFix2.y;
				param.z = num19 + param.y * screen.tileWeight.z + (float)num13 * roofFix2.z;
				param.color = GetRoofLight(lot);
				param.snow = idRoofTile == 0 && flag3;
				param.shadowFix = 0f;
				if (num14 == 1)
				{
					SourceMaterial.Row mat = matBlock;
					RenderRow renderRow;
					if (roofStyle.type == RoofStyle.Type.FlatFloor)
					{
						if (cell.HasFullBlock && cell.IsRoomEdge && !flag3)
						{
							continue;
						}
						renderRow = EMono.sources.floors.rows[num8];
						renderRow.SetRenderParam(param, mat, 0);
						param.matColor = lot.colRoof;
					}
					else
					{
						renderRow = row;
						renderRow.SetRenderParam(param, mat, 0);
						param.matColor = lot.colBlock;
					}
					renderRow.renderData.Draw(param);
					if (idRoofTile != 0)
					{
						renderRow = EMono.sources.blocks.rows[EMono.sources.objs.rows[idRoofTile].idRoof];
						int num20 = (reverse ? 1 : 0) + ((!flag) ? 2 : 0);
						renderRow.SetRenderParam(param, MATERIAL.sourceGold, num20);
						param.matColor = lot.colRoof;
						if (roofStyle.type == RoofStyle.Type.FlatFloor)
						{
							param.x += roofStyle.posFixBlock.x;
							param.y += roofStyle.posFixBlock.y;
							param.z += roofStyle.posFixBlock.z;
						}
						if (!flag)
						{
							param.z += 0.5f;
						}
						if (flag3)
						{
							param.matColor = 104025f;
							if (roofStyle.type != RoofStyle.Type.FlatFloor)
							{
								param.z += roofStyle.snowZ;
							}
							param.tile = renderRow.renderData.ConvertTile(renderRow.snowTile) + num20;
							renderRow.renderData.Draw(param);
						}
						else
						{
							renderRow.renderData.Draw(param);
						}
					}
					else if (flag3 && roofStyle.type == RoofStyle.Type.FlatFloor)
					{
						param.matColor = 104025f;
						param.tile = 10f;
						param.x += roofStyle.snowFix.x;
						param.y += roofStyle.snowFix.y;
						param.z += roofStyle.snowZ + roofStyle.snowFix.z;
						renderRow.renderData.Draw(param);
					}
				}
				else
				{
					if (idRoofTile != 0)
					{
						int num21 = ((!reverse) ? ((num14 != 0) ? 2 : 0) : ((num14 != 0) ? 1 : 3));
						if (lot.altRoof && !flag && (roofStyle.type == RoofStyle.Type.Default || roofStyle.type == RoofStyle.Type.DefaultNoTop))
						{
							param.shadowFix = num21 + 1;
						}
						RenderRow renderRow = EMono.sources.objs.rows[idRoofTile];
						renderRow.SetRenderParam(param, MATERIAL.sourceGold, num21);
						param.matColor = lot.colRoof;
						if (flag3)
						{
							param.matColor = 104025f;
							param.z += roofStyle.snowZ;
							param.tile = renderRow.renderData.ConvertTile(renderRow.snowTile) + num21 + (lot.altRoof ? 8 : 0);
							renderRow.renderData.Draw(param);
						}
						else
						{
							param.tile += (lot.altRoof ? 8 : 0);
							renderRow.renderData.Draw(param);
						}
						param.shadowFix = 0f;
					}
					if (num13 >= 0)
					{
						param.y += roofRampFix.y;
						param.z += roofRampFix.z;
						RenderRow renderRow = EMono.sources.blocks.rows[num9];
						renderRow.SetRenderParam(param, MATERIAL.sourceGold, (!reverse) ? ((num14 != 0) ? 2 : 0) : ((num14 != 0) ? 1 : 3));
						param.matColor = lot.colBlock;
						renderRow.renderData.Draw(param);
					}
				}
				CellEffect effect = cell.effect;
				if (effect != null && effect.FireAmount > 0)
				{
					rendererEffect.Draw(param, cell.effect.FireAmount);
				}
				if (num13 < 1)
				{
					continue;
				}
				if (roofStyle.type != RoofStyle.Type.Flat)
				{
					param.snow = false;
				}
				for (int i = 0; i < num13; i++)
				{
					param.x = (float)(cx + cz) * screen.tileAlign.x + roofFix.x + (float)i * roofFix2.x + vector.x * (float)(reverse ? 1 : (-1));
					param.y = num18 + (float)i * roofFix2.y;
					param.z = num19 + param.y * screen.tileWeight.z + (float)i * roofFix2.z;
					RenderRow renderRow = row;
					renderRow.SetRenderParam(param, MATERIAL.sourceGold, 0);
					param.matColor = lot.colBlock;
					renderRow.renderData.Draw(param);
					index++;
					CellEffect effect2 = cell.effect;
					if (effect2 != null && effect2.FireAmount > 0 && Rand.bytes[index % Rand.MaxBytes] % 3 == 0)
					{
						rendererEffect.Draw(param, cell.effect.FireAmount);
					}
				}
			}
			num12++;
			if (roofStyle.type != RoofStyle.Type.Flat)
			{
				if (num12 == num10)
				{
					num14 = 1;
				}
				if (num12 == num11)
				{
					num14 = 2;
					num13++;
				}
				num13 += num14 switch
				{
					1 => 0, 
					0 => 1, 
					_ => -1, 
				};
			}
		}
	}

	public static int GetColorInt(ref Color matColor, int p)
	{
		if (p == 0)
		{
			return 104025;
		}
		return p * 262144 + (int)(matColor.r * 50f) * 4096 + (int)(matColor.g * 50f) * 64 + (int)(matColor.b * 50f);
	}

	public void SetRoofHeight(MeshPassParam _param, Cell _cell, int _cx, int _cz, int h = 0, int altitude = 0, int dirWall = -1, bool ignoreAltitudeY = false)
	{
		Room room = _cell.room;
		if (room == null && dirWall != -1)
		{
			if (dirWall == 0 && _cell.Front.room != null)
			{
				room = _cell.Front.room;
				_cell = _cell.Front;
			}
			else if (_cell.Right.room != null)
			{
				room = _cell.Right.room;
				_cell = _cell.Right;
			}
		}
		if (room != null)
		{
			Lot lot = room.lot;
			RoofStyle roofStyle = roofStyles[lot.idRoofStyle];
			float num = (float)(_cz - _cx) * screen.tileAlign.y + (float)lot.mh * _heightMod.y + lot.realHeight + roofFix.y;
			float num2 = 1000f + param.x * screen.tileWeight.x + (float)lot.mh * _heightMod.z + lot.realHeight * roofFix3.z + roofFix.z + roofStyle.posFix.z;
			if (lot.height == 1)
			{
				num += roofStyle.lowRoofFix.y;
				num2 += roofStyle.lowRoofFix.z;
			}
			_param.x = (float)(_cx + _cz) * screen.tileAlign.x + roofFix.x + (float)h * roofFix2.x;
			_param.y = num + (float)h * roofFix2.y;
			_param.z = num2 + _param.y * screen.tileWeight.z + (float)h * roofFix2.z + heightModRoofBlock.y;
		}
		if (!ignoreAltitudeY || room != null)
		{
			_param.y += (float)altitude * _heightMod.y;
		}
		_param.z += (float)altitude * heightModRoofBlock.z;
	}
}
