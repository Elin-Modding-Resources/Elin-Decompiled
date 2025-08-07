using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.UI;

public class LayerEditSkin : ELayer
{
	public Image imageSkin;

	public Chara chara;

	public UIDynamicList list;

	public SpriteData currentData;

	public void Activate(Chara _chara)
	{
		chara = _chara;
		RefreshImage();
		RefreshList();
	}

	public void RefreshImage(SpriteData data = null)
	{
		data?.LoadPref();
		imageSkin.sprite = chara.GetSprite();
		imageSkin.SetNativeSize();
		WidgetRoster.SetDirty();
		currentData = data ?? chara.spriteReplacer?.data;
	}

	public void OnClickClear()
	{
		SE.Trash();
		chara.c_idSpriteReplacer = null;
		chara._CreateRenderer();
		RefreshImage();
	}

	public void OnClickEdit()
	{
		if (currentData == null)
		{
			SE.Beep();
			return;
		}
		string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(currentData.path);
		string text = CorePath.custom + "Skin/" + fileNameWithoutExtension + ".pref";
		if (!File.Exists(text))
		{
			(currentData.pref ?? chara.Pref).WriteIni(text);
		}
		SE.Click();
		Util.Run(text);
	}

	public void RefreshList()
	{
		list.Clear();
		list.callbacks = new UIList.Callback<SpriteData, UIButton>
		{
			onClick = delegate(SpriteData a, UIButton b)
			{
				list.Select(a);
				SE.Click();
				chara.c_idSpriteReplacer = Path.GetFileName(a.path);
				chara._CreateRenderer();
				RefreshImage(a);
			},
			onRedraw = delegate(SpriteData a, UIButton b, int i)
			{
				b.mainText.SetText(Path.GetFileName(a.path));
				a.Init();
				b.icon.sprite = a.GetSprite();
				b.icon.SetNativeSize();
				b.tooltip.lang = a.path;
			},
			onList = delegate
			{
				List<SpriteReplacer> obj = SpriteReplacer.ListSkins().Values.ToList();
				obj.Sort((SpriteReplacer a, SpriteReplacer b) => Lang.comparer.Compare(a.data.path, b.data.path));
				foreach (SpriteReplacer item in obj)
				{
					list.Add(item.data);
				}
			}
		};
		list.List();
	}

	private void OnApplicationFocus(bool focus)
	{
		if (focus && currentData != null)
		{
			RefreshImage(currentData);
		}
	}
}
