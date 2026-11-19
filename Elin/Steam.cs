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
		bool cancelled = false;
		LayerProgress.Start("Uploading").onCancel = delegate
		{
			cancelled = true;
		};
		p.UpdateMeta(updateOnly: true);
		currentPackage = p;
		QueryAllMyPublished(delegate(List<WorkshopItem> items)
		{
			if (!cancelled)
			{
				CreateOrUpdateUserContent(items);
			}
		}, delegate(string error)
		{
			if (!cancelled)
			{
				OnPublishError(error);
			}
		});
	}

	private static void OnPublishError(string error)
	{
		LayerProgress.completed = true;
		Debug.LogError(error);
		Dialog.Ok("mod_publish_error".lang() + "\n\n" + error);
	}

	private static string IsUgcError(string error, EResult? result)
	{
		switch (result)
		{
		default:
		{
			EResult? eResult = result;
			return error + " (" + eResult.ToString() + ")";
		}
		case null:
		case EResult.k_EResultNone:
		case EResult.k_EResultOK:
			return error;
		}
	}

	public static void QueryAllMyPublished(Action<List<WorkshopItem>> onComplete, Action<string> onFail = null, float timeout = 60f)
	{
		List<WorkshopItem> results = new List<WorkshopItem>();
		HashSet<PublishedFileId_t> ids = new HashSet<PublishedFileId_t>();
		AppId_t app = SteamUtils.GetAppID();
		AccountID_t account = SteamUser.GetSteamID().GetAccountID();
		CallResult<SteamUGCQueryCompleted_t> callResult = CallResult<SteamUGCQueryCompleted_t>.Create();
		UGCQueryHandle_t handle = UGCQueryHandle_t.Invalid;
		uint page = 0u;
		bool done = false;
		float time = 0f;
		NextPage();
		if (!done && (bool)Instance)
		{
			Instance.StartCoroutine(Timeout());
		}
		void Fail(string reason)
		{
			if (!done)
			{
				done = true;
				callResult.Cancel();
				Release();
				string text = "UgcQuery " + reason + " at page " + page + " (" + results.Count + " items fetched)";
				if (onFail != null)
				{
					onFail(text);
				}
				else
				{
					Debug.LogError(text);
				}
			}
		}
		void NextPage()
		{
			page++;
			time = Time.realtimeSinceStartup;
			handle = SteamUGC.CreateQueryUserUGCRequest(account, EUserUGCList.k_EUserUGCList_Published, EUGCMatchingUGCType.k_EUGCMatchingUGCType_Items_ReadyToUse, EUserUGCListSortOrder.k_EUserUGCListSortOrder_CreationOrderDesc, app, app, page);
			if (handle == UGCQueryHandle_t.Invalid)
			{
				Fail("could not be created");
			}
			else
			{
				SteamUGC.SetReturnKeyValueTags(handle, bReturnKeyValueTags: true);
				SteamUGC.SetReturnMetadata(handle, bReturnMetadata: true);
				SteamUGC.SetReturnLongDescription(handle, bReturnLongDescription: true);
				SteamAPICall_t steamAPICall_t = SteamUGC.SendQueryUGCRequest(handle);
				if (steamAPICall_t == SteamAPICall_t.Invalid)
				{
					Fail("could not be sent");
				}
				else
				{
					callResult.Set(steamAPICall_t, OnQueryCompleted);
				}
			}
		}
		void OnQueryCompleted(SteamUGCQueryCompleted_t r, bool ioFailure)
		{
			if (!done)
			{
				if (ioFailure || r.m_eResult != EResult.k_EResultOK)
				{
					Fail("failed with " + (ioFailure ? "IO failure" : r.m_eResult.ToString()));
				}
				else
				{
					for (uint num = 0u; num < r.m_unNumResultsReturned; num++)
					{
						if (SteamUGC.GetQueryUGCResult(r.m_handle, num, out var pDetails) && ids.Add(pDetails.m_nPublishedFileId))
						{
							WorkshopItem workshopItem = new WorkshopItem(pDetails);
							SteamUGC.GetQueryUGCMetadata(r.m_handle, num, out workshopItem.metadata, 5000u);
							uint queryUGCNumKeyValueTags = SteamUGC.GetQueryUGCNumKeyValueTags(r.m_handle, num);
							workshopItem.keyValueTags = new StringKeyValuePair[queryUGCNumKeyValueTags];
							for (uint num2 = 0u; num2 < queryUGCNumKeyValueTags; num2++)
							{
								SteamUGC.GetQueryUGCKeyValueTag(r.m_handle, num, num2, out var pchKey, 255u, out var pchValue, 255u);
								workshopItem.keyValueTags[num2] = new StringKeyValuePair
								{
									key = pchKey?.Trim(),
									value = pchValue?.Trim()
								};
							}
							results.Add(workshopItem);
						}
					}
					Release();
					if (r.m_unNumResultsReturned == 0 || page * 50 >= r.m_unTotalMatchingResults)
					{
						done = true;
						onComplete?.Invoke(results);
					}
					else
					{
						EClass.core.actionsNextFrame.Add(NextPage);
					}
				}
			}
		}
		void Release()
		{
			if (!(handle == UGCQueryHandle_t.Invalid))
			{
				SteamUGC.ReleaseQueryUGCRequest(handle);
				handle = UGCQueryHandle_t.Invalid;
			}
		}
		IEnumerator Timeout()
		{
			while (!done)
			{
				if (Time.realtimeSinceStartup - time > timeout)
				{
					Fail("timed out after " + timeout + "s");
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
				string errorMessage = result.errorMessage;
				ref SubmitItemUpdateResult_t? submitItemUpdateResult = ref result.submitItemUpdateResult;
				OnPublishError(IsUgcError(errorMessage, submitItemUpdateResult.HasValue ? new EResult?(submitItemUpdateResult.GetValueOrDefault().m_eResult) : result.createItemResult?.m_eResult));
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
				OnPublishError(IsUgcError(result.errorMessage, result.submitItemUpdateResult?.m_eResult));
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
		WorkshopItemData result = new WorkshopItemData
		{
			appId = steamworks.settings.applicationId,
			title = p.title,
			description = p.description,
			content = directoryInfo,
			preview = fileInfo,
			metadata = (p.id ?? ""),
			tags = p.tags,
			visibility = ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityPublic
		};
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
