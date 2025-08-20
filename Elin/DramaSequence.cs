using System.Collections.Generic;
using UnityEngine;

public class DramaSequence : EClass
{
	public enum Template
	{
		Default
	}

	public string id;

	public DramaManager manager;

	public Dictionary<string, int> steps = new Dictionary<string, int>();

	public Dictionary<string, DramaActor> actors = new Dictionary<string, DramaActor>();

	public List<DramaEvent> events = new List<DramaEvent>();

	public bool isLoop;

	public bool canCancel = true;

	public bool isExited;

	public bool fullPortrait;

	public DramaEventTalk firstTalk;

	public List<DramaEvent> tempEvents = new List<DramaEvent>();

	private DramaEvent currentEvent;

	public string message = "";

	public string skipJump;

	public string lastStep;

	public string lastlastStep;

	public DramaSetup setup;

	private int currentEventID;

	public DialogDrama dialog => manager.dialog;

	public void Clear()
	{
		steps.Clear();
		actors.Clear();
		events.Clear();
		tempEvents.Clear();
	}

	public DramaActor GetActor(string id)
	{
		if (actors.ContainsKey(id))
		{
			return actors[id];
		}
		if (EClass.sources.persons.map.ContainsKey(id))
		{
			return AddActor(id, new Person(id));
		}
		if (EClass.core.IsGameStarted)
		{
			Chara chara = EClass.game.cards.globalCharas.Find(id) ?? EClass._map.FindChara(id);
			if (chara != null)
			{
				return AddActor(id, new Person(chara));
			}
		}
		if (actors.Count <= 0)
		{
			return GetActor("narrator");
		}
		return actors.FirstItem();
	}

	public T GetEvent<T>(string idStep) where T : DramaEvent
	{
		foreach (DramaEvent @event in events)
		{
			if (@event.step == idStep)
			{
				return @event as T;
			}
		}
		return null;
	}

	public DramaActor AddActor(string id, Person person)
	{
		if (actors.ContainsKey(id))
		{
			return actors[id];
		}
		DramaActor dramaActor = Util.Instantiate(manager.moldActor, manager.actorPos);
		dramaActor.Init(this, id, person);
		actors.Add(id, dramaActor);
		return dramaActor;
	}

	public void AddStep(string id)
	{
		steps.Add(id, events.Count);
		events.Add(new DramaEvent
		{
			sequence = this,
			step = id
		});
	}

	public DramaEvent AddEvent(DramaEvent e)
	{
		if (!e.step.IsEmpty())
		{
			steps.Add(e.step, events.Count);
		}
		e.sequence = this;
		events.Add(e);
		return e;
	}

	public void PlayNext()
	{
		Play(currentEventID + 1);
	}

	public void Play(string id)
	{
		if (id == "last")
		{
			id = lastlastStep;
		}
		if (!id.IsEmpty() && !steps.ContainsKey(id))
		{
			Debug.Log(id);
		}
		Play((!string.IsNullOrEmpty(id)) ? steps[id] : 0);
	}

	public void Play(int eventID = 0)
	{
		if (isExited)
		{
			return;
		}
		if (eventID >= events.Count)
		{
			if (!isLoop)
			{
				Exit();
				return;
			}
			eventID = 0;
		}
		currentEventID = eventID;
		currentEvent = events[eventID];
		currentEvent.Reset();
		string text = eventID + "/";
		foreach (KeyValuePair<string, int> step in steps)
		{
			if (step.Value == eventID)
			{
				text += step.Key;
			}
			if (step.Value == eventID && !step.Key.StartsWith("flag"))
			{
				lastlastStep = lastStep;
				lastStep = step.Key;
				break;
			}
		}
		OnUpdate();
	}

	public void Exit()
	{
		isExited = true;
		currentEvent = null;
		manager.SetActive(enable: false);
	}

	public void OnUpdate()
	{
		if (tempEvents.Count > 0)
		{
			if (tempEvents[0].Play() && tempEvents.Count > 0)
			{
				tempEvents.RemoveAt(0);
			}
		}
		else
		{
			if (currentEvent == null)
			{
				return;
			}
			if (currentEvent is DramaEventGoto)
			{
				string text = currentEvent.idJump;
				if (text == "*")
				{
					if (setup.step.IsEmpty())
					{
						PlayNext();
						return;
					}
					text = setup.step;
				}
				Play(text);
			}
			else if (currentEvent.Play())
			{
				PlayNext();
			}
		}
	}
}
