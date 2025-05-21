using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ACT : EClass
{
	public new static ActWait Wait = new ActWait();

	public static ActChat Chat = new ActChat();

	public static ActPick Pick = new ActPick();

	public static ActKick Kick = new ActKick();

	public static ActMelee Melee = new ActMelee();

	public static ActRanged Ranged = new ActRanged();

	public static ActThrow Throw = new ActThrow();

	public static ActItem Item = new ActItem();

	public static Dictionary<string, Act> dict = new Dictionary<string, Act>();

	public static void Init()
	{
		foreach (SourceElement.Row row in EClass.sources.elements.rows)
		{
			if (row.group == "ABILITY" || row.group == "SPELL")
			{
				dict[row.alias] = Create(row);
			}
		}
	}

	public static Act Create(int id)
	{
		return Create(EClass.sources.elements.map.TryGetValue(id, 6003));
	}

	public static Act Create(string id)
	{
		return Create(EClass.sources.elements.alias[id]);
	}

	public static Act Create(SourceElement.Row row)
	{
		if (row == null)
		{
			row = EClass.sources.elements.alias["AI_SelfHarm"];
		}
		Act act = ClassCache.Create<Act>(row.type.IsEmpty(row.alias), "Elin") ?? new Act();
		if (act != null)
		{
			act.id = row.id;
			act._source = row;
		}
		return act;
	}
}
public class Act : Element
{
	public enum CostType
	{
		None,
		MP,
		SP
	}

	public struct Cost
	{
		public int cost;

		public CostType type;
	}

	public static Chara CC;

	public static Card TC;

	public static Point TP = new Point();

	public static Thing TOOL;

	public static int powerMod = 100;

	public static bool forcePt;

	public static Act CurrentAct;

	public override bool ShowPotential => false;

	public override bool UsePotential => false;

	public override bool ShowRelativeAttribute => true;

	public virtual CursorInfo CursorIcon => CursorSystem.Action;

	public virtual string ID => base.source.alias;

	public virtual bool IsAct => true;

	public virtual bool WillEndTurn => true;

	public virtual bool CloseLayers => true;

	public virtual int LeftHand => 0;

	public virtual int RightHand => 0;

	public virtual int ElementPowerMod => base.source.eleP;

	public virtual bool ShowMapHighlight => false;

	public virtual bool ShowMapHighlightBlock => false;

	public virtual bool PickHeldOnStart => false;

	public virtual bool DropHeldOnStart => false;

	public virtual bool CanPressRepeat => false;

	public virtual bool CanAutofire => false;

	public virtual bool ResetAxis => false;

	public virtual bool RequireTool => false;

	public virtual bool IsHostileAct => false;

	public virtual bool HideRightInfo => false;

	public virtual bool HaveLongPressAction => TargetType.CanSelectParty;

	public virtual float Radius => base.source.radius;

	public virtual int PerformDistance => 1;

	public virtual int MaxRadius => 99;

	public virtual TargetType TargetType
	{
		get
		{
			if (!base.source.target.IsEmpty())
			{
				switch (base.source.target)
				{
				case "Enemy":
					return TargetType.Enemy;
				case "Chara":
					return TargetType.Chara;
				case "Ground":
					return TargetType.Ground;
				case "Neighbor":
					return TargetType.SelfAndNeighbor;
				case "Self":
					return TargetType.Self;
				case "Select":
					return TargetType.Select;
				case "Party":
					return TargetType.Party;
				case "SelfParty":
					return TargetType.SelfParty;
				}
			}
			return TargetType.Any;
		}
	}

	public virtual bool LocalAct => true;

	public virtual bool CanRapidFire => false;

	public virtual float RapidDelay => 0.2f;

	public virtual bool ShowAuto => false;

	public virtual bool IsCrime => EClass._zone.IsCrime(EClass.pc, this);

	public virtual CursorInfo GetCursorIcon(Card c)
	{
		return CursorIcon;
	}

	public virtual MultiSprite GetStateIcon()
	{
		return null;
	}

	public override Sprite GetSprite()
	{
		return base.source.GetSprite();
	}

	public virtual bool ShowMouseHint(Card c)
	{
		return true;
	}

	public virtual bool IsValidTC(Card c)
	{
		if (!c.isChara)
		{
			return c.trait.CanBeAttacked;
		}
		return true;
	}

	public virtual string GetHintText(string str = "")
	{
		return GetText(str);
	}

	public virtual string GetText(string str = "")
	{
		if (base.source != null)
		{
			id = base.source.id;
		}
		if (id == 0)
		{
			if (!Lang.Has(ToString()))
			{
				return ToString() + "/" + base.source?.ToString() + "/" + id;
			}
			return Lang.Get(ToString());
		}
		return Lang.ParseRaw(base.source.GetName(), str);
	}

	public virtual string GetTextSmall(Card c)
	{
		if (c == null)
		{
			return null;
		}
		return c.Name + c.GetExtraName();
	}

	public bool IsTargetHostileParty()
	{
		if (base.source.abilityType.Length != 0)
		{
			switch (base.source.abilityType[0])
			{
			case "debuff":
			case "debuffStats":
			case "attack":
			case "dot":
				return true;
			}
		}
		return false;
	}

	public virtual Color GetActPlanColor()
	{
		if (!IsCrime)
		{
			return EClass.Colors.colorAct;
		}
		return EClass.Colors.colorHostileAct;
	}

	public virtual bool Perform()
	{
		int num = id;
		if ((num == 8230 || num == 8232) && TC.isThing)
		{
			int power = CC.elements.GetOrCreateElement(base.source.id).GetPower(CC);
			ActEffect.Proc(base.source.proc[0].ToEnum<EffectId>(), power, BlessedState.Normal, CC, TC);
			return true;
		}
		if (base.source.proc.Length != 0)
		{
			string text = base.source.aliasRef.IsEmpty(CC.MainElement.source.alias);
			string text2 = base.source.proc[0];
			if (text2 == "LulwyTrick" || text2 == "BuffStats")
			{
				text = base.source.proc[1];
			}
			else if (text == "mold")
			{
				text = CC.MainElement.source.alias;
			}
			if (TargetType.Range == TargetRange.Self && !forcePt)
			{
				TC = CC;
				TP.Set(CC.pos);
			}
			int power2 = CC.elements.GetOrCreateElement(base.source.id).GetPower(CC);
			ActEffect.ProcAt(base.source.proc[0].ToEnum<EffectId>(), power2, BlessedState.Normal, CC, TC, TP, base.source.tag.Contains("neg"), new ActRef
			{
				n1 = base.source.proc.TryGet(1, returnNull: true),
				aliasEle = text,
				act = this
			});
		}
		return true;
	}

	public virtual bool CanPerform()
	{
		return true;
	}

	public bool Perform(Chara _cc, Card _tc = null, Point _tp = null)
	{
		if (CanPerform(_cc, _tc, _tp))
		{
			return Perform();
		}
		return false;
	}

	public bool CanPerform(Chara _cc, Card _tc = null, Point _tp = null)
	{
		CC = _cc;
		TC = _tc;
		TP.Set(_tp ?? TC?.pos ?? CC.pos);
		TargetType tt = TargetType;
		if (forcePt && tt.Range == TargetRange.Self && !CC.IsPC)
		{
			tt = TargetType.Chara;
		}
		if (LocalAct && EClass._zone.IsRegion)
		{
			return false;
		}
		if (tt.Range == TargetRange.Self)
		{
			TP.Set(CC.pos);
			return CanPerform();
		}
		if (CC.IsPC && EClass.core.config.game.shiftToUseNegativeAbilityOnSelf)
		{
			Chara firstChara = TP.FirstChara;
			if (firstChara != null && firstChara.IsPCFactionOrMinion)
			{
				bool flag = base.source.tag.Contains("neg");
				if (base.source.id != 6011 && flag && !EInput.isShiftDown)
				{
					return false;
				}
			}
		}
		if (!(this is ActMelee) && tt.Range == TargetRange.Chara && (TC == null || !CC.CanSee(TC)))
		{
			return false;
		}
		bool distCheck = DistCheck(CC.pos, TP);
		if (!distCheck && (CC.IsMultisize || (TC != null && TC.IsMultisize)))
		{
			if (CC.IsMultisize)
			{
				CC.ForeachPoint(delegate(Point p, bool main)
				{
					DistCheckMulti(p, TC);
				});
			}
			else
			{
				DistCheckMulti(CC.pos, TC);
			}
		}
		if (!distCheck)
		{
			return false;
		}
		return CanPerform();
		bool DistCheck(Point p1, Point p2)
		{
			int num = p1.Distance(p2);
			if (tt.RequireLos)
			{
				if (CC.IsPC)
				{
					if (!p2.IsSync)
					{
						return false;
					}
				}
				else if ((num != 1 || !CC.IsMultisize) && !Los.IsVisible(p1, p2))
				{
					return false;
				}
			}
			if (num >= tt.LimitDist)
			{
				return false;
			}
			if (num >= MaxRadius)
			{
				return false;
			}
			if (tt.RequireChara && !p2.HasChara)
			{
				return false;
			}
			if (tt.CanOnlyTargetEnemy)
			{
				foreach (Chara chara in p2.Charas)
				{
					if (CC.IsHostile(chara))
					{
						return true;
					}
				}
				return false;
			}
			return true;
		}
		void DistCheckMulti(Point p1, Card c)
		{
			if (c == null && !distCheck)
			{
				distCheck = DistCheck(p1, TP);
			}
			else
			{
				c.ForeachPoint(delegate(Point p, bool main)
				{
					if (!distCheck)
					{
						distCheck = DistCheck(p1, p);
					}
				});
			}
		}
	}

	public static void SetReference(Chara _cc, Card _tc = null, Point _tp = null)
	{
		CC = _cc;
		TC = _tc;
		TP.Set(_tp ?? TC.pos);
	}

	public virtual bool IsToolValid()
	{
		if (CC == EClass.pc && TOOL != null)
		{
			return TOOL.parent == EClass.pc;
		}
		return false;
	}

	public static void SetTool(Thing t)
	{
		TOOL = t;
	}

	public new void SetImage(Image image)
	{
		image.sprite = GetSprite() ?? EClass.core.refs.icons.defaultAbility;
		if (!base.source.aliasRef.IsEmpty())
		{
			image.color = EClass.setting.elements[base.source.aliasRef].colorSprite;
		}
		else
		{
			image.color = Color.white;
		}
		image.rectTransform.pivot = new Vector2(0.5f, 0.5f);
		image.SetNativeSize();
	}

	public virtual void OnMarkMapHighlights()
	{
		EClass._map.ForeachSphere(EClass.pc.pos.x, EClass.pc.pos.z, base.act.MaxRadius, delegate(Point p)
		{
			if (!p.HasBlock && ShouldMapHighlight(p))
			{
				p.SetHighlight(8);
			}
		});
	}

	public virtual bool ShouldMapHighlight(Point p)
	{
		if (base.act.TargetType.ShowMapHighlight && base.act.ShowMapHighlight)
		{
			return base.act.CanPerform(EClass.pc, null, p);
		}
		return false;
	}
}
