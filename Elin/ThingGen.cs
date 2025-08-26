using System.Linq;
using UnityEngine;

public class ThingGen : CardGen
{
	public static Thing _Create(string id, int idMat = -1, int lv = -1)
	{
		Thing thing = new Thing();
		CardRow s = EClass.sources.cards.map.TryGetValue(id);
		if (s == null)
		{
			Debug.LogError("exception: Item not found:" + id);
			id = "869";
			s = EClass.sources.cards.map.TryGetValue(id);
		}
		if (s.isOrigin)
		{
			id = SpawnListThing.Get("origin_" + id, (SourceThing.Row a) => a.origin == s).Select(lv)?.id ?? EClass.sources.cards.rows.Where((CardRow a) => a.origin == s).First().id;
		}
		if (lv < 1)
		{
			lv = 1;
		}
		thing.Create(id, idMat, lv);
		if (thing.trait is TraitAmmo)
		{
			thing.SetNum(EClass.rnd(20) + 10);
		}
		return thing;
	}

	public static Thing TestCreate()
	{
		return _Create(SpawnList.Get("thing").Select().id);
	}

	public static Thing CreateCurrency(int a, string id = "money")
	{
		return Create(id).SetNum(a);
	}

	public static Thing CreateParcel(string idLang = null, params Thing[] things)
	{
		Thing thing = Create("parcel");
		foreach (Thing c in things)
		{
			thing.AddCard(c);
		}
		thing.c_idRefName = idLang.lang();
		return thing;
	}

	public static Thing Create(string id, int idMat = -1, int lv = -1)
	{
		return _Create(id, idMat, lv);
	}

	public static Thing Create(string id, string idMat, int lv = -1)
	{
		return Create(id, idMat.IsEmpty() ? (-1) : EClass.sources.materials.alias[idMat].id, lv);
	}

	public static Thing CreateFromFilter(string id, int lv = -1)
	{
		return _Create(SpawnList.Get(id).Select(lv).id, -1, lv);
	}

	public static Thing CreateRawMaterial(SourceMaterial.Row m)
	{
		Thing thing = Create(m.thing);
		thing.ChangeMaterial(m.id);
		return thing;
	}

	public static Thing CreateFromCategory(string idCat, int lv = -1)
	{
		return _Create(SpawnListThing.Get("cat_" + idCat, (SourceThing.Row s) => EClass.sources.categories.map[s.category].IsChildOf(idCat)).Select(lv).id, -1, lv);
	}

	public static Thing CreateFromTag(string idTag, int lv = -1)
	{
		return _Create(SpawnListThing.Get("tag_" + idTag, (SourceThing.Row s) => s.tag.Contains(idTag)).Select(lv).id, -1, lv);
	}

	public static Thing CreateBill(int pay, bool tax)
	{
		Thing thing = Create(tax ? "bill_tax" : "bill");
		thing.c_bill = pay;
		if (tax)
		{
			EClass.player.stats.taxBills += thing.c_bill;
			EClass.player.taxBills++;
		}
		else
		{
			EClass.player.unpaidBill += thing.c_bill;
		}
		return thing;
	}

	public static Thing CreateBlock(int id, int idMat)
	{
		Thing thing = Create(EClass.sources.blocks.rows[id].idThing, idMat);
		thing.refVal = id;
		return thing;
	}

	public static Thing CreateFloor(int id, int idMat, bool platform = false)
	{
		_ = EClass.sources.floors.rows[id];
		Thing thing = Create(platform ? "platform" : "floor", idMat);
		thing.refVal = id;
		return thing;
	}

	public static Thing CreateObj(int id, int idMat)
	{
		_ = EClass.sources.objs.rows[id];
		Thing thing = Create("obj", idMat);
		thing.refVal = id;
		return thing;
	}

	public static Thing CreateMap(string idSource = null, int lv = -1)
	{
		if (idSource.IsEmpty())
		{
			idSource = EClass.game.world.region.GetRandomSiteSource().id;
		}
		if (lv == -1)
		{
			lv = 1 + EClass.rnd(EClass.rnd(50) + 1);
		}
		return Create("map");
	}

	public static Thing CreatePlan(int ele)
	{
		Thing thing = Create("book_plan");
		thing.refVal = ele;
		return thing;
	}

	public static Thing CreateRecipe(string id)
	{
		Thing thing = Create("rp_random");
		thing.SetStr(53, id);
		return thing;
	}

	public static Thing CreateSpellbook(string alias, int num = 1)
	{
		return CreateSpellbook(EClass.sources.elements.alias[alias].id, num);
	}

	public static Thing CreateSpellbook(int ele, int num = 1)
	{
		Thing thing = Create("spellbook").SetNum(num);
		TraitSpellbook.Create(thing, ele);
		return thing;
	}

	public static Thing CreateRedBook(string id, int num = 1)
	{
		Thing thing = Create("book").SetNum(num);
		thing.SetStr(53, id);
		thing.idSkin = (thing.trait as TraitBook).Item.skin;
		return thing;
	}

	public static Thing CreateSkillbook(int ele, int num = 1)
	{
		Thing thing = Create("book_skill").SetNum(num);
		thing.refVal = ele;
		return thing;
	}

	public static Thing CreateScroll(int ele, int num = 1)
	{
		Thing thing = Create("scroll_random").SetNum(num);
		TraitSpellbook.Create(thing, ele);
		return thing;
	}

	public static Thing CreateRune(int ele, int v, bool free = false)
	{
		Thing thing = Create(free ? "rune_free" : "rune");
		thing.refVal = ele;
		thing.encLV = v;
		return thing;
	}

	public static Thing CreateRod(int ele, int charge = -1)
	{
		Thing thing = Create("rod");
		TraitRod.Create(thing, ele, charge);
		return thing;
	}

	public static Thing CreatePotion(int ele, int num = 1)
	{
		Thing thing = Create("1163").SetNum(num);
		TraitPotion.Create(thing, ele);
		return thing;
	}

	public static Thing CreatePerfume(int ele, int num = 1)
	{
		Thing thing = Create("perfume").SetNum(num);
		TraitPotion.Create(thing, ele);
		return thing;
	}

	public static Thing CreateLetter(string idLetter)
	{
		Thing thing = Create("letter");
		thing.SetStr(53, idLetter);
		return thing;
	}

	public static Thing CreateCardboardBox(int uidZone = -1)
	{
		Thing thing = Create("cardboard_box", new string[5] { "pine", "wood_birch", "wood_acacia", "oak", "cedar" }.RandomItem());
		thing.things.DestroyAll();
		Spatial spatial = ((uidZone == -1) ? null : EClass.game.spatials.map.TryGetValue(uidZone));
		if (spatial != null)
		{
			thing.c_idRefName = "sender_header".lang("sender_post".lang(spatial.Name));
		}
		return thing;
	}

	public static Thing CreateTreasure(string id, int lv, TreasureType type = TreasureType.Map)
	{
		Thing thing = Create(id, -1, lv);
		CreateTreasureContent(thing, lv, type, clearContent: true);
		return thing;
	}

	public static void CreateTreasureContent(Thing t, int lv, TreasureType type, bool clearContent)
	{
		int miracleChance = lv;
		int num = EClass.curve(lv, 20, 15);
		bool flag = true;
		bool flag2 = true;
		int guaranteedMythical = ((type == TreasureType.BossQuest) ? 1 : 0);
		ChangeSeed();
		t.AddEditorTag(EditorTag.PreciousContainer);
		if (clearContent)
		{
			t.things.DestroyAll();
		}
		switch (type)
		{
		case TreasureType.Map:
			miracleChance += 50;
			t.Add("money", EClass.rndHalf(1000 + num * 100));
			t.Add("money2", EClass.rndHalf(Mathf.Min(3 + num / 10, 10)));
			t.Add("medal", EClass.rnd(3));
			break;
		case TreasureType.BossNefia:
		case TreasureType.BossQuest:
			t.Add("money", EClass.rndHalf(500 + num * 50));
			t.Add("plat", EClass.rndHalf(Mathf.Min(3 + num / 10, 15)));
			t.Add("rp_random", 1, lv);
			if (EClass.rnd(5) == 0)
			{
				t.Add("medal", EClass.rnd(3));
			}
			else if (EClass.rnd(3) == 0)
			{
				t.Add("map_treasure", 1, EClass.rndHalf(lv + 10));
			}
			else
			{
				t.Add("book_skill", 1, lv);
			}
			t.c_lockLv /= 2;
			break;
		case TreasureType.SurvivalRaid:
			t.Add("plat", EClass.rndHalf(Mathf.Min(3 + num / 10, 15)));
			t.Add("money2", EClass.rndHalf(Mathf.Min(3 + num / 10, 10)));
			t.Add("medal", EClass.rndHalf(8));
			t.Add("book_skill", 1, lv);
			t.Add("book_skill", 1, lv);
			t.c_lockLv /= 3;
			break;
		case TreasureType.RandomChest:
			flag2 = false;
			miracleChance /= 2;
			if (miracleChance > 50)
			{
				miracleChance = 50;
			}
			if (EClass.rnd(2) == 0)
			{
				t.Add("money", EClass.rndHalf(10 + num * 25));
			}
			else if (EClass.rnd(2) == 0)
			{
				t.Add("money2", 1 + EClass.rnd(Mathf.Min(2 + num / 25, 5)));
			}
			else
			{
				t.Add("plat", 1 + EClass.rnd(Mathf.Min(2 + num / 25, 5)));
			}
			if (EClass._zone is Zone_Civilized)
			{
				flag = false;
				break;
			}
			if (EClass.rnd(10) == 0)
			{
				t.Add("map_treasure", 1, EClass.rndHalf(lv + 10));
			}
			if (EClass.rnd(3) == 0)
			{
				t.AddCard(Create("rp_random", -1, lv));
			}
			if (t.c_lockLv > 0 && EClass.rnd(3) == 0)
			{
				t.Add("medal");
			}
			break;
		}
		if (type == TreasureType.RandomChest && EClass.rnd(2) == 0)
		{
			t.AddCard(CreateFromCategory("junk", lv));
		}
		else if (EClass.rnd(3) == 0)
		{
			t.Add("rp_random", 1, lv);
		}
		else if (type != TreasureType.Map && EClass.rnd(2) == 0)
		{
			Thing thing = CreateFromCategory("furniture", lv);
			if (!thing.IsContainer && (t.c_lockLv == 0 || thing.SelfWeight < 10000))
			{
				t.AddCard(thing);
			}
		}
		else
		{
			t.AddCard(CreateFromCategory("junk", lv));
		}
		if (EClass.rnd(3) == 0)
		{
			t.Add("book_ancient", 1, lv);
		}
		if (t.c_lockLv > 0)
		{
			miracleChance += 20 + (int)Mathf.Sqrt(t.c_lockLv * 5);
		}
		if (flag)
		{
			SetRarity(100);
			t.AddCard(CreateFromFilter("eq", lv));
			if (flag2)
			{
				SetRarity(20);
				t.AddCard(CreateFromFilter("eq", lv));
			}
		}
		Rand.SetSeed();
		static void ChangeSeed()
		{
			EClass.player.seedChest++;
			Rand.SetSeed(EClass.game.seed + EClass.player.seedChest);
		}
		void SetRarity(int mtp)
		{
			ChangeSeed();
			Rarity rarity = ((miracleChance * mtp / 100 < EClass.rnd(100)) ? Rarity.Superior : ((EClass.rnd(20) == 0) ? Rarity.Mythical : Rarity.Legendary));
			if (guaranteedMythical > 0)
			{
				guaranteedMythical--;
				rarity = Rarity.Mythical;
			}
			CardBlueprint.Set(new CardBlueprint
			{
				rarity = rarity
			});
		}
	}

	public static void TryLickChest(Thing chest)
	{
		foreach (Chara chara in EClass._map.charas)
		{
			if (!chara.HasElement(1412) || chara.Dist(chest) >= 5)
			{
				continue;
			}
			chara.Say("lick", chara, chest);
			chest.PlaySound("offering");
			chest.PlayEffect("mutation");
			{
				foreach (Thing thing in chest.things)
				{
					thing.TryLickEnchant(chara, msg: false);
				}
				break;
			}
		}
	}
}
