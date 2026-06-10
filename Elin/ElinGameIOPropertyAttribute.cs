using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;

[AttributeUsage(AttributeTargets.Property)]
public class ElinGameIOPropertyAttribute : ElinGameIOEventAttribute
{
	private static bool _registered;

	private static readonly Dictionary<string, (PropertyInfo property, Func<object> getter, Action<object> setter)> _contextVars = new Dictionary<string, (PropertyInfo, Func<object>, Action<object>)>();

	public string ChunkName { get; }

	public ElinGameIOPropertyAttribute(string chunkName)
	{
		ChunkName = chunkName;
	}

	public override void Register(PropertyInfo property)
	{
		if (!_registered)
		{
			BaseModManager.SubscribeEvent<GameIOContext>("elin.game.post_load", LoadGameIOProperty);
			BaseModManager.SubscribeEvent<GameIOContext>("elin.game.post_save", SaveGameIOProperty);
			_registered = true;
		}
		MethodInfo getMethod = property.GetMethod;
		MethodInfo setMethod = property.SetMethod;
		if (!(getMethod == null) && !(setMethod == null) && setMethod.IsStatic)
		{
			Func<object> item = getMethod.CreateDelegate<Func<object>>();
			Action<object> item2 = CreateSetterDelegate(setMethod);
			Type declaringType = property.DeclaringType;
			_contextVars[declaringType.FullName + ":" + ChunkName] = (property, item, item2);
		}
	}

	private static Action<object> CreateSetterDelegate(MethodInfo method)
	{
		Type parameterType = method.GetParameters()[0].ParameterType;
		ParameterExpression parameterExpression = Expression.Parameter(typeof(object), "obj");
		UnaryExpression arg = Expression.Convert(parameterExpression, parameterType);
		return Expression.Lambda<Action<object>>(Expression.Call(method, arg), new ParameterExpression[1] { parameterExpression }).Compile();
	}

	private static void LoadGameIOProperty(GameIOContext context)
	{
		if (!context.Load<Dictionary<string, object>>("context_vars", out var data))
		{
			data = new Dictionary<string, object>();
		}
		KeyValuePair<string, (PropertyInfo, Func<object>, Action<object>)>[] array = _contextVars.ToArray();
		foreach (KeyValuePair<string, (PropertyInfo, Func<object>, Action<object>)> keyValuePair in array)
		{
			keyValuePair.Deconstruct(out var key, out var value);
			(PropertyInfo, Func<object>, Action<object>) tuple = value;
			string text = key;
			var (propertyInfo, _, action) = tuple;
			try
			{
				string key2 = propertyInfo.DeclaringType.FullName + ":" + text;
				object valueOrDefault = data.GetValueOrDefault(key2);
				action(valueOrDefault);
			}
			catch (Exception arg)
			{
				Debug.LogError($"#io failed to populate context var {text}\n{arg}");
				_contextVars.Remove(text);
			}
		}
	}

	private static void SaveGameIOProperty(GameIOContext context)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>(StringComparer.Ordinal);
		KeyValuePair<string, (PropertyInfo, Func<object>, Action<object>)>[] array = _contextVars.ToArray();
		foreach (KeyValuePair<string, (PropertyInfo, Func<object>, Action<object>)> keyValuePair in array)
		{
			keyValuePair.Deconstruct(out var key, out var value);
			(PropertyInfo, Func<object>, Action<object>) tuple = value;
			string text = key;
			var (propertyInfo, func, _) = tuple;
			try
			{
				object value2 = func();
				string key2 = propertyInfo.DeclaringType.FullName + ":" + text;
				dictionary[key2] = value2;
			}
			catch (Exception arg)
			{
				Debug.LogError($"#io failed to save context var {text}\n{arg}");
				_contextVars.Remove(text);
			}
		}
		context.Save("context_vars", dictionary);
	}
}
