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
			Core.Instance.ui.Say("Possible Mod Error, please contact the mod author.");
			Core.Instance.ui.Say(ex.Message);
			Debug.LogError(ex.Message);
			Debug.LogError(assemblyName + "/" + typeName);
			if (ModUtil.fallbackTypes.TryGetValue(typeName, out var value))
			{
				Debug.Log(typeName + "/" + Type.GetType(value));
				return Type.GetType(value);
			}
			if (typeName.StartsWith("Quest"))
			{
				return typeof(QuestDummy);
			}
			if (typeName.StartsWith("Zone"))
			{
				return typeof(Zone);
			}
			if (typeName.StartsWith("Religion"))
			{
				return typeof(ReligionCustom);
			}
			return typeof(UnknownTypePlaceholder);
		}
	}
}
