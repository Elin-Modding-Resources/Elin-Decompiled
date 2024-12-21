using System;

[Serializable]
public struct Version
{
	public int major;

	public int minor;

	public int batch;

	public int fix;

	public bool demo;

	public string GetText()
	{
		return ((minor >= 23) ? "EA" : "Beta") + " " + minor + "." + batch + ((fix == 0) ? "" : (" fix " + fix)) + (demo ? "demo".lang() : "");
	}

	public int GetInt()
	{
		return major * 1000000 + minor * 1000 + batch;
	}

	public int GetInt(int _major, int _minor, int _batch)
	{
		return _major * 1000000 + _minor * 1000 + _batch;
	}

	public bool IsBelow(int _major, int _minor, int _batch)
	{
		return GetInt() < GetInt(_major, _minor, _batch);
	}

	public bool IsBelow(Version v)
	{
		return IsBelow(v.GetInt());
	}

	public bool IsBelow(int _int)
	{
		return GetInt() < _int;
	}

	public static Version Get(string str)
	{
		if (str.IsEmpty())
		{
			return default(Version);
		}
		string[] array = str.Split('.');
		if (array.Length < 3)
		{
			return default(Version);
		}
		Version result = default(Version);
		result.major = array[0].ToInt();
		result.minor = array[1].ToInt();
		result.batch = array[2].ToInt();
		return result;
	}

	public static Version Get(int i)
	{
		Version result = default(Version);
		result.major = i / 1000000;
		result.minor = i / 1000 % 1000;
		result.batch = i % 1000;
		return result;
	}

	public bool IsSaveCompatible(Version v)
	{
		if (IsBelow(v))
		{
			return false;
		}
		if (major == v.major)
		{
			return minor >= 21;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		Version version = (Version)obj;
		if (version.major == major && version.minor == minor)
		{
			return version.batch == batch;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return GetInt();
	}
}
