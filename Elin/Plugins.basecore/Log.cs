using UnityEngine;

public static class Log
{
	public static string system;

	public static void App(string s)
	{
		if (!Application.isEditor)
		{
			Debug.Log(s);
		}
	}
}
