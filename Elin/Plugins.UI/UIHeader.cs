using UnityEngine;

public class UIHeader : MonoBehaviour
{
	public string lang1;

	public UIText text1;

	public bool rebuildLayout;

	private void Start()
	{
		if (!lang1.IsEmpty())
		{
			text1.SetLang(lang1);
		}
		if (rebuildLayout)
		{
			text1.RebuildLayout();
			this.RebuildLayout();
		}
	}

	public void SetText(string s)
	{
		lang1 = null;
		text1.SetText(s.lang().ToTitleCase());
		text1.RebuildLayout();
		this.RebuildLayout();
	}
}
