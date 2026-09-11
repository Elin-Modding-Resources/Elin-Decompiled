public struct Wall
{
	public static Map map;

	public int id;

	public int idMat;

	public SourceBlock.Row source => Cell.blockSource[id];

	public SourceMaterial.Row mat => Cell.matSource[idMat];
}
