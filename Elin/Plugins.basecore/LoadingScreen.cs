using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
	public Text moldText;

	public LayoutGroup layout;

	public Text Log(string s)
	{
		Debug.Log(s);
		Text text = Util.Instantiate(moldText, layout);
		text.text = s;
		return text;
	}
}
