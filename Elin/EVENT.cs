using System.Collections.Generic;

public static class EVENT
{
	public class ElinEventArgs<T>
	{
		public bool IsUsed { get; private set; }

		public T data { get; private set; }

		public virtual void SetData(T newData)
		{
			data = newData;
		}

		public virtual void Use()
		{
			IsUsed = true;
		}
	}

	public class ElinFeatApplyEventArgs : ElinEventArgs<int>
	{
		public Feat feat;

		public ElementContainer owner;

		public bool hint;
	}

	public class ElinDramaParseActionEventArgs : ElinEventArgs<string>
	{
		public DramaManager dm;

		public Dictionary<string, string> line;
	}

	public const string Undefined = "undefined";

	public const string ModsActivated = "elin.mods.activated";

	public const string SourceImporting = "elin.source.importing";

	public const string SourceImported = "elin.source.imported";

	public const string SourceLangSet = "elin.source.lang_set";

	public const string PreSave = "elin.game.pre_save";

	public const string PostSave = "elin.game.post_save";

	public const string PreLoad = "elin.game.pre_load";

	public const string PostLoad = "elin.game.post_load";

	public const string NewGame = "elin.game.start_new";

	public const string PreSceneInit = "elin.scene.pre_init";

	public const string PostSceneInit = "elin.scene.post_init";

	public const string FeatApply = "elin.feat.apply";

	public const string DramaParseAction = "elin.drama.parse_action";

	public const string CharaCreated = "elin.chara_created";

	public const string ThingCreated = "elin.thing_created";
}
