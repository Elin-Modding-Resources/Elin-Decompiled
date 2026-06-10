using System;
using System.Reflection;

public sealed class ElinPostLoadAttribute : ElinGameIOEventAttribute
{
	public override void Register(MethodInfo method)
	{
		Action<GameIOContext> handler = method.CreateDelegate<Action<GameIOContext>>();
		BaseModManager.SubscribeEvent("elin.game.start_new", handler);
		BaseModManager.SubscribeEvent("elin.game.post_load", handler);
	}
}
