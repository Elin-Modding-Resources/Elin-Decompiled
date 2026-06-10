using System;
using System.Reflection;

[AttributeUsage(AttributeTargets.Method)]
public sealed class ElinContextMenuEntryAttribute : ElinEventBaseAttribute
{
	public string DisplayName { get; }

	public string MenuEntry { get; }

	public ElinContextMenuEntryAttribute(string langEntry, string langDisplay = "")
	{
		MenuEntry = langEntry.lang();
		DisplayName = langDisplay.IsEmpty(MenuEntry.Split('/')[^1]).lang();
	}

	public override void Register(MethodInfo method)
	{
		ModUtil.AddContextMenuEntry(method.CreateDelegate<Action>(), MenuEntry, DisplayName);
	}
}
