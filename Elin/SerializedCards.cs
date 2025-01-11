using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class SerializedCards : EClass
{
	public class Data : EClass
	{
		[JsonProperty]
		public int[] ints = new int[30];

		[JsonProperty]
		public string[] strs = new string[10];

		[JsonProperty]
		public Dictionary<int, object> obj;

		[JsonProperty]
		public Dictionary<int, string> cstr;

		public BitArray32 _bits1;

		public int idDyeMat
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

		public int idV
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

		public int idMat
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

		public int x
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

		public int z
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

		public int dir
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

		public int placeState
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

		public int altitude
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

		public int fx
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

		public int fy
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

		public int lightColor
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

		public int bits1
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

		public int tile
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

		public int refVal
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

		public int idSkin
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

		public int idBacker
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

		public int bits2
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

		public int uidEditor
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

		public int lv
		{
			get
			{
				return ints[20];
			}
			set
			{
				ints[20] = value;
			}
		}

		public int element
		{
			get
			{
				return ints[21];
			}
			set
			{
				ints[21] = value;
			}
		}

		public string id
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

		public string idEditor
		{
			get
			{
				return strs[1];
			}
			set
			{
				strs[1] = value;
			}
		}

		public string idTrait
		{
			get
			{
				return strs[2];
			}
			set
			{
				strs[2] = value;
			}
		}

		public string tags
		{
			get
			{
				return strs[3];
			}
			set
			{
				strs[3] = value;
			}
		}

		public string idRender
		{
			get
			{
				return strs[4];
			}
			set
			{
				strs[4] = value;
			}
		}

		public string traitVals
		{
			get
			{
				return strs[5];
			}
			set
			{
				strs[5] = value;
			}
		}

		public string idRefCard
		{
			get
			{
				return strs[6];
			}
			set
			{
				strs[6] = value;
			}
		}

		public string idDeity
		{
			get
			{
				return strs[7];
			}
			set
			{
				strs[7] = value;
			}
		}

		public bool isDead
		{
			get
			{
				return _bits1[0];
			}
			set
			{
				_bits1[0] = value;
			}
		}

		public bool isEmpty
		{
			get
			{
				return _bits1[1];
			}
			set
			{
				_bits1[1] = value;
			}
		}
	}

	[JsonProperty]
	public List<Data> cards = new List<Data>();

	[JsonProperty]
	public int version = 2;

	public List<Card> importedCards = new List<Card>();

	public void Add(Card c)
	{
		Data data = new Data
		{
			id = c.id,
			idEditor = c.c_idEditor,
			idRefCard = c.c_idRefCard,
			idTrait = c.c_idTrait,
			tags = c.c_editorTags,
			traitVals = c.c_editorTraitVal,
			obj = c.mapObj,
			idMat = c.material.id,
			x = c.pos.x,
			z = c.pos.z,
			placeState = (int)c.placeState,
			dir = c.dir,
			altitude = c.altitude,
			fx = (int)(c.fx * 1000f),
			fy = (int)(c.fy * 1000f),
			lightColor = c.c_lightColor,
			bits1 = c._bits1.ToInt(),
			bits2 = c._bits2.ToInt(),
			tile = c.sourceCard.tiles[0],
			idRender = c.sourceCard.idRenderData,
			refVal = c.refVal,
			idSkin = c.idSkin,
			idDeity = c.c_idDeity,
			isEmpty = (c.things.Count == 0)
		};
		if (c.c_idBacker != 0)
		{
			SourceBacker.Row row = EClass.sources.backers.map.TryGetValue(c.c_idBacker);
			if (row != null && row.isStatic != 0)
			{
				data.idBacker = c.c_idBacker;
			}
		}
		if (c.material.id == c.DefaultMaterial.id)
		{
			data.idMat = -1;
		}
		data.idDyeMat = (c.isDyed ? c.c_dyeMat : (-1));
		if (c.isChara)
		{
			Point orgPos = c.Chara.orgPos;
			if (orgPos != null)
			{
				data.x = orgPos.x;
				data.z = orgPos.z;
			}
			if (c.Chara.isDead)
			{
				data.isDead = true;
			}
			if (c.Chara.uidEditor == 0)
			{
				int num = 1;
				foreach (Chara item in EClass._map.charas.Concat(EClass._map.deadCharas))
				{
					if (item.uidEditor >= num)
					{
						num = item.uidEditor + 1;
					}
				}
				c.Chara.uidEditor = num;
			}
			data.lv = c.LV;
			data.element = c.c_idMainElement;
			data.uidEditor = c.Chara.uidEditor;
		}
		TryAddStr(52);
		cards.Add(data);
		data.ints[0] = data._bits1.ToInt();
		void TryAddStr(int key)
		{
			string str = c.GetStr(key);
			if (!str.IsEmpty())
			{
				if (data.cstr == null)
				{
					data.cstr = new Dictionary<int, string>();
				}
				data.cstr[key] = str;
			}
		}
	}

	public void Restore(Map map, Map orgMap, bool addToZone, PartialMap partial = null)
	{
		List<Thing> things = map.things;
		List<Chara> serializedCharas = map.serializedCharas;
		bool isUserZone = EClass._zone.IsUserZone;
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		Dictionary<int, int> dictionary2 = new Dictionary<int, int>();
		importedCards.Clear();
		foreach (Data card4 in cards)
		{
			int num = card4.dir;
			Point point = new Point(card4.x, card4.z);
			int index = point.index;
			if (partial != null)
			{
				if (partial.result.ruined.Contains(point.index))
				{
					continue;
				}
				num -= partial.dir;
				int num2 = point.x - partial.offsetX;
				int num3 = point.z - partial.offsetZ;
				switch (partial.dir)
				{
				case 1:
				{
					int num5 = num2;
					num2 = num3;
					num3 = -num5;
					break;
				}
				case 2:
					num2 = -num2;
					num3 = -num3;
					break;
				case 3:
				{
					int num4 = num2;
					num2 = -num3;
					num3 = num4;
					break;
				}
				}
				point.x = num2 + partial.destX;
				point.z = num3 + partial.destZ;
				if (!partial.validPoints.Contains(point.index) || !point.IsValid)
				{
					continue;
				}
				if (!partial.editMode)
				{
					Card card = null;
					string id = card4.id;
					if (!(id == "sign_spawnThing"))
					{
						if (id == "sign_spawnChara")
						{
							card = CharaGen.CreateFromFilter(card4.idRefCard.IsEmpty(EClass._zone.biome.spawn.GetRandomCharaId()), EClass._zone.DangerLv);
						}
					}
					else
					{
						card = ThingGen.CreateFromFilter(card4.idRefCard.IsEmpty(EClass._zone.biome.spawn.GetRandomThingId()), EClass._zone.DangerLv);
					}
					if (card != null)
					{
						card.pos = point;
						EClass._zone.AddCard(card, card.pos);
						if (card.trait.IsDoor)
						{
							EClass._map.OnSetBlockOrDoor(card.pos.x, card.pos.z);
						}
						continue;
					}
				}
			}
			card4._bits1.SetInt(card4.ints[0]);
			string text = card4.id;
			if (card4.idV != 0)
			{
				text = card4.idV.ToString() ?? "";
			}
			if (addToZone && partial != null && !partial.editMode)
			{
				string id = card4.id;
				if (!(id == "editor_torch"))
				{
					if (id == "editor_torch_wall")
					{
						text = EClass._zone.biome.style.GetIdLight(wall: true);
					}
				}
				else
				{
					text = EClass._zone.biome.style.GetIdLight(wall: false);
				}
			}
			CardRow source = EClass.sources.cards.map.TryGetValue(text);
			if (source == null)
			{
				continue;
			}
			Card card2 = null;
			if (source.isChara)
			{
				if (orgMap != null)
				{
					bool flag = false;
					if (source.quality >= 4)
					{
						foreach (Chara value2 in EClass.game.cards.globalCharas.Values)
						{
							if (value2.id == text)
							{
								flag = true;
								break;
							}
						}
					}
					if (!flag)
					{
						foreach (Chara item in orgMap.charas.Concat(orgMap.serializedCharas).Concat(orgMap.deadCharas))
						{
							if (source.quality >= 4 && item.id == text)
							{
								flag = true;
								break;
							}
							if (item.id == text && item.orgPos != null && item.orgPos.Equals(point))
							{
								flag = true;
								break;
							}
						}
					}
					if (flag)
					{
						continue;
					}
				}
				if (isUserZone && ClassExtension.TryGetValue<int, int>((IDictionary<int, int>)dictionary, index, 0) >= 3)
				{
					continue;
				}
				card2 = CharaGen.Create(text);
				if (card4.ints.Length > 20)
				{
					card2.Chara.SetLv(card4.lv);
					if (card4.element != 0)
					{
						card2.Chara.SetMainElement(card4.element);
					}
				}
				card2.Chara.orgPos = point.Copy();
				if (card4.isDead)
				{
					card2.hp = -1;
					card2.Chara.isDead = true;
				}
				card2.Chara.hunger.value = EClass.rnd(EClass.rnd(20) + 1);
				if (!addToZone)
				{
					serializedCharas.Add(card2.Chara);
				}
				if (isUserZone)
				{
					dictionary[index] = ClassExtension.TryGetValue<int, int>((IDictionary<int, int>)dictionary, index, 0) + 1;
				}
			}
			else
			{
				PlaceState placeState = card4.placeState.ToEnum<PlaceState>();
				if (isUserZone && ((ClassExtension.TryGetValue<int, int>((IDictionary<int, int>)dictionary2, index, 0) >= 20 && text != "waystone" && text != "core_zone") || (placeState != PlaceState.installed && !card4.bits1.IsOn(13)) || text == "medal"))
				{
					continue;
				}
				if (source.isOrigin)
				{
					text = SpawnListThing.Get("origin_" + text, (SourceThing.Row a) => a.origin == source).GetFirst().id;
				}
				card2 = ThingGen.Create(text, -1, EClass._zone.DangerLv);
				card2.ChangeMaterial((card4.idMat == -1) ? card2.DefaultMaterial.id : card4.idMat);
				if (!addToZone)
				{
					things.Add(card2.Thing);
				}
				card2.altitude = card4.altitude;
				card2.placeState = placeState;
				card2.c_lightColor = card4.lightColor;
				if (isUserZone)
				{
					dictionary2[index] = ClassExtension.TryGetValue<int, int>((IDictionary<int, int>)dictionary2, index, 0) + 1;
				}
			}
			if (num < 0)
			{
				num = Mathf.Abs(card2.sourceCard.tiles.Length + num);
			}
			card2.pos = point;
			card2.dir = num;
			card2._bits1.SetInt(card4.bits1);
			card2._bits2.SetInt(card4.bits2);
			card2.isPlayerCreation = true;
			card2.autoRefuel = true;
			card2.c_editorTraitVal = card4.traitVals;
			card2.c_idRefCard = card4.idRefCard;
			card2.isImported = true;
			card2.refVal = card4.refVal;
			card2.idSkin = card4.idSkin;
			card2.c_idDeity = card4.idDeity;
			if (card4.idBacker != 0)
			{
				Debug.Log(card4.idBacker);
				card2.c_idBacker = card4.idBacker;
			}
			if (version >= 2 && card4.idDyeMat != -1)
			{
				card2.Dye(EClass.sources.materials.rows[card4.idDyeMat]);
			}
			card2.mapObj = card4.obj;
			if (card2.mapObj?.TryGetValue(2) != null)
			{
				card2.mapObj.Remove(2);
			}
			if (card4.cstr != null)
			{
				foreach (KeyValuePair<int, string> item2 in card4.cstr)
				{
					card2.SetStr(item2.Key, item2.Value);
				}
			}
			if (card2.freePos)
			{
				card2.fx = (float)card4.fx * 0.001f;
				card2.fy = (float)card4.fy * 0.001f;
			}
			if (!card4.idEditor.IsEmpty())
			{
				card2.c_idEditor = card4.idEditor;
			}
			if (!card4.idTrait.IsEmpty())
			{
				card2.c_idTrait = card4.idTrait;
				card2.ApplyTrait();
				card2.trait.OnCreate(EClass._zone.lv);
			}
			if (!card4.tags.IsEmpty())
			{
				card2.c_editorTags = card4.tags;
				try
				{
					string[] array = card4.tags.Split(',');
					foreach (string value in array)
					{
						card2.ApplyEditorTags(value.ToEnum<EditorTag>());
					}
				}
				catch
				{
					Debug.LogWarning("Could not convert editor tag:" + card2.Name + "/" + card4.tags);
				}
			}
			if (isUserZone)
			{
				if ((card2.isHidden || card2.isMasked) && ((card2.TileType.IsBlockPass && card2.IsInstalled) || card2.trait is TraitCoreZone || card2.trait is TraitWaystone))
				{
					Card card3 = card2;
					bool isHidden = (card2.isMasked = false);
					card3.isHidden = isHidden;
				}
				if (card2.IsContainer && card4.isEmpty)
				{
					card2.things.DestroyAll();
				}
			}
			if (card2.isChara)
			{
				card2.Chara.homeZone = EClass._zone;
				card2.Chara.uidEditor = card4.uidEditor;
				if (card2.isBackerContent)
				{
					card2.ApplyBacker(card2.c_idBacker);
				}
				card2.SetObj(5, null);
			}
			if (addToZone)
			{
				EClass._zone.AddCard(card2, card2.pos);
				if (card2.trait.IsDoor)
				{
					EClass._map.OnSetBlockOrDoor(card2.pos.x, card2.pos.z);
				}
				if (partial != null && !card2.sourceCard.lightData.IsEmpty())
				{
					partial.result.hasLight = true;
				}
			}
			importedCards.Add(card2);
		}
		foreach (Card importedCard in importedCards)
		{
			if (importedCard.trait is TraitShackle)
			{
				foreach (Card importedCard2 in importedCards)
				{
					if (importedCard2.isRestrained && importedCard2.pos.Equals(importedCard.pos))
					{
						importedCard.c_uidRefCard = importedCard2.uid;
					}
				}
			}
			importedCard.trait.OnImportMap();
		}
	}
}
