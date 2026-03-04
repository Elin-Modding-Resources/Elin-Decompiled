using UnityEngine;

public class LayerConsole : ELayer
{
	public ReflexConsole console => ELayer.ui.console;

	public override void OnAfterInit()
	{
		console.Open();
	}

	public override void OnUpdateInput()
	{
		if (Input.GetKeyDown(KeyCode.Escape) || EInput.action == EAction.Console)
		{
			Close();
		}
	}

	public override void OnKill()
	{
		console.Close();
	}
}
