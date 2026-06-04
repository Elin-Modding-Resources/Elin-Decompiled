using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptOut)]
public class CustomBiographyContent : CustomFileContent
{
	private const int FallbackWordKey = -100;

	private static int _lastWordKey;

	public string background;

	public int birthDay;

	public int birthMonth;

	public int birthYear;

	public string birthLocation;

	public string birthPlace;

	public string dad;

	public string mom;

	public string likeThing;

	public string likeHobby;

	public string favCategory;

	public string favFood;

	public static CustomBiographyContent CreateFromId(string biographyId, ModPackage owner = null)
	{
		var (fileInfo, eMod) = PackageIterator.GetFilesEx("Data/bio_" + biographyId + ".json").LastOrDefault();
		if (fileInfo == null)
		{
			return null;
		}
		if (owner == null)
		{
			owner = eMod as ModPackage;
		}
		return new CustomBiographyContent
		{
			ContentId = "Biography/" + biographyId,
			Owner = owner,
			File = fileInfo
		};
	}

	public override void OnSetLang(string lang)
	{
		CustomBiographyContent customBiographyContent = CreateFromId(base.ContentId.Split('/')[^1]);
		if (customBiographyContent != null)
		{
			base.File = customBiographyContent.File;
			base.LastModified = DateTime.MinValue;
		}
	}

	public void RefreshCharaBio(Chara chara)
	{
		Load();
		Biography bio = chara.bio;
		bio.birthDay = birthDay;
		bio.birthMonth = birthMonth;
		bio.birthYear = birthYear;
		Dictionary<int, LangWord.Row> langWord = EClass.sources.langWord.map;
		if (!langWord.ContainsKey(-100))
		{
			_lastWordKey = -100;
			SetTempWord("");
		}
		if (!birthPlace.IsEmpty())
		{
			bio.idHome = SetTempWord(birthPlace);
		}
		if (!birthLocation.IsEmpty())
		{
			bio.idLoc = SetTempWord(birthLocation);
		}
		if (!dad.IsEmpty())
		{
			bio.idAdvDad = -100;
			bio.idDad = SetTempWord(dad);
		}
		if (!mom.IsEmpty())
		{
			bio.idAdvMom = -100;
			bio.idMom = SetTempWord(mom);
		}
		if (!likeThing.IsEmpty() && EClass.sources.things.map.ContainsKey(likeThing))
		{
			bio.idLike = likeThing;
		}
		if (!likeHobby.IsEmpty() && Core.GetElement(likeHobby) != EClass.sources.elements.rows[0].id)
		{
			bio.idHobby = -100;
		}
		int SetTempWord(string text)
		{
			while (langWord.ContainsKey(_lastWordKey))
			{
				_lastWordKey--;
			}
			Dictionary<int, LangWord.Row> dictionary = langWord;
			int lastWordKey = _lastWordKey;
			LangWord.Row obj = new LangWord.Row
			{
				id = _lastWordKey
			};
			LangWord.Row row = obj;
			dictionary[lastWordKey] = obj;
			row.name = (row.name_JP = (row.name_L = text));
			return _lastWordKey;
		}
	}

	protected override void LoadContent()
	{
		CustomBiographyContent customBiographyContent = IO.LoadFile<CustomBiographyContent>(base.File.FullName);
		background = customBiographyContent.background;
		birthDay = customBiographyContent.birthDay;
		birthMonth = customBiographyContent.birthMonth;
		birthYear = customBiographyContent.birthYear;
		birthLocation = customBiographyContent.birthLocation;
		birthPlace = customBiographyContent.birthPlace;
		dad = customBiographyContent.dad;
		mom = customBiographyContent.mom;
		likeThing = customBiographyContent.likeThing;
		likeHobby = customBiographyContent.likeHobby;
		favCategory = customBiographyContent.favCategory;
		favFood = customBiographyContent.favFood;
	}
}
