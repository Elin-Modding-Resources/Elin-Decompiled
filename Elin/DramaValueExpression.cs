using System.Text.RegularExpressions;
using UnityEngine;

public class DramaValueExpression
{
	private static readonly Regex _listSyntaxRegex = new Regex("^(?<op>>=|<=|==|!=|--|\\+\\+|[><=+\\-*/x])?(?<number>.*)$", RegexOptions.Compiled);

	public string Expression { get; }

	public string Op { get; }

	public string Rhs { get; }

	public DramaValueExpression(string expression)
	{
		Expression = expression.Trim();
		Match match = _listSyntaxRegex.Match(Expression);
		Op = match.Groups["op"].Value;
		Rhs = match.Groups["number"].Value;
	}

	public static bool TryParse(string expression, out DramaValueExpression valueExpr)
	{
		valueExpr = new DramaValueExpression(expression);
		return true;
	}

	public static implicit operator DramaValueExpression(string expr)
	{
		return new DramaValueExpression(expr);
	}

	public static implicit operator string(DramaValueExpression expr)
	{
		return expr.Expression;
	}

	public int Calc(int lhs)
	{
		if (Expression.IsEmpty())
		{
			return lhs;
		}
		if (!$"{lhs} {Expression}".TryEvaluateAsCalc(out int result, (object)null))
		{
			return lhs;
		}
		return result;
	}

	public float Calc(float lhs)
	{
		if (Expression.IsEmpty())
		{
			return lhs;
		}
		if (!$"{lhs} {Expression}".TryEvaluateAsCalc(out float result, (object)null))
		{
			return lhs;
		}
		return result;
	}

	public float Diff(float lhs)
	{
		return ModOrSet(lhs) - lhs;
	}

	public int Diff(int lhs)
	{
		return ModOrSet(lhs) - lhs;
	}

	public float ModOrSet(float lhs)
	{
		if (Expression.IsEmpty())
		{
			return lhs;
		}
		string op = Op;
		if (!(op == "++"))
		{
			if (op == "--")
			{
				return lhs - 1f;
			}
			if (!float.TryParse(Rhs, out var result))
			{
				return lhs;
			}
			switch (Op)
			{
			case "+":
				return lhs + result;
			case "-":
				return lhs - result;
			case "*":
			case "x":
				return lhs * result;
			case "/":
				return lhs / result;
			case "=":
			case "==":
				return result;
			case "":
				return result;
			default:
				return lhs;
			}
		}
		return lhs + 1f;
	}

	public int ModOrSet(int lhs)
	{
		if (Expression.IsEmpty())
		{
			return lhs;
		}
		string op = Op;
		if (!(op == "++"))
		{
			if (op == "--")
			{
				return lhs - 1;
			}
			if (!int.TryParse(Rhs, out var result))
			{
				return lhs;
			}
			switch (Op)
			{
			case "+":
				return lhs + result;
			case "-":
				return lhs - result;
			case "*":
			case "x":
				return lhs * result;
			case "/":
				return lhs / result;
			case "=":
			case "==":
				return result;
			case "":
				return result;
			default:
				return lhs;
			}
		}
		return lhs + 1;
	}

	public bool Compare(float lhs)
	{
		if (Expression.IsEmpty())
		{
			return false;
		}
		if (Op == "")
		{
			if (float.TryParse(Rhs, out var result))
			{
				return Mathf.Approximately(lhs, result);
			}
			return false;
		}
		if (!float.TryParse(Rhs, out var result2))
		{
			return false;
		}
		switch (Op)
		{
		case ">":
			return lhs > result2;
		case "<":
			return lhs < result2;
		case "=":
		case "==":
			return Mathf.Approximately(lhs, result2);
		case ">=":
			return lhs >= result2;
		case "<=":
			return lhs <= result2;
		case "!=":
			return !Mathf.Approximately(lhs, result2);
		default:
			return false;
		}
	}

	public bool Compare(int lhs)
	{
		if (Expression.IsEmpty())
		{
			return false;
		}
		if (Op == "")
		{
			if (int.TryParse(Rhs, out var result))
			{
				return lhs == result;
			}
			return false;
		}
		if (!int.TryParse(Rhs, out var result2))
		{
			return false;
		}
		switch (Op)
		{
		case ">":
			return lhs > result2;
		case "<":
			return lhs < result2;
		case "=":
		case "==":
			return lhs == result2;
		case ">=":
			return lhs >= result2;
		case "<=":
			return lhs <= result2;
		case "!=":
			return lhs != result2;
		default:
			return false;
		}
	}
}
