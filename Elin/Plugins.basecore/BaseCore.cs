using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BaseCore : MonoBehaviour
{
	public static bool IsOffline;

	public static BaseCore Instance;

	public static bool resetRuntime;

	public static Func<bool> BlockInput;

	public Version version;

	public Version versionMoongate;

	public Version versionMod;

	public ReleaseMode releaseMode;

	public string langCode;

	public EventSystem eventSystem;

	public List<Action> actionsLateUpdate = new List<Action>();

	public List<Action> actionsNextFrame = new List<Action>();

	[NonSerialized]
	public Canvas canvas;

	[NonSerialized]
	public string forceLangCode;

	[NonSerialized]
	public int frame;

	protected int lastScreenWidth;

	protected int lastScreenHeight;

	public virtual float uiScale => 1f;

	protected virtual void Awake()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("Temp");
		for (int i = 0; i < array.Length; i++)
		{
			UnityEngine.Object.DestroyImmediate(array[i]);
		}
	}

	public virtual void ConsumeInput()
	{
	}

	public void WaitForEndOfFrame(Action action)
	{
		StartCoroutine(_WaitForEndOfFrame(action));
	}

	private IEnumerator _WaitForEndOfFrame(Action action)
	{
		yield return new WaitForEndOfFrame();
		action();
	}

	public virtual void StopEventSystem(float duration = 0.2f)
	{
	}

	public virtual void StopEventSystem(Component c, Action action, float duration = 0.15f)
	{
	}

	public virtual void FreezeScreen(float duration)
	{
	}

	public virtual void UnfreezeScreen()
	{
	}

	public virtual void RebuildBGMList()
	{
	}
}
