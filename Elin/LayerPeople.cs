using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class LayerPeople : ELayer
{
	public enum Mode
	{
		Default,
		Select,
		Double,
		Hire
	}

	public enum ShowMode
	{
		Job,
		Race,
		Work
	}

	public static Chara slaveToBuy;

	public ShowMode showMode;

	public LayoutGroup layoutMenu;

	public Action onConfirm;

	public UIMultiList multi;

	public override bool HeaderIsListOf(int id)
	{
		return true;
	}

	public override void OnInit()
	{
		showMode = ELayer.player.pref.modePoeple;
		if (multi.owners.Count == 0)
		{
			multi.AddOwner(0, new ListPeople
			{
				memberType = FactionMemberType.Default
			});
			multi.AddOwner(0, new ListPeople
			{
				memberType = FactionMemberType.Livestock
			});
			multi.AddOwner(0, new ListPeople
			{
				memberType = FactionMemberType.Guest
			});
			langHint = "h_residents";
		}
		multi.Build(ELayer.player.pref.sortPeople);
		multi.owners[0].menu = new WindowMenu(layoutMenu);
	}

	public LayerPeople SetOnConfirm(Action _onConfirm)
	{
		onConfirm = _onConfirm;
		return this;
	}

	public void Confirm()
	{
		if (onConfirm != null)
		{
			onConfirm();
		}
		Close();
	}

	public override void OnKill()
	{
		ELayer.player.pref.sortPeople = multi.owners[0].list.sortMode;
		ELayer.player.pref.modePoeple = showMode;
		ELayer.scene.screenElin.focusOption = null;
	}

	public override void OnSwitchContent(Window window)
	{
		if (multi.Double)
		{
			multi.owners[window.windowIndex].OnSwitchContent();
		}
		else
		{
			multi.owners[window.idTab].OnSwitchContent();
		}
	}

	public static LayerPeople Create(Mode mode)
	{
		string path = "LayerPeople";
		if (mode == Mode.Double)
		{
			path = "LayerPeople/LayerPeopleDouble";
		}
		return Layer.Create(path) as LayerPeople;
	}

	public static LayerPeople Create<T>(string langHint = null, Chara owner = null) where T : BaseListPeople
	{
		LayerPeople layerPeople = Create(Mode.Select);
		T o = new T
		{
			owner = owner
		};
		layerPeople.multi.AddOwner(0, o);
		layerPeople.langHint = langHint;
		return layerPeople;
	}

	public static LayerPeople CreateReserve()
	{
		LayerPeople layerPeople = Create(Mode.Hire);
		layerPeople.multi.AddOwner(0, new ListPeopleCallReserve
		{
			textHeader = "actCallReserve"
		});
		layerPeople.langHint = "h_reserve";
		ELayer.ui.AddLayer(layerPeople);
		return layerPeople;
	}

	public static LayerPeople CreateBed(TraitBed bed)
	{
		LayerPeople layerPeople = Create(Mode.Double);
		layerPeople.multi.AddOwner(0, new ListPeopleBed
		{
			textHeader = "candidates",
			bed = bed
		});
		layerPeople.multi.AddOwner(1, new ListPeopleBed
		{
			textHeader = "listBedHolder".lang(bed.MaxHolders.ToString() ?? ""),
			bed = bed
		});
		ELayer.ui.AddLayer(layerPeople);
		return layerPeople;
	}

	public static LayerPeople CreateSelectEmbarkMembers(List<Chara> settlers)
	{
		LayerPeople layerPeople = Create(Mode.Double);
		List<Chara> list = new List<Chara>();
		foreach (Chara chara in ELayer.game.lastActiveZone.map.charas)
		{
			if (chara.IsHomeMember())
			{
				list.Add(chara);
			}
		}
		layerPeople.multi.AddOwner(0, new ListPeopleEmbark
		{
			textHeader = "candidates",
			charas = list
		});
		layerPeople.multi.AddOwner(1, new ListPeopleEmbark
		{
			textHeader = "listEmbark",
			charas = settlers
		});
		return layerPeople;
	}

	public static LayerPeople Create(BaseListPeople list)
	{
		LayerPeople layerPeople = Create(Mode.Select);
		layerPeople.multi.AddOwner(0, list);
		return layerPeople;
	}

	public static LayerPeople CreateSelect(string langHeader, string langHint, Action<UIList> onList, Action<Chara> onClick, Func<Chara, string> _onShowSubText = null)
	{
		LayerPeople layerPeople = Create(Mode.Select);
		layerPeople.multi.AddOwner(0, new ListPeopleSelect
		{
			textHeader = langHeader,
			onList = onList,
			onClick = onClick,
			onShowSubText = _onShowSubText
		});
		layerPeople.langHint = langHint;
		ELayer.ui.AddLayer(layerPeople);
		return layerPeople;
	}
}
