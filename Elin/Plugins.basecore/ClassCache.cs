using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

public class ClassCache<T>
{
	public Dictionary<string, Func<T>> dict = new Dictionary<string, Func<T>>();

	public T Create<T2>(Type type)
	{
		if (type == null)
		{
			return default(T);
		}
		Func<T> func = dict.TryGetValue(type.Name);
		if (func != null)
		{
			return func();
		}
		ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
		if (constructor == null)
		{
			return default(T);
		}
		func = Expression.Lambda<Func<T>>(Expression.New(constructor), Array.Empty<ParameterExpression>()).Compile();
		dict.Add(type.Name, func);
		return func();
	}

	public T Create<T2>(string id, string assembly)
	{
		Type type = Type.GetType(id + ", " + assembly);
		if (type == null)
		{
			foreach (string assembly2 in ClassCache.assemblies)
			{
				type = Type.GetType(id + ", " + assembly2);
				if (type != null)
				{
					break;
				}
			}
		}
		return Create<T2>(type);
	}
}
public class ClassCache
{
	public static ClassCache<object> caches = new ClassCache<object>();

	public static HashSet<string> assemblies = new HashSet<string>();

	public static T Create<T>(string id, string assembly = "Assembly-CSharp")
	{
		return (T)caches.Create<T>(id, assembly);
	}
}
