public class TraitParchmentBottle : TraitParchment
{
	public override BookList.Item Item => BottleMessageList.GetItem(base.IdItem);

	public override void OnCreate(int lv)
	{
		owner.SetStr(53, BottleMessageList.GetRandomItem().id);
	}
}
