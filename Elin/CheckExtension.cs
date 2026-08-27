public static class CheckExtension
{
	public static bool IsFail(this Check.Result r)
	{
		if (r != Check.Result.CriticalFail)
		{
			return r == Check.Result.Fail;
		}
		return true;
	}

	public static bool IsPass(this Check.Result r)
	{
		if (r != Check.Result.CriticalPass)
		{
			return r == Check.Result.Pass;
		}
		return true;
	}
}
