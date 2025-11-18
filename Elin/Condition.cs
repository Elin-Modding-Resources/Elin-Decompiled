using System;

public class Condition : BaseCondition
{
	public static bool ignoreEffect;

	public virtual bool IsKilled => base.value <= 0;

	public virtual string TextDuration
	{
		get
		{
			object obj;
			if (!base.isPerfume)
			{
				obj = base.value.ToString();
				if (obj == null)
				{
					return "";
				}
			}
			else
			{
				obj = "";
			}
			return (string)obj;
		}
	}

	public override BaseNotification CreateNotification()
	{
		return new NotificationCondition
		{
			condition = this
		};
	}

	public static T Create<T>(int power = 100, Action<T> onCreate = null) where T : Condition
	{
		return (T)Create(typeof(T).Name, power, delegate(Condition c)
		{
			onCreate(c as T);
		});
	}

	public static Condition Create(string alias, int power = 100, Action<Condition> onCreate = null)
	{
		SourceStat.Row row = EClass.sources.stats.alias[alias];
		Condition condition = ClassCache.Create<Condition>(row.type.IsEmpty(alias), "Elin");
		condition.power = power;
		condition.id = row.id;
		condition._source = row;
		onCreate?.Invoke(condition);
		return condition;
	}

	public virtual void OnStacked(int p)
	{
		base.value += EvaluateTurn(p);
		SetPhase();
	}

	public Condition SetPerfume(int duration = 3)
	{
		base.isPerfume = true;
		base.value = duration;
		return this;
	}

	public override void Tick()
	{
		Mod(-1);
	}

	public virtual void OnCalculateFov(Fov fov, ref int radius, ref float power)
	{
	}

	public virtual void OnCreateFov(Fov fov)
	{
	}

	public override void OnValueChanged()
	{
		if (base.value <= 0)
		{
			Kill();
		}
	}

	public void Kill(bool silent = false)
	{
		base.value = 0;
		owner.conditions.Remove(this);
		if (!silent && !owner.isDead && !base.source.textEnd.IsEmpty())
		{
			owner.Say(base.source.GetText("textEnd"), owner, RefString1);
		}
		PlayEndEffect();
		if (elements != null)
		{
			elements.SetParent();
		}
		OnRemoved();
		owner.SetDirtySpeed();
		if (ShouldRefresh)
		{
			owner.Refresh();
		}
	}
}
