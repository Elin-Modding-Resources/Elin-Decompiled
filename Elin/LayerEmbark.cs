using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayerEmbark : ELayer
{
	public enum Mode
	{
		top,
		previewMap,
		member,
		map
	}

	public static LayerEmbark Instance;

	public UIMapPreview mapPreview;

	public GameObject goTop;

	public GameObject goPreviewMap;

	public GameObject goMembers;

	public GameObject goMap;

	public UICharaMaker maker;

	public UIMapSelector selector;

	public UIList listMembers;

	public InputField inputHomeName;

	public InputField inputPlayerAlias;

	public InputField inputSupplyType;

	public InputField inputRegion;

	public InputField inputEra;

	public InputField inputPlayerName;

	public UICardInfo cardInfo;

	public UIButton buttonRerollSupply;

	public UIButton toggleSkipQuests;

	public UIButton buttonEmbark;

	public UISelectableGroup groupGameMode;

	public RectTransform mapHolder1;

	public RectTransform mapHolder2;

	public Mode mode;

	public Mode lastMode;

	public EmbarkActor moldActor;

	public GridLayoutGroup gridPreview;

	public UIMapPreview moldPreview;

	public List<UIMapPreview> previews;

	private bool firstPreview = true;

	public GameBlueprint bp => ELayer.game.bp;

	public LayerTitle title => LayerTitle.Instance;

	public override void OnAfterAddLayer()
	{
		Instance = this;
		if (ELayer.game == null)
		{
			Game.Create();
		}
		if (bp.map != null)
		{
			mapPreview.SetMap(bp.map);
		}
		if (!LayerTitle.actor)
		{
			LayerTitle.actor = Util.Instantiate(moldActor);
		}
		RefreshMembers();
		SwitchMode(Mode.top);
		ELayer.ui.AddLayer<LayerEditBio>();
	}

	public void OnClickBack()
	{
		if (mode == Mode.previewMap && lastMode != Mode.map)
		{
			SE.Click();
		}
		SwitchMode(Mode.top);
	}

	public void SwitchMode(int i)
	{
		SwitchMode(i.ToEnum<Mode>());
	}

	public void SwitchMode(Mode _mode)
	{
		bool flag = lastMode == Mode.map;
		if (_mode == Mode.top && mode == Mode.previewMap && flag)
		{
			_mode = Mode.map;
		}
		lastMode = mode;
		mode = _mode;
		goPreviewMap.SetActive(mode == Mode.previewMap);
		goTop.SetActive(mode == Mode.top);
		goMembers.SetActive(mode == Mode.member);
		buttonEmbark.SetActive(mode == Mode.top);
		goMap.SetActive(mode == Mode.map);
		switch (mode)
		{
		case Mode.top:
			mapPreview.transform.SetParent(mapHolder1, worldPositionStays: false);
			ELayer.ui.hud.hint.Show("hintEmbarkTop".lang(), icon: false);
			selector.WriteNote(ELayer.player.zone);
			break;
		case Mode.previewMap:
			SE.Play("click_paper");
			ELayer.ui.hud.hint.Show("hintEmbarkPreview".lang(), icon: false);
			RerollPreviewMap();
			break;
		case Mode.member:
			ELayer.ui.AddLayer<LayerEditBio>().SetChara(null, delegate
			{
				RefreshMembers();
				SwitchMode(Mode.top);
			});
			break;
		case Mode.map:
			mapPreview.transform.SetParent(mapHolder2, worldPositionStays: false);
			ELayer.ui.hud.hint.Show("hintEmbarkMap".lang(), icon: false);
			break;
		}
	}

	public override bool OnBack()
	{
		if (mode == Mode.map)
		{
			return false;
		}
		if (mode != Mode.top)
		{
			SwitchMode(Mode.top);
			return false;
		}
		return base.OnBack();
	}

	private void Update()
	{
		if (mode == Mode.map)
		{
			ActionMode.Title.InputMovement();
		}
	}

	public override void OnKill()
	{
	}

	public void RerollPreviewMap()
	{
		if (firstPreview)
		{
			for (int i = 0; i < 8; i++)
			{
				UIMapPreview p = Util.Instantiate(moldPreview, gridPreview);
				previews.Add(p);
				p.button.onClick.AddListener(delegate
				{
					if (!p.thread.done)
					{
						SE.Beep();
					}
					else
					{
						OnClickPreview(p);
					}
				});
			}
			firstPreview = false;
		}
		foreach (UIMapPreview preview in previews)
		{
			preview.GenerateMap(bp);
		}
	}

	public void OnClickPreview(UIMapPreview preview)
	{
		bp.map = preview.map;
		bp.genSetting = preview.thread.bp.genSetting;
		mapPreview.SetMap(bp.map);
		SwitchMode(Mode.top);
	}

	public void RefreshMembers()
	{
		listMembers.Clear();
		listMembers.callbacks = new UIList.Callback<Chara, ButtonChara>
		{
			onInstantiate = delegate(Chara a, ButtonChara b)
			{
				b.SetChara(a, ButtonChara.Mode.Embark);
				a.elements.ListBestSkills();
				b.item.button2.SetActive(a == ELayer.player.chara);
				b.item.button1.SetActive(a == ELayer.player.chara);
				b.item.button1.onClick.AddListener(delegate
				{
					ELayer.ui.AddLayer<LayerEditBio>().SetChara(null, delegate
					{
						RefreshMembers();
					});
				});
			}
		};
		foreach (Chara chara in bp.charas)
		{
			if (chara.IsPCC)
			{
				listMembers.Add(chara);
			}
		}
		listMembers.Refresh();
	}

	public void RerollPC()
	{
		bp.charas.Remove(ELayer.player.chara);
		bp.charas.Insert(0, ELayer.player.chara);
		(listMembers.buttons[0].component as ButtonChara).SetChara(ELayer.player.chara, ButtonChara.Mode.Embark);
	}

	public void RerollMembers()
	{
		bp.RerollChara();
		RefreshMembers();
	}

	public string GetAlias()
	{
		if (ELayer.rnd(4) == 0)
		{
			return ELayer.player.title;
		}
		return bp.charas.RandomItem().Name;
	}

	public void RerollPlayerAlias()
	{
		ELayer.player.title = WordGen.Get("title");
	}

	public void RerollPlayerName()
	{
		ELayer.player.chara.c_altName = NameGen.getRandomName();
	}

	public void OnEndEditPlayerName()
	{
		ELayer.player.chara.c_altName = inputPlayerName.text;
	}

	public void ListPlayerAlias()
	{
		ELayer.ui.AddLayer<LayerList>().SetStringList(() => WordGen.GetList("title"), delegate(int a, string b)
		{
			ELayer.player.title = b;
		});
	}
}
