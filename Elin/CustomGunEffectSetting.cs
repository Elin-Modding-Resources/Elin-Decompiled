using System.Collections.Generic;
using System.IO;

public class CustomGunEffectSetting : CustomFileContent
{
	public Dictionary<string, CustomGunEffectData> items = new Dictionary<string, CustomGunEffectData>();

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
		Dictionary<string, CustomGunEffectData> dictionary = IO.LoadFile<Dictionary<string, CustomGunEffectData>>(base.File.FullName, compress: false, CustomGunEffectData.JsonSettings);
		items = dictionary ?? new Dictionary<string, CustomGunEffectData>();
	}

	public override string ToString()
	{
		return $"{base.ContentId}/items({items.Count})";
	}
}
