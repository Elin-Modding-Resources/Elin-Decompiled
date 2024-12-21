using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class Util
{
	public static Color[] alphaMap;

	public static Canvas canvas;

	static Util()
	{
		alphaMap = new Color[256];
		for (int i = 0; i < 256; i++)
		{
			alphaMap[i] = new Color(1f, 1f, 1f, 1f * (float)i / 255f);
		}
	}

	public static int Distance(int pX1, int pY1, int pX2, int pY2)
	{
		return (int)Math.Sqrt((pX1 - pX2) * (pX1 - pX2) + (pY1 - pY2) * (pY1 - pY2));
	}

	public static T CopyComponent<T>(T original, GameObject destination) where T : Component
	{
		Type type = original.GetType();
		T val = destination.GetComponent(type) as T;
		if (!val)
		{
			val = destination.AddComponent(type) as T;
		}
		FieldInfo[] fields = type.GetFields();
		foreach (FieldInfo fieldInfo in fields)
		{
			if (!fieldInfo.IsStatic)
			{
				fieldInfo.SetValue(val, fieldInfo.GetValue(original));
			}
		}
		PropertyInfo[] properties = type.GetProperties();
		foreach (PropertyInfo propertyInfo in properties)
		{
			if (propertyInfo.CanWrite && propertyInfo.CanWrite && !(propertyInfo.Name == "name") && !(propertyInfo.Name == "usedByComposite") && !(propertyInfo.Name == "density"))
			{
				propertyInfo.SetValue(val, propertyInfo.GetValue(original, null), null);
			}
		}
		return val;
	}

	public static T Instantiate<T>(T t, Component parent = null) where T : Component
	{
		T val = UnityEngine.Object.Instantiate(t);
		val.transform.SetParent(parent ? parent.transform : null, worldPositionStays: false);
		if (!val.gameObject.activeSelf)
		{
			val.gameObject.SetActive(value: true);
		}
		return val;
	}

	public static Transform Instantiate(string path, Component parent = null)
	{
		return Instantiate<Transform>(path, parent);
	}

	public static T Instantiate<T>(string path, Component parent = null) where T : Component
	{
		T val = Resources.Load<T>(path);
		if (val == null)
		{
			return null;
		}
		return Instantiate(val, parent);
	}

	public static void DestroyChildren(Transform trans)
	{
		for (int num = trans.childCount - 1; num >= 0; num--)
		{
			UnityEngine.Object.Destroy(trans.GetChild(num).gameObject);
		}
	}

	public static void RebuildLayoutInParents(Component c, Component root)
	{
		Transform transform = c.transform;
		transform.RebuildLayout();
		Transform parent = transform.parent;
		if ((bool)parent && !(parent == root.transform))
		{
			RebuildLayoutInParents(parent, root);
		}
	}

	public static void SetRectToFitScreen(RectTransform rect)
	{
		rect.anchorMin = Vector2.zero;
		rect.anchorMax = Vector2.one;
		rect.localPosition = Vector3.zero;
		rect.anchoredPosition = Vector2.zero;
		rect.sizeDelta = Vector2.zero;
	}

	public static void ClampToScreen(RectTransform rect, int margin = 0)
	{
		RectTransform rectTransform = BaseCore.Instance.canvas.Rect();
		Vector3 localPosition = rect.localPosition;
		Vector3 vector = rectTransform.rect.min - rect.rect.min;
		Vector3 vector2 = rectTransform.rect.max - rect.rect.max;
		localPosition.x = Mathf.Clamp(localPosition.x, vector.x + (float)margin, vector2.x - (float)margin);
		localPosition.y = Mathf.Clamp(localPosition.y, vector.y + (float)margin, vector2.y - (float)margin);
		rect.localPosition = localPosition;
	}

	public static float GetAngle(Vector3 self, Vector3 target)
	{
		Vector3 vector = target - self;
		return Mathf.Atan2(vector.y * -1f, vector.x) * 57.29578f + 90f;
	}

	public static float GetAngle(float x, float y)
	{
		return Mathf.Atan2(0f - y, x) * 57.29578f + 90f;
	}

	public static VectorDir GetVectorDir(Vector3 normal)
	{
		if (Math.Abs(normal.y - 1f) < 0.1f)
		{
			return VectorDir.down;
		}
		if (Math.Abs(normal.x - 1f) < 0.1f)
		{
			return VectorDir.left;
		}
		if (Math.Abs(normal.x - -1f) < 0.1f)
		{
			return VectorDir.right;
		}
		if (Math.Abs(normal.z - 1f) < 0.1f)
		{
			return VectorDir.back;
		}
		if (Math.Abs(normal.z - -1f) < 0.1f)
		{
			return VectorDir.forward;
		}
		return VectorDir.up;
	}

	public static Vector2 ConvertAxis(Vector2 v)
	{
		float x = v.x;
		float y = v.y;
		if (y == 1f)
		{
			if (x == 0f)
			{
				return new Vector2(-1f, 1f);
			}
			if (x == 1f)
			{
				return new Vector2(0f, 1f);
			}
			if (x == -1f)
			{
				return new Vector2(-1f, 0f);
			}
		}
		if (y == 0f)
		{
			if (x == 0f)
			{
				return Vector2.zero;
			}
			if (x == 1f)
			{
				return new Vector2(1f, 1f);
			}
			if (x == -1f)
			{
				return new Vector2(-1f, -1f);
			}
		}
		if (y == -1f)
		{
			if (x == 0f)
			{
				return new Vector2(1f, -1f);
			}
			if (x == 1f)
			{
				return new Vector2(1f, 0f);
			}
			if (x == -1f)
			{
				return new Vector2(0f, -1f);
			}
		}
		return Vector3.zero;
	}

	public static Vector2 WorldToUIPos(Vector3 worldPoint, RectTransform container)
	{
		Vector2 localPoint = Vector2.zero;
		Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, worldPoint);
		RectTransformUtility.ScreenPointToLocalPointInRectangle(container, screenPoint, canvas.worldCamera, out localPoint);
		return localPoint;
	}

	public static T RandomEnum<T>()
	{
		Array values = Enum.GetValues(typeof(T));
		return (T)values.GetValue(new System.Random().Next(values.Length));
	}

	public static List<T> EnumToList<T>() where T : Enum
	{
		return Enum.GetValues(typeof(T)).Cast<T>().ToList();
	}

	public static void ShowExplorer(string itemPath, bool selectFirstFile = false)
	{
		if (selectFirstFile)
		{
			FileInfo[] files = new DirectoryInfo(itemPath).GetFiles();
			if (files.Length != 0)
			{
				itemPath = itemPath + "/" + files[0].Name;
			}
		}
		itemPath = itemPath.Replace("/", "\\");
		Process.Start("explorer.exe", "/select," + itemPath);
	}

	public static void Run(string itemPath)
	{
		Process.Start(itemPath);
	}

	public static T[,] ResizeArray<T>(T[,] original, int x, int y, Func<int, int, T> func)
	{
		T[,] array = new T[x, y];
		int num = Math.Min(x, original.GetLength(0));
		int num2 = Math.Min(y, original.GetLength(1));
		for (int i = 0; i < x; i++)
		{
			for (int j = 0; j < y; j++)
			{
				if (i >= num || j >= num2)
				{
					array[i, j] = func(i, j);
				}
				else
				{
					array[i, j] = original[i, j];
				}
			}
		}
		return array;
	}
}
