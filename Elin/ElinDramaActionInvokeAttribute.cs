using System;
using System.Reflection;

[AttributeUsage(AttributeTargets.Method)]
public sealed class ElinDramaActionInvokeAttribute : ElinEventBaseAttribute
{
	public string Contract { get; }

	public ElinDramaActionInvokeAttribute(string contract = null)
	{
		Contract = contract;
	}

	public override void Register(MethodInfo method)
	{
		CustomDramaExpansion.AddDramaInvokeMethod(method.Name, method, Contract);
	}
}
