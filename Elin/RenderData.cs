using System;
using UnityEngine;

public class RenderData : EScriptable, IRenderer
{
	public const int HeldLightMod = 1572864;

	public static Quaternion deadRotation = Quaternion.Euler(0f, 0f, 90f);

	[NonSerialized]
	public Vector3 _offset;

	public static GameSetting.RenderSetting renderSetting;

	public int idShadow;

	public MeshPass pass;

	public Vector3 offset;

	public Vector3 offsetBack;

	public Vector3 offsetShadow;

	public Vector3 heldPos;

	public Vector2 imagePivot = new Vector2(0.5f, 0.25f);

	public Vector2 imageScale = new Vector2(1f, 1f);

	public Vector2 size;

	public bool multiSize;

	public bool animate;

	public bool useOffsetBack;

	public bool persistActor;

	public bool symmetry = true;

	public float hangedFixZ;

	public float stackZ;

	public SubPassData subCrate;

	public SourcePref shadowPref;

	public RenderData subData;

	[NonSerialized]
	public bool initialized;

	[NonSerialized]
	private Sprite _sprite;

	[NonSerialized]
	public bool hasSubPass;

	[NonSerialized]
	public bool hasSnowPass;

	private float orgX;

	private float orgY;

	private float orgZ;

	public static Quaternion shadowRotation => Quaternion.Euler(0f, 0f, renderSetting.shadowAngle);

	public static Vector3 shadowScale => renderSetting.shadowScale;

	public static Vector3 shadowOffset => renderSetting.shadowOffset;

	public virtual bool SkipOnMap => false;

	public virtual string pathSprite => "Scene/Render/Data/";

	public virtual string prefabName => "ThingActor";

	public virtual bool ForceAltHeldPosition => false;

	public virtual CardActor CreateActor()
	{
		return null;
	}

	private void Awake()
	{
		Init();
	}

	public void Init()
	{
		if (!initialized)
		{
			initialized = true;
			_offset = offset;
			hasSubPass = pass?.subPass;
			hasSnowPass = pass?.snowPass;
			OnInit();
		}
	}

	public virtual void OnInit()
	{
	}

	public int ConvertTile(int tile)
	{
		return tile / 100 * (int)pass.pmesh.tiling.x + tile % 100;
	}

	public void RenderToRenderCam(RenderParam p)
	{
		Vector3 renderPos = EClass.scene.camSupport.renderPos;
		if (multiSize)
		{
			renderPos.y -= 0.8f;
		}
		p.x = renderPos.x;
		p.y = renderPos.y;
		p.z = renderPos.z;
		Draw(p);
	}

	public void Draw(RenderParam p, int tile)
	{
		p.tile = tile;
		Draw(p);
	}

	public virtual void Draw(RenderParam p)
	{
		MeshPass meshPass = ((hasSubPass && SubPassData.Current.enable) ? pass.subPass : pass);
		MeshBatch meshBatch = meshPass.batches[meshPass.batchIdx];
		int num = ((p.tile > 0f) ? 1 : (-1));
		if (useOffsetBack)
		{
			_offset = ((p.dir == 2 || p.dir == 3) ? offsetBack : offset);
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
		meshPass.idx++;
		if (meshPass.idx == meshPass.batchSize)
		{
			meshPass.NextBatch();
		}
	}

	private void OnValidate()
	{
		_offset = offset;
	}

	public void DrawRepeatTo(RenderParam p, float maxY, float height, ref Vector3 peakFix, int skipRepeat = 0, int fire = 0, bool isBlock = false)
	{
		int num = (int)((maxY + height + peakFix.x - p.y) / peakFix.y);
		bool snow = p.snow;
		if (num == 0)
		{
			if (skipRepeat == 0)
			{
				Draw(p);
			}
			return;
		}
		orgY = p.y;
		orgZ = p.z;
		p.snow = !isBlock && snow;
		for (int i = 0; i < num + 1; i++)
		{
			if (i == num)
			{
				p.y = maxY + height - peakFix.y;
				p.z -= peakFix.z + peakFix.x;
				p.snow = snow;
			}
			if (skipRepeat > 0)
			{
				if (i >= skipRepeat && num > skipRepeat)
				{
					Draw(p);
				}
			}
			else
			{
				Draw(p);
			}
			if (fire > 0 && i != num)
			{
				EClass.screen.tileMap.rendererEffect.Draw(p, fire);
				fire--;
			}
			p.y += peakFix.y;
			p.z += peakFix.z;
		}
		p.y = orgY;
		p.z = orgZ;
	}

	public void DrawRepeat(RenderParam p, int count, float size, bool skipFirst = false)
	{
		if (count == 1)
		{
			if (!skipFirst)
			{
				Draw(p);
			}
			return;
		}
		bool snow = p.snow;
		orgY = p.y;
		orgZ = p.z;
		p.snow = false;
		for (int i = 0; i < count; i++)
		{
			if (i == count - 1)
			{
				p.snow = snow;
			}
			if (i != 0 || !skipFirst)
			{
				Draw(p);
			}
			p.y += renderSetting.peakFix.y * size;
			p.z += renderSetting.peakFix.z * size;
		}
		p.y = orgY;
		p.z = orgZ;
	}

	public virtual void DrawWithRotation(RenderParam p, float angle)
	{
	}

	public virtual void DrawShadow(RenderParam p)
	{
		int num = ((p.tile > 0f) ? 1 : (-1));
		MeshPass shadowPass = pass.shadowPass;
		MeshBatch meshBatch = shadowPass.batches[shadowPass.batchIdx];
		meshBatch.matrices[shadowPass.idx].SetTRS(p.NewVector3 + offset + shadowOffset, shadowRotation, shadowScale);
		meshBatch.tiles[shadowPass.idx] = p.tile;
		shadowPass.idx++;
		if (shadowPass.idx == shadowPass.batchSize)
		{
			shadowPass.NextBatch();
		}
		if (multiSize)
		{
			shadowPass = pass.shadowPass;
			MeshBatch meshBatch2 = shadowPass.batches[shadowPass.batchIdx];
			meshBatch2.matrices[shadowPass.idx].SetTRS(p.v + offsetShadow, shadowRotation, shadowScale);
			meshBatch2.tiles[shadowPass.idx] = p.tile - pass.pmesh.tiling.x * (float)num;
			shadowPass.idx++;
			if (shadowPass.idx == shadowPass.batchSize)
			{
				shadowPass.NextBatch();
			}
		}
	}
}
