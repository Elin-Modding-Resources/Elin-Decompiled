using System;
using System.Reflection;

[AttributeUsage(AttributeTargets.Method)]
public sealed class ElinPostSceneInitAttribute : ElinEventBaseAttribute
{
	public override void Register(MethodInfo method)
	{
		Action<Scene.Mode> handler = method.CreateDelegate<Action<Scene.Mode>>();
		BaseModManager.SubscribeEvent("elin.scene.post_init", handler);
	}
}
