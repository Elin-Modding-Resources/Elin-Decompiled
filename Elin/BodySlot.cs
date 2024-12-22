using Newtonsoft.Json;

public class BodySlot : EClass
{
	[JsonProperty(PropertyName = "E")]
	public int elementId;

	public Thing thing;

	public int index;

	public int indexPart;

	public string name => EClass.sources.elements.map[elementId].GetText();

	public SourceElement.Row element => EClass.sources.elements.map[elementId];

	public bool IsEquipping(Thing t)
	{
		return t.c_equippedSlot == index + 1;
	}
}
