using System.Collections.Generic;
using System.Linq;

public class TaskDump : Task
{
	public static void TryPerform()
	{
		if (EClass.ui.AllowInventoryInteractions)
		{
			if (!EClass._zone.IsPCFaction && !(EClass._zone is Zone_Tent))
			{
				Msg.Say("dump_invalid");
			}
			else if (!EClass.pc.HasNoGoal)
			{
				SE.Beep();
			}
			else
			{
				EClass.pc.SetAIImmediate(new TaskDump());
			}
		}
	}

	public override bool CanManualCancel()
	{
		return true;
	}

	public override IEnumerable<Status> Run()
	{
		List<Thing> containers = new List<Thing>();
		EClass._map.things.ForEach(delegate(Thing t)
		{
			if (IsValidContainer(t))
			{
				containers.Add(t);
			}
		});
		while (containers.Count > 0)
		{
			bool flag = true;
			int num = SortVal(containers.First());
			foreach (Thing item in containers)
			{
				if (SortVal(item) != num)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				containers.Sort((Thing a, Thing b) => EClass.pc.Dist(b) - EClass.pc.Dist(a));
			}
			else
			{
				containers.Sort((Thing a, Thing b) => SortVal(a) - SortVal(b));
			}
			Thing c = containers.LastItem();
			containers.RemoveAt(containers.Count - 1);
			if (!IsValidContainer(c))
			{
				continue;
			}
			EClass.ui.CloseLayers();
			Point pos = c.pos.Copy();
			if (c.IsMultisize)
			{
				int minDist = 999;
				c.ForeachPoint(delegate(Point p, bool isCenter)
				{
					if (p.IsValid && p.IsInBounds)
					{
						PathProgress pathProgress = PathManager.Instance.RequestPathImmediate(EClass.pc.pos, p, EClass.pc, PathManager.MoveType.Default, -1, 1);
						if (pathProgress.HasPath && pathProgress.nodes.Count < minDist)
						{
							minDist = pathProgress.nodes.Count;
							pos = p.Copy();
						}
					}
				});
			}
			yield return DoGoto(pos, 1, ignoreConnection: false, () => Status.Running);
			if (!IsValidContainer(c))
			{
				continue;
			}
			if (!c.ExistsOnMap)
			{
				Msg.Say("dump_noExist", c);
				continue;
			}
			if (EClass.pc.Dist(c) > 1)
			{
				Msg.Say("dump_tooFar", c);
				continue;
			}
			List<Thing> list = ListThingsToPut(c);
			int num2 = 0;
			if (c.trait is TraitShippingChest)
			{
				c = EClass.game.cards.container_shipping;
			}
			foreach (Thing item2 in list)
			{
				if (!c.things.IsFull(item2) && !item2.isDestroyed)
				{
					item2.PlaySoundDrop(spatial: false);
					if (c.parent is Card)
					{
						(c.parent as Card).SetDirtyWeight();
					}
					Msg.Say("dump_item", item2, c);
					c.AddCard(item2);
					num2++;
				}
			}
			if (num2 > 0)
			{
				Msg.Say("dump_dumped", num2.ToString() ?? "", c.Name);
			}
		}
		static int SortVal(Thing t)
		{
			return t.GetWindowSaveData()?.priority ?? 0;
		}
	}

	public override void OnCancelOrSuccess()
	{
		Msg.Say("dump_end");
		SE.Play("auto_dump");
	}

	public bool IsValidContainer(Thing c)
	{
		if ((!c.ExistsOnMap && !c.IsInstalled) || !c.CanSearchContents)
		{
			return false;
		}
		if (c.trait is TraitShippingChest)
		{
			c = EClass.game.cards.container_shipping;
		}
		if (c.GetWindowSaveData() == null || c.GetWindowSaveData().autodump == AutodumpFlag.none)
		{
			return false;
		}
		if (ListThingsToPut(c).Count == 0)
		{
			return false;
		}
		return true;
	}

	public List<Thing> ListThingsToPut(Thing c)
	{
		List<Thing> list = new List<Thing>();
		if (c.trait is TraitShippingChest)
		{
			c = EClass.game.cards.container_shipping;
		}
		Window.SaveData data = c.GetWindowSaveData();
		if (data == null)
		{
			return list;
		}
		switch (data.autodump)
		{
		case AutodumpFlag.distribution:
			EClass.pc.things.Foreach(delegate(Thing t)
			{
				if (!ExcludeDump(t))
				{
					if (data.userFilter)
					{
						switch (data.IsFilterPass(t.GetName(NameStyle.Full, 1)))
						{
						case Window.SaveData.FilterResult.Block:
							return;
						case Window.SaveData.FilterResult.PassWithoutFurtherTest:
							list.Add(t);
							return;
						}
					}
					if (data.advDistribution)
					{
						foreach (int cat in data.cats)
						{
							if (t.category.uid == cat)
							{
								list.Add(t);
								break;
							}
						}
						return;
					}
					ContainerFlag containerFlag = t.category.GetRoot().id.ToEnum<ContainerFlag>();
					if (containerFlag == ContainerFlag.none)
					{
						containerFlag = ContainerFlag.other;
					}
					if (!data.flag.HasFlag(containerFlag))
					{
						list.Add(t);
					}
				}
			});
			break;
		case AutodumpFlag.existing:
			foreach (Thing ct in c.things)
			{
				EClass.pc.things.Foreach(delegate(Thing t)
				{
					if (!ExcludeDump(t))
					{
						if (data.userFilter)
						{
							switch (data.IsFilterPass(t.GetName(NameStyle.Full, 1)))
							{
							case Window.SaveData.FilterResult.Block:
								return;
							case Window.SaveData.FilterResult.PassWithoutFurtherTest:
								list.Add(t);
								return;
							}
						}
						if (t.CanStackTo(ct))
						{
							list.Add(t);
						}
					}
				});
			}
			break;
		case AutodumpFlag.sameCategory:
		{
			if (c.things.Count == 0)
			{
				break;
			}
			HashSet<SourceCategory.Row> cats = new HashSet<SourceCategory.Row>();
			foreach (Thing thing in c.things)
			{
				cats.Add(thing.category);
			}
			EClass.pc.things.Foreach(delegate(Thing t)
			{
				if (!ExcludeDump(t))
				{
					if (data.userFilter)
					{
						switch (data.IsFilterPass(t.GetName(NameStyle.Full, 1)))
						{
						case Window.SaveData.FilterResult.Block:
							return;
						case Window.SaveData.FilterResult.PassWithoutFurtherTest:
							list.Add(t);
							return;
						}
					}
					if (cats.Contains(t.category))
					{
						list.Add(t);
					}
				}
			});
			break;
		}
		}
		return list;
		bool ExcludeDump(Thing t)
		{
			if (t.isEquipped || t.c_isImportant || !t.trait.CanBeDropped || t.IsHotItem || t.trait is TraitToolBelt || t.trait is TraitAbility)
			{
				return true;
			}
			if (t.IsContainer && t.things.Count > 0)
			{
				return true;
			}
			if (data.noRotten && t.IsDecayed)
			{
				return true;
			}
			if (data.onlyRottable && t.trait.Decay == 0)
			{
				return true;
			}
			if (!(t.parent is Card card))
			{
				return false;
			}
			return card.GetWindowSaveData()?.excludeDump ?? false;
		}
	}
}
