using System.Collections.Generic;
using Newtonsoft.Json;

public class TaskCraft : Task
{
	[JsonProperty]
	public Thing factory;

	[JsonProperty]
	public Recipe recipe;

	[JsonProperty]
	public int num;

	[JsonProperty]
	public int costSP;

	[JsonProperty]
	public List<Thing> resources = new List<Thing>();

	[JsonProperty]
	public int[] reqs;

	[JsonProperty]
	public bool repeat;

	[JsonProperty]
	public bool floatMode;

	public BlessedState blessed;

	public LayerCraft layer;

	public Point CraftPos
	{
		get
		{
			if (factory == null || !factory.ExistsOnMap)
			{
				return EClass.pc.pos;
			}
			return factory.pos;
		}
	}

	public override int MaxRestart => 99999999;

	public override bool CanManualCancel()
	{
		return true;
	}

	public void ResetReq()
	{
		reqs = new int[recipe.ingredients.Count];
		for (int i = 0; i < recipe.ingredients.Count; i++)
		{
			reqs[i] = recipe.ingredients[i].req;
		}
	}

	public override bool CanProgress()
	{
		return true;
	}

	public override bool _CanPerformTask(Chara chara, int radius)
	{
		return true;
	}

	public override void OnCreateProgress(Progress_Custom p)
	{
		p.maxProgress = 5;
	}

	public override void OnBeforeProgress()
	{
	}

	public override void OnProgress()
	{
		Element orCreateElement = owner.elements.GetOrCreateElement(recipe.source.GetReqSkill());
		owner.PlaySound(recipe.GetMainMaterial().GetSoundCraft(recipe.renderRow));
		owner.elements.ModExp(orCreateElement.id, 20f);
	}

	public override IEnumerable<Status> Run()
	{
		OnBeforeProgress();
		if (!CanProgress())
		{
			yield return Cancel();
		}
		yield return DoProgress();
		ResetReq();
		if (repeat)
		{
			yield return Restart();
		}
		if ((bool)layer)
		{
			layer.OnCompleteCraft();
		}
		if ((bool)LayerCraftFloat.Instance)
		{
			LayerCraftFloat.Instance.OnCompleteCraft();
		}
	}

	public bool IsIngredientsValid(bool destoryResources = false, int numCraft = 1)
	{
		bool flag = true;
		for (int i = 0; i < recipe.ingredients.Count; i++)
		{
			Thing thing = recipe.ingredients[i].thing;
			if (thing == null || thing.isDestroyed || thing.Num < reqs[i] * numCraft)
			{
				flag = false;
				break;
			}
			if (destoryResources)
			{
				resources.Add(thing.Split(reqs[i]));
			}
		}
		if (!flag)
		{
			return false;
		}
		if (destoryResources)
		{
			foreach (Thing resource in resources)
			{
				resource.Destroy();
			}
			resources.Clear();
		}
		return true;
	}

	public override void OnProgressComplete()
	{
		if (owner == null)
		{
			return;
		}
		Element orCreateElement = owner.elements.GetOrCreateElement(recipe.source.GetReqSkill());
		for (int i = 0; i < num; i++)
		{
			ResetReq();
			if (!IsIngredientsValid(destoryResources: true))
			{
				Msg.Say("invalidCraftResource");
				return;
			}
			recipe.Craft(blessed, i == 0);
			owner.elements.ModExp(orCreateElement.id, 200f);
			resources.Clear();
		}
		EClass.Sound.Play("craft");
		Effect.Get("smoke").Play(CraftPos);
		Effect.Get("mine").Play(CraftPos).SetParticleColor(recipe.GetColorMaterial().GetColor())
			.Emit(10 + EClass.rnd(10));
		EClass.pc.stamina.Mod(-costSP);
	}

	public void PutOutResources()
	{
		foreach (Thing resource in resources)
		{
			EClass._zone.AddCard(resource, CraftPos);
		}
		resources.Clear();
	}
}
