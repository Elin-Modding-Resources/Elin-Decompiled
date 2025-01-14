using System;
using Newtonsoft.Json.Serialization;
using UnityEngine;

public class GameSerializationBinder : DefaultSerializationBinder, ISerializationBinder
{
	internal static readonly GameSerializationBinder Instance = new GameSerializationBinder();

	public override Type BindToType(string assemblyName, string typeName)
	{
		try
		{
			return base.BindToType(assemblyName, typeName);
		}
		catch (Exception ex)
		{
			Debug.LogError(ex.Message);
			Debug.LogError(assemblyName + "/" + typeName);
			if (ModUtil.fallbackTypes.ContainsKey(typeName))
			{
				Debug.Log(typeName + "/" + Type.GetType(ModUtil.fallbackTypes[typeName]));
				return Type.GetType(ModUtil.fallbackTypes[typeName]);
			}
			if (typeName.Contains("Quest"))
			{
				return typeof(QuestDummy);
			}
			return typeof(object);
		}
	}
}
