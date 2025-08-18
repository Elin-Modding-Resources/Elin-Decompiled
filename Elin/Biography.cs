using System;
using Newtonsoft.Json;

public class Biography : EClass
{
	public static string[] idFemale = new string[5] { "shojo", "sister", "sister_cat", "younglady", "sister_undead" };

	[JsonProperty]
	public int[] ints = new int[20];

	[JsonProperty]
	public string[] strs = new string[3];

	public int[] personalities = new int[5];

	public string idLike
	{
		get
		{
			return strs[0];
		}
		set
		{
			strs[0] = value;
		}
	}

	public int gender
	{
		get
		{
			return ints[0];
		}
		set
		{
			ints[0] = value;
		}
	}

	public int weight
	{
		get
		{
			return ints[2];
		}
		set
		{
			ints[2] = value;
		}
	}

	public int height
	{
		get
		{
			return ints[3];
		}
		set
		{
			ints[3] = value;
		}
	}

	public int birthDay
	{
		get
		{
			return ints[4];
		}
		set
		{
			ints[4] = value;
		}
	}

	public int birthMonth
	{
		get
		{
			return ints[5];
		}
		set
		{
			ints[5] = value;
		}
	}

	public int birthYear
	{
		get
		{
			return ints[6];
		}
		set
		{
			ints[6] = value;
		}
	}

	public int idHome
	{
		get
		{
			return ints[7];
		}
		set
		{
			ints[7] = value;
		}
	}

	public int idDad
	{
		get
		{
			return ints[8];
		}
		set
		{
			ints[8] = value;
		}
	}

	public int idMom
	{
		get
		{
			return ints[9];
		}
		set
		{
			ints[9] = value;
		}
	}

	public int idLoc
	{
		get
		{
			return ints[10];
		}
		set
		{
			ints[10] = value;
		}
	}

	public int idAdvDad
	{
		get
		{
			return ints[11];
		}
		set
		{
			ints[11] = value;
		}
	}

	public int idAdvMom
	{
		get
		{
			return ints[12];
		}
		set
		{
			ints[12] = value;
		}
	}

	public int idHobby
	{
		get
		{
			return ints[13];
		}
		set
		{
			ints[13] = value;
		}
	}

	public int stability
	{
		get
		{
			return ints[14];
		}
		set
		{
			ints[14] = value;
		}
	}

	public int law
	{
		get
		{
			return ints[15];
		}
		set
		{
			ints[15] = value;
		}
	}

	public int affection
	{
		get
		{
			return ints[16];
		}
		set
		{
			ints[16] = value;
		}
	}

	public int dominance
	{
		get
		{
			return ints[17];
		}
		set
		{
			ints[17] = value;
		}
	}

	public int extroversion
	{
		get
		{
			return ints[18];
		}
		set
		{
			ints[18] = value;
		}
	}

	public int idInterest
	{
		get
		{
			return ints[19];
		}
		set
		{
			ints[19] = value;
		}
	}

	public string nameHome => StrBio(idHome);

	public string nameLoc => StrBio(idLoc);

	public string nameDad => "textParent".lang(StrBio(idAdvDad), StrBio(idDad));

	public string nameMom => "textParent".lang(StrBio(idAdvMom), StrBio(idMom));

	public string nameBirthplace => "birthLoc2".lang(nameHome, nameLoc);

	public bool IsUnderAge(Chara c)
	{
		return GetAge(c) < 18;
	}

	public string TextAge(Chara c)
	{
		return Lang.Parse("age", (GetAge(c) >= 1000) ? "???" : (GetAge(c).ToString() ?? ""));
	}

	public int GetAge(Chara c)
	{
		if (c.c_lockedAge != 0)
		{
			return c.c_lockedAge - 1;
		}
		if (c.IsUnique)
		{
			string[] array = c.source.bio.Split('/');
			if (array.Length > 1)
			{
				return int.Parse(array[1]);
			}
		}
		return EClass.world.date.year - birthYear;
	}

	public void SetAge(Chara c, int a)
	{
		if (c.IsUnique)
		{
			c.c_lockedAge = a + 1;
			string[] array = c.source.bio.Split('/');
			if (array.Length > 1)
			{
				SetBirthYear(c, int.Parse(array[1]));
			}
		}
		else if (c.c_lockedAge == 0)
		{
			SetBirthYear(c, a);
		}
		else
		{
			c.c_lockedAge = a + 1;
		}
	}

	public void SetBirthYear(Chara c, int a)
	{
		birthYear = EClass.world.date.year - a;
	}

	public void Generate(Chara c)
	{
		string bio = c.source.bio;
		_ = c.Chara.race;
		stability = Rand.rndNormal(100);
		law = Rand.rndNormal(100);
		affection = Rand.rndNormal(100);
		dominance = Rand.rndNormal(100);
		extroversion = Rand.rndNormal(100);
		idInterest = EClass.rnd(Enum.GetNames(typeof(Interest)).Length);
		if (idFemale.Contains(c.id) || c.race.id == "roran")
		{
			SetGender(1);
		}
		else
		{
			SetGender(Gender.GetRandom());
		}
		RerollBio(c);
		c.SetRandomTone();
		c.SetRandomTalk();
		bool flag = c.IsHuman;
		if (!bio.IsEmpty())
		{
			string[] array = bio.Split('/');
			SetGender((array[0] == "f") ? 1 : 2);
			if (array.Length > 1)
			{
				if (!c.source.HasTag(CTAG.randomPortrait))
				{
					flag = false;
				}
				SetBirthYear(c, int.Parse(array[1]));
				c.pccData = IO.LoadFile<PCCData>(CorePath.packageCore + "Data/PCC/" + c.id + ".txt");
			}
			if (array.Length > 2)
			{
				height = int.Parse(array[2]);
			}
			if (array.Length > 3)
			{
				weight = int.Parse(array[3]);
			}
			if (array.Length > 4)
			{
				c.c_idTone = array[4];
			}
			if (array.Length > 5 && !array[5].IsEmpty())
			{
				c.c_idTalk = array[5];
			}
		}
		if (c.trait is TraitGuildClerk)
		{
			flag = true;
		}
		if (flag && !c.HasTag(CTAG.noPortrait))
		{
			SetPortrait(c);
		}
		if (c.id == "prostitute" && GetAge(c) < 15)
		{
			SetBirthYear(c, 15);
		}
		SourceThing.Row row = EClass.sources.things.rows.RandomItem();
		idLike = row.id;
		idHobby = EClass.sources.elements.hobbies.RandomItem().id;
	}

	public void RerollBio(Chara c, int ageIndex = 0, bool keepParent = false)
	{
		GenerateBirthday(c, ageIndex);
		GenerateAppearance(c);
		if (!keepParent)
		{
			GenerateDad();
			GenerateMom();
			idHome = RndBio("home");
			idLoc = RndBio("loc");
		}
	}

	public void GenerateBirthday(Chara c, int ageIndex = 0)
	{
		SourceRace.Row race = c.race;
		int num = race.age[0];
		int num2 = race.age[1];
		if (ageIndex != 0)
		{
			int num3 = (num2 - num) / 4;
			SetBirthYear(c, Rand.Range(num + num3 * (ageIndex - 1), num + num3 * ageIndex));
		}
		else
		{
			SetBirthYear(c, Rand.Range(num, num2));
		}
		birthDay = EClass.rnd(30) + 1;
		birthMonth = EClass.rnd(12) + 1;
	}

	public void GenerateAppearance(Chara c)
	{
		SourceRace.Row race = c.race;
		height = race.height + EClass.rnd(race.height / 5 + 1) - EClass.rnd(race.height / 5 + 1);
		if (c.source.tag.Contains("mini"))
		{
			height /= 10;
		}
		if (c.source.tag.Contains("mini"))
		{
			height /= 10;
		}
		weight = height * height * (EClass.rnd(6) + 18) / 10000;
	}

	public void GenerateDad()
	{
		idDad = RndBio("parent");
		idAdvDad = RndBio("adv");
	}

	public void GenerateMom()
	{
		idMom = RndBio("parent");
		idAdvMom = RndBio("adv");
	}

	private int RndBio(string group)
	{
		return WordGen.GetID(group);
	}

	private string StrBio(int id)
	{
		object obj;
		if (!EClass.sources.langWord.map.ContainsKey(id))
		{
			obj = id.ToString();
			if (obj == null)
			{
				return "";
			}
		}
		else
		{
			obj = EClass.sources.langWord.map[id].GetText().Split(',')[0];
		}
		return (string)obj;
	}

	public void SetGender(int g)
	{
		gender = g;
		if (gender > 2)
		{
			gender = 0;
		}
	}

	public void SetPortrait(Chara c)
	{
		switch (c.id)
		{
		case "shojo":
			c.c_idPortrait = Portrait.GetRandomPortrait("special_f-littlegirl");
			break;
		case "sister":
		case "sister_shark":
		case "sister_penguin":
			c.c_idPortrait = Portrait.GetRandomPortrait("special_f-littlesister");
			break;
		case "citizen_exile":
			c.c_idPortrait = "special_n-exile";
			break;
		default:
			c.c_idPortrait = Portrait.GetRandomPortrait(gender, c.GetIdPortraitCat());
			break;
		}
	}

	public string TextBio(Chara c)
	{
		return c.race.GetText().ToTitleCase(wholeText: true) + " " + TextAge(c) + " " + Lang._gender(gender);
	}

	public string TextBio2(Chara c)
	{
		return Lang.Parse("heightWeight", height.ToString() ?? "", weight.ToString() ?? "") + " " + ((c.material.alias == "meat") ? "" : c.material.GetName().ToTitleCase(wholeText: true));
	}

	public string TextBioSlave(Chara c)
	{
		return " (" + Lang.GetList("genders_animal")[c.bio.gender] + " " + TextAge(c) + ")";
	}

	public string TextBirthDate(Chara c, bool _age = false)
	{
		return Lang.Parse("birthText", (birthYear >= 0) ? (birthYear.ToString() ?? "") : "???", birthMonth.ToString() ?? "", birthDay.ToString() ?? "") + (_age ? (" (" + TextAge(c) + ")") : "");
	}

	public string TextAppearance()
	{
		return Lang.Parse("heightWeight", height.ToString() ?? "", weight.ToString() ?? "");
	}
}
