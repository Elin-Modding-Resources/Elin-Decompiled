public class TerrainMenu : EMono
{
	public UISlider sliderRadius;

	public ActionMode am;

	public UISelectableGroup group;

	public int radius;

	public void Show(ActionMode _am)
	{
		bool useSubMenu = _am.UseSubMenu;
		am = _am;
		base.gameObject.SetActive(useSubMenu);
		if (!useSubMenu)
		{
			return;
		}
		sliderRadius.value = am.brushRadius;
		sliderRadius.transform.parent.SetActive(am.UseSubMenuSlider);
		group.checkbox = !am.SubMenuAsGroup;
		foreach (UIButton componentsInDirectChild in group.GetComponentsInDirectChildren<UIButton>())
		{
			componentsInDirectChild.SetActive(enable: true);
		}
		group.Init(am.SubMenuModeIndex, delegate(int a)
		{
			am.OnClickSubMenu(a);
		});
		for (int num = 0; num < group.list.Count; num++)
		{
			UIButton uIButton = group.list[num];
			string text = am.OnSetSubMenuButton(num, uIButton);
			uIButton.SetActive(text != null);
			if (text != null)
			{
				uIButton.mainText.SetText(text.lang());
			}
		}
		this.RebuildLayout(recursive: true);
	}

	public void OnChangeRadius(float a)
	{
		am.brushRadius = (int)a;
		sliderRadius.textMain.text = "radius".lang() + ": " + am.brushRadius;
	}
}
