public class RendererReplacer : EClass
{
	public int tile = 1820;

	public int mat = -1;

	public RenderData data;

	public SourcePref pref;

	public PCCData pccData;

	public static RendererReplacer CreateFrom(string id, int shift = 0, int mat = -1)
	{
		CardRow cardRow = EClass.sources.cards.map.TryGetValue(id);
		if (cardRow == null || cardRow._tiles.Length == 0)
		{
			cardRow = EClass.sources.cards.map["money2"];
		}
		return new RendererReplacer
		{
			tile = cardRow._tiles[0] + shift,
			data = cardRow.renderData,
			pref = cardRow.pref,
			mat = mat
		};
	}

	public static RendererReplacer CreateFromPCC(string id, PCCData pccData)
	{
		CardRow cardRow = EClass.sources.cards.map.TryGetValue(id);
		if (cardRow == null)
		{
			cardRow = EClass.sources.cards.map["money2"];
		}
		return new RendererReplacer
		{
			data = cardRow.renderData,
			pref = cardRow.pref,
			pccData = pccData
		};
	}
}
