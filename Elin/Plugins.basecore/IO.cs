using System;
using System.IO;
using System.Security;
using System.Security.Permissions;
using LZ4;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;

public class IO
{
	public enum Compression
	{
		LZ4,
		None
	}

	public static string log;

	public static JsonSerializerSettings jsReadGeneral = new JsonSerializerSettings
	{
		NullValueHandling = NullValueHandling.Ignore,
		DefaultValueHandling = DefaultValueHandling.Ignore,
		PreserveReferencesHandling = PreserveReferencesHandling.Objects,
		TypeNameHandling = TypeNameHandling.Auto,
		Error = OnError
	};

	public static JsonSerializerSettings jsWriteGeneral = new JsonSerializerSettings
	{
		NullValueHandling = NullValueHandling.Ignore,
		DefaultValueHandling = DefaultValueHandling.Ignore,
		PreserveReferencesHandling = PreserveReferencesHandling.Objects,
		TypeNameHandling = TypeNameHandling.Auto,
		ContractResolver = ShouldSerializeContractResolver.Instance,
		Error = OnError
	};

	public static JsonSerializerSettings jsWriteConfig = new JsonSerializerSettings
	{
		NullValueHandling = NullValueHandling.Ignore,
		DefaultValueHandling = DefaultValueHandling.Populate,
		PreserveReferencesHandling = PreserveReferencesHandling.Objects,
		TypeNameHandling = TypeNameHandling.Auto,
		ContractResolver = ShouldSerializeContractResolver.Instance,
		Error = OnError
	};

	public static Formatting formatting = Formatting.Indented;

	public static TextureImportSetting.Data importSetting = new TextureImportSetting.Data();

	public static JsonSerializerSettings dpSetting = new JsonSerializerSettings
	{
		NullValueHandling = NullValueHandling.Ignore,
		DefaultValueHandling = DefaultValueHandling.Include,
		PreserveReferencesHandling = PreserveReferencesHandling.Objects,
		TypeNameHandling = TypeNameHandling.Auto,
		Error = OnError
	};

	public static Formatting dpFormat = Formatting.Indented;

	public static string TempPath => Application.persistentDataPath + "/Temp";

	public static void OnError(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs args)
	{
	}

	public static void PrintLog()
	{
		if (!log.IsEmpty())
		{
			Debug.LogWarning(log);
			log = null;
		}
	}

	public static string GetJSON(object obj)
	{
		return JsonConvert.SerializeObject(obj, formatting, jsWriteGeneral);
	}

	public static T LoadJSON<T>(string json)
	{
		return JsonConvert.DeserializeObject<T>(json, jsReadGeneral);
	}

	public static void SaveFile(string path, object obj, bool compress = false, JsonSerializerSettings setting = null)
	{
		string text = JsonConvert.SerializeObject(obj, formatting, setting ?? jsWriteGeneral);
		CreateDirectory(Path.GetDirectoryName(path));
		Debug.Log("#io SaveFile;" + path);
		if (compress)
		{
			Compress(path, text);
		}
		else
		{
			File.WriteAllText(path, text);
		}
	}

	public static void SaveText(string path, string text)
	{
		CreateDirectory(Path.GetDirectoryName(path));
		Debug.Log("#io SaveFile;" + path);
		File.WriteAllText(path, text);
	}

	public static void SaveTextArray(string path, string[] text)
	{
		CreateDirectory(Path.GetDirectoryName(path));
		Debug.Log("#io SaveFile;" + path);
		File.WriteAllLines(path, text);
	}

	public static T LoadFile<T>(string path, bool compress = false, JsonSerializerSettings setting = null)
	{
		if (!File.Exists(path))
		{
			Debug.Log("File does not exist:" + path);
			return default(T);
		}
		string value = (IsCompressed(path) ? Decompress(path) : File.ReadAllText(path));
		Debug.Log("#io LoadFile;" + path);
		return JsonConvert.DeserializeObject<T>(value, setting ?? jsReadGeneral);
	}

	public static T LoadStreamJson<T>(MemoryStream stream, JsonSerializerSettings setting = null)
	{
		stream.Position = 0L;
		string value = "";
		using (StreamReader streamReader = new StreamReader(stream))
		{
			value = streamReader.ReadToEnd();
		}
		return JsonConvert.DeserializeObject<T>(value, setting ?? jsReadGeneral);
	}

	public static void WriteLZ4(string _path, byte[] _bytes, Compression compression = Compression.None)
	{
		byte[] bytes = ((compression == Compression.LZ4) ? LZ4Codec.Wrap(_bytes, 0, _bytes.Length) : _bytes);
		for (int i = 0; i < 5; i++)
		{
			string path = _path + ((i == 0) ? "" : (".b" + i));
			try
			{
				File.WriteAllBytes(path, bytes);
				break;
			}
			catch (Exception message)
			{
				Debug.Log(message);
			}
		}
	}

	public static byte[] ReadLZ4(string _path, int size, Compression compression)
	{
		for (int i = 0; i < 5; i++)
		{
			string text = _path + ((i == 0) ? "" : (".b" + i));
			if (!File.Exists(text))
			{
				Debug.Log("Couldn't find:" + text);
				continue;
			}
			byte[] array = File.ReadAllBytes(text);
			if (compression == Compression.LZ4)
			{
				try
				{
					return ReadLZ4(array);
				}
				catch (Exception message)
				{
					Debug.Log(message);
				}
			}
			if (array.Length == size)
			{
				return array;
			}
		}
		return null;
	}

	public static byte[] ReadLZ4(byte[] bytes)
	{
		try
		{
			return LZ4Codec.Unwrap(bytes);
		}
		catch
		{
			Debug.Log("Exception: Failed to unwrap:");
			return bytes;
		}
	}

	public static bool IsCompressed(string path)
	{
		byte[] array;
		using (BinaryReader binaryReader = new BinaryReader(File.OpenRead(path)))
		{
			binaryReader.BaseStream.Seek(0L, SeekOrigin.Begin);
			array = binaryReader.ReadBytes(4);
		}
		if (array.Length > 3 && array[0] == 123 && array[1] == 13 && array[2] == 10 && array[3] == 32)
		{
			return false;
		}
		return true;
	}

	public static void Compress(string path, string text)
	{
		Debug.Log("Compressing: " + path);
		using FileStream innerStream = new FileStream(path, FileMode.Create);
		using LZ4Stream stream = new LZ4Stream(innerStream, LZ4StreamMode.Compress);
		using StreamWriter streamWriter = new StreamWriter(stream);
		streamWriter.Write(text);
	}

	public static string Decompress(string path)
	{
		try
		{
			using FileStream innerStream = new FileStream(path, FileMode.Open);
			using LZ4Stream stream = new LZ4Stream(innerStream, LZ4StreamMode.Decompress);
			using StreamReader streamReader = new StreamReader(stream);
			return streamReader.ReadToEnd();
		}
		catch (Exception message)
		{
			Debug.Log(message);
		}
		Debug.Log("Cannot decompress:" + IsCompressed(path) + "/" + path);
		string text = File.ReadAllText(path);
		Debug.Log(text);
		return text.IsEmpty("");
	}

	public static void CopyDir(string sourceDirectory, string targetDirectory, Func<string, bool> funcExclude = null)
	{
		DirectoryInfo directoryInfo = new DirectoryInfo(sourceDirectory);
		DirectoryInfo target = new DirectoryInfo(targetDirectory);
		if (!directoryInfo.Exists)
		{
			Debug.Log("Source dir doesn't exist:" + directoryInfo.FullName);
		}
		else
		{
			_CopyDir(directoryInfo, target, funcExclude);
		}
	}

	public static void _CopyDir(DirectoryInfo source, DirectoryInfo target, Func<string, bool> funcExclude = null)
	{
		if (funcExclude == null || !funcExclude(source.Name))
		{
			Directory.CreateDirectory(target.FullName);
			FileInfo[] files = source.GetFiles();
			foreach (FileInfo fileInfo in files)
			{
				fileInfo.CopyTo(Path.Combine(target.FullName, fileInfo.Name), overwrite: true);
			}
			DirectoryInfo[] directories = source.GetDirectories();
			foreach (DirectoryInfo directoryInfo in directories)
			{
				DirectoryInfo target2 = target.CreateSubdirectory(directoryInfo.Name);
				_CopyDir(directoryInfo, target2, funcExclude);
			}
		}
	}

	public static void Copy(string fromPath, string toPath)
	{
		if (!File.Exists(fromPath))
		{
			Debug.Log("File does not exist:" + fromPath);
			return;
		}
		FileInfo fileInfo = new FileInfo(fromPath);
		DirectoryInfo directoryInfo = new DirectoryInfo(toPath);
		if (!Directory.Exists(directoryInfo.FullName))
		{
			CreateDirectory(directoryInfo.FullName);
		}
		File.Copy(fileInfo.FullName, directoryInfo.FullName + "/" + fileInfo.Name, overwrite: true);
	}

	public static void CopyAs(string fromPath, string toPath)
	{
		if (!File.Exists(fromPath))
		{
			Debug.LogError("File does not exist:" + fromPath);
		}
		else
		{
			File.Copy(fromPath, toPath, overwrite: true);
		}
	}

	public static void CopyAll(string fromPath, string toPath, bool overwrite = true)
	{
		CreateDirectory(toPath);
		string[] directories = Directory.GetDirectories(fromPath, "*", SearchOption.AllDirectories);
		for (int i = 0; i < directories.Length; i++)
		{
			Directory.CreateDirectory(directories[i].Replace(fromPath, toPath));
		}
		directories = Directory.GetFiles(fromPath, "*.*", SearchOption.AllDirectories);
		foreach (string text in directories)
		{
			string text2 = text.Replace(fromPath, toPath);
			if (overwrite || !File.Exists(text2))
			{
				File.Copy(text, text2, overwrite: true);
			}
		}
	}

	public static void DeleteFile(string path)
	{
		if (File.Exists(path))
		{
			File.Delete(path);
		}
	}

	public static void DeleteFiles(string path)
	{
		if (Directory.Exists(path))
		{
			FileInfo[] files = new DirectoryInfo(path).GetFiles();
			for (int i = 0; i < files.Length; i++)
			{
				files[i].Delete();
			}
		}
	}

	public static void CreateDirectory(string path)
	{
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}
	}

	public static void DeleteDirectory(string path)
	{
		path = path.Replace("\\\\?\\", "");
		if (Directory.Exists(path))
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(path);
			try
			{
				new FileIOPermission(FileIOPermissionAccess.AllAccess, path).Demand();
			}
			catch (SecurityException ex)
			{
				Debug.Log(ex.ToString());
			}
			if (directoryInfo.Exists)
			{
				directoryInfo.Delete(recursive: true);
			}
		}
	}

	public static T Duplicate<T>(T t)
	{
		return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(t, formatting, jsWriteGeneral), jsReadGeneral);
	}

	public static void CreateTempDirectory(string path = null)
	{
		CreateDirectory(path ?? TempPath);
	}

	public static void DeleteTempDirectory(string path = null)
	{
		DeleteDirectory(path ?? TempPath);
	}

	public static T LoadObject<T>(FileInfo file, object option = null) where T : UnityEngine.Object
	{
		return LoadObject<T>(file.FullName, option);
	}

	public static T LoadObject<T>(string _path, object option = null) where T : UnityEngine.Object
	{
		Type typeFromHandle = typeof(T);
		if (typeFromHandle == typeof(Sprite))
		{
			SpriteLoadOption spriteLoadOption = option as SpriteLoadOption;
			Texture2D texture2D = LoadPNG(_path);
			if (!texture2D)
			{
				return null;
			}
			return Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), spriteLoadOption?.pivot ?? new Vector2(0.5f, 0f), 100f) as T;
		}
		if (typeFromHandle == typeof(Texture2D))
		{
			return LoadPNG(_path) as T;
		}
		if (typeof(ExcelData).IsAssignableFrom(typeFromHandle))
		{
			T val = Activator.CreateInstance<T>();
			(val as ExcelData).path = _path;
			return val;
		}
		if (typeFromHandle == typeof(TextData))
		{
			return new TextData
			{
				lines = File.ReadAllLines(_path)
			} as T;
		}
		return null;
	}

	public static void SavePNG(Texture2D tex, string _path)
	{
		byte[] bytes = tex.EncodeToPNG();
		File.WriteAllBytes(_path, bytes);
	}

	public static Texture2D LoadPNG(string _path, FilterMode filter = FilterMode.Point)
	{
		return ImageLoader.LoadPNG(_path, filter);
	}

	public static T DeepCopy<T>(T target)
	{
		return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(target, dpFormat, dpSetting), dpSetting);
	}

	public static string[] LoadTextArray(string _path)
	{
		if (!File.Exists(_path))
		{
			_path += ".txt";
			if (!File.Exists(_path))
			{
				Debug.Log(_path);
				return new string[0];
			}
		}
		return File.ReadAllLines(_path);
	}

	public static string LoadText(string _path)
	{
		string[] array = LoadTextArray(_path);
		string text = "";
		string[] array2 = array;
		foreach (string text2 in array2)
		{
			text = text + text2 + Environment.NewLine;
		}
		return text;
	}
}
