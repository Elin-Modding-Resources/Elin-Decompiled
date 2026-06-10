using System.Collections.Generic;
using System.IO;

public class CustomGunEffectSetting : CustomFileContent
{
	public Dictionary<string, CustomGunEffectData> items;

	public static CustomGunEffectSetting CreateFromFile(FileInfo file, ModPackage owner = null)
	{
		if (owner == null)
		{
			owner = ModUtil.FindFileProviderPackage(file);
		}
		return new CustomGunEffectSetting
		{
			ContentId = "GunEffect/" + owner.id,
			Owner = owner,
			File = file
		};
	}

	protected override void LoadContent()
	{
		Dictionary<string, CustomGunEffectData> dictionary = IO.LoadFile<Dictionary<string, CustomGunEffectData>>(base.File.FullName, compress: false, GameIOContext.Settings);
		items = dictionary;
	}

	public override string ToString()
	{
		return $"{base.ContentId}/items({items.Count})";
	}
}
