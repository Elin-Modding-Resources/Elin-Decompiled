using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class Net : MonoBehaviour
{
	public class ChatLog
	{
		public string name;

		public string msg;
	}

	public class VoteLog
	{
		public string name;

		public int count;

		public int index;

		public int time;
	}

	public class DownloadMeta
	{
		public string name;

		public string title;

		public string id;

		public string path;

		public string cat;

		public string tag;

		public string date;

		public int version;

		public bool IsValidVersion()
		{
			if (!Version.Get(version).IsBelow(EClass.core.versionMoongate))
			{
				return Version.Get(version).IsSameOrBelow(EClass.core.version);
			}
			return false;
		}
	}

	public class DownloadCahce
	{
		public Dictionary<string, string> items = new Dictionary<string, string>();
	}

	public List<ChatLog> chatList;

	private const string urlScript = "http://ylva.php.xdomain.jp/script/";

	private const string urlChat = "http://ylva.php.xdomain.jp/script/chat/";

	private const string urlVote = "http://ylva.php.xdomain.jp/script/vote/";

	private const string urlUpload = "http://ylva.php.xdomain.jp/script/uploader/";

	public static bool isUploading;

	public static bool ShowNetError
	{
		get
		{
			if (!EClass.core.config.test.showNetError)
			{
				return EClass.debug.enable;
			}
			return true;
		}
	}

	public void ShowVote(string logs)
	{
		StringReader stringReader = new StringReader(logs);
		for (string text = stringReader.ReadLine(); text != null; text = stringReader.ReadLine())
		{
			Debug.Log(text);
		}
	}

	public void ShowChat(string logs)
	{
		foreach (JToken item in (JArray)JsonConvert.DeserializeObject(logs))
		{
			ChatLog chatLog = item.ToObject<ChatLog>();
			Debug.Log(chatLog.name + "/" + chatLog.msg);
		}
	}

	public static async UniTask<bool> UploadFile(string id, string password, string name, string title, string path, string idLang, string cat = "Home", string tag = "")
	{
		if (isUploading)
		{
			EClass.ui.Say("sys_uploadUploading");
			return false;
		}
		EClass.ui.Say("sys_uploadStart");
		byte[] array;
		using (FileStream fileStream = File.OpenRead(path))
		{
			array = new byte[fileStream.Length];
			fileStream.Read(array, 0, (int)fileStream.Length);
		}
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("mode", 1);
		wWWForm.AddField("id", id);
		wWWForm.AddField("name", name);
		wWWForm.AddField("title", title);
		wWWForm.AddField("cat", cat);
		wWWForm.AddField("tag", tag);
		wWWForm.AddField("idLang", idLang);
		wWWForm.AddField("password", password);
		wWWForm.AddField("submit", "Send");
		wWWForm.AddField("version", EClass.core.version.GetInt().ToString() ?? "");
		wWWForm.AddBinaryData("file", array, "file.z");
		isUploading = true;
		Debug.Log(id);
		Debug.Log(name);
		Debug.Log(title);
		Debug.Log(idLang);
		Debug.Log(password);
		Debug.Log(tag);
		Debug.Log(array.Length);
		using (UnityWebRequest www = UnityWebRequest.Post("http://ylva.php.xdomain.jp/script/uploader/uploader.php", wWWForm))
		{
			try
			{
				await www.SendWebRequest();
				Debug.Log(www.result.ToString());
				Debug.Log(www.downloadHandler.ToString());
				Debug.Log(www.downloadHandler.text);
				Debug.Log(www.downloadHandler.text.ToString());
			}
			catch (Exception ex)
			{
				EClass.ui.Say(ex.Message);
			}
			isUploading = false;
			if (www.result != UnityWebRequest.Result.Success)
			{
				EClass.ui.Say((www.responseCode == 401) ? "sys_uploadConflict" : "sys_uploadFail");
				return false;
			}
		}
		EClass.ui.Say("sys_uploadSuccess");
		return true;
	}

	public static async UniTask<FileInfo> DownloadFile(DownloadMeta item, string path, string idLang)
	{
		string fn = item.id + ".z";
		DownloadCahce caches = IO.LoadFile<DownloadCahce>(CorePath.ZoneSaveUser + "cache.txt") ?? new DownloadCahce();
		if (caches.items.TryGetValue(item.id) == item.date.Replace("\"", "") && File.Exists(path + fn))
		{
			Debug.Log("Returning Cache:" + path + fn);
			return new FileInfo(path + fn);
		}
		using UnityWebRequest www = UnityWebRequest.Get("http://ylva.php.xdomain.jp/script/uploader/files/" + idLang + "/" + fn);
		www.downloadHandler = new DownloadHandlerFile(path + fn);
		try
		{
			await www.SendWebRequest();
		}
		catch (Exception ex)
		{
			EClass.ui.Say(ex.Message);
		}
		FileInfo fileInfo = new FileInfo(path + fn);
		if (!fileInfo.Exists || www.result != UnityWebRequest.Result.Success)
		{
			if (ShowNetError)
			{
				EClass.ui.Say(www.error);
			}
			return null;
		}
		caches.items[item.id] = item.date;
		IO.SaveFile(CorePath.ZoneSaveUser + "cache.txt", caches);
		return fileInfo;
	}

	public static async UniTask<List<DownloadMeta>> GetFileList(string idLang)
	{
		List<DownloadMeta> list = new List<DownloadMeta>();
		using UnityWebRequest www = UnityWebRequest.Get("http://ylva.php.xdomain.jp/script/uploader/files/" + idLang + "/index.txt");
		try
		{
			await www.SendWebRequest();
		}
		catch (Exception ex)
		{
			EClass.ui.Say(ex.Message);
		}
		if (www.result != UnityWebRequest.Result.Success)
		{
			if (ShowNetError)
			{
				EClass.ui.Say(www.error);
			}
			return null;
		}
		StringReader stringReader = new StringReader(www.downloadHandler.text);
		for (string text = stringReader.ReadLine(); text != null; text = stringReader.ReadLine())
		{
			if (!string.IsNullOrEmpty(text))
			{
				string[] array = text.Split(',');
				list.Add(new DownloadMeta
				{
					path = array[0],
					id = Path.GetFileNameWithoutExtension(array[0]),
					name = array[2],
					title = array[3],
					cat = array[5],
					date = array[6].Replace("\"", ""),
					version = ((array.Length >= 9) ? array[8].ToInt() : 0),
					tag = ((array.Length >= 10) ? array[9] : "")
				});
			}
		}
		return list;
	}

	public static async UniTask<bool> SendVote(int id, string idLang)
	{
		try
		{
			Debug.Log("Start Sending Vote:");
			WWWForm wWWForm = new WWWForm();
			wWWForm.AddField("vote", id.ToString() ?? "");
			wWWForm.AddField("idLang", idLang);
			wWWForm.AddField("submit", "Send");
			using UnityWebRequest www = UnityWebRequest.Post("http://ylva.php.xdomain.jp/script/vote/vote.php", wWWForm);
			await www.SendWebRequest();
			if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
			{
				if (ShowNetError)
				{
					EClass.ui.Say(www.error);
				}
				return false;
			}
			Debug.Log(www.downloadHandler.text);
			return true;
		}
		catch
		{
			return true;
		}
	}

	public static async UniTask<List<VoteLog>> GetVote(string idLang)
	{
		List<VoteLog> list = new List<VoteLog>();
		try
		{
			string uri = $"http://ylva.php.xdomain.jp/script/vote/logs/data_{idLang}.txt";
			using UnityWebRequest www = UnityWebRequest.Get(uri);
			await www.SendWebRequest();
			if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
			{
				if (ShowNetError)
				{
					EClass.ui.Say(www.error);
				}
			}
			else
			{
				StringReader stringReader = new StringReader(www.downloadHandler.text);
				int num = 0;
				for (string text = stringReader.ReadLine(); text != null; text = stringReader.ReadLine())
				{
					string[] array = text.Split(',');
					if (num == 0)
					{
						list.Add(new VoteLog
						{
							name = array[0].Replace("\"", ""),
							time = array[2].ToInt(),
							index = num
						});
					}
					else
					{
						list.Add(new VoteLog
						{
							name = array[0].Replace("\"", ""),
							count = array[1].ToInt(),
							index = num
						});
					}
					num++;
				}
			}
			return list;
		}
		catch
		{
			return list;
		}
	}

	public static async UniTask<bool> SendChat(string name, string msg, ChatCategory cat, string idLang)
	{
		if (EClass.core.version.demo)
		{
			return false;
		}
		try
		{
			if (EClass.debug.enable)
			{
				idLang = "DEBUG";
			}
			Debug.Log("Start Sending Text:");
			WWWForm wWWForm = new WWWForm();
			wWWForm.AddField("submit", "Send");
			wWWForm.AddField("name", name);
			wWWForm.AddField("msg", msg);
			wWWForm.AddField("cat", cat.ToString());
			wWWForm.AddField("idLang", idLang);
			try
			{
				using UnityWebRequest www = UnityWebRequest.Post("http://ylva.php.xdomain.jp/script/chat/chat.php", wWWForm);
				await www.SendWebRequest();
				if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
				{
					if (ShowNetError)
					{
						EClass.ui.Say(www.error);
					}
					return false;
				}
				Debug.Log(www.downloadHandler.text);
				return true;
			}
			catch
			{
			}
		}
		catch
		{
		}
		return false;
	}

	public static async UniTask<List<ChatLog>> GetChat(ChatCategory cat, string idLang)
	{
		List<ChatLog> list = new List<ChatLog>();
		try
		{
			if (EClass.debug.enable)
			{
				idLang = "DEBUG";
			}
			string uri = $"http://ylva.php.xdomain.jp/script/chat/logs/all_{idLang}.json";
			using UnityWebRequest www = UnityWebRequest.Get(uri);
			await www.SendWebRequest();
			if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
			{
				return null;
			}
			Debug.Log("Download Chat Logs: Success");
			foreach (JToken item in (JArray)JsonConvert.DeserializeObject(www.downloadHandler.text))
			{
				list.Add(item.ToObject<ChatLog>());
			}
			foreach (ChatLog item2 in list)
			{
				item2.msg = item2.msg.Replace("\n", "").Replace("\r", "").Replace("&quot;", "\"")
					.ToTitleCase();
			}
			list.Reverse();
			return list;
		}
		catch
		{
			return list;
		}
	}
}
