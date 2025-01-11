public class TraitDrawingPaper : TraitCanvas
{
	public override bool PointFilter => true;

	public override float Scale => 2f;

	public override TraitPainter.Type CanvasType => TraitPainter.Type.Paper;

	public override void TrySetAct(ActPlan p)
	{
		base.TrySetAct(p);
		if (p.altAction)
		{
			p.TrySetAct("actPaint", delegate
			{
				EClass.ui.AddLayer<LayerPixelPaint>().SetCanvas(this);
				return false;
			}, owner);
		}
	}
}
