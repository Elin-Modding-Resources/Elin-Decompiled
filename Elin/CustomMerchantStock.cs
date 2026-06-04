using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

[JsonObject(MemberSerialization.OptOut)]
public class CustomMerchantStock : CustomFileContent
{
	public List<CustomThingContent> items = new List<CustomThingContent>();

	public List<Thing> Generate(Card owner)
	{
		Load();
		List<Thing> list = new List<Thing>();
		int createLv = owner?.trait?.ShopLv ?? (-1);
		foreach (CustomThingContent item2 in items)
		{
			try
			{
				Thing item = item2.Create(createLv);
				list.Add(item);
			}
			catch (Exception ex)
			{
				ModUtil.LogModError("can't create stock item '" + item2.ContentId + "'\n" + ex.Message, base.Owner);
				Debug.LogError(ex);
			}
		}
		return list;
	}

	public static CustomMerchantStock CreateFromId(string stockId, ModPackage owner)
	{
		var (fileInfo, eMod) = PackageIterator.GetFilesEx("Data/stock_" + stockId + ".json").LastOrDefault();
		if (fileInfo == null)
		{
			return null;
		}
		if (owner == null)
		{
			owner = eMod as ModPackage;
		}
		return new CustomMerchantStock
		{
			ContentId = "MerchantStock/" + stockId,
			Owner = owner,
			File = fileInfo
		};
	}

	protected override void LoadContent()
	{
		CustomMerchantStock customMerchantStock = IO.LoadFile<CustomMerchantStock>(base.File.FullName);
		items = customMerchantStock.items;
	}

	public override string ToString()
	{
		return $"{base.ContentId}/items({items.Count})";
	}
}
