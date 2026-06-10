using System;
using System.Reflection;

public abstract class ElinEventBaseAttribute : Attribute
{
	public virtual void Register(MethodInfo method)
	{
	}

	public virtual void Register(PropertyInfo property)
	{
	}
}
