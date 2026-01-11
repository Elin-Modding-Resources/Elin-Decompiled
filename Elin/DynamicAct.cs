using System;

public class DynamicAct : Act
{
	public static DynamicAct lastAct;

	public new string id;

	public Func<bool> onPerform;

	public Func<bool> canRepeat;

	public bool closeLayers;

	public bool isHostileAct;

	public bool localAct = true;

	public CursorInfo cursor;

	public int dist = 1;

	public TargetType _targetType = TargetType.Any;

	public override string ID => id;

	public override TargetType TargetType => _targetType;

	public override bool IsHostileAct => isHostileAct;

	public override CursorInfo CursorIcon => cursor;

	public override bool ShouldEndMimicry => false;

	public override bool CloseLayers => closeLayers;

	public override int PerformDistance => dist;

	public override bool CanPressRepeat
	{
		get
		{
			if (canRepeat == null)
			{
				return false;
			}
			return canRepeat();
		}
	}

	public override bool LocalAct
	{
		get
		{
			if (!(id == "actNewZone"))
			{
				return localAct;
			}
			return false;
		}
	}

	public override string GetText(string str = "")
	{
		return Lang.Get(id);
	}

	public DynamicAct(string _id, Func<bool> _onPerform = null, bool _closeLayers = false)
	{
		id = _id;
		onPerform = _onPerform;
		closeLayers = _closeLayers;
		lastAct = this;
	}

	public override bool Perform()
	{
		if (onPerform != null)
		{
			return onPerform();
		}
		return false;
	}
}
