using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class Party : EClass
{
	[JsonProperty]
	public int uidLeader;

	[JsonProperty]
	public List<int> uidMembers = new List<int>();

	public List<Chara> _members;

	public RefChara refLeader = new RefChara();

	public List<Chara> members
	{
		get
		{
			if (_members != null)
			{
				return _members;
			}
			return SetMembers();
		}
	}

	public Chara leader
	{
		get
		{
			return refLeader.GetAndCache(uidLeader);
		}
		set
		{
			refLeader.Set(ref uidLeader, value);
		}
	}

	public List<Chara> SetMembers()
	{
		_members = new List<Chara>();
		foreach (int uidMember in uidMembers)
		{
			members.Add(RefChara.Get(uidMember));
		}
		return _members;
	}

	public void AddMemeber(Chara c, bool showMsg = false)
	{
		if (c.party == this)
		{
			return;
		}
		if (!c.IsGlobal)
		{
			Debug.LogError("exception: " + c?.ToString() + " is not global chara");
		}
		members.Add(c);
		uidMembers.Add(c.uid);
		c.party = this;
		c.isSale = false;
		c.SetBool(18, enable: false);
		if (c.homeBranch != null)
		{
			c.RefreshWorkElements(c.homeBranch.elements);
			c.homeBranch.RefreshEfficiency();
			c.homeBranch.policies.Validate();
			if (c.homeBranch.owner.map != null)
			{
				c.homeBranch.owner.map.props.sales.Remove(c);
			}
		}
		if (showMsg)
		{
			Msg.Say("party_join", c.Name);
			SE.Play("party_join");
		}
		WidgetRoster.SetDirty();
	}

	public void RemoveMember(Chara c)
	{
		if (c.host != null)
		{
			ActRide.Unride(c.host, c.host.parasite == c);
		}
		members.Remove(c);
		uidMembers.Remove(c.uid);
		c.party = null;
		c.c_wasInPcParty = false;
		c.SetDirtySpeed();
		if (c.homeBranch != null)
		{
			c.homeBranch.RefreshEfficiency();
			c.RefreshWorkElements(c.homeBranch.elements);
		}
		WidgetRoster.SetDirty();
	}

	public void Replace(Chara c, int index)
	{
		members.Remove(c);
		uidMembers.Remove(c.uid);
		members.Insert(index, c);
		uidMembers.Insert(index, c.uid);
	}

	public void SetLeader(Chara c)
	{
		leader = c;
	}

	public Element GetPartySkill(int ele)
	{
		return GetBestSkill(ele);
	}

	public void ModExpPartySkill(int ele, int a)
	{
	}

	public Element GetBestSkill(int ele)
	{
		Element element = Element.Create(ele);
		foreach (Chara member in members)
		{
			if (member.IsAliveInCurrentZone && member.Evalue(ele) > element.Value)
			{
				element = member.elements.GetElement(ele);
			}
		}
		return element;
	}

	public bool IsCriticallyWounded(bool includePc = false)
	{
		foreach (Chara member in members)
		{
			if ((!includePc || !member.IsPC) && member.IsCriticallyWounded())
			{
				return true;
			}
		}
		return false;
	}

	public int EValue(int ele)
	{
		int num = 0;
		foreach (Chara member in members)
		{
			if (member.Evalue(ele) > num)
			{
				num = member.Evalue(ele);
			}
		}
		return num;
	}

	public bool HasElement(int ele)
	{
		foreach (Chara member in members)
		{
			if (member.HasElement(ele))
			{
				return true;
			}
		}
		return false;
	}

	public int Count()
	{
		int num = 0;
		foreach (Chara member in members)
		{
			if (!member.isDead)
			{
				num++;
			}
		}
		return num;
	}
}
