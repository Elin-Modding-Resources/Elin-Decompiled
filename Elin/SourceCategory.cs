using System;
using System.Collections.Generic;

public class SourceCategory : SourceDataString<SourceCategory.Row>
{
	[Serializable]
	public class Row : BaseRow
	{
		public string id;

		public int uid;

		public string name_JP;

		public string name;

		public string _parent;

		public string recipeCat;

		public int slot;

		public int skill;

		public int maxStack;

		public int tileDummy;

		public bool installOne;

		public int ignoreBless;

		public string[] tag;

		public string idThing;

		public string[] recycle;

		public int costSP;

		public int gift;

		public int deliver;

		public int offer;

		public int ticket;

		public int sortVal;

		public int flag;

		[NonSerialized]
		public Row parent;

		[NonSerialized]
		public List<Row> children;

		[NonSerialized]
		public string name_L;

		public override bool UseAlias => false;

		public override string GetAlias => "n";

		public bool IsChildOf(string id)
		{
			return IsChildOf(EClass.sources.categories.map[id]);
		}

		public bool IsChildOf(Row r)
		{
			if (r == this)
			{
				return true;
			}
			if (parent != null)
			{
				return parent.IsChildOf(r);
			}
			return false;
		}

		public bool IsChildOf(int _uid)
		{
			if (uid == _uid)
			{
				return true;
			}
			if (parent != null)
			{
				return parent.IsChildOf(_uid);
			}
			return false;
		}

		public bool Contatin(int _uid)
		{
			if (uid == _uid)
			{
				return true;
			}
			foreach (Row child in children)
			{
				if (child.Contatin(_uid))
				{
					return true;
				}
			}
			return false;
		}

		public Row GetRoot()
		{
			if (parent == null)
			{
				return this;
			}
			return parent.GetRoot();
		}

		public Row GetSecondRoot()
		{
			if (parent == null || parent.parent == null)
			{
				return this;
			}
			return parent.GetSecondRoot();
		}

		public string GetIdThing()
		{
			if (!idThing.IsEmpty())
			{
				return idThing;
			}
			return parent.GetIdThing();
		}
	}

	public override Row CreateRow()
	{
		return new Row
		{
			id = SourceData.GetString(0),
			uid = SourceData.GetInt(1),
			name_JP = SourceData.GetString(2),
			name = SourceData.GetString(3),
			_parent = SourceData.GetString(4),
			recipeCat = SourceData.GetString(5),
			slot = Core.GetElement(SourceData.GetStr(6)),
			skill = Core.GetElement(SourceData.GetStr(7)),
			maxStack = SourceData.GetInt(8),
			tileDummy = SourceData.GetInt(9),
			installOne = SourceData.GetBool(10),
			ignoreBless = SourceData.GetInt(11),
			tag = SourceData.GetStringArray(12),
			idThing = SourceData.GetString(13),
			recycle = SourceData.GetStringArray(14),
			costSP = SourceData.GetInt(15),
			gift = SourceData.GetInt(16),
			deliver = SourceData.GetInt(17),
			offer = SourceData.GetInt(18),
			ticket = SourceData.GetInt(19),
			sortVal = SourceData.GetInt(20),
			flag = SourceData.GetInt(21)
		};
	}

	public override void SetRow(Row r)
	{
		map[r.id] = r;
	}

	public override void OnInit()
	{
		foreach (Row row in rows)
		{
			row.children = new List<Row>();
		}
		foreach (Row row2 in rows)
		{
			if (!row2._parent.IsEmpty())
			{
				(row2.parent = map[row2._parent]).children.Add(row2);
			}
		}
	}
}
