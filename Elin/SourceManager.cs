using System.Collections.Generic;
using System.Reflection;

public class SourceManager : EMono
{
	public LangGeneral langGeneral;

	public LangList langList;

	public LangGame langGame;

	public LangWord langWord;

	public LangNote langNote;

	public SourceCard cards = new SourceCard();

	public SourceChara charas;

	public SourceCharaText charaText;

	public SourceTactics tactics;

	public SourcePerson persons;

	public SourceThing things;

	public SourceElement elements;

	public SourceCalc calc;

	public SourceCheck checks;

	public SourceRace races;

	public SourceCategory categories;

	public SourceMaterial materials;

	public SourceSpawnList spawnLists;

	public SourceBlock blocks;

	public SourceFloor floors;

	public SourceCellEffect cellEffects;

	public SourceObj objs;

	public SourceFaction factions;

	public SourceReligion religions;

	public SourceJob jobs;

	public SourceHobby hobbies;

	public SourceQuest quests;

	public SourceStat stats;

	public SourceArea areas;

	public SourceZone zones;

	public SourceZoneAffix zoneAffixes;

	public SourceResearch researches;

	public SourceHomeResource homeResources;

	public SourceCollectible collectibles;

	public SourceGlobalTile globalTiles;

	public SourceThingV thingV;

	public SourceFood foods;

	public SourceRecipe recipes;

	public SourceBacker backers;

	public SourceAsset asset;

	public SourceKeyItem keyItems;

	private ExcelData _dataGodTalk;

	private List<SourceData> list = new List<SourceData>();

	public bool initialized;

	public ExcelData dataGodTalk => _dataGodTalk ?? (_dataGodTalk = new ExcelData(Lang.setting.dir + "Data/god_talk.xlsx", 3));

	public void InitLang()
	{
		langGeneral.Init();
		langGame.Init();
		langList.Init();
		langNote.Init();
		langWord.Init();
		Lang.General = langGeneral;
		Lang.Game = langGame;
		Lang.List = langList;
		Lang.Note = langNote;
		WordGen.source = langWord;
	}

	public void OnChangeLang()
	{
		_dataGodTalk = null;
	}

	public void Init()
	{
		if (initialized)
		{
			return;
		}
		initialized = true;
		list.Clear();
		FieldInfo[] fields = GetType().GetFields();
		foreach (FieldInfo fieldInfo in fields)
		{
			if (typeof(SourceData).IsAssignableFrom(fieldInfo.FieldType))
			{
				list.Add((SourceData)fieldInfo.GetValue(this));
			}
		}
		elements.Init();
		materials.Init();
		charas.Init();
		things.Init();
		thingV.Init();
		foods.Init();
		cards.Init();
		checks.Init();
		races.Init();
		persons.Init();
		categories.Init();
		spawnLists.Init();
		religions.Init();
		factions.Init();
		jobs.Init();
		hobbies.Init();
		floors.Init();
		blocks.Init();
		cellEffects.Init();
		objs.Init();
		stats.Init();
		areas.Init();
		zones.Init();
		zoneAffixes.Init();
		researches.Init();
		homeResources.Init();
		globalTiles.Init();
		floors.OnAfterInit();
		quests.Init();
		charaText.Init();
		calc.Init();
		recipes.Init();
		backers.Init();
		tactics.Init();
		keyItems.Init();
		ACT.Init();
		TimeTable.Init();
		Element.ListAttackElements.Clear();
		for (int j = 910; j < 927; j++)
		{
			Element.ListAttackElements.Add(EMono.sources.elements.map[j]);
		}
	}

	public void ExportSourceTexts(string path)
	{
		foreach (SourceData item in list)
		{
			item.ExportTexts(path);
		}
	}

	public void UpdateSourceTexts(string path)
	{
		foreach (SourceData item in list)
		{
			item.ExportTexts(path, update: true);
		}
	}

	public void ImportSourceTexts()
	{
		foreach (SourceData item in list)
		{
			if (item is SourceThingV)
			{
				things.ImportTexts(item.nameSheet);
			}
			else
			{
				item.ImportTexts();
			}
		}
	}

	public void ValidateLang()
	{
		Log.system = "";
		foreach (SourceData item in list)
		{
			item.ValidateLang();
		}
		string text = Lang.setting.dir + "validation.txt";
		IO.SaveText(text, Log.system);
		Util.Run(text);
	}
}
