public class CharaGen : CardGen
{
	public static int objLv;

	public static Chara _Create(string id, int idMat = -1, int lv = -1)
	{
		Chara chara = new Chara();
		if (lv < 1)
		{
			lv = 1;
		}
		objLv = lv;
		chara.Create(id, idMat, lv);
		return chara;
	}

	public static Chara Create(string id, int lv = -1)
	{
		return _Create(id, -1, lv);
	}

	public static Chara CreateFromFilter(string id, int lv = -1, int levelRange = -1)
	{
		return CreateFromFilter(SpawnList.Get(id), lv, levelRange);
	}

	public static Chara CreateFromFilter(SpawnList list, int lv = -1, int levelRange = -1)
	{
		return Create(list.Select(lv, levelRange).id, lv);
	}

	public static Chara CreateFromElement(string idEle, int lv = -1, string idFilter = "chara")
	{
		SpawnList spawnList = SpawnListChara.Get("chara_ele" + idEle, delegate(SourceChara.Row c)
		{
			string[] mainElement = c.mainElement;
			for (int i = 0; i < mainElement.Length; i++)
			{
				if (mainElement[i].Split(',')[0] == idEle)
				{
					return true;
				}
			}
			return false;
		});
		CardBlueprint.Set(new CardBlueprint
		{
			idEle = idEle
		});
		return Create(spawnList.Select(lv).id, lv);
	}

	public static Chara CreateWealthy(int lv = -1)
	{
		return CreateFromFilter(SpawnListChara.Get("c_wealthy", (SourceChara.Row r) => r.works.Contains("Rich") || r.hobbies.Contains("Rich")), lv);
	}
}
