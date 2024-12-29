using System;
using UnityEngine;

public class UIFollow : MonoBehaviour
{
	public Transform target;

	public Vector3 fixPos;

	public bool followPos;

	public Func<bool> endCondition;

	public void SetTarget(Transform _trans)
	{
		target = _trans;
	}

	public void LateUpdate()
	{
		if (followPos)
		{
			GameObject go = base.gameObject;
			BaseCore.Instance.actionsLateUpdate.Add(delegate
			{
				if (go != null)
				{
					base.transform.Rect().anchoredPosition = Camera.main.WorldToScreenPoint(fixPos) / BaseCore.Instance.uiScale;
				}
			});
		}
		else if (!target || !target.gameObject.activeSelf || (endCondition != null && endCondition()))
		{
			UnityEngine.Object.DestroyImmediate(base.gameObject);
		}
		else
		{
			base.transform.position = target.position + fixPos;
		}
	}
}
