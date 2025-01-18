using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class SourceThingV : SourceDataString<SourceThingV.Row>
{
	[Serializable]
	public class Row : SourceThing.Row
	{
		public string[] parse;

		public override bool UseAlias => false;

		public override string GetAlias => "n";
	}

	public override string[] ImportFields => new string[1] { "unit" };

	public override Row CreateRow()
	{
		return new Row
		{
			id = SourceData.GetString(0),
			_origin = SourceData.GetString(1),
			name_JP = SourceData.GetString(2),
			unit_JP = SourceData.GetString(3),
			name = SourceData.GetString(4),
			unit = SourceData.GetString(5),
			tiles = SourceData.GetIntArray(6),
			skins = SourceData.GetIntArray(7),
			parse = SourceData.GetStringArray(8),
			vals = SourceData.GetStringArray(9),
			trait = SourceData.GetStringArray(11),
			LV = SourceData.GetInt(12),
			chance = SourceData.GetInt(13),
			value = SourceData.GetInt(14),
			weight = SourceData.GetInt(15),
			recipeKey = SourceData.GetStringArray(16),
			factory = SourceData.GetStringArray(17),
			components = SourceData.GetStringArray(18),
			defMat = SourceData.GetString(19),
			category = SourceData.GetString(20),
			tag = SourceData.GetStringArray(21),
			detail_JP = SourceData.GetString(22),
			detail = SourceData.GetString(23)
		};
	}

	public override void SetRow(Row r)
	{
		map[r.id] = r;
	}

	public override void RestorePref()
	{
		foreach (SourceThing.Row row2 in EClass.sources.things.rows)
		{
			SourcePref sourcePref = EClass.sources.things._rows.TryGetValue(row2.id)?.pref;
			if (sourcePref == null && EClass.sources.asset.renames.ContainsKey(row2.id))
			{
				sourcePref = EClass.sources.things._rows.TryGetValue(EClass.sources.asset.renames[row2.id])?.pref;
			}
			row2.pref = sourcePref ?? new SourcePref();
		}
		Dictionary<string, SourceThing.Row> dictionary = new Dictionary<string, SourceThing.Row>();
		foreach (SourceThing.Row row3 in EClass.sources.things.rows)
		{
			dictionary[row3.id] = row3;
		}
		foreach (SourceThing.Row row4 in EClass.sources.things.rows)
		{
			if (!row4.pref.UsePref && !row4._origin.IsEmpty())
			{
				SourceThing.Row row = dictionary[row4._origin];
				row4.pref = IO.DeepCopy(row.pref);
				row4.pref.flags |= PrefFlag.UsePref;
			}
		}
	}

	public override void ValidatePref()
	{
		foreach (SourceThing.Row row in EClass.sources.things.rows)
		{
			row.pref.Validate();
		}
	}

	public override void Reset()
	{
		base.Reset();
		EClass.sources.things.Reset();
	}

	public override void OnAfterImportData()
	{
		Dictionary<string, SourceThing.Row> dictionary = new Dictionary<string, SourceThing.Row>();
		foreach (SourceThing.Row row2 in EClass.sources.things.rows)
		{
			dictionary[row2.id] = row2;
		}
		System.Reflection.FieldInfo[] fields = EClass.sources.things.rows[0].GetType().GetFields();
		foreach (Row row3 in rows)
		{
			SourceThing.Row row = new SourceThing.Row();
			SourceThing.Row o = dictionary[row3._origin];
			System.Reflection.FieldInfo[] array = fields;
			foreach (System.Reflection.FieldInfo fieldInfo in array)
			{
				if (!(fieldInfo.Name == "parse"))
				{
					row.SetField(fieldInfo.Name, o.GetField<object>(fieldInfo.Name));
				}
			}
			row.id = row3.id;
			row._origin = row3._origin;
			if (row3.LV != 0)
			{
				row.LV = row3.LV;
			}
			if (row3.chance != -1)
			{
				row.chance = row3.chance;
			}
			if (row3.value != -1)
			{
				row.value = row3.value;
			}
			else
			{
				row.value += EClass.rnd(row.value / 2);
			}
			if (row3.weight != -1)
			{
				row.weight = row3.weight;
			}
			if (!row3.tiles.IsEmpty())
			{
				row.tiles = row3.tiles;
			}
			if (!row3.skins.IsEmpty())
			{
				row.skins = row3.skins;
			}
			if (!row3.name.IsEmpty())
			{
				row.name = row3.name;
			}
			if (!row3.name_JP.IsEmpty())
			{
				row.name_JP = row3.name_JP;
			}
			if (!row3.detail.IsEmpty())
			{
				row.detail = row3.detail;
			}
			if (!row3.detail_JP.IsEmpty())
			{
				row.detail_JP = row3.detail_JP;
			}
			if (!row3.unit.IsEmpty())
			{
				row.unit = row3.unit;
			}
			if (!row3.unit_JP.IsEmpty())
			{
				row.unit_JP = row3.unit_JP;
			}
			if (!row3.vals.IsEmpty())
			{
				row.vals = row3.vals;
			}
			if (!row3.components.IsEmpty())
			{
				row.components = row3.components;
			}
			if (!row3.defMat.IsEmpty())
			{
				row.defMat = row3.defMat;
			}
			if (!row3.trait.IsEmpty())
			{
				row.trait = row3.trait;
			}
			if (!row3.category.IsEmpty())
			{
				row.category = row3.category;
			}
			if (!row3.factory.IsEmpty())
			{
				row.factory = row3.factory;
			}
			if (!row3.tag.IsEmpty())
			{
				row.tag = row3.tag;
			}
			row.recipeKey = row3.recipeKey;
			if (!row3.parse.IsEmpty())
			{
				switch (row3._origin)
				{
				case "lamp_ceil2":
				case "window":
				case "windowL":
					row.idExtra = row3.parse[0];
					break;
				default:
				{
					string[] parse = row3.parse;
					for (int i = 0; i < parse.Length; i++)
					{
						string[] array2 = parse[i].Split('/');
						switch (array2[0])
						{
						case "render":
							row._idRenderData = array2[1];
							break;
						case "tiletype":
							row._tileType = array2[1];
							break;
						case "anime":
							row.anime = ((array2.Length <= 4) ? ((array2.Length <= 3) ? new int[2]
							{
								array2[1].ToInt(),
								array2[2].ToInt()
							} : new int[3]
							{
								array2[1].ToInt(),
								array2[2].ToInt(),
								array2[3].ToInt()
							}) : new int[4]
							{
								array2[1].ToInt(),
								array2[2].ToInt(),
								array2[3].ToInt(),
								array2[4].ToInt()
							});
							break;
						case "skin":
							row.skins = new int[1] { array2[1].ToInt() };
							break;
						case "alt":
							row.altTiles = new int[1] { array2[1].ToInt() };
							row.ignoreAltFix = true;
							break;
						case "naming":
							row.naming = array2[1];
							break;
						case "ex":
							row.idActorEx = array2[1];
							break;
						case "sound":
							row.idSound = array2[1];
							break;
						case "color":
							row.colorMod = 100;
							break;
						case "no_color":
							row.colorMod = 0;
							break;
						case "unique":
							row.quality = 4;
							break;
						case "ele":
						{
							int[] second = new int[2]
							{
								Core.GetCurrent().sources.elements.alias[array2[1]].id,
								array2[2].ToInt()
							};
							row.elements = row.elements.Concat(second).ToArray();
							break;
						}
						}
					}
					break;
				}
				}
			}
			OnImportRow(row3, row);
			row.OnImportData(EClass.sources.things);
			EClass.sources.things.rows.Add(row);
		}
		rows.Clear();
	}

	public virtual void OnImportRow(Row _r, SourceThing.Row c)
	{
	}
}
