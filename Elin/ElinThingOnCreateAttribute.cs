using System;
using System.Reflection;

[AttributeUsage(AttributeTargets.Method)]
public sealed class ElinThingOnCreateAttribute : ElinEventBaseAttribute
{
	public override void Register(MethodInfo method)
	{
		Action<Thing> handler = method.CreateDelegate<Action<Thing>>();
		BaseModManager.SubscribeEvent("elin.thing_created", handler);
	}
}
