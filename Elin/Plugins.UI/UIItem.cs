using UnityEngine;
using UnityEngine.UI;

public class UIItem : MonoBehaviour
{
	public UIText text1;

	public UIText text2;

	public UIText text3;

	public UIText text4;

	public Image image1;

	public Image image2;

	public UIButton button1;

	public UIButton button2;

	public UIButton button3;

	public UIItem item;

	public Toggle toggle;

	public UIDropdown dd;

	public object refObj;

	public void SetTopic(string lang1, string lang2)
	{
		text1.SetText(lang1.lang());
		text2.SetText(lang2.lang());
	}

	public void SetWidth(int w)
	{
		LayoutElement orCreate = this.GetOrCreate<LayoutElement>();
		float preferredWidth = (orCreate.minWidth = w);
		orCreate.preferredWidth = preferredWidth;
	}

	public void Hyphenate()
	{
		if (Lang.setting.hyphenation)
		{
			HyphenationJpn hyphenationJpn = text1.GetComponent<HyphenationJpn>() ?? text1.gameObject.AddComponent<HyphenationJpn>();
			if ((bool)hyphenationJpn)
			{
				text1.hyphenation = hyphenationJpn;
				hyphenationJpn.enabled = true;
				text1.SetText(text1.text);
			}
		}
	}
}
