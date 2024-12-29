using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHighlight : MonoBehaviour, IUISkin
{
	public class Highlight
	{
		public Component compo;

		public Highlight(Component compo)
		{
			this.compo = compo;
		}

		public void Kill()
		{
			if ((bool)compo && (bool)compo.gameObject)
			{
				Object.Destroy(compo.gameObject);
			}
		}
	}

	public Image imageHighlight;

	protected List<Highlight> list = new List<Highlight>();

	private void Awake()
	{
		ApplySkin();
	}

	public void Add(Component target)
	{
		Image image = Object.Instantiate(imageHighlight);
		image.transform.SetParent(target.transform, worldPositionStays: false);
		list.Add(new Highlight(image));
	}

	public void Clear()
	{
		for (int num = list.Count - 1; num >= 0; num--)
		{
			list[num].Kill();
		}
		list.Clear();
	}

	public void ApplySkin()
	{
	}
}
