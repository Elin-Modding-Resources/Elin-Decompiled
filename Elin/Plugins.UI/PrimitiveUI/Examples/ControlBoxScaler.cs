using UnityEngine;
using UnityEngine.UI;

namespace PrimitiveUI.Examples;

public class ControlBoxScaler : MonoBehaviour
{
	public Text[] textElements;

	private Vector2 screenSize;

	private void Start()
	{
		screenSize = new Vector2(Screen.width, Screen.height);
		Text[] array = textElements;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].fontSize = Mathf.FloorToInt(Screen.height / 20);
		}
	}

	private void Update()
	{
		if ((float)Screen.width != screenSize.x || (float)Screen.height != screenSize.y)
		{
			Text[] array = textElements;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].fontSize = Mathf.FloorToInt(Screen.height / 20);
			}
			screenSize = new Vector2(Screen.width, Screen.height);
		}
	}
}
