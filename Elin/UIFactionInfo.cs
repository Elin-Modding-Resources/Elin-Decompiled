using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIFactionInfo : EMono
{
	public UINote note;

	public Image imageFull;

	public Anime imageIn;

	public Anime imageOut;

	public Tween tween;

	public Transform transImage;

	public Transform pivot;

	public UIButton buttonBuy;

	public UIButton buttonVisit;

	public UIButton buttonExplore;

	public int gx;

	public int gy;

	public Zone zone;

	public LayoutGroup layoutButtons;

	public EloMap elomap => EMono.scene.elomapActor.elomap;

	public EloMapActor actor => EMono.scene.elomapActor;

	private void Awake()
	{
		if ((bool)imageFull)
		{
			imageFull.SetAlpha(0f);
		}
	}

	public void SetFaction(Faction f)
	{
		note.Clear();
		note.AddHeader("HeaderNoteFaction", f.Name);
		note.AddTopic("relation", f.relation.GetTextRelation() ?? "");
		note.AddTopic("type", f.TextType ?? "");
		note.Build();
		if (!imageFull)
		{
			return;
		}
		transImage.SetPositionX(pivot.position.x);
		Sprite sprite = Resources.Load<Sprite>("Media/Graphics/Image/Faction/" + f.source.id + "_avatar");
		TweenUtil.KillTween(ref tween);
		if ((bool)sprite)
		{
			if (sprite != imageFull.sprite)
			{
				imageFull.SetAlpha(0f);
			}
			imageFull.sprite = sprite;
			imageFull.SetNativeSize();
			tween = imageIn.Play(imageFull.transform);
		}
		else
		{
			tween = imageOut.Play(imageFull.transform);
		}
	}

	public void SetReligion(Religion f)
	{
		note.Clear();
		UIItem c = note.AddHeader("HeaderFaction", "", f.GetSprite());
		c.Find<UIText>("Name", recursive: true).SetText(f.Name);
		c.Find<UIItem>("Topic1", recursive: true).SetTopic("relationGod", f.relation.ToString() ?? "");
		c.Find<UIItem>("Topic2", recursive: true).SetTopic("type", f.source.GetText("textType") ?? "");
		c.Find<UIItem>("Topic3", recursive: true).SetTopic("avatar", f.source.textAvatar ?? "");
		string lang = "GSSleep";
		if (f.id == "harmony")
		{
			lang = "GSWithdraw";
		}
		if (f.id == "oblivion")
		{
			lang = "GSStray";
		}
		c.Find<UIItem>("Topic4", recursive: true).SetTopic("status", lang);
		note.AddText(f.source.GetDetail()).Hyphenate();
		note.Build();
		if (!imageFull)
		{
			return;
		}
		transImage.SetPositionX(pivot.position.x);
		Sprite sprite = Resources.Load<Sprite>("Media/Graphics/Image/Faction/" + f.source.id + "_avatar");
		TweenUtil.KillTween(ref tween);
		if ((bool)sprite)
		{
			if (sprite != imageFull.sprite)
			{
				imageFull.SetAlpha(0f);
			}
			imageFull.sprite = sprite;
			imageFull.SetNativeSize();
			tween = imageIn.Play(imageFull.transform);
		}
		else
		{
			tween = imageOut.Play(imageFull.transform);
		}
	}

	public void SetZone(Zone _zone)
	{
		zone = _zone;
		FactionBranch branch = zone.branch;
		bool isPCFaction = zone.IsPCFaction;
		RankedZone rankedZone = (from a in EMono.game.spatials.ranks.GetList()
			where a.z == zone
			select a).First();
		gx = zone.x;
		gy = zone.y;
		note.Clear();
		note.AddHeader("HeaderNoteFaction", zone.Name);
		note.AddTopic("mainFaction", zone.mainFaction.Name ?? "");
		note.AddTopic("influence", zone.influence.ToString() ?? "");
		if (isPCFaction)
		{
			note.AddTopic("branchLv", branch.TextLv);
		}
		note.AddTopic("wealth", (isPCFaction ? branch.resources.worth.value : rankedZone.value).ToFormat() ?? "");
		note.AddTopic("ranking", EMono.game.spatials.ranks.GetRankText(zone) ?? "");
		if (isPCFaction)
		{
			note.AddTopic("rank_income", "rank_income2".lang(EMono.game.spatials.ranks.GetIncome(zone).ToFormat()));
		}
		if (isPCFaction)
		{
			note.Space();
			note.AddHeaderTopic("landfeat".lang());
			List<Element> list = _zone.ListLandFeats();
			for (int i = 0; i < list.Count; i++)
			{
				note.AddText(list[i].Name + (((i == 1 && branch.lv < 4) || (i == 2 && branch.lv < 7)) ? "landfeat_locked".lang() : ""));
			}
			note.Space();
			note.AddHeaderTopic("listRoamers".lang());
			int num = 0;
			foreach (Chara value in EMono.game.cards.globalCharas.Values)
			{
				if (value.homeBranch == branch)
				{
					note.AddText(value.Name);
					num++;
					if (num > 5)
					{
						break;
					}
				}
			}
			if (num == 0)
			{
				note.AddText("????????");
			}
			note.Space();
			note.AddHeaderTopic("listPolicies".lang());
			foreach (Element value2 in branch.elements.dict.Values)
			{
				if (value2.source.category == "policy")
				{
					note.AddText(value2.Name);
				}
			}
		}
		note.Build();
	}

	public void Clear()
	{
		note.Clear();
		note.Build();
	}
}
