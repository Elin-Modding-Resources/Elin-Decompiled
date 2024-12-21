public class SourceDataInt<T> : SourceData<T, int> where T : SourceData.BaseRow
{
	public override T GetRow(string id)
	{
		return map.TryGetValue(id.ToInt());
	}
}
