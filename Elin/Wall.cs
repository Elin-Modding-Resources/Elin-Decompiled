public struct Wall
{
	public static Map map;

	public byte id;

	public byte idMat;

	public SourceBlock.Row source => Cell.blockSource[id];

	public SourceMaterial.Row mat => Cell.matSource[idMat];
}
