using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RenderRow : SourceData.BaseRow, IRenderSource
{
	public static Dictionary<string, RenderData> DictRenderData = new Dictionary<string, RenderData>();

	public int[] tiles;

	public int[] _tiles;

	public int[] skins;

	public int colorMod;

	public int sort;

	public int value;

	public int LV;

	public int chance;

	public int tempChance;

	public int snowTile;

	public string name;

	public string name_JP;

	public string detail;

	public string detail_JP;

	public string _idRenderData;

	public string _tileType;

	public string defMat;

	public string colorType;

	public string category;

	public string idSound;

	public string aliasPref;

	public string[] components;

	public string[] factory;

	public string[] recipeKey;

	public string[] tag;

	public int W = 1;

	public int H = 1;

	public bool multisize;

	public bool fixedMaterial;

	public SourcePref pref;

	[NonSerialized]
	public RenderData renderData;

	[NonSerialized]
	public Sprite[,] sprites;

	[NonSerialized]
	public TileType tileType;

	[NonSerialized]
	public bool useAltColor;

	[NonSerialized]
	public bool useRandomColor;

	[NonSerialized]
	public SourceMaterial.Row DefaultMaterial;

	[NonSerialized]
	public SpriteReplacer replacer;

	[NonSerialized]
	public string _nameSearch;

	[NonSerialized]
	public string _nameSearchJP;

	[NonSerialized]
	private SourceCategory.Row _category;

	[NonSerialized]
	private string _recipeCat;

	public virtual string idString => "";

	public virtual string RecipeID => "";

	public virtual string pathRenderData => "Scene/Render/Data/";

	public virtual string idRenderData => _idRenderData;

	public virtual RenderData defaultRenderData => ResourceCache.Load<RenderData>(pathRenderData + "_Default");

	public virtual string pathSprite => "Scene/Render/Data/";

	public virtual string idSprite => "";

	public virtual string prefabName => "ThingActor";

	public SourceCategory.Row Category => _category ?? (_category = sources.categories.map[category]);

	public string RecipeCat => _recipeCat ?? (_recipeCat = Category.recipeCat);

	public SourceManager sources => Core.Instance.sources;

	public bool HasTag(CTAG _tag)
	{
		return tag.Contains(_tag.ToString());
	}

	public virtual string GetSearchName(bool jp)
	{
		object obj;
		if (!jp)
		{
			obj = _nameSearch;
			if (obj == null)
			{
				return _nameSearch = name.ToLower();
			}
		}
		else
		{
			obj = _nameSearchJP ?? (_nameSearchJP = GetText().ToLower());
		}
		return (string)obj;
	}

	public bool ContainsTag(string _tag)
	{
		return tag.Contains(_tag);
	}

	public override void OnImportData(SourceData data)
	{
		base.OnImportData(data);
		_tiles = new int[0];
		SetTiles();
		if (defMat.Length > 0 && defMat[0] == '!')
		{
			fixedMaterial = true;
			defMat = defMat.Substring(1, defMat.Length - 1);
		}
		else
		{
			fixedMaterial = false;
		}
	}

	public void SetRenderData()
	{
		replacer = new SpriteReplacer();
		if (idRenderData.IsEmpty())
		{
			this.renderData = defaultRenderData;
		}
		else if (idRenderData[0] == '@')
		{
			string[] array = idRenderData.Replace("@", "").Split('#');
			string text = array[0];
			if (array.Length > 1)
			{
				aliasPref = array[1];
			}
			this.renderData = DictRenderData.TryGetValue(text);
			if (this.renderData == null)
			{
				RenderData renderData = (ResourceCache.Load<RenderData>(pathRenderData + text) ?? defaultRenderData).Instantiate();
				if (this is SourceChara.Row)
				{
					renderData.offset.x += renderData.pass.pmesh.size.x * 0.5f;
					renderData.offset.y += renderData.pass.pmesh.size.y * 0.5f;
				}
				else
				{
					float num = renderData.pass.pmesh.size.x * 0.5f + renderData.pass.pmesh.pos.x;
					float num2 = renderData.pass.pmesh.size.y * 0.5f + renderData.pass.pmesh.pos.y;
					renderData.offset.x += num;
					renderData.offset.y += num2;
					renderData.offsetBack.x += num;
					renderData.offsetBack.y += num2;
					renderData.heldPos.x += num * 0.7f;
					renderData.heldPos.y += num2 * 0.7f;
				}
				renderData.pass = null;
				this.renderData = renderData;
				DictRenderData[text] = renderData;
			}
		}
		else
		{
			this.renderData = ResourceCache.Load<RenderData>(pathRenderData + idRenderData) ?? defaultRenderData;
		}
		if (!Application.isEditor && pref == null)
		{
			if (aliasPref.IsEmpty())
			{
				pref = new SourcePref();
			}
			else
			{
				pref = sources.cards.map[aliasPref].pref;
			}
		}
		if (!this.renderData.initialized)
		{
			this.renderData.Init();
		}
		SetTiles();
		string text2 = colorType;
		if (!(text2 == "alt"))
		{
			if (text2 == "random")
			{
				useRandomColor = true;
			}
		}
		else
		{
			useAltColor = true;
		}
		DefaultMaterial = sources.materials.alias[defMat];
	}

	public virtual void SetTiles()
	{
		if ((bool)renderData && (bool)renderData.pass && _tiles.Length != tiles.Length)
		{
			_tiles = new int[tiles.Length];
			for (int i = 0; i < tiles.Length; i++)
			{
				_tiles[i] = tiles[i] / 100 * (int)renderData.pass.pmesh.tiling.x + tiles[i] % 100;
			}
		}
	}

	public int ConvertTile(int tile)
	{
		return renderData.ConvertTile(tile);
	}

	public Sprite GetSprite(int dir = 0, int skin = 0, bool snow = false)
	{
		if (replacer.HasSprite(idSprite))
		{
			return replacer.data.GetSprite(snow);
		}
		int[] array = null ?? _tiles;
		if (sprites == null)
		{
			sprites = new Sprite[(skins == null) ? 1 : (skins.Length + 1), (array.Length == 0) ? 1 : array.Length];
		}
		if (skin != 0 && skin >= sprites.GetLength(0))
		{
			skin = 0;
		}
		if (dir < 0)
		{
			dir *= -1;
		}
		if (dir >= array.Length)
		{
			dir = 0;
		}
		if (!sprites[skin, dir])
		{
			MeshPass pass = renderData.pass;
			if ((bool)pass)
			{
				Texture2D texture2D = pass.mat.GetTexture("_MainTex") as Texture2D;
				ProceduralMesh pmesh = pass.pmesh;
				int num = 0;
				num = ((skin != 0) ? Mathf.Abs(array[dir] + skins[skin - 1] * ((array[dir] > 0) ? 1 : (-1))) : Mathf.Abs(array[dir]));
				int num2 = (int)((float)texture2D.width / pmesh.tiling.x);
				int num3 = (int)((float)texture2D.height / pmesh.tiling.y);
				int num4 = (int)((float)num % pmesh.tiling.x);
				int num5 = (int)((float)num / pmesh.tiling.x);
				sprites[skin, dir] = Sprite.Create(texture2D, new Rect(num4 * num2, texture2D.height - (num5 + 1) * num3, num2, num3 * ((!renderData.multiSize) ? 1 : 2)), Vector2.zero, 100f, 0u, SpriteMeshType.FullRect);
			}
			else
			{
				sprites[skin, dir] = SpriteSheet.Get(idSprite);
				if (!sprites[skin, dir])
				{
					sprites[skin, dir] = Resources.Load<Sprite>(pathSprite + idSprite) ?? Resources.Load<Sprite>(pathSprite + idSprite + "_0");
				}
			}
			if (!sprites[skin, dir])
			{
				sprites[skin, dir] = Core.Instance.refs.spriteNull;
			}
		}
		return sprites[skin, dir];
	}

	public void SetSpriteRenderer(SpriteRenderer sr, Sprite sprite = null, int matCol = 0, bool setTransform = true, int dir = 0)
	{
		sr.sprite = sprite ?? GetSprite(dir);
		if (renderData is RenderDataThing)
		{
			sr.sprite = EClass.core.refs.spriteThingActor;
		}
		int num = ((matCol == 0) ? 104025 : matCol);
		float num2 = (float)(num / 262144) * 0.01f;
		float num3 = 0.02f;
		float num4 = 0.3f;
		if (num2 != 0f)
		{
			num3 *= num2;
		}
		Color color = new Color(num3 * (float)(num % 262144 / 4096) + num4, num3 * (float)(num % 4096 / 64) + num4, num3 * (float)(num % 64) + num4, 1f);
		sr.color = color;
		if (setTransform)
		{
			float num5 = sr.bounds.max.x - sr.bounds.min.x;
			_ = sr.bounds.max;
			_ = sr.bounds.min;
			sr.transform.localPosition = new Vector3(-0.5f * num5, 0f, 0f);
		}
	}

	public virtual SourcePref GetPref()
	{
		return pref;
	}

	public void SetImage(Image image, Sprite sprite = null, int matCol = 0, bool setNativeSize = true, int dir = 0, int idSkin = 0)
	{
		image.sprite = sprite ?? GetSprite(dir, idSkin);
		int num = ((matCol == 0) ? 104025 : matCol);
		float num2 = 0.02f;
		Color color = new Color(num2 * (float)(num % 262144 / 4096), num2 * (float)(num % 4096 / 64), num2 * (float)(num % 64), (float)(num / 262144) * 0.01f);
		color.a = 1f;
		image.color = color;
		RectTransform rectTransform = image.Rect();
		SourcePref sourcePref = GetPref();
		rectTransform.pivot = renderData.imagePivot - new Vector2(0.01f * (float)sourcePref.pivotX, 0.01f * (float)sourcePref.pivotY);
		float x = Mathf.Abs(image.transform.localScale.x) * (float)((_tiles == null || _tiles.Length == 0 || _tiles[dir % _tiles.Length] >= 0) ? 1 : (-1));
		float y = image.transform.localScale.y;
		image.transform.localScale = new Vector3(x, y, image.transform.localScale.z);
		sourcePref.Validate();
		if (setNativeSize)
		{
			image.SetNativeSize();
			if (renderData.imageScale.x != 1f || renderData.imageScale.y != 1f || sourcePref.scaleIcon != 0)
			{
				rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x * (renderData.imageScale.x + (float)sourcePref.scaleIcon * 0.01f), rectTransform.sizeDelta.y * (renderData.imageScale.y + (float)sourcePref.scaleIcon * 0.01f));
			}
		}
	}

	public void SetRenderParam(RenderParam p, SourceMaterial.Row mat, int dir)
	{
		p.tile = GetTile(mat ?? DefaultMaterial, dir);
		p.matColor = GetColorInt(mat ?? DefaultMaterial);
		p.mat = mat ?? DefaultMaterial;
		p.liquidLv = 0;
		p.dir = dir;
		p.cell = null;
	}

	public virtual RenderParam GetRenderParam(SourceMaterial.Row mat, int dir, Point point = null, int bridgeHeight = -1)
	{
		RenderParam shared = RenderParam.shared;
		shared.tile = GetTile(mat, dir);
		shared.matColor = GetColorInt(mat);
		shared.mat = mat;
		shared.liquidLv = 0;
		shared.dir = dir;
		shared.cell = null;
		Vector3 vector;
		if (point == null)
		{
			shared.color = 11010048f;
			vector = Core.Instance.scene.camSupport.renderPos;
			if (renderData.multiSize)
			{
				vector.y -= 0.8f;
			}
		}
		else
		{
			shared.color = 10485760f;
			_ = Core.Instance.screen.tileMap;
			vector = point.Position(bridgeHeight);
		}
		shared.x = vector.x;
		shared.y = vector.y;
		shared.z = vector.z;
		return shared;
	}

	public virtual int GetTile(SourceMaterial.Row mat, int dir = 0)
	{
		return _tiles[dir % _tiles.Length];
	}

	public int GetColorInt(SourceMaterial.Row mat)
	{
		if (useAltColor)
		{
			if (colorMod != 0)
			{
				return BaseTileMap.GetColorInt(ref mat.altColor, colorMod);
			}
			return 104025;
		}
		if (colorMod != 0)
		{
			return BaseTileMap.GetColorInt(ref mat.matColor, colorMod);
		}
		return 104025;
	}
}
