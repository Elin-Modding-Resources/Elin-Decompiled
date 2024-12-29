using System;
using UnityEngine;

public class UIHighlightObject : MonoBehaviour
{
	public Func<bool> ShouldDestroy;

	public Action OnUpdate;

	public Transform trans;

	public CanvasGroup cg;

	private void Update()
	{
		if (ShouldDestroy != null && ShouldDestroy())
		{
			UnityEngine.Object.DestroyImmediate(base.gameObject);
		}
		else
		{
			OnUpdate();
		}
	}
}
