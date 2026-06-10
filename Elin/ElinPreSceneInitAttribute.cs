using System;
using System.Reflection;

[AttributeUsage(AttributeTargets.Method)]
public sealed class ElinPreSceneInitAttribute : ElinEventBaseAttribute
{
	public override void Register(MethodInfo method)
	{
		Action<Scene.Mode> handler = method.CreateDelegate<Action<Scene.Mode>>();
		BaseModManager.SubscribeEvent("elin.scene.pre_init", handler);
	}
}
