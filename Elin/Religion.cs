using System;
using Newtonsoft.Json;
using UnityEngine;

public class Religion : EClass
{
	public enum ConvertType
	{
		Default,
		Campaign
	}

	[JsonProperty]
	public int relation;

	[JsonProperty]
	public int giftRank;

	[JsonProperty]
	public int mood;

	public static Religion recentWrath;

	private SourceReligion.Row _source;

	public virtual string id => "";

	public virtual bool IsAvailable => false;

	public virtual string Name => source.GetName();

	public SourceReligion.Row source
	{
		get
		{
			SourceReligion.Row row = _source;
			if (row == null)
			{
				SourceReligion.Row obj = EClass.sources.religions.map.TryGetValue(id) ?? EClass.sources.religions.map["eyth"];
				SourceReligion.Row row2 = obj;
				_source = obj;
				row = row2;
			}
			return row;
		}
	}

	public virtual string NameShort => source.GetTextArray("name2")[1];

	public virtual string NameDomain => source.GetTextArray("name2")[0];

	public virtual string TextType => ("sub_" + source.type).lang();

	public virtual string TextGodGender => source.GetText("textType");

	public virtual string TextMood => GetTextTemper();

	public bool IsEyth => id == "eyth";

	public bool IsEhekatl => id == "luck";

	public bool IsOpatos => id == "earth";

	public virtual bool IsMinorGod => false;

	public virtual bool CanJoin => true;

	public virtual SourceElement.Row GetFeat(int i)
	{
		return EClass.sources.elements.alias["featGod_" + id + i];
	}

	public virtual void Init()
	{
		relation = source.relation;
	}

	public virtual void OnLoad()
	{
	}

	public virtual void OnAdvanceDay()
	{
	}

	public virtual Sprite GetSprite()
	{
		return ResourceCache.Load<Sprite>("Media/Graphics/Image/Faction/" + source.id) ?? ResourceCache.Load<Sprite>("Media/Graphics/Image/Faction/eyth");
	}

	public virtual void SetTextRelation(UIText text)
	{
		if (relation > 100)
		{
			text.SetText("reFriend".lang(), FontColor.Good);
		}
		else if (relation < -100)
		{
			text.SetText("reEnemy".lang(), FontColor.Bad);
		}
		else
		{
			text.SetText("reNone".lang(), FontColor.Passive);
		}
	}

	public virtual string GetTextBenefit()
	{
		string text = "<color=green>";
		for (int i = 0; i < source.elements.Length; i += 2)
		{
			if (i != 0)
			{
				text = text + Lang.words.comma + Lang.space;
			}
			text += EClass.sources.elements.map[source.elements[i]].GetName();
		}
		text += "</color>";
		return source.GetText("textBenefit") + Environment.NewLine + Environment.NewLine + (IsEyth ? "" : "textBenefit".lang(text));
	}

	public virtual string GetTextTemper(int _temper = -99999)
	{
		if (IsEyth)
		{
			return "-";
		}
		if (_temper == -99999)
		{
			_temper = mood;
		}
		string[] list = Lang.GetList("temper");
		if (_temper <= -15)
		{
			if (_temper > -85)
			{
				if (_temper <= -45)
				{
					return list[1].ToTitleCase().TagColor(FontColor.Bad);
				}
				return list[2].ToTitleCase();
			}
			return list[0].ToTitleCase().TagColor(FontColor.Bad);
		}
		if (_temper < 45)
		{
			if (_temper < 15)
			{
				return list[3].ToTitleCase();
			}
			return list[4].ToTitleCase();
		}
		if (_temper < 85)
		{
			return list[5].ToTitleCase().TagColor(FontColor.Great);
		}
		return list[6].ToTitleCase().TagColor(FontColor.Good);
	}

	public virtual void Revelation(string idTalk, int chance = 100)
	{
		if (!IsEyth && EClass.rnd(100) <= chance)
		{
			Talk(idTalk, EClass.pc);
		}
	}

	public virtual void Talk(string idTalk, Card c = null, Card agent = null)
	{
		Msg.SetColor(Msg.colors.TalkGod);
		Msg.Say("<i>" + GetGodTalk(idTalk) + " </i>", c ?? EClass.pc);
	}

	public virtual string GetGodTalk(string idTalk)
	{
		return MOD.listGodTalk.GetTalk(id, idTalk);
	}

	public virtual int GetOfferingMtp(Thing t)
	{
		return 0;
	}

	public virtual int GetOfferingValue(Thing t, int num = -1)
	{
		t.CheckJustCooked();
		if (num == -1)
		{
			num = t.Num;
		}
		long v = 0L;
		if (t.source._origin == "meat")
		{
			v = Mathf.Clamp(t.SelfWeight / 10, 1, 1000);
			if (t.refCard == null)
			{
				v /= 10L;
			}
		}
		else if (GetOfferingMtp(t) > 0)
		{
			SetValue(t.category, GetOfferingMtp(t));
		}
		else
		{
			string[] cat_offer = source.cat_offer;
			foreach (string key in cat_offer)
			{
				if (t.category.IsChildOf(key))
				{
					SetValue(EClass.sources.categories.map[key], 1);
					break;
				}
			}
		}
		if (v == 0L)
		{
			return 0;
		}
		if (t.IsDecayed)
		{
			v /= 10L;
		}
		v = v * (100 + Mathf.Min(t.LV * 2, 100) + (t.HasElement(757) ? 50 : 0)) / 100;
		v = (int)Mathf.Clamp(Mathf.Max(v, 1f) * (float)num, 1f, 214748370f);
		return (int)v;
		void SetValue(SourceCategory.Row cat, int mtp)
		{
			v = Mathf.Clamp(t.SelfWeight / 10, 50, 1000);
			v = v * cat.offer * mtp / 100;
		}
	}

	public virtual int GetGiftRank()
	{
		if (IsEyth || source.rewards.Length == 0)
		{
			return -1;
		}
		int num = EClass.pc.Evalue(85);
		if (giftRank == 0 && (num >= 15 || EClass.debug.enable))
		{
			return 1;
		}
		if (source.rewards.Length >= 2 && giftRank == 1 && (num >= 30 || EClass.debug.enable))
		{
			return 2;
		}
		return -1;
	}

	public virtual bool TryGetGift()
	{
		int num = GetGiftRank();
		if (num == -1)
		{
			return false;
		}
		Point point = EClass.pc.pos.GetNearestPoint(allowBlock: false, allowChara: false, allowInstalled: false) ?? EClass.pc.pos;
		switch (num)
		{
		case 1:
		{
			Talk("pet");
			Chara chara = CharaGen.Create(source.rewards[0]);
			EClass._zone.AddCard(chara, point);
			chara.MakeAlly();
			chara.PlayEffect("aura_heaven");
			giftRank = 1;
			return true;
		}
		case 2:
		{
			Talk("gift");
			string[] array = source.rewards[1].Split('|');
			string[] array2 = array;
			foreach (string text in array2)
			{
				EClass.game.religions.Reforge(text, point, text == array[0]);
			}
			giftRank = 2;
			return true;
		}
		default:
			return false;
		}
	}

	public virtual void OnReforge(Thing t)
	{
		t.c_idDeity = id;
		if (IsIgnoreReforge(t))
		{
			return;
		}
		foreach (Element value in t.elements.dict.Values)
		{
			int num = value.id;
			if ((uint)(num - 64) > 3u && num != 92 && IsFaithElement(value))
			{
				value.vExp = -1;
			}
		}
	}

	public virtual bool IsIgnoreReforge(Thing t)
	{
		return false;
	}

	public virtual bool IsValidArtifact(string id)
	{
		return false;
	}

	public virtual bool IsFaithElement(Element e)
	{
		return false;
	}

	public virtual string[] GetValidArtifacts()
	{
		return Array.Empty<string>();
	}

	public virtual void OnBecomeBranchFaith()
	{
	}

	public virtual void JoinFaith(Chara c, ConvertType type = ConvertType.Default)
	{
		if (!c.IsPC)
		{
			c.faith = this;
			c.RefreshFaithElement();
			EClass.Sound.Play("worship");
			Msg.Say("changeFaith", c, Name);
			return;
		}
		if (c.faith != this)
		{
			c.faith.LeaveFaith(c, this, type);
		}
		if (type != ConvertType.Campaign)
		{
			EClass.pc.c_daysWithGod = 0;
		}
		Msg.Say("worship", Name);
		Talk("worship", c);
		EClass.Sound.Play("worship");
		c.PlayEffect("aura_heaven");
		c.faith = this;
		OnJoinFaith();
		if (IsEyth)
		{
			mood = 0;
		}
		else
		{
			mood = 50;
		}
		c.RefreshFaithElement();
		if (!c.HasElement(306))
		{
			c.elements.Learn(306);
		}
		if (!c.IsPC)
		{
			return;
		}
		EClass.pc.faction.charaElements.OnJoinFaith();
		if (EClass._zone.lv == 0)
		{
			if (EClass._zone is Zone_Mifu)
			{
				EClass._map.config.blossom = EClass.pc.faith == EClass.game.religions.Trickery;
				EClass.screen.RefreshWeather();
			}
			if (EClass._zone is Zone_Nefu)
			{
				EClass._map.config.blossom = EClass.pc.faith == EClass.game.religions.MoonShadow;
				EClass.screen.RefreshWeather();
			}
			if (EClass._zone is Zone_Aquli)
			{
				EClass._map.config.fixedCondition = ((EClass.pc.faith == EClass.game.religions.Strife) ? Weather.Condition.Ether : Weather.Condition.None);
				EClass.screen.RefreshWeather();
			}
		}
		EClass.pc.PurgeEythArtifact();
		if (this is ReligionHealing)
		{
			Steam.GetAchievement(ID_Achievement.FAITH_HEALING);
		}
	}

	public virtual void LeaveFaith(Chara c, Religion newFaith, ConvertType type)
	{
		bool flag = (newFaith == EClass.game.religions.Trickery && this == EClass.game.religions.MoonShadow) || (newFaith == EClass.game.religions.MoonShadow && this == EClass.game.religions.Trickery);
		if (c.IsPC)
		{
			Msg.Say("worship2");
			if (!flag && type != ConvertType.Campaign)
			{
				Punish(c);
			}
		}
		if (flag)
		{
			Talk("regards");
			c.elements.SetBase(85, c.Evalue(85) / 2);
		}
		else
		{
			c.elements.SetBase(85, 0);
		}
		c.RemoveAllStances();
		if (c.IsPC)
		{
			EClass.pc.faction.charaElements.OnLeaveFaith();
		}
		OnLeaveFaith();
		c.RefreshFaithElement();
	}

	public virtual void Punish(Chara c)
	{
		if (c.mimicry != null)
		{
			c.mimicry.Kill();
		}
		Talk("wrath");
		if (c.Evalue(1228) > 0)
		{
			c.SayNothingHappans();
			return;
		}
		c.hp = 1;
		c.mana.value = 1;
		c.stamina.value = 1;
		if (c.HasCondition<ConWrath>())
		{
			recentWrath = this;
			c.DamageHP(999999L, AttackSource.Wrath);
			recentWrath = null;
			return;
		}
		Thing thing = ThingGen.Create("punish_ball");
		int num = 0;
		foreach (Religion item in EClass.game.religions.list)
		{
			if (item.giftRank > 0)
			{
				num++;
			}
		}
		if (num >= 4)
		{
			thing.idSkin = 1;
		}
		thing.ChangeWeight(EClass.pc.WeightLimit / 4 + 1000);
		c.AddThing(thing);
		c.AddCondition<ConWrath>(2000 + (c.IsPC ? (EClass.pc.c_daysWithGod * 20) : 0));
	}

	public virtual void PunishTakeOver(Chara c)
	{
		if (c.mimicry != null)
		{
			c.mimicry.Kill();
		}
		Talk("takeoverFail");
		if (c.Evalue(1228) > 0)
		{
			c.SayNothingHappans();
			return;
		}
		c.hp /= 2;
		if (c.mana.value > 0)
		{
			c.mana.value = c.mana.value / 2;
		}
		if (c.stamina.value > 0)
		{
			c.stamina.value = c.stamina.value / 2;
		}
		if (c.HasCondition<ConWrath>())
		{
			recentWrath = this;
			c.DamageHP(999999L, AttackSource.Wrath);
			recentWrath = null;
			return;
		}
		Thing thing = ThingGen.Create("punish_ball");
		thing.c_weight = EClass.pc.WeightLimit / 4 + 1000;
		thing.isWeightChanged = true;
		thing.SetDirtyWeight();
		c.AddThing(thing);
		c.AddCondition<ConWrath>(200);
	}

	public virtual void OnJoinFaith()
	{
	}

	public virtual void OnLeaveFaith()
	{
	}

	public virtual void OnChangeHour()
	{
		if (IsEyth)
		{
			mood = 0;
		}
		else
		{
			mood = EClass.rnd(200) - 100;
		}
	}
}
