using Newtonsoft.Json;
using UnityEngine;

public class TraitHouseBoard : TraitBoard
{
	public class Data
	{
		[JsonProperty]
		public int idRoofStyle;

		[JsonProperty]
		public int height;

		[JsonProperty]
		public int idBGM;

		[JsonProperty]
		public int heightFix;

		[JsonProperty]
		public int idRoofTile;

		[JsonProperty]
		public int idBlock = 30;

		[JsonProperty]
		public int idRamp;

		[JsonProperty]
		public int idDeco;

		[JsonProperty]
		public int idDeco2;

		[JsonProperty]
		public int decoFix;

		[JsonProperty]
		public int decoFix2;

		[JsonProperty]
		public int colRoof;

		[JsonProperty]
		public int colBlock;

		[JsonProperty]
		public int colDeco;

		[JsonProperty]
		public int colDeco2;

		[JsonProperty]
		public bool reverse;

		[JsonProperty]
		public bool snow;

		[JsonProperty]
		public bool altRoof;
	}

	public Data data
	{
		get
		{
			return owner.GetObj<Data>(1);
		}
		set
		{
			owner.SetObj(1, value);
		}
	}

	public override bool IsHomeItem => true;

	public override bool CanBeMasked => true;

	public override bool ShouldTryRefreshRoom => true;

	public override bool MaskOnBuild => true;

	public override bool ShowContextOnPick => true;

	public bool CanBeUsed
	{
		get
		{
			if (owner.IsInstalled && owner.pos.cell.room != null)
			{
				return data != null;
			}
			return false;
		}
	}

	public void ApplyData()
	{
		if (owner.IsInstalled && owner.pos.cell.room != null)
		{
			owner.pos.cell.room.lot.RefreshData(data);
		}
	}

	public override void TrySetAct(ActPlan p)
	{
		if (!EClass.debug.enable && !EClass._zone.IsPCFactionOrTent)
		{
			return;
		}
		Room room = owner.pos.cell.room;
		if (room != null && room.lot.board == this)
		{
			p.TrySetAct("actLotBGM", delegate
			{
				EClass.ui.AddLayer<LayerEditPlaylist>().Activate(this);
				return false;
			}, owner);
			p.TrySetAct("actChangeRoof", delegate
			{
				EClass.ui.AddLayer<LayerEditHouse>().SetBoard(this);
				return false;
			}, owner);
		}
	}

	public override void OnChangePlaceState(PlaceState state)
	{
		Debug.Log("Lot OnChangePlaceState " + owner?.ToString() + ": " + owner.placeState.ToString() + "/" + owner.pos);
		EClass._map.rooms.dirtyLots = true;
	}
}
