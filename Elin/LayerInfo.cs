using UnityEngine;

public class LayerInfo : ELayer
{
	public UICardInfo info;

	public UINote note;

	public bool examine;

	public Vector2 size;

	public Vector2 padding;

	public override void OnAfterInit()
	{
		base.OnAfterInit();
		TooltipManager.Instance.HideTooltips(immediate: true);
	}

	public void SetElement(Element e)
	{
		windows[0].SetCaption(e.Name);
		info.SetElement(e);
	}

	public void Set(object o, bool _examine = false)
	{
		if (o is Thing)
		{
			SetThing(o as Thing, _examine);
		}
	}

	public void Resize()
	{
		windows[0].Rect().sizeDelta = new Vector2(Mathf.Max(info.Rect().sizeDelta.x + padding.x, size.x), size.y);
	}

	public void SetThing(Thing t, bool _examine = false)
	{
		examine = _examine;
		windows[0].SetCaption(t.NameSimple.ToTitleCase());
		info.SetThing(t);
		Resize();
	}

	public void SetBlock(Cell cell)
	{
		windows[0].SetCaption(cell.GetBlockName());
		info.SetBlock(cell);
		Resize();
	}

	public void SetFloor(Cell cell)
	{
		windows[0].SetCaption(cell.GetFloorName());
		info.SetFloor(cell);
		Resize();
	}

	public void SetLiquid(Cell cell)
	{
		windows[0].SetCaption(cell.GetLiquidName());
		info.SetLiquid(cell);
		Resize();
	}

	public void SetZone(Zone z)
	{
		note.Clear();
		note.AddHeader(z.Name);
		note.AddText(z.source.GetDetail());
		note.Build();
		Resize();
	}

	public void SetObj(Cell cell)
	{
		windows[0].SetCaption(cell.sourceObj.GetName());
		info.SetObj(cell);
		Resize();
	}

	public override void OnKill()
	{
		base.OnKill();
		TweenUtil.Tween(0.2f, delegate
		{
			UIButton.TryShowTip<UIButton>();
		});
	}

	public override void OnUpdateInput()
	{
		base.OnUpdateInput();
		if (examine && ELayer.core.config.input.altExamine)
		{
			if (!Input.GetKey(EInput.keys.examine.key))
			{
				Close();
			}
		}
		else if (Input.GetKeyDown(EInput.keys.examine.key))
		{
			Close();
		}
	}
}
