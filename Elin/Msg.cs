using System;
using UnityEngine;

public class Msg : EClass
{
	public static ThirstPersonInfo thirdPerson1 = new ThirstPersonInfo();

	public static ThirstPersonInfo thirdPerson2 = new ThirstPersonInfo();

	public static Color currentColor = colors.Default;

	public static bool alwaysVisible;

	public static bool ignoreAll;

	public static WidgetMainText mainText => WidgetMainText.Instance;

	public static WidgetFeed feed => WidgetFeed.Instance;

	public static MsgColors colors => EClass.core.refs.msgColors;

	public static void SetColor()
	{
		currentColor = colors.Default;
	}

	public static void SetColor(Color color)
	{
		currentColor = color;
	}

	public static void SetColor(string id)
	{
		currentColor = colors.colors[id];
	}

	public static string GetRawText(string idLang, string ref1, string ref2 = null, string ref3 = null, string ref4 = null)
	{
		thirdPerson1.Set(ref1);
		thirdPerson2.Set(ref2);
		return GameLang.Parse(GetGameText(idLang), IsThirdPerson(ref1), ref1, ref2, ref3, ref4);
	}

	public static string GetRawText(string idLang, Card c1, Card c2, string ref1 = null, string ref2 = null)
	{
		thirdPerson1.Set(c1);
		thirdPerson2.Set(c2);
		return GameLang.Parse(GetGameText(idLang), IsThirdPerson(c1), GetName(c1), GetName(c2), ref1, ref2);
	}

	public static string GetRawText(string idLang, Card c1, string ref1 = null, string ref2 = null, string ref3 = null)
	{
		thirdPerson1.Set(c1);
		thirdPerson2.Set(ref1);
		return GameLang.Parse(GetGameText(idLang), IsThirdPerson(c1), GetName(c1), ref1, ref2, ref3);
	}

	public static string Say(string idLang, string ref1, string ref2 = null, string ref3 = null, string ref4 = null)
	{
		thirdPerson1.Set(ref1);
		thirdPerson2.Set(ref2);
		return SayRaw(GameLang.Parse(GetGameText(idLang), IsThirdPerson(ref1), ref1, ref2, ref3, ref4));
	}

	public static string Say(string idLang, Card c1, Card c2, string ref1 = null, string ref2 = null)
	{
		thirdPerson1.Set(c1);
		thirdPerson2.Set(c2);
		return SayRaw(GameLang.Parse(GetGameText(idLang), IsThirdPerson(c1), GetName(c1), GetName(c2), ref1, ref2));
	}

	public static string Say(string idLang, Card c1, string ref1 = null, string ref2 = null, string ref3 = null)
	{
		thirdPerson1.Set(c1);
		thirdPerson2.Set(ref1);
		return SayRaw(GameLang.Parse(GetGameText(idLang), IsThirdPerson(c1), GetName(c1), ref1, ref2, ref3));
	}

	public static string Say(string idLang, Card c1, int i, string ref1 = null)
	{
		thirdPerson1.Set(c1);
		thirdPerson2.Set(ref1);
		return SayRaw(GameLang.Parse(GetGameText(idLang), IsThirdPerson(i), GetName(c1), i.ToString() ?? "", ref1));
	}

	public static string Say(string idLang, int i, string ref1 = null, string ref2 = null)
	{
		return SayRaw(GameLang.Parse(GetGameText(idLang), IsThirdPerson(i), ref1, ref2));
	}

	public static string Say(string idLang)
	{
		return SayRaw(GetGameText(idLang));
	}

	public static string SayNothingHappen()
	{
		return Say("nothingHappens");
	}

	public static string SayCannotUseHere()
	{
		return Say("cannot_use_here");
	}

	public static void SayGod(string s, Card owner = null)
	{
		SetColor(colors.TalkGod);
		Say(s.Bracket());
		owner?.SayRaw("@1" + s);
	}

	public static string SayRaw(string text)
	{
		if (ignoreAll)
		{
			currentColor = colors.Default;
			return "";
		}
		ToUpperFirst(text);
		if ((bool)feed)
		{
			feed.System(text);
		}
		if ((bool)mainText)
		{
			mainText.Append(text, currentColor);
		}
		currentColor = colors.Default;
		alwaysVisible = false;
		EClass.game.log.Add(text);
		return text;
	}

	public static void Append(Sprite sprite)
	{
		mainText.Append(sprite);
	}

	public static void AquireItem(string itemName)
	{
		Say("getItem", itemName ?? "");
	}

	public static void Nerun(string lang, string idPortrait = "UN_nerun")
	{
		string text = GameLang.Convert(lang.lang());
		if ((bool)feed)
		{
			feed.Nerun(text, idPortrait);
		}
		else if ((bool)mainText)
		{
			mainText.Append(text.Bracket(1), colors.Talk);
		}
	}

	public static void SayHomeMember(string lang)
	{
		string text = GameLang.Convert(lang.lang());
		if ((bool)feed)
		{
			feed.Nerun(text);
		}
		else if ((bool)mainText)
		{
			mainText.Append(text.Bracket(1), colors.Talk);
		}
	}

	public static void SayPic(Card c, string lang)
	{
		if (c != null)
		{
			string text = GameLang.Convert(lang.lang());
			if ((bool)feed)
			{
				feed.SayRaw(c, text);
			}
			else if ((bool)mainText)
			{
				mainText.Append(text.Bracket(1), colors.Talk);
			}
		}
	}

	public static void SayPic(string idPortrait, string lang, string _idPop = null)
	{
		string text = GameLang.Convert(lang.lang());
		if ((bool)feed)
		{
			feed.SayRaw(idPortrait, text, _idPop);
		}
		else if ((bool)mainText)
		{
			mainText.Append(text.Bracket(1), colors.Talk);
		}
	}

	public static PopItem Talk(Card c, string id)
	{
		PopItem result = null;
		if (c == null)
		{
			return null;
		}
		string text = GameLang.Convert(c.GetTalkText(id, stripPun: true));
		if ((bool)feed && c != null)
		{
			result = feed.SayRaw(c, text);
		}
		else if ((bool)mainText)
		{
			mainText.Append(text.Bracket(1), colors.Talk);
		}
		return result;
	}

	public static PopItem TalkHomeMemeber(string id)
	{
		Chara chara = null;
		if (EClass.Branch != null)
		{
			for (int i = 0; i < 99; i++)
			{
				chara = EClass.Branch.members.RandomItem();
				if (chara != EClass.pc)
				{
					break;
				}
			}
		}
		if (chara == null)
		{
			chara = EClass.pc;
		}
		return Talk(chara, id);
	}

	public static PopItem TalkMaid(string id)
	{
		foreach (Chara chara in EClass._map.charas)
		{
			if (chara.uid == EClass.Branch?.uidMaid)
			{
				return Talk(chara, id);
			}
		}
		return TalkHomeMemeber(id);
	}

	public static string GetGameText(string idLang, Card card = null)
	{
		LangGame.Row row = EClass.core.sources.langGame.map.TryGetValue(idLang);
		if (row == null)
		{
			return idLang.lang();
		}
		if (!row.effect.IsEmpty() && row.effect == "destroy")
		{
			mainText.Append(mainText.spriteDestroy);
		}
		if (!row.color.IsEmpty())
		{
			SetColor(colors.colors[row.color]);
		}
		if (!row.sound.IsEmpty())
		{
			EClass.Sound.Play(row.sound);
		}
		return row.GetText("text").Split(Environment.NewLine.ToCharArray()).RandomItem();
	}

	public static string GetName(Card c)
	{
		if (c == null)
		{
			return "null";
		}
		if (c.IsPC)
		{
			return "you".lang();
		}
		if (!alwaysVisible && (EClass.pc.isBlind || !EClass.pc.CanSee(c)) && c.parent == EClass._zone)
		{
			return (c.isChara ? "someone" : "something").lang();
		}
		return c.Name;
	}

	public static bool IsThirdPerson(Card c)
	{
		if (c == null)
		{
			return false;
		}
		if (c.IsPC || c.Num > 1)
		{
			return false;
		}
		return true;
	}

	public static bool IsThirdPerson(string n)
	{
		if (!int.TryParse(n, out var result))
		{
			return false;
		}
		return IsThirdPerson(result);
	}

	public static bool IsThirdPerson(int i)
	{
		return i <= 1;
	}

	public static void NewLine()
	{
		if ((bool)mainText)
		{
			mainText.NewLine();
		}
	}

	public unsafe static void ToUpperFirst(string str)
	{
		if (str != null)
		{
			fixed (char* ptr = str)
			{
				*ptr = char.ToUpper(*ptr);
			}
		}
	}
}
