using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

public class ClassCache<T>
{
	public Dictionary<string, Func<T>> dict = new Dictionary<string, Func<T>>();

	public Func<T> CreateDelegate(Type type)
	{
		Func<T> result = () => default(T);
		if (type == null)
		{
			return result;
		}
		ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
		if (constructor != null)
		{
			result = Expression.Lambda<Func<T>>(Expression.New(constructor), Array.Empty<ParameterExpression>()).Compile();
		}
		return result;
	}

	public T Create<T2>(string id, string assembly)
	{
		id = id.Trim();
		Func<T> func = dict.TryGetValue(id);
		if (func != null)
		{
			return func();
		}
		Type type = Type.GetType(id + ", " + assembly);
		if (type == null)
		{
			for (int num = ClassCache.typeLoaders.Count - 1; num >= 0; num--)
			{
				type = ClassCache.typeLoaders[num](id);
				if (type != null)
				{
					break;
				}
			}
		}
		func = CreateDelegate(type);
		dict[id] = func;
		return func();
	}
}
public class ClassCache
{
	public static ClassCache<object> caches = new ClassCache<object>();

	public static HashSet<string> assemblies = new HashSet<string>();

	public static List<Func<string, Type>> typeLoaders = new List<Func<string, Type>> { LoadTypeFromGlobalNamespace };

	public static HashSet<Type> modTypes = new HashSet<Type>();

	public static T Create<T>(string id, string assembly = "Assembly-CSharp")
	{
		return (T)caches.Create<T>(id, assembly);
	}

	public static Type LoadTypeFromGlobalNamespace(string id)
	{
		foreach (string assembly in assemblies)
		{
			Type type = Type.GetType(id + ", " + assembly);
			if (type != null)
			{
				return type;
			}
		}
		return null;
	}
}
