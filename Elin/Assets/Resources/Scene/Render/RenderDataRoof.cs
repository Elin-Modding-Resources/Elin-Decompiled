using UnityEngine;

namespace Assets.Resources.Scene.Render;

public class RenderDataRoof : RenderDataTile
{
	public Vector3[] offsets;

	public Vector3[] offsetFixes;

	public Vector3[] offsetFixes2;

	public override void Draw(RenderParam p)
	{
		MeshPass meshPass = ((hasSubPass && SubPassData.Current.enable) ? pass.subPass : pass);
		MeshBatch meshBatch = meshPass.batches[meshPass.batchIdx];
		int num = 1;
		_offset = offsets[(int)p.tile % offsets.Length] + offsetFixes[(int)p.tile % 2];
		if (p.shadowFix != 0f)
		{
			_offset += offsetFixes2[(int)p.shadowFix - 1];
		}
		if (meshPass == pass.subPass)
		{
			meshBatch.colors[meshPass.idx] = p.color - 1572864f;
			meshBatch.matrices[meshPass.idx].SetTRS(p.NewVector3 + _offset + SubPassData.Current.offset, SubPassData.Current.rotation, SubPassData.Current.scale);
		}
		else
		{
			meshBatch.colors[meshPass.idx] = p.color;
			meshBatch.matrices[meshPass.idx].m03 = p.x + _offset.x * (float)num;
			meshBatch.matrices[meshPass.idx].m13 = p.y + _offset.y;
			meshBatch.matrices[meshPass.idx].m23 = p.z + _offset.z;
		}
		meshBatch.tiles[meshPass.idx] = p.tile + (float)(p.liquidLv * 10000 * num);
		meshBatch.matColors[meshPass.idx] = p.matColor;
		meshPass.idx++;
		if (meshPass.idx == meshPass.batchSize)
		{
			meshPass.NextBatch();
		}
		if (p.snow && hasSnowPass && meshPass == pass)
		{
			meshPass = pass.snowPass;
			meshBatch = meshPass.batches[meshPass.batchIdx];
			meshBatch.colors[meshPass.idx] = p.color;
			meshBatch.matrices[meshPass.idx].m03 = p.x + _offset.x * (float)num;
			meshBatch.matrices[meshPass.idx].m13 = p.y + _offset.y;
			meshBatch.matrices[meshPass.idx].m23 = p.z + _offset.z - 0.01f;
			meshBatch.tiles[meshPass.idx] = p.tile + (float)(p.liquidLv * 10000 * num);
			meshBatch.matColors[meshPass.idx] = 104025f;
			meshPass.idx++;
			if (meshPass.idx == meshPass.batchSize)
			{
				meshPass.NextBatch();
			}
		}
	}
}
