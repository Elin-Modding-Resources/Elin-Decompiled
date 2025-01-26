using System;
using Newtonsoft.Json;

public class MapMetaData : EClass
{
	[JsonProperty]
	public string name;

	[JsonProperty]
	public string id;

	[JsonProperty]
	public string type;

	[JsonProperty]
	public int version;

	[JsonProperty]
	public PartialMap partial;

	public string path;

	public DateTime date;

	public bool IsValidVersion()
	{
		return !Version.Get(version).IsBelow(EClass.core.versionMoongate);
	}
}
