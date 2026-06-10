using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;

public class DramaInvokeDetail
{
	private Func<DramaManager, Dictionary<string, string>, string[], bool> _compiled;

	private static readonly MethodInfo _stringIsNullOrEmpty = typeof(string).GetMethod("IsNullOrEmpty", new Type[1] { typeof(string) });

	private static readonly ConstructorInfo _argumentException = typeof(ArgumentException).GetConstructor(new Type[1] { typeof(string) });

	public MethodInfo Method { get; }

	public string Contract { get; }

	public bool NoDiscard => Contract.Contains("nodiscard");

	public bool ParamPassthrough => Contract.Contains("passthrough");

	public DramaInvokeDetail(MethodInfo method, string contract = null)
	{
		Method = method;
		Contract = contract;
		ValidateSignature();
	}

	private void ValidateSignature()
	{
		if (!(Method == null))
		{
			if (Method.ContainsGenericParameters)
			{
				throw new NotSupportedException("#drama action cannot contain generic parameters");
			}
			if (!Method.IsStatic)
			{
				throw new NotSupportedException("#drama action must be static");
			}
			ParameterInfo[] parameters = Method.GetParameters();
			if (parameters.Length < 2 || parameters[0].ParameterType != typeof(DramaManager) || parameters[1].ParameterType != typeof(Dictionary<string, string>) || Method.ReturnType != typeof(bool))
			{
				throw new ArgumentException("#drama action parameters must start with 'DramaManager dm, Dictionary<string, string>' and returns 'bool'");
			}
		}
	}

	private Func<DramaManager, Dictionary<string, string>, string[], bool> BuildDelegate()
	{
		ParameterInfo[] parameters = Method.GetParameters();
		ParameterExpression parameterExpression = Expression.Parameter(typeof(DramaManager), "dm");
		ParameterExpression parameterExpression2 = Expression.Parameter(typeof(Dictionary<string, string>), "line");
		ParameterExpression parameterExpression3 = Expression.Parameter(typeof(string[]), "parameters");
		List<Expression> list = new List<Expression> { parameterExpression, parameterExpression2 };
		if (parameters.Length == 3 && parameters[2].ParameterType == typeof(string[]))
		{
			list.Add(parameterExpression3);
		}
		else if (parameters.Length > 2)
		{
			for (int i = 2; i < parameters.Length; i++)
			{
				ParameterInfo parameter = parameters[i];
				int num = i - 2;
				Expression expr = Expression.Condition(Expression.LessThan(Expression.Constant(num), Expression.ArrayLength(parameterExpression3)), Expression.ArrayIndex(parameterExpression3, Expression.Constant(num)), Expression.Constant(null, typeof(string)));
				list.Add(CreateConvertExpression(expr, parameter));
			}
		}
		return Expression.Lambda<Func<DramaManager, Dictionary<string, string>, string[], bool>>(Expression.Call(null, Method, list), new ParameterExpression[3] { parameterExpression, parameterExpression2, parameterExpression3 }).Compile();
	}

	private static Expression CreateConvertExpression(Expression expr, ParameterInfo parameter)
	{
		Type parameterType = parameter.ParameterType;
		bool isOptional = parameter.IsOptional;
		object defaultValue = parameter.DefaultValue;
		string text = $"required action parameter #{parameter.Position - 2} '{parameter.Name}'";
		if (parameterType == typeof(string))
		{
			MethodCallExpression test = Expression.Call(_stringIsNullOrEmpty, expr);
			if (isOptional)
			{
				ConstantExpression ifTrue = Expression.Constant(defaultValue, typeof(string));
				return Expression.Condition(test, ifTrue, expr);
			}
			string value = "#drama " + text + " cannot be empty";
			UnaryExpression ifTrue2 = Expression.Throw(Expression.New(_argumentException, Expression.Constant(value)), typeof(string));
			return Expression.Condition(test, ifTrue2, expr);
		}
		MethodInfo tryParseMethod = GetTryParseMethod(parameterType);
		if (tryParseMethod == null)
		{
			throw new NotSupportedException("#drama type '" + parameterType.Name + "' does not have a public static TryParse method");
		}
		ParameterExpression parameterExpression = Expression.Variable(parameterType, "parsed");
		Expression test2 = Expression.Call(tryParseMethod, expr, parameterExpression);
		Expression expression;
		if (isOptional)
		{
			expression = Expression.Constant(defaultValue, parameterType);
		}
		else
		{
			string value2 = "#drama can't parse " + text + " to type '" + parameterType.Name + "'";
			expression = Expression.Throw(Expression.New(_argumentException, Expression.Constant(value2)), parameterType);
		}
		ConditionalExpression conditionalExpression = Expression.Condition(Expression.Call(_stringIsNullOrEmpty, expr), expression, Expression.Condition(test2, parameterExpression, expression));
		return Expression.Block(new ParameterExpression[1] { parameterExpression }, conditionalExpression);
	}

	private static MethodInfo GetTryParseMethod(Type type)
	{
		if (type.IsEnum)
		{
			MethodInfo method = typeof(Enum).GetMethod("TryParse", BindingFlags.Static | BindingFlags.Public, null, new Type[2]
			{
				typeof(string),
				type.MakeByRefType()
			}, null);
			if (method != null)
			{
				return method.MakeGenericMethod(type);
			}
		}
		return type.GetMethod("TryParse", new Type[2]
		{
			typeof(string),
			type.MakeByRefType()
		});
	}

	public bool SafeInvoke(DramaManager dm, Dictionary<string, string> line, params string[] parameters)
	{
		if (Method == null)
		{
			return true;
		}
		try
		{
			if (_compiled == null)
			{
				_compiled = BuildDelegate();
			}
			return _compiled(dm, line, parameters);
		}
		catch (Exception ex)
		{
			ModUtil.LogModError("exception while invoking drama action '" + Method.TryToString() + "'\n" + ex.Message, new FileInfo(dm.path));
			Debug.LogException(ex);
			return false;
		}
	}
}
