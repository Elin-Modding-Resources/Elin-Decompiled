using UnityEngine;

public class ManlySingleton<T> : SingletonBase where T : MonoBehaviour
{
	private static T myInstance;

	public static T Instance
	{
		get
		{
			if (myInstance == null)
			{
				Object[] array = Object.FindObjectsOfType(typeof(T));
				if (array.Length > 1)
				{
					Debug.LogError("<B>Doubleton?</B> Do you really want two instances of <B><i>" + typeof(T).Name + "</i></B>?\n", array[1]);
				}
				if (array.Length >= 1)
				{
					myInstance = (T)array[0];
				}
				if (myInstance == null)
				{
					Debug.LogError("An instance of " + typeof(T).Name + " could not be found. Add it to the scene.");
				}
			}
			return myInstance;
		}
	}

	public static T InstanceOrNull => myInstance ?? (myInstance = (T)Object.FindObjectOfType(typeof(T)));

	public static Transform STransform => Instance.transform;

	public static Vector3 SPosition
	{
		get
		{
			return Instance.transform.position;
		}
		set
		{
			Instance.transform.position = value;
		}
	}

	public static GameObject SGameObject => Instance.gameObject;

	public static bool Exists()
	{
		return InstanceOrNull != null;
	}

	public override void Elect()
	{
		myInstance = this as T;
	}

	protected void Abdicate()
	{
		if (myInstance == this)
		{
			myInstance = null;
		}
	}

	protected virtual void OnDestroy()
	{
		myInstance = null;
	}
}
