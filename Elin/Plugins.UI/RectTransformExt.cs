using UnityEngine;

public static class RectTransformExt
{
	public static void GetLocalCorners(this RectTransform rt, Vector3[] fourCornersArray, Canvas canvas, float insetL, float insetR, float insetY)
	{
		rt.GetLocalCorners(fourCornersArray);
		Vector3 vector = canvas.CorrectLossyScale();
		fourCornersArray[0].x += insetL * vector.x;
		fourCornersArray[0].y += insetY * vector.y;
		fourCornersArray[1].x += insetL * vector.x;
		fourCornersArray[1].y -= insetY * vector.y;
		fourCornersArray[2].x -= insetR * vector.x;
		fourCornersArray[2].y -= insetY * vector.y;
		fourCornersArray[3].x -= insetR * vector.x;
		fourCornersArray[3].y += insetY * vector.y;
	}

	public static void GetScreenCorners(this RectTransform rt, Vector3[] fourCornersArray, Canvas canvas, float insetL, float insetR, float insetY)
	{
		rt.GetWorldCorners(fourCornersArray);
		if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
		{
			for (int i = 0; i < 4; i++)
			{
				fourCornersArray[i] = canvas.worldCamera.WorldToScreenPoint(fourCornersArray[i]);
				fourCornersArray[i].z = 0f;
			}
		}
		Vector3 vector = canvas.CorrectLossyScale();
		fourCornersArray[0].x += insetL * vector.x;
		fourCornersArray[0].y += insetY * vector.y;
		fourCornersArray[1].x += insetL * vector.x;
		fourCornersArray[1].y -= insetY * vector.y;
		fourCornersArray[2].x -= insetR * vector.x;
		fourCornersArray[2].y -= insetY * vector.y;
		fourCornersArray[3].x -= insetR * vector.x;
		fourCornersArray[3].y += insetY * vector.y;
	}
}
