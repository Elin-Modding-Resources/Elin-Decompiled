﻿using System;
using UnityEngine;

public class ScreenGuide : EMono
{
	public void DrawLine(Vector3 from, Vector3 to)
	{
		this.lr.positionCount = 2;
		from.z = (to.z = -300f);
		this.lr.SetPosition(0, from);
		this.lr.SetPosition(1, to);
	}

	public void DrawFloor(Point pos, int tile)
	{
	}

	public void DrawBlock(Point pos, int tile)
	{
	}

	public void OnDrawPass()
	{
	}

	public void OnEndOfFrame()
	{
		this.lr.positionCount = 0;
	}

	public unsafe void DrawWall(Point point, int color, bool useMarkerPass = false, float offsetZ = 0f)
	{
		int num = -1;
		if (num != -1)
		{
			Vector3 vector = *point.Position();
			EMono.screen.guide.passGuideFloor.Add(vector.x, vector.y, vector.z - 0.01f, 0f, 0f);
		}
		SourceBlock.Row sourceBlock = point.sourceBlock;
		RenderParam renderParam = sourceBlock.GetRenderParam(point.matBlock, point.cell.blockDir, point, num);
		renderParam.matColor = (float)color;
		renderParam.z -= 0.01f;
		if (useMarkerPass)
		{
			renderParam.x += sourceBlock.renderData.offset.x;
			renderParam.y += sourceBlock.renderData.offset.y;
			renderParam.z += sourceBlock.renderData.offset.z + offsetZ;
			this.passBlockMarker.Add(renderParam);
		}
		else
		{
			sourceBlock.renderData.Draw(renderParam);
		}
		if (point.cell.blockDir == 2)
		{
			renderParam.tile *= -1f;
			if (useMarkerPass)
			{
				this.passBlockMarker.Add(renderParam);
				return;
			}
			sourceBlock.renderData.Draw(renderParam);
		}
	}

	public MeshPass passGuideBlock;

	public MeshPass passGuideFloor;

	public MeshPass passArea;

	public MeshPass passBlockMarker;

	public LineRenderer lr;

	public bool isActive = true;
}