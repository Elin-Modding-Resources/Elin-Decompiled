using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HeathenEngineering.SteamworksIntegration;
using HeathenEngineering.SteamworksIntegration.API;
using Steamworks;
using UnityEngine;

public class Steam : MonoBehaviour
{
	public static Steam Instance;

	public SteamworksBehaviour steamworks;

	public UserGeneratedContentQueryManager ugc;

	public WorkshopItem testData;

	public BaseModPackage currentPackage;

	private void Awake()
	{
		Instance = this;
	}

	public void Init()
	{
		SteamAPI.Init();
		BaseCore.IsOffline = !App.Client.LoggedOn || SteamSettings.behaviour == null;
	}

	public void CheckUpdate()
	{
	}

	public void CheckDLC()
	{
		Debug.Log(HasDLC(ID_DLC.Test));
	}

	public static bool HasDLC(ID_DLC id)
	{
		return id switch
		{
			ID_DLC.CursedManor => true, 
			ID_DLC.BackerReward => EClass.core.config.HasBackerRewardCode(), 
			_ => EClass.core.config.HasBackerRewardCode(), 
		};
	}

	public static void GetAchievement(ID_Achievement id)
	{
		if (BaseCore.IsOffline)
		{
			return;
		}
		try
		{
			foreach (AchievementObject achievement in Instance.steamworks.settings.achievements)
			{
				if (achievement.Id == id.ToString())
				{
					if (achievement.IsAchieved)
					{
						return;
					}
					try
					{
						achievement.Unlock();
						achievement.Store();
						EClass.ui.Say("sys_acv".lang(achievement.Name), Resources.Load<Sprite>("Media/Graphics/Icon/Achievement/acv_" + id));
						SE.Play("achievement");
						return;
					}
					catch (Exception ex)
					{
						Debug.LogError("Error Achievement 1:" + ex);
						return;
					}
				}
			}
		}
		catch (Exception ex2)
		{
			Debug.LogError("Error Achievement 2:" + ex2);
		}
		Debug.Log("Achievement not found:" + id);
	}

	public static void ResetAllAchievement()
	{
		foreach (AchievementObject achievement in Instance.steamworks.settings.achievements)
		{
			achievement.ClearAchievement();
			achievement.Store();
		}
	}

	public static void ResetAchievement(ID_Achievement id)
	{
		AchievementObject achievementObject = Instance.steamworks.settings.achievements.FirstOrDefault((AchievementObject a) => a.Id == id.ToString());
		if (achievementObject != null)
		{
			achievementObject.ClearAchievement();
			achievementObject.Store();
		}
	}

	public void TestHasDLC()
	{
		Debug.Log(HasDLC(ID_DLC.Test));
		Debug.Log(HasDLC(ID_DLC.CursedManor));
	}

	public void CreateUserContent(BaseModPackage p)
	{
		LayerProgress.Start("Uploading").onCancel = delegate
		{
		};
		p.UpdateMeta(updateOnly: true);
		currentPackage = p;
		QueryAllMyPublished(CreateOrUpdateUserContent, delegate(string error)
		{
			LayerProgress.completed = true;
			Debug.LogError(error);
			Dialog.Ok("mod_publish_error");
		});
	}

	public static void QueryAllMyPublished(Action<List<WorkshopItem>> onComplete, Action<string> onFail = null, float timeout = 10f)
	{
		List<WorkshopItem> results = new List<WorkshopItem>();
		HashSet<PublishedFileId_t> ids = new HashSet<PublishedFileId_t>();
		uint page = 1u;
		bool done = false;
		UgcQuery query = UgcQuery.GetMyPublished();
		ExecutePage();
		if (!done && (bool)Instance)
		{
			Instance.StartCoroutine(Timeout());
		}
		void ExecutePage()
		{
			query.SetReturnKeyValueTags(tags: true);
			if (!query.Execute(HandleResults))
			{
				ReportError("UgcQuery.Execute failed at page " + page);
			}
		}
		void HandleResults(UgcQuery q)
		{
			if (!done)
			{
				results.AddRange(q.ResultsList.Where((WorkshopItem r) => ids.Add(r.FileId)));
				if (q.ResultsList.Count == 0 || page >= q.pageCount)
				{
					done = true;
					query.Dispose();
					onComplete?.Invoke(results);
				}
				else
				{
					query.SetPage(++page);
					ExecutePage();
				}
			}
		}
		void ReportError(string error)
		{
			if (!done)
			{
				done = true;
				query.Dispose();
				if (onFail != null)
				{
					onFail(error);
				}
				else
				{
					Debug.LogError(error);
				}
			}
		}
		IEnumerator Timeout()
		{
			uint lastPage = page;
			float time = Time.realtimeSinceStartup;
			while (!done)
			{
				if (lastPage != page)
				{
					lastPage = page;
					time = Time.realtimeSinceStartup;
				}
				if (Time.realtimeSinceStartup - time > timeout)
				{
					ReportError("UgcQuery timed out at page " + page + " (" + results.Count + " items fetched)");
					break;
				}
				yield return null;
			}
		}
	}

	private void CreateOrUpdateUserContent(List<WorkshopItem> items)
	{
		Debug.Log("Creating Content2");
		BaseModPackage baseModPackage = currentPackage;
		Debug.Log(items.Count);
		foreach (WorkshopItem item in items)
		{
			if (item.keyValueTags.IsEmpty())
			{
				continue;
			}
			StringKeyValuePair[] keyValueTags = item.keyValueTags;
			for (int i = 0; i < keyValueTags.Length; i++)
			{
				StringKeyValuePair stringKeyValuePair = keyValueTags[i];
				if (stringKeyValuePair.key == "id" && stringKeyValuePair.value == baseModPackage.id && item.Owner.IsMe)
				{
					Debug.Log("Updating Content");
					UpdateUserContent(item.FileId);
					return;
				}
			}
		}
		Debug.Log("Creating Content");
		CreateItemData(baseModPackage).Create(null, null, new WorkshopItemKeyValueTag[1]
		{
			new WorkshopItemKeyValueTag
			{
				key = "id",
				value = baseModPackage.id
			}
		}, delegate(WorkshopItemDataCreateStatus result)
		{
			LayerProgress.completed = true;
			if (result.hasError)
			{
				Dialog.Ok("mod_publish_error");
				EClass.ui.Say(result.errorMessage);
				Debug.Log("error:" + result.errorMessage);
			}
			else
			{
				Dialog.Ok("mod_created");
				Debug.Log("created");
			}
		});
	}

	public void UpdateUserContent(PublishedFileId_t fileId)
	{
		Debug.Log("Updating Content");
		BaseModPackage p = currentPackage;
		WorkshopItemData workshopItemData = CreateItemData(p);
		workshopItemData.publishedFileId = fileId;
		workshopItemData.Update(delegate(WorkshopItemDataUpdateStatus result)
		{
			LayerProgress.completed = true;
			if (result.hasError)
			{
				Dialog.Ok("mod_publish_error");
				EClass.ui.Say(result.errorMessage);
				Debug.Log("error:" + result.errorMessage);
			}
			else
			{
				Dialog.Ok("mod_updated");
				Debug.Log("updated");
			}
		});
	}

	public WorkshopItemData CreateItemData(BaseModPackage p)
	{
		FileInfo fileInfo = new FileInfo(p.dirInfo.FullName + "/preview.jpg");
		DirectoryInfo directoryInfo = new DirectoryInfo(p.dirInfo.FullName);
		WorkshopItemData workshopItemData = default(WorkshopItemData);
		workshopItemData.appId = steamworks.settings.applicationId;
		workshopItemData.title = p.title;
		workshopItemData.description = p.description;
		workshopItemData.content = directoryInfo;
		workshopItemData.preview = fileInfo;
		workshopItemData.metadata = p.id ?? "";
		workshopItemData.tags = p.tags;
		workshopItemData.visibility = ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityPublic;
		WorkshopItemData result = workshopItemData;
		switch (p.visibility)
		{
		case "Unlisted":
			result.visibility = ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityUnlisted;
			break;
		case "Private":
			result.visibility = ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityPrivate;
			break;
		case "FriendsOnly":
			result.visibility = ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityFriendsOnly;
			break;
		}
		Debug.Log(App.Client.Owner.id);
		Debug.Log(result.appId);
		Debug.Log(p.id);
		Debug.Log(directoryInfo.Exists + "/" + directoryInfo.FullName);
		Debug.Log(fileInfo.Exists + "/" + fileInfo.FullName);
		return result;
	}
}
