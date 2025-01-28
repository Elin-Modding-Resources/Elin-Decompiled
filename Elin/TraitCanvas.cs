using UnityEngine;
using UnityEngine.UI;

public class TraitCanvas : Trait
{
	public override bool CanName => true;

	public virtual bool PointFilter => false;

	public virtual float Scale => 1f;

	public virtual TraitPainter.Type CanvasType => TraitPainter.Type.Paint;

	public override void OnSetCardGrid(ButtonGrid b)
	{
		if (owner.c_textureData != null)
		{
			Sprite paintSprite = owner.GetPaintSprite();
			b.Attach<Image>("canvas", rightAttach: false).sprite = paintSprite;
		}
	}

	public override void TrySetAct(ActPlan p)
	{
		if (EClass._zone.IsUserZone && owner.Num == 1 && owner.c_textureData != null && owner.IsInstalled)
		{
			p.TrySetAct("actTakeOut", delegate
			{
				owner.noSell = true;
				owner.isNPCProperty = false;
				EClass.pc.PickOrDrop(owner.pos, owner.Thing);
				return true;
			}, owner);
		}
	}
}
