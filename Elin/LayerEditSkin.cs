using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.UI;

public class LayerEditSkin : ELayer
{
	public Image imageSkin;

	public Chara chara;

	public UIDynamicList list;

	public void Activate(Chara _chara)
	{
		chara = _chara;
		RefreshImage();
		RefreshList();
	}

	public void RefreshImage()
	{
		imageSkin.sprite = chara.GetSprite();
		imageSkin.SetNativeSize();
		if ((bool)WidgetRoster.Instance)
		{
			WidgetRoster.Instance.Build();
		}
	}

	public void OnClickClear()
	{
		SE.Trash();
		chara.c_idSpriteReplacer = null;
		chara._CreateRenderer();
		RefreshImage();
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
				RefreshImage();
			},
			onRedraw = delegate(SpriteData a, UIButton b, int i)
			{
				b.mainText.SetText(Path.GetFileName(a.path));
				a.Init();
				b.icon.sprite = a.GetSprite();
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
}
