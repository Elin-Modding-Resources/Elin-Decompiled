using System;
using System.Collections.Generic;
using UnityEngine;

public class ContentCodex : EContent
{
	public UIDynamicList list;

	public UIText textName;

	public UIText textOwn;

	public UIText textValue;

	public UIText textKills;

	public UIText textWeakspot;

	public UIText textSpawns;

	public UIText textBonus;

	public UINote note;

	public CodexCreature currentCodex;

	public Transform transInfo;

	public UIButton buttonTake;

	public UIButton buttonAddCards;

	public UIButton buttonToggleCollect;

	public Vector2 pivot;

	public Vector2 localPos;

	public override void OnSwitchContent(int idTab)
	{
		if (list.isBuilt)
		{
			if (currentCodex != null)
			{
				list.Select(currentCodex);
				RefreshInfo();
			}
		}
		else
		{
			RefreshList();
			buttonToggleCollect.SetToggle(EClass.game.config.autoCollectCard, delegate(bool a)
			{
				EClass.game.config.autoCollectCard = a;
			});
		}
	}

	public void RefreshList()
	{
		currentCodex = null;
		list.Clear();
		list.callbacks = new UIList.Callback<CodexCreature, ButtonGrid>
		{
			onClick = delegate(CodexCreature a, ButtonGrid b)
			{
				currentCodex = a;
				RefreshInfo();
				list.Select(a);
			},
			onRedraw = delegate(CodexCreature a, ButtonGrid b, int i)
			{
				b.refObj = a.source;
				b.mainText.SetText(a.Name);
				b.subText.text = a.numCard.ToString() ?? "";
				b.subText2.SetText(a.source._id + ".");
				if (EClass.core.uiScale < 1.2f)
				{
					b.icon.transform.localScale = new Vector3(1f / EClass.core.uiScale, 1f / EClass.core.uiScale, 1f);
				}
				int idSkin = ((EClass.core.config.game.antiSpider && a.source.race == "spider" && a.source.tiles.Length > 1) ? 1 : 0);
				a.SetImage(b.icon, idSkin);
			},
			onList = delegate
			{
				List<CodexCreature> list = new List<CodexCreature>();
				foreach (KeyValuePair<string, CodexCreature> creature in EClass.player.codex.creatures)
				{
					if (creature.Value.numCard > 0 && EClass.sources.charas.map.ContainsKey(creature.Value.id))
					{
						list.Add(creature.Value);
					}
				}
				list.Sort((CodexCreature a, CodexCreature b) => a.source._id - b.source._id);
				foreach (CodexCreature item in list)
				{
					this.list.Add(item);
				}
			}
		};
		list.List();
		bool flag = list.objects.Count > 0;
		if (flag)
		{
			list.dsv.scrollByItemIndex(0);
			list.Select(list.objects[0], invoke: true);
			currentCodex = list.objects[0] as CodexCreature;
		}
		transInfo.SetActive(flag);
		buttonTake.SetActive(flag);
	}

	public void RefreshInfo()
	{
		CodexCreature codexCreature = currentCodex;
		if (codexCreature != null)
		{
			UINote uINote = note;
			SourceChara.Row source = codexCreature.source;
			uINote.Clear();
			UIItem uIItem = uINote.AddHeader("HeaderCodex", codexCreature.Name.Replace("『", Environment.NewLine + "『"));
			int idSkin = ((EClass.core.config.game.antiSpider && codexCreature.source.race == "spider" && codexCreature.source.tiles.Length > 1) ? 1 : 0);
			codexCreature.SetImage(uIItem.image1, idSkin);
			uIItem.text2.text = "Lv." + source.LV + Environment.NewLine + EClass.sources.races.map[source.race].GetName().ToTitleCase(wholeText: true) + Environment.NewLine + EClass.sources.jobs.map[source.job].GetName().ToTitleCase(wholeText: true);
			uIItem.image1.rectTransform.pivot = pivot;
			uIItem.image1.rectTransform.localPosition = localPos;
			uIItem.image1.rectTransform.localScale = new Vector3(0f - uIItem.image1.transform.localScale.x, uIItem.image1.transform.localScale.y, 1f);
			uINote.Build();
			textOwn.text = codexCreature.numCard.ToString() ?? "";
			textValue.text = codexCreature.source.value.ToString() ?? "";
			textKills.text = codexCreature.kills.ToString() ?? "";
			textWeakspot.SetText((codexCreature.weakspot == 0) ? "undiscovered".lang() : "discovered".lang(codexCreature.weakspot.ToString() ?? ""), (codexCreature.weakspot == 0) ? FontColor.Default : FontColor.Good);
			textSpawns.text = codexCreature.spawns.ToString() ?? "";
			textBonus.text = codexCreature.GetTextBonus();
			currentCodex = codexCreature;
		}
	}

	public void OnClickAddCards()
	{
		List<Thing> list = EClass.pc.things.List((Thing c) => c.id == "figure3", onlyAccessible: true);
		int num = 0;
		if (list.Count > 0)
		{
			foreach (Thing item in list)
			{
				EClass.player.codex.AddCard(item.c_idRefCard, item.Num);
				num += item.Num;
				item.Destroy();
				RefreshList();
			}
			SE.Play("use_card");
			Msg.Say("addedCards", num, num.ToString() ?? "");
		}
		else
		{
			SE.BeepSmall();
			Msg.Say("noCard");
		}
		RefreshInfo();
	}

	public static void Collect(Thing t)
	{
		EClass.player.codex.AddCard(t.c_idRefCard, t.Num);
		SE.Play("use_card");
		Msg.Say("addedCards", t.Num, t.Num.ToString() ?? "");
		t.Destroy();
	}

	public void OnClickGetCard()
	{
		currentCodex.numCard--;
		Thing thing = ThingGen.Create("figure3");
		thing.MakeFigureFrom(currentCodex.id);
		bool autoCollectCard = EClass.game.config.autoCollectCard;
		EClass.game.config.autoCollectCard = false;
		EClass.pc.Pick(thing);
		EClass.game.config.autoCollectCard = autoCollectCard;
		if (currentCodex.numCard == 0)
		{
			RefreshList();
			return;
		}
		list.Redraw();
		list.Select(currentCodex);
		RefreshInfo();
	}
}
