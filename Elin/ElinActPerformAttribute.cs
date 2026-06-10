using System;
using System.Reflection;

[AttributeUsage(AttributeTargets.Method)]
public sealed class ElinActPerformAttribute : ElinEventBaseAttribute
{
	public override void Register(MethodInfo method)
	{
		Action<Act> handler = method.CreateDelegate<Action<Act>>();
		BaseModManager.SubscribeEvent("elin.act_performed", handler);
	}
}
