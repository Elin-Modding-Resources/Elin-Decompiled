using System;
using System.Reflection;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class ElinDramaActionParserAttribute : ElinEventBaseAttribute
{
	public string DramaAction { get; }

	public ElinDramaActionParserAttribute(string action)
	{
		if (action.IsEmpty())
		{
			throw new ArgumentNullException("action");
		}
		DramaAction = action;
	}

	public override void Register(MethodInfo method)
	{
		DramaActionParser parser = method.CreateDelegate<DramaActionParser>();
		CustomDramaExpansion.AddDramaActionParser(DramaAction, parser);
	}
}
