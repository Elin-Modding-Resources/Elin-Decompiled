using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class QuestCraft : Quest
{
	public class Req
	{
		[JsonProperty]
		public string idThing;

		[JsonProperty]
		public int num;

		public Req()
		{
		}

		public Req(string id, int n)
		{
			idThing = id;
			num = n;
		}
	}

	[JsonProperty]
	public List<Req> req1 = new List<Req>();

	[JsonProperty]
	public List<Req> req2 = new List<Req>();

	[JsonProperty]
	public int progress;

	public bool ConsumeGoods => false;

	public List<Req> CurrentReq
	{
		get
		{
			if (progress != 0)
			{
				return req2;
			}
			return req1;
		}
	}

	public bool hasSecondReq => req2.Count > 0;

	public override bool CanDeliverToClient(Chara c)
	{
		if (c.quest != this)
		{
			return false;
		}
		bool result = true;
		foreach (Req item in CurrentReq)
		{
			Thing thing = EClass.pc.things.Find(item.idThing);
			if (thing == null || thing.Num < item.num)
			{
				result = false;
				break;
			}
		}
		return result;
	}

	public override bool Deliver(Chara c, Thing t = null)
	{
		if (!CanDeliverToClient(c))
		{
			return false;
		}
		_ = ConsumeGoods;
		if (progress == 0 && hasSecondReq)
		{
			UpdateJournal();
		}
		else
		{
			EClass.game.quests.Complete(this);
		}
		OnProgressComplete();
		return true;
	}

	public void OnProgressComplete()
	{
		if (progress == 0)
		{
			string text = id;
			if (!(text == "fiama1"))
			{
				if (text == "ash1")
				{
					EClass._zone.AddCard(ThingGen.Create("string"), EClass.pc.pos);
				}
			}
			else
			{
				EClass._zone.AddCard(ThingGen.Create("water"), EClass.pc.pos);
			}
		}
		progress++;
	}

	public override void OnDropReward()
	{
		string text = id;
		if (!(text == "fiama1"))
		{
			if (text == "ash1")
			{
				DropReward(ThingGen.Create("crimAle"));
			}
		}
		else
		{
			DropReward(ThingGen.Create("crimAle"));
		}
	}

	public override void OnComplete()
	{
		string text = id;
		if (!(text == "ash1"))
		{
			if (text == "fiama1")
			{
				QuestCraft obj = Quest.Create("fiama2") as QuestCraft;
				obj.SetClient(base.chara);
				obj.req1.Add(new Req("torch_held", 1));
			}
		}
		else
		{
			QuestCraft obj2 = Quest.Create("ash2") as QuestCraft;
			obj2.SetClient(base.chara);
			obj2.req1.Add(new Req("torch_held", 1));
		}
	}

	public override string GetTextProgress()
	{
		string text = "";
		foreach (Req item in CurrentReq)
		{
			text = text + (text.IsEmpty() ? "" : Environment.NewLine) + "progressShowSupply".lang(EClass.sources.things.map[item.idThing].GetName(item.num));
		}
		return text;
	}

	public override string GetDetailText(bool onJournal = false)
	{
		return source.GetDetail().Split('|')[progress];
	}

	public override string GetTalkProgress()
	{
		return GetDetailText();
	}

	public override string GetTalkComplete()
	{
		if (progress != 1 || !hasSecondReq)
		{
			return Parse(source.GetText("talkComplete"));
		}
		return GetDetailText();
	}
}
