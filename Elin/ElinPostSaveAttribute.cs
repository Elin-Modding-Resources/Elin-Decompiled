using System;
using System.Reflection;

public sealed class ElinPostSaveAttribute : ElinGameIOEventAttribute
{
	public override void Register(MethodInfo method)
	{
		Action<GameIOContext> handler = method.CreateDelegate<Action<GameIOContext>>();
		BaseModManager.SubscribeEvent("elin.game.post_save", handler);
	}
}
