using System.Collections.Generic;
using Newtonsoft.Json;

public class CharaMassTransfer : EClass
{
	[JsonProperty]
	public List<CharaOriginalPositionData> list = new List<CharaOriginalPositionData>();

	public void Add(Chara c)
	{
		CharaOriginalPositionData item = new CharaOriginalPositionData
		{
			uid = c.uid,
			uidZone = c.currentZone.uid,
			x = c.pos.x,
			z = c.pos.z,
			noMove = c.noMove
		};
		list.Add(item);
	}

	public void Restore()
	{
		foreach (CharaOriginalPositionData item in list)
		{
			Chara chara = EClass.game.cards.globalCharas.Find(item.uid);
			if (chara != null)
			{
				chara.MoveZone(EClass.game.spatials.Find(item.uidZone), new ZoneTransition
				{
					state = ZoneTransition.EnterState.Exact,
					x = item.x,
					z = item.z
				});
				chara.pos.x = item.x;
				chara.pos.z = item.z;
				chara.noMove = item.noMove;
			}
		}
	}
}
