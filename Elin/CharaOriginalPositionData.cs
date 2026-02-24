using Newtonsoft.Json;

public class CharaOriginalPositionData : EClass
{
	[JsonProperty]
	public int uid;

	[JsonProperty]
	public int uidZone;

	[JsonProperty]
	public int x;

	[JsonProperty]
	public int z;

	[JsonProperty]
	public bool noMove;
}
