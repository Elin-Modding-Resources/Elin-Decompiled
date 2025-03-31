using UnityEngine;

public class TraitScroll : Trait
{
	public virtual SourceElement.Row source => null;

	public virtual int eleParent
	{
		get
		{
			if (source == null || source.aliasParent.IsEmpty())
			{
				return 76;
			}
			return EClass.sources.elements.alias[source.aliasParent].id;
		}
	}

	public virtual float MTPValue => 1f;

	public virtual bool UseSourceValue => true;

	public override bool IsNoShop
	{
		get
		{
			if (source != null)
			{
				return source.tag.Contains("noShop");
			}
			return false;
		}
	}

	public override SourceElement.Row GetRefElement()
	{
		return source;
	}

	public override bool CanRead(Chara c)
	{
		if (c.IsPC && !c.isBlind)
		{
			if (EClass._zone.IsUserZone)
			{
				return !owner.isNPCProperty;
			}
			return true;
		}
		return false;
	}

	public override int GetValue()
	{
		if (source != null && source.value != 0)
		{
			return (int)Mathf.Max(MTPValue * (float)source.value, 1f);
		}
		return base.GetValue();
	}
}
