using UnityEngine;

public class RenderDataObjV : RenderDataObj
{
	public bool topOnly;

	public Vector3 topFix;

	public override bool ForceAltHeldPosition => true;

	public override void Draw(RenderParam p)
	{
		bool flag = false;
		MeshPass meshPass = ((hasSubPass && SubPassData.Current.enable) ? pass.subPass : pass);
		MeshBatch meshBatch = meshPass.batches[meshPass.batchIdx];
		int num = ((p.tile > 0f) ? 1 : (-1));
		if (meshPass == pass.subPass)
		{
			meshBatch.colors[meshPass.idx] = p.color - 1572864f;
			meshBatch.matrices[meshPass.idx].SetTRS(p.NewVector3 + offset + SubPassData.Current.offset, SubPassData.Current.rotation, SubPassData.Current.scale);
		}
		else
		{
			meshBatch.colors[meshPass.idx] = p.color;
			meshBatch.matrices[meshPass.idx].m03 = p.x + offset.x * (float)num;
			meshBatch.matrices[meshPass.idx].m13 = p.y + offset.y;
			meshBatch.matrices[meshPass.idx].m23 = p.z + offset.z;
		}
		meshBatch.tiles[meshPass.idx] = p.tile + ((multiSize && flag) ? meshPass.pmesh.tiling.x : 0f) + (float)(p.liquidLv * 10000 * num);
		meshBatch.matColors[meshPass.idx] = p.matColor;
		meshPass.idx++;
		if (meshPass.idx == meshPass.batchSize)
		{
			meshPass.NextBatch();
			meshBatch = meshPass.batches[meshPass.batchIdx];
		}
		if (multiSize && !flag)
		{
			meshBatch.tiles[meshPass.idx] = p.tile - meshPass.pmesh.tiling.x * (float)num;
			meshBatch.matColors[meshPass.idx] = p.matColor;
			if (meshPass == pass.subPass)
			{
				meshBatch.colors[meshPass.idx] = p.color - 1572864f;
				meshBatch.matrices[meshPass.idx].SetTRS(p.NewVector3 + offset + new Vector3(0f, meshPass.pmesh.size.y * SubPassData.Current.scale.y, 0f) + SubPassData.Current.offset, SubPassData.Current.rotation, SubPassData.Current.scale);
			}
			else
			{
				meshBatch.colors[meshPass.idx] = p.color;
				meshBatch.matrices[meshPass.idx].m03 = p.x + (topFix.x + offset.x) * (float)num;
				meshBatch.matrices[meshPass.idx].m13 = p.y + topFix.y + offset.y + meshPass.pmesh.size.y;
				meshBatch.matrices[meshPass.idx].m23 = p.z + topFix.z + offset.z + RenderData.renderSetting.vFix.z;
			}
			meshPass.idx++;
			if (meshPass.idx == meshPass.batchSize)
			{
				meshPass.NextBatch();
			}
		}
		if (!hasSnowPass || !p.snow || !(meshPass == pass))
		{
			return;
		}
		meshPass = pass.snowPass;
		meshBatch = meshPass.batches[meshPass.batchIdx];
		meshBatch.colors[meshPass.idx] = p.color;
		meshBatch.matrices[meshPass.idx].m03 = p.x + offset.x * (float)num;
		meshBatch.matrices[meshPass.idx].m13 = p.y + offset.y;
		meshBatch.matrices[meshPass.idx].m23 = p.z + offset.z + snowZ;
		meshBatch.tiles[meshPass.idx] = p.tile + ((multiSize && flag) ? meshPass.pmesh.tiling.x : 0f) + (float)(p.liquidLv * 10000 * num);
		meshBatch.matColors[meshPass.idx] = 104025f;
		meshPass.idx++;
		if (meshPass.idx == meshPass.batchSize)
		{
			meshPass.NextBatch();
			meshBatch = meshPass.batches[meshPass.batchIdx];
		}
		if (multiSize && !flag)
		{
			meshBatch.tiles[meshPass.idx] = p.tile - meshPass.pmesh.tiling.x * (float)num;
			meshBatch.matColors[meshPass.idx] = 104025f;
			meshBatch.colors[meshPass.idx] = p.color;
			meshBatch.matrices[meshPass.idx].m03 = p.x + offset.x * (float)num;
			meshBatch.matrices[meshPass.idx].m13 = p.y + offset.y + meshPass.pmesh.size.y;
			meshBatch.matrices[meshPass.idx].m23 = p.z + offset.z + RenderData.renderSetting.vFix.z + snowZ;
			meshPass.idx++;
			if (meshPass.idx == meshPass.batchSize)
			{
				meshPass.NextBatch();
			}
		}
	}

	private void OnValidate()
	{
		_offset = offset;
	}
}
