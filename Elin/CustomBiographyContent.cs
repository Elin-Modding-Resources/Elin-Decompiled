using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptOut)]
public class CustomBiographyContent : CustomFileContent
{
	private const int FallbackWordKey = -100;

	private static int _lastWordKey = -101;

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

	private int[] _tempLangKey = new int[4];

	private bool _initialized;

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
		if (_initialized)
		{
			return;
		}
		Biography bio = chara.bio;
		if (birthDay != 0)
		{
			bio.birthDay = birthDay;
		}
		if (birthMonth != 0)
		{
			bio.birthMonth = birthMonth;
		}
		if (birthYear != 0)
		{
			bio.birthYear = birthYear;
		}
		Dictionary<int, LangWord.Row> langWord = EClass.sources.langWord.map;
		if (!langWord.ContainsKey(-100))
		{
			_lastWordKey = -100;
			Dictionary<int, LangWord.Row> dictionary = langWord;
			LangWord.Row obj = new LangWord.Row
			{
				id = -100
			};
			LangWord.Row row = obj;
			dictionary[-100] = obj;
			row.name = (row.name_JP = (row.name_L = ""));
		}
		if (!birthPlace.IsEmpty())
		{
			bio.idHome = SetTempWord(birthPlace, 0);
		}
		if (!birthLocation.IsEmpty())
		{
			bio.idLoc = SetTempWord(birthLocation, 1);
		}
		if (!dad.IsEmpty())
		{
			bio.idAdvDad = -100;
			bio.idDad = SetTempWord(dad, 2);
		}
		if (!mom.IsEmpty())
		{
			bio.idAdvMom = -100;
			bio.idMom = SetTempWord(mom, 3);
		}
		if (!likeThing.IsEmpty() && EClass.sources.cards.map.ContainsKey(likeThing))
		{
			bio.idLike = likeThing;
		}
		if (!likeHobby.IsEmpty())
		{
			int element = Core.GetElement(likeHobby);
			if (element != EClass.sources.elements.rows[0].id)
			{
				bio.idHobby = element;
			}
		}
		_initialized = true;
		int SetTempWord(string text, int tempIndex)
		{
			int num = _tempLangKey[tempIndex];
			if (num == 0 || !langWord.ContainsKey(num))
			{
				while (langWord.ContainsKey(_lastWordKey))
				{
					_lastWordKey--;
				}
				num = _lastWordKey;
			}
			LangWord.Row row3 = (langWord[num] = new LangWord.Row
			{
				id = num
			});
			row3.name = (row3.name_JP = (row3.name_L = text));
			return _tempLangKey[tempIndex] = num;
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
		_initialized = false;
	}
}
