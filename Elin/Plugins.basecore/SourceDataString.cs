public class SourceDataString<T> : SourceData<T, string> where T : SourceData.BaseRow
{
	public override T GetRow(string id)
	{
		return map.TryGetValue(id);
	}
}
