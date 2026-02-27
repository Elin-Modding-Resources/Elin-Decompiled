using Newtonsoft.Json;

public class LoveData : EClass
{
	[JsonProperty]
	public int dateMarriage;

	[JsonProperty]
	public int dateWedding;

	[JsonProperty]
	public int uidZoneMarriage;

	[JsonProperty]
	public string nameZoneMarriage;

	[JsonProperty]
	public bool gaveCutter;

	public bool IsWed => dateWedding != 0;
}
