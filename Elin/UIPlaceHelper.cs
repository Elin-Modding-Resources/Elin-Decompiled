using UnityEngine;

public class UIPlaceHelper : EMono
{
	public UIRawImage bgGrid;

	public ButtonGrid buttonHeight;

	public ButtonGrid buttonFreePlace;

	public ButtonGrid buttonAutoWall;

	public void Refresh()
	{
		Recipe recipe = HotItemHeld.recipe;
		bool flag = recipe != null;
		if (EMono.player.currentHotItem.Thing == null || (EMono.player.currentHotItem.Thing.trait.IsTool && !HotItemHeld.disableTool))
		{
			flag = false;
		}
		base.gameObject.SetActive(flag);
		if (flag)
		{
			ActionMode.Build.recipe = recipe;
			buttonAutoWall.SetActive(enable: false);
			buttonAutoWall.mainText.text = (EMono.game.config.autoWall ? "On" : "Off");
			buttonAutoWall.icon.SetAlpha(EMono.game.config.autoWall ? 1f : 0.4f);
			buttonAutoWall.SetOnClick(delegate
			{
				SE.Tab();
				EMono.game.config.autoWall = !EMono.game.config.autoWall;
				Refresh();
			});
			buttonFreePlace.SetActive(enable: false);
			buttonFreePlace.mainText.text = (EMono.game.config.freePos ? "On" : "Off");
			buttonFreePlace.icon.SetAlpha(EMono.game.config.freePos ? 1f : 0.4f);
			buttonFreePlace.SetOnClick(delegate
			{
				SE.Tab();
				EMono.game.config.freePos = !EMono.game.config.freePos;
				Refresh();
			});
			buttonHeight.SetActive(recipe.MaxAltitude != 0);
			buttonHeight.mainText.text = ((ActionMode.Build.altitude >= 0) ? "+" : "") + ActionMode.Build.altitude;
			buttonHeight.SetOnClick(delegate
			{
				SE.Tab();
				ActionMode.Build.ModAltitude(1);
				Refresh();
			});
			buttonHeight.onRightClick = delegate
			{
				SE.Tab();
				ActionMode.Build.ModAltitude(-1);
				Refresh();
			};
			buttonHeight.onInputWheel = delegate(int a)
			{
				SE.Tab();
				ActionMode.Build.ModAltitude(a);
				Refresh();
			};
			bgGrid.uvRect = new Rect(1f, 1f, this.GetComponentsInDirectChildren<ButtonGrid>(includeInactive: false).Count, 1f);
			this.RebuildLayout();
		}
	}
}
