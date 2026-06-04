using UnityEngine;

public class ReligionCustom : Religion
{
	public CustomReligionContent content;

	public override string id => content?.SourceId ?? "";

	public override bool CanJoin => content?.canJoin ?? true;

	public override bool IsMinorGod => content?.isMinorGod ?? false;

	public override bool IsAvailable => true;

	public override Sprite GetSprite()
	{
		return ModUtil.LoadSprite(id, null, null, 150, 200) ?? base.GetSprite();
	}

	public override bool IsValidArtifact(string thingId)
	{
		return content.artifacts.Contains(thingId);
	}

	public override string[] GetValidArtifacts()
	{
		return content.artifacts.ToArray();
	}

	public override bool IsFaithElement(Element e)
	{
		return content.elements.Contains(e.source.alias);
	}

	public override int GetOfferingMtp(Thing t)
	{
		if (content.offeringMtp.TryGetValue(t.id, out var value))
		{
			return value;
		}
		return base.GetOfferingMtp(t);
	}

	public override int GetOfferingValue(Thing t, int num = -1)
	{
		if (num == -1)
		{
			num = t.Num;
		}
		int offeringValue = base.GetOfferingValue(t, num);
		if (content.offeringValue.TryGetValue(t.id, out var value))
		{
			var args = new
			{
				@base = offeringValue,
				lv = t.LV,
				rarity = t.rarityLv
			};
			if (value.TryEvaluateAsCalc(out int result, (object)args))
			{
				return result;
			}
		}
		return offeringValue;
	}

	public override void Punish(Chara c)
	{
		if (content.noPunish)
		{
			c.SayNothingHappans();
		}
		else
		{
			base.Punish(c);
		}
	}

	public override void PunishTakeOver(Chara c)
	{
		if (content.noPunishTakeover)
		{
			c.SayNothingHappans();
		}
		else
		{
			base.PunishTakeOver(c);
		}
	}
}
