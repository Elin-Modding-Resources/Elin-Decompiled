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

	public bool IsWed => dateWedding != 0;
}
