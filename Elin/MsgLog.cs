using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;

public class MsgLog : EClass
{
	public class Data : EClass
	{
		[JsonProperty]
		public string text;

		[JsonProperty]
		public string col;

		[JsonProperty]
		public Date date;
	}

	[JsonProperty]
	public Dictionary<int, Data> dict = new Dictionary<int, Data>();

	[JsonProperty]
	public int currentLogIndex;

	[JsonProperty]
	public string id;

	public int maxLog
	{
		get
		{
			if (id == "chronicle")
			{
				return 9999;
			}
			return 50;
		}
	}

	[OnSerializing]
	private void OnSerializing(StreamingContext context)
	{
		foreach (int item in dict.Keys.ToList())
		{
			if (item >= currentLogIndex || item <= currentLogIndex - maxLog)
			{
				dict.Remove(item);
			}
		}
	}

	public void Add(Data data)
	{
		dict.Add(currentLogIndex, data);
		currentLogIndex++;
		if (currentLogIndex >= maxLog)
		{
			dict.Remove(currentLogIndex - maxLog);
		}
	}

	public Data Add(string text, FontColor c)
	{
		return Add(text, c.ToString());
	}

	public Data Add(string text, string col = null)
	{
		Data data = new Data
		{
			text = text,
			col = col
		};
		data.date = (VirtualDate.current ?? EClass.world.date).Copy();
		Add(data);
		return data;
	}

	public List<Data> GetList(bool reverse = false)
	{
		List<Data> list = new List<Data>();
		foreach (Data value in dict.Values)
		{
			list.Add(value);
		}
		list.Sort((Data a, Data b) => a.date.GetRaw() - b.date.GetRaw());
		if (reverse)
		{
			list.Reverse();
		}
		return list;
	}
}
