using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[JsonObject(MemberSerialization.OptOut)]
public class GameIndex : EClass
{
	public Date date;

	public Date real;

	public string id;

	public string color;

	public int difficulty;

	public int days;

	public int deepest;

	public Version version;

	public string zoneName;

	public string factionName;

	public string pcName;

	public string aka;

	public string idPortrait;

	public string idRace;

	public string idJob;

	public bool madeBackup;

	public bool isBackup;

	public bool cloud;

	public Dictionary<string, string> fallbackTypes = new Dictionary<string, string>();

	[JsonIgnore]
	public string path;

	public string Title => factionName ?? "Unknown";

	public string RealDate => real.GetText(Date.TextFormat.LogPlusYear);

	public string GameData => date.GetText(Date.TextFormat.LogPlusYear);

	public string FormTitle => id + ": " + zoneName + "(" + factionName + ") " + RealDate;

	public bool IsCorrupted
	{
		get
		{
			if (zoneName == null)
			{
				return pcName == null;
			}
			return false;
		}
	}

	public GameIndex Create(Game game)
	{
		if (game != null)
		{
			factionName = EClass.Home.name;
			zoneName = game.player.zone.Name;
			pcName = EClass.pc.c_altName;
			aka = EClass.pc.Aka;
			date = game.world.date.Copy();
			difficulty = game.idDifficulty;
			idPortrait = EClass.pc.c_idPortrait;
			idRace = EClass.pc.race.id;
			idJob = EClass.pc.job.id;
			days = EClass.player.stats.days;
			deepest = EClass.player.stats.deepest;
			fallbackTypes = ModUtil.fallbackTypes;
			Color color = EClass.pc.pccData?.GetHairColor(applyMod: true) ?? Color.white;
			this.color = ColorUtility.ToHtmlStringRGBA(color);
		}
		else
		{
			date = new Date();
		}
		real = new Date
		{
			year = DateTime.Now.Year,
			month = DateTime.Now.Month,
			day = DateTime.Now.Day,
			hour = DateTime.Now.Hour,
			min = DateTime.Now.Minute
		};
		version = EClass.core.version;
		return this;
	}
}
