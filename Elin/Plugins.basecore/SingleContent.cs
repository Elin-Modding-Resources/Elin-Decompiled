using System.Collections.Generic;
using UnityEngine;

public class SingleContent : MonoBehaviour
{
	public List<GameObject> exclude;

	public void Select(GameObject go)
	{
		if (exclude.Contains(go))
		{
			return;
		}
		foreach (Transform componentsInDirectChild in this.GetComponentsInDirectChildren<Transform>())
		{
			if (!exclude.Contains(componentsInDirectChild.gameObject) && !componentsInDirectChild.gameObject.tag.Contains("PivotTooltip") && componentsInDirectChild.gameObject != go)
			{
				componentsInDirectChild.gameObject.SetActive(value: false);
			}
		}
	}
}
