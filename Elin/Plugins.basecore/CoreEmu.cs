public class CoreEmu : BaseCore
{
	public BaseModManager mods = new BaseModManager();

	public new static CoreEmu Instance;

	protected override void Awake()
	{
		base.Awake();
		Instance = this;
		BaseCore.Instance = this;
		CorePath.Init();
	}
}
