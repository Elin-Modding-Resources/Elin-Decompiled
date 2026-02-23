using UnityEngine;
using UnityEngine.UI;

public class UIHangIcon : EMono
{
	public static int clicked;

	public Image image;

	public UIButton button;

	public string id;

	public bool rightClick;

	public bool windowCorner;

	private Sprite original;

	public static Sprite lastCorner;

	private void Awake()
	{
		if (!image && (bool)button)
		{
			image = button.image;
		}
		original = image.sprite;
		if ((bool)button && !rightClick)
		{
			button.onClick.AddListener(OnClick);
		}
		Refresh();
	}

	public void OnClickCorner()
	{
		clicked++;
		Debug.Log(clicked);
		if (clicked >= 50)
		{
			Steam.GetAchievement(ID_Achievement.WINDOW_CHARM);
		}
		SE.Play("Ambience/Random/windchime1");
	}

	public void OnClick()
	{
		if (!EMono.core.IsGameStarted)
		{
			return;
		}
		ActionMode.DefaultMode.Activate();
		if (EMono.ui.contextMenu.isActive)
		{
			EMono.ui.contextMenu.currentMenu.Hide();
			EInput.rightMouse.Consume();
		}
		LayerCollectible layerCollectible = EMono.ui.ToggleLayer<LayerCollectible>();
		if (layerCollectible == null)
		{
			return;
		}
		EMono.ui.hud.hint.Show("h_hang");
		layerCollectible.onClick = delegate(Hoard.Item a)
		{
			EMono.player.hangIcons[id] = a.id;
			if (this != null && base.gameObject != null)
			{
				Refresh();
			}
			return true;
		};
	}

	public void Refresh()
	{
		if (!Core.Instance)
		{
			return;
		}
		if (!EMono.core.IsGameStarted || (windowCorner && !EMono.core.config.ui.cornerHoard))
		{
			image.sprite = (lastCorner = EMono.core.refs.spritesCorner.NextItem(lastCorner));
		}
		else
		{
			string str = ((!windowCorner) ? EMono.player.hangIcons.TryGetValue(id) : EMono.player.hoard.items.RandomItem()?.id);
			if (!str.IsEmpty())
			{
				image.sprite = EMono.player.hoard.GetSprite(str);
			}
		}
		image.SetNativeSize();
	}
}
