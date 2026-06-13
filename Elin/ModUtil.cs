using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Newtonsoft.Json;
using ReflexCLI;
using ReflexCLI.Attributes;
using UnityEngine;
using UnityEngine.Networking;

[ConsoleCommandClassCustomizer("Mod")]
public class ModUtil : EClass
{
	public static Dictionary<string, string> fallbackTypes = new Dictionary<string, string>();

	private static readonly Dictionary<string, ICustomContent> _customContent = new Dictionary<string, ICustomContent>();

	private static readonly List<Func<string, object, int>> _customCalcEvaluator = new List<Func<string, object, int>>();

	private static readonly HashSet<Type> _checkedAttributedTypes = new HashSet<Type>();

	private static readonly Dictionary<string, Texture2D> _cachedTextures = new Dictionary<string, Texture2D>();

	private static Effect _rodTemplate;

	public static SourceImporter sourceImporter = new SourceImporter(SourceMapping);

	public static readonly List<ContextMenuProxy> contextMenuProxies = new List<ContextMenuProxy>();

	public static IReadOnlyDictionary<string, SourceData> SourceMapping => (from f in typeof(SourceManager).GetFields()
		where typeof(SourceData).IsAssignableFrom(f.FieldType)
		select f).ToDictionary((FieldInfo f) => f.FieldType.Name, (FieldInfo f) => f.GetValue(EClass.sources) as SourceData);

	public static void LoadTypeFallback()
	{
		string text = "type_resolver.txt";
		string[] array = Array.Empty<string>();
		if (File.Exists(CorePath.RootData + text))
		{
			array = IO.LoadTextArray(CorePath.RootData + text);
		}
		else
		{
			array = new string[2] { "TrueArena,ArenaWaveEvent,ZoneEvent", "Elin-GeneRecombinator,Elin_GeneRecombinator.IncubationSacrifice,Chara" };
			IO.SaveTextArray(CorePath.RootData + text, array);
		}
		string[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			string[] array3 = array2[i].Split(',');
			if (array3.Length >= 2)
			{
				RegisterSerializedTypeFallback(array3[0], array3[1], array3[2]);
			}
		}
	}

	public static void RegisterSerializedTypeFallback(string nameAssembly, string nameType, string nameFallbackType)
	{
		fallbackTypes[nameType] = nameFallbackType;
	}

	public static void LogModError(string message, BaseModPackage package = null)
	{
		string text = "#mod/" + package?.title + " (" + package?.id + ")\n" + message;
		UnityEngine.Debug.LogWarning(text.RemoveAllTags());
		if ((package?.isInPackages ?? false) || Application.isEditor)
		{
			EGui.CreatePopup(text);
		}
	}

	public static void LogModError(string message, SourceData.BaseRow row)
	{
		LogModError(message, FindSourceRowPackage(row));
	}

	public static void LogModError(string message, Type type)
	{
		LogModError(message, FindFileProviderPackage(new FileInfo(type.Assembly.Location)));
	}

	public static void LogModError(string message, FileInfo file)
	{
		LogModError(message, FindFileProviderPackage(file));
	}

	public static void LogModError(string message, DirectoryInfo dir)
	{
		LogModError(message, FindDirectoryProviderPackage(dir));
	}

	public static ModPackage GetModPackage(string modId)
	{
		return ModManagerCore.Instance.MappedPackages.GetValueOrDefault(modId) as ModPackage;
	}

	public static ModPackage FindFileProviderPackage(FileInfo file)
	{
		string path = file.FullName.NormalizePath();
		return ModManager.Instance.packages.LastOrDefault((BaseModPackage p) => path.StartsWith(p.dirInfo.FullName.NormalizePath())) as ModPackage;
	}

	public static ModPackage FindDirectoryProviderPackage(DirectoryInfo dir)
	{
		string path = dir.FullName.NormalizePath();
		return ModManager.Instance.packages.LastOrDefault((BaseModPackage p) => path.StartsWith(p.dirInfo.FullName.NormalizePath())) as ModPackage;
	}

	public static ModPackage FindSourceRowPackage(SourceData.BaseRow row)
	{
		return ModManagerCore.Instance.packages.OfType<ModPackage>().LastOrDefault((ModPackage p) => p.sourceRows.Contains(row));
	}

	public static Zone FindZoneByFullName(string zoneFullName = "ntyris/0", bool useRandomFallback = false)
	{
		if (zoneFullName.IsEmpty())
		{
			return null;
		}
		zoneFullName = zoneFullName.Replace('/', '@');
		Zone zone = EClass.game.spatials.Find((Zone z) => z.ZoneFullName == zoneFullName);
		if (zone != null)
		{
			return zone;
		}
		int num = zoneFullName.LastIndexOf('@');
		int destLv = 0;
		string zoneType;
		if (num > 0 && num < zoneFullName.Length - 1)
		{
			zoneType = zoneFullName[..num];
			destLv = zoneFullName[(num + 1)..].ToInt();
		}
		else
		{
			zoneType = zoneFullName.Replace("@", "");
		}
		string zoneId = zoneType.Replace("Zone_", "");
		zoneType = "Zone_" + zoneId;
		zone = EClass.game.spatials.Find((Zone z) => z.GetType().Name == zoneType || z.id == zoneId)?.FindOrCreateLevel(destLv);
		if (zone == null && (zoneId == "*" || useRandomFallback))
		{
			zone = (from z in EClass.game.spatials.map.Values.OfType<Zone>()
				where z.CanSpawnAdv
				select z).RandomItem();
		}
		return zone;
	}

	public static void OnModsActivated()
	{
		CommandRegistry.assemblies.Add(typeof(EScript).Assembly);
		SoundManager.current.soundLoaders.Add(LoadSoundData);
		UIBook.topicLoaders.Add(LoadTopicFiles);
		BookList.booklistLoaders.Add(LoadBookList);
		Lang.excelDialogLoaders.Add(LoadExcelDialog);
		LoadEffectTemplate();
		DynamicAsset<Effect>.assetLoaders.Add(LoadEffect);
		BaseModManager.SubscribeEvent<string>("elin.source.lang_set", OnSetLang);
		BaseModManager.SubscribeEvent<List<Religion>>("elin.religion_importing", OnReligionImporting);
		BaseModManager.SubscribeEvent("elin.source.importing", OnSourceImporting);
		BaseModManager.SubscribeEvent("elin.source.imported", OnSourceImported);
		BaseModManager.PublishEvent("elin.mods.activated");
		RegisterElinEventAttributes();
		CoroutineHelper.Deferred(RegisterElinEventAttributes);
	}

	private static void OnSourceImporting()
	{
		ExcelParser.allowTrimming = true;
		ImportAllModSourceSheets();
	}

	private static void OnSourceImported()
	{
		_customContent.Clear();
		foreach (EMod activatedUserMod in ModManager.Instance.ActivatedUserMods)
		{
			try
			{
				activatedUserMod.GenerateCustomContentProfiles();
				foreach (ICustomContent item in activatedUserMod.customContent)
				{
					AddContent(item);
				}
			}
			catch (Exception ex)
			{
				LogModError("exception while generating custom source profiles\n" + ex.Message, activatedUserMod);
				UnityEngine.Debug.LogException(ex);
			}
		}
		ImportAllGunEffectSettings();
		CustomReligionContent.managed.Clear();
		foreach (CustomReligionContent item2 in _customContent.Values.OfType<CustomReligionContent>())
		{
			item2.RegisterCustomReligion();
		}
		CustomReligionContent.Init();
		SourceCache.FinalizeCache();
		SourceCache.InvalidateCacheBlobs();
		SourceCache.ClearDetail();
		ExcelParser.ClearStaticRows();
		if (EClass.core.launchArgs.Contains("EXPORTSOURCE"))
		{
			string text = CorePath.rootExe + "/SourceExport/" + EClass.core.version.GetText();
			ExportAllSourceDataCsv(text);
			UnityEngine.Debug.Log("#source exported current version to " + text);
		}
	}

	[ElinPreLoad]
	private static void OnPreLoadInit(GameIOContext context)
	{
	}

	[ElinPostLoad]
	private static void OnPostLoadInit(GameIOContext context)
	{
		EClass.player.knownBGMs.RemoveWhere((int id) => !EClass.core.refs.dictBGM.ContainsKey(id));
		List<ICustomContent> contents = new List<ICustomContent>();
		CustomReligionContent.LoadReligionData(context);
		LoadCustomContent<CustomZoneContent>();
		LoadCustomContent<CustomCharaContent>();
		LoadOtherCustomContent();
		void LoadCustomContent<T>() where T : class, ICustomContent
		{
			UnityEngine.Debug.Log("#mod-content loading " + typeof(T).Name + "...");
			foreach (T item in _customContent.Values.OfType<T>())
			{
				try
				{
					item.OnGameLoad(context);
					contents.Add(item);
				}
				catch (Exception ex2)
				{
					LogModError("exception while loading custom content '" + item.ContentId + "'\n" + ex2.Message, item.Owner);
					UnityEngine.Debug.LogException(ex2);
				}
			}
		}
		void LoadOtherCustomContent()
		{
			foreach (ICustomContent item2 in _customContent.Values.Except(contents))
			{
				try
				{
					item2.OnGameLoad(context);
				}
				catch (Exception ex)
				{
					LogModError("exception while loading custom content '" + item2.ContentId + "'\n" + ex.Message, item2.Owner);
					UnityEngine.Debug.LogException(ex);
				}
			}
		}
	}

	[ElinPreSave]
	private static void OnPreSaveInit(GameIOContext context)
	{
	}

	[ElinPostSave]
	private static void OnPostSaveInit(GameIOContext context)
	{
		CustomReligionContent.SaveReligionData(context);
		foreach (ICustomContent value in _customContent.Values)
		{
			try
			{
				value.OnGameSave(context);
			}
			catch (Exception ex)
			{
				LogModError("exception while saving custom content '" + value.ContentId + "'\n" + ex.Message, value.Owner);
				UnityEngine.Debug.LogException(ex);
			}
		}
	}

	private static void OnReligionImporting(List<Religion> list)
	{
		list.AddRange(CustomReligionContent.GetCustomReligions());
	}

	[ElinCharaOnCreate]
	private static void OnCharaCreated(Chara chara)
	{
		if (TryGetContent<CustomCharaContent>("Chara/" + chara.id, out var content))
		{
			content.OnCharaCreated(chara);
		}
		if (TryGetContent<CustomBiographyContent>("Biography/" + chara.id, out var content2))
		{
			content2.RefreshCharaBio(chara);
		}
	}

	[ElinThingOnCreate]
	private static void OnThingCreated(Thing thing)
	{
		if (TryGetContent<CustomThingContent>("Thing/" + thing.id, out var content))
		{
			content.OnThingCreated(thing);
		}
	}

	[ElinActPerform]
	private static void OnActPerformed(Act act)
	{
		if (act.HasTag("godAbility"))
		{
			CustomReligionContent.managed.Values.FirstOrDefault((ReligionCustom r) => r.content.godAbilities.Contains(act.ID))?.Talk("ability");
		}
	}

	private static void OnSetLang(string lang)
	{
		PackageIterator.ClearCache();
		PackageIterator.RebuildAllMappings(lang);
		SourceLocalization.SetLang(lang);
		foreach (EMod activatedUserMod in ModManager.Instance.ActivatedUserMods)
		{
			if (activatedUserMod.IsSourceLocalizable && (activatedUserMod.isInPackages || Application.isEditor))
			{
				activatedUserMod.UpdateSourceLocalizationFile(lang);
			}
		}
		foreach (string item in LoadGodTalk())
		{
			MOD.listGodTalk.Add(new ExcelData(item));
		}
		foreach (string item2 in LoadCharaTalk())
		{
			MOD.listTalk.Add(new ExcelData(item2));
		}
		foreach (string item3 in LoadCharaTone())
		{
			MOD.tones.Add(new ExcelData(item3));
		}
		BookList.dict = null;
		BottleMessageList.list = null;
		Lang.excelDialog = null;
		foreach (CustomFileContent item4 in _customContent.Values.OfType<CustomFileContent>())
		{
			item4.OnSetLang(lang);
		}
	}

	public static bool TryGetContent<T>(string contentId, [NotNullWhen(true)] out T content) where T : class, ICustomContent
	{
		ICustomContent value;
		bool num = _customContent.TryGetValue(contentId, out value);
		content = value as T;
		if (num)
		{
			return content != null;
		}
		return false;
	}

	public static bool HasContent(string contentId)
	{
		return _customContent.ContainsKey(contentId);
	}

	public static void AddContent(ICustomContent content)
	{
		UnityEngine.Debug.Log(_customContent.Remove(content.ContentId, out var value) ? ("#mod-content override '" + content.ContentId + "' from '" + value.Owner.id + "' to '" + content.Owner.id + "'") : ("#mod-content added '" + content.ContentId + "' from '" + content.Owner.id + "'"));
		_customContent[content.ContentId] = content;
	}

	public static void FixDefaultCharaRowPref(SourceChara.Row r)
	{
		r.pref = new SourcePref
		{
			pivotY = -10
		};
	}

	public static List<Thing> GenerateMerchantStock(Card owner, string stockId = null, bool forceRestock = false)
	{
		List<Thing> list = new List<Thing>();
		if (stockId.IsEmpty())
		{
			stockId = owner.GetStr("merchant_override");
		}
		if (stockId.IsEmpty())
		{
			return list;
		}
		string[] array = stockId.Split('|');
		HashSet<string> orCreate = EClass.player.noRestocks.GetOrCreate(owner.trait.IdNoRestock, () => new HashSet<string>());
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (!TryGetContent<CustomMerchantStock>("MerchantStock/" + text, out var content))
			{
				continue;
			}
			foreach (Thing item in content.Generate(owner))
			{
				bool @bool = item.GetBool(101);
				if (!(!forceRestock && @bool) || !orCreate.Contains(item.trait.IdNoRestock))
				{
					if (@bool)
					{
						orCreate.Add(item.trait.IdNoRestock);
					}
					list.Add(item);
				}
			}
		}
		return list;
	}

	public static void RegisterElinEventAttributes()
	{
		ClassCache.modTypes.Add(typeof(ModUtil));
		ClassCache.modTypes.Add(typeof(CustomDramaExpansion));
		foreach (var item3 in ClassCache.modTypes.Except(_checkedAttributedTypes).MembersWith<ElinEventBaseAttribute>())
		{
			MemberInfo item = item3.member;
			ElinEventBaseAttribute[] item2 = item3.attrs;
			foreach (ElinEventBaseAttribute elinEventBaseAttribute in item2)
			{
				try
				{
					if (!(item is PropertyInfo property))
					{
						if (item is MethodInfo method)
						{
							elinEventBaseAttribute.Register(method);
						}
					}
					else
					{
						elinEventBaseAttribute.Register(property);
					}
				}
				catch (Exception ex)
				{
					LogModError("exception while registering attribute '" + elinEventBaseAttribute.GetType().Name + "' from '" + item.TryToString() + "'\n" + ex.Message, item.DeclaringType);
					UnityEngine.Debug.LogException(ex);
				}
			}
		}
		_checkedAttributedTypes.UnionWith(ClassCache.modTypes);
	}

	public static List<string> LoadBookList()
	{
		List<string> list = new List<string>();
		DirectoryInfo[] directories = PackageIterator.GetDirectories("Text");
		foreach (DirectoryInfo directoryInfo in directories)
		{
			list.AddRange(Directory.GetDirectories(directoryInfo.FullName));
			UnityEngine.Debug.Log("#mod-content loaded book list " + directoryInfo.ShortPath());
		}
		return list;
	}

	public static List<string> LoadTopicFiles()
	{
		List<string> list = new List<string>();
		FileInfo[] files = PackageIterator.GetFiles("Text/Help/_topics.txt");
		foreach (FileInfo fileInfo in files)
		{
			list.AddRange(IO.LoadTextArray(fileInfo.FullName));
			UnityEngine.Debug.Log("#mod-content loaded book topics " + fileInfo.ShortPath());
		}
		return list;
	}

	public static List<string> LoadExcelDialog()
	{
		List<string> list = new List<string>();
		FileInfo[] files = PackageIterator.GetFiles("Dialog/dialog.xlsx");
		foreach (FileInfo fileInfo in files)
		{
			list.Add(fileInfo.FullName);
			UnityEngine.Debug.Log("#mod-content loaded dialog " + fileInfo.ShortPath());
		}
		return list;
	}

	public static List<string> LoadCharaTalk()
	{
		List<string> list = new List<string>();
		FileInfo[] files = PackageIterator.GetFiles("Data/chara_talk.xlsx");
		foreach (FileInfo fileInfo in files)
		{
			list.Add(fileInfo.FullName);
			UnityEngine.Debug.Log("#mod-content loaded chara tone " + fileInfo.ShortPath());
		}
		return list;
	}

	public static List<string> LoadCharaTone()
	{
		List<string> list = new List<string>();
		FileInfo[] files = PackageIterator.GetFiles("Data/chara_tone.xlsx");
		foreach (FileInfo fileInfo in files)
		{
			list.Add(fileInfo.FullName);
			UnityEngine.Debug.Log("#mod-content loaded chara talk " + fileInfo.ShortPath());
		}
		return list;
	}

	public static Sprite LoadSprite(string spritePath, Vector2? pivot = null, string name = null, int resizeWidth = 0, int resizeHeight = 0)
	{
		if (spritePath.IsEmpty())
		{
			return null;
		}
		if (!spritePath.EndsWith(".png"))
		{
			if (SpriteReplacer.dictModItems.TryGetValue(spritePath, out var value))
			{
				spritePath = value;
			}
			spritePath += ".png";
		}
		if (!File.Exists(spritePath))
		{
			FileInfo[] files = PackageIterator.GetFiles(Path.Combine("Texture", spritePath));
			if (files.IsEmpty())
			{
				return null;
			}
			spritePath = files[^1].FullName;
		}
		Vector2 valueOrDefault = pivot.GetValueOrDefault();
		if (!pivot.HasValue)
		{
			valueOrDefault = new Vector2(0.5f, 0.5f);
			pivot = valueOrDefault;
		}
		string text = $"{spritePath}/{pivot}/{resizeWidth}/{resizeHeight}";
		if (name == null)
		{
			name = text;
		}
		try
		{
			if (!_cachedTextures.TryGetValue(text, out var value2))
			{
				value2 = IO.LoadPNG(spritePath);
				if (value2 == null)
				{
					return null;
				}
				if (resizeWidth != 0 && resizeHeight != 0 && value2.width != resizeWidth && value2.height != resizeHeight)
				{
					Texture2D texture2D = value2.Rescale(resizeWidth, resizeHeight);
					UnityEngine.Object.Destroy(value2);
					value2 = texture2D;
					value2.name = text;
				}
			}
			_cachedTextures[text] = value2;
		}
		catch (Exception arg)
		{
			UnityEngine.Debug.LogError($"#mod-content failed to load sprite {spritePath.ShortPath()}\n{arg}");
			return null;
		}
		Texture2D texture2D2 = _cachedTextures[text];
		Sprite sprite = Sprite.Create(texture2D2, new Rect(0f, 0f, texture2D2.width, texture2D2.height), pivot.Value, 100f, 0u, SpriteMeshType.FullRect);
		sprite.name = name;
		return sprite;
	}

	public static Sprite AppendSpriteSheet(string id, int resizeWidth = 0, int resizeHeight = 0, string pattern = "@")
	{
		Dictionary<string, string> dictModItems = SpriteReplacer.dictModItems;
		if (!dictModItems.TryGetValue(id, out var value) && pattern != "")
		{
			value = dictModItems.Where((KeyValuePair<string, string> kv) => kv.Key.StartsWith(pattern)).FirstOrDefault((KeyValuePair<string, string> kv) => id.StartsWith(kv.Key[pattern.Length..])).Value;
		}
		string spritePath = value;
		string name = id;
		Sprite sprite = LoadSprite(spritePath, null, name, resizeWidth, resizeHeight);
		if (sprite == null)
		{
			return null;
		}
		if (SpriteSheet.dict.TryGetValue(id, out var value2) && value2.texture.width == sprite.texture.width && value2.texture.height == sprite.texture.height)
		{
			return value2;
		}
		return SpriteSheet.dict[sprite.name] = sprite;
	}

	public static List<string> LoadGodTalk()
	{
		List<string> list = new List<string>();
		FileInfo[] files = PackageIterator.GetFiles("Data/god_talk.xlsx");
		foreach (FileInfo fileInfo in files)
		{
			list.Add(fileInfo.FullName);
			UnityEngine.Debug.Log("#mod-content loaded god talk " + fileInfo.ShortPath());
		}
		return list;
	}

	public static Dictionary<string, CustomGunEffectData> LoadGunEffects()
	{
		Dictionary<string, CustomGunEffectData> dictionary = new Dictionary<string, CustomGunEffectData>();
		(FileInfo, EMod)[] filesEx = PackageIterator.GetFilesEx("Data/EffectSetting.guns.json");
		for (int i = 0; i < filesEx.Length; i++)
		{
			(FileInfo, EMod) tuple = filesEx[i];
			FileInfo item = tuple.Item1;
			EMod item2 = tuple.Item2;
			CustomGunEffectSetting customGunEffectSetting = CustomGunEffectSetting.CreateFromFile(item, item2 as ModPackage);
			_customContent[customGunEffectSetting.ContentId] = customGunEffectSetting;
		}
		foreach (CustomGunEffectSetting item3 in _customContent.Values.OfType<CustomGunEffectSetting>())
		{
			item3.Load();
			dictionary.Merge(item3.items);
			UnityEngine.Debug.Log($"#mod-content loaded {item3}");
		}
		return dictionary;
	}

	public static void ImportAllGunEffectSettings()
	{
		foreach (KeyValuePair<string, CustomGunEffectData> item in LoadGunEffects())
		{
			item.Deconstruct(out var key, out var value);
			string key2 = key;
			GameSetting.EffectData value2 = value.CreateEffectData();
			EClass.setting.effect.guns[key2] = value2;
		}
	}

	public static string ExportAllGunEffectSettings()
	{
		UD_String_EffectData guns = EClass.setting.effect.guns;
		string text = CorePath.rootExe + "/guns.json";
		Dictionary<string, CustomGunEffectData> dictionary = new Dictionary<string, CustomGunEffectData>();
		foreach (string key in guns.Keys)
		{
			CustomGunEffectData value = CustomGunEffectData.CreateFromId(key);
			dictionary[key] = value;
		}
		File.WriteAllText(text, JsonConvert.SerializeObject(dictionary, Formatting.Indented, GameIOContext.Settings));
		return $"dumped {dictionary.Count} guns data to {text}";
	}

	private static Effect LoadEffect(string id)
	{
		if (id.IsEmpty())
		{
			return null;
		}
		if (!_rodTemplate)
		{
			return null;
		}
		string effectId = id.Split('/')[^1];
		Sprite sprite = LoadSprite(effectId);
		if (!sprite)
		{
			return null;
		}
		Effect effect = UnityEngine.Object.Instantiate(_rodTemplate);
		effect.name = effectId;
		effect.sprites = Slice().ToArray();
		UnityEngine.Object.DontDestroyOnLoad(effect);
		UnityEngine.Debug.Log("#mod-content loaded custom effect '" + effectId + "'");
		return effect;
		IEnumerable<Sprite> Slice()
		{
			int height = (int)sprite.rect.height;
			float frames = sprite.rect.width / (float)height;
			if (frames != 0f)
			{
				int i = 0;
				while ((float)i < frames)
				{
					Sprite sprite2 = Sprite.Create(rect: new Rect(i * height, 0f, height, height), texture: sprite.texture, pivot: new Vector2(0.5f, 0.5f * (128f / (float)height)), pixelsPerUnit: 100f, extrude: 0u, meshType: SpriteMeshType.FullRect);
					sprite2.name = $"{effectId}{i:D4}";
					yield return sprite2;
					int num = i + 1;
					i = num;
				}
			}
		}
	}

	private static void LoadEffectTemplate()
	{
		_rodTemplate = Resources.Load<Effect>("Media/Effect/General/rod");
		if (!_rodTemplate)
		{
			UnityEngine.Debug.LogWarning("#mod-content cannot initialize rod effect template");
		}
	}

	public static SerializableSoundData GetSoundMeta(string soundPath)
	{
		string path = Path.ChangeExtension(soundPath, ".json");
		SerializableSoundData serializableSoundData;
		if (File.Exists(path))
		{
			try
			{
				serializableSoundData = IO.LoadFile<SerializableSoundData>(path);
				if (serializableSoundData.dataVersion == SerializableSoundData.SoundDataMetaVersion.V1)
				{
					return serializableSoundData;
				}
			}
			catch
			{
			}
		}
		serializableSoundData = new SerializableSoundData();
		if (soundPath.NormalizePath().Contains("/Sound/BGM/"))
		{
			serializableSoundData.type = SoundData.Type.BGM;
			serializableSoundData.bgmDataOptional = new SerializableBGMData
			{
				parts = new List<BGMData.Part>
				{
					new BGMData.Part()
				}
			};
		}
		IO.SaveFile(path, serializableSoundData);
		return serializableSoundData;
	}

	public static AudioType GetAudioType(string extension)
	{
		return extension.ToLowerInvariant().Trim() switch
		{
			".acc" => AudioType.ACC, 
			".mp3" => AudioType.MPEG, 
			".ogg" => AudioType.OGGVORBIS, 
			".wav" => AudioType.WAV, 
			_ => AudioType.UNKNOWN, 
		};
	}

	public static SoundData LoadSoundData(string soundId)
	{
		if (!MOD.sounds.TryGetValue(soundId, out var value) || !value.Exists)
		{
			return null;
		}
		return LoadSoundData(value);
	}

	public static SoundData LoadSoundData(FileInfo soundFile)
	{
		string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(soundFile.FullName);
		string fullName = soundFile.FullName;
		AudioType audioType = GetAudioType(soundFile.Extension);
		bool stream = fullName.NormalizePath().Contains("/BGM/") && audioType == AudioType.OGGVORBIS;
		using UnityWebRequest unityWebRequest = AudioClipStream.GetAudioClip("file://" + fullName, audioType, compressed: false, stream);
		unityWebRequest.SendWebRequest();
		Stopwatch stopwatch = Stopwatch.StartNew();
		while (!unityWebRequest.isDone && stopwatch.ElapsedMilliseconds < 5000)
		{
			Thread.Sleep(1);
		}
		if (unityWebRequest.result != UnityWebRequest.Result.Success)
		{
			UnityEngine.Debug.LogError("#sound '" + fileNameWithoutExtension + "' failed to load: " + unityWebRequest.error.IsEmpty("timeout"));
			return null;
		}
		AudioClip content = DownloadHandlerAudioClip.GetContent(unityWebRequest);
		int? num = content?.samples;
		if (!num.HasValue || num.GetValueOrDefault() <= 0)
		{
			UnityEngine.Debug.LogError($"#sound '{fileNameWithoutExtension}' sample is null: {audioType}");
			return null;
		}
		content.name = fileNameWithoutExtension;
		SoundData soundData = GetSoundMeta(fullName).ToSoundData();
		if (soundData is BGMData bGMData)
		{
			bGMData._name = Path.GetFileNameWithoutExtension(fullName);
			if (bGMData.song == null)
			{
				bGMData.song = new BGMData.SongData();
				bGMData.song.parts.Add(new BGMData.Part());
			}
		}
		soundData.clip = content;
		soundData.name = fileNameWithoutExtension;
		UnityEngine.Debug.Log($"#sound '{fileNameWithoutExtension}' loaded: {audioType}/{content.length}s");
		SoundManager.current.dictData[fileNameWithoutExtension] = soundData;
		return soundData;
	}

	public static void AddOrReplaceBGM(string bgmId)
	{
		List<BGMData> bgms = Core.Instance.refs.bgms;
		Dictionary<int, BGMData> dictBGM = Core.Instance.refs.dictBGM;
		BGMData bGMData = LoadSoundData(bgmId) as BGMData;
		if (bGMData == null)
		{
			return;
		}
		bGMData.name = bgmId.Replace("BGM/", "");
		if (bGMData.id <= 0)
		{
			Match match = Regex.Match(bGMData.name, "^(\\d+)(?:_(.*))?$");
			if (match.Success)
			{
				int id = match.Groups[1].Value.ToInt();
				string text = (match.Groups[2].Success ? match.Groups[2].Value : null);
				bGMData.id = id;
				if (!text.IsEmpty())
				{
					bGMData.name = text;
				}
			}
			else
			{
				bGMData.id = bgms.Count + 1;
				UnityEngine.Debug.Log($"#sound bgm unassigned/{bGMData.id}/{bGMData.name}");
			}
		}
		if (dictBGM.TryGetValue(bGMData.id, out var value))
		{
			value.clip = bGMData.clip;
			UnityEngine.Debug.Log($"#sound bgm replace/{bGMData.id}/{value.name}/>/{bGMData.name}");
		}
		else
		{
			bgms.Add(bGMData);
			dictBGM[bGMData.id] = bGMData;
			UnityEngine.Debug.Log($"#sound bgm addon/{bGMData.id}/{bGMData.name}");
		}
	}

	public static ModPackage FindSoundPackage(string soundId)
	{
		return ModManagerCore.Instance.packages.OfType<ModPackage>().LastOrDefault((ModPackage p) => p.sounds.Keys.Contains(soundId));
	}

	public static Playlist CreatePlaylist(ref List<int> bgmIds, Playlist mold = null)
	{
		Playlist playlist = EClass.Sound.plBlank.Instantiate();
		if (bgmIds.Count == 0 && (bool)mold)
		{
			bgmIds = mold.ToInts();
			playlist.shuffle = mold.shuffle;
			playlist.minSwitchTime = mold.minSwitchTime;
			playlist.nextBGMOnSwitch = mold.nextBGMOnSwitch;
			playlist.ignoreLoop = mold.ignoreLoop;
			playlist.keepBGMifSamePlaylist = mold.keepBGMifSamePlaylist;
			playlist.name = mold.name;
		}
		foreach (int bgmId in bgmIds)
		{
			if (EClass.core.refs.dictBGM.TryGetValue(bgmId, out var value))
			{
				playlist.list.Add(new Playlist.Item
				{
					data = value
				});
			}
		}
		return playlist;
	}

	public static void SetBGMKnown(int bgmId, bool known = true)
	{
		known &= EClass.core.refs.dictBGM.ContainsKey(bgmId);
		if (known)
		{
			EClass.player.knownBGMs.Add(bgmId);
		}
		else
		{
			EClass.player.knownBGMs.Remove(bgmId);
		}
	}

	[Obsolete("use ImportModSourceSheets for proper caching and localizaiton support")]
	public static void ImportExcel(string pathToExcelFile, string sheetName, SourceData source)
	{
		UnityEngine.Debug.Log("ImportExcel source:" + source?.ToString() + " Path:" + pathToExcelFile);
		using FileStream @is = File.Open(pathToExcelFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
		XSSFWorkbook xSSFWorkbook = new XSSFWorkbook((Stream)@is);
		for (int i = 0; i < xSSFWorkbook.NumberOfSheets; i++)
		{
			ISheet sheetAt = xSSFWorkbook.GetSheetAt(i);
			if (sheetAt.SheetName != sheetName)
			{
				continue;
			}
			UnityEngine.Debug.Log("Importing Sheet:" + sheetName);
			try
			{
				ExcelParser.path = pathToExcelFile;
				if (!source.ImportData(sheetAt, new FileInfo(pathToExcelFile).Name, overwrite: true))
				{
					UnityEngine.Debug.LogError(ERROR.msg);
					break;
				}
				UnityEngine.Debug.Log("Imported " + sheetAt.SheetName);
				source.Reset();
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.LogError("[Error] Skipping import " + sheetAt.SheetName + " :" + ex.Message + "/" + ex.Source + "/" + ex.StackTrace);
				break;
			}
		}
	}

	public static SourceData FindSourceByName(string sourceData)
	{
		return sourceImporter.FindSourceByName(sourceData);
	}

	public static void ImportModSourceSheets(string modId)
	{
		try
		{
			SourceCache.InvalidateCacheVersion();
			EMod valueOrDefault = ModManager.Instance.MappedPackages.GetValueOrDefault(modId);
			if (valueOrDefault == null)
			{
				return;
			}
			List<string> list = new List<string>();
			foreach (FileInfo sourceSheet in valueOrDefault.Mapping.SourceSheets)
			{
				sourceImporter.fileProviders[sourceSheet.FullName] = valueOrDefault;
				list.Add(sourceSheet.FullName);
			}
			sourceImporter.ImportFilesCached(list);
			UnityEngine.Debug.Log("#source finished importing workbooks from " + modId);
		}
		catch (Exception exception)
		{
			UnityEngine.Debug.LogException(exception);
		}
	}

	public static void ImportAllModSourceSheets()
	{
		try
		{
			SourceCache.InvalidateCacheVersion();
			SourceImporter.HotInit(new SourceData[2]
			{
				EClass.sources.elements,
				EClass.sources.materials
			});
			List<string> list = new List<string>();
			foreach (BaseModPackage package in ModManager.Instance.packages)
			{
				if (!(package is ModPackage modPackage) || !package.activated || package.id == null)
				{
					continue;
				}
				foreach (FileInfo sourceSheet in modPackage.Mapping.SourceSheets)
				{
					if (!sourceSheet.Name.StartsWith(".") && !sourceSheet.Name.Contains('~'))
					{
						sourceImporter.fileProviders[sourceSheet.FullName] = modPackage;
						list.Add(sourceSheet.FullName);
					}
				}
			}
			sourceImporter.ImportFilesCached(list);
		}
		catch (Exception exception)
		{
			UnityEngine.Debug.LogException(exception);
		}
		UnityEngine.Debug.Log("#source finished importing workbooks");
	}

	public static string ExportSourceDataCsv(string sourceData, string delimiter = ",")
	{
		SourceData sourceData2 = FindSourceByName(sourceData);
		if (sourceData2 == null)
		{
			return "";
		}
		IReadOnlyDictionary<string, string> typeMapping = sourceData2.GetTypeMapping();
		(string, string)[] array = (from kv in sourceData2.GetRowMapping()
			orderby kv.Value
			select (column: kv.Key, type: typeMapping[kv.Key])).ToArray();
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(string.Join(delimiter, array.Select(((string column, string type) p) => p.column)));
		stringBuilder.AppendLine(string.Join(delimiter, array.Select(((string column, string type) p) => p.type)));
		SourceData.BaseRow[] array2 = sourceData2.ExportRows();
		foreach (SourceData.BaseRow baseRow in array2)
		{
			List<string> list = new List<string>();
			(string, string)[] array3 = array;
			for (int j = 0; j < array3.Length; j++)
			{
				(string, string) tuple = array3[j];
				string item = tuple.Item1;
				string item2 = tuple.Item2;
				object value = baseRow.GetRowFields()[item].GetValue(baseRow);
				object obj = value;
				IEnumerable enumerable;
				if (!(obj is int[] array4))
				{
					if (!(obj is int key))
					{
						if (obj is string item3)
						{
							list.Add(item3);
							continue;
						}
						enumerable = obj as IEnumerable;
						if (enumerable != null)
						{
							goto IL_024a;
						}
					}
					else if (item2 == "element_id")
					{
						list.Add(EClass.sources.elements.map[key].alias);
						continue;
					}
					list.Add(value.ToString());
					continue;
				}
				if (!(item2 == "elements"))
				{
					enumerable = (IEnumerable)obj;
					goto IL_024a;
				}
				List<string> list2 = new List<string>();
				for (int k = 0; k < array4.Length - 1; k += 2)
				{
					string alias = EClass.sources.elements.map[array4[k]].alias;
					string text = array4[k + 1].ToString();
					list2.Add(alias + "/" + text);
				}
				string item4 = string.Join(',', list2);
				list.Add(item4);
				continue;
				IL_024a:
				string item5 = string.Join(',', enumerable.OfType<object>());
				list.Add(item5);
			}
			IEnumerable<string> values = list.Select(delegate(string f)
			{
				string text2 = f.Replace("\r", "").Replace("\n", "\\n").Replace("\"", "\"\"");
				return (!text2.IsEmpty()) ? ("\"" + text2 + "\"") : "";
			});
			stringBuilder.AppendLine(string.Join(delimiter, values));
		}
		return stringBuilder.ToString();
	}

	public static void ExportAllSourceDataCsv(string dir)
	{
		IO.CreateDirectory(dir);
		foreach (string key in SourceMapping.Keys)
		{
			File.WriteAllText(Path.Combine(dir, key + ".csv"), ExportSourceDataCsv(key));
		}
	}

	public static string ExportSourceDataJson(string sourceData, Formatting format = Formatting.Indented)
	{
		SourceData sourceData2 = FindSourceByName(sourceData);
		if (sourceData2 == null)
		{
			return "";
		}
		return JsonConvert.SerializeObject(sourceData2.ExportRows(), format);
	}

	public static void ExportAllSourceDataJson(string dir)
	{
		IO.CreateDirectory(dir);
		foreach (string key in SourceMapping.Keys)
		{
			File.WriteAllText(Path.Combine(dir, key + ".json"), ExportSourceDataJson(key));
		}
	}

	public static void AddContextMenuEntry(Action onClick, string menuEntry, string displayName = "")
	{
		if (onClick == null || menuEntry.IsEmpty())
		{
			return;
		}
		string[] array = menuEntry.Split('/', StringSplitOptions.RemoveEmptyEntries);
		List<ContextMenuProxy> children = contextMenuProxies;
		for (int i = 0; i < array.Length; i++)
		{
			bool flag = i < array.Length - 1;
			string part = array[i];
			ContextMenuProxy contextMenuProxy = children.Find((ContextMenuProxy p) => p.MenuEntry == part);
			if (contextMenuProxy == null)
			{
				contextMenuProxy = new ContextMenuProxy(flag ? part : displayName, part)
				{
					onClick = ((i == array.Length - 1 && flag) ? null : new Action(SafeInvoke)),
					isMenu = flag
				};
				children.Add(contextMenuProxy);
			}
			else if (contextMenuProxy.isMenu != flag)
			{
				UnityEngine.Debug.LogWarning("#mod-content attempt to add context menu entry with same name but different types\n" + part + " -> " + (contextMenuProxy.isMenu ? "submenu" : "button"));
				return;
			}
			children = contextMenuProxy.children;
		}
		UnityEngine.Debug.Log("#mod-content added context menu entry '" + menuEntry + "' from '" + onClick.Method.TryToString() + "'");
		void SafeInvoke()
		{
			try
			{
				onClick();
			}
			catch (Exception exception)
			{
				UnityEngine.Debug.LogException(exception);
			}
		}
	}

	public static void RemoveContextMenuEntry(string entry)
	{
		string[] array = entry.Split('/', StringSplitOptions.RemoveEmptyEntries);
		if (array.Length == 0)
		{
			return;
		}
		List<ContextMenuProxy> children = contextMenuProxies;
		ContextMenuProxy contextMenuProxy = null;
		List<ContextMenuProxy> list = null;
		string[] array2 = array;
		foreach (string part in array2)
		{
			contextMenuProxy = children.Find((ContextMenuProxy p) => p.MenuEntry == part);
			if (contextMenuProxy == null)
			{
				return;
			}
			list = children;
			children = contextMenuProxy.children;
		}
		list?.Remove(contextMenuProxy);
	}
}
