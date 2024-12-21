using UnityEngine;

public static class ConsoleProDebug
{
	public static void Clear()
	{
	}

	public static void LogToFilter(string inLog, string inFilterName)
	{
		Debug.Log(inLog + "\nCPAPI:{\"cmd\":\"Filter\" \"name\":\"" + inFilterName + "\"}");
	}

	public static void LogAsType(string inLog, string inTypeName)
	{
		Debug.Log(inLog + "\nCPAPI:{\"cmd\":\"LogType\" \"name\":\"" + inTypeName + "\"}");
	}

	public static void Watch(string inName, string inValue)
	{
		Debug.Log(inName + " : " + inValue + "\nCPAPI:{\"cmd\":\"Watch\" \"name\":\"" + inName + "\"}");
	}

	public static void Search(string inText)
	{
		Debug.Log("\nCPAPI:{\"cmd\":\"Search\" \"text\":\"" + inText + "\"}");
	}
}
