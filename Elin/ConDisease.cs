using System.Linq;
using Newtonsoft.Json;

public class ConDisease : BadCondition
{
	[JsonProperty]
	private ElementContainer ec = new ElementContainer();

	public override bool PreventRegen
	{
		get
		{
			if (GetPhase() <= 0)
			{
				return EClass.rnd(2) == 0;
			}
			return true;
		}
	}

	public override void SetOwner(Chara _owner, bool onDeserialize = false)
	{
		base.SetOwner(_owner);
		ec.SetParent(owner);
	}

	public override void Tick()
	{
		if (EClass.rnd(20) == 0)
		{
			Mod((EClass.rnd(2) == 0) ? 1 : (-1));
		}
		if (EClass.rnd(EClass.debug.enable ? 1 : 200) == 0)
		{
			SourceElement.Row row = EClass.sources.elements.rows.Where((SourceElement.Row e) => e.tag.Contains("primary")).RandomItem();
			ec.ModBase(row.id, -1);
		}
	}

	public override void OnRemoved()
	{
		ec.SetParent();
	}
}
