using System;

[Serializable]
public class GameDifficultySetting : EClass
{
	public int tier;

	public bool allowManualSave;

	public bool allowRevive;

	public bool deleteGameOnDeath;

	public int ID => EClass.setting.start.difficulties.IndexOf(this);

	public string Name => Lang.GetList("difficulties")[ID];
}
