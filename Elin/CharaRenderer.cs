using UnityEngine;

public class CharaRenderer : CardRenderer
{
	public new Chara owner;

	public Vector3 orgPos;

	private float moveTimer;

	private float nextframeTimer;

	public bool first = true;

	public bool ignoreFirst;

	public Point movePoint = new Point();

	public AnimationCurve hopCurve;

	public PCCData pccData;

	private float actTime = 0.3f;

	private float lastShadowFix;

	private float lastShadowFix2;

	public int step;

	public bool isMoving;

	public static int _animeFramePC;

	public static int _animeFramePCC;

	public static int _animeFrame;

	public static bool smoothmove;

	public int currentDir
	{
		get
		{
			if (!hasActor)
			{
				if (!owner.flipX)
				{
					return 1;
				}
				return 0;
			}
			return actor.currentDir;
		}
	}

	public override bool IsMoving => isMoving;

	public override void SetOwner(Card c)
	{
		owner = c as Chara;
		if (pccData != null)
		{
			data = ((pccData.GetBodySet2().id == "unique") ? EClass.core.refs.renderers.pcc_L : EClass.core.refs.renderers.pcc);
		}
		base.SetOwner(c);
	}

	public override void OnEnterScreen()
	{
		base.OnEnterScreen();
		if (!ignoreFirst)
		{
			first = true;
		}
		ignoreFirst = false;
		nextframeTimer = 0f;
		if (EClass.core.config.game.haltOnSpotEnemy && owner.ExistsOnMap && !EClass._zone.IsRegion && owner.IsHostile() && EClass.pc.CanSeeLos(owner))
		{
			EClass.player.enemySpotted = true;
		}
	}

	public override void Draw(RenderParam p, ref Vector3 v, bool drawShadow)
	{
		base.Draw(p, ref v, drawShadow);
		if ((Zone.sourceHat != null || owner.hat != null) && owner.Pref.hatY != 0f && owner.host == null)
		{
			DrawHat();
		}
		if (owner.IsPC)
		{
			if (owner.held != null && !owner.IsDeadOrSleeping && (!owner.held.trait.ShowAsTool || HotItemHeld.disableTool))
			{
				DrawHeld();
			}
		}
		else if (owner.held != null && !owner.IsDeadOrSleeping && !owner.held.trait.ShowAsTool)
		{
			DrawHeld();
		}
	}

	public override void NextFrame()
	{
		base.NextFrame();
		owner.idleTimer = RenderObject.animeSetting.idleTime;
	}

	public override void UpdatePosition(ref Vector3 destPos, RenderParam p)
	{
		bool isPC = owner.IsPC;
		int num = (isPC ? _animeFramePC : (hasActor ? _animeFramePCC : _animeFrame));
		bool flag = isPC || num >= 10;
		if (num == 0 || Scene.skipAnime || first || !data.animate || (owner.IsDeadOrSleeping && pccData != null && !owner.IsPC))
		{
			first = false;
			position = destPos;
			if (isPC)
			{
				EClass.player.position = position;
			}
			movePoint.Set(owner.pos);
			step = 9999999;
			isMoving = false;
			moveTimer = 0f;
			RefreshSprite();
			p.x = position.x;
			p.y = position.y;
			p.z = position.z;
			lastShadowFix = (lastShadowFix2 = p.shadowFix);
			return;
		}
		if (!owner.pos.Equals(movePoint))
		{
			if (RenderObject.gameSpeed > 1f)
			{
				p.shadowFix = (lastShadowFix = 0f);
			}
			else
			{
				p.shadowFix = (lastShadowFix = lastShadowFix2);
			}
			movePoint.Set(owner.pos);
			orgPos = position;
			actTime = owner.actTime;
			RefreshSprite();
			step = 1;
			if (!isMoving)
			{
				isMoving = true;
				nextframeTimer = (RenderObject.animeSetting.nextFrameInterval + actTime / 4f) / 2f;
			}
			moveTimer = 0f;
		}
		if (num < 100)
		{
			num = (int)((float)num / 0.3f * actTime);
		}
		float num2 = (actTime + ((!isPC && owner.IsPCParty) ? RenderObject.animeSetting.animeExtraTimeParty : RenderObject.animeSetting.animeExtraTime) * RenderObject.gameSpeed) / (float)num;
		moveTimer += RenderObject.gameDelta;
		if (step >= num)
		{
			if (position == destPos)
			{
				if (isMoving && isPC)
				{
					EClass.player.position = position;
				}
				isMoving = false;
				moveTimer = 0f;
			}
			else if (!flag)
			{
				position = destPos;
			}
			else if (moveTimer >= num2)
			{
				int num3 = (int)(moveTimer / num2);
				moveTimer -= (float)num3 * num2;
				if (Vector3.Distance(position, destPos) < RenderObject.animeSetting.destRadius)
				{
					position = destPos;
					if (isMoving && isPC)
					{
						EClass.player.position = position;
					}
					isMoving = false;
				}
				else
				{
					position.z = destPos.z;
					Vector3 vector = Vector3.MoveTowards(position, destPos, (float)num3 * RenderObject.animeSetting.slowSpeed / (float)step);
					position = vector;
				}
			}
			if (owner.idleTimer > 0f)
			{
				owner.idleTimer -= RenderObject.gameDelta;
				if (flag && owner.idleTimer <= 0f)
				{
					IdleFrame();
					nextframeTimer = 0f;
				}
			}
			p.x = position.x;
			p.y = position.y;
			p.z = position.z;
			lastShadowFix = (lastShadowFix2 = p.shadowFix);
			if (isPC)
			{
				CellDetail detail = EClass.pc.Cell.detail;
				if (detail == null || detail.anime == null)
				{
					position.z = destPos.z;
					Vector3 vector2 = Vector3.MoveTowards(position, destPos, 0.1f);
					position = vector2;
					EClass.player.position = Vector3.Lerp(EClass.player.position, position, 0.6f);
				}
			}
			return;
		}
		if (moveTimer >= num2)
		{
			int num4 = (int)(moveTimer / num2);
			step += num4;
			moveTimer -= (float)num4 * num2;
			if (step >= num)
			{
				step = num;
			}
		}
		owner.idleTimer = RenderObject.animeSetting.idleTime;
		float num5 = (float)step / (float)num;
		p.shadowFix = p.shadowFix * num5 + lastShadowFix * (1f - num5);
		lastShadowFix2 = p.shadowFix;
		Vector3 vector3 = orgPos + (destPos - orgPos) * num5 * ((flag && smoothmove) ? RenderObject.animeSetting.fixedMove : 1f);
		if (destPos.z < orgPos.z)
		{
			vector3.z = orgPos.z + (destPos.z - orgPos.z) * RenderObject.animeSetting.gradientZForward.Evaluate(num5);
		}
		else
		{
			vector3.z = orgPos.z + (destPos.z - orgPos.z) * RenderObject.animeSetting.gradientZBack.Evaluate(num5);
		}
		position = vector3;
		if (hasActor)
		{
			if (EClass.core.config.graphic.spriteFrameMode == 0)
			{
				nextframeTimer += RenderObject.gameDelta * 0.5f;
				float nextFrameInterval = RenderObject.animeSetting.nextFrameInterval;
				if (nextframeTimer > nextFrameInterval)
				{
					if (owner.ai is AI_Trolley)
					{
						actor.IdleFrame();
					}
					else
					{
						actor.NextFrame();
					}
					nextframeTimer -= nextFrameInterval;
				}
			}
			else
			{
				nextframeTimer += RenderObject.gameDelta;
				float num6 = RenderObject.animeSetting.nextFrameInterval + actTime / 4f * (0.5f + RenderObject.gameSpeed / 2f);
				if (nextframeTimer > num6)
				{
					if (owner.ai is AI_Trolley)
					{
						actor.IdleFrame();
					}
					else
					{
						actor.NextFrame();
					}
					nextframeTimer -= num6;
				}
			}
		}
		p.x = position.x;
		p.y = position.y;
		p.z = position.z;
		if (isPC)
		{
			EClass.player.position = vector3;
		}
		else if (!hasActor && num >= 5 && hopCurve != null)
		{
			p.y += hopCurve.Evaluate(num5) * RenderObject.animeSetting.hopStrength;
		}
	}

	public void DrawHat()
	{
		if (pccData == null || !owner.IsDeadOrSleeping)
		{
			CardRow cardRow = Zone.sourceHat ?? owner.hat;
			SourcePref pref = GetPref();
			bool flag = currentDir == 1 || currentDir == 3;
			int liquidLv = RenderObject.currentParam.liquidLv;
			float num = ((replacer != null) ? replacer.pref.hatY : pref.hatY);
			if (pccData != null)
			{
				num += RenderObject.renderSetting.hatPos[actor.GetFrame()].y;
			}
			RenderObject.currentParam.liquidLv = 0;
			RenderObject.currentParam.x += 0.01f * (float)pref.equipX;
			RenderObject.currentParam.y += num + 0.01f * (float)pref.equipY;
			RenderObject.currentParam.z -= pref.hatY;
			RenderObject.currentParam.tile = cardRow._tiles[owner.uid % cardRow._tiles.Length] * ((!flag) ? 1 : (-1));
			cardRow.renderData.Draw(RenderObject.currentParam);
			RenderObject.currentParam.y -= num;
			RenderObject.currentParam.liquidLv = liquidLv;
		}
	}

	public override void DrawHeld()
	{
		Card held = owner.held;
		int num = currentDir;
		RenderData renderData = held.renderer.data;
		SourcePref pref = held.Pref;
		bool flag = num == 1 || num == 3;
		if (held.isChara)
		{
			Vector3[] array = (EClass.player.altHeldPos ? RenderObject.renderSetting.heldPosChara2 : RenderObject.renderSetting.heldPosChara);
			RenderObject.currentParam.x += array[num].x;
			RenderObject.currentParam.y += array[num].y;
			RenderObject.currentParam.z += array[num].z + data.offset.z - renderData.offset.z;
			if (EClass.player.altHeldPos)
			{
				held.SetDir((owner.dir == 0) ? 2 : ((owner.dir == 1) ? 1 : ((owner.dir != 2) ? 3 : 0)));
			}
			else
			{
				held.SetDir((owner.dir == 0) ? 2 : owner.dir);
			}
		}
		else
		{
			Vector3[] heldPos = RenderObject.renderSetting.heldPos;
			if (hasActor)
			{
				RenderObject.currentParam.x += heldPos[num].x;
				RenderObject.currentParam.y += heldPos[num].y + 0.02f * (float)(actor.GetFrame() % 2);
				RenderObject.currentParam.z += heldPos[num].z + data.offset.z - renderData.offset.z;
			}
			else
			{
				RenderObject.currentParam.x += heldPos[4].x;
				RenderObject.currentParam.y += heldPos[4].y;
				RenderObject.currentParam.z += heldPos[4].z + data.offset.z - renderData.offset.z;
			}
			if (held.trait.NoHeldDir || !held.sourceCard.ContainsTag("noHeldDir"))
			{
				held.dir = (owner.flipX ? 1 : 0);
			}
		}
		if (!owner.Cell.sourceSurface.tileType.IsDeepWater)
		{
			int liquidLv = RenderObject.currentParam.liquidLv;
			if (held.isChara)
			{
				RenderObject.currentParam.liquidLv = 0;
			}
			held.SetRenderParam(RenderObject.currentParam);
			RenderObject.currentParam.x += renderData.heldPos.x - renderData.offset.x + (flag ? 0.01f : (-0.01f)) * (float)pref.equipX;
			RenderObject.currentParam.y += renderData.heldPos.y - renderData.offset.y + 0.01f * (float)pref.equipY;
			RenderObject.currentParam.z += renderData.heldPos.z;
			RenderObject.tempV.x = RenderObject.currentParam.x;
			RenderObject.tempV.y = RenderObject.currentParam.y;
			RenderObject.tempV.z = RenderObject.currentParam.z;
			held.renderer.Draw(RenderObject.currentParam, ref RenderObject.tempV, drawShadow: false);
			RenderObject.currentParam.liquidLv = liquidLv;
		}
	}

	public override void RefreshStateIcon()
	{
		if (!isSynced)
		{
			return;
		}
		TCState tCState = GetTC<TCState>();
		MultiSprite stateIcon = owner.ai.GetStateIcon();
		if (stateIcon == null)
		{
			if ((bool)tCState)
			{
				RemoveTC(tCState);
			}
			return;
		}
		if (!tCState)
		{
			tCState = AddTC<TCState>(PoolManager.Spawn(EClass.core.refs.tcs.state));
		}
		tCState.SetSprite(stateIcon);
	}

	public override void SetFirst(bool first)
	{
		this.first = first;
		ignoreFirst = !first;
	}

	public override void SetFirst(bool first, Vector3 pos)
	{
		this.first = first;
		ignoreFirst = !first;
		position = pos;
		if (owner.IsPC)
		{
			EClass.player.position = position;
		}
		movePoint.Set(owner.pos);
		step = 9999999;
		isMoving = false;
		moveTimer = 0f;
	}

	public override void Refresh()
	{
	}
}
