using Newtonsoft.Json.Serialization;

public class GameContractResolver : DefaultContractResolver
{
	public static readonly GameContractResolver Instance = new GameContractResolver();
}
