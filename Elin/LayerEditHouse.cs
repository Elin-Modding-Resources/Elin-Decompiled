using System.Collections.Generic;
using System.IO;
using System.Linq;
using SFB;
using UnityEngine;

public class LayerEditHouse : ELayer
{
	public TraitHouseBoard board;

	public UISlider sliderBlock;

	public UISlider sliderRoof;

	public UISlider sliderRamp;

	public UISlider sliderWallHeight;

	public UISlider sliderHeightFix;

	public UISlider sliderDeco;

	public UISlider sliderDeco2;

	public UISlider sliderDecoFix;

	public UISlider sliderDecoFix2;

	public UIDropdown ddRoofStyle;

	public UIDropdown ddTemplate;

	public UIButton toggleReverse;

	public UIButton toggleAltRoof;

	public UIButton buttonColorRoof;

	public UIButton buttonColorBlock;

	public UIButton buttonColorDeco;

	public UIButton buttonColorDeco2;

	public UIButton toggleAtrium;

	public int maxWallDeco;

	private bool first = true;

	public TraitHouseBoard.Data data => board.data;

	public override void OnInit()
	{
		windows[0].AddBottomButton("save", delegate
		{
			ELayer.core.WaitForEndOfFrame(delegate
			{
				string text = StandaloneFileBrowser.SaveFilePanel("Save Template", CorePath.LotTemplate, "new house", "tpl");
				if (!string.IsNullOrEmpty(text))
				{
					IO.SaveFile(text, board.data);
					RefreshTemplates();
				}
			});
		});
	}

	public void SetBoard(TraitHouseBoard b)
	{
		if ((bool)BuildMenu.Instance)
		{
			BuildMenu.Instance.SetActive(enable: false);
		}
		ELayer.screen.tileMap.usingHouseBoard = true;
		board = b;
		ddRoofStyle.SetList(data.idRoofStyle, ELayer.screen.tileMap.roofStyles, (RoofStyle a, int i) => a.GetName(i).lang(), delegate(int a, RoofStyle roofStyle)
		{
			data.idRoofStyle = a;
			ApplyData();
			RefreshBlockList();
		});
		toggleReverse.SetToggle(data.reverse, delegate(bool on)
		{
			data.reverse = on;
			ApplyData();
		});
		toggleAltRoof.SetToggle(data.altRoof, delegate(bool on)
		{
			data.altRoof = on;
			ApplyData();
		});
		List<int> blocks = new List<int>();
		int num = 0;
		blocks.Add(0);
		foreach (SourceObj.Row row in ELayer.sources.objs.rows)
		{
			if (row.tileType == TileType.Roof && !row.tag.Contains("hidden"))
			{
				blocks.Add(row.id);
			}
		}
		num = blocks.IndexOf(data.idRoofTile);
		if (num == -1)
		{
			num = 0;
		}
		sliderRoof.SetList(num, blocks, delegate(int a, int idRoofTile)
		{
			data.idRoofTile = idRoofTile;
			ApplyData();
		}, (int a) => blocks.IndexOf(a).ToString() ?? "");
		List<int> ramps = new List<int>();
		foreach (SourceBlock.Row row2 in ELayer.sources.blocks.rows)
		{
			if (row2.tileType == TileType.Stairs || row2.tileType == TileType.Slope)
			{
				ramps.Add(row2.id);
			}
		}
		int num2 = ramps.IndexOf(data.idRamp);
		if (num2 == -1)
		{
			num2 = 0;
		}
		sliderRamp.SetList(num2, ramps, delegate(int a, int idRamp)
		{
			data.idRamp = idRamp;
			ApplyData();
		}, (int a) => ramps.IndexOf(a).ToString() ?? "");
		sliderWallHeight.SetSlider(data.height, delegate(float a)
		{
			data.height = (int)a;
			ApplyData();
			return a.ToString() ?? "";
		}, 1, 20);
		sliderHeightFix.SetSlider(data.heightFix, delegate(float a)
		{
			data.heightFix = (int)a;
			ApplyData();
			return a.ToString() ?? "";
		}, 0, 60);
		sliderDecoFix.SetSlider(data.decoFix, delegate(float a)
		{
			data.decoFix = (int)a;
			ApplyData();
			return a.ToString() ?? "";
		}, -20, 20);
		sliderDecoFix2.SetSlider(data.decoFix2, delegate(float a)
		{
			data.decoFix2 = (int)a;
			ApplyData();
			return a.ToString() ?? "";
		}, -20, 400);
		sliderDeco.SetSlider(data.idDeco, delegate(float a)
		{
			data.idDeco = (int)a;
			ApplyData();
			return a.ToString() ?? "";
		}, 0, maxWallDeco);
		sliderDeco2.SetSlider(data.idDeco2, delegate(float a)
		{
			data.idDeco2 = (int)a;
			ApplyData();
			return a.ToString() ?? "";
		}, 0, maxWallDeco);
		if (first)
		{
			RefreshTemplates();
		}
		RefreshBlockList();
		first = false;
	}

	private void RefreshBlockList()
	{
		bool num = ELayer.screen.tileMap.roofStyles[data.idRoofStyle].type != RoofStyle.Type.FlatFloor;
		List<int> blocks = new List<int>();
		if (num)
		{
			foreach (SourceBlock.Row row in ELayer.sources.blocks.rows)
			{
				if (row.tileType == TileType.Block)
				{
					blocks.Add(row.id);
				}
			}
		}
		else
		{
			foreach (SourceFloor.Row row2 in ELayer.sources.floors.rows)
			{
				blocks.Add(row2.id);
			}
		}
		int num2 = blocks.IndexOf(data.idBlock);
		if (num2 == -1)
		{
			num2 = 0;
		}
		sliderBlock.SetList(num2, blocks, delegate(int a, int b)
		{
			data.idBlock = b;
			ApplyData();
		}, (int a) => blocks.IndexOf(a).ToString() ?? "");
		buttonColorRoof.icon.color = IntColor.FromInt(data.colRoof);
		buttonColorRoof.SetOnClick(delegate
		{
			ELayer.ui.AddLayer<LayerColorPicker>().SetColor(IntColor.FromInt(data.colRoof), IntColor.FromInt(data.colRoof), delegate(PickerState state, Color c)
			{
				data.colRoof = IntColor.ToInt(c);
				buttonColorRoof.icon.color = c;
				board.ApplyData();
			});
		});
		buttonColorBlock.icon.color = IntColor.FromInt(data.colBlock);
		buttonColorBlock.SetOnClick(delegate
		{
			ELayer.ui.AddLayer<LayerColorPicker>().SetColor(IntColor.FromInt(data.colBlock), IntColor.FromInt(data.colBlock), delegate(PickerState state, Color c)
			{
				data.colBlock = IntColor.ToInt(c);
				buttonColorBlock.icon.color = c;
				board.ApplyData();
			});
		});
		buttonColorDeco.icon.color = IntColor.FromInt(data.colDeco);
		buttonColorDeco.SetOnClick(delegate
		{
			ELayer.ui.AddLayer<LayerColorPicker>().SetColor(IntColor.FromInt(data.colDeco), IntColor.FromInt(data.colDeco), delegate(PickerState state, Color c)
			{
				data.colDeco = IntColor.ToInt(c);
				buttonColorDeco.icon.color = c;
				board.ApplyData();
			});
		});
		buttonColorDeco2.icon.color = IntColor.FromInt(data.colDeco2);
		buttonColorDeco2.SetOnClick(delegate
		{
			ELayer.ui.AddLayer<LayerColorPicker>().SetColor(IntColor.FromInt(data.colDeco2), IntColor.FromInt(data.colDeco2), delegate(PickerState state, Color c)
			{
				data.colDeco2 = IntColor.ToInt(c);
				buttonColorDeco2.icon.color = c;
				board.ApplyData();
			});
		});
	}

	public void RefreshTemplates()
	{
		FileInfo[] list = (from a in new DirectoryInfo(CorePath.LotTemplate).GetFiles()
			where a.Name.EndsWith(".tpl")
			select a).ToArray();
		ddTemplate.SetList(0, list, (FileInfo a, int b) => a.Name, delegate(int a, FileInfo b)
		{
			int idBGM = board.data.idBGM;
			board.data = IO.LoadFile<TraitHouseBoard.Data>(b.FullName);
			board.data.idBGM = idBGM;
			ApplyData();
			SetBoard(board);
		});
	}

	public void ApplyData()
	{
		board.ApplyData();
		toggleAltRoof.SetActive(board.data.idRoofTile != 0);
	}

	public override void OnKill()
	{
		ELayer.screen.tileMap.usingHouseBoard = false;
		if ((bool)BuildMenu.Instance)
		{
			BuildMenu.Instance.SetActive(enable: true);
		}
	}
}
