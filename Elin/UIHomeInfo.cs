using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHomeInfo : EMono
{
	public class ReportData
	{
		public enum ReportType
		{
			Default,
			Warning,
			Critical,
			Good,
			Great,
			Trial
		}

		public ReportType type;

		public string text;

		public Action onClick;

		public Action<UITooltip> onShowTooltip;

		public FontColor GetFontColor()
		{
			return type switch
			{
				ReportType.Warning => FontColor.Warning, 
				ReportType.Critical => FontColor.Bad, 
				ReportType.Good => FontColor.Good, 
				ReportType.Great => FontColor.Good, 
				ReportType.Trial => FontColor.Great, 
				_ => FontColor.Default, 
			};
		}
	}

	public Transform transLayout;

	public UIText textFaction;

	public UIText textDateFound;

	public UIText textGod;

	public UIText textMember;

	public UIText textLand;

	public UIText textHeaderReport;

	public UIText textDev;

	public UIText textElec;

	public UIText textCivility;

	public UIText textName;

	public UIText textRank;

	public UIText textPopu;

	public UIText textFaith;

	public UIText textTemper;

	public UIText textSoil;

	public UIText textContentLv;

	public UIText textPasture;

	public UIButton buttonHappinessResident;

	public UIButton buttonHappinessLivestock;

	public UIButton buttonWorth;

	public UIButton buttonRank;

	public UIButton buttonLv;

	public UIButton buttonRanking;

	public UIList listCurrencies;

	public UIList listResources;

	public UIList listSkill;

	public UIList listReport;

	public Faction faction;

	public FactionBranch branch;

	public Zone zone;

	public Image imageFaith;

	public Image barTemper;

	public List<ReportData> reports = new List<ReportData>();

	public Sprite[] iconReports;

	public int tabResearch => 1;

	public void Refresh()
	{
		EMono._zone.RefreshElectricity();
		reports.Clear();
		faction = EMono.pc.faction;
		branch = EMono.Branch ?? EMono.pc.homeZone.branch;
		zone = EMono.Branch?.owner ?? EMono.pc.homeZone;
		branch.resources.Refresh();
		RefreshInfo();
		RefreshResources();
		RefreshReport();
		transLayout.RebuildLayoutTo(this);
	}

	public void RefreshInfo()
	{
		textHeaderReport.text = "headerHomeReport".lang(EMono.world.date.year.ToString() ?? "", EMono.world.date.month + "/" + EMono.world.date.day);
		textFaction.text = EMono.pc.faction.Name;
		textGod.text = branch.faith.Name;
		textContentLv.text = branch.DangerLV + " " + ("(" + branch.ContentLV + ")").TagSize(12);
		textDateFound.text = Date.GetText(branch.dateFound, Date.TextFormat.YearMonthDay);
		textPasture.text = branch.CountPasture() + " " + ("(-" + branch.GetPastureCost() + "dailyIncome".lang() + ")").TagSize(12);
		bool flag = branch.CountMembers(FactionMemberType.Default) > branch.MaxPopulation;
		textMember.SetText(branch.CountMembers(FactionMemberType.Default) + " / " + branch.MaxPopulation, (!flag) ? FontColor.Default : FontColor.Bad);
		if (flag)
		{
			AddReport("rPopOver", ReportData.ReportType.Warning);
		}
		textDev.text = (EMono._zone.development / 10).ToString() ?? "";
		textLand.text = EMono._map.bounds.Width * EMono._map.bounds.Height / 100 + " ㎢";
		int civility = branch.GetCivility();
		textCivility.text = civility + " " + ("(" + Lang.GetList("civility")[Mathf.Clamp(civility / 20, 0, 4)] + ")").TagSize(12);
		int electricity = EMono._zone.GetElectricity(cost: true);
		int electricity2 = EMono._zone.GetElectricity();
		textElec.SetText("elecLv".lang(electricity2.ToString() ?? "", (electricity.ToString() ?? "").TagColor((electricity <= electricity2) ? FontColor.Good : FontColor.Bad)));
		if (electricity > electricity2)
		{
			AddReport("rElecOver", ReportData.ReportType.Warning);
		}
		int soilCost = zone.GetSoilCost();
		int maxSoil = branch.MaxSoil;
		textSoil.SetText("soilLv".lang(maxSoil.ToString() ?? "", ((maxSoil - soilCost).ToString() ?? "").TagColor((soilCost <= maxSoil) ? FontColor.Good : FontColor.Bad)));
		if (soilCost > maxSoil)
		{
			AddReport("rSoilOver", ReportData.ReportType.Warning);
		}
		textName.SetText(zone.Name);
		textRank.SetText(branch.RankText);
		textPopu.SetText(faction.name);
		textFaith.SetText(branch.faith.Name + Environment.NewLine + (" ♥" + branch.RankText).TagSize(13));
		imageFaith.sprite = branch.faith.GetSprite();
		textTemper.SetText(branch.faith.GetTextTemper(branch.temper));
		barTemper.fillAmount = 0.5f + (float)branch.temper * 0.01f * 0.5f;
		barTemper.color = ((branch.temper > -25) ? SkinManager.CurrentColors.textGood : SkinManager.CurrentColors.textBad);
		if ((bool)buttonWorth)
		{
			buttonWorth.mainText.SetText(branch.Worth.ToFormat());
			buttonWorth.SetTooltip(delegate(UITooltip t)
			{
				branch.resources.worth.WriteNote(t.note);
			});
		}
		buttonLv.mainText.SetText(branch.lv.ToString() ?? "");
		buttonLv.SetTooltip(delegate(UITooltip t)
		{
			UINote i = t.note;
			i.Clear();
			i.AddHeader("branchLv".lang());
			i.AddTopic("TopicLeft", "vCurrent".lang(), EMono.Branch.TextLv);
			i.Space();
			Write(branch.lv);
			if (branch.lv < branch.MaxLv)
			{
				i.Space();
				i.AddHeaderTopic("nextLevel");
				Write(branch.lv + 1);
			}
			t.note.Build();
			void Write(int lv)
			{
				string[] array = branch.GetHearthHint(lv).SplitNewline();
				for (int j = 0; j < array.Length; j++)
				{
					string text = array[j].Replace(Environment.NewLine, "");
					if (!text.IsEmpty())
					{
						i.AddText(text);
					}
				}
			}
		});
		buttonRanking.mainText.SetText(EMono.game.spatials.ranks.GetRankText(EMono._zone));
		buttonRanking.SetTooltip(delegate(UITooltip t)
		{
			UINote note2 = t.note;
			note2.Clear();
			note2.AddHeader("homeRanking".lang());
			note2.AddTopic("TopicLeft", "vCurrent".lang(), EMono.game.spatials.ranks.GetRankText(EMono._zone));
			note2.Space();
			note2.AddHeader("HeaderTopic", "rank_income".lang());
			note2.Space(1);
			note2.AddText("rank_income2".lang(EMono.game.spatials.ranks.GetIncome(EMono._zone).ToFormat()));
			t.note.Build();
		});
		buttonRank.mainText.SetText(branch.rank.ToString() ?? "");
		buttonRank.SetTooltip(delegate(UITooltip t)
		{
			UINote note = t.note;
			note.Clear();
			note.AddHeader("factionRank2".lang());
			note.AddTopic("TopicLeft", "vCurrent".lang(), branch.rank.ToString() ?? "");
			t.note.Build();
		});
		buttonHappinessResident.mainText.text = branch.happiness.residents.GetText();
		buttonHappinessLivestock.mainText.text = branch.happiness.livestocks.GetText();
		buttonHappinessResident.subText.text = branch.CountMembers(FactionMemberType.Default).ToString() ?? "";
		buttonHappinessLivestock.subText.text = branch.CountMembers(FactionMemberType.Livestock).ToString() ?? "";
		buttonHappinessResident.SetTooltip(delegate(UITooltip t)
		{
			branch.happiness.residents.WriteNote(t.note);
		});
		buttonHappinessLivestock.SetTooltip(delegate(UITooltip t)
		{
			branch.happiness.livestocks.WriteNote(t.note);
		});
	}

	public string GetTextHappiness(List<Chara> list)
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		foreach (Chara item in list)
		{
			int happiness = item.GetHappiness();
			if (happiness >= 80)
			{
				num2++;
			}
			else if (happiness >= 30)
			{
				num3++;
			}
			else
			{
				num4++;
			}
			num += happiness;
		}
		int avg = ((list.Count != 0) ? (num / list.Count) : 0);
		string s = "(" + num2.ToString().TagColor(FontColor.Good) + "/" + num3.ToString().TagColor(FontColor.Default) + "/" + num4.ToString().TagColor(FontColor.Bad) + ")";
		return (((list.Count == 0) ? " - " : avg.ToString()) + "%").TagColorGoodBad(() => list.Count == 0 || avg >= 50) + " " + s.TagSize(13);
	}

	public void RefreshResources()
	{
		listCurrencies.Clear();
		listCurrencies.callbacks = new UIList.Callback<BaseHomeResource, ItemHomeResource>
		{
			onInstantiate = delegate(BaseHomeResource a, ItemHomeResource b)
			{
				b.SetResource(a);
			},
			onList = delegate
			{
				foreach (BaseHomeResource item in branch.resources.list)
				{
					if (item.IsCurrency && item.IsAvailable)
					{
						listCurrencies.Add(item);
					}
				}
			}
		};
		listCurrencies.List();
		listResources.Clear();
		listResources.callbacks = new UIList.Callback<BaseHomeResource, ItemHomeResource>
		{
			onInstantiate = delegate(BaseHomeResource a, ItemHomeResource b)
			{
				b.SetResource(a);
			},
			onList = delegate
			{
				foreach (BaseHomeResource item2 in branch.resources.list)
				{
					if (item2.IsRate && item2.IsAvailable)
					{
						listResources.Add(item2);
					}
				}
			}
		};
		listResources.List();
		listSkill.Clear();
		listSkill.callbacks = new UIList.Callback<BaseHomeResource, ItemHomeResource>
		{
			onInstantiate = delegate(BaseHomeResource a, ItemHomeResource b)
			{
				b.SetResource(a);
			},
			onList = delegate
			{
				foreach (BaseHomeResource item3 in branch.resources.list)
				{
					if (item3.IsSkill)
					{
						listSkill.Add(item3);
					}
				}
			}
		};
		listSkill.List();
	}

	public void AddReport(string text, ReportData.ReportType type = ReportData.ReportType.Default, Action onClick = null, Action<UITooltip> onShowTooltip = null)
	{
		ReportData item = new ReportData
		{
			text = text.lang(),
			type = type,
			onClick = onClick,
			onShowTooltip = onShowTooltip
		};
		reports.Add(item);
	}

	public void RefreshReport()
	{
		if (EMono.Branch.meetings.list.Count > 0)
		{
			AddReport("rMeeting".lang(EMono.Branch.meetings.list.Count.ToString() ?? ""));
		}
		foreach (ZoneEvent item in branch.owner.events.list)
		{
			if (item.HasReport)
			{
				AddReport(item.GetText(), ReportData.ReportType.Trial);
			}
		}
		foreach (ResearchPlan newPlan in branch.researches.newPlans)
		{
			AddReport("rNewPlan".lang(newPlan.Name ?? ""), ReportData.ReportType.Good, delegate
			{
			});
		}
		foreach (ResearchPlan plan in branch.researches.plans)
		{
			if (plan.exp == plan.MaxExp)
			{
				AddReport("vResearched".lang(plan.Name), ReportData.ReportType.Great, delegate
				{
				});
			}
		}
		foreach (Chara member in branch.members)
		{
			if (member.isDead || member.IsPCParty || member.memberType != 0 || member.noMove)
			{
				continue;
			}
			bool flag = false;
			if (EMono.game.activeZone == branch.owner)
			{
				foreach (Hobby item2 in member.ListWorks())
				{
					if (item2.GetEfficiency(member) > 0)
					{
						flag = true;
						break;
					}
				}
			}
			else if (member.GetWorkSummary().work != null)
			{
				flag = true;
			}
			if (!flag)
			{
				AddReport("rNoWork".lang(member.Name).ToTitleCase(), ReportData.ReportType.Warning);
			}
		}
		if (EMono.player.taxBills > 0)
		{
			AddReport("rBill".lang(EMono.player.taxBills.ToString() ?? ""), ReportData.ReportType.Warning);
		}
		if (reports.Count == 0)
		{
			AddReport("rNoReport", ReportData.ReportType.Good);
		}
		AddReport("rTax".lang((30 - EMono.world.date.day + 1).ToString() ?? "", faction.GetTotalTax(evasion: false).ToFormat()), ReportData.ReportType.Default, null, delegate(UITooltip t)
		{
			t.note.Clear();
			faction.SetTaxTooltip(t.note);
			t.note.Build();
		});
		if (EMono.debug.showExtra)
		{
			AddReport("efficiency:" + branch.efficiency);
		}
		reports.Sort((ReportData a, ReportData b) => b.type - a.type);
		listReport.Clear();
		listReport.callbacks = new UIList.Callback<ReportData, ItemGeneral>
		{
			onClick = delegate(ReportData a, ItemGeneral b)
			{
				if (a.onClick != null)
				{
					a.onClick();
				}
			},
			onInstantiate = delegate(ReportData a, ItemGeneral b)
			{
				b.SetMainText(a.text, iconReports[(int)a.type]);
				b.button1.mainText.SetColor(a.GetFontColor());
				if (a.onClick != null)
				{
					b.AddSubButton(EMono.core.refs.icons.go, a.onClick);
				}
				b.Build();
				if (a.onShowTooltip != null)
				{
					b.button1.SetTooltip(a.onShowTooltip);
				}
			},
			onList = delegate
			{
				foreach (ReportData report in reports)
				{
					listReport.Add(report);
				}
			},
			onRefresh = null
		};
		listReport.List();
	}
}
