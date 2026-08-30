using System.Collections.Generic;
using Newtonsoft.Json;

public class PolicyManager : EClass
{
	[JsonProperty]
	public List<Policy> list = new List<Policy>();

	public FactionBranch owner;

	public void SetOwner(FactionBranch _owner)
	{
		owner = _owner;
		foreach (Policy item in list)
		{
			item.SetOwner(owner);
		}
		RefreshEffects();
	}

	public void AddPolicy(string id)
	{
		AddPolicy(EClass.sources.elements.alias[id].id);
	}

	public Policy AddPolicy(int id, bool show = true)
	{
		if (owner.elements.GetElement(id) == null)
		{
			owner.elements.SetBase(id, 1);
		}
		Policy policy = new Policy
		{
			id = id
		};
		policy.SetOwner(owner);
		list.Add(policy);
		if (show)
		{
			WidgetPopText.Say("rewardPolicy".lang(EClass.sources.elements.map[id].GetName()));
		}
		if (policy.source.tag.Contains("globalPolicy"))
		{
			EClass.pc.faction.AddGlobalPolicy(policy.id);
		}
		return policy;
	}

	public void Activate(int id)
	{
		foreach (Policy item in list)
		{
			if (item.id == id)
			{
				item.active = true;
			}
		}
	}

	public void SetActive(int id, bool active)
	{
		foreach (Policy item in list)
		{
			if (item.id == id)
			{
				item.active = active;
			}
		}
	}

	public bool IsActive(int id, int days = -1)
	{
		foreach (Policy item in list)
		{
			if (item.active && item.id == id && (days == -1 || item.days >= days))
			{
				return true;
			}
		}
		return false;
	}

	public bool HasPolicy(int id)
	{
		foreach (Policy item in list)
		{
			if (item.id == id)
			{
				return true;
			}
		}
		return false;
	}

	public void OnSimulateHour(VirtualDate date)
	{
		foreach (Policy item in list)
		{
			if (item.active)
			{
				item.OnAdvanceHour(date);
				if (date.hour == 0)
				{
					item.days++;
				}
			}
			else
			{
				item.days = 0;
			}
		}
	}

	public int GetValue(int id)
	{
		foreach (Policy item in list)
		{
			if (item.id == id && item.active)
			{
				return item.Ele?.Value ?? 0;
			}
		}
		return 0;
	}

	public int CurrentAP()
	{
		int num = 0;
		foreach (Policy item in list)
		{
			if (item.active)
			{
				num += item.Cost;
			}
		}
		return num;
	}

	public void RefreshEffects()
	{
		foreach (Happiness item in owner.happiness.list)
		{
			item.OnRefreshEffect();
		}
		foreach (BaseHomeResource item2 in owner.resources.list)
		{
			item2.OnRefreshEffect();
		}
		foreach (Policy item3 in list)
		{
			if (item3.active)
			{
				item3.RefreshEffect();
			}
		}
		owner.resources.SetDirty();
	}

	public void Validate()
	{
	}
}
