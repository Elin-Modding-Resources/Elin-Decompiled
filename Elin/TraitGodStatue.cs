using UnityEngine;

public class TraitGodStatue : TraitPowerStatue
{
	public Religion Religion => EClass.game.religions.dictAll[GetParam(1)];

	public override bool IsImplemented()
	{
		return true;
	}

	public override void OnCreate(int lv)
	{
		base.OnCreate(lv);
		OnChangeMaterial();
	}

	public void OnChangeMaterial()
	{
		owner.isOn = owner.material.alias == "gold";
		owner.rarity = (owner.isOn ? Rarity.Artifact : Rarity.Normal);
		if (Religion.id == "machine")
		{
			owner.AddCard(GetManiGene());
		}
		if (owner.placeState == PlaceState.installed)
		{
			owner.renderer.RefreshExtra();
		}
	}

	public Thing GetManiGene()
	{
		owner.things.DestroyAll();
		Debug.Log("Mani:" + owner.c_seed);
		Rand.SetSeed(owner.c_seed);
		CardRow r = SpawnList.Get("chara").Select(100);
		Rand.SetSeed(owner.c_seed);
		Thing thing = DNA.GenerateGene(r, DNA.Type.Superior, owner.LV, owner.c_seed);
		thing.c_DNA.cost = thing.c_DNA.cost / 2;
		thing.MakeRefFrom("mani");
		Rand.SetSeed();
		owner.c_seed++;
		return thing;
	}

	public override void _OnUse(Chara c)
	{
		Religion.Talk("shrine");
		switch (Religion.id)
		{
		case "harvest":
		{
			Thing t2 = ThingGen.Create("book_kumiromi");
			EClass.pc.Pick(t2);
			break;
		}
		case "machine":
		{
			Thing t = owner.things.Find("gene") ?? GetManiGene();
			EClass.pc.Pick(t);
			break;
		}
		case "healing":
			Msg.Say("jure_hug");
			EClass.player.ModKeyItem("jure_feather");
			Msg.Say("jure_hug2");
			break;
		case "luck":
			EClass.player.ModKeyItem((!EClass.player.wellWished && EClass.player.CountKeyItem("well_wish") == 0) ? "well_wish" : "well_enhance");
			break;
		case "wind":
			EClass.pc.Pick(ThingGen.Create("blood_angel"));
			break;
		case "earth":
		case "element":
		{
			Rand.SetSeed(owner.c_seed);
			SourceMaterial.Row randomMaterial = MATERIAL.GetRandomMaterial(((owner.LV < 200) ? (owner.LV / 2) : (owner.LV % 50 * 2)) + 10, (Religion.id == "earth") ? "metal" : "leather", tryLevelMatTier: true);
			Thing thing = ThingGen.Create("mathammer");
			thing.ChangeMaterial(randomMaterial);
			thing.noSell = true;
			Rand.SetSeed();
			EClass.pc.Pick(thing);
			break;
		}
		}
	}
}
