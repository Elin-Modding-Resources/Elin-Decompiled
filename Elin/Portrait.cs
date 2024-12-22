using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Portrait : UIButton
{
	public static ModItemList<Sprite> modPortraitBGFs = new ModItemList<Sprite>(3);

	public static ModItemList<Sprite> modPortraitBGs = new ModItemList<Sprite>(3);

	public static ModItemList<Sprite> modPortraits = new ModItemList<Sprite>(3);

	public static ModItemList<Sprite> modOverlays = new ModItemList<Sprite>(3);

	public static ModItemList<Sprite> modFull = new ModItemList<Sprite>(3);

	public static Dictionary<string, List<ModItem<Sprite>>> dictList = new Dictionary<string, List<ModItem<Sprite>>>();

	public static HashSet<string> allIds = new HashSet<string>();

	public Image portrait;

	public Image overlay;

	public Image imageChara;

	public Image imageFaith;

	public Image imageFull;

	public Image imageFrame;

	public Sprite spriteNoPortrait;

	public bool enableFull = true;

	public bool hideIfNoPortrait;

	public bool fixSpritePos = true;

	public bool showPortrait = true;

	public float charaScale = 1f;

	private FilterMode filter;

	public static List<ModItem<Sprite>> ListPlayerPortraits(int gender, bool nullPortrait = false)
	{
		List<ModItem<Sprite>> list = ListPortraits(gender, "c").Concat(ListPortraits(gender, "guard")).Concat(ListPortraits(gender, "special")).Concat(ListPortraits(gender, "foxfolk"))
			.ToList();
		if (nullPortrait)
		{
			list.Insert(0, new ModItem<Sprite>(null));
		}
		return list;
	}

	public static List<ModItem<Sprite>> ListPortraits(string idDict)
	{
		List<ModItem<Sprite>> list = dictList.TryGetValue(idDict);
		if (list == null || list.Count == 0)
		{
			list = new List<ModItem<Sprite>>();
			foreach (ModItem<Sprite> item in modPortraits.list)
			{
				if (item.id.Contains(idDict))
				{
					list.Add(item);
				}
			}
			dictList.Add(idDict, list);
		}
		return list;
	}

	public static List<ModItem<Sprite>> ListPortraits(int gender, string cat)
	{
		if (cat.IsEmpty())
		{
			cat = "c";
		}
		string key = cat + gender;
		List<ModItem<Sprite>> list = dictList.TryGetValue(key);
		if (list == null || list.Count == 0)
		{
			list = new List<ModItem<Sprite>>();
			foreach (ModItem<Sprite> item in modPortraits.list)
			{
				string[] array = item.id.Split('-')[0].Split('_');
				if (!(array[0] != cat))
				{
					int num = ((array.Length > 1) ? ((array[1] == "m") ? 2 : ((array[1] == "f") ? 1 : 0)) : 0);
					if (num == 0 || gender == 0 || gender == num)
					{
						list.Add(item);
					}
				}
			}
			dictList.Add(key, list);
		}
		return list;
	}

	public static string GetRandomPortrait(string idDict)
	{
		return ListPortraits(idDict).RandomItem().id;
	}

	public static string GetRandomPortrait(int gender, string cat)
	{
		return ListPortraits(gender, cat).RandomItem().id;
	}

	public static bool Exists(string id)
	{
		return modPortraits.dict.ContainsKey(id);
	}

	public void SetPerson(Person p)
	{
		if (p.hasChara)
		{
			SetChara(p.chara);
		}
		else if (!p.idPortrait.IsEmpty())
		{
			Rand.SetSeed(p.uidChara);
			SetPortrait(p.idPortrait, PCCManager.current.GetBodySet("female").map["hair"].GetRandomColor());
			Rand.SetSeed();
		}
		else
		{
			SetPortrait(p.source.portrait);
		}
	}

	public void SetChara(Chara c, PCCData pccData = null)
	{
		portrait.enabled = true;
		overlay.enabled = true;
		if ((bool)imageChara)
		{
			imageChara.sprite = c.GetSprite();
			imageChara.SetNativeSize();
			if (fixSpritePos)
			{
				imageChara.transform.localScale = (c.IsPCC ? new Vector3(1.3f, 1.4f, 1f) : new Vector3(1f, 1f, 1f)) * charaScale;
				imageChara.rectTransform.pivot = (c.IsPCC ? new Vector2(0.5f, 0.4f) : new Vector2(0.5f, 0.2f));
				imageChara.rectTransform.anchoredPosition = (c.IsPCC ? new Vector2(-25f, 20f) : new Vector2(-25f, 20f));
			}
		}
		if ((bool)imageFaith)
		{
			imageFaith.sprite = c.faith.GetSprite();
		}
		if (c.GetIdPortrait().IsEmpty() || modPortraits.GetItem(c.GetIdPortrait(), returnNull: true) == null)
		{
			if (hideIfNoPortrait)
			{
				portrait.enabled = false;
				overlay.enabled = false;
				return;
			}
			if ((bool)spriteNoPortrait)
			{
				SetPortrait(isPortrait: false, spriteNoPortrait);
				return;
			}
			SetPortrait(isPortrait: false, c.GetSprite());
			c.SetImage(portrait);
			portrait.transform.localScale = new Vector3(1f, 1f, 1f);
			portrait.preserveAspect = true;
			portrait.SetNativeSize();
			portrait.Rect().SetAnchor(0.5f, 0f, 0.5f, 0f);
			portrait.Rect().SetPivot(0.5f, 0f);
			portrait.Rect().anchoredPosition = new Vector2(0f, 0f);
			portrait.material = EClass.core.refs.matUIPortraitChara;
			return;
		}
		if (pccData == null && c.isChara)
		{
			pccData = c.Chara.pccData;
		}
		Color colorOverlay = pccData?.GetHairColor(applyMod: true) ?? Color.white;
		overlay.enabled = true;
		if (pccData == null)
		{
			if (c.GetInt(105) != 0)
			{
				colorOverlay = IntColor.FromInt(c.GetInt(105));
			}
			else
			{
				colorOverlay = GetRandomHairColor(c);
				if (c.id == "shojo")
				{
					overlay.enabled = false;
				}
			}
		}
		SetPortrait(c.GetIdPortrait(), colorOverlay);
	}

	public Color GetRandomHairColor(Chara c)
	{
		Rand.SetSeed(c.uid);
		Color randomColor = PCCManager.current.GetBodySet("female").map["hair"].GetRandomColor();
		Rand.SetSeed();
		return randomColor;
	}

	public void SetPortrait(string id, Color colorOverlay = default(Color), bool applyColorMod = true)
	{
		Sprite @object = modPortraits.GetItem(id).GetObject();
		Sprite object2 = modOverlays.GetObject(id + "-overlay");
		Sprite spriteFull = ((enableFull && (bool)imageFull) ? modFull.GetObject(id + "-full") : null);
		if (applyColorMod)
		{
			colorOverlay = PCCManager.current.ApplyColorMod(colorOverlay);
		}
		SetPortrait(isPortrait: true, @object, object2, colorOverlay, spriteFull);
	}

	public void SetPortrait(bool isPortrait, Sprite spritePortrait, Sprite spriteOverlay = null, Color colorOverlay = default(Color), Sprite spriteFull = null)
	{
		filter = ((isPortrait && Core.Instance.config.test.aaPortrait) ? FilterMode.Bilinear : FilterMode.Point);
		portrait.rectTransform.anchorMin = overlay.rectTransform.anchorMin;
		portrait.rectTransform.anchorMax = overlay.rectTransform.anchorMax;
		portrait.rectTransform.sizeDelta = overlay.rectTransform.sizeDelta;
		if ((bool)spritePortrait)
		{
			portrait.sprite = spritePortrait;
			FixTexture(spritePortrait);
		}
		if ((bool)overlay)
		{
			FixTexture(spriteOverlay);
			overlay.sprite = spriteOverlay;
			overlay.color = new Color(colorOverlay.r * 1.1f, colorOverlay.g * 1.1f, colorOverlay.b * 1.1f, colorOverlay.a);
			overlay.SetActive(spriteOverlay);
		}
		if (enableFull && (bool)imageFull)
		{
			FixTexture(spriteFull);
			imageFull.sprite = spriteFull;
			base.gameObject.SetActive(!spriteFull);
			imageFull.SetActive(spriteFull);
		}
		if (!showPortrait)
		{
			portrait.SetActive(enable: false);
			overlay.SetActive(enable: false);
		}
		void FixTexture(Sprite s)
		{
			if ((bool)s && isPortrait)
			{
				s.texture.wrapMode = TextureWrapMode.Clamp;
				if (s.texture.filterMode != filter)
				{
					s.texture.filterMode = filter;
				}
			}
		}
	}
}
