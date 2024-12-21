using UnityEngine;

public class PrefabFix : MonoBehaviour
{
	public int siblingIndex;

	private void Awake()
	{
		base.transform.SetSiblingIndex(siblingIndex);
	}
}
