using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ContentGallery : EContent
{
	public class Page : UIBook.Page
	{
		public class Item
		{
			public int id;
		}

		public List<string> ids = new List<string>();

		public override void BuildNote(UINote n, string idTopic)
		{
			foreach (string id in ids)
			{
				UIItem uIItem = n.AddItem("ItemGallery");
				int idx = id.ToInt();
				string path = EClass.core.refs.dictSketches2[idx];
				Sprite sprite = sprites.TryGetValue(idx);
				if (!sprite)
				{
					Texture2D texture2D = IO.LoadPNG(path + "_t");
					sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
					sprites[idx] = sprite;
				}
				uIItem.image1.sprite = sprite;
				uIItem.text1.text = "#" + id;
				uIItem.button1.SetOnClick(delegate
				{
					SE.Play("click_recipe");
					Sprite sprite2 = spritesFull.TryGetValue(idx);
					if (!sprite2)
					{
						Texture2D texture2D2 = IO.LoadPNG(path);
						sprite2 = Sprite.Create(texture2D2, new Rect(0f, 0f, texture2D2.width, texture2D2.height), new Vector2(0.5f, 0.5f));
						spritesFull[idx] = sprite2;
					}
					EClass.ui.AddLayer<LayerImage>().SetImage(sprite2);
				});
			}
		}
	}

	public static int lastPage;

	public static bool listMode;

	public static Dictionary<int, Sprite> sprites = new Dictionary<int, Sprite>();

	public static Dictionary<int, Sprite> spritesFull = new Dictionary<int, Sprite>();

	public Transform transBig;

	public Image imageBig;

	public UIBook book;

	public UIText textCollected;

	public GridLayoutGroup[] grids;

	public Vector2[] gridSize;

	private bool first = true;

	public override void OnSwitchContent(int idTab)
	{
		if (!first)
		{
			return;
		}
		if (EClass.debug.allArt)
		{
			EClass.player.sketches.Clear();
			foreach (int key in EClass.core.refs.dictSketches2.Keys)
			{
				EClass.player.sketches.Add(key);
			}
		}
		Refresh();
		first = false;
	}

	public void Refresh()
	{
		book.pages.Clear();
		GridLayoutGroup[] array = grids;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].cellSize = gridSize[listMode ? 1 : 0];
		}
		Page page = new Page();
		List<int> list = EClass.player.sketches.ToList();
		list.Sort((int a, int b) => a - b);
		foreach (int item in list)
		{
			page.ids.Add(item.ToString() ?? "");
			if (page.ids.Count >= (listMode ? 8 : 2))
			{
				book.AddPage(page);
				page = new Page();
			}
		}
		if (page.ids.Count > 0)
		{
			book.AddPage(page);
		}
		book.currentPage = lastPage;
		book.Show();
		textCollected.SetText("sketch_collected".lang((list.Count * 100 / EClass.core.refs.dictSketches2.Count()).ToString() ?? ""));
	}

	public void OnClickHelp()
	{
		LayerHelp.Toggle("other", "gallery");
	}

	public void ToggleMode()
	{
		listMode = !listMode;
		lastPage = (listMode ? (book.currentPage / 4) : (book.currentPage * 4));
		SE.Tab();
		Refresh();
	}

	private void OnDestroy()
	{
		lastPage = book.currentPage;
		foreach (Sprite item in sprites.Values.Concat(spritesFull.Values))
		{
			Object.Destroy(item.texture);
			Object.Destroy(item);
		}
		sprites.Clear();
		spritesFull.Clear();
	}
}
