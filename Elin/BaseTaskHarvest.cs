using System;
using UnityEngine;

public class BaseTaskHarvest : TaskDesignation
{
	public enum HarvestType
	{
		Floor,
		Block,
		Obj,
		Thing,
		Disassemble
	}

	public Thing target;

	public Element reqElement;

	public int matHardness;

	public int idEle;

	public int reqLv;

	public int toolLv;

	public int maxProgress;

	public int efficiency;

	public int difficulty;

	public int effectFrame = 1;

	public bool isToolRequired;

	public Thing tool;

	public bool IsTooHard
	{
		get
		{
			if (isToolRequired)
			{
				return reqLv > toolLv;
			}
			return false;
		}
	}

	public bool IsHarvest => pos.cell.CanHarvest();

	public virtual string IdRecipe => "";

	public virtual bool CanReapSeed => pos.cell.CanReapSeed();

	public virtual HarvestType harvestType => HarvestType.Block;

	public virtual bool IsGrowth => false;

	public override bool CanPressRepeat => true;

	public override int LeftHand => -1;

	public override CursorInfo CursorIcon => CursorSystem.Cut;

	public virtual string GetBaseText(string str)
	{
		return base.GetText(str);
	}

	public override string GetTextSmall(Card c)
	{
		return pos.cell.GetObjName();
	}

	public override string GetText(string str = "")
	{
		if (owner == null)
		{
			owner = EClass.pc;
		}
		SetTarget(EClass.pc);
		string text = "tHarvest".lang(GetBaseText(str), (EClass.pc.Tool == null) ? "hand".lang() : owner.Tool.NameSimple, toolLv.ToString() ?? "");
		if (EClass.pc.HasElement(1661) && EClass.player.recipes.CanCeomUpWithRecipe(IdRecipe))
		{
			text = text + " " + "TaskDisassemble_newrecipe".lang();
		}
		text = text + " (" + GetTextDifficulty() + ")";
		if (IsTooHard)
		{
			text = text.TagColor(Color.gray);
		}
		return text;
	}

	public static Thing GetBestTool(Chara c, Point p)
	{
		BaseTaskHarvest task = new BaseTaskHarvest
		{
			pos = p
		};
		task.SetOwner(c);
		Thing tool = null;
		int best = 0;
		c.things.Foreach(delegate(Thing t)
		{
			if (t.trait is TraitTool)
			{
				task.SetTarget(c, t);
				if (task.efficiency > best)
				{
					best = task.efficiency;
					tool = t;
				}
			}
		});
		return tool;
	}

	public void SetTarget(Chara c, Thing _tool = null)
	{
		if (c == null)
		{
			c = EClass.pc;
		}
		if (_tool == null)
		{
			_tool = c.Tool;
		}
		tool = _tool;
		string[] array = null;
		SourceMaterial.Row row = null;
		switch (this.harvestType)
		{
		case HarvestType.Obj:
			array = pos.sourceObj.reqHarvest;
			row = pos.cell.matObj;
			break;
		case HarvestType.Thing:
			array = target.trait.ReqHarvest.Split(',');
			row = target.material;
			break;
		case HarvestType.Disassemble:
			if (target == null)
			{
				return;
			}
			array = new string[2] { "handicraft", "1" };
			row = target.material;
			break;
		case HarvestType.Floor:
			if (pos.cell.HasBridge)
			{
				array = pos.sourceBridge.reqHarvest;
				row = pos.matBridge;
			}
			else
			{
				array = pos.sourceFloor.reqHarvest;
				row = pos.matFloor;
			}
			break;
		case HarvestType.Block:
			if (pos.HasBlock)
			{
				array = pos.sourceBlock.reqHarvest;
				row = pos.matBlock;
				break;
			}
			array = new string[pos.sourceFloor.reqHarvest.Length];
			Array.Copy(pos.sourceFloor.reqHarvest, array, pos.sourceFloor.reqHarvest.Length);
			array[0] = "mining";
			row = pos.matFloor;
			break;
		}
		matHardness = row.hardness;
		if (this.harvestType == HarvestType.Obj)
		{
			matHardness = matHardness * ((pos.growth != null) ? pos.growth.GetHp() : pos.sourceObj.hp) / 100;
		}
		if (row.tag.Contains("hard"))
		{
			matHardness *= 3;
		}
		HarvestType harvestType = this.harvestType;
		if ((uint)harvestType <= 1u && row.id == 0)
		{
			matHardness *= 100;
		}
		idEle = (IsHarvest ? 250 : EClass.sources.elements.alias[array[0]].id);
		reqElement = Element.Create(idEle);
		int num = array[1].ToInt();
		reqLv = matHardness + num;
		isToolRequired = ((!IsHarvest && idEle != 250) ? true : false);
		toolLv = 0;
		if (tool != null)
		{
			int num2 = ((idEle == 220 || idEle == 225) ? GetToolEfficiency(tool, row) : 100);
			toolLv += tool.material.hardness * num2 / 100;
			toolLv += (int)(tool.encLV + tool.blessedState);
		}
		toolLv += c.Evalue(idEle) * 3 / 2;
		if (toolLv < 0)
		{
			toolLv = 1;
		}
		efficiency = (toolLv + 5) * 100 / ((reqLv + 5) * 140 / 100);
		if (efficiency < 50)
		{
			efficiency = 50;
		}
		if (IsTooHard)
		{
			difficulty = 3;
		}
		else if (efficiency > 150)
		{
			difficulty = 0;
		}
		else if (efficiency > 100)
		{
			difficulty = 1;
		}
		else
		{
			difficulty = 2;
		}
		PlantData plantData = EClass._map.TryGetPlant(pos.cell);
		if (this.harvestType == HarvestType.Obj && IsHarvest && plantData != null && plantData.size > 0)
		{
			maxProgress = (int)Mathf.Clamp(Rand.Range(8f, 10f) * (float)(100 + plantData.size * 100 + plantData.size * plantData.size * 30) / (float)efficiency, 2f, 1000f);
		}
		else
		{
			maxProgress = (int)Mathf.Clamp(Rand.Range(8f, 16f) * 100f / (float)efficiency, 2f, 30f);
		}
	}

	public static int GetReqEle(string _raw)
	{
		string[] array = _raw.Split(',');
		return EClass.sources.elements.alias[array[0]].id;
	}

	public int GetToolEfficiency(Thing t, SourceMaterial.Row mat)
	{
		int[] toolEfficiency = GetToolEfficiency(mat);
		_ = new int[2];
		if (!t.HasElement(220))
		{
			toolEfficiency[0] = 0;
		}
		if (!t.HasElement(225))
		{
			toolEfficiency[1] = 0;
		}
		if (t.trait is TraitToolHammer)
		{
			toolEfficiency[0] = 50 + owner.Evalue(261);
		}
		int num = 20;
		int[] array = toolEfficiency;
		foreach (int num2 in array)
		{
			if (num2 > num)
			{
				num = num2;
			}
		}
		return num;
	}

	public override bool CanProgress()
	{
		if (tool != owner.Tool)
		{
			return false;
		}
		return base.CanProgress();
	}

	public int[] GetToolEfficiency(SourceMaterial.Row mat)
	{
		switch (mat.category)
		{
		case "bone":
			return new int[2] { 100, 50 };
		case "ore":
		case "crystal":
		case "rock":
		case "gem":
			return new int[2] { 100, 25 };
		case "organic":
		case "skin":
			return new int[2] { 50, 100 };
		case "grass":
		case "fiber":
		case "wood":
			return new int[2] { 25, 100 };
		case "soil":
		case "water":
			return new int[2] { 100, 100 };
		default:
			return new int[2] { 100, 100 };
		}
	}

	public string GetTextDifficulty()
	{
		return Lang.GetList("skillDiff")[difficulty];
	}
}
