using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class ZoneEventManager : EClass
{
	public Zone zone;

	[JsonProperty]
	public List<ZoneEvent> list = new List<ZoneEvent>();

	[JsonProperty]
	public List<ZonePreEnterEvent> listPreEnter = new List<ZonePreEnterEvent>();

	public void OnLoad(Zone _zone)
	{
		zone = _zone;
		foreach (ZoneEvent item in list)
		{
			item.zone = zone;
			item.OnLoad();
		}
	}

	public void Add<T>(bool allowDuplicate = false) where T : ZoneEvent
	{
		Add(Activator.CreateInstance<T>(), allowDuplicate);
	}

	public void Add(ZoneEvent e, bool allowDuplicate = false)
	{
		if (EClass.debug.skipEvent && e.debugSkip)
		{
			return;
		}
		if (!allowDuplicate)
		{
			foreach (ZoneEvent item in list)
			{
				if (e.GetType() == item.GetType())
				{
					return;
				}
			}
		}
		list.Add(e);
		e.zone = zone;
		e.Init();
		Debug.Log("#game zone event " + e.GetType()?.ToString() + " added.");
	}

	public void AddPreEnter(ZonePreEnterEvent e, bool executeIfActiveZone = true)
	{
		if (zone.IsActiveZone && executeIfActiveZone)
		{
			e.Execute();
		}
		else
		{
			listPreEnter.Add(e);
		}
	}

	public T GetEvent<T>() where T : ZoneEvent
	{
		foreach (ZoneEvent item in list)
		{
			if (item is T)
			{
				return item as T;
			}
		}
		return null;
	}

	public void Remove<T>() where T : ZoneEvent
	{
		for (int num = list.Count - 1; num >= 0; num--)
		{
			if (list[num] is T)
			{
				list[num].Kill();
			}
		}
	}

	public void Remove(ZoneEvent e)
	{
		list.Remove(e);
	}

	public void Clear()
	{
		list.Clear();
	}

	public void Tick(float delta)
	{
		list.ForeachReverse(delegate(ZoneEvent e)
		{
			e.Tick(delta);
		});
	}

	public void OnVisit()
	{
		foreach (ZoneEvent item in list)
		{
			item.OnVisit();
		}
	}

	public void OnLeaveZone()
	{
		foreach (ZoneEvent item in list)
		{
			item.OnLeaveZone();
		}
	}

	public void OnCharaDie(Chara c)
	{
		list.ForeachReverse(delegate(ZoneEvent e)
		{
			e.OnCharaDie(c);
		});
	}

	public void OnSimulateHour()
	{
		if (list.Count == 0)
		{
			return;
		}
		foreach (ZoneEvent item in list.Copy())
		{
			item.OnSimulateHour();
		}
	}
}
