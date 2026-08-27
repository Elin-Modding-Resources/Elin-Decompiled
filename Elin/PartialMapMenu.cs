using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class PartialMapMenu : EMono
{
	public static PartialMapMenu Instance;

	public static DirectoryInfo lastDir;

	public static DirectoryInfo lastDirSub;

	public GameObject goMain;

	public GameObject goCat;

	public GameObject goOption;

	public UIList listCat;

	public UIList listCatSub;

	public UIDynamicList list;

	public CanvasGroup cg;

	public UIButton buttonSave;

	public UIButton buttonEdit;

	public UIButton buttonDelete;

	public UIButton toggleRotate;

	public UIButton toggleIgnoreBlock;

	public Dictionary<string, Sprite> dictSprite = new Dictionary<string, Sprite>();

	public DirectoryInfo rootDir;

	public AM_Copy am => ActionMode.Copy;

	public DirectoryInfo dir => am.dir;

	public bool IsEditor => MapPiece.IsEditor;

	public PartialMap partial => am.partialMap;

	public static void Activate()
	{
		if (!Instance)
		{
			Instance = Util.Instantiate<PartialMapMenu>("UI/BuildMenu/PartialMapMenu", EMono.ui);
			Instance.Init();
		}
		Instance.Refresh();
		MapPiece.CacheMap.Clear();
	}

	public void Init()
	{
		rootDir = new DirectoryInfo(CorePath.MapPieceSaveUser);
		goCat.SetActive(IsEditor);
		RefreshOptions();
		if (!IsEditor)
		{
			return;
		}
		RefreshCategory();
		if (lastDir != null)
		{
			listCat.Select((DirectoryInfo a) => a.FullName == lastDir.FullName);
			am.dir = lastDir;
		}
		if (lastDirSub != null)
		{
			RefreshCategory(lastDir);
			listCatSub.Select((DirectoryInfo a) => a.FullName == lastDirSub.FullName);
			am.dir = lastDirSub;
		}
	}

	public void Deactivate()
	{
		DestorySprites();
		Object.DestroyImmediate(Instance.gameObject);
	}

	public void SetVisible(bool visible)
	{
		RefreshOptions();
		goMain.SetActive(visible);
	}

	public void DestorySprites()
	{
		foreach (Sprite value in dictSprite.Values)
		{
			Object.Destroy(value.texture);
			Object.Destroy(value);
		}
		dictSprite.Clear();
	}

	public void RefreshOptions()
	{
		goOption.SetActive(IsEditor && partial != null);
		if (partial != null)
		{
			toggleRotate.SetToggle(partial.allowRotate, delegate(bool on)
			{
				partial.allowRotate = on;
				partial.Update();
			});
			toggleIgnoreBlock.SetToggle(partial.ignoreBlock, delegate(bool on)
			{
				partial.ignoreBlock = on;
				partial.Update();
			});
		}
	}

	public void Refresh()
	{
		list.callbacks = new UIList.Callback<MapMetaData, ButtonGrid>
		{
			onClick = delegate(MapMetaData a, ButtonGrid b)
			{
				SE.Click();
				am.Import(a.path);
				am.partialMap.name = a.name;
			},
			onRedraw = delegate(MapMetaData a, ButtonGrid b, int i)
			{
				string path = a.path.GetFullFileNameWithoutExtension() + ".txt";
				if (File.Exists(path))
				{
					string str = File.ReadAllText(path);
					if (!str.IsEmpty())
					{
						a.name = str;
					}
				}
				if (a.name.IsEmpty())
				{
					a.name = "unknown";
				}
				b.mainText.SetText(IsEditor ? new FileInfo(a.path).Name : a.name);
				b.subText.text = Lang._currency(a.partial.value);
				Sprite sprite = dictSprite.TryGetValue(a.path);
				if (!sprite)
				{
					string path2 = a.path.GetFullFileNameWithoutExtension() + ".jpg";
					if (File.Exists(path2))
					{
						Texture2D texture2D = new Texture2D(1, 1);
						byte[] source = File.ReadAllBytes(path2);
						texture2D.LoadImage(source.ToArray());
						sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
						b.icon.sprite = sprite;
						dictSprite[a.path] = sprite;
					}
				}
				if ((bool)sprite)
				{
					b.icon.sprite = sprite;
				}
				UIButton componentInDirectChildren = b.GetComponentInDirectChildren<UIButton>();
				componentInDirectChildren.SetActive(IsEditor);
				componentInDirectChildren.SetOnClick(delegate
				{
					SE.Click();
					am.overwritePath = a.path;
					am.RefreshMenu(show: false);
				});
			},
			onList = delegate
			{
				foreach (FileInfo item in dir.GetFiles().Concat(MOD.listPartialMaps))
				{
					if (item.Name.EndsWith(".mp"))
					{
						MapMetaData metaData = Map.GetMetaData(item.FullName);
						if (metaData != null && metaData.partial != null)
						{
							list.Add(metaData);
						}
					}
				}
			}
		};
		list.List();
	}

	public void RefreshCategory(DirectoryInfo dir = null)
	{
		UIList _listCat = listCat;
		bool isMain = true;
		if (dir == null)
		{
			dir = rootDir;
		}
		if (dir.FullName != rootDir.FullName)
		{
			_listCat = listCatSub;
			isMain = false;
		}
		_listCat.Clear();
		_listCat.callbacks = new UIList.Callback<DirectoryInfo, UIButton>
		{
			onClick = delegate(DirectoryInfo a, UIButton b)
			{
				SE.Click();
				_listCat.Select(a);
				am.dir = a;
				if (isMain)
				{
					lastDir = (lastDirSub = a);
					RefreshCategory(a);
				}
				else
				{
					lastDirSub = a;
				}
				Refresh();
			},
			onInstantiate = delegate(DirectoryInfo a, UIButton b)
			{
				int num = 0;
				FileInfo[] files = a.GetFiles("*.mp", SearchOption.AllDirectories);
				for (int i = 0; i < files.Length; i++)
				{
					_ = files[i];
					num++;
				}
				b.mainText.text = ((a == dir && !isMain) ? "(Root)" : (a.Name + "(" + num + ")"));
			},
			onList = delegate
			{
				_listCat.Add(isMain ? rootDir : dir);
				DirectoryInfo[] array = (isMain ? new DirectoryInfo(CorePath.MapPieceSave).GetDirectories() : dir.GetDirectories());
				foreach (DirectoryInfo o in array)
				{
					_listCat.Add(o);
				}
			}
		};
		_listCat.List();
		listCatSub.SetActive(listCatSub.items.Count > 1);
	}

	public void OpenFolder()
	{
		Util.ShowExplorer(CorePath.MapPieceSave + "Test");
	}
}
