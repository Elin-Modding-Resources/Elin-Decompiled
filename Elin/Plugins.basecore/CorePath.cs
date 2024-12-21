using System;
using System.IO;
using UnityEngine;

[Serializable]
public class CorePath
{
	public class UI
	{
		public const string UIMain = "UI/";

		public const string Layer = "UI/Layer/";

		public const string Content = "UI/Content/";

		public const string Window = "UI/Window/";

		public const string WindowBase = "UI/Window/Base/";

		public const string WindowElement = "UI/Window/Base/Element/";

		public const string Widget = "UI/Widget/";

		public const string Element = "UI/Element/";

		public const string Header = "UI/Element/Header/";

		public const string Text = "UI/Element/Text/";

		public const string Note = "UI/Element/Note/";

		public const string Deco = "UI/Element/Deco/";

		public const string Item = "UI/Element/Item/";

		public const string Button = "UI/Element/Button/";

		public const string Other = "UI/Element/Other/";

		public const string List = "UI/Element/List/";

		public const string Pop = "UI/Pop/";

		public const string Util = "UI/Util/";
	}

	public class CorePackage
	{
		public static string TextData => Lang.setting.dir + "Data/";

		public static string TextDialog
		{
			get
			{
				if (Lang.isBuiltin)
				{
					return packageCore + "Lang/_Dialog/";
				}
				return Lang.setting.dir + "Dialog/";
			}
		}

		public static string TextNarration => Lang.setting.dir + "Narration/";

		public static string TextCommon => packageCore + "Lang/_Common/";

		public static string Text => Lang.setting.dir + "Text/";

		public static string TextEN => packageCore + "Lang/EN/Text/";

		public static string Book => Text + "Book/";

		public static string Scroll => Text + "Scroll/";

		public static string Help => Text + "Help/";

		public static string HelpEN => packageCore + "Lang/EN/Text/HELP/";

		public static string News => Text + "News/";

		public static string Background => Text + "Background/";

		public static string Playlist => packageCore + "Sound/Playlist/";

		public static string LangImportMod => Lang.setting.dir + "Game/";

		public static string Ride => packageCore + "Actor/PCC/ride/";
	}

	public const string Scene = "Scene/";

	public const string SceneProfile = "Scene/Profile/";

	public const string FowProfile = "Scene/Profile/Fow/";

	public const string SceneTemplate = "Scene/Template/";

	public const string Lut = "Scene/Profile/Lut/";

	public const string PostEffect = "Scene/Profile/PostEffect/";

	public const string Data_Raw = "Data/Raw/";

	public const string Map = "World/Map/";

	public const string MapGen = "World/Map/Gen/";

	public const string ZoneProfile = "World/Zone/Profile/";

	public const string Media = "Media/";

	public const string Gallery = "Media/Gallery/";

	public const string Anime = "Media/Anime/";

	public const string Sound = "Media/Sound/";

	public const string Effect = "Media/Effect/";

	public const string TextEffect = "Media/Text/";

	public const string Particle = "Media/Effect/Particle/";

	public const string Graphics = "Media/Graphics/";

	public const string Icon = "Media/Graphics/Icon/";

	public const string IconElement = "Media/Graphics/Icon/Element/";

	public const string IconAchievement = "Media/Graphics/Icon/Achievement/";

	public const string IconRecipe = "Media/Graphics/Icon/Recipe/";

	public const string Image = "Media/Graphics/Image/";

	public const string Deco = "Media/Graphics/Deco/";

	public const string Drama = "Media/Drama/";

	public const string DramaActor = "Media/Drama/Actor/";

	public const string BuildMenu = "UI/BuildMenu/";

	public const string Render = "Scene/Render/";

	public const string Actor = "Scene/Render/Actor/";

	public const string RenderData = "Scene/Render/Data/";

	public const string TC = "Scene/Render/Actor/Component/";

	public const string Hoard = "UI/Layer/Hoard/";

	public const string News = "UI/Layer/LayerNewspaper/";

	[NonSerialized]
	public static string packageCore;

	[NonSerialized]
	public static string user;

	[NonSerialized]
	public static string custom;

	[NonSerialized]
	public static string rootMod;

	[NonSerialized]
	public static string rootExe;

	public static string Text_Popup => Lang.setting.dir + "Etc/Popup/";

	public static string Text_DialogHelp => Lang.setting.dir + "Etc/DialogHelp/";

	public static string DramaData => packageCore + "Lang/_Dialog/Drama/";

	public static string DramaDataLocal => Lang.setting.dir + "Dialog/Drama/";

	public static string ConfigFile => RootSave + "/config.txt";

	public static string VersionFile => RootSave + "/version.txt";

	public static string ShareSettingFile => RootSave + "/share_setting.txt";

	public static string coreWidget => packageCore + "Widget/";

	public static string WidgetSave => user + "Widget/";

	public static string LotTemplate => user + "Lot Template/";

	public static string SceneCustomizerSave => user + "Scene/";

	public static string ZoneSave => packageCore + "Map/";

	public static string ZoneSaveUser => user + "Map/";

	public static string MapPieceSave => packageCore + "Map Piece/";

	public static string MapPieceSaveUser => user + "Map Piece/";

	public static string RootData => Application.persistentDataPath + "/";

	public static string RootSave => Application.persistentDataPath + "/Save/";

	public static string RootSaveCloud => Application.persistentDataPath + "/Cloud Save/";

	public static string Temp => RootSave + "_Temp/";

	public static string PathIni => RootSave + "elin.ini";

	public static string PathBackupOld => RootSave + "Backup/";

	public static string PathBackup => RootData + "Backup/";

	public static string PathBackupCloud => RootData + "Cloud Backup/";

	public static void Init()
	{
		Debug.Log("Init CorePath");
		rootMod = Application.dataPath + "/../Package/";
		if (!Directory.Exists(rootMod))
		{
			rootMod = Application.streamingAssetsPath + "/Package/";
		}
		rootExe = Application.dataPath + "/../";
		packageCore = rootMod + "_Elona/";
		user = Application.persistentDataPath + "/User/";
		custom = Application.persistentDataPath + "/Custom/";
		if (!Application.isPlaying)
		{
			return;
		}
		if (!Directory.Exists(PathBackup))
		{
			if (Directory.Exists(PathBackupOld))
			{
				Directory.Move(PathBackupOld, PathBackup);
			}
			else
			{
				IO.CreateDirectory(PathBackup);
			}
		}
		IO.CreateDirectory(PathBackupCloud);
		IO.CreateDirectory(RootSave);
		IO.CreateDirectory(RootSaveCloud);
		string text = (Application.isEditor ? (Application.streamingAssetsPath + "/User/") : (rootExe + "User/"));
		if (!Application.isEditor || !Directory.Exists(user))
		{
			Debug.Log("Copy User Folder:" + text + "  to  " + user);
			IO.CreateDirectory(user);
			IO.CopyAll(text, user, overwrite: false);
			IO.CreateDirectory(SceneCustomizerSave);
		}
		text = (Application.isEditor ? (Application.streamingAssetsPath + "/Custom/") : (rootExe + "Custom/"));
		Debug.Log("Copy Custom Folder:" + text + "  to  " + custom);
		IO.CreateDirectory(custom);
		IO.CopyAll(text, custom, overwrite: false);
	}
}
