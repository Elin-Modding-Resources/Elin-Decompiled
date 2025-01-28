using UnityEngine;

public class AM_Paint : AM_BaseTileSelect
{
	public SpriteRenderer srRect;

	public TraitPainter painter;

	private bool completed;

	public override BaseTileSelector.SelectType selectType => BaseTileSelector.SelectType.None;

	public override bool ShowActionHint => false;

	public override bool ShowMouseoverTarget => false;

	public override bool enableMouseInfo => false;

	public override bool ShowMaskedThings => false;

	public override void OnActivate()
	{
		srRect = Util.Instantiate<SpriteRenderer>("Media/Graphics/paintRect");
		completed = false;
		Msg.Say((painter.PaintType == TraitPainter.Type.Camera) ? "askPhoto" : "askPaint");
	}

	public override void OnDeactivate()
	{
		Object.Destroy(srRect.gameObject);
	}

	public void SetPainter(TraitPainter p)
	{
		painter = p;
		Activate();
	}

	public override void OnUpdateInput()
	{
		if (completed)
		{
			return;
		}
		bool flag = true;
		TraitCanvas canvas = painter.GetCanvas();
		if (canvas == null || canvas.owner.GetRootCard() != EClass.pc)
		{
			Deactivate();
			return;
		}
		if (EClass.ui.isPointerOverUI)
		{
			flag = false;
		}
		srRect.color = (flag ? Color.green : Color.red);
		Vector3 mousePosition = Input.mousePosition;
		srRect.transform.position = Camera.main.ScreenToWorldPoint(mousePosition).SetZ(-100f);
		float num = 0.02f / EClass.screen.Zoom;
		Sprite sprite = canvas.owner.GetSprite();
		srRect.transform.localScale = new Vector3((float)sprite.texture.width * num, (float)sprite.texture.height * num, 1f);
		if (!EInput.leftMouse.clicked)
		{
			return;
		}
		if (!flag)
		{
			SE.Beep();
			return;
		}
		srRect.enabled = false;
		completed = true;
		EClass.core.actionsNextFrame.Add(delegate
		{
			EClass.core.actionsNextFrame.Add(delegate
			{
				ActionMode.DefaultMode.Activate();
				EClass.pc.SetAI(new AI_Paint
				{
					painter = painter,
					canvas = canvas,
					data = canvas.owner.GetPaintData()
				});
			});
		});
	}
}
