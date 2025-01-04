using System;

public class TraitBoat : Trait
{
	public override bool IsFloating => true;

	public override bool IsGround => true;

	public override void OnChangePlaceState(PlaceState state)
	{
		if (state == PlaceState.installed)
		{
			ForeachDeck(delegate(Point p)
			{
				p.SetFloor(1, 57);
				p.cell.floorDir = ((owner.dir % 2 != 0) ? 1 : 0);
			});
		}
	}

	public override void OnRemovedFromZone()
	{
		if (owner.placeState == PlaceState.installed)
		{
			SourceMaterial.Row mat = GetWaterMat();
			ForeachDeck(delegate(Point p)
			{
				p.SetFloor(mat.id, 43);
			});
		}
	}

	public void ForeachDeck(Action<Point> action)
	{
		int num = 1 + ((owner.dir % 2 == 1 && owner.id != "boat3") ? 1 : 0);
		int num2 = 1 + ((owner.dir % 2 == 0 && owner.id != "boat3") ? 1 : 0);
		int x = owner.pos.x - owner.W + num;
		int z = owner.pos.z + num2;
		int mx = owner.pos.x - num;
		int mz = owner.pos.z + owner.H - num2;
		owner.pos.ForeachMultiSize(owner.W, owner.H, delegate(Point p, bool main)
		{
			if (p.x > x && p.z >= z && p.x <= mx && p.z < mz)
			{
				action(p);
			}
		});
	}

	public SourceMaterial.Row GetWaterMat()
	{
		int mat = 67;
		owner.ForeachPoint(delegate(Point p, bool main)
		{
			if (p.cell.IsFloorWater)
			{
				mat = p.matFloor.id;
			}
		});
		return EClass.sources.materials.rows[mat];
	}
}
