using Newtonsoft.Json;

public class ZoneEvent : EClass
{
	[JsonProperty]
	public float time;

	[JsonProperty]
	public int rounds;

	[JsonProperty]
	public int hoursElapsed;

	[JsonProperty]
	public int minElapsed;

	public Zone zone;

	public bool firstTick = true;

	public SourceQuest.Row source => EClass.sources.quests.map[id];

	public virtual string id => "";

	public virtual string TextWidgetDate => "";

	public virtual float roundInterval => 1f;

	public virtual Playlist playlist => null;

	public virtual bool debugSkip => false;

	public virtual string RefStr2 => null;

	public virtual string RefStr3 => null;

	public virtual int hoursToKill => 0;

	public virtual bool HasReport => false;

	public string Name => source.GetName();

	public virtual string GetText()
	{
		return Lang.ParseRaw(source.GetDetail().Split('|')[1], "", RefStr2, RefStr3);
	}

	public void Tick(float delta)
	{
		if (firstTick)
		{
			firstTick = false;
			OnFirstTick();
		}
		OnTick();
		time += delta;
		if (time > roundInterval * (float)rounds)
		{
			rounds++;
			OnTickRound();
		}
	}

	public void Init()
	{
		OnInit();
	}

	public void OnSimulateHour()
	{
		hoursElapsed++;
		if (hoursToKill != 0 && hoursElapsed >= hoursToKill)
		{
			Kill();
		}
	}

	public virtual void OnVisit()
	{
	}

	public virtual void OnInit()
	{
	}

	public virtual void OnFirstTick()
	{
	}

	public virtual void OnTick()
	{
	}

	public virtual void OnTickRound()
	{
	}

	public virtual void OnKill()
	{
	}

	public virtual void OnCharaDie(Chara c)
	{
	}

	public virtual void OnLeaveZone()
	{
	}

	public void OnLoad(Zone _zone)
	{
		zone = _zone;
	}

	public void Kill()
	{
		if (zone == null)
		{
			zone = EClass._zone;
		}
		zone.events.list.Remove(this);
		OnKill();
	}
}
