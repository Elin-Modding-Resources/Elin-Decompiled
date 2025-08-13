using System.Collections.Generic;
using Newtonsoft.Json;

public class MixedFoodData : EClass
{
	[JsonProperty]
	public List<string> texts = new List<string>();
}
