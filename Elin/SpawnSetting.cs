public class SpawnSetting
{
	public int filterLv = -1;

	public int fixedLv = -1;

	public int tries = 100;

	public int levelRange = -1;

	public bool isBoss;

	public bool isEvolved;

	public string idSpawnList;

	public string id;

	public string idEle;

	public Rarity rarity = Rarity.Random;

	public SpawnHostility hostility = SpawnHostility.Enemy;

	public Hostility? forcedHostility;

	public SpawnPosition position;

	public static SpawnSetting Default = new SpawnSetting();

	public static SpawnSetting Debug = new SpawnSetting();

	public static SpawnSetting Evolved(int fixedLv = -1)
	{
		return new SpawnSetting
		{
			fixedLv = fixedLv,
			rarity = Rarity.Legendary,
			forcedHostility = Hostility.Neutral,
			isEvolved = true,
			tries = 10000
		};
	}

	public static SpawnSetting Boss(int filterLv, int fixedLv = -1)
	{
		return new SpawnSetting
		{
			filterLv = filterLv - 2,
			fixedLv = fixedLv,
			rarity = Rarity.Legendary,
			isBoss = true,
			tries = 10000,
			levelRange = 5,
			forcedHostility = Hostility.Enemy
		};
	}

	public static SpawnSetting Boss(string id, string idEle = null, int fixedLv = -1)
	{
		return new SpawnSetting
		{
			id = id,
			idEle = idEle,
			fixedLv = fixedLv,
			rarity = Rarity.Legendary,
			isBoss = true,
			tries = 10000
		};
	}

	public static SpawnSetting Encounter(int lv)
	{
		return new SpawnSetting
		{
			filterLv = lv,
			hostility = SpawnHostility.Random
		};
	}

	public static SpawnSetting Mob(string id, string idEle = null, int fixedLv = -1)
	{
		return new SpawnSetting
		{
			id = id,
			idEle = idEle,
			fixedLv = fixedLv
		};
	}

	public static SpawnSetting HomeWild(int lv)
	{
		return new SpawnSetting
		{
			filterLv = lv,
			hostility = SpawnHostility.Random,
			idSpawnList = "c_animal",
			position = SpawnPosition.Outside,
			rarity = Rarity.Normal
		};
	}

	public static SpawnSetting HomeGuest(int lv)
	{
		return new SpawnSetting
		{
			filterLv = lv,
			hostility = SpawnHostility.Neutral,
			idSpawnList = "c_guest",
			position = SpawnPosition.Guest,
			rarity = Rarity.Normal
		};
	}

	public static SpawnSetting HomeEnemy(int lv)
	{
		return new SpawnSetting
		{
			filterLv = lv,
			position = SpawnPosition.Outside,
			rarity = Rarity.Normal,
			hostility = SpawnHostility.Enemy
		};
	}

	public static SpawnSetting DefenseEnemy(int lv)
	{
		return new SpawnSetting
		{
			filterLv = lv,
			position = SpawnPosition.Outside,
			rarity = Rarity.Normal
		};
	}

	public static SpawnSetting Fish()
	{
		return new SpawnSetting
		{
			hostility = SpawnHostility.Random,
			idSpawnList = "c_randomFish"
		};
	}
}
