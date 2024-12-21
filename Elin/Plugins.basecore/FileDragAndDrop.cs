using System;
using System.Collections.Generic;
using B83.Win32;
using UnityEngine;

public class FileDragAndDrop : MonoBehaviour
{
	public static Action<List<string>> onDrop;

	private void OnEnable()
	{
		UnityDragAndDropHook.InstallHook();
		UnityDragAndDropHook.OnDroppedFiles += OnFiles;
	}

	private void OnDisable()
	{
		UnityDragAndDropHook.UninstallHook();
	}

	private void OnFiles(List<string> aFiles, POINT aPos)
	{
		if (onDrop != null)
		{
			onDrop(aFiles);
		}
	}
}
