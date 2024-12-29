using System;
using UnityEngine;
using UnityEngine.UI;

public class UINote : MonoBehaviour
{
	public RectTransform target;

	public UINoteProfile prof;

	public SkinType skinType;

	public string idDefaultText;

	[NonSerialized]
	public LayoutGroup layout;

	[NonSerialized]
	public RectTransform cur;

	public bool extraRebuild;

	private SkinRootStatic oldTempSkin;

	private void Awake()
	{
		if (!target)
		{
			target = this.Rect();
		}
		cur = target;
		layout = GetComponent<LayoutGroup>();
	}

	public void Clear()
	{
		if (!layout)
		{
			layout = GetComponent<LayoutGroup>();
		}
		layout.DestroyChildren(destroyInactive: true);
		oldTempSkin = SkinManager.tempSkin;
		if (skinType != 0)
		{
			SkinManager.tempSkin = SkinManager.CurrentSkin.GetSkin(skinType);
		}
	}

	public void Build()
	{
		if (extraRebuild)
		{
			this.RebuildLayoutTo<Layer>();
		}
		this.RebuildLayout(recursive: true);
		this.RebuildLayoutTo<Layer>();
		this.RebuildLayoutTo<UIContent>();
		this.RebuildLayoutTo<UIContent>();
		SkinManager.tempSkin = oldTempSkin;
	}

	public void Space(int sizeY = 0, int sizeX = 1)
	{
		RectTransform rectTransform = Load<Transform>("UI/Element/Deco/Space").Rect();
		rectTransform.sizeDelta = ((sizeY == 0) ? prof.sizeSpace : new Vector2(sizeX, sizeY));
		if (sizeX != 1)
		{
			rectTransform.GetComponent<LayoutElement>().preferredWidth = sizeX;
		}
	}

	public UINote AddNote(string id)
	{
		UINote componentInChildren = Load<Transform>("UI/Element/Note/" + id).gameObject.GetComponentInChildren<UINote>();
		extraRebuild = true;
		return componentInChildren;
	}

	public UIItem AddHeader(string text, Sprite sprite = null)
	{
		return AddHeader("HeaderNote", text, sprite);
	}

	public UIItem AddHeaderCard(string text, Sprite sprite = null)
	{
		return AddHeader("HeaderCard", text, sprite);
	}

	public UIItem AddHeaderTopic(string text, Sprite sprite = null)
	{
		return AddHeader("HeaderTopic", text, sprite);
	}

	public UIItem AddHeader(string id, string text, Sprite sprite = null)
	{
		UIItem uIItem = Load("UI/Element/Header/" + id);
		uIItem.text1.text = text.lang();
		if ((bool)uIItem.image1)
		{
			if ((bool)sprite)
			{
				uIItem.image1.sprite = sprite;
				uIItem.image1.SetNativeSize();
			}
			else
			{
				uIItem.image1.SetActive(enable: false);
			}
		}
		return uIItem;
	}

	public UIItem AddText(string text, FontColor color = FontColor.DontChange)
	{
		return AddText(null, text, color);
	}

	public UIItem AddText(string id, string text, FontColor color = FontColor.DontChange)
	{
		UIItem uIItem = Load("UI/Element/Text/" + id.IsEmpty(idDefaultText.IsEmpty("NoteText")));
		if (color != 0)
		{
			uIItem.text1.SetText(text.lang(), color);
		}
		else
		{
			uIItem.text1.SetText(text.lang());
		}
		return uIItem;
	}

	public UIItem AddText(string id, string text, Color color)
	{
		UIItem uIItem = Load("UI/Element/Text/" + id.IsEmpty(idDefaultText.IsEmpty("NoteText")));
		uIItem.text1.SetText(text.lang(), color);
		return uIItem;
	}

	public UIItem AddItem(string id)
	{
		return Load("UI/Element/Item/" + id);
	}

	public UIItem AddTopic(string id, string text, string value = null)
	{
		UIItem uIItem = Load("UI/Element/Text/" + id);
		uIItem.text1.SetText(text.lang());
		uIItem.text2.SetText(value);
		uIItem.text2.SetActive(!value.IsEmpty());
		return uIItem;
	}

	public UIItem AddTopic(string text, string value = null)
	{
		return AddTopic("TopicDefault", text, value);
	}

	public void AddImage(Sprite sprite)
	{
		Image image = Load("UI/Element/Deco/ImageNote").image1;
		image.sprite = sprite;
		image.SetNativeSize();
		image.transform.parent.Rect().sizeDelta = image.Rect().sizeDelta;
		if (sprite == null)
		{
			image.transform.parent.SetActive(enable: false);
		}
	}

	public void AddImage(string idFile)
	{
		Image image = Load("UI/Element/Deco/ImageNote").image1;
		Sprite sprite2 = (image.sprite = Resources.Load<Sprite>("Media/Graphics/Image/" + idFile));
		image.SetNativeSize();
		image.transform.parent.Rect().sizeDelta = image.Rect().sizeDelta;
		if (sprite2 == null)
		{
			image.transform.parent.SetActive(enable: false);
		}
	}

	public UIButton AddButton(string text, Action onClick)
	{
		UIButton uIButton = Load<UIButton>("UI/Element/Button/ButtonNote");
		uIButton.mainText.text = text;
		uIButton.onClick.AddListener(delegate
		{
			onClick();
		});
		return uIButton;
	}

	public UIButton AddButtonLink(string text, string url)
	{
		UIButton button = Load<UIItem>("UI/Element/Item/ItemNoteLink").button1;
		button.mainText.text = text;
		button.onClick.AddListener(delegate
		{
			SE.Click();
			Application.OpenURL(url);
		});
		return button;
	}

	public UIDropdown AddDropdown(string id = "DropdownDefault")
	{
		return Load<UIDropdown>("UI/Element/Other/" + id);
	}

	public UIButton AddToggle(string idLang = "", bool isOn = false, Action<bool> action = null)
	{
		UIButton uIButton = Load<UIButton>("UI/Element/Button/ButtonToggle");
		uIButton.SetToggle(isOn, action);
		uIButton.mainText.text = idLang.lang().ToTitleCase();
		return uIButton;
	}

	public Transform AddPrefab(string path)
	{
		return Util.Instantiate(path, this);
	}

	public T AddExtra<T>(string path) where T : Component
	{
		return Util.Instantiate<T>("UI/Element/Note/Extra/" + path, this);
	}

	private UIItem Load(string path)
	{
		return Load<UIItem>(path);
	}

	private T Load<T>(string path) where T : Component
	{
		return Util.Instantiate<T>(path, layout);
	}
}
