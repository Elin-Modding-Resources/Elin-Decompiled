using System.Collections.Generic;
using System.IO;
using B83.Win32;
using UnityEngine;

public class ImageExample : MonoBehaviour
{
	private class DropInfo
	{
		public string file;

		public Vector2 pos;
	}

	private Texture2D[] textures = new Texture2D[6];

	private DropInfo dropInfo;

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
		string text = "";
		foreach (string aFile in aFiles)
		{
			switch (new FileInfo(aFile).Extension.ToLower())
			{
			case ".png":
			case ".jpg":
			case ".jpeg":
				text = aFile;
				goto end_IL_0053;
			}
			continue;
			end_IL_0053:
			break;
		}
		if (text != "")
		{
			DropInfo dropInfo = new DropInfo
			{
				file = text,
				pos = new Vector2(aPos.x, aPos.y)
			};
			this.dropInfo = dropInfo;
		}
	}

	private void LoadImage(int aIndex, DropInfo aInfo)
	{
		if (aInfo != null && GUILayoutUtility.GetLastRect().Contains(aInfo.pos))
		{
			byte[] data = File.ReadAllBytes(aInfo.file);
			Texture2D texture2D = new Texture2D(1, 1);
			texture2D.LoadImage(data);
			if (textures[aIndex] != null)
			{
				Object.Destroy(textures[aIndex]);
			}
			textures[aIndex] = texture2D;
		}
	}

	private void OnGUI()
	{
		DropInfo aInfo = null;
		if (Event.current.type == EventType.Repaint && dropInfo != null)
		{
			aInfo = dropInfo;
			dropInfo = null;
		}
		GUILayout.BeginHorizontal();
		for (int i = 0; i < 3; i++)
		{
			if (textures[i] != null)
			{
				GUILayout.Label(textures[i], GUILayout.Width(200f), GUILayout.Height(200f));
			}
			else
			{
				GUILayout.Box("Drag image here", GUILayout.Width(200f), GUILayout.Height(200f));
			}
			LoadImage(i, aInfo);
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		for (int j = 3; j < 6; j++)
		{
			if (textures[j] != null)
			{
				GUILayout.Label(textures[j], GUILayout.Width(200f), GUILayout.Height(200f));
			}
			else
			{
				GUILayout.Box("Drag image here", GUILayout.Width(200f), GUILayout.Height(200f));
			}
			LoadImage(j, aInfo);
		}
		GUILayout.EndHorizontal();
	}
}
