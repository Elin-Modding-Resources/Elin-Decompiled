using System;
using System.Collections.Generic;

public class ContextMenuProxy
{
	public readonly List<ContextMenuProxy> children = new List<ContextMenuProxy>();

	public Action onClick;

	public bool isMenu;

	public string DisplayName { get; }

	public string MenuEntry { get; }

	public ContextMenuProxy(string displayName, string menuEntry)
	{
		DisplayName = displayName;
		MenuEntry = menuEntry;
	}
}
