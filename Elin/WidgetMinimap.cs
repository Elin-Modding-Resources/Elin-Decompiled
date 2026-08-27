using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WidgetMinimap : Widget, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IDragHandler, IChangeResolution, IInitializePotentialDragHandler
{
	public class Extra
	{
		public bool rotate;

		public bool monoColor;

		public bool limitBounds;

		public int alpha;

		public int scale;

		public int width;
	}

	public static WidgetMinimap Instance;

	public float interval;

	public float intervalPS;

	public float psSize;

	public RectTransform rectAll;

	public RectTransform rectMap;

	public Vector2 baseSize;

	private Point pos = new Point();

	public UIButton buttonMaptool;

	public UIMapPreview preview;

	public Color colorAlly;

	public Color colorEnemy;

	public Color colorGuest;

	public ParticleSystem psAlly;

	public ParticleSystem psEnemy;

	public Transform transBound;

	public Vector2 posPs;

	public Transform transCam;

	private Point point = new Point();

	private int count;

	public override Type SetSiblingAfter => typeof(WidgetBottomBar);

	public Extra extra => base.config.extra as Extra;

	public int Size => preview.Size;

	public override bool AlwaysBottom => true;

	public override object CreateExtra()
	{
		return new Extra();
	}

	public override void OnActivate()
	{
		Instance = this;
		preview.limitBounds = extra.limitBounds;
		preview.monoColor = extra.monoColor;
		preview.SetMap(EMono._map);
		InvokeRepeating("RefreshMarkers", 0f, interval);
		RefreshStyle();
		buttonMaptool.SetItem(new HotItemContext
		{
			id = "mapTool"
		});
	}

	public static void UpdateMap(List<Cell> newPoints)
	{
		if ((bool)Instance && Instance.preview.map.zone.IsActiveZone)
		{
			Instance.preview.UpdateMap(newPoints);
		}
		newPoints.Clear();
	}

	public static void UpdateMap(int x, int z)
	{
		if ((bool)Instance && Instance.preview.map.zone.IsActiveZone)
		{
			Instance.preview.UpdateMap(x, z);
		}
	}

	public static void UpdateMap()
	{
		if ((bool)Instance && Instance.preview.map.zone.IsActiveZone)
		{
			Instance.preview.SetMap(EMono._map);
		}
	}

	public void OnMoveZone()
	{
		preview.SetMap(EMono._map);
		RefreshStyle();
	}

	public void Reload()
	{
		preview.SetMap(preview.map);
	}

	public void OnPointerDown(PointerEventData e)
	{
		if (!EMono.pc.HasNoGoal || EMono.game.activeZone.IsRegion || EMono.ui.IsActive)
		{
			SE.BeepSmall();
			return;
		}
		Vector2 size = rectMap.rect.size;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(rectMap, e.position, e.pressEventCamera, out var localPoint);
		if (localPoint.x < 0f)
		{
			localPoint.x = 0f;
		}
		else if (localPoint.x >= size.x)
		{
			localPoint.x = size.x;
		}
		if (localPoint.y < 0f)
		{
			localPoint.y = 0f;
		}
		else if (localPoint.y >= size.y)
		{
			localPoint.y = size.y;
		}
		pos.Set((int)((float)Size * localPoint.x / size.x), (int)((float)Size * localPoint.y / size.y));
		pos.Clamp();
		if (preview.limitBounds)
		{
			pos.x += EMono._map.bounds.x - preview.offsetX;
			pos.z += EMono._map.bounds.z - preview.offsetZ;
		}
		if (pos.cell.outOfBounds || !pos.cell.isSeen)
		{
			SE.BeepSmall();
		}
		else
		{
			EMono.pc.SetAIImmediate(new AI_Goto(pos, 0));
		}
	}

	public void OnPointerUp(PointerEventData e)
	{
	}

	public void OnInitializePotentialDrag(PointerEventData ped)
	{
		ped.useDragThreshold = false;
	}

	public void OnDrag(PointerEventData e)
	{
	}

	public override void OnSetContextMenu(UIContextMenu m)
	{
		UIContextMenu uIContextMenu = m.AddChild("setting");
		uIContextMenu.AddSlider("size", (float a) => extra.scale.ToString() ?? "", extra.scale, delegate(float a)
		{
			extra.scale = (int)a;
			RefreshStyle();
		}, 5f, 20f, isInt: true);
		uIContextMenu.AddSlider("width", (float a) => 100 + extra.width + "%", extra.width, delegate(float a)
		{
			extra.width = (int)a;
			RefreshStyle();
		}, -25f, 25f, isInt: true);
		uIContextMenu.AddSlider("alpha", (float a) => extra.alpha.ToString() ?? "", extra.alpha, delegate(float a)
		{
			extra.alpha = (int)a;
			RefreshStyle();
		}, 0f, 255f, isInt: true);
		uIContextMenu.AddToggle("rotation", extra.rotate, delegate(bool a)
		{
			extra.rotate = a;
			RefreshStyle();
		});
		uIContextMenu.AddToggle("monoColor", extra.monoColor, delegate(bool a)
		{
			extra.monoColor = (preview.monoColor = a);
			preview.SetMap(EMono._map);
			RefreshStyle();
		});
		SetBaseContextMenu(m);
	}

	public void RefreshStyle()
	{
		rectAll.localEulerAngles = ((extra.rotate && !EMono._zone.IsRegion) ? new Vector3(60f, 0f, -45f) : Vector3.zero);
		preview.matMap.SetColor("_Color", new Color(1f, 1f, 1f, (float)extra.alpha / 255f));
		this.Rect().sizeDelta = new Vector2(0.1f * baseSize.x * (float)(10 + extra.scale) * (float)(100 + extra.width) * 0.01f, 0.1f * baseSize.y * (float)(10 + extra.scale));
	}

	public void OnUpdate()
	{
		point.Set(EMono.screen.position);
		if (preview.limitBounds)
		{
			point.x -= EMono._map.bounds.x - preview.offsetX;
			point.z -= EMono._map.bounds.z - preview.offsetZ;
		}
		float x = (float)(point.x / Size) - 0.5f;
		float y = (float)(point.z / Size) - 0.5f;
		transBound.localScale = new Vector3(Mathf.Min((float)EMono.screen.width / (float)Size * 1.5f, 1f), Mathf.Min((float)EMono.screen.height / (float)Size / 2f, 1f), 1f);
		transBound.localPosition = new Vector3(x, y, 50f);
		transCam.localScale = new Vector3(1f / EMono.core.uiScale, 1f / EMono.core.uiScale, 1f);
	}

	public void RefreshMarkers()
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		psAlly.Clear();
		EmitParticle(EMono.pc, psAlly, colorAlly);
		if (!EMono._zone.ShowEnemyOnMinimap)
		{
			return;
		}
		foreach (Chara chara in EMono._map.charas)
		{
			if (chara.IsHostile())
			{
				EmitParticle(chara, psEnemy, colorEnemy);
			}
		}
	}

	public void EmitParticle(Card c, ParticleSystem ps, Color col)
	{
		int num = c.pos.x;
		int num2 = c.pos.z;
		float z = (c.IsPCFactionOrMinion ? 9f : (10f + 0.01f * (float)(c.pos.z * 200 + c.pos.x)));
		if (preview.limitBounds)
		{
			num -= EMono._map.bounds.x - preview.offsetX;
			num2 -= EMono._map.bounds.z - preview.offsetZ;
		}
		float x = (float)num / (float)Size - 0.5f;
		float y = (float)num2 / (float)Size - 0.5f;
		count++;
		ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams
		{
			position = new Vector3(x, y, z),
			startSize = psSize,
			startLifetime = intervalPS,
			startColor = col
		};
		psAlly.Emit(emitParams, 1);
	}
}
