using System;
using System.Text;

public class GameLang : EClass
{
	public static string refDrama1;

	public static string refDrama2;

	public static string refDrama3;

	public static string refDrama4;

	public static string ConvertDrama(string text, Chara c = null)
	{
		if (!EClass.core.IsGameStarted)
		{
			return text;
		}
		StringBuilder stringBuilder = new StringBuilder();
		string newValue = "";
		string newValue2 = "";
		string name = EClass.game.religions.GetRandomReligion(onlyJoinable: false).Name;
		string @ref = "-";
		if (c != null)
		{
			newValue = c.NameBraced;
			newValue2 = (c.IsMale ? "his" : "her").lang();
			name = c.faith.Name;
			@ref = CalcGold.Hire(c).ToString() ?? "";
		}
		bool flag = false;
		string text2 = "";
		for (int i = 0; i < text.Length; i++)
		{
			if (flag)
			{
				if (text[i] == ']')
				{
					flag = false;
					if (text2 == "qFriend" && EClass._zone.IsPCFaction)
					{
						text2 = "qFriendHome";
					}
					string[] list = Lang.GetList(text2);
					if (list != null)
					{
						stringBuilder.Append(list.RandomItem());
					}
					else
					{
						stringBuilder.Append(text2);
					}
				}
				else
				{
					text2 += text[i];
				}
			}
			else if (text[i] == '[')
			{
				text2 = "";
				flag = true;
			}
			else
			{
				stringBuilder.Append(text[i]);
			}
		}
		if (DramaManager.TG != null)
		{
			stringBuilder.Replace("#tg", DramaManager.TG.Name);
		}
		if (DramaChoice.lastChoice != null)
		{
			stringBuilder.Replace("#last_choice", DramaChoice.lastChoice.text);
		}
		stringBuilder.Replace("#newline", Environment.NewLine);
		stringBuilder.Replace("#costHire", "costHire".lang(@ref));
		stringBuilder.Replace("#self", newValue);
		stringBuilder.Replace("#his", newValue2);
		stringBuilder.Replace("#me", newValue);
		stringBuilder.Replace("#1", refDrama1);
		stringBuilder.Replace("#2", refDrama2);
		stringBuilder.Replace("#3", refDrama3);
		stringBuilder.Replace("#4", refDrama4);
		stringBuilder.Replace("#god", name);
		stringBuilder.Replace("#player", EClass.player.title);
		stringBuilder.Replace("#title", EClass.player.title);
		stringBuilder.Replace("#zone", EClass._zone.Name);
		stringBuilder.Replace("#guild_title", Guild.CurrentDrama.relation.TextTitle);
		stringBuilder.Replace("#guild", Guild.CurrentDrama.Name);
		stringBuilder.Replace("#race", EClass.pc.race.GetName());
		stringBuilder.Replace("#pc", EClass.pc.NameSimple);
		stringBuilder.Replace("#aka", EClass.pc.Aka);
		stringBuilder.Replace("#bigdaddy", "bigdaddy".lang());
		stringBuilder.Replace("#festival", EClass._zone.IsFestival ? (EClass._zone.id + "_festival").lang() : "_festival".lang());
		stringBuilder.Replace("#brother2", (EClass.pc.IsMale ? "brother" : "sister").lang());
		stringBuilder.Replace("#brother", Lang.GetList(EClass.pc.IsMale ? "bro" : "sis").RandomItem());
		stringBuilder.Replace("#onii2", Lang.GetList(EClass.pc.IsMale ? "onii2" : "onee2").RandomItem());
		stringBuilder.Replace("#onii", Lang.GetList(EClass.pc.IsMale ? "onii" : "onee").RandomItem());
		stringBuilder.Replace("#gender", Lang.GetList("gendersDrama")[EClass.pc.bio.gender]);
		stringBuilder.Replace("#he", ((EClass.pc.bio.gender == 2) ? "he" : "she").lang());
		stringBuilder.Replace("#He", ((EClass.pc.bio.gender == 2) ? "he" : "she").lang().ToTitleCase());
		return Convert(stringBuilder);
	}

	public static string Convert(string text)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(text);
		return Convert(stringBuilder);
	}

	public static string Convert(StringBuilder sb)
	{
		sb.Replace("#player", EClass.player.title);
		sb.Replace("#pc_him", EClass.pc.IsMale ? "him".lang() : "her".lang());
		sb.Replace("#pc_his", EClass.pc.IsMale ? "his".lang() : "her".lang());
		sb.Replace("#pc_race", EClass.pc.race.GetName());
		sb.Replace("#pc", EClass.pc.NameSimple);
		return sb.ToString();
	}

	public static string Parse(string text, bool thirdPerson, string val1, string val2 = null, string val3 = null, string val4 = null)
	{
		StringBuilder stringBuilder = new StringBuilder(text);
		if (val1 != null)
		{
			if (Lang.articlesToRemove.Count > 0)
			{
				string text2 = val1;
				foreach (char[] item in Lang.articlesToRemove)
				{
					text2 = text2.TrimStart(item);
				}
				stringBuilder.Replace("-#1", text2);
			}
			stringBuilder.Replace("#1", val1 ?? "");
		}
		if (val2 != null)
		{
			if (Lang.articlesToRemove.Count > 0)
			{
				string text3 = val2;
				foreach (char[] item2 in Lang.articlesToRemove)
				{
					text3 = text3.TrimStart(item2);
				}
				stringBuilder.Replace("-#2", text3);
			}
			stringBuilder.Replace("#2", val2);
		}
		if (val3 != null)
		{
			stringBuilder.Replace("#3", val3);
		}
		if (val4 != null)
		{
			stringBuilder.Replace("#4", val4);
		}
		if (int.TryParse(val1, out var result))
		{
			stringBuilder.Replace("#(s)", (result <= 1) ? "" : "_s".lang());
		}
		if (Msg.thirdPerson2.active)
		{
			stringBuilder.Replace("#your2", Msg.thirdPerson2.your);
			stringBuilder.Replace("#himself2", Msg.thirdPerson2.himself);
			stringBuilder.Replace("#him2", Msg.thirdPerson2.him);
			stringBuilder.Replace("#his2", Msg.thirdPerson2.his);
			stringBuilder.Replace("#he2", Msg.thirdPerson2.he);
			stringBuilder.Replace("(s2)", Msg.thirdPerson2.thirdperson ? "s" : "");
		}
		if (Msg.thirdPerson1.active)
		{
			stringBuilder.Replace("#your", Msg.thirdPerson1.your);
			stringBuilder.Replace("#himself", Msg.thirdPerson1.himself);
			stringBuilder.Replace("#him", Msg.thirdPerson1.him);
			stringBuilder.Replace("#his", Msg.thirdPerson1.his);
			stringBuilder.Replace("#he", Msg.thirdPerson1.he);
		}
		if (Lang.setting.thirdperson)
		{
			return ConvertThirdPerson(stringBuilder.ToString(), thirdPerson);
		}
		return Convert(stringBuilder);
	}

	public static string ConvertThirdPerson(string text, bool thirdPerson)
	{
		string[] array = text.Split(' ');
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = true;
		string[] array2 = array;
		foreach (string text2 in array2)
		{
			if (flag)
			{
				flag = false;
			}
			else
			{
				stringBuilder.Append(' ');
			}
			if (text2.Contains('/') && !text2.Contains('<'))
			{
				string[] array3 = text2.Split('/');
				stringBuilder.Append(thirdPerson ? array3[1] : array3[0]);
			}
			else
			{
				stringBuilder.Append(text2);
			}
		}
		stringBuilder.Replace("(s)", thirdPerson ? "s" : "");
		stringBuilder.Replace("(es)", thirdPerson ? "es" : "");
		return Convert(stringBuilder);
	}
}
