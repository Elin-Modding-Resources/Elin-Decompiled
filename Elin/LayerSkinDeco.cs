using System;
using System.Collections.Generic;
using UnityEngine;

public class LayerSkinDeco : ELayer
{
	[NonSerialized]
	public Widget widget;

	public Transform widgetHolder;

	public List<SkinDeco> decos => widget.config.skin.decos;

	public SkinConfig cfg => widget.config.skin;

	public override void OnInit()
	{
		base.OnInit();
	}

	public void SetWidget(Widget _widget)
	{
		widget = _widget;
		foreach (SkinDeco deco in decos)
		{
			Activate(deco.actor);
		}
		widget.transform.SetParent(widgetHolder, worldPositionStays: false);
	}

	public override void OnUpdateInput()
	{
		if (EInput.leftMouse.clicked)
		{
			if (EInput.leftMouse.dragging)
			{
				return;
			}
			SkinDecoActor selected = InputModuleEX.GetComponentOf<SkinDecoActor>();
			Vector3 clickPos = Input.mousePosition;
			UIContextMenu m = ELayer.ui.CreateContextMenu();
			if ((bool)selected)
			{
				SkinDeco deco = selected.owner;
				m.AddButton("editColor", delegate
				{
					ELayer.ui.AddLayer<LayerColorPicker>().SetColor(deco.color, Color.white, delegate(PickerState state, Color _c)
					{
						deco.color = _c;
						selected.Refresh();
					});
				});
				m.AddButton("bringToTop", delegate
				{
					decos.Remove(deco);
					decos.Add(deco);
					selected.transform.SetAsLastSibling();
				});
				m.AddSlider("rotation", (float n) => (n * 45f).ToString() ?? "", deco.rz, delegate(float a)
				{
					deco.rz = (int)a;
					selected.Refresh();
				}, 0f, 7f, isInt: true);
				m.AddSlider("size", (float n) => n.ToString() ?? "", Mathf.Abs(deco.sx), delegate(float a)
				{
					deco.sx = (int)a * ((deco.sx > 0) ? 1 : (-1));
					deco.sy = (int)a * ((deco.sy > 0) ? 1 : (-1));
					selected.Refresh();
				}, 10f, 400f, isInt: true);
				m.AddToggle("shadow", deco.shadow, delegate(bool a)
				{
					deco.shadow = a;
					selected.Refresh();
				});
				m.AddToggle("reverse", deco.reverse, delegate(bool a)
				{
					deco.reverse = a;
					selected.Refresh();
				});
				m.AddToggle("placeBeforeWidget", deco.bottom, delegate(bool a)
				{
					deco.bottom = a;
					selected.Refresh();
				});
				m.AddButton("removeDeco", delegate
				{
					widget.RemoveDeco(selected.owner);
				});
			}
			else
			{
				UIContextMenu uIContextMenu = m.AddChild("addDeco");
				UIList uIList = Util.Instantiate<UIList>("UI/Element/List/ListImageGrid", uIContextMenu);
				uIList.callbacks = new UIList.Callback<Sprite, UIButton>
				{
					onInstantiate = delegate(Sprite a, UIButton _b)
					{
						_b.icon.sprite = a;
					},
					onClick = delegate(Sprite a, UIButton _b)
					{
						SkinDeco skinDeco = new SkinDeco
						{
							sx = 100,
							sy = 100,
							color = Color.white
						};
						skinDeco.id = int.Parse(a.name.Remove(0, 4));
						widget.AddDeco(skinDeco);
						SkinDecoActor actor = skinDeco.actor;
						Activate(actor);
						actor.transform.position = clickPos;
						EInput.Consume();
						m.Hide();
					}
				};
				Sprite[] array = Resources.LoadAll<Sprite>("Media/Graphics/Deco/");
				foreach (Sprite o in array)
				{
					uIList.Add(o);
				}
				uIList.Refresh();
			}
			m.Show();
		}
		if (EInput.rightMouse.down)
		{
			UIContextMenu uIContextMenu2 = ELayer.ui.CreateContextMenu();
			uIContextMenu2.AddButton("quitEdit", delegate
			{
				Close();
			});
			uIContextMenu2.Show();
		}
	}

	public void Activate(SkinDecoActor actor)
	{
		actor.image.raycastTarget = true;
		actor.transform.SetParent(base.transform, worldPositionStays: true);
		UIDragPanel uIDragPanel = actor.gameObject.AddComponent<UIDragPanel>();
		uIDragPanel.target = actor.Rect();
		uIDragPanel.bound = actor.Rect();
		uIDragPanel.clamp = false;
	}

	public override void OnKill()
	{
		widget.transform.SetParent(ELayer.ui.widgets.transform, worldPositionStays: false);
		widget.RefreshOrder();
		foreach (SkinDeco deco in decos)
		{
			SkinDecoActor actor = deco.actor;
			actor.image.raycastTarget = false;
			UnityEngine.Object.DestroyImmediate(actor.gameObject.GetComponent<UIDragPanel>());
			actor.transform.SetParent(widget.transform, worldPositionStays: true);
			deco.x = (int)actor.Rect().anchoredPosition.x;
			deco.y = (int)actor.Rect().anchoredPosition.y;
			actor.Refresh();
		}
	}
}
