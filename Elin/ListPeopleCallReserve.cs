using System;
using System.Linq;

public class ListPeopleCallReserve : BaseListPeople
{
	public bool skipDialog;

	public override void OnInstantiate(Chara a, ItemGeneral b)
	{
		b.SetSubText(a.job.GetName().ToTitleCase(), 280);
		if (a.trait.CanBeBanished)
		{
			b.AddSubButton(EClass.core.refs.icons.trash, delegate
			{
				Action func = delegate
				{
					EClass.Home.RemoveReserve(a);
					a.OnBanish();
					list.List();
					SE.Trash();
				};
				if (skipDialog)
				{
					func();
				}
				else
				{
					Dialog.Choice("dialogDeleteRecruit", delegate(Dialog d)
					{
						d.AddButton("yes".lang(), delegate
						{
							func();
						});
						d.AddButton("yesAndSkip".lang(), delegate
						{
							func();
							skipDialog = true;
						});
						d.AddButton("no".lang());
					});
				}
			});
		}
		AddSubButtonWork(b, a);
	}

	public override void OnClick(Chara c, ItemGeneral i)
	{
		BaseCore.Instance.FreezeScreen(0.1f);
		EClass.Branch.Recruit(c);
		EClass.Sound.Play("good");
		list.List();
	}

	public override void OnList()
	{
		foreach (HireInfo item in EClass.Home.listReserve)
		{
			list.Add(item.chara);
		}
	}

	public HireInfo GetInfo(Chara c)
	{
		return EClass.Home.listReserve.First((HireInfo a) => a.chara == c);
	}
}
