using UnityEngine;

public class TraitScrollMap : TraitScroll
{
	public string idSourceZone => owner.GetStr(30);

	public SourceZone.Row sourceZone => EClass.sources.zones.map[idSourceZone];

	public override bool CanStack => false;

	public bool IsBlank
	{
		get
		{
			if (!idSourceZone.IsEmpty())
			{
				return !EClass.sources.zones.map.ContainsKey(idSourceZone);
			}
			return true;
		}
	}

	public bool HasPrefix => owner.GetInt(24) != 0;

	public override int GetActDuration(Chara c)
	{
		return 5;
	}

	public override void SetName(ref string s)
	{
		if (!IsBlank)
		{
			s = "_of".lang(sourceZone.GetName(), s);
			if (HasPrefix)
			{
				s = EClass.sources.zoneAffixes.map[owner.GetInt(24)].GetName() + Lang.space + s;
			}
		}
		int num = owner.GetInt(25);
		if (num > 0)
		{
			s = s + " Lv " + (num + 1);
		}
	}

	public override void OnRead(Chara c)
	{
		int lv = owner.GetInt(25);
		Zone zone = EClass.world.region.CreateRandomSite(EClass._zone, 8, idSourceZone, updateMesh: true, lv);
		if (zone == null)
		{
			Msg.Say("nothingHappens");
			return;
		}
		zone.isKnown = true;
		if (HasPrefix)
		{
			zone.idPrefix = owner.GetInt(24);
		}
		Msg.Say("discoverZone", zone.NameWithDangerLevel);
		owner.ModNum(-1);
		Debug.Log(zone.Name + "/" + zone.x + "/" + zone.y);
	}
}
