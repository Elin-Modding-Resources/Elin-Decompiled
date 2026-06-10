using System;
using System.Reflection;

public sealed class ElinPreLoadAttribute : ElinGameIOEventAttribute
{
	public override void Register(MethodInfo method)
	{
		Action<GameIOContext> handler = method.CreateDelegate<Action<GameIOContext>>();
		BaseModManager.SubscribeEvent("elin.game.pre_load", handler);
	}
}
