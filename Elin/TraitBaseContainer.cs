using UnityEngine;

public class TraitBaseContainer : Trait
{
	public bool HasChara => owner.c_idRefCard != null;

	public override bool ShowChildrenNumber
	{
		get
		{
			if (!HasChara)
			{
				return base.ShowChildrenNumber;
			}
			return false;
		}
	}

	public override string IDInvStyle
	{
		get
		{
			if (GetParam(3) != null)
			{
				return GetParam(3);
			}
			return DefaultIdInvStyle;
		}
	}

	public override RefCardName RefCardName => RefCardName.None;

	public string idContainer
	{
		get
		{
			if (GetParam(4) != null)
			{
				return GetParam(4);
			}
			return DefaultIdContainer;
		}
	}

	public int Width
	{
		get
		{
			if (owner.sourceCard.trait.Length <= 1)
			{
				return DefaultWidth;
			}
			return GetParam(1).ToInt();
		}
	}

	public int Height
	{
		get
		{
			if (owner.sourceCard.trait.Length <= 2)
			{
				return DefaultHeight;
			}
			return GetParam(2).ToInt();
		}
	}

	public virtual string DefaultIdInvStyle => base.IDInvStyle;

	public virtual string DefaultIdContainer => "default";

	public virtual int DefaultWidth => 6;

	public virtual int DefaultHeight => 2;

	public virtual int ChanceLock => 0;

	public virtual int ChanceMedal => 50;

	public override int DecaySpeedChild => 50;

	public override bool IsContainer => true;

	public virtual bool ShowOpenActAsCrime => owner.isNPCProperty;

	public override bool UseAltTiles
	{
		get
		{
			if (owner.things.Count == 0)
			{
				return owner.c_lockLv == 0;
			}
			return false;
		}
	}

	public override void OnCreate(int lv)
	{
		owner.things.SetSize(Width, Height);
		if (ChanceLock > 0 && ChanceLock > EClass.rnd(100))
		{
			lv += 10;
			owner.c_lockLv = EClass.curve(5 + lv / 2 + EClass.rnd(lv / 2), 50, 10, 80);
		}
		Prespawn(lv);
	}

	public virtual void Prespawn(int lv)
	{
		if (!CanOpenContainer)
		{
			return;
		}
		int num = 1 + EClass.rnd(2);
		int num2 = EClass.curve(lv, 20, 15);
		for (int i = 0; i < num; i++)
		{
			Thing thing = null;
			if (EClass.sources.spawnLists.map.ContainsKey(idContainer))
			{
				thing = ThingGen.CreateFromFilter(idContainer);
			}
			else if (EClass.sources.categories.map.ContainsKey(idContainer))
			{
				thing = ThingGen.CreateFromCategory(idContainer);
			}
			else
			{
				string text = idContainer;
				thing = ((!(text == "money")) ? ((!(text == "provision")) ? ((!EClass.sources.things.map.ContainsKey(idContainer)) ? ThingGen.CreateFromFilter("container_general", (lv + owner.c_lockLv > 0) ? 5 : 0) : ThingGen.Create(idContainer)) : ThingGen.CreateFromCategory((EClass.rnd(2) == 0) ? "preserved" : "drink")) : ((EClass.rnd(2) == 0) ? ThingGen.Create("money").SetNum(10 + EClass.rnd(50 + num2 * 25)) : ((EClass.rnd(2) != 0) ? ThingGen.Create("plat") : ThingGen.Create("money2").SetNum(1 + EClass.rnd(Mathf.Min(2 + num2 / 50, 5))))));
			}
			if (thing != null)
			{
				owner.AddCard(thing);
			}
		}
		if (EClass.rnd(ChanceMedal) == 0)
		{
			owner.Add("medal");
		}
	}

	public void PutChara(string id)
	{
		owner.c_idRefCard = id;
		owner.AddCard(ThingGen.Create("junk"));
	}

	public override void SetName(ref string s)
	{
		if (owner.Thing.IsSharedContainer)
		{
			s = "_shared".lang(s);
		}
		if (!owner.c_idRefName.IsEmpty() && owner.c_altName.IsEmpty())
		{
			s = "_written".lang(owner.c_idRefName, s);
		}
	}
}
