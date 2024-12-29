using UnityEngine.UI;

public class EventAwareSlider : Slider
{
	public void SetValue(float value, bool sendEvent)
	{
		Set(value, sendEvent);
	}
}
