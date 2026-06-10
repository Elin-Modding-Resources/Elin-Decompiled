using System;
using System.Reflection;

[AttributeUsage(AttributeTargets.Method)]
public sealed class ElinCharaOnCreateAttribute : ElinEventBaseAttribute
{
	public override void Register(MethodInfo method)
	{
		Action<Chara> handler = method.CreateDelegate<Action<Chara>>();
		BaseModManager.SubscribeEvent("elin.chara_created", handler);
	}
}
