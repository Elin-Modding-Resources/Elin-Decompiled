using Newtonsoft.Json;
using UnityEngine;

[JsonObject(MemberSerialization.OptOut)]
public class CustomZoneContent : CustomSourceContent
{
	public string parent;

	public bool autoSpawn;

	public override string SourceType => "SourceZone";

	public static CustomZoneContent CreateFromRow(SourceZone.Row r, ModPackage owner = null)
	{
		if (owner == null)
		{
			owner = ModUtil.FindSourceRowPackage(r);
		}
		CustomZoneContent customZoneContent = new CustomZoneContent
		{
			ContentId = "Zone/" + r.id,
			SourceId = r.id,
			Owner = owner
		};
		string[] tag = r.tag;
		for (int i = 0; i < tag.Length; i++)
		{
			var (text, str, _) = CustomSourceContent.GetParams(tag[i]);
			if (text == "addMap")
			{
				customZoneContent.parent = str.IsEmpty(r.parent.IsEmpty("ntyris"));
				customZoneContent.autoSpawn = true;
			}
		}
		return customZoneContent;
	}

	public override void OnGameLoad(GameIOContext context)
	{
		if (!autoSpawn || parent == null)
		{
			return;
		}
		Debug.Log($"#mod-content loading {this}");
		if (EClass.game.spatials.Find(base.SourceId) != null)
		{
			Debug.Log("#mod-content skipping existing custom zone " + base.SourceId);
			return;
		}
		Zone zone = ModUtil.FindZoneByFullName(parent);
		if (zone != null)
		{
			Spatial arg = SpatialGen.Create(base.SourceId, zone, register: true);
			Debug.Log($"#mod-content spawned custom zone {arg}");
			return;
		}
		ModUtil.LogModError("source zone row '" + base.SourceId + "' has invalid addMap parent '" + parent + "'", base.Owner);
	}

	public override string ToString()
	{
		return base.ContentId + "/" + parent;
	}
}
