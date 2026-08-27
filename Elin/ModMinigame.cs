using UnityEngine;

public class ModMinigame<T> : MiniGame where T : Component
{
	public T game;

	public void Load()
	{
		Debug.Log(path);
		if (!game)
		{
			asset = AssetBundle.LoadFromFile(path + "/Asset_" + id + "/asset");
			Object obj = asset.LoadAsset(id);
			Debug.Log(obj);
			go = Object.Instantiate(obj) as GameObject;
			Debug.Log(go);
			game = go.GetComponentInChildren<T>();
		}
		SetAudioMixer(go);
		Debug.Log(game);
	}

	public void Kill()
	{
		Object.Destroy(go);
		game = null;
		if ((bool)asset)
		{
			asset.Unload(unloadAllLoadedObjects: true);
		}
	}
}
