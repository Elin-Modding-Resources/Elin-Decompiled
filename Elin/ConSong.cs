using Newtonsoft.Json;

public class ConSong : BaseBuff
{
	[JsonProperty]
	public int p2;

	public override int P2
	{
		get
		{
			return p2;
		}
		set
		{
			p2 = value;
		}
	}
}
