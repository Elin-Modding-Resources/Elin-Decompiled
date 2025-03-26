using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class BaseCondition : BaseStats
{
	[JsonProperty]
	public int[] _ints = new int[5];

	public int phase = -1;

	public ElementContainer elements;

	public Chara owner;

	public int value
	{
		get
		{
			return _ints[0];
		}
		set
		{
			_ints[0] = value;
		}
	}

	public int power
	{
		get
		{
			return _ints[1];
		}
		set
		{
			_ints[1] = value;
		}
	}

	public int refVal
	{
		get
		{
			return _ints[2];
		}
		set
		{
			_ints[2] = value;
		}
	}

	public int refVal2
	{
		get
		{
			return _ints[3];
		}
		set
		{
			_ints[3] = value;
		}
	}

	public bool givenByPcParty
	{
		get
		{
			return (_ints[4] & 2) != 0;
		}
		set
		{
			_ints[4] = (value ? (_ints[4] | 2) : (_ints[4] & -3));
		}
	}

	public bool isPerfume
	{
		get
		{
			return (_ints[4] & 4) != 0;
		}
		set
		{
			_ints[4] = (value ? (_ints[4] | 4) : (_ints[4] & -5));
		}
	}

	public override Chara Owner => owner;

	public virtual string Name => base.source.GetText();

	public virtual bool IsToggle => false;

	public virtual bool WillOverride => false;

	public virtual bool AllowMultipleInstance => false;

	public virtual bool ConsumeTurn => false;

	public virtual bool HasDuration => base.source.duration != "-1";

	public virtual bool PreventRegen => false;

	public virtual bool ShouldRefresh => false;

	public virtual bool CancelAI => ConsumeTurn;

	public virtual bool TimeBased => false;

	public virtual bool SyncRide => false;

	public virtual bool UseElements => base.source.elements.Length != 0;

	public virtual int GainResistFactor => base.source.gainRes;

	public virtual int P2 => 0;

	public SourceElement.Row sourceElement => EClass.sources.elements.map[refVal];

	public virtual bool IsElemental => false;

	public virtual string RefString1
	{
		get
		{
			if (!IsElemental)
			{
				return "";
			}
			return sourceElement.GetName().ToLower();
		}
	}

	public virtual bool CanManualRemove => false;

	public virtual bool CanStack(Condition c)
	{
		return true;
	}

	public virtual bool TryMove(Point p)
	{
		return true;
	}

	public void SetElement(int id)
	{
		refVal = id;
	}

	public void SetRefVal(int a, int b)
	{
		refVal = a;
		refVal2 = b;
	}

	public virtual Color GetSpriteColor()
	{
		if (!IsElemental)
		{
			return Color.white;
		}
		return EClass.setting.elements[EClass.sources.elements.map[refVal].alias].colorSprite;
	}

	public override string ToString()
	{
		return Name + " " + value + " " + phase;
	}

	public override string GetText()
	{
		if (!IsNullPhase())
		{
			return GetPhaseStr();
		}
		return "";
	}

	public override int GetValue()
	{
		return value;
	}

	public override Color GetColor(Gradient g)
	{
		return g.Evaluate((base.source.phase.LastItem() == 0) ? 0f : ((float)phase / (float)base.source.phase.LastItem()));
	}

	public virtual void SetOwner(Chara _owner, bool onDeserialize = false)
	{
		owner = _owner;
		phase = GetPhase();
		if (EmoIcon > owner.emoIcon)
		{
			owner.emoIcon = EmoIcon;
		}
		if (UseElements)
		{
			elements = new ElementContainer();
			if (base.source.elements.Length != 0)
			{
				for (int i = 0; i < base.source.elements.Length; i += 2)
				{
					elements.SetBase(GetElementSource(i).id, base.source.elements[i + 1].Calc(power, 0, P2));
				}
			}
			elements.SetParent(owner);
		}
		if (onDeserialize)
		{
			OnChangePhase(phase, phase);
		}
	}

	public SourceElement.Row GetElementSource(int i)
	{
		string text = base.source.elements[i];
		if (text == "res")
		{
			text = sourceElement.aliasRef;
		}
		else if (text == "ele")
		{
			text = sourceElement.alias;
		}
		return EClass.sources.elements.alias[text];
	}

	public void Start()
	{
		OnBeforeStart();
		phase = -1;
		SetPhase();
		OnStart();
		OnStartOrStack();
		PlayEffect();
		if (base.source.nullify.Length == 0)
		{
			return;
		}
		for (int num = owner.conditions.Count - 1; num >= 0; num--)
		{
			if (TryNullify(owner.conditions[num]))
			{
				owner.conditions[num].Kill();
			}
		}
	}

	public bool TryNullify(Condition c)
	{
		if (base.source.nullify.Length == 0)
		{
			return false;
		}
		string[] nullify = base.source.nullify;
		foreach (string text in nullify)
		{
			if (c.source.alias == text)
			{
				owner.Say("nullify", owner, Name.ToLower(), c.Name.ToLower());
				return true;
			}
		}
		return false;
	}

	public virtual void OnBeforeStart()
	{
	}

	public virtual void OnStart()
	{
	}

	public virtual void OnStartOrStack()
	{
	}

	public virtual void PlayEffect()
	{
		if (!Condition.ignoreEffect && base.source.effect.Length != 0)
		{
			if (!base.source.effect[0].IsEmpty())
			{
				owner.PlayEffect(base.source.effect[0]);
			}
			owner.PlaySound((base.source.effect.Length >= 2) ? base.source.effect[1] : base.source.effect[0]);
		}
	}

	public virtual void PlayEndEffect()
	{
		if (base.source.effect.Length >= 3)
		{
			if (!base.source.effect[2].IsEmpty())
			{
				owner.PlayEffect(base.source.effect[2]);
			}
			owner.PlaySound((base.source.effect.Length >= 4) ? base.source.effect[3] : base.source.effect[2]);
		}
	}

	public virtual void OnRefresh()
	{
	}

	public void SetPhase()
	{
		int num = GetPhase();
		if (phase != num)
		{
			int num2 = phase;
			phase = num;
			PhaseMsg(num > num2);
			OnChangePhase(num2, num);
		}
	}

	public bool IsNullPhase()
	{
		if (base.source.strPhase.Length == 0)
		{
			return false;
		}
		return base.source.strPhase[GetPhase()] == "#";
	}

	public override int GetPhase()
	{
		return base.source.phase[Mathf.Clamp(value, 0, 99) / 10];
	}

	public void PhaseMsg(bool inc)
	{
		bool flag = (base.source.invert ? (!inc) : inc);
		string[] array = (inc ? base.source.GetText("textPhase") : base.source.GetText("textPhase2")).Split(Environment.NewLine.ToCharArray());
		if (array.Length > phase && !array[phase].IsEmpty())
		{
			if (Type == ConditionType.Stance)
			{
				Msg.SetColor("ono");
			}
			else if (!base.source.invert && flag)
			{
				Msg.SetColor("negative");
			}
			else if (base.source.invert && !flag)
			{
				Msg.SetColor("positive");
			}
			PopText();
			owner.Say(array[phase].Split('|').RandomItem(), owner, RefString1);
		}
	}

	public override string GetPhaseStr()
	{
		string[] textArray = base.source.GetTextArray("strPhase");
		if (textArray.Length == 0)
		{
			return Name;
		}
		return textArray[phase].IsEmpty("");
	}

	public virtual void Tick()
	{
	}

	public void Mod(int a, bool force = false)
	{
		if ((!isPerfume || force) && value != 0)
		{
			value += a;
			SetPhase();
			OnValueChanged();
		}
	}

	public virtual void OnValueChanged()
	{
	}

	public virtual void OnChangePhase(int lastPhase, int newPhase)
	{
	}

	public virtual void OnRemoved()
	{
	}

	public virtual int EvaluatePower(int p)
	{
		return p;
	}

	public virtual int EvaluateTurn(int p)
	{
		return Mathf.Max(1, base.source.duration.Calc(p, 0, P2));
	}

	public virtual BaseNotification CreateNotification()
	{
		return new BaseNotification();
	}

	public Element GetDefenseAttribute(Chara c = null)
	{
		if (base.source.defenseAttb.IsEmpty())
		{
			return null;
		}
		if (c == null)
		{
			return Element.Create(base.source.defenseAttb[0]);
		}
		return c.elements.GetOrCreateElement(base.source.defenseAttb[0]);
	}

	public override void _WriteNote(UINote n, bool asChild = false)
	{
		List<string> list = new List<string>();
		Element defenseAttribute = GetDefenseAttribute();
		if (defenseAttribute != null)
		{
			list.Add("hintDefenseAttb".lang(defenseAttribute.Name.ToTitleCase()));
		}
		OnWriteNote(list);
		string[] nullify = base.source.nullify;
		foreach (string key in nullify)
		{
			list.Add("hintNullify".lang(EClass.sources.stats.alias[key].GetName()));
		}
		for (int j = 0; j < base.source.elements.Length; j += 2)
		{
			Element element = Element.Create(GetElementSource(j).id, base.source.elements[j + 1].Calc(power, 0, P2));
			if (elements != null && elements.Has(element.id) && elements.Value(element.id) != element.Value)
			{
				list.Add("modValue".lang(element.Name, ((element.Value < 0) ? "" : "+") + elements.Value(element.id) + " (" + element.Value + ")"));
			}
			else
			{
				list.Add("modValue".lang(element.Name, ((element.Value < 0) ? "" : "+") + element.Value));
			}
		}
		if (list.Count <= 0)
		{
			return;
		}
		if (!asChild)
		{
			n.Space(8);
		}
		foreach (string item in list)
		{
			n.AddText("_bullet".lang() + item);
		}
	}

	public virtual void OnWriteNote(List<string> list)
	{
	}

	public virtual RendererReplacer GetRendererReplacer()
	{
		return null;
	}
}
