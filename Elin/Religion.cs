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

	public SourceReligion.Row _source;

	public virtual string id => "";

	public virtual bool IsAvailable => false;

	public string Name => source.GetName();

	public SourceReligion.Row source => _source ?? (_source = EClass.sources.religions.map[id]);

	public string NameShort => source.GetTextArray("name2")[1];

	public string NameDomain => source.GetTextArray("name2")[0];

	public string TextType => ("sub_" + source.type).lang();

	public string TextGodGender => source.GetText("textType");

	public string TextMood => GetTextTemper();

	public bool IsEyth => id == "eyth";

	public bool IsEhekatl => id == "luck";

	public bool IsOpatos => id == "earth";

	public virtual bool IsMinorGod => false;

	public virtual bool CanJoin => true;

	public virtual SourceElement.Row GetFeat(int i)
	{
		return EClass.sources.elements.alias["featGod_" + id + i];
	}

	public void Init()
	{
		relation = source.relation;
	}

	public void OnLoad()
	{
	}

	public void OnAdvanceDay()
	{
	}

	public Sprite GetSprite()
	{
		return ResourceCache.Load<Sprite>("Media/Graphics/Image/Faction/" + source.id) ?? ResourceCache.Load<Sprite>("Media/Graphics/Image/Faction/eyth");
	}

	public void SetTextRelation(UIText text)
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

	public string GetTextBenefit()
	{
		string text = "";
		for (int i = 0; i < source.elements.Length; i += 2)
		{
			if (i != 0)
			{
				text = text + Lang.words.comma + Lang.space;
			}
			text += EClass.sources.elements.map[source.elements[i]].GetName();
		}
		return source.GetText("textBenefit") + (IsEyth ? "" : "textBenefit".lang(text));
	}

	public string GetTextTemper(int _temper = -99999)
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
		if (_temper <= -85)
		{
			return list[0].ToTitleCase().TagColor(FontColor.Bad);
		}
		if (_temper <= -45)
		{
			return list[1].ToTitleCase().TagColor(FontColor.Bad);
		}
		if (_temper <= -15)
		{
			return list[2].ToTitleCase();
		}
		if (_temper < 15)
		{
			return list[3].ToTitleCase();
		}
		if (_temper < 45)
		{
			return list[4].ToTitleCase();
		}
		if (_temper < 85)
		{
			return list[5].ToTitleCase().TagColor(FontColor.Great);
		}
		return list[6].ToTitleCase().TagColor(FontColor.Good);
	}

	public void Revelation(string idTalk, int chance = 100)
	{
		if (!IsEyth && EClass.rnd(100) <= chance)
		{
			Talk(idTalk, EClass.pc);
		}
	}

	public void Talk(string idTalk, Card c = null, Card agent = null)
	{
		if (!IsEyth)
		{
			Msg.SetColor(Msg.colors.TalkGod);
			Msg.Say("<i>" + GetGodTalk(idTalk) + " </i>", c ?? EClass.pc);
		}
	}

	public string GetGodTalk(string suffix)
	{
		return EClass.sources.dataGodTalk.GetText(id, suffix).Split(Environment.NewLine.ToCharArray()).RandomItem();
	}

	public virtual int GetOfferingMtp(Thing t)
	{
		return 0;
	}

	public int GetOfferingValue(Thing t, int num = -1)
	{
		t.CheckJustCooked();
		if (num == -1)
		{
			num = t.Num;
		}
		int v = 0;
		if (t.source._origin == "meat")
		{
			v = Mathf.Clamp(t.SelfWeight / 10, 1, 1000);
			if (t.refCard == null)
			{
				v /= 10;
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
		if (v == 0)
		{
			return 0;
		}
		if (t.IsDecayed)
		{
			v /= 10;
		}
		v = v * (100 + Mathf.Min(t.LV * 2, 100) + (t.HasElement(757) ? 50 : 0)) / 100;
		v = Mathf.Max(v, 1) * num;
		return v;
		void SetValue(SourceCategory.Row cat, int mtp)
		{
			v = Mathf.Clamp(t.SelfWeight / 10, 50, 1000);
			v *= cat.offer * mtp / 100;
		}
	}

	public bool TryGetGift()
	{
		if (IsEyth || source.rewards.Length == 0)
		{
			return false;
		}
		Point point = EClass.pc.pos.GetNearestPoint(allowBlock: false, allowChara: false, allowInstalled: false) ?? EClass.pc.pos;
		int num = EClass.pc.Evalue(85);
		if (giftRank == 0 && (num >= 15 || EClass.debug.enable))
		{
			Talk("pet");
			Chara chara = CharaGen.Create(source.rewards[0]);
			EClass._zone.AddCard(chara, point);
			chara.MakeAlly();
			chara.PlayEffect("aura_heaven");
			giftRank = 1;
			return true;
		}
		if (source.rewards.Length >= 2 && giftRank == 1 && (num >= 30 || EClass.debug.enable))
		{
			Talk("gift");
			string[] array = source.rewards[1].Split('|');
			string[] array2 = array;
			foreach (string text in array2)
			{
				Reforge(text, point, text == array[0]);
			}
			giftRank = 2;
			return true;
		}
		return false;
	}

	public bool IsValidArtifact(string id)
	{
		if (giftRank < 2)
		{
			return false;
		}
		return id switch
		{
			"gun_mani" => this == EClass.game.religions.Machine, 
			"cloak_mani" => this == EClass.game.religions.Machine, 
			"scythe_kumi" => this == EClass.game.religions.Harvest, 
			"blunt_earth" => this == EClass.game.religions.Earth, 
			"luckydagger" => this == EClass.game.religions.Luck, 
			"staff_element" => this == EClass.game.religions.Element, 
			"windbow" => this == EClass.game.religions.Wind, 
			"shirt_wind" => this == EClass.game.religions.Wind, 
			"pole_holy" => this == EClass.game.religions.Healing, 
			"sword_muramasa2" => this == EClass.game.religions.MoonShadow, 
			"kogitsunemaru" => this == EClass.game.religions.Trickery, 
			"warmonger" => this == EClass.game.religions.Strife, 
			_ => false, 
		};
	}

	public static Thing Reforge(string id, Point pos = null, bool first = true)
	{
		if (pos == null)
		{
			pos = EClass.pc.pos.Copy();
		}
		pos.Set(pos.GetNearestPoint(allowBlock: false, allowChara: false, allowInstalled: false, ignoreCenter: true) ?? pos);
		Thing thing = ThingGen.Create(id);
		foreach (Element value in thing.elements.dict.Values)
		{
			if (value.id == 66 || value.id == 67 || value.id == 64 || value.id == 65 || value.id == 92)
			{
				continue;
			}
			switch (id)
			{
			case "gun_mani":
				thing.c_idDeity = EClass.game.religions.Machine.id;
				break;
			case "cloak_mani":
				if (value.id == 427 || value.id == 957 || value.id == 105 || value.id == 466 || value.id == 664)
				{
					value.vExp = -1;
				}
				thing.c_idDeity = EClass.game.religions.Machine.id;
				break;
			case "scythe_kumi":
				if (value.id == 6650 || value.id == 480 || value.id == 959 || value.id == 428 || value.id == 640 || value.id == 665)
				{
					value.vExp = -1;
				}
				thing.c_idDeity = EClass.game.religions.Harvest.id;
				break;
			case "blunt_earth":
				if (value.id == 70 || value.id == 55 || value.id == 56 || value.id == 954 || value.id == 423 || value.id == 421)
				{
					value.vExp = -1;
				}
				thing.c_idDeity = EClass.game.religions.Earth.id;
				break;
			case "luckydagger":
				if (value.id != 426)
				{
					value.vExp = -1;
				}
				thing.c_idDeity = EClass.game.religions.Luck.id;
				break;
			case "staff_element":
				if (value.id == 411 || (value is Resistance && value.id != 959))
				{
					value.vExp = -1;
				}
				thing.c_idDeity = EClass.game.religions.Element.id;
				break;
			case "windbow":
				thing.c_idDeity = EClass.game.religions.Wind.id;
				break;
			case "shirt_wind":
				if (!(value is Resistance) && value.id != 226 && value.id != 152 && value.id != 77)
				{
					value.vExp = -1;
				}
				thing.c_idDeity = EClass.game.religions.Wind.id;
				break;
			case "pole_holy":
				if (value.id == 60 || value.id == 461 || value.id == 423)
				{
					value.vExp = -1;
				}
				thing.c_idDeity = EClass.game.religions.Healing.id;
				break;
			case "sword_muramasa2":
				if (value.id == 401 || value.id == 916 || value.id == 661)
				{
					value.vExp = -1;
				}
				thing.c_idDeity = EClass.game.religions.MoonShadow.id;
				break;
			case "kogitsunemaru":
				if (value.id != 656)
				{
					value.vExp = -1;
				}
				thing.c_idDeity = EClass.game.religions.Trickery.id;
				break;
			case "warmonger":
				if (value.id == 468 || value.id == 423 || value.id == 463 || value.id == 460 || value.id == 464 || value.id == 465)
				{
					value.vExp = -1;
				}
				thing.c_idDeity = EClass.game.religions.Strife.id;
				break;
			}
		}
		EClass._zone.AddCard(thing, pos);
		pos.PlayEffect("aura_heaven");
		if (first)
		{
			pos.PlaySound("godbless");
		}
		return thing;
	}

	public virtual void OnBecomeBranchFaith()
	{
	}

	public void JoinFaith(Chara c, ConvertType type = ConvertType.Default)
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
		if (c.IsPC)
		{
			EClass.pc.faction.charaElements.OnJoinFaith();
		}
	}

	public void LeaveFaith(Chara c, Religion newFaith, ConvertType type)
	{
		if (IsEyth)
		{
			return;
		}
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
		if (c.IsPC)
		{
			EClass.pc.faction.charaElements.OnLeaveFaith();
		}
		OnLeaveFaith();
		c.RefreshFaithElement();
	}

	public void Punish(Chara c)
	{
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
			c.DamageHP(999999, AttackSource.Wrath);
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

	public void PunishTakeOver(Chara c)
	{
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
			c.DamageHP(999999, AttackSource.Wrath);
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

	public void OnChangeHour()
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
